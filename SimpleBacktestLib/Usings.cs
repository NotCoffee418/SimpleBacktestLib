global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using SimpleBacktestLib.Internal.Logic;
global using System.Runtime.CompilerServices;
global using SimpleBacktestLib.Internal.InternalModels;
global using SimpleBacktestLib.Models;
global using SimpleBacktestLib.TradingManagers;
global using SimpleBacktestLib.Internal.Helpers;
global using System.Collections.Immutable;


// Imported
global using Microsoft.Extensions.Logging;

// Allow tests on internal functions
[assembly: InternalsVisibleToAttribute("SimpleBacktestLib.Tests")]

// Handle `init` on .NET Standard 2.1
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}