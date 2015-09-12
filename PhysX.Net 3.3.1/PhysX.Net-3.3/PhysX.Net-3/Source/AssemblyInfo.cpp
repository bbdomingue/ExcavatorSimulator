#include "stdafx.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly:AssemblyTitleAttribute("PhysX.Net for NVIDIA PhysX 3.3.1")];
[assembly:AssemblyDescriptionAttribute(".NET wrapper for NVIDIA PhysX 3.3.1")];
[assembly:AssemblyConfigurationAttribute("")];
[assembly:AssemblyCompanyAttribute("StillDesign")];
[assembly:AssemblyProductAttribute("PhysX.Net")];
[assembly:AssemblyCopyrightAttribute("Copyright (c) StillDesign 2014")];
[assembly:AssemblyTrademarkAttribute("")];
[assembly:AssemblyCultureAttribute("")];

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the value or you can default the Revision and Build Numbers
// by using the '*' as shown below:

// Versioning - http://semver.org/
[assembly:AssemblyVersionAttribute("0.3.0")];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:InternalsVisibleToAttribute("PhysX.Test")];
[assembly:InternalsVisibleToAttribute("APEX.Net")];