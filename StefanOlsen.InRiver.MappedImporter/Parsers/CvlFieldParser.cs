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
using System.Xml.XPath;
using inRiver.Remoting;
using inRiver.Remoting.Objects;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;
using StefanOlsen.InRiver.MappedImporter.Utilities;

namespace StefanOlsen.InRiver.MappedImporter.Parsers
{
    public class CvlFieldParser : IFieldParser
    {
        private readonly IinRiverManager _inRiverManager;

        public CvlFieldParser(IinRiverManager inRiverManager)
        {
            _inRiverManager = inRiverManager;
        }

        public object GetAttributeValue(XPathNavigator parentNode, BaseField fieldMapping, string attributeName)
        {
            CvlField cvlFieldMapping = (CvlField)fieldMapping;

            string value = parentNode.GetAttribute(attributeName, string.Empty);

            string cvlValue = GetCvlFieldValue(cvlFieldMapping.Cvl, value, cvlFieldMapping.AddValues);

            return cvlValue;
        }

        public object GetElementValue(XPathNavigator parentNode, BaseField fieldMapping, XPathExpression xpath)
        {
            CvlField cvlFieldMapping = (CvlField) fieldMapping;

            var node = parentNode.SelectSingleNode(xpath);
            if (node == null)
            {
                return null;
            }

            string cvlValue = GetCvlFieldValue(cvlFieldMapping.Cvl, node.Value, cvlFieldMapping.AddValues);

            return cvlValue;
        }

        private string GetCvlFieldValue(string cvl, string value, bool addValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string key = value.RemoveSpecialCharacters();
            key = key.ToLowerInvariant();

            CVLValue cvlValue = _inRiverManager.ModelService.GetCVLValueByKey(key, cvl);
            if (cvlValue != null)
            {
                return cvlValue.Key;
            }

            if (!addValue)
            {
                return null;
            }

            cvlValue = new CVLValue {CVLId = cvl, Key = key, Value = value};
            _inRiverManager.ModelService.AddCVLValue(cvlValue);

            return key;
        }
    }
}
