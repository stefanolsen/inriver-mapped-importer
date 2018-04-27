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
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting;
using inRiver.Remoting.Objects;

namespace StefanOlsen.InRiver.MappedImporter.Mappers
{
    public class EntityRepository
    {
        private const string UniqueValueEntityIdFormatKey = "{0}_{1}";

        private readonly IinRiverManager _inRiverManager;

        private readonly Dictionary<string, int> _uniqueValueEntityIds = new Dictionary<string, int>();

        public EntityRepository(IinRiverManager inRiverManager)
        {
            _inRiverManager = inRiverManager;
        }

        public Entity AddEntity(Entity entity)
        {
            //entity = _inRiverManager.DataService.AddEntity(entity);

            foreach (var field in entity.Fields.Where(f=>f.FieldType.Unique))
            {
                string key = string.Format(UniqueValueEntityIdFormatKey, field.FieldType.Id, field.Data);
                if (_uniqueValueEntityIds.ContainsKey(key))
                {
                    continue;
                }

                _uniqueValueEntityIds.Add(key, entity.Id);
            }

            return entity;
        }

        public bool DeleteEntity(int id)
        {
            bool success = _inRiverManager.DataService.DeleteEntity(id);

            return success;
        }

        public Entity UpdateEntity(Entity entity)
        {
            //entity = _inRiverManager.DataService.UpdateEntity(entity);

            return entity;
        }

        public Entity GetEntityByUniqueValue(string fieldTypeId, string value, LoadLevel level)
        {
            Entity entity = _inRiverManager.DataService.GetEntityByUniqueValue(fieldTypeId, value, level);
            if (entity == null)
            {
                return null;
            }

            string key = string.Format(UniqueValueEntityIdFormatKey, fieldTypeId, value);
            if (_uniqueValueEntityIds.ContainsKey(key))
            {
                return entity;
            }

            _uniqueValueEntityIds.Add(key, entity.Id);

            return entity;
        }

        public int GetEntityIdByUniqueValue(string fieldTypeId, string value)
        {
            string key = string.Format(UniqueValueEntityIdFormatKey, fieldTypeId, value);
            if (_uniqueValueEntityIds.TryGetValue(key, out int entityId))
            {
                return entityId;
            }

            entityId = GetEntityByUniqueValue(fieldTypeId, value, LoadLevel.Shallow)?.Id ?? 0;

            return entityId;
        }

        public Link AddLink(Link link)
        {
            //link = _inRiverManager.DataService.AddLink(link);

            return link;
        }

        public bool LinkAlreadyExists(int sourceEntityId, int targetEntityId, int? linkEntityId, string linkTypeId)
        {
            bool exists = _inRiverManager.DataService.LinkAlreadyExists(
                sourceEntityId, targetEntityId, linkEntityId, linkTypeId);

            return exists;
        }
    }
}
