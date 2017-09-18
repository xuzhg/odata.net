//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;
using System.Diagnostics;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal abstract class EdmModelCsdlSchemaWriter
    {
        public IEdmModel Model { get; }

        public Version Version { get; }

        public VersioningDictionary<string, string> NamespaceAliasMappings { get; }

        internal EdmModelCsdlSchemaWriter(IEdmModel model, Version edmVersion)
        {
            Version = edmVersion;
            Model = model;
            NamespaceAliasMappings = model.GetNamespaceAliases();
        }

        internal static string PathAsXml(IEnumerable<string> path)
        {
            return EdmUtil.JoinInternal("/", path);
        }

        #region Write EDM Reference
        internal abstract void WriteReferencesStart(IEnumerable<IEdmReference> references);

        internal abstract void WriteReferencesEnd(IEnumerable<IEdmReference> references);

        internal abstract void WriteReferenceElementStart(IEdmReference reference);

        internal abstract void WriteReferenceElementEnd(IEdmReference reference);

        internal abstract void WriteReferenceIncludesStart(IEnumerable<IEdmInclude> includes);

        internal abstract void WriteReferenceIncludesEnd(IEnumerable<IEdmInclude> includes);

        internal abstract void WriteReferenceIncludeAnnotationsStart(IEnumerable<IEdmIncludeAnnotations> annotations);

        internal abstract void WriteReferenceIncludeAnnotationsEnd(IEnumerable<IEdmIncludeAnnotations> annotations);

        internal abstract void WriteIncludeElement(IEdmInclude include);

        internal abstract void WriteIncludeAnnotationsElement(IEdmIncludeAnnotations includeAnnotations);
        #endregion

        internal abstract void WriteTermElementHeader(IEdmTerm term, bool inlineType);

        internal abstract void WriteComplexTypeElementHeader(IEdmComplexType complexType);
        internal abstract void WriteComplexTypeElementEnd(IEdmComplexType complexType);

        internal abstract void WriteEnumTypeElementHeader(IEdmEnumType enumType);
        internal abstract void WriteEnumTypeElementEnd(IEdmEnumType enumType);

        internal abstract void WriteEntityContainerElementStart(IEdmEntityContainer container);
        internal abstract void WriteEntityContainerElementEnd(IEdmEntityContainer container);

        internal abstract void WriteEntitySetElementStart(IEdmEntitySet entitySet);
        internal abstract void WriteEntitySetElementEnd(IEdmEntitySet entitySet);

        internal abstract void WriteSingletonElementStart(IEdmSingleton singleton);
        internal abstract void WriteSingletonElementEnd(IEdmSingleton singleton);

        internal abstract void WriteNavigationPropertyBinding(IEdmNavigationSource navigationSource, IEdmNavigationPropertyBinding binding);

        internal abstract void WriteEntityTypeElementHeader(IEdmEntityType entityType);
        internal abstract void WriteEntityTypeElementEnd(IEdmEntityType entityType);

        internal abstract void WriteDelaredKeyPropertiesElementHeader();
        internal abstract void WriteDelaredKeyPropertiesElementEnd();

        internal abstract void WritePropertyRefElement(IEdmStructuralProperty property);

        internal abstract void WriteNavigationPropertyElementHeader(IEdmNavigationProperty member);
        internal abstract void WriteNavigationPropertyElementEnd(IEdmNavigationProperty member);
        
        internal abstract void WriteOperationActionElement(string elementName, EdmOnDeleteAction operationAction);

        internal abstract void WriteSchemaElementStart(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings);

        internal abstract void WriteSchemaElementEnd(EdmSchema schema, string alias);

        internal abstract void WriteAnnotationsElementHeader(string annotationsTarget);

        internal abstract void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType);

        internal abstract void WriteEnumMemberElementHeader(IEdmEnumMember member);
        internal abstract void WriteEnumMemberElementEnd(IEdmEnumMember member);

        internal abstract void WriteNullableAttribute(IEdmTypeReference reference);

        internal virtual void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference)
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

        internal abstract void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference);

        internal abstract void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference);

        internal abstract void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference);

        internal abstract void WriteStringTypeAttributes(IEdmStringTypeReference reference);

        internal abstract void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference);

        internal abstract void WriteReferentialConstraintElementHeader(IEdmNavigationProperty constraint);

        internal abstract void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair);

        internal abstract void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation);

        internal abstract void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation);

        internal abstract void WriteActionElementHeader(IEdmAction action);

        internal abstract void WriteFunctionElementHeader(IEdmFunction function);

        internal abstract void WriteReturnTypeElementHeader();

        internal abstract void WriteTypeAttribute(IEdmTypeReference typeReference);

        internal abstract void WriteActionImportElementHeader(IEdmActionImport actionImport);

        internal abstract void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport);

        internal abstract void WriteActionImportElementEnd(IEdmActionImport actionImport);

        internal abstract void WriteFunctionImportElementEnd(IEdmFunctionImport functionImport);

        internal abstract void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType);

        internal abstract void WriteOperationParameterEndElement(IEdmOperationParameter parameter);

        internal abstract void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType);

        internal abstract void WriteInlineExpression(IEdmExpression expression);

        internal abstract void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline);

        internal abstract void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline);

        internal abstract void WriteRecordExpressionElementHeader(IEdmRecordExpression expression);

        internal abstract void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline);

        internal abstract void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression);

        internal abstract void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression);

        internal abstract void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression);

        internal abstract void WriteNullConstantExpressionElement(IEdmNullExpression expression);

        internal abstract void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression);

        internal abstract void WriteDateTimeOffsetConstantExpressionElement(
            IEdmDateTimeOffsetConstantExpression expression);

        internal abstract void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression);

        internal abstract void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression);

        internal abstract void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression);

        internal abstract void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression);

        internal abstract void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression);

        internal abstract void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression);

        internal abstract void WritePathExpressionElement(IEdmPathExpression expression);

        internal abstract void WritePropertyPathExpressionElement(IEdmPathExpression expression);

        internal abstract void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression);

        internal abstract void WriteIfExpressionElementHeader(IEdmIfExpression expression);

        internal abstract void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression);

        internal abstract void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement);

        internal abstract void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression);

        internal abstract void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType);

        internal abstract void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType);

        internal abstract void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression);

        internal abstract void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition);

        internal abstract void WriteEndElement();
        internal abstract void WriteEndElements();

        protected string TypeReferenceAsXml(IEdmTypeReference type)
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

        protected string TypeDefinitionAsXml(IEdmSchemaType type)
        {
            return this.SerializationName(type);
        }

        protected string SerializationName(IEdmSchemaElement element)
        {
            if (this.NamespaceAliasMappings != null)
            {
                string alias;
                if (this.NamespaceAliasMappings.TryGetValue(element.Namespace, out alias))
                {
                    return alias + "." + element.Name;
                }
            }

            return element.FullName();
        }

        protected string FunctionAsXml(IEdmOperation operation)
        {
            return this.SerializationName(operation);
        }

        protected string TermAsXml(IEdmTerm term)
        {
            if (term == null)
            {
                return string.Empty;
            }

            return this.SerializationName(term);
        }
    }
}
