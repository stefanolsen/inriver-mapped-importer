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
using inRiver.Remoting.Objects;

namespace StefanOlsen.InRiver.MappedImporter.Mappers
{
    public class ModelsRepository
    {
        private readonly IinRiverManager _inRiverManager;

        private readonly Dictionary<string, EntityType> _entityTypes = new Dictionary<string, EntityType>();
        private readonly Dictionary<string, FieldType> _fieldTypes = new Dictionary<string, FieldType>();
        private readonly Dictionary<string, LinkType> _linkTypes = new Dictionary<string, LinkType>();

        public ModelsRepository(IinRiverManager inRiverManager)
        {
            _inRiverManager = inRiverManager;
        }

        public EntityType GetEntityType(string id)
        {
            if (_entityTypes.TryGetValue(id, out EntityType entityType))
            {
                return entityType;
            }

            entityType = _inRiverManager.ModelService.GetEntityType(id);
            _entityTypes.Add(entityType.Id, entityType);

            return entityType;
        }
        public FieldType GetFieldType(string id)
        {
            if (_fieldTypes.TryGetValue(id, out FieldType fieldType))
            {
                return fieldType;
            }

            fieldType = _inRiverManager.ModelService.GetFieldType(id);
            _fieldTypes.Add(fieldType.Id, fieldType);

            return fieldType;
        }


        public LinkType GetLinkType(string id)
        {
            if (_linkTypes.TryGetValue(id, out LinkType linkType))
            {
                return linkType;
            }

            linkType = _inRiverManager.ModelService.GetLinkType(id);
            _linkTypes.Add(linkType.Id, linkType);

            return linkType;
        }
    }
}
