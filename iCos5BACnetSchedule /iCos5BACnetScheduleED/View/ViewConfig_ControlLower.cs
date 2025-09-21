using iCos5.BACnet.Schedule;
using iCos5.BACnet.Zenon;
using Scada.AddIn.Contracts.Function;
using Scada.AddIn.Contracts.Variable;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iCos5BACnetScheduleED.View
{
  public partial class ViewConfig : UserControl
  {
    public bool IsProgress
    {
      get { return (bool)GetValue(IsProgressProperty); }
      set { SetValue(IsProgressProperty, value); }
    }

    public static readonly DependencyProperty IsProgressProperty =
      DependencyProperty.Register("IsProgress", typeof(bool), typeof(ViewConfig),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private void setIsProgress(string message)
    {
      setMessage(message);
      IsProgress = true;
    }

    private void resetIsProgress()
    {
      IsProgress = false;
    }

    private void setMessage(string message)
    {
      _parentWinMain?.SetProgressDialogMessage(message);
    }

    private async void BACnetDeviceListUpdate_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        setIsProgress("태그 목록에서 BACnet 스케줄 태크를 읽는 중입니다.");
        ((ScheduleConfig)DataContext).BACnetDriverDevices = await Task.Run(() => getDriverDeviceNodes());
        ScheduleDriverTreeview.ItemsSource = ((ScheduleConfig)DataContext).BACnetDriverDevices;
        resetIsProgress();
      }
      catch (Exception ex)
      {
        resetIsProgress();
        MessageBox.Show($"[BACnetDeviceListUpdate_Click]{ex}", ScheduleConfig.Constants.SolutionName);
      }
    }

    private ObservableCollection<BACnetScheduleDriver> getDriverDeviceNodes()
    {
      ObservableCollection<BACnetScheduleDriver> driverList = new ObservableCollection<BACnetScheduleDriver>();

      foreach (IDriver driver in _zenonProject.DriverCollection)
      {
        if (driver.Name.Contains("BACNETNG"))
        {
          driver.InitializeConfiguration();

          BACnetScheduleDriver drvNode = new BACnetScheduleDriver()
          {
            DriverIdentification  = driver.Identification,
            ConnectionName        = driver.GetDynamicProperty("DrvConfig.Options.Connection").ToString(),
            DeviceServer1Name     = driver.GetDynamicProperty("DrvConfig.Options.DeviceNameServer1").ToString(),
            DeviceServer1ID       = Convert.ToInt32(driver.GetDynamicProperty($"DrvConfig.Options.DeviceIDServer1")),
            DeviceServer2Name     = driver.GetDynamicProperty("DrvConfig.Options.DeviceNameServer2").ToString(),
            DeviceServer2ID       = Convert.ToInt32(driver.GetDynamicProperty($"DrvConfig.Options.DeviceIDServer2"))
          };

          uint numberOfDevice = Convert.ToUInt32(driver.GetDynamicProperty("DrvConfig.Connections"));

          for (int i = 0; i < numberOfDevice; i++)
          {
            BACnetScheduleDevice devNode = new BACnetScheduleDevice()
            {
              DeviceName    = driver.GetDynamicProperty($"DrvConfig.Connections[{i}].DeviceName").ToString(),
              DeviceAddress = driver.GetDynamicProperty($"DrvConfig.Connections[{i}].DeviceAddress").ToString(),
              ProcessId     = Convert.ToUInt32(driver.GetDynamicProperty($"DrvConfig.Connections[{i}].ProcessId")),
            };

            drvNode.BACnetDevices.Add(devNode);
          }

          driver.EndConfiguration(false);
          driverList.Add(drvNode);
        }
      }

      foreach (IVariable variable in _zenonProject.VariableCollection)
      {
        IDriver driver = variable.Driver;

        if (driver.Name.Contains("BACNETNG"))
        {
          uint netAddr = Convert.ToUInt32(variable.GetDynamicProperty("NetAddr"));
          ushort objectType = Convert.ToUInt16(variable.GetDynamicProperty("BACnetObjectType"));
          uint instanceID = Convert.ToUInt32(variable.GetDynamicProperty("BACnetObjectInstanceID"));
          int propertyID = Convert.ToInt32(variable.GetDynamicProperty("BACnetPropertyID"));

          if ((BACnetObjectTypes)objectType == BACnetObjectTypes.Schedule)
          {
            if ((BACnetPropertyID)propertyID == BACnetPropertyID.WeeklySchedule)
            {
              foreach (BACnetScheduleDriver scheduleDriver in driverList)
              {
                if (driver.Identification.Equals(scheduleDriver.DriverIdentification))
                {
                  BACnetScheduleDevice scheduleDevice = scheduleDriver.GetBACnetDevice(netAddr);
                  ScheduleVariable scheduleVariable = scheduleDevice.GetScheduleVariable(instanceID, variable.Name.Replace(".weekly-schedule", ""));
                  scheduleVariable.Weekly = variable.Name;
                }
              }
            }
            else if ((BACnetPropertyID)propertyID == BACnetPropertyID.ExeptionSchedule)
            {
              foreach (BACnetScheduleDriver scheduleDriver in driverList)
              {
                if (driver.Identification.Equals(scheduleDriver.DriverIdentification))
                {
                  BACnetScheduleDevice scheduleDevice = scheduleDriver.GetBACnetDevice(netAddr);
                  ScheduleVariable scheduleVariable = scheduleDevice.GetScheduleVariable(instanceID, variable.Name.Replace(".exception-schedule", ""));
                  scheduleVariable.Exception = variable.Name;
                }
              }
            }
          }
        }
      }

      foreach (BACnetScheduleDriver scheduleDriver in driverList)
      {
        scheduleDriver.Sort();
      }

      return driverList;
    }

    private async void BACnetDeviceListApply_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        ScheduleConfig config = (ScheduleConfig)DataContext;
        setIsProgress("BACnet 스케줄 자동 설정하는 중입니다.");

        await Task.Run(() =>
        {
          foreach (BACnetScheduleDriver scheduleDriver in config.BACnetDriverDevices)
          {
            foreach (BACnetScheduleDevice scheduleDevice in scheduleDriver.BACnetDevices)
            {
              foreach (ScheduleVariable scheduleVariable in scheduleDevice.ScheduleVariables)
              {
                try
                {
                  IFunction function = _zenonProject.FunctionCollection[scheduleVariable.FunctionName];

                  if (function == null)
                  {
                    function = _zenonProject.FunctionCollection.Create(scheduleVariable.FunctionName, FunctionType.WriteSetValue);
                  }
                  else if (function.Type != FunctionType.WriteSetValue)
                  {
                    _zenonProject.FunctionCollection.Delete(scheduleVariable.FunctionName);
                    function = _zenonProject.FunctionCollection.Create(scheduleVariable.FunctionName, FunctionType.WriteSetValue);
                  }

                  function.SetDynamicProperty("SetValue.Variable", ScheduleConfig.Constants.DialogueParameterVariable);
                  function.SetDynamicProperty("SetValue.StrValue", $"{scheduleDriver.DriverIdentification}/{scheduleDevice.DeviceName}/{scheduleVariable.TargetObjectName}/{scheduleVariable.InstanceID}");
                  function.SetDynamicProperty("SetValue.IsDirect", true);
                }
                catch (Exception ex)
                {
                  MessageBox.Show($"[{scheduleVariable.FunctionName}]{ex}", ScheduleConfig.Constants.SolutionName);
                }
              }
            }
          }
        });

        resetIsProgress();
      }
      catch (Exception ex)
      {
        resetIsProgress();
        MessageBox.Show($"[BACnetDeviceListApply_Click]{ex}", ScheduleConfig.Constants.SolutionName);
      }
    }

    private void ScheduleDriverTreeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      DeviceScheduleDetailControl.DataContext = e.NewValue;
    }
  }
}
