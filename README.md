# StefanOlsen.InRiver.MappedImporter

## Overview
This is an inbound connector for InRiver iPMC, developed to be generic, simple, configurable and efficient. For an introduction to the connector [read my blog post here](https://stefanolsen.com/posts/an-efficient-and-open-sourced-inbound-connector-for-inriver-ipmc/).

## Installation
Download a release package from this repository and upload it to iPMC. 

Alternatively, download this repository and build the solution in Release mode. Add all of the compiled assembly files to a zip file and upload it to iPMC.

On setting up the inbound extension make sure that the selected Extension Type is "InboundDataExtension".

On the settings page for this extension, there is only one settings string: "MAPPING\_CONFIGURATION\_XML". The value of this setting should be the whole mapping document string.

## Configuration
All entities, fields, links, field sets and languages used in this inbound connector have to be configured or mapped in a mapping XML document. An example of such a mapping document can be found [here](https://github.com/stefanolsen/inriver-mapped-importer/blob/master/StefanOlsen.InRiver.MappedImporter.Tests/FieldMapping.xml).

### Namespaces
For the XML parser to work, all XML namespaces in the input document must be mapped. 

__Example:__

```
<XmlNamespaces>
  <Namespace Prefix="" Uri="http://schemas.stefanolsen.com/inRiver/TestCatalog"/>
  <Namespace Prefix="ns" Uri="http://schemas.stefanolsen.com/inRiver/TestCatalog"/>
</XmlNamespaces>
```

__Properties:__

| Attribute | Required | Notes
|:----------|:---------|:------------
| Prefix    | Yes      | Prefix used in document and XPath expression.
| Uri       | Yes      | Full namespace URI.

### Languages
All languages to be supported by this connector need to be mapped in this list of languages.

__Example:__

```
<Languages>
  <Language Original="da" InRiver="da"/>
  <Language Original="en" InRiver="en"/>
</Languages>
```

__Properties:__

| Attribute | Required | Notes
|:----------|:---------|:------------
| Original  | Yes      | Language code of XML elements.
| InRiver   | Yes      | Language code in InRiver.

### Entities
All entities need to be mapped for them to be imported.

The mapping document root can contain more than one entity type, but the parser will import all available entities of an entity type before moving on to the next.

An entity mapping can also contain child entity types, again importing all instances of that entity type before moving on to the next one. 

However, by mapping child entities, like items and SKU's, as child entity types, all child entities will be imported for a given product before the importer moves on to the next product (and it's child entities).

This can make a very big difference, both for performance and for importing items, SKU's and links together with their products.

__Example:__

```
<Entity EntityType="Product"
        Root="/ns:catalog-export/ns:product" 
        UniqueFieldType="ProductNumber">
  <Fields>
  ...
  </Fields>
  <FieldSets>
  ...
  </FieldSets>
  <Entity>
  ...
  </Entity>
  <ParentLinks>
  ...
  </ParentLinks>
  <ChildLinks>
  ...
  </ChildLinks>
</Entity>
```

__Properties:__

| Attribute       | Required | Notes
|:----------------|:---------|:------------
| EntityType      | Yes      | Name of the field in the business model
| Root            | Yes      | XPath to the logical root of the data for this entity. Relative to the parent entity's root (or to the document root).
| UniqueFieldType | Yes      | Name of a unique field on the entity in the business model. Used for look-up and matching.

### Fields
All field need to be mapped for them to be imported.

Available field types:

* Boolean field
* DateTime field
* Integer field
* CVL field
* String field
* Locale String field
* SKU field

__Example:__

```
<BooleanField FieldType="IsSearchable" ElementPath="ns:is-searchable"/>
<IntegerField FieldType="MinimumAmount" ElementPath="ns:minimum-amount"/>
<DateTimeField FieldType="AvailableFrom" ElementPath="ns:availability/ns:from"/>
<StringField FieldType="ProductNumber" ElementPath="./@id"/>
<LocaleStringField FieldType="ProductName" ElementPath="ns:product-name"/>
<CvlField FieldType="ProductBrand" ElementPath="ns:brand" Cvl="Brand" AddValues="true" Multivalue="false" Separator=";"/>
<SKUField KeyAttribute="ean">
  <SKUElement Name="size" ElementPath="ns:size"/>
</SKUField>
```

__Properties:__

Field mappings:

| Attribute    | Required | Applicable to   | Notes
|:-------------|:---------|:----------------|:------------
| FieldType    | Yes      | All fields      | Name of the field in the business model
| ElementPath  | Yes      | All fields      | XPath to the value of an attribute or an element. Relative to the entity's root.
| Cvl          | Yes      | CvlField        | Name of CVL to use.
| AddValues    | No      | CvlField         | True if unknown CVL values should be added automatically.
| Multivalue   | No      | CvlField         | True if the CVL field supports multiple values.
| Separator    | No      | CvlField         | One character to be used as a separator when the parsed value can contain multiple values.
| KeyAttribute | Yes      | SKUField        | Name of XML attribute to be used as the key in SKU elements.

SKU element mapping:

| Attribute    | Required | Notes
|:-------------|:---------|:------------
| Name         | Yes      | Name of an XML element to created in the SKU document.
| ElementPath  | Yes      | XPath to the value of an attribute or an element. Relative to the entity's root.

### Field sets
One field set mapping can be mapped per entity.

To assign a field set, the parser will look up an XPath expression and look that parsed value up in a list of known values and their related field set names.

__Example:__

```
<FieldSets XPath="ns:industry">
  <FieldSet FieldSetName="DIYProduct" Value="diy"/>
  <FieldSet FieldSetName="FashionProduct" Value="fashionretail"/>
  <FieldSet FieldSetName="FashionRetailComposition" Value="fashionretail"/>
  <FieldSet FieldSetName="Food" Value="food"/>
  <FieldSet FieldSetName="FurnitureProduct" Value="furniture"/>
  <FieldSet FieldSetName="ManufacturingProduct" Value="manufacturing"/>
  <FieldSet FieldSetName="RetailElectronicsProduct" Value="fashionretail"/>
</FieldSets>
```
__Properties:__

FieldSets:

| Attribute    | Required | Notes
|:-------------|:---------|:------------
| XPath        | Yes      | XPath to the value of an attribute or an element. Relative to the entity's root.

FieldSet:

| Attribute    | Required | Notes
|:-------------|:---------|:------------
| FieldSetName | Yes      | Name of a field set from the business model.
| FieldValue   | Yes      | String value that, if matched, assigns the field set name to the current entity.

### Parent links

To add links from a child entity to a parent entity, add a mapping for that link inside the child entity element.

__Example:__

```
<ParentLinks>
  <Link LinkType="ProductItem"
        SourcePath="string(../@id)" 
        TargetUniqueFieldType="ProductNumber"/>
</ParentLinks>
```

__Properties:__

| Attribute             | Required | Notes
|:----------------------|:---------|:------------
| LinkType              | Yes      | Name of a link type to use for this link.
| SourcePath            | Yes      | XPath to the value of an attribute or an element. Relative to the entity's root. The value must be a unique value identifying the other entity.
| TargetUniqueFieldType | Yes      | Name of a unique field on the target entity type. Used for looking up the related entity.

### Child links

To add links from a parent entity to a child entity, add a mapping for that link inside the parent entity element.

__Example:__

```
<ChildLinks>
  <Link LinkType="ProductItem"
        SourcePath="string(../@id)" 
        TargetUniqueFieldType="ProductNumber"/>
</ChildLinks>
```

__Properties:__

| Attribute             | Required | Notes
|:----------------------|:---------|:------------
| LinkType              | Yes      | Name of a link type to use for this link.
| SourcePath            | Yes      | XPath to the value of an attribute or an element. Relative to the entity's root. The value must be a unique value identifying the other entity.
| TargetUniqueFieldType | Yes      | Name of a unique field on the target entity type. Used for looking up the related entity.

## Extending
Extending the solution with a new field parsers is relatively easy to do.
It could be that a field needs to be set to a substring of a value. Or it might need to be a static value. Or something else.
Either way, creating a new field parser involves this:

1. Add a new element to the XML Schema file, `ImportMapping.xsd`.
2. Update the autogenerated file, `ImportMapping.designer.cs` using a tool like xsd2code.
3. Create a new class, implementing the interface `IFieldParser` interface.
4. Register the new field parser to the new mapping element, by adding a new case to `FieldParserFactory.CreateFieldParser(Type fieldType)`.
5. Add a new field mapping in your mapping document, using the new field mapping type.