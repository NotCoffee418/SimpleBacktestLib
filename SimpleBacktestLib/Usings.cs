global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using SimpleBacktestLib.Internal.Logic;
global using System.Runtime.CompilerServices;
global using System.Collections.Immutable;
global using SimpleBacktestLib.Internal.InternalModels;
global using SimpleBacktestLib.Models;
global using SimpleBacktestLib.TradingManagers;
global using SimpleBacktestLib.Internal.Helpers;


// Imported
global using Microsoft.Extensions.Logging;

// Allow tests on internal functions
[assembly: InternalsVisibleToAttribute("SimpleBacktestLib.Tests")]