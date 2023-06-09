﻿using System.Reflection;
using System.Runtime.InteropServices;
using AccuracyDisplay;
using MelonLoader;

[assembly: MelonInfo(typeof(ModMain), MelonBuildInfo.Name, MelonBuildInfo.Version, MelonBuildInfo.Author, MelonBuildInfo.DownloadLink)]
[assembly: MelonGame("PeroPeroGames", "MuseDash")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(MelonBuildInfo.Name)]
[assembly: AssemblyDescription(MelonBuildInfo.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct(MelonBuildInfo.Name)]
[assembly: AssemblyCopyright("Copyright ©  2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("D6F44FE5-0FC3-440D-9977-44210EF543D2")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(MelonBuildInfo.Version)]
[assembly: AssemblyFileVersion(MelonBuildInfo.Version)]