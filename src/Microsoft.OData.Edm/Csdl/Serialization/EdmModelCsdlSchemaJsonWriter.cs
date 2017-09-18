//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;
using Newtonsoft.Json;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal class EdmModelCsdlSchemaJsonWriter : EdmModelCsdlSchemaWriter
    {
        protected readonly JsonWriter JsonWriter;

        public EdmModelCsdlSchemaJsonWriter(IEdmModel model, JsonWriter jsonWriter, Version edmVersion)
            : base(model, edmVersion)
        {
            JsonWriter = jsonWriter;
        }

        internal override void WriteReferencesStart(IEnumerable<IEdmReference> references)
        {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Element_Reference); // $Reference:{
            JsonWriter.WriteStartObject();
        }

        internal override void WriteReferencesEnd(IEnumerable<IEdmReference> references)
        {
            JsonWriter.WriteEndObject(); // }
        }

        internal override void WriteReferenceIncludesStart(IEnumerable<IEdmInclude> includes)
        {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Element_Include); // $Include:[
            JsonWriter.WriteStartArray();
        }

        internal override void WriteReferenceIncludesEnd(IEnumerable<IEdmInclude> includes)
        {
            JsonWriter.WriteEndArray();
        }

        internal override void WriteReferenceIncludeAnnotationsStart(IEnumerable<IEdmIncludeAnnotations> annotations)
        {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Element_IncludeAnnotations); // $IncludeAnnotations:[
            JsonWriter.WriteStartArray(); // [
        }

        internal override void WriteReferenceIncludeAnnotationsEnd(IEnumerable<IEdmIncludeAnnotations> annotations)
        {
            JsonWriter.WriteEndArray(); // ]
        }

        internal override void WriteReferenceElementStart(IEdmReference reference)
        {
            JsonWriter.WritePropertyName(reference.Uri.ToString()); // "http://vocabs.odata.org/capabilities/v1" :{
            JsonWriter.WriteStartObject();
        }

        internal override void WriteReferenceElementEnd(IEdmReference reference)
        {
            JsonWriter.WriteEndObject(); // }
        }

        internal override void WriteIncludeElement(IEdmInclude include)
        {
            JsonWriter.WriteStartObject(); // {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_Namespace);
            JsonWriter.WriteValue(include.Namespace);
            JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_Alias);
            JsonWriter.WriteValue(include.Alias);
            JsonWriter.WriteEndObject(); // }
        }

        internal override void WriteIncludeAnnotationsElement(IEdmIncludeAnnotations includeAnnotations)
        {
            JsonWriter.WriteStartObject(); // {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_TermNamespace);
            JsonWriter.WriteValue(includeAnnotations.TermNamespace);

            if (!String.IsNullOrWhiteSpace(includeAnnotations.TargetNamespace))
            {
                JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_TargetNamespace);
                JsonWriter.WriteValue(includeAnnotations.TargetNamespace);
            }

            if (!String.IsNullOrWhiteSpace(includeAnnotations.Qualifier))
            {
                JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_Qualifier);
                JsonWriter.WriteValue(includeAnnotations.Qualifier);
            }

            JsonWriter.WriteEndObject(); // }
        }

        internal override void WriteTermElementHeader(IEdmTerm term, bool inlineType)
        {
            // A term is represented as a member of the schema object
            // whose name is the unqualified name of the term and whose value is an object
            JsonWriter.WritePropertyName(term.Name);
            JsonWriter.WriteStartObject();

            // The term object MUST contain the member $Kind with a string value of Term.
            WriteProperty(CsdlConstants.JsonKind, CsdlConstants.Element_Term);

            /*
            if (inlineType && term.Type != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, term.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, term.DefaultValue, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_AppliesTo, term.AppliesTo, EdmValueWriter.StringAsXml);*/
        }

        internal override void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            // A complex type is represented as a member of the schema object:
            // whose name is the unqualified name of the complex type
            // whose value is an object.
            JsonWriter.WritePropertyName(complexType.Name);
            JsonWriter.WriteStartObject();

            // The complex type object MUST contain the member $Kind with a string value of ComplexType.
            WriteProperty(CsdlConstants.JsonKind, CsdlConstants.Element_ComplexType);

            // It MAY contain the members $BaseType, $Abstract, $OpenType.
            IEdmComplexType baseType = complexType.BaseComplexType();
            if (baseType != null)
            {
                WriteProperty("$" + CsdlConstants.Attribute_BaseType, baseType.FullTypeName());
            }

            if (complexType.IsAbstract)
            {
                WriteProperty("$" + CsdlConstants.Attribute_Abstract, true);
            }

            if (complexType.IsOpen)
            {
                WriteProperty("$" + CsdlConstants.Attribute_OpenType, true);
            }
        }

        internal override void WriteComplexTypeElementEnd(IEdmComplexType complexType)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteEnumTypeElementHeader(IEdmEnumType enumType)
        {
            // An enumeration type is represented as a member of the schema object
            // whose name is the unqualified name of the enumeration type
            // whose value is an object.
            JsonWriter.WritePropertyName(enumType.Name);
            JsonWriter.WriteStartObject();

            // The enumeration type object MUST contain the member $Kind with a string value of EnumType
            WriteProperty(CsdlConstants.JsonKind, CsdlConstants.Element_EnumType);

            // It MAY contain the members $UnderlyingType.
            // An enumeration type MAY specify one of Edm.Byte, Edm.SByte, Edm.Int16, Edm.Int32, or Edm.Int64 as its underlying type. 
            // If not explicitly specified, Edm.Int32 is used as the underlying type.
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                WriteProperty(CsdlConstants.JsonUnderlyingType, TypeDefinitionAsXml(enumType.UnderlyingType));
            }

            // It MAY contain the members $IsFlags.
            // The value of $IsFlags is one of the Boolean literals true or false. Absence of the member means false
            if (enumType.IsFlags != CsdlConstants.Default_IsFlags)
            {
                WriteProperty("$" + CsdlConstants.Attribute_IsFlags, enumType.IsFlags);
            }
        }

        internal override void WriteEnumTypeElementEnd(IEdmEnumType enumType)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteEntityContainerElementStart(IEdmEntityContainer container)
        {
            JsonWriter.WritePropertyName(container.Name);
            JsonWriter.WriteStartObject(); // {

            JsonWriter.WritePropertyName(CsdlConstants.JsonKind);
            JsonWriter.WriteValue(CsdlConstants.Element_EntityContainer);
        }

        internal override void WriteEntityContainerElementEnd(IEdmEntityContainer container)
        {
            JsonWriter.WriteEndObject(); // }
        }

        internal override void WriteEntitySetElementStart(IEdmEntitySet entitySet)
        {
            JsonWriter.WritePropertyName(entitySet.Name);
            JsonWriter.WriteStartObject();

            JsonWriter.WritePropertyName(CsdlConstants.JsonKind);
            JsonWriter.WriteValue(CsdlConstants.Element_EntitySet);

            JsonWriter.WritePropertyName(CsdlConstants.JsonType);
            JsonWriter.WriteValue(entitySet.EntityType().FullName());
        }

        internal override void WriteEntitySetElementEnd(IEdmEntitySet entitySet)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteSingletonElementStart(IEdmSingleton singleton)
        {
            JsonWriter.WritePropertyName(singleton.Name);
            JsonWriter.WriteStartObject();

            JsonWriter.WritePropertyName(CsdlConstants.JsonKind);
            JsonWriter.WriteValue(CsdlConstants.Element_Singleton);

            JsonWriter.WritePropertyName(CsdlConstants.JsonType);
            JsonWriter.WriteValue(singleton.EntityType().FullName());
        }

        internal override void WriteSingletonElementEnd(IEdmSingleton singleton)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteNavigationPropertyBinding(IEdmNavigationSource navigationSource,
            IEdmNavigationPropertyBinding binding)
        {

        }

        internal override void WriteEntityTypeElementHeader(IEdmEntityType entityType)
        {
            // An entity type is represented as a member of the schema object:
            // whose name is the unqualified name of the entity type
            // whose value is an object.
            JsonWriter.WritePropertyName(entityType.Name);
            JsonWriter.WriteStartObject();

            // The entity type object MUST contain the member $Kind with a string value of EntityType.
            WriteProperty(CsdlConstants.JsonKind, "EntityType");

            // It MAY contain the members $BaseType, $Abstract, $OpenType, $HasStream, and $Key.
            IEdmEntityType baseType = entityType.BaseEntityType();
            if (baseType != null)
            {
                WriteProperty("$" + CsdlConstants.Attribute_BaseType, baseType.FullTypeName());
            }

            if (entityType.IsAbstract)
            {
                WriteProperty("$" + CsdlConstants.Attribute_Abstract, true);
            }

            if (entityType.IsOpen)
            {
                WriteProperty("$" + CsdlConstants.Attribute_OpenType, true);
            }

            // HasStream value should be inherited.  Only have it on base type is sufficient.
            bool writeHasStream = entityType.HasStream &&
                                  (entityType.BaseEntityType() == null ||
                                   (entityType.BaseEntityType() != null && !entityType.BaseEntityType().HasStream));
            if (writeHasStream)
            {
                WriteProperty("$" + CsdlConstants.Attribute_HasStream, true);
            }
        }

        internal override void WriteEntityTypeElementEnd(IEdmEntityType entityType)
        {
            JsonWriter.WriteEndObject();
        }

        private void WriteProperty(string propertyName, object propertyValue)
        {
            JsonWriter.WritePropertyName(propertyName);
            JsonWriter.WriteValue(propertyValue);
        }

        internal override void WriteDelaredKeyPropertiesElementHeader()
        {
            JsonWriter.WritePropertyName("$" + CsdlConstants.Element_Key);
            JsonWriter.WriteStartArray(); // [
        }

        internal override void WriteDelaredKeyPropertiesElementEnd()
        {
            JsonWriter.WriteEndArray(); // ]
        }

        internal override void WritePropertyRefElement(IEdmStructuralProperty property)
        {
            JsonWriter.WriteValue(property.Name);
        }

        internal override void WriteNavigationPropertyElementHeader(IEdmNavigationProperty member)
        {
            // Navigation properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            JsonWriter.WritePropertyName(member.Name);
            JsonWriter.WriteStartObject();

            // The navigation property object MUST contain the member $Kind with a string value of NavigationProperty.
            WriteProperty(CsdlConstants.JsonKind, CsdlConstants.Element_NavigationProperty);

            if (member.Type.IsCollection())
            {
                // For collection-valued navigation properties:
                // the value of $Type is the qualified name of the navigation property’s item type, 
                // the member $Collection MUST be present with the literal value true.
                WriteProperty(CsdlConstants.JsonCollection, true);
                WriteProperty(CsdlConstants.JsonType, TypeReferenceAsXml(member.Type.AsCollection().ElementType()));

                // Nullable MUST NOT be specified for a collection-valued navigation property
            }
            else
            {
                // For single-valued navigation properties the value of $Type is the qualified name of the navigation property’s type.
                WriteProperty(CsdlConstants.JsonType, TypeReferenceAsXml(member.Type));

                // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means true.
                if (member.Type.IsNullable != CsdlConstants.Default_Nullable)
                {
                    WriteProperty("$" + CsdlConstants.Attribute_Nullable, member.Type.IsNullable);
                }
            }

            // The value of $Partner is a string containing the path to the partner navigation property.
            if (member.Partner != null)
            {
                WriteProperty("$" + CsdlConstants.Attribute_Partner, EdmValueWriter.StringAsXml(member.GetPartnerPath().Path));
            }

            // The value of $ContainsTarget is one of the Boolean literals true or false.Absence of the member means false.
            if (member.ContainsTarget != CsdlConstants.Default_ContainsTarget)
            {
                WriteProperty("$" + CsdlConstants.Attribute_ContainsTarget, member.ContainsTarget);
            }
        }

        internal override void WriteNavigationPropertyElementEnd(IEdmNavigationProperty member)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteOperationActionElement(string elementName, EdmOnDeleteAction operationAction)
        {
        }

        internal override void WriteSchemaElementStart(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            JsonWriter.WritePropertyName(schema.Namespace); // "org.example": {
            JsonWriter.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(alias))
            {
                JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_Alias);
                JsonWriter.WriteValue(alias);
            }

            if (mappings != null)
            {
                // foreach (KeyValuePair<string, string> mapping in mappings)
                // {
                //     this.JsonWriter.WriteAttributeString(EdmConstants.XmlNamespacePrefix, mapping.Key, null, mapping.Value);
                // }
            }
        }

        internal override void WriteSchemaElementEnd(EdmSchema schema, string aliass)
        {
             JsonWriter.WriteEndObject(); // End of Schema object
        }

        internal override void WriteAnnotationsElementHeader(string annotationsTarget)
        {
            
        }

        internal override void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType)
        {
            // Structural properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            JsonWriter.WritePropertyName(property.Name);
            JsonWriter.WriteStartObject();

            // The property object MAY contain the member $Kind with a string value of Property.
            WriteProperty(CsdlConstants.JsonKind, CsdlConstants.Element_Property);

            IEdmTypeReference propertyType = property.Type;
            if (property.Type.IsCollection())
            {
                WriteProperty(CsdlConstants.JsonCollection, true);
                propertyType = property.Type.AsCollection().ElementType();
            }

            // Absence of the $Type member means the type is Edm.String.
            if (!propertyType.IsString())
            {
                WriteProperty(CsdlConstants.JsonType, propertyType.FullName());
            }
        }

        internal override void WriteEnumMemberElementHeader(IEdmEnumMember member)
        {
            JsonWriter.WritePropertyName(member.Name);
            JsonWriter.WriteValue(member.Value.Value);
        }

        internal override void WriteEnumMemberElementEnd(IEdmEnumMember member)
        {
            // Nothing here
        }

        internal override void WriteNullableAttribute(IEdmTypeReference reference)
        {
            if (reference.IsNullable != CsdlConstants.Default_Nullable)
            {
                WriteProperty("$" + CsdlConstants.Attribute_Nullable, reference.IsNullable);
            }
        }

        internal void WriteOptionalProperty<T>(string name, T value)
        {
            if (value != null)
            {
                WriteProperty(name, value);
            }
        }

        internal void WriteOptionalProperty<T>(string name, T value, T defaultValue)
        {
            if (!value.Equals(defaultValue))
            {
                WriteProperty(name, value);
            }
        }

        internal override void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                // CSDLXML defines a symbolic value max that is only allowed in OData 4.0 responses.
                // This symbolic value is not allowed in CDSL JSON documents at all
            }
            else
            {
                WriteOptionalProperty("$" + CsdlConstants.Attribute_MaxLength, reference.MaxLength);
            }
        }

        internal override void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            WriteOptionalProperty("$" + CsdlConstants.Attribute_Precision, reference.Precision);
            WriteOptionalProperty("$" + CsdlConstants.Attribute_Scale, reference.Scale);
        }

        internal override void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
        {
            if (reference.IsGeography())
            {
                WriteOptionalProperty("$" + CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeographySrid);
            }
            else if (reference.IsGeometry())
            {
                WriteOptionalProperty("$" + CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeometrySrid);
            }
        }

        internal override void WriteStringTypeAttributes(IEdmStringTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                // CSDLXML defines a symbolic value max that is only allowed in OData 4.0 responses.
                // This symbolic value is not allowed in CDSL JSON documents at all
            }
            else
            {
                WriteOptionalProperty("$" + CsdlConstants.Attribute_MaxLength, reference.MaxLength);
            }

            if (reference.IsUnicode != null)
            {
                WriteOptionalProperty("$" + CsdlConstants.Attribute_Unicode, reference.IsUnicode, CsdlConstants.Default_IsUnicode);
            }
        }

        internal override void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                WriteOptionalProperty(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision);
            }
        }

        internal override void WriteReferentialConstraintElementHeader(IEdmNavigationProperty constraint)
        {
            
        }

        internal override void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair)
        {
            
        }

        internal override void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation)
        {
            
        }

        internal override void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation)
        {
            
        }

        internal override void WriteActionElementHeader(IEdmAction action)
        {
            
        }

        internal override void WriteFunctionElementHeader(IEdmFunction function)
        {
            
        }

        internal override void WriteReturnTypeElementHeader()
        {
            
        }

        internal override void WriteTypeAttribute(IEdmTypeReference typeReference)
        {
            
        }

        internal override void WriteActionImportElementHeader(IEdmActionImport actionImport)
        {
            WriteOperationImportAttributes(actionImport, CsdlConstants.Element_ActionImport, CsdlConstants.Attribute_Action);
        }

        internal override void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
        {
            WriteOperationImportAttributes(functionImport, CsdlConstants.Element_FunctionImport, CsdlConstants.Attribute_Function);

            if (functionImport.IncludeInServiceDocument)
            {
                JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_IncludeInServiceDocument);
                JsonWriter.WriteValue(true);
            }
        }

        internal override void WriteActionImportElementEnd(IEdmActionImport actionImport)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteFunctionImportElementEnd(IEdmFunctionImport functionImport)
        {
            JsonWriter.WriteEndObject();
        }

        private void WriteOperationImportAttributes(IEdmOperationImport operationImport, string kindName, string operationName)
        {
            JsonWriter.WritePropertyName(operationImport.Name);
            JsonWriter.WriteStartObject();

            JsonWriter.WritePropertyName(CsdlConstants.JsonKind);
            JsonWriter.WriteValue("$" + kindName);

            JsonWriter.WritePropertyName("$" + operationName);
            JsonWriter.WriteValue(operationImport.Operation.FullName());

            if (operationImport.EntitySet != null)
            {
                var pathExpression = operationImport.EntitySet as IEdmPathExpression;
                if (pathExpression != null)
                {
                    JsonWriter.WritePropertyName("$" + CsdlConstants.Attribute_EntitySet);
                    JsonWriter.WriteValue(PathAsXml(pathExpression.PathSegments));
                }
                else
                {
                    throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                }
            }
        }

        internal override void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType)
        {
            
        }

        internal override void WriteOperationParameterEndElement(IEdmOperationParameter parameter)
        {
            
        }

        internal override void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            
        }

        internal override void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline)
        {
        }

        internal override void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline)
        {
            
        }

        internal override void WriteRecordExpressionElementHeader(IEdmRecordExpression expression)
        {
            
        }

        internal override void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline)
        {
            
        }

        internal override void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression)
        {
            
        }

        internal override void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression)
        {
            
        }

        internal override void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression)
        {
            
        }

        internal override void WriteNullConstantExpressionElement(IEdmNullExpression expression)
        {
            
        }

        internal override void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression)
        {
            
        }

        internal override void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression)
        {
            
        }

        internal override void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression)
        {
            
        }

        internal override void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression)
        {
            
        }

        internal override void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression)
        {
            
        }

        internal override void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression)
        {
        }

        internal override void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression)
        {
        }

        internal override void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression)
        {
        }

        internal override void WritePathExpressionElement(IEdmPathExpression expression)
        {
        }

        internal override void WritePropertyPathExpressionElement(IEdmPathExpression expression)
        {
        }

        internal override void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression)
        {
        }

        internal override void WriteIfExpressionElementHeader(IEdmIfExpression expression)
        {
        }

        internal override void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression)
        {
        }

        internal override void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement)
        {
        }

        internal override void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression)
        {
        }

        internal override void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType)
        {
        }

        internal override void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType)
        {
        }

        internal override void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression)
        {
        }

        internal override void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition)
        {
            // A type definition is represented as a member of the schema object
            // whose name is the unqualified name of the type definition and whose value is an object
            JsonWriter.WritePropertyName(typeDefinition.Name);
            JsonWriter.WriteStartObject();

            // The type definition object MUST contain the member $Kind with a string value of TypeDefinition
            WriteProperty(CsdlConstants.JsonKind, CsdlConstants.Element_TypeDefinition);

            // The type definition object MUST contain the member $UnderlyingType
            WriteProperty(CsdlConstants.JsonUnderlyingType, TypeDefinitionAsXml(typeDefinition.UnderlyingType));
        }

        internal override void WriteEndElement()
        {
             JsonWriter.WriteEndObject();
        }

        internal override void WriteEndElements()
        {
            JsonWriter.WriteEndArray();
        }

        internal override void WriteInlineExpression(IEdmExpression expression)
        {
        }

#if false
        internal override void WriteReferenceStart(IEnumerable<IEdmReference> references)
        {
            if (references.Any())
            {
                JsonWriter.WritePropertyName("$Reference");
                JsonWriter.WriteStartObject();
            }
        }

        internal override void WriteReferenceEnd(IEnumerable<IEdmReference> references)
        {
            if (references.Any())
            {
                JsonWriter.WriteEndObject();
            }
        }

        internal override void WriteReferenceElementStart(IEdmReference reference)
        {
            JsonWriter.WritePropertyName(reference.Uri.ToString());
            JsonWriter.WriteStartObject();
        }

        internal override void WriteReferenceElementEnd(IEdmReference reference)
        {
            JsonWriter.WriteEndObject();
        }

        internal override void WriteReferenceElementHeader(IEdmReference reference)
        {
            JsonWriter.WritePropertyName("$Reference");
            JsonWriter.WriteStartObject();
        }

        internal override void WriteReferenceIncludes(IEnumerable<IEdmInclude> includes)
        {
            var edmIncludes = includes.ToList();
            if (edmIncludes.Any())
            {
                JsonWriter.WritePropertyName("$Include");
                JsonWriter.WriteStartArray(); // [

                foreach (var edmInclude in edmIncludes)
                {
                    JsonWriter.WriteStartObject(); // {
                    JsonWriter.WritePropertyName("$Namespace");
                    JsonWriter.WriteValue(edmInclude.Namespace);
                    JsonWriter.WritePropertyName("$Alias");
                    JsonWriter.WriteValue(edmInclude.Alias);
                    JsonWriter.WriteEndObject(); // }
                }

                JsonWriter.WriteEndArray();  // ]
            }
        }

        internal override void WriteIncludeStart(IEdmInclude include)
        {
            JsonWriter.WritePropertyName("$Include");
            JsonWriter.WriteStartArray(); // [

            JsonWriter.WriteStartObject(); // {
            JsonWriter.WritePropertyName("$Namespace");
            JsonWriter.WriteValue(include.Namespace);
            JsonWriter.WritePropertyName("Alias");
            JsonWriter.WriteValue(include.Alias);

            JsonWriter.WriteEndObject(); // }

            JsonWriter.WriteEndArray();  // ]
        }


    }

    internal interface ICsdlSchemaReferenceWriter
    {
        void WriteReferences(IEnumerable<IEdmReference> references);
    }

    internal class CsdlSchemaREferenceXmlWirter : ICsdlSchemaReferenceWriter
    {
        private readonly XmlWriter _xmlWriter;
        private readonly IEdmModel _model;

        public CsdlSchemaREferenceXmlWirter(IEdmModel model, XmlWriter xmlWriter)
        {
            _xmlWriter = xmlWriter;
            _model = model;
        }

        public void WriteReferences(IEnumerable<IEdmReference> references)
        {
        }
    }

    internal class CsdlSchemaREferenceJsonWirter : ICsdlSchemaReferenceWriter
    {
        private readonly JsonWriter _jsonWriter;
        private readonly IEdmModel _model;

        public CsdlSchemaREferenceJsonWirter(IEdmModel model, JsonWriter jsonWriter)
        {
            _jsonWriter = jsonWriter;
            _model = model;
        }

        public void WriteReferences(IEnumerable<IEdmReference> references)
        {
            var edmReferences = references as IList<IEdmReference> ?? references.ToList();
            if (!edmReferences.Any())
            {
                return;
            }

            _jsonWriter.WritePropertyName("$Reference");
            _jsonWriter.WriteStartObject();

            foreach (IEdmReference tmp in edmReferences)
            {
                _jsonWriter.WritePropertyName(tmp.Uri.ToString());
                _jsonWriter.WriteStartObject();

                WriteIncludes(tmp.Includes);
                WriteIncludeAnnotations(tmp.IncludeAnnotations);

                _jsonWriter.WriteEndObject();
            }

            _jsonWriter.WriteEndObject();
        }

        private void WriteIncludes(IEnumerable<IEdmInclude> includes)
        {
            if (includes == null)
            {
                return;
            }

            var edmIncludes = includes.ToList();
            if (edmIncludes.Any())
            {
                _jsonWriter.WritePropertyName("$Include");
                _jsonWriter.WriteStartArray(); // [

                foreach (var edmInclude in edmIncludes)
                {
                    _jsonWriter.WriteStartObject(); // {
                    _jsonWriter.WritePropertyName("$Namespace");
                    _jsonWriter.WriteValue(edmInclude.Namespace);
                    _jsonWriter.WritePropertyName("$Alias");
                    _jsonWriter.WriteValue(edmInclude.Alias);
                    _jsonWriter.WriteEndObject(); // }
                }

                _jsonWriter.WriteEndArray();  // ]
            }
        }

        private void WriteIncludeAnnotations(IEnumerable<IEdmIncludeAnnotations> annotations)
        {
            if (annotations == null)
            {
                return;
            }

            var edmAnnotations = annotations.ToList();
            if (edmAnnotations.Any())
            {
                _jsonWriter.WritePropertyName("$IncludeAnnotations");
                _jsonWriter.WriteStartArray(); // [

                foreach (var edmAnnotation in edmAnnotations)
                {
                    _jsonWriter.WriteStartObject(); // {
                    _jsonWriter.WritePropertyName("$TermNamespace");
                    _jsonWriter.WriteValue(edmAnnotation.TermNamespace);

                    if (!String.IsNullOrWhiteSpace(edmAnnotation.TargetNamespace))
                    {
                        _jsonWriter.WritePropertyName("$TargetNamespace");
                        _jsonWriter.WriteValue(edmAnnotation.TargetNamespace);
                    }

                    if (!String.IsNullOrWhiteSpace(edmAnnotation.Qualifier))
                    {
                        _jsonWriter.WritePropertyName("$Qualifier");
                        _jsonWriter.WriteValue(edmAnnotation.Qualifier);
                    }

                    _jsonWriter.WriteEndObject(); // }
                }

                _jsonWriter.WriteEndArray();  // ]
            }
        }
#endif
    }

#if false
    internal abstract class EdmModelCsdlSchemaWriter
    {
        protected Version version;
        private readonly string edmxNamespace;
        private readonly VersioningDictionary<string, string> namespaceAliasMappings;
        private readonly IEdmModel model;

        internal EdmModelCsdlSchemaWriter(IEdmModel model, VersioningDictionary<string, string> namespaceAliasMappings, Version edmVersion)
        {
            this.version = edmVersion;
            this.edmxNamespace = CsdlConstants.SupportedEdmxVersions[edmVersion];
            this.model = model;
            this.namespaceAliasMappings = namespaceAliasMappings;
        }

        internal static string PathAsXml(IEnumerable<string> path)
        {
            return EdmUtil.JoinInternal("/", path);
        }

        internal abstract void WriteReferenceStart(IEnumerable<IEdmReference> references);

        internal abstract void WriteReferenceElementHeader(IEdmReference reference);

        internal abstract void WriteIncludeElement(IEdmInclude include);

        internal abstract void WriteReferenceEnd(IEnumerable<IEdmReference> references);

        internal void WriteIncludeAnnotationsElement(IEdmIncludeAnnotations includeAnnotations)
        {
            // e.g. <edmx:IncludeAnnotations ... />
            this.xmlWriter.WriteStartElement(CsdlConstants.Prefix_Edmx, CsdlConstants.Element_IncludeAnnotations, this.edmxNamespace);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_TermNamespace, includeAnnotations.TermNamespace, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, includeAnnotations.Qualifier, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_TargetNamespace, includeAnnotations.TargetNamespace, EdmValueWriter.StringAsXml);
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteTermElementHeader(IEdmTerm term, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Term);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, term.Name, EdmValueWriter.StringAsXml);
            if (inlineType && term.Type != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, term.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, term.DefaultValue, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_AppliesTo, term.AppliesTo, EdmValueWriter.StringAsXml);
        }

        internal void WriteComplexTypeElementHeader(IEdmComplexType complexType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ComplexType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, complexType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, complexType.BaseComplexType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, complexType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, complexType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteEnumTypeElementHeader(IEdmEnumType enumType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, enumType.Name, EdmValueWriter.StringAsXml);
            if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, enumType.UnderlyingType, this.TypeDefinitionAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_IsFlags, enumType.IsFlags, CsdlConstants.Default_IsFlags, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteEntityContainerElementHeader(IEdmEntityContainer container)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityContainer);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, container.Name, EdmValueWriter.StringAsXml);
            CsdlSemanticsEntityContainer tmp = container as CsdlSemanticsEntityContainer;
            CsdlEntityContainer csdlContainer = null;
            if (tmp != null && (csdlContainer = tmp.Element as CsdlEntityContainer) != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Extends, csdlContainer.Extends, EdmValueWriter.StringAsXml);
            }
        }

        internal void WriteEntitySetElementHeader(IEdmEntitySet entitySet)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntitySet);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entitySet.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_EntityType, entitySet.EntityType().FullName(), EdmValueWriter.StringAsXml);
        }

        internal void WriteSingletonElementHeader(IEdmSingleton singleton)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Singleton);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, singleton.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, singleton.EntityType().FullName(), EdmValueWriter.StringAsXml);
        }

        internal void WriteNavigationPropertyBinding(IEdmNavigationSource navigationSource, IEdmNavigationPropertyBinding binding)
        {
            this.WriteNavigationPropertyBinding(binding);
        }

        internal void WriteEntityTypeElementHeader(IEdmEntityType entityType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EntityType);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, entityType.Name, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_BaseType, entityType.BaseEntityType(), this.TypeDefinitionAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Abstract, entityType.IsAbstract, CsdlConstants.Default_Abstract, EdmValueWriter.BooleanAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_OpenType, entityType.IsOpen, CsdlConstants.Default_OpenType, EdmValueWriter.BooleanAsXml);

            // HasStream value should be inherited.  Only have it on base type is sufficient.
            bool writeHasStream = entityType.HasStream && (entityType.BaseEntityType() == null || (entityType.BaseEntityType() != null && !entityType.BaseEntityType().HasStream));
            this.WriteOptionalAttribute(CsdlConstants.Attribute_HasStream, writeHasStream, CsdlConstants.Default_HasStream, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteDelaredKeyPropertiesElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Key);
        }

        internal void WritePropertyRefElement(IEdmStructuralProperty property)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyRef);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteNavigationPropertyElementHeader(IEdmNavigationProperty member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationProperty);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);

            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, member.Type, this.TypeReferenceAsXml);
            if (!member.Type.IsCollection() && member.Type.IsNullable != CsdlConstants.Default_Nullable)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Nullable, member.Type.IsNullable, EdmValueWriter.BooleanAsXml);
            }

            if (member.Partner != null)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Partner, member.GetPartnerPath().Path, EdmValueWriter.StringAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_ContainsTarget, member.ContainsTarget, CsdlConstants.Default_ContainsTarget, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteOperationActionElement(string elementName, EdmOnDeleteAction operationAction)
        {
            this.xmlWriter.WriteStartElement(elementName);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Action, operationAction.ToString(), EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteSchemaElementHeader(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string xmlNamespace = GetCsdlNamespace(this.version);
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Schema, xmlNamespace);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Namespace, schema.Namespace, string.Empty, EdmValueWriter.StringAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Alias, alias, EdmValueWriter.StringAsXml);
            if (mappings != null)
            {
                foreach (KeyValuePair<string, string> mapping in mappings)
                {
                    this.xmlWriter.WriteAttributeString(EdmConstants.XmlNamespacePrefix, mapping.Key, null, mapping.Value);
                }
            }
        }

        internal void WriteAnnotationsElementHeader(string annotationsTarget)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotations);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, annotationsTarget, EdmValueWriter.StringAsXml);
        }

        internal void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Property);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, property.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, property.Type, this.TypeReferenceAsXml);
            }

            this.WriteOptionalAttribute(CsdlConstants.Attribute_DefaultValue, property.DefaultValueString, EdmValueWriter.StringAsXml);
        }

        internal void WriteEnumMemberElementHeader(IEdmEnumMember member)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Member);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, member.Name, EdmValueWriter.StringAsXml);
            bool? isExplicit = member.IsValueExplicit(this.model);
            if (!isExplicit.HasValue || isExplicit.Value)
            {
                this.xmlWriter.WriteAttributeString(CsdlConstants.Attribute_Value, EdmValueWriter.LongAsXml(member.Value.Value));
            }
        }

        internal void WriteNullableAttribute(IEdmTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Nullable, reference.IsNullable, CsdlConstants.Default_Nullable, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference)
        {
            IEdmTypeReference actualTypeReference = reference.AsActualTypeReference();

            if (actualTypeReference.IsBinary())
            {
                this.WriteBinaryTypeAttributes(actualTypeReference.AsBinary());
            }
            else if (actualTypeReference.IsString())
            {
                this.WriteStringTypeAttributes(actualTypeReference.AsString());
            }
            else if (actualTypeReference.IsTemporal())
            {
                this.WriteTemporalTypeAttributes(actualTypeReference.AsTemporal());
            }
            else if (actualTypeReference.IsDecimal())
            {
                this.WriteDecimalTypeAttributes(actualTypeReference.AsDecimal());
            }
            else if (actualTypeReference.IsSpatial())
            {
                this.WriteSpatialTypeAttributes(actualTypeReference.AsSpatial());
            }
        }

        internal void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }
        }

        internal void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference)
        {
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, EdmValueWriter.IntAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Scale, reference.Scale, CsdlConstants.Default_Scale, ScaleAsXml);
        }

        internal void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference)
        {
            if (reference.IsGeography())
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeographySrid, SridAsXml);
            }
            else if (reference.IsGeometry())
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Srid, reference.SpatialReferenceIdentifier, CsdlConstants.Default_SpatialGeometrySrid, SridAsXml);
            }
        }

        internal void WriteStringTypeAttributes(IEdmStringTypeReference reference)
        {
            if (reference.IsUnbounded)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_MaxLength, CsdlConstants.Value_Max, EdmValueWriter.StringAsXml);
            }
            else
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_MaxLength, reference.MaxLength, EdmValueWriter.IntAsXml);
            }

            if (reference.IsUnicode != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Unicode, reference.IsUnicode, CsdlConstants.Default_IsUnicode, EdmValueWriter.BooleanAsXml);
            }
        }

        internal void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference)
        {
            if (reference.Precision != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_Precision, reference.Precision, CsdlConstants.Default_TemporalPrecision, EdmValueWriter.IntAsXml);
            }
        }

        internal void WriteReferentialConstraintElementHeader(IEdmNavigationProperty constraint)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferentialConstraint);
        }

        internal void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReferentialConstraint);

            // <EntityType Name="Product">
            //   ...
            //   <Property Name="CategoryID" Type="Edm.String" Nullable="false"/>
            //  <NavigationProperty Name="Category" Type="Self.Category" Nullable="false">
            //     <ReferentialConstraint Property="CategoryID" ReferencedProperty="ID" />
            //   </NavigationProperty>
            // </EntityType>
            // the above CategoryID is DependentProperty, ID is PrincipalProperty.
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, pair.DependentProperty.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_ReferencedProperty, pair.PrincipalProperty.Name, EdmValueWriter.StringAsXml);
            this.WriteEndElement();
        }

        internal void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteAttributeString(annotation.Name, annotation.NamespaceUri, EdmValueWriter.PrimitiveValueAsXml(edmValue));
            }
        }

        internal void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation)
        {
            var edmValue = (IEdmPrimitiveValue)annotation.Value;
            if (edmValue != null)
            {
                this.xmlWriter.WriteRaw(((IEdmStringValue)edmValue).Value);
            }
        }

        internal void WriteActionElementHeader(IEdmAction action)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Action);
            this.WriteOperationElementAttributes(action);
        }

        internal void WriteFunctionElementHeader(IEdmFunction function)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Function);
            this.WriteOperationElementAttributes(function);

            if (function.IsComposable)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_IsComposable, function.IsComposable, EdmValueWriter.BooleanAsXml);
            }
        }

        internal void WriteReturnTypeElementHeader()
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ReturnType);
        }

        internal void WriteTypeAttribute(IEdmTypeReference typeReference)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, typeReference, this.TypeReferenceAsXml);
        }

        internal void WriteActionImportElementHeader(IEdmActionImport actionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_ActionImport);
            this.WriteOperationImportAttributes(actionImport, CsdlConstants.Attribute_Action);
        }

        internal void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_FunctionImport);
            this.WriteOperationImportAttributes(functionImport, CsdlConstants.Attribute_Function);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_IncludeInServiceDocument, functionImport.IncludeInServiceDocument, CsdlConstants.Default_IncludeInServiceDocument, EdmValueWriter.BooleanAsXml);
        }

        internal void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Parameter);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, parameter.Name, EdmValueWriter.StringAsXml);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, parameter.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteOperationParameterEndElement(IEdmOperationParameter parameter)
        {
            IEdmOptionalParameter optionalParameter = parameter as IEdmOptionalParameter;
            if (optionalParameter != null && !(optionalParameter.VocabularyAnnotations(this.model).Any(a => a.Term == CoreVocabularyModel.OptionalParameterTerm)))
            {
                string defaultValue = optionalParameter.DefaultValueString;
                EdmRecordExpression optionalValue = new EdmRecordExpression();

                this.WriteVocabularyAnnotationElementHeader(new EdmVocabularyAnnotation(parameter, CoreVocabularyModel.OptionalParameterTerm, optionalValue), false);
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    EdmPropertyConstructor property = new EdmPropertyConstructor(CsdlConstants.Attribute_DefaultValue, new EdmStringConstant(defaultValue));
                    this.WriteRecordExpressionElementHeader(optionalValue);
                    this.WritePropertyValueElementHeader(property, true);
                    this.WriteEndElement();
                    this.WriteEndElement();
                }

                this.WriteEndElement();
            }

            this.WriteEndElement();
        }

        internal void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_CollectionType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_ElementType, collectionType.ElementType, this.TypeReferenceAsXml);
            }
        }

        internal void WriteInlineExpression(IEdmExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Binary, ((IEdmBinaryConstantExpression)expression).Value, EdmValueWriter.BinaryAsXml);
                    break;
                case EdmExpressionKind.BooleanConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Bool, ((IEdmBooleanConstantExpression)expression).Value, EdmValueWriter.BooleanAsXml);
                    break;
                case EdmExpressionKind.DateTimeOffsetConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_DateTimeOffset, ((IEdmDateTimeOffsetConstantExpression)expression).Value, EdmValueWriter.DateTimeOffsetAsXml);
                    break;
                case EdmExpressionKind.DecimalConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Decimal, ((IEdmDecimalConstantExpression)expression).Value, EdmValueWriter.DecimalAsXml);
                    break;
                case EdmExpressionKind.FloatingConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Float, ((IEdmFloatingConstantExpression)expression).Value, EdmValueWriter.FloatAsXml);
                    break;
                case EdmExpressionKind.GuidConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Guid, ((IEdmGuidConstantExpression)expression).Value, EdmValueWriter.GuidAsXml);
                    break;
                case EdmExpressionKind.IntegerConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Int, ((IEdmIntegerConstantExpression)expression).Value, EdmValueWriter.LongAsXml);
                    break;
                case EdmExpressionKind.Path:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, ((IEdmPathExpression)expression).PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.PropertyPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_PropertyPath, ((IEdmPathExpression)expression).PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.NavigationPropertyPath:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_NavigationPropertyPath, ((IEdmPathExpression)expression).PathSegments, PathAsXml);
                    break;
                case EdmExpressionKind.StringConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_String, ((IEdmStringConstantExpression)expression).Value, EdmValueWriter.StringAsXml);
                    break;
                case EdmExpressionKind.DurationConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Duration, ((IEdmDurationConstantExpression)expression).Value, EdmValueWriter.DurationAsXml);
                    break;
                case EdmExpressionKind.DateConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_Date, ((IEdmDateConstantExpression)expression).Value, EdmValueWriter.DateAsXml);
                    break;
                case EdmExpressionKind.TimeOfDayConstant:
                    this.WriteRequiredAttribute(CsdlConstants.Attribute_TimeOfDay, ((IEdmTimeOfDayConstantExpression)expression).Value, EdmValueWriter.TimeOfDayAsXml);
                    break;
                default:
                    Debug.Assert(false, "Attempted to inline an expression that was not one of the expected inlineable types.");
                    break;
            }
        }

        internal void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Annotation);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Term, annotation.Term, this.TermAsXml);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Qualifier, annotation.Qualifier, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(annotation.Value);
            }
        }

        internal void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, value.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(value.Value);
            }
        }

        internal void WriteRecordExpressionElementHeader(IEdmRecordExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Record);
            this.WriteOptionalAttribute(CsdlConstants.Attribute_Type, expression.DeclaredType, this.TypeReferenceAsXml);
        }

        internal void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyValue);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Property, constructor.Name, EdmValueWriter.StringAsXml);
            if (isInline)
            {
                this.WriteInlineExpression(constructor.Value);
            }
        }

        internal void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_String);

            this.xmlWriter.WriteString(EdmValueWriter.StringAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Binary);
            this.xmlWriter.WriteString(EdmValueWriter.BinaryAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Bool);
            this.xmlWriter.WriteString(EdmValueWriter.BooleanAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteNullConstantExpressionElement(IEdmNullExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Null);
            this.WriteEndElement();
        }

        internal void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Date);
            this.xmlWriter.WriteString(EdmValueWriter.DateAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_DateTimeOffset);
            this.xmlWriter.WriteString(EdmValueWriter.DateTimeOffsetAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Duration);
            this.xmlWriter.WriteString(EdmValueWriter.DurationAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Decimal);
            this.xmlWriter.WriteString(EdmValueWriter.DecimalAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Float);
            this.xmlWriter.WriteString(EdmValueWriter.FloatAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Apply);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Function, expression.AppliedFunction, this.FunctionAsXml);
        }

        internal void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Guid);
            this.xmlWriter.WriteString(EdmValueWriter.GuidAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Int);
            this.xmlWriter.WriteString(EdmValueWriter.LongAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WritePathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Path);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal void WritePropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_PropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyPath);
            this.xmlWriter.WriteString(PathAsXml(expression.PathSegments));
            this.WriteEndElement();
        }

        internal void WriteIfExpressionElementHeader(IEdmIfExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_If);
        }

        internal void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Collection);
        }

        internal void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_LabeledElement);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, labeledElement.Name, EdmValueWriter.StringAsXml);
        }

        internal void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TimeOfDay);
            this.xmlWriter.WriteString(EdmValueWriter.TimeOfDayAsXml(expression.Value));
            this.WriteEndElement();
        }

        internal void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_IsType);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_Cast);
            if (inlineType)
            {
                this.WriteRequiredAttribute(CsdlConstants.Attribute_Type, expression.Type, this.TypeReferenceAsXml);
            }
        }

        internal void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_EnumMember);
            this.xmlWriter.WriteString(EnumMemberAsXml(expression.EnumMembers));
            this.WriteEndElement();
        }

        internal void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_TypeDefinition);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, typeDefinition.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(CsdlConstants.Attribute_UnderlyingType, typeDefinition.UnderlyingType, this.TypeDefinitionAsXml);
        }

        internal void WriteEndElement()
        {
            this.xmlWriter.WriteEndElement();
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, T defaultValue, Func<T, string> toXml)
        {
            if (!value.Equals(defaultValue))
            {
                this.xmlWriter.WriteAttributeString(attribute, toXml(value));
            }
        }

        internal void WriteOptionalAttribute<T>(string attribute, T value, Func<T, string> toXml)
        {
            if (value != null)
            {
                this.xmlWriter.WriteAttributeString(attribute, toXml(value));
            }
        }

        internal void WriteRequiredAttribute<T>(string attribute, T value, Func<T, string> toXml)
        {
            this.xmlWriter.WriteAttributeString(attribute, toXml(value));
        }

        private void WriteOperationElementAttributes(IEdmOperation operation)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, operation.Name, EdmValueWriter.StringAsXml);

            if (operation.IsBound)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_IsBound, operation.IsBound, EdmValueWriter.BooleanAsXml);
            }

            if (operation.EntitySetPath != null)
            {
                this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySetPath, operation.EntitySetPath.PathSegments, PathAsXml);
            }
        }

        private void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding)
        {
            this.xmlWriter.WriteStartElement(CsdlConstants.Element_NavigationPropertyBinding);

            this.WriteRequiredAttribute(CsdlConstants.Attribute_Path, binding.Path.Path, EdmValueWriter.StringAsXml);

            // TODO: handle container names, etc.
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Target, binding.Target.Name, EdmValueWriter.StringAsXml);

            this.xmlWriter.WriteEndElement();
        }

        private static string EnumMemberAsXml(IEnumerable<IEdmEnumMember> members)
        {
            string enumTypeName = members.First().DeclaringType.FullName();
            List<string> memberList = new List<string>();
            foreach (var member in members)
            {
                memberList.Add(enumTypeName + "/" + member.Name);
            }

            return string.Join(" ", memberList.ToArray());
        }

        private static string SridAsXml(int? i)
        {
            return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_SridVariable;
        }

        private static string ScaleAsXml(int? i)
        {
            return i.HasValue ? Convert.ToString(i.Value, CultureInfo.InvariantCulture) : CsdlConstants.Value_ScaleVariable;
        }

        private static string GetCsdlNamespace(Version edmVersion)
        {
            string[] @namespaces;
            if (CsdlConstants.SupportedVersions.TryGetValue(edmVersion, out @namespaces))
            {
                return @namespaces[0];
            }

            throw new InvalidOperationException(Strings.Serializer_UnknownEdmVersion);
        }

        private void WriteOperationImportAttributes(IEdmOperationImport operationImport, string operationAttributeName)
        {
            this.WriteRequiredAttribute(CsdlConstants.Attribute_Name, operationImport.Name, EdmValueWriter.StringAsXml);
            this.WriteRequiredAttribute(operationAttributeName, operationImport.Operation.FullName(), EdmValueWriter.StringAsXml);

            if (operationImport.EntitySet != null)
            {
                var pathExpression = operationImport.EntitySet as IEdmPathExpression;
                if (pathExpression != null)
                {
                    this.WriteOptionalAttribute(CsdlConstants.Attribute_EntitySet, pathExpression.PathSegments, PathAsXml);
                }
                else
                {
                    throw new InvalidOperationException(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                }
            }
        }

        private string SerializationName(IEdmSchemaElement element)
        {
            if (this.namespaceAliasMappings != null)
            {
                string alias;
                if (this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias))
                {
                    return alias + "." + element.Name;
                }
            }

            return element.FullName();
        }

        private string TypeReferenceAsXml(IEdmTypeReference type)
        {
            if (type.IsCollection())
            {
                IEdmCollectionTypeReference collectionReference = type.AsCollection();
                Debug.Assert(collectionReference.ElementType().Definition is IEdmSchemaElement, "Cannot inline parameter type if not a named element or collection of named elements");
                return CsdlConstants.Value_Collection + "(" + this.SerializationName((IEdmSchemaElement)collectionReference.ElementType().Definition) + ")";
            }
            else if (type.IsEntityReference())
            {
                return CsdlConstants.Value_Ref + "(" + this.SerializationName(type.AsEntityReference().EntityReferenceDefinition().EntityType) + ")";
            }

            Debug.Assert(type.Definition is IEdmSchemaElement, "Cannot inline parameter type if not a named element or collection of named elements");
            return this.SerializationName((IEdmSchemaElement)type.Definition);
        }

        private string TypeDefinitionAsXml(IEdmSchemaType type)
        {
            return this.SerializationName(type);
        }

        private string FunctionAsXml(IEdmOperation operation)
        {
            return this.SerializationName(operation);
        }

        private string TermAsXml(IEdmTerm term)
        {
            if (term == null)
            {
                return string.Empty;
            }

            return this.SerializationName(term);
        }
    }
#endif
}
