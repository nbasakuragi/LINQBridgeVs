﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO.Abstractions;

namespace LINQBridge.DynamicVisualizers.Utils
{
    public static class AssemblyFinder
    {
        private const string SearchPattern = "*{0}*.dll";

        private static readonly Func<string, bool> IsSystemAssembly =
            name => name.Contains("Microsoft") || name.Contains("System") || name.Contains("mscorlib");

        public static IFileSystem FileSystem { get; set; }

        public static IEnumerable<string> GetReferencedAssembliesPath(this Assembly assembly, bool includeSystemAssemblies = false)
        {
            var retPaths = new List<string>();

            var referencedAssemblies = assembly.GetReferencedAssemblies()
                                               .Where(name => includeSystemAssemblies || !IsSystemAssembly(name.Name))
                                               .Select(name => name.Name)
                                               .ToList();

            if (!referencedAssemblies.Any()) return retPaths;



            var currentAssemblyPath = FileSystem.DirectoryInfo.FromDirectoryName(assembly.Location);
            Logging.Log.Write("currentAssemblyPath: {0}", currentAssemblyPath);

            if (currentAssemblyPath == null) return Enumerable.Empty<string>();


            referencedAssemblies
                .ForEach(s =>
                             {
                                 retPaths.Add(FindPath(s, currentAssemblyPath.FullName));
                                 Logging.Log.Write("Assembly Path {0}", s);
                             });


            return retPaths.Where(s => !string.IsNullOrEmpty(s));
        }

        internal static string FindPath(string fileToSearch, string rootPath)
        {
            if (rootPath == null) return string.Empty;
            Logging.Log.Write("SearchPattern: {0}", string.Format(SearchPattern, fileToSearch));

            try
            {
                var file = FileSystem.Directory
                    .EnumerateFiles(rootPath, string.Format(SearchPattern, fileToSearch), System.IO.SearchOption.AllDirectories)
                    .AsParallel()
                    .OrderByDescending(info => FileSystem.FileInfo.FromFileName(info).LastAccessTime)
                    .FirstOrDefault();

                if (file == null)
                {
                    var parent = FileSystem.DirectoryInfo.FromDirectoryName(rootPath).Parent;
                    return parent != null ? FindPath(fileToSearch, parent.FullName) : string.Empty;
                }


                Logging.Log.Write("File Found {0}", file);
                return file;
            }
            catch (Exception e)
            {
                Logging.Log.Write(e, "FindPath");
                throw;
            }


        }


    }
}