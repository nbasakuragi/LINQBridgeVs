﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using BridgeVs.Logging;
using BridgeVs.DynamicCore;
using BridgeVs.DynamicCore.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.UnitTest;
using BridgeVs.Locations;

namespace DynamicCore.UnitTest
{
    [TestClass]
    public class DynamicDebuggerVisualizerTest
    {
        private readonly Message _message = new Message
        {
            FileName = DateTime.Now.ToShortDateString().Replace("/", ""),
            TypeFullName = typeof(CustomType1).FullName,
            TypeNamespace = typeof(CustomType1).Namespace,
            TypeName = typeof(CustomType1).Name
        };

       
        private static IFileSystem _fileSystem;

        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\myfile.txt", new MockFileData("Testing is meh.") },
                { @"c:\demo\jQuery.js", new MockFileData("some js") },
                { @"c:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void DeployLinqScriptTest()
        {
            Log.Configure("UnitTest", "DynamicDebuggerVisualizerUnitTest");

            DynamicDebuggerVisualizer cVisualizerObjectSource = new DynamicDebuggerVisualizer(_fileSystem);
            cVisualizerObjectSource.DeployLinqScript(_message);

            string dstScriptPath = CommonFolderPaths.LinqPadQueryFolder;

            string fileNamePath = Path.Combine(dstScriptPath, string.Format(_message.FileName, _message.TypeFullName));

            Assert.IsTrue(_fileSystem.File.Exists(fileNamePath));
        }
    }
}
