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

﻿using System;
using System.Collections.Generic;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Extension.Interface;
using inRiver.Remoting.Log;
using StefanOlsen.InRiver.MappedImporter.Models;
using StefanOlsen.InRiver.MappedImporter.Utilities;

namespace StefanOlsen.InRiver.MappedImporter
{
    public class MappedInboundExtension : IInboundDataExtension
    {
        public inRiverContext Context { get; set; }
        public Dictionary<string, string> DefaultSettings => new Dictionary<string, string>
        {
            {"MAPPING_CONFIGURATION_XML", "Insert XML mapping here" }
        };

        public string Test()
        {
            return "Nothing to test.";
        }

        public string Add(string value)
        {
            string mappingDocument = Context.Settings.GetStringValue("MAPPING_CONFIGURATION_XML");
            if (mappingDocument == null)
            {
                throw new Exception("No setting called MAPPING_CONFIGURATION_XML was found.");
            }

            Context.Log(LogLevel.Information, "Initializing extension configuration and field mappings.");
            CatalogDocument document = new CatalogDocument(Context);
            document.Initialize(mappingDocument, value);
            Context.Log(LogLevel.Information, "Finished initializing extension configuration and field mappings.");

            Context.Log(LogLevel.Information, "Parsing XML document and importing entities.");
            IEnumerable<MappedEntity> mappedEntities = document.GetEntities();

            ImportProcessor processor = new ImportProcessor(Context);
            processor.ImportEntities(mappedEntities);
            Context.Log(LogLevel.Information, "Finished parsing XML document and importing entities.");

            return "SUCCESS";
        }

        public string Update(string value)
        {
            throw new NotImplementedException();
        }

        public string Delete(string value)
        {
            throw new NotImplementedException();
        }
    }
}
