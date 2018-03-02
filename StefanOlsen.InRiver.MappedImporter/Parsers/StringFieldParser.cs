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

﻿using System.Xml;
using System.Xml.XPath;

namespace StefanOlsen.InRiver.MappedImporter.Parsers
{
    public class StringFieldParser : IFieldParser
    {
        private readonly IXmlNamespaceResolver _namespaceResolver;

        public StringFieldParser(IXmlNamespaceResolver namespaceResolver)
        {
            _namespaceResolver = namespaceResolver;
        }

        public object GetAttributeValue(XPathNavigator parentNode, string attributeName)
        {
            string value = parentNode.GetAttribute(attributeName, string.Empty);

            return value;
        }

        public object GetElementValue(XPathNavigator parentNode, string xpath)
        {
            var node = parentNode.SelectSingleNode(xpath, _namespaceResolver);

            string value = node?.Value;

            return value;
        }
    }
}
