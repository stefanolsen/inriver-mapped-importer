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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using inRiver.Remoting;
using StefanOlsen.InRiver.MappedImporter.Mappers;
using StefanOlsen.InRiver.MappedImporter.Models;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;

namespace StefanOlsen.InRiver.MappedImporter
{
    public class CatalogDocument
    {
        private readonly IinRiverManager _inRiverManager;
        private readonly XmlNamespaceManager _namespaceResolver;

        private EntityMapper _entityMapper;
        private ImportMapping _importMapping;
        private XPathNavigator _rootNavigator;
        private XPathDocument _xPathDocument;

        private bool _isInitialized;

        public CatalogDocument(IinRiverManager inRiverManager)
        {
            _inRiverManager = inRiverManager;
            _namespaceResolver = new XmlNamespaceManager(new NameTable());
        }

        public void Initialize(string mappingDocument, string dataDocument)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"This {nameof(CatalogDocument)} is already initialized.");
            }

            InitializeMapping(mappingDocument);
            LoadData(dataDocument);

            _isInitialized = true;
        }

        public IEnumerable<MappedEntity> GetEntities()
        {
            var rootEntityMapping = _importMapping.Entity;


            var entityNodes = _rootNavigator.Select(rootEntityMapping.Root, _namespaceResolver);
            foreach (XPathNavigator entityNode in entityNodes)
            {
                MappedEntity entity = _entityMapper.GetEntity(entityNode, rootEntityMapping);

                yield return entity;
            }
        }

        private void InitializeMapping(string mappingDocument)
        {
            var serializer = new XmlSerializer(typeof(ImportMapping));

            using (TextReader reader = new StringReader(mappingDocument))
            {
                _importMapping = (ImportMapping)serializer.Deserialize(reader);
            }

            foreach (var ns in _importMapping.XmlNamespaces)
            {
                _namespaceResolver.AddNamespace(ns.Prefix, ns.Uri);
            }

            _entityMapper = new EntityMapper(_namespaceResolver, _importMapping);
        }

        private void LoadData(string dataDocument)
        {
            Stream dataStream = GetStream(dataDocument);
            _xPathDocument = new XPathDocument(dataStream);

            _rootNavigator = _xPathDocument.CreateNavigator();
        }

        private static Stream GetStream(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Stream stream = new MemoryStream(bytes, false);

            return stream;
        }
    }
}
