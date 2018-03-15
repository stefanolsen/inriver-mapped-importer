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
using System.Xml;
using inRiver.Remoting;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;

namespace StefanOlsen.InRiver.MappedImporter.Parsers
{
    internal class FieldParserFactory
    {
        private readonly IinRiverManager _inRiverManager;
        private readonly IDictionary<string, IFieldParser> _cachedFieldParsers;
        private readonly IDictionary<string, CultureInfo> _supportedCultures;

        public FieldParserFactory(
            IinRiverManager inRiverManager,
            ImportMapping importMapping)
        {
            _inRiverManager = inRiverManager;
            _cachedFieldParsers = new Dictionary<string, IFieldParser>();

            _supportedCultures = importMapping.Languages?.ToDictionary(
                lang => lang.Original,
                lang => CultureInfo.GetCultureInfo(lang.InRiver));
        }

        public IFieldParser GetFieldParser(BaseField fieldMapping)
        {
            Type fieldType = fieldMapping.GetType();
            string fieldTypeName = fieldType.Name;

            if (_cachedFieldParsers.TryGetValue(fieldTypeName, out IFieldParser fieldParser))
            {
                return fieldParser;
            }

            fieldParser = CreateFieldParser(fieldType);
            _cachedFieldParsers.Add(fieldTypeName, fieldParser);

            return fieldParser;
        }

        private IFieldParser CreateFieldParser(Type fieldType)
        {
            IFieldParser fieldParser;

            switch (fieldType.Name)
            {
                case nameof(BooleanField):
                    fieldParser = new BooleanFieldParser();
                    break;
                case nameof(DateTimeField):
                    fieldParser = new DateTimeFieldParser();
                    break;
                case nameof(IntegerField):
                    fieldParser = new IntegerFieldParser();
                    break;
                case nameof(LocaleStringField):
                    fieldParser = new LocaleStringFieldParser(_supportedCultures);
                    break;
                case nameof(StringField):
                    fieldParser = new StringFieldParser();
                    break;
                case nameof(CvlField):
                    fieldParser = new CvlFieldParser(_inRiverManager);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldType));
            }

            return fieldParser;
        }
    }
}
