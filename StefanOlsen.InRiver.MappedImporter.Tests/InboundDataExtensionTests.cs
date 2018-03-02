/*
 * Copyright (c) 2018 Stefan Holm Olsen
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

﻿using System.Collections.Generic;
using inRiver.Remoting;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Extension.Interface;
using inRiver.Remoting.Log;
using StefanOlsen.InRiver.MappedImporter.Tests.Fakes;
using Xunit;
using Xunit.Abstractions;

namespace StefanOlsen.InRiver.MappedImporter.Tests
{
    public class InboundDataExtensionTests
    {
        private const string MappingConfigurationFilePath =
            "StefanOlsen.InRiver.MappedImporter.Tests.FieldMapping.xml";
        private const string TestDataFilePath =
            "StefanOlsen.InRiver.MappedImporter.Tests.TestData.xml";

        private readonly XUnitLogger _extensionLog;

        public InboundDataExtensionTests(ITestOutputHelper outputHelper)
        {
            _extensionLog = new XUnitLogger(outputHelper);
        }

        private IInboundDataExtension CreateExtension()
        {
            string mappingConfiguration = EmbeddedResourceHelper.GetResourceTextFile(MappingConfigurationFilePath);

            IinRiverManager inRiverManager = new FakeInRiverManager();

            inRiverContext context = new inRiverContext(inRiverManager, _extensionLog);
            context.Settings = new Dictionary<string, string>()
            {
                {"MAPPING_CONFIGURATION_XML", mappingConfiguration}
            };

            IInboundDataExtension inboundDataExtension = null;
            inboundDataExtension.Context = context;

            return inboundDataExtension;
        }

        [Fact]
        public void TestAddData()
        {
            string testData = EmbeddedResourceHelper.GetResourceTextFile(MappingConfigurationFilePath);

            IInboundDataExtension extension = CreateExtension();
            
            string result = extension.Add(testData);

            _extensionLog.Log(LogLevel.Information, "Result text: " + result);
        }
    }
}
