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
using inRiver.Remoting.Extension;
using inRiver.Remoting.Log;
using inRiver.Remoting.Objects;
using StefanOlsen.InRiver.MappedImporter.Mappers;
using StefanOlsen.InRiver.MappedImporter.Models;
using StefanOlsen.InRiver.MappedImporter.Utilities;

namespace StefanOlsen.InRiver.MappedImporter
{
    public class ImportProcessor
    {
        private readonly EntityRepository _entityRepository;
        private readonly ModelsRepository _modelsRepository;
        private readonly inRiverContext _context;

        public ImportProcessor(inRiverContext context)
        {
            _context = context;
            _entityRepository = new EntityRepository(context.ExtensionManager);
            _modelsRepository = new ModelsRepository(context.ExtensionManager);
        }

        public void DeleteEntities(IEnumerable<MappedEntity> mappedEntities)
        {
            foreach (MappedEntity mappedEntity in mappedEntities)
            {
                Entity entity = _entityRepository
                    .GetEntityByUniqueValue(
                        mappedEntity.UniqueFieldType,
                        mappedEntity.UniqueFieldValue,
                        LoadLevel.Shallow);
                if (entity == null)
                {
                    continue;
                }

                _entityRepository.DeleteEntity(entity.Id);
            }
        }

        public void ImportEntities(IEnumerable<MappedEntity> mappedEntities)
        {
            int count = 0;

            foreach (MappedEntity mappedEntity in mappedEntities)
            {
                bool newEntity = false;
                Entity entity = _entityRepository
                    .GetEntityByUniqueValue(
                        mappedEntity.UniqueFieldType,
                        mappedEntity.UniqueFieldValue,
                        LoadLevel.DataOnly);
                if (entity == null)
                {
                    EntityType entityType = _modelsRepository
                        .GetEntityType(mappedEntity.EntityType);
                    entity = Entity.CreateEntity(entityType);
                    newEntity = true;
                }

                bool entityModified = false;
                foreach (MappedField mappedField in mappedEntity.Fields)
                {
                    // This extension method is much more efficient than the built-in GetField() method.
                    Field field = entity.GetFieldOrdinal(mappedField.Name);
                    if (field == null)
                    {
                        throw new InvalidOperationException(
                            $"The field '{mappedField.Name}' does not exist in the model.");
                    }

                    object fieldValue = mappedField.Value;

                    field.Data = fieldValue;

                    entityModified = true;
                }

                if (entity.FieldSetId != mappedEntity.FieldSet)
                {
                    entity.FieldSetId = mappedEntity.FieldSet;
                    entityModified = true;
                }

                if (entity.Id == 0)
                {
                    entity = _entityRepository.AddEntity(entity);
                }
                else if (entityModified)
                {
                    entity = _entityRepository.UpdateEntity(entity);
                }

                foreach (MappedLink mappedLink in mappedEntity.Links)
                {
                    ImportLink(mappedLink, entity, newEntity);
                }

                count++;
            }

            _context.Log(LogLevel.Information, $"Finished importing {count} entities.");
        }

        private void ImportLink(MappedLink mappedLink, Entity currentEntity, bool newEntity)
        {
            int linkedEntityId = _entityRepository.GetEntityIdByUniqueValue(
                mappedLink.LinkedUniqueFieldType,
                mappedLink.LinkedUniqueFieldValue);
            if (linkedEntityId == 0)
            {
                return;
            }

            LinkType linkType = _modelsRepository.GetLinkType(mappedLink.LinkType);
            if (linkType == null)
            {
                throw new InvalidOperationException("The link type '{mappedLink.LinkType}' does not exist in the model.");
            }

            Entity sourceEntity;
            Entity targetEntity;
            if (mappedLink.Direction == LinkDirection.ChildParent)
            {
                sourceEntity = new Entity {Id = linkedEntityId};
                targetEntity = currentEntity;
            }
            else
            {
                sourceEntity = currentEntity;
                targetEntity = new Entity {Id = linkedEntityId};
            }

            // If entity is new, there would be no existing links.
            // If it is not new, skip adding a link if it already exists.
            bool linkExists = !newEntity &&
                              _entityRepository.LinkAlreadyExists(
                                  sourceEntity.Id, targetEntity.Id, null, linkType.Id);
            if (linkExists)
            {
                return;
            }

            Link link = new Link
            {
                LinkType = linkType,
                Source = sourceEntity,
                Target = targetEntity,
                Index = mappedLink.SortIndex
            };

            _entityRepository.AddLink(link);
        }
    }
}
