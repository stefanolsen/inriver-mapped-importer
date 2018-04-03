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
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using StefanOlsen.InRiver.MappedImporter.Models;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;

namespace StefanOlsen.InRiver.MappedImporter.Parsers
{
    public class SkuFieldParser : IFieldParser
    {
        private const string RootNamespaceSkuDocument = "http://schemas.stefanolsen.com/inRiver/SKU-document";
        private readonly XmlSerializerNamespaces _serializerNamespaces;
        private readonly IXmlNamespaceResolver _namespaceResolver;

        public SkuFieldParser(IXmlNamespaceResolver namespaceResolver)
        {
            _namespaceResolver = namespaceResolver;

            _serializerNamespaces = new XmlSerializerNamespaces();
            _serializerNamespaces.Add("", RootNamespaceSkuDocument);
        }

        public object GetAttributeValue(XPathNavigator parentNode, BaseField fieldMapping, string attributeName)
        {
            throw new NotSupportedException();
        }

        public object GetElementValue(XPathNavigator parentNode, BaseField fieldMapping, XPathExpression xpath)
        {
            var descendants = parentNode.Select(xpath);
            var skuList = new List<SKU>(descendants.Count);

            SKUField skuFieldMapping = (SKUField)fieldMapping;

            foreach (XPathNavigator descendant in descendants)
            {
                var skuElements = new List<XmlElement>();

                var sku = new SKU();
                if (!string.IsNullOrEmpty(skuFieldMapping.KeyAttribute))
                {
                    sku.Id = descendant.GetAttribute(skuFieldMapping.KeyAttribute, string.Empty);
                }
                
                var doc = new XmlDocument();
                foreach (var element in skuFieldMapping.SKUElement)
                {
                    var skuElement = doc.CreateElement(element.Name, RootNamespaceSkuDocument);
                    var node = descendant.SelectSingleNode(element.ElementPath, _namespaceResolver);
                    if (node == null)
                    {
                        continue;
                    }

                    skuElement.InnerText = node.Value;
                    skuElements.Add(skuElement);
                }

                sku.Any = skuElements.ToArray();
                skuList.Add(sku);
            }

            var skuDocument = new SKUs { SKU = skuList.ToArray() };

            var serializer = new XmlSerializer(typeof(SKUs));
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter))
            {
                serializer.Serialize(xmlWriter, skuDocument, _serializerNamespaces);
                string xml = stringWriter.ToString();

                return xml;
            }
        }
    }
}
