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
using inRiver.Remoting;
using inRiver.Remoting.Objects;
using inRiver.Remoting.Query;

namespace StefanOlsen.InRiver.MappedImporter.Tests.Fakes
{
    public class FakeDataService : IDataService
    {
        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        private readonly Dictionary<int, Link> _links = new Dictionary<int, Link>();
        private readonly Dictionary<int, ICollection<int>> _linksBySourceIds = new Dictionary<int, ICollection<int>>();
        private readonly Dictionary<string, int> _uniqueValues = new Dictionary<string, int>();
        private int _lastEntityId;
        private int _lastLinkId;

        public Entity AddEntity(Entity entity)
        {
            _lastEntityId++;

            entity.Id = _lastEntityId;
            _entities.Add(entity.Id, entity);

            foreach (Field uniqueField in entity.Fields.Where(f => f.FieldType.Unique))
            {
                _uniqueValues.Add(uniqueField.Data.ToString(), entity.Id);
            }

            return entity;
        }

        public Entity GetEntity(int id, LoadLevel level)
        {
            if (!_entities.TryGetValue(id, out Entity entity))
            {
                return null;
            }

            entity.LoadLevel = level;

            return entity;
        }

        public bool DeleteEntity(int id)
        {
            return _entities.Remove(id);
        }

        public bool DeleteAllEntities()
        {
            _entities.Clear();

            return true;
        }

        public Entity UpdateEntity(Entity entity)
        {
            if (!_entities.ContainsKey(entity.Id))
            {
                return null;
            }

            _entities[entity.Id] = entity;

            return entity;
        }

        public Entity UpdateFieldsForEntity(List<Field> fields)
        {
            throw new NotImplementedException();
        }

        public Field GetField(int entityId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public object GetFieldValue(int entityId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public List<Field> GetFieldsForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<Field> GetFields(int entityId, List<string> fieldTypeIds)
        {
            throw new NotImplementedException();
        }

        public List<Field> GetFieldHistory(int entityId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public List<FieldRevision> GetFieldRevisions(int entityId, string fieldTypeId, int maxNumberOfRevisions)
        {
            throw new NotImplementedException();
        }

        public List<Field> GetAllFieldsByFieldType(string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public Entity CreateNewVersion(int entityId)
        {
            throw new NotImplementedException();
        }

        public Entity GetEntityVersion(int entityId, int version)
        {
            throw new NotImplementedException();
        }

        public Entity GetEntityByUniqueValue(string fieldTypeId, string value, LoadLevel level)
        {
            if (!_uniqueValues.TryGetValue(value, out int entityId))
            {
                return null;
            }

            return GetEntity(entityId, level);
        }

        public List<Entity> GetEntities(List<int> idList, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public Entity LockEntity(int id, string username)
        {
            throw new NotImplementedException();
        }

        public Entity UnLockEntity(int id)
        {
            throw new NotImplementedException();
        }

        public bool UnLockEntities(List<int> entityIds)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetAllLockedEntities()
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetAllLockedEntitiesForUser(string username)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetAllEntitiesForEntityType(string entityTypeId, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetEntitiesForEntityType(int maxCount, string entityTypeId, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetResourcesForLightBoard(int entityId, bool includeSubEntitesResources)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetEntityResourcesForLightBoard(List<int> entityIds, bool includeSubEntitesResources)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetResourcesForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<Entity> GetResourcesForEntities(List<int> entityIds)
        {
            throw new NotImplementedException();
        }

        public List<EntityHistory> GetEntityHistory(int entityId)
        {
            throw new NotImplementedException();
        }

        public Entity SetEntityFieldSet(int entityId, string fieldSetId)
        {
            throw new NotImplementedException();
        }

        public Entity RemoveEntityFieldSet(int entityId)
        {
            throw new NotImplementedException();
        }

        public Entity SetMainPicture(int entityId, int resourceId)
        {
            throw new NotImplementedException();
        }

        public int GetEntityCountForEntityType(string entityTypeId)
        {
            throw new NotImplementedException();
        }

        public List<int> GetAllEntityIdsForEntityType(string entityTypeId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllFieldValuesForField(string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllFieldValuesForFieldCaseSensitive(string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public void ReCalculateDisplayValuesForAllEntities()
        {
            throw new NotImplementedException();
        }

        public void ReCalculateMainPictureForAllEntities()
        {
            throw new NotImplementedException();
        }

        public Link AddLink(Link link)
        {
            _lastLinkId++;

            link.Id = _lastLinkId;
            _links.Add(link.Id, link);

            if (!_linksBySourceIds.TryGetValue(link.Source.Id, out ICollection<int> linkIds))
            {
                linkIds = new List<int>();
                _linksBySourceIds.Add(link.Source.Id, linkIds);
            }

            linkIds.Add(link.Id);

            return link;
        }

        public bool DeleteLink(int linkId)
        {
            throw new NotImplementedException();
        }

        public Link UpdateLinkSortOrder(int linkId, int index)
        {
            throw new NotImplementedException();
        }

        public Link GetLink(int linkId)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetLinksForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetInboundLinksForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetOutboundLinksForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public bool LinkAlreadyExists(int sourceEntityId, int targetEntityId, int? linkEntityId, string linkTypeId)
        {
            if (!_linksBySourceIds.TryGetValue(sourceEntityId, out ICollection<int> linkIds))
            {
                return false;
            }

            foreach (var linkId in linkIds)
            {
                if (!_links.TryGetValue(linkId, out Link link))
                {
                    continue;
                }

                if (link.Target.Id == targetEntityId && 
                    link.LinkType.Id == linkTypeId)
                {
                    return true;
                }
            }

            return false;
        }

        public Link FindLink(int sourceEntityId, int targetEntityId, int? linkEntityId, string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLinks(List<int> linkIds)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllLinks()
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllLinksForLinkType(string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public bool AddLinks(List<Link> links)
        {
            throw new NotImplementedException();
        }

        public bool UpdateLinks(List<Link> links)
        {
            throw new NotImplementedException();
        }

        public bool Inactivate(int id)
        {
            throw new NotImplementedException();
        }

        public bool Activate(int id)
        {
            throw new NotImplementedException();
        }

        public Link AddLinkLast(Link link)
        {
            throw new NotImplementedException();
        }

        public Link AddLinkFirst(Link link)
        {
            throw new NotImplementedException();
        }

        public Link AddLinkAt(Link link, int index)
        {
            throw new NotImplementedException();
        }

        public bool AddLinksForEntityAndLinkTypeAt(List<Link> links, int index)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetOutboundLinksForEntityAndLinkType(int entityId, string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Entity, List<Link>> GetOutboundLinksForEntitiesAndLinkType(List<int> entityIds, string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Entity, List<Link>> GetInboundLinksForEntitiesAndLinkType(List<int> entityIds, string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Entity, List<Link>> GetLinksForLinkEntities(List<int> entityIds)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetInboundLinksForEntityAndLinkType(int entityId, string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetLinksForLinkEntity(int linkEntityId)
        {
            throw new NotImplementedException();
        }

        public int GetLinkCountForLinkType(string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessDetail> GetEntityCompletenessDetails(int entityId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, int> GetAssortmentLinkStructureCount(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetOutboundLinksForEntityAndLinkTypeAndLinkEntity(int entityId, string linkTypeId, int linkEntityId)
        {
            throw new NotImplementedException();
        }

        public List<Link> GetAllLinksForLinkType(string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition GetLinkRuleDefinition(int id)
        {
            throw new NotImplementedException();
        }

        public List<LinkRuleDefinition> GetLinkRuleDefinitionsForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition GetLinkRuleDefinitionForEntityWithLinkTypeId(int entityId, string linkTypeId)
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition AddLinkRuleDefinition(LinkRuleDefinition definition)
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition UpdateLinkRuleDefinition(LinkRuleDefinition definition)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLinkRuleDefinition(int id)
        {
            throw new NotImplementedException();
        }

        public List<LinkRuleDefinition> GetAllLinkRuleDefinitions()
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition DeleteAllRulesForLinkRuleDefinition(int definitionId)
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition SetLinkRuleDefinitionRules(int definitionId, List<LinkRule> rules)
        {
            throw new NotImplementedException();
        }

        public LinkRuleDefinition SetLinkRuleDefinitionQueries(int definitionId, List<LinkRuleQuery> queries)
        {
            throw new NotImplementedException();
        }

        public void UpdateLinksForLinkRuleDefinitions()
        {
            throw new NotImplementedException();
        }

        public List<Entity> Search(Criteria critera, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> Search(Query query, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> Search(ComplexQuery query, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> LinkSearch(LinkQuery linkQuery, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> SystemSearch(SystemQuery systemQuery, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> SearchCompleteness(CompletenessQuery query, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> SearchSpecification(SpecificationQuery query, LoadLevel level)
        {
            throw new NotImplementedException();
        }

        public List<Entity> QuickSearch(string searchString)
        {
            throw new NotImplementedException();
        }

        public List<SearchSuggestion> GetQuickSearchSuggestion(string prefix)
        {
            throw new NotImplementedException();
        }

        public List<TaskCategory> GetTasksAssignedToUser(string username, int maxAmount)
        {
            throw new NotImplementedException();
        }

        public List<TaskCategory> GetTasksCreatedByUser(string username, int maxAmount)
        {
            throw new NotImplementedException();
        }

        public List<TaskCategory> GetTasksByUserAndGroup(string username, string groupId, int maxAmount)
        {
            throw new NotImplementedException();
        }

        public List<Comment> GetAllCommentsForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public bool AddComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Comment AddCommentWithResult(Comment comment)
        {
            throw new NotImplementedException();
        }

        public bool DeleteComment(int id)
        {
            throw new NotImplementedException();
        }

        public bool EntityHasComment(int entityId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCommentsForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllSpecificationFieldTypes()
        {
            throw new NotImplementedException();
        }

        public List<SpecificationFieldType> GetAllSpecificationFieldTypes()
        {
            throw new NotImplementedException();
        }

        public SpecificationField UpdateSpecificationField(SpecificationField specificationField)
        {
            throw new NotImplementedException();
        }

        public void AddSpecificationField(SpecificationField field)
        {
            throw new NotImplementedException();
        }

        public SpecificationField GetSpecificationField(int entityId, string specificationFieldTypeId)
        {
            throw new NotImplementedException();
        }

        public List<SpecificationField> GetSpecificationFieldsForEntity(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<SpecificationFieldType> GetSpecificationTemplateFieldTypes(int entityId)
        {
            throw new NotImplementedException();
        }

        public List<SpecificationFieldType> GetSpecificationTemplateFieldTypesForFormatList(int entityId)
        {
            throw new NotImplementedException();
        }

        public SpecificationFieldType GetSpecificationFieldType(string specificationFieldTypeId)
        {
            throw new NotImplementedException();
        }

        public SpecificationFieldType AddSpecificationFieldType(SpecificationFieldType specFieldType)
        {
            throw new NotImplementedException();
        }

        public void DeleteSpecificationFieldType(string specificationFieldTypeId)
        {
            throw new NotImplementedException();
        }

        public SpecificationFieldType UpdateSpecificationFieldType(SpecificationFieldType specificationFieldType)
        {
            throw new NotImplementedException();
        }

        public void EnableSpecificationTemplateFieldType(bool enabled, int specificationTemplateId, string specificationFieldId)
        {
            throw new NotImplementedException();
        }

        public bool GetIsEnabledSpecificationTemplateFieldType(int specificationTemplateId, string specificationFieldId)
        {
            throw new NotImplementedException();
        }

        public string GetFormattedValue(SpecificationFieldType specificationFieldType, int entityId)
        {
            throw new NotImplementedException();
        }

        public string GetFormattedValueWithCultureInfo(SpecificationFieldType specificationFieldType, int entityId, CultureInfo ci)
        {
            throw new NotImplementedException();
        }

        public void CopySpecificationFields(int sourceEntityId, List<int> targetEntityIds)
        {
            throw new NotImplementedException();
        }

        public List<Category> GetAllSpecificationCategories()
        {
            throw new NotImplementedException();
        }

        public Category GetSpecificationCategory(string id)
        {
            throw new NotImplementedException();
        }

        public Category AddSpecificationCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Category UpdateSpecificationCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSpecificationCategory(string id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllSpecificationCategories()
        {
            throw new NotImplementedException();
        }

        public string GetSpecificationAsHtml(int specificationEntityId, int entityId, CultureInfo ci)
        {
            throw new NotImplementedException();
        }

        public List<SpecificationFieldType> GetAllSpecificationFieldTypesForCategory(string categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
