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
using System.Globalization;
using System.Linq;
using System.Xml.XPath;
using inRiver.Remoting.Objects;
using StefanOlsen.InRiver.MappedImporter.Mappers;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;
using StefanOlsen.InRiver.MappedImporter.Utilities;

namespace StefanOlsen.InRiver.MappedImporter.Parsers
{
    public class CvlFieldParser : IFieldParser
    {
        private readonly CvlRepository _cvlRepository;
        private readonly IDictionary<string, CultureInfo> _supportedCultures;

        public CvlFieldParser(
            CvlRepository cvlRepository,
            IDictionary<string, CultureInfo> supportedCultures)
        {
            _cvlRepository = cvlRepository;
            _supportedCultures = supportedCultures;
        }

        public object GetElementValue(XPathNavigator parentNode, BaseField fieldMapping, XPathExpression xpath)
        {
            CvlField cvlFieldMapping = (CvlField)fieldMapping;

            var node = parentNode.SelectSingleNode(xpath);
            if (node == null)
            {
                return null;
            }

            string cvlId = cvlFieldMapping.Cvl;
            bool addValues = cvlFieldMapping.AddValues;
            string cvlValue;

            if (!cvlFieldMapping.Multivalue)
            {
                cvlValue = GetCvlFieldValue(cvlId, node.Value, addValues);
            }
            else
            {
                string[] values = node.Value.Split(
                    new[] { cvlFieldMapping.Separator },
                    StringSplitOptions.RemoveEmptyEntries);
                cvlValue = string.Join(";", values
                    .Select(cv => GetCvlFieldValue(cvlId, cv, addValues))
                    .Where(s => s != null));
            }

            return cvlValue;
        }

        private string GetCvlFieldValue(string cvlId, string value, bool addValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string key = value.RemoveSpecialCharacters();
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            CVL cvl = _cvlRepository.GetCVL(cvlId);
            if (cvl == null)
            {
                return null;
            }

            CVLValue cvlValue = _cvlRepository.GetCVLValueByKey(cvl.Id, key);
            if (cvlValue != null)
            {
                return cvlValue.Key;
            }

            if (!addValue)
            {
                return null;
            }

            if (cvl.DataType == DataType.String)
            {
                cvlValue = new CVLValue { CVLId = cvl.Id, Key = key, Value = value };
            }
            else if (cvl.DataType == DataType.LocaleString)
            {
                var localeString = new LocaleString();
                foreach (CultureInfo culture in _supportedCultures.Values)
                {
                    localeString[culture] = value;
                }

                cvlValue = new CVLValue { CVLId = cvl.Id, Key = key, Value = localeString };
            }
            else
            {
                return null;
            }

            _cvlRepository.AddCVLValue(cvlValue);

            return key;
        }
    }
}
