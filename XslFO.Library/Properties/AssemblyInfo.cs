using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("XslFO.Library")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tech-Experts, Inc.")]
[assembly: AssemblyProduct("XslFO.Library")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("aacd68c1-efb2-4164-ada2-8250d5846417")]

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
[assembly: AssemblyVersion("1.0.*")]
//NOTE FROM: http://stackoverflow.com/questions/511789/how-do-i-persuade-a-vs2005-msi-to-upgrade/513169#513169
//  An easier way to manage this is to REMOVE the AssemblyFileVersion from all assemblies, including the main executable and all the managed DLLs.
//  In each of your AssemblyInfo.cs files, I recommend doing something like this if you don't care about the version numbers, but want to have some traceability.
//  Everything still compiles fine, and if you don't have the AssemblyFileVersion defined, then the installer assumes that everything is different every time (which is probably fine if you are installing all of the DLLs next to the main EXE).
//  I spent a long time figuring this out, especially if I don't want to have to increment anything manually!
//  don't need this:
//[assembly: AssemblyFileVersion("1.0.0.0")]
