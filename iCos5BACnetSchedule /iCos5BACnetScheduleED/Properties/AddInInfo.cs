using Mono.Addins;

// Declares that this assembly is an add-in
[assembly: Addin("HDCLabsBACnetScheduleED", "1.1")]

// Declares that this add-in depends on the scada v1.0 add-in root
[assembly: AddinDependency("::scada", "1.0")]

[assembly: AddinName("HDCLabsBACnetScheduleED")]
[assembly: AddinDescription("This wizard is for BACnet Schedule viewer")]