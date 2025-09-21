using System;
using System.Collections.Generic;
using System.Text;

namespace iCos5.BACnet.Zenon
{
  public class BACnetTagNode
  {
    public BACnetTagClass TagClass { get; set; }
    public BACnetTagType TagType { get; set; }
    public object TagValue { get; set; }

    public BACnetTagNode() 
    {
      TagClass = BACnetTagClass.ContextConstucted;
      TagType = BACnetTagType.Null;
      TagValue = "";
    }

    public BACnetTagNode(BACnetTagClass tagClass, BACnetTagType tagType, object tagValue)
    {
      TagClass = tagClass;
      TagType = tagType;
      TagValue = tagValue;
    }

    public BACnetTagNode(string code, BACnetCodeType codeType = BACnetCodeType.Full, int contextTagNumber = 4)
    {
      string strCode = "";

      switch (codeType)
      {
        case BACnetCodeType.Full:
        default:
          strCode = code.Trim();
          break;
        case BACnetCodeType.WithoutBraces:
          strCode = "{" + code.Trim() + "}";
          break;
        case BACnetCodeType.OnlyValue:
          strCode = "{" + $"<{contextTagNumber}> {code.Trim()}" + "}";
          break;
      }

      BACnetTagParserState state = BACnetTagParserState.Ready;
      BACnetTagParserState subState = BACnetTagParserState.Ready;
      StringBuilder strBuilder = new StringBuilder();
      int depth = 0;

      foreach (char chr in strCode)
      {
        switch (state)
        {
          case BACnetTagParserState.Ready:
            if (chr == '{')
            {
              state = BACnetTagParserState.OpenTag;
            }

            break;
          case BACnetTagParserState.OpenTag:
            switch (chr)
            {
              case '<':
                TagClass = BACnetTagClass.ContextConstucted;
                state = BACnetTagParserState.TagType;
                break;
              case '(':
                TagClass = BACnetTagClass.ContextSpecific;
                state = BACnetTagParserState.TagType;
                break;
              case '[':
                TagClass = BACnetTagClass.Application;
                state = BACnetTagParserState.TagType;
                break;
            }

            break;
          case BACnetTagParserState.TagType:
            switch (chr)
            {
              case '>':
                if (TagClass == BACnetTagClass.ContextConstucted)
                {
                  TagType = (BACnetTagType)Convert.ToInt32(strBuilder.ToString());
                  state = BACnetTagParserState.SkipSpace;
                }

                break;
              case ')':
                if (TagClass == BACnetTagClass.ContextSpecific)
                {
                  TagType = (BACnetTagType)Convert.ToInt32(strBuilder.ToString());
                  state = BACnetTagParserState.SkipSpace;
                }

                break;
              case ']':
                if (TagClass == BACnetTagClass.Application)
                {
                  TagType = (BACnetTagType)Convert.ToInt32(strBuilder.ToString());
                  state = BACnetTagParserState.SkipSpace;
                }

                break;
              default:
                strBuilder.Append(chr);
                break;
            }

            break;
          case BACnetTagParserState.SkipSpace:
            if (chr != ' ')
            {
              strBuilder = new StringBuilder();
              strBuilder.Append(chr);
              state = BACnetTagParserState.TagValue;

              if (TagClass == BACnetTagClass.ContextConstucted)
              {
                depth--;
                TagValue = new List<BACnetTagNode>();
                subState = BACnetTagParserState.OpenTag;
              }
            }

            break;
          case BACnetTagParserState.TagValue:
            if (TagClass == BACnetTagClass.ContextConstucted)
            {
              switch (subState)
              {
                case BACnetTagParserState.Ready:
                  if (chr == '{')
                  {
                    depth--;
                    strBuilder = new StringBuilder();
                    strBuilder.Append(chr);
                    subState = BACnetTagParserState.OpenTag;
                  }

                  break;
                case BACnetTagParserState.OpenTag:
                  strBuilder.Append(chr);

                  if (chr == '{')
                  {
                    depth--;
                  }
                  else if (chr == '}')
                  {
                    depth++;

                    if (depth == 0)
                    {
                      ((List<BACnetTagNode>)TagValue).Add(new BACnetTagNode(strBuilder.ToString()));
                      subState = BACnetTagParserState.SkipComma;
                    }
                  }

                  break;
                case BACnetTagParserState.SkipComma:
                  if (chr == ',')
                  {
                    subState = BACnetTagParserState.Ready;
                  }
                    
                  break;
              }
            }
            else
            {
              if (chr == '}')
              {
                if (TagClass == BACnetTagClass.Application)
                {
                  genApplicationTag(strBuilder.ToString());
                }
                else
                {
                  TagValue= strBuilder.ToString();
                }
              }
              else
              {
                strBuilder.Append(chr);
              }
            }

            break;
        }
      }
    }

    public string GetCode()
    {
      string strTagType = "";
      string strTagValue = "";

      switch (TagClass)
      {
        case BACnetTagClass.ContextConstucted:
        default:
          strTagType = $"<{(int)TagType}>";

          List<string> strValues = new List<string>();

          foreach (BACnetTagNode bacNode in (List<BACnetTagNode>)TagValue)
          {
            strValues.Add(bacNode.GetCode());
          }

          strTagValue = string.Join(",", strValues);
          break;
        case BACnetTagClass.ContextSpecific:
          strTagType = $"({(int)TagType})";
          strTagValue = TagValue.ToString();
          break;
        case BACnetTagClass.Application:
          strTagType = $"[{(int)TagType}]";
          strTagValue = getApplicationValueCode();
          break;
      }

      return "{" + $"{strTagType} {strTagValue}" + "}";
    }

    public string GetCodeOnlyValues()
    {
      string strTagValue = "";

      switch (TagClass)
      {
        case BACnetTagClass.ContextConstucted:
        default:
          List<string> strValues = new List<string>();

          foreach (BACnetTagNode bacNode in (List<BACnetTagNode>)TagValue)
          {
            strValues.Add(bacNode.GetCode());
          }

          strTagValue = string.Join(",", strValues);
          break;
        case BACnetTagClass.ContextSpecific:
          strTagValue = TagValue.ToString();
          break;
        case BACnetTagClass.Application:
          strTagValue = getApplicationValueCode();
          break;
      }

      return strTagValue;
    }

    private void genApplicationTag(string valueCode)
    {
      switch (TagType)
      {
        case BACnetTagType.Null:
        default:
          TagValue = null;
          break;
        case BACnetTagType.Bool:
          TagValue = Convert.ToBoolean(valueCode.Trim());
          break;
        case BACnetTagType.UInt:
          TagValue = Convert.ToUInt32(valueCode.Trim());
          break;
        case BACnetTagType.Int:
        case BACnetTagType.Enum:
          TagValue = Convert.ToInt32(valueCode.Trim());
          break;
        case BACnetTagType.Real:
          TagValue = Convert.ToSingle(valueCode.Trim());
          break;
        case BACnetTagType.Double:
          TagValue = Convert.ToDouble(valueCode.Trim());
          break;
        case BACnetTagType.OctetString:
        case BACnetTagType.CharString:
        case BACnetTagType.BitString:
        case BACnetTagType.ObjectID:
          TagValue = valueCode.Trim();
          break;
        case BACnetTagType.Date:
          string[] strDate = valueCode.Trim().Split('.');
          TagValue = new DateTime(Convert.ToInt32(strDate[0]) + 1900, Convert.ToInt32(strDate[1]), Convert.ToInt32(strDate[2]));
          break;
        case BACnetTagType.Time:
          string[] strTime = valueCode.Trim().Split(':');
          TagValue = new DateTime(1900, 1, 1, Convert.ToInt32(strTime[0]), Convert.ToInt32(strTime[1]), Convert.ToInt32(strTime[2]), Convert.ToInt32(strTime[3]));
          break;
      }
    }

    private string getApplicationValueCode()
    {
      switch (TagType)
      {
        case BACnetTagType.Null:
        default:
          return "";
        case BACnetTagType.Bool:
          return Convert.ToBoolean(TagValue).ToString();
        case BACnetTagType.UInt:
          return Convert.ToUInt32(TagValue).ToString();
        case BACnetTagType.Int:
        case BACnetTagType.Enum:
          return Convert.ToInt32(TagValue).ToString();
        case BACnetTagType.Real:
          return Convert.ToSingle(TagValue).ToString();
        case BACnetTagType.Double:
          return Convert.ToDouble(TagValue).ToString();
        case BACnetTagType.OctetString:
        case BACnetTagType.CharString:
        case BACnetTagType.BitString:
        case BACnetTagType.ObjectID:
          return (string)TagValue;
        case BACnetTagType.Date:
          DateTime date = (DateTime)TagValue;
          int dayOfWeek = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
          return $"{date.Year - 1900}.{date.Month}.{date.Day}.{dayOfWeek}";
        case BACnetTagType.Time:
          return ((DateTime)TagValue).ToString("HH:mm:ss:fff");
      }
    }
  }
}
