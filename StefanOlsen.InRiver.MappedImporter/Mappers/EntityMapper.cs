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
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using inRiver.Remoting;
using StefanOlsen.InRiver.MappedImporter.Models;
using StefanOlsen.InRiver.MappedImporter.Models.Mapping;
using StefanOlsen.InRiver.MappedImporter.Parsers;

namespace StefanOlsen.InRiver.MappedImporter.Mappers
{
    public class EntityMapper
    {
        private readonly IXmlNamespaceResolver _namespaceResolver;
        private readonly IinRiverManager _inRiverManager;
        private readonly ImportMapping _importMapping;
        private readonly FieldParserFactory _fieldParserFactory;
        private readonly IDictionary<string, XPathExpression> _cachedXPathExpressions;

        internal EntityMapper(
            IXmlNamespaceResolver namespaceResolver,
            IinRiverManager inRiverManager,
            ImportMapping importMapping)
        {
            _namespaceResolver = namespaceResolver;
            _importMapping = importMapping;
            _inRiverManager = inRiverManager;

            _fieldParserFactory = new FieldParserFactory(inRiverManager, importMapping);
            _cachedXPathExpressions = new Dictionary<string, XPathExpression>();
        }

        public IEnumerable<MappedEntity> GetEntities(XPathNavigator parentNode, EntityMapping entityMapping)
        {
            var entityNodes = parentNode.Select(entityMapping.Root, _namespaceResolver);
            foreach (XPathNavigator entityNode in entityNodes)
            {
                MappedEntity entity = GetEntity(entityNode, entityMapping);

                yield return entity;

                EntityMapping[] entityMappings = entityMapping.Entity;
                if (entityMappings == null || entityMappings.Length == 0)
                {
                    continue;
                }

                // Recursive call of this method to get child entities.
                IEnumerable<MappedEntity> subEntities =
                    entityMappings.SelectMany(em => GetEntities(entityNode, em));
                foreach (var subEntity in subEntities)
                {
                    yield return subEntity;
                }
            }
        }

        public MappedEntity GetEntity(XPathNavigator parentNode, EntityMapping entityMapping)
        {
            var mappedEntity = new MappedEntity();
            mappedEntity.EntityType = entityMapping.EntityType;
            mappedEntity.Fields = GetFields(parentNode, entityMapping.Fields);
            mappedEntity.FieldSet = GetFieldSet(parentNode, entityMapping);
            mappedEntity.Links = GetLinks(parentNode, entityMapping);
            mappedEntity.UniqueFieldType = entityMapping.UniqueFieldType;

            return mappedEntity;
        }

        private string GetFieldSet(XPathNavigator parentNode, EntityMapping entityMapping)
        {
            FieldSets fieldSets = entityMapping.FieldSets;
            if (fieldSets == null)
            {
                return null;
            }

            string fieldSetValue = parentNode
                .Evaluate(fieldSets.XPath, _namespaceResolver)?
                .ToString();
            if (fieldSetValue == null)
            {
                return null;
            }

            return fieldSets.FieldSet
                .FirstOrDefault(fs => fs.Value == fieldSetValue)?
                .FieldSetName;
        }

        private ICollection<MappedField> GetFields(XPathNavigator parentNode, BaseField[] fieldMappings)
        {
            var mappedFields = new List<MappedField>();
            if (fieldMappings == null)
            {
                return null;
            }

            foreach (BaseField fieldMapping in fieldMappings)
            {
                object fieldValue = GetFieldValue(parentNode, fieldMapping);

                var mappedField = new MappedField(fieldMapping.FieldType, fieldValue);

                mappedFields.Add(mappedField);
            }

            return mappedFields;
        }

        private IEnumerable<MappedLink> GetLinks(XPathNavigator parentNode, EntityMapping entityMapping)
        {
            if (entityMapping.Entity == null)
            {
                yield break;
            }

            foreach (EntityMapping e in entityMapping.Entity)
            {
                foreach (LinkMapping link in e.Links)
                {
                    yield return GetLink(parentNode, link);
                }
            }
        }

        private MappedLink GetLink(XPathNavigator parentNode, LinkMapping linkMapping)
        {
            var mappedLink = new MappedLink();
            mappedLink.LinkType = linkMapping.LinkType;

            LinkEntityMapping linkEntityMapping = linkMapping.LinkEntity;
            if (linkEntityMapping != null)
            {
                mappedLink.LinkEntityType = linkEntityMapping.EntityType;
                mappedLink.Fields = GetFields(parentNode, linkEntityMapping.Fields);
            }

            return mappedLink;
        }

        private object GetFieldValue(XPathNavigator parentNode, BaseField fieldMapping)
        {
            IFieldParser fieldParser = _fieldParserFactory.GetFieldParser(fieldMapping);

            object value;
            if (!string.IsNullOrEmpty(fieldMapping.ElementPath))
            {
                XPathExpression xPathExpression = GetCachedExpression(fieldMapping.ElementPath);
                value = fieldParser.GetElementValue(parentNode, fieldMapping, xPathExpression);
            }
            else if (!string.IsNullOrEmpty(fieldMapping.AttributeName))
            {
                value = fieldParser.GetAttributeValue(parentNode, fieldMapping, fieldMapping.AttributeName);
            }
            else
            {
                throw new InvalidOperationException("Field mappings must have either ElementPath or AttributeName specified.");
            }

            return value;
        }

        private XPathExpression GetCachedExpression(string xpath)
        {
            if (_cachedXPathExpressions.TryGetValue(xpath, out XPathExpression xPathExpression))
            {
                return xPathExpression;
            }

            xPathExpression = XPathExpression.Compile(xpath, _namespaceResolver);
            _cachedXPathExpressions.Add(xpath, xPathExpression);

            return xPathExpression;
        }
    }
}
