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

namespace StefanOlsen.InRiver.MappedImporter.Tests.Fakes
{
    public class FakeModelService : IModelService
    {
        private readonly Dictionary<string, CVL> _cvls;
        private readonly Dictionary<string, List<CVLValue>> _cvlValues;
        private readonly Dictionary<string, EntityType> _entityTypes;
        private readonly Dictionary<string, FieldSet> _fieldSets;
        private readonly Dictionary<string, LinkType> _linkTypes;

        public FakeModelService()
        {
            _cvls = new Dictionary<string, CVL>();
            _cvlValues = new Dictionary<string, List<CVLValue>>();
            _entityTypes = new Dictionary<string, EntityType>();
            _fieldSets = new Dictionary<string, FieldSet>();
            _linkTypes = new Dictionary<string, LinkType>();

            InitializeCvls();
            InitializeEntityTypes();
            InitializeFieldSets();
            InitializeLinkTypes();
        }

        private void InitializeCvls()
        {
            var brandCvl = new CVL
            {
                Id = "Brand",
                DataType = DataType.String
            };

            _cvls.Add(brandCvl.Id, brandCvl);
            _cvlValues.Add(brandCvl.Id, new List<CVLValue>());

            var mainCategoryCvl = new CVL
            {
                Id = "MainCategory",
                DataType = DataType.LocaleString
            };

            _cvls.Add(mainCategoryCvl.Id, mainCategoryCvl);
            _cvlValues.Add(mainCategoryCvl.Id, new List<CVLValue>());

            var subCategoryCvl = new CVL
            {
                Id = "SubCategory",
                DataType = DataType.LocaleString
            };

            _cvls.Add(subCategoryCvl.Id, subCategoryCvl);
            _cvlValues.Add(subCategoryCvl.Id, new List<CVLValue>());

            var marketCvl = new CVL
            {
                Id = "Market",
                DataType = DataType.LocaleString
            };

            _cvls.Add(marketCvl.Id, marketCvl);
            _cvlValues.Add(marketCvl.Id, new List<CVLValue>());

            var genderCvl = new CVL
            {
                Id = "Gender",
                DataType = DataType.LocaleString
            };

            _cvls.Add(genderCvl.Id, genderCvl);
            _cvlValues.Add(genderCvl.Id, new List<CVLValue>());

            var industryCvl = new CVL
            {
                Id = "Industry",
                DataType = DataType.LocaleString
            };

            _cvls.Add(industryCvl.Id, industryCvl);
            _cvlValues.Add(industryCvl.Id, new List<CVLValue>());

            var itemSeasonCvl = new CVL
            {
                Id = "ItemSeason",
                DataType = DataType.LocaleString
            };

            _cvls.Add(itemSeasonCvl.Id, itemSeasonCvl);
            _cvlValues.Add(itemSeasonCvl.Id, new List<CVLValue>());
        }

        private void InitializeEntityTypes()
        {
            var productEntityType = new EntityType
            {
                Id = "Product",
                FieldTypes = new List<FieldType>
                {
                    new FieldType {Id = "ProductName", DataType = DataType.LocaleString},
                    new FieldType {Id = "ProductNumber", DataType = DataType.String, Unique = true},
                    new FieldType {Id = "ProductBrand", DataType = DataType.CVL, CVLId = "Brand"},
                    new FieldType {Id = "ProductMainCategory", DataType = DataType.CVL, CVLId = "MainCategory"},
                    new FieldType {Id = "ProductSubCategory", DataType = DataType.CVL, CVLId = "SubCategory"},
                    new FieldType {Id = "ProductMarkets", DataType = DataType.CVL, CVLId = "Market"},
                    new FieldType {Id = "ProductFashionGender", DataType = DataType.CVL, CVLId = "Gender"},
                    new FieldType {Id = "ProductIndustry", DataType = DataType.CVL, CVLId = "Industry"},
                    new FieldType {Id = "ProductDevMode", DataType = DataType.Boolean}
                }
            };

            var itemEntityType = new EntityType
            {
                Id = "Item",
                FieldTypes = new List<FieldType>
                {
                    new FieldType {Id = "ItemName", DataType = DataType.LocaleString},
                    new FieldType {Id = "ItemNumber", DataType = DataType.String, Unique = true},
                    new FieldType {Id = "ItemUPCcode", DataType = DataType.String},
                    new FieldType {Id = "ItemIndustry", DataType = DataType.CVL},
                    new FieldType {Id = "ItemFashionSeason", DataType = DataType.CVL},
                    new FieldType {Id = "ItemDevMode", DataType = DataType.Boolean}
                }
            };

            _entityTypes.Add(productEntityType.Id, productEntityType);
            _entityTypes.Add(itemEntityType.Id, itemEntityType);
        }

        private void InitializeFieldSets()
        {
            _fieldSets.Add("DIYProduct", new FieldSet { Id = "DIYProduct" });
            _fieldSets.Add("FashionProduct", new FieldSet { Id = "FashionProduct" });
            _fieldSets.Add("FashionRetailComposition", new FieldSet { Id = "FashionRetailComposition" });
            _fieldSets.Add("Food", new FieldSet { Id = "Food" });
            _fieldSets.Add("FurnitureProduct", new FieldSet { Id = "FurnitureProduct" });
            _fieldSets.Add("ManufacturingProduct", new FieldSet { Id = "ManufacturingProduct" });
            _fieldSets.Add("RetailElectronicsProduct", new FieldSet { Id = "RetailElectronicsProduct" });
            _fieldSets.Add("DIYItem", new FieldSet { Id = "DIYItem" });
            _fieldSets.Add("DIYItem2", new FieldSet { Id = "DIYItem2" });
            _fieldSets.Add("FashionItem", new FieldSet { Id = "FashionItem" });
            _fieldSets.Add("ItemFood", new FieldSet { Id = "ItemFood" });
            _fieldSets.Add("ItemDIYLawnMower", new FieldSet { Id = "ItemDIYLawnMower" });
            _fieldSets.Add("ItemManufacturingSolarConnections", new FieldSet { Id = "ItemManufacturingSolarConnections" });
            _fieldSets.Add("ManufacturingWaterHeater", new FieldSet { Id = "ManufacturingWaterHeater" });
            _fieldSets.Add("RetailElectronicsItem", new FieldSet { Id = "RetailElectronicsItem" });
            _fieldSets.Add("WirelessHeadset", new FieldSet { Id = "WirelessHeadset" });
        }

        private void InitializeLinkTypes()
        {
            _linkTypes.Add("ProductItem",
                new LinkType
                {
                    Id = "ProductItem",
                    SourceEntityTypeId = "Product",
                    TargetEntityTypeId = "Item"
                });
        }

        public EntityType AddEntityType(EntityType entityType)
        {
            throw new NotImplementedException();
        }

        public bool DeleteEntityType(string id)
        {
            throw new NotImplementedException();
        }

        public List<EntityType> GetAllEntityTypes()
        {
            return _entityTypes.Values.ToList();
        }

        public bool DeleteAllEntityTypes()
        {
            throw new NotImplementedException();
        }

        public EntityType GetEntityType(string id)
        {
            return _entityTypes.TryGetValue(id, out EntityType entityType)
                ? entityType
                : null;
        }

        public EntityType UpdateEntityType(EntityType entityType)
        {
            throw new NotImplementedException();
        }

        public List<EntityTypeStatistics> GetAllEntityTypeStatistics()
        {
            throw new NotImplementedException();
        }

        public LinkType AddLinkType(LinkType linkType)
        {
            throw new NotImplementedException();
        }

        public LinkType GetLinkType(string id)
        {
            return _linkTypes.TryGetValue(id, out LinkType linkType)
                ? linkType
                : null;
        }

        public List<LinkType> GetAllLinkTypes()
        {
            throw new NotImplementedException();
        }

        public List<LinkType> GetLinkTypesForEntityType(string entityTypeId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllLinkTypes()
        {
            throw new NotImplementedException();
        }

        public bool DeleteLinkType(string id)
        {
            throw new NotImplementedException();
        }

        public LinkType UpdateLinkType(LinkType linkType)
        {
            throw new NotImplementedException();
        }

        public Category AddCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Category GetCategory(string id)
        {
            throw new NotImplementedException();
        }

        public List<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public bool DeleteCategory(string id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCategories()
        {
            throw new NotImplementedException();
        }

        public Category UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public List<Category> GetCategoriesForEntityType(string entityTypeId)
        {
            throw new NotImplementedException();
        }

        public CVL AddCVL(CVL cvl)
        {
            throw new NotImplementedException();
        }

        public CVL GetCVL(string id)
        {
            return _cvls.TryGetValue(id, out CVL cvl)
                ? cvl
                : null;
        }

        public List<CVL> GetAllCVLs()
        {
            return _cvls.Values.ToList();
        }

        public int GetCVLCount()
        {
            return _cvls.Count;
        }

        public bool DeleteCVL(string id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCVLs()
        {
            throw new NotImplementedException();
        }

        public CVL UpdateCVL(CVL cvl)
        {
            throw new NotImplementedException();
        }

        public CVLValue AddCVLValue(CVLValue cvlValue)
        {
            if (!_cvlValues.TryGetValue(cvlValue.CVLId, out List<CVLValue> cvlValues))
            {
                return null;
            }

            if (cvlValues.Any(cv => cv.Key == cvlValue.Key))
            {
                return null;
            }

            cvlValues.Add(cvlValue);

            return cvlValue;
        }

        public bool DeleteCVLValue(int id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCVLValues()
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCVLValuesForCVL(string cvlId)
        {
            throw new NotImplementedException();
        }

        public CVLValue GetCVLValue(int id)
        {
            throw new NotImplementedException();
        }

        public CVLValue GetCVLValueByKey(string key, string cvlId)
        {
            if (!_cvlValues.TryGetValue(cvlId, out List<CVLValue> cvlValues))
            {
                return null;
            }

            return cvlValues.FirstOrDefault(cv => cv.Key == key);
        }

        public List<CVLValue> GetCVLValuesForCVL(string cvlId, bool forceGet = false)
        {
            return _cvlValues.TryGetValue(cvlId, out List<CVLValue> cvlValues)
                ? cvlValues
                : null;
        }

        public CVLValue UpdateCVLValue(CVLValue cvlValue)
        {
            throw new NotImplementedException();
        }

        public List<CVLValue> GetAllCVLValues()
        {
            throw new NotImplementedException();
        }

        public string GetCVLValueForLanguage(string cvlId, string cvlKey, CultureInfo ci)
        {
            throw new NotImplementedException();
        }

        public FieldView AddFieldView(FieldView fieldview)
        {
            throw new NotImplementedException();
        }

        public FieldView GetFieldView(string id)
        {
            throw new NotImplementedException();
        }

        public List<FieldView> GetAllFieldViews()
        {
            throw new NotImplementedException();
        }

        public FieldView UpdateFieldView(FieldView fieldView)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFieldView(string id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllFieldViews()
        {
            throw new NotImplementedException();
        }

        public bool DeleteFieldTypeFromFieldView(string fieldViewId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public bool AddFieldTypeToFieldView(string fieldViewId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public List<FieldView> GetFieldViewsForEntityType(string entityTypeId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetFieldTypesForFieldView(string fieldViewId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetFieldViewsForFieldType(string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public FieldSet AddFieldSet(FieldSet fieldSet)
        {
            throw new NotImplementedException();
        }

        public FieldSet GetFieldSet(string id)
        {
            return _fieldSets.TryGetValue(id, out FieldSet fieldSet)
                ? fieldSet
                : null;
        }

        public List<FieldSet> GetAllFieldSets()
        {
            throw new NotImplementedException();
        }

        public FieldSet UpdateFieldSet(FieldSet fieldSet)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFieldSet(string id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllFieldSets()
        {
            throw new NotImplementedException();
        }

        public bool DeleteFieldTypeFromFieldSet(string fieldSetId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public bool AddFieldTypeToFieldSet(string fieldSetId, string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public List<FieldSet> GetFieldSetsForEntityType(string entityTypeId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetFieldTypesForFieldSet(string fieldSetId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetFieldSetsForFieldType(string fieldTypeId)
        {
            throw new NotImplementedException();
        }

        public FieldType AddFieldType(FieldType fieldType)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFieldType(string id)
        {
            throw new NotImplementedException();
        }

        public List<FieldType> GetAllFieldTypes()
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllFieldTypes()
        {
            throw new NotImplementedException();
        }

        public FieldType GetFieldType(string id)
        {
            throw new NotImplementedException();
        }

        public FieldType UpdateFieldType(FieldType fieldType)
        {
            throw new NotImplementedException();
        }

        public List<FieldType> GetFieldTypesForEntityTypeAndCategory(string entityTypeId, string categoryId)
        {
            throw new NotImplementedException();
        }

        public string ExportModelAsXmlString(bool includeCVLItems)
        {
            throw new NotImplementedException();
        }

        public bool ImportModelFromXmlString(string xml)
        {
            throw new NotImplementedException();
        }

        public CompletenessDefinition AddCompletenessDefinition(CompletenessDefinition definition)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCompletenessDefinitions()
        {
            throw new NotImplementedException();
        }

        public CompletenessDefinition UpdateCompletenessDefinition(CompletenessDefinition definition)
        {
            throw new NotImplementedException();
        }

        public CompletenessDefinition GetCompletenessDefinition(int id)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessDefinition> GetAllCompletenessDefinitions()
        {
            throw new NotImplementedException();
        }

        public bool DeleteCompletenessDefinition(int id)
        {
            throw new NotImplementedException();
        }

        public void ReCalculateCompletenessForDefinition(int definitionId)
        {
            throw new NotImplementedException();
        }

        public CompletenessGroup AddCompletenessGroup(CompletenessGroup group)
        {
            throw new NotImplementedException();
        }

        public CompletenessGroup UpdateCompletenessGroup(CompletenessGroup rule)
        {
            throw new NotImplementedException();
        }

        public CompletenessGroup GetCompletenessGroup(int id)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessGroup> GetCompletenessGroupForDefinition(int definitionId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCompletenessGroup(int id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCompletenessGroups()
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCompletenessGroupsForDefinition(int definitionId)
        {
            throw new NotImplementedException();
        }

        public CompletenessBusinessRule GetCompletenessBusinessRulesByGroupAndRule(int groupId, int ruleId)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessBusinessRule> GetCompletenessBusinessRulesForGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public CompletenessBusinessRule AddCompletenessBusinessRule(CompletenessBusinessRule rule)
        {
            throw new NotImplementedException();
        }

        public CompletenessBusinessRule ConnectExistingCompletenessBusinessRuleToNewGroup(CompletenessBusinessRule rule, int groupId)
        {
            throw new NotImplementedException();
        }

        public CompletenessBusinessRule UpdateCompletenessBusinessRuleForGroup(CompletenessBusinessRule rule, int groupId)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessBusinessRule> GetAllCompletenessBusinessRules()
        {
            throw new NotImplementedException();
        }

        public bool DeleteCompletenessBusinessRuleForGroup(int ruleId, int groupId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAllCompletenessBusinessRule()
        {
            throw new NotImplementedException();
        }

        public List<CompletenessCritera> GetAllCompletenessCriteras()
        {
            throw new NotImplementedException();
        }

        public CompletenessCritera GetCompletenessCriteraByType(string type)
        {
            throw new NotImplementedException();
        }

        public bool SetCompletenessBusinessRuleSettings(int ruleId, List<CompletenessRuleSetting> settings)
        {
            throw new NotImplementedException();
        }

        public CompletenessRuleSetting UpdatedCompletenessRuleSetting(int settingId, string value)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessAction> UpdateCompletenessActions(int definitionId, int ruleId, string trigger, List<CompletenessAction> actions)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessAction> GetCompletenessActionsByDefinitionIdRuleIdAndTrigger(int definitionid, int ruleid, string actiontrigger)
        {
            throw new NotImplementedException();
        }

        public List<CompletenessAction> GetCompletenessActionsByDefinitionIdGroupIdAndTrigger(int definitionid, int groupId, string actiontrigger)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCompletenessAction(int id)
        {
            throw new NotImplementedException();
        }

        public CompletenessAction GetCompletenessAction(int id)
        {
            throw new NotImplementedException();
        }

        public CompletenessAction AddCompletenessAction(CompletenessAction action)
        {
            throw new NotImplementedException();
        }

        public CompletenessAction UpdateCompletenessAction(CompletenessAction action)
        {
            throw new NotImplementedException();
        }

        public EnvironmentLatestChangesSince GetEnvironmentLatestChanges(DateTime sinceUtcDt)
        {
            throw new NotImplementedException();
        }
    }
}
