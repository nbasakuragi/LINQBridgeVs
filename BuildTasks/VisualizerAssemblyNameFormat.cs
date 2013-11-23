﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQBridge.BuildTasks
{
    internal static class VisualizerAssemblyNameFormat
    {
        internal static string GetTargetVisualizerAssemblyName(string vsVersion, string assembly)
        { 
            return System.IO.Path.GetFileNameWithoutExtension(assembly) + ".Visualizer.V" + vsVersion + ".dll";

        }
    }
}