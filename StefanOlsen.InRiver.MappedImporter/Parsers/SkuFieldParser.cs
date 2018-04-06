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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;
using StefanOlsen.InRiver.MappedImporter.Utilities;

namespace StefanOlsen.InRiver.MappedImporter.Parsers
{
    public class SkuFieldParser : IFieldParser
    {
        private readonly CachedXPathCompiler _xPathCompiler;

        public SkuFieldParser(CachedXPathCompiler xPathCompiler)
        {
            _xPathCompiler = xPathCompiler;
        }

        public object GetAttributeValue(XPathNavigator parentNode, BaseField fieldMapping, string attributeName)
        {
            throw new NotSupportedException();
        }

        public object GetElementValue(XPathNavigator parentNode, BaseField fieldMapping, XPathExpression xpath)
        {
            SKUField skuFieldMapping = (SKUField)fieldMapping;

            var descendants = parentNode.Select(xpath);

            var doc = new XmlDocument();
            XmlElement rootNode = doc.CreateElement("SKUs");
            doc.AppendChild(rootNode);

            foreach (XPathNavigator descendant in descendants)
            {
                var skuNode = doc.CreateElement("SKU");

                if (!string.IsNullOrEmpty(skuFieldMapping.KeyAttribute))
                {
                    string id = descendant.GetAttribute(skuFieldMapping.KeyAttribute, string.Empty);

                    XmlAttribute idAttribute = doc.CreateAttribute("Id");
                    idAttribute.Value = id;

                    skuNode.Attributes.Append(idAttribute);
                }
                
                foreach (var element in skuFieldMapping.SKUElement)
                {
                    XPathExpression xPathExpression = _xPathCompiler.GetCachedExpression(element.ElementPath);
                    var node = descendant.SelectSingleNode(xPathExpression);
                    if (node == null)
                    {
                        continue;
                    }

                    var fieldElement = doc.CreateElement(element.Name);
                    fieldElement.InnerText = node.Value;

                    skuNode.AppendChild(fieldElement);
                }

                rootNode.AppendChild(skuNode);
            }

            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter))
            {
                doc.WriteTo(xmlWriter);
                xmlWriter.Flush();

                string xml = stringWriter.ToString();

                return xml;
            }
        }
    }
}
