//---------------------------------------------------------------------
// <copyright file="ODataUriConversionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Metadata;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData
{
    /// <summary>
    /// Utility functions for writing values for use in a URL.
    /// </summary>
    internal static class ODataUriConversionUtils
    {
        /// <summary>
        /// Converts a primitive to a string for use in a Url.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="version">OData version to be compliant with.</param>
        /// <returns>A string representation of <paramref name="value"/> to be added to a Url.</returns>
        internal static string ConvertToUriPrimitiveLiteral(object value, ODataVersion version)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            // TODO: Differences between Astoria and ODL's Uri literals
            /* This should have the same behavior of Astoria with these differences:
             * 1) Cannot handle the System.Data.Linq.Binary type
             * 2) Cannot handle the System.Data.Linq.XElement type
             * 3) Astoria does not put a 'd' or 'D' on double values
             */

            // for legacy backwards compatibility reasons, use the formatter which does not URL-encode the resulting string.
            return LiteralFormatter.ForConstantsWithoutEncoding.Format(value);
        }

        /// <summary>
        /// Converts an enum value to a string for use in a Url.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="version">OData version to be compliant with.</param>
        /// <returns>A string representation of <paramref name="value"/> to be added to a Url.</returns>
        internal static string ConvertToUriEnumLiteral(ODataEnumValue value, ODataVersion version)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");
            ExceptionUtils.CheckArgumentNotNull(value.TypeName, "value.TypeName");
            ExceptionUtils.CheckArgumentNotNull(value.Value, "value.Value");

            // not URL-encode the resulting string:
            return string.Format(CultureInfo.InvariantCulture, "{0}'{1}'", value.TypeName, value.Value);
        }

        /// <summary>
        /// Converts the given string <paramref name="value"/> to an ODataCollectionValue and returns it.
        /// Tries in both JSON light and Verbose JSON.
        /// </summary>
        /// <remarks>Does not handle primitive values.</remarks>
        /// <param name="value">Value to be deserialized.</param>
        /// <param name="model">Model to use for verification.</param>
        /// <param name="typeReference">Expected type reference from deserialization. If null, verification will be skipped.</param>
        /// <returns>An ODataCollectionValue that results from the deserialization of <paramref name="value"/>.</returns>
        internal static object ConvertFromCollectionValue(string value, IEdmModel model, IEdmTypeReference typeReference)
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            using (StringReader reader = new StringReader(value))
            {
                ODataMessageInfo messageInfo = new ODataMessageInfo
                {
                    MediaType = new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType),
                    Model = model,
                    IsResponse = false,
                    IsAsync = false,
                    MessageStream = null,
                };

                using (ODataJsonLightInputContext context = new ODataJsonLightInputContext(reader, messageInfo, settings))
                {
                    ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(context);

                    // TODO: The way JSON array literals look in the URI is different that response payload with an array in it.
                    // The fact that we have to manually setup the underlying reader shows this different in the protocol.
                    // There is a discussion on if we should change this or not.
                    deserializer.JsonReader.Read(); // Move to first thing
                    object rawResult = deserializer.ReadNonEntityValue(
                        null /*payloadTypeName*/,
                        typeReference,
                        null /*DuplicatePropertyNameChecker*/,
                        null /*CollectionWithoutExpectedTypeValidator*/,
                        true /*validateNullValue*/,
                        false /*isTopLevelPropertyValue*/,
                        false /*insideComplexValue*/,
                        null /*propertyName*/);
                    deserializer.ReadPayloadEnd(false);

                    Debug.Assert(rawResult is ODataCollectionValue, "rawResult is ODataCollectionValue");
                    return rawResult;
                }
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="primitiveValue"/> is or can be coerced to <paramref name="expectedTypeReference"/>, and coerces it if necessary.
        /// </summary>
        /// <param name="primitiveValue">An EDM primitive value to verify.</param>
        /// <param name="literalValue">The literal value that was parsed as this primitiveValue.</param>
        /// <param name="model">Model to verify against.</param>
        /// <param name="expectedTypeReference">Expected type reference.</param>
        /// <returns>Coerced version of the <paramref name="primitiveValue"/>.</returns>
        internal static object VerifyAndCoerceUriPrimitiveLiteral(
            object primitiveValue,
            string literalValue,
            IEdmModel model,
            IEdmTypeReference expectedTypeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            ExceptionUtils.CheckArgumentNotNull(literalValue, "literalValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(expectedTypeReference, "expectedTypeReference");

            // First deal with null literal
            ODataNullValue nullValue = primitiveValue as ODataNullValue;
            if (nullValue != null)
            {
                if (!expectedTypeReference.IsNullable)
                {
                    throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType(expectedTypeReference.FullName()));
                }

                return nullValue;
            }

            // Only other positive case is a numeric primitive that needs to be coerced
            IEdmPrimitiveTypeReference expectedPrimitiveTypeReference = expectedTypeReference.AsPrimitiveOrNull();
            if (expectedPrimitiveTypeReference == null)
            {
                throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(expectedTypeReference.FullName(), literalValue));
            }

            object coercedResult = CoerceNumericType(primitiveValue, expectedPrimitiveTypeReference.PrimitiveDefinition());
            if (coercedResult != null)
            {
                return coercedResult;
            }

            // if expectedTypeReference is set, need to coerce cast
            coercedResult = CoerceTemporalType(primitiveValue, expectedPrimitiveTypeReference.PrimitiveDefinition());
            if (coercedResult != null)
            {
                return coercedResult;
            }

            Type actualType = primitiveValue.GetType();
            Type targetType = TypeUtils.GetNonNullableType(EdmLibraryExtensions.GetPrimitiveClrType(expectedPrimitiveTypeReference));

            // If target type is assignable from actual type, we're OK
            if (targetType.IsAssignableFrom(actualType))
            {
                return primitiveValue;
            }

            throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(expectedPrimitiveTypeReference.FullName(), literalValue));
        }

        /// <summary>
        /// Converts a <see cref="ODataNestedResourceValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="resource">Nested resource to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <returns>A string representation of <paramref name="resource"/> to be added to a Url.</returns>
        internal static string ConvertToNestedResourceLiteral(ODataNestedResourceValue resource, IEdmModel model, ODataVersion version)
        {
            ExceptionUtils.CheckArgumentNotNull(resource, "resource");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            return ConverToJsonLightLiteral(
                model,
                context =>
                {
                    ODataWriter writer = context.CreateODataUriParameterResourceWriter(null, null);
                    WriteNestedResourceValue(writer, resource);
                });
        }

        /// <summary>
        /// Converts a <see cref="ODataNestedResourceSetValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="resourceSet">Nested resource set to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <returns>A string representation of <paramref name="resourceSet"/> to be added to a Url.</returns>
        internal static string ConvertToNestedResourceSetLiteral(ODataNestedResourceSetValue resourceSet, IEdmModel model, ODataVersion version)
        {
            ExceptionUtils.CheckArgumentNotNull(resourceSet, "resourceSet");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            return ConverToJsonLightLiteral(
                model,
                context =>
                {
                    ODataWriter writer = context.CreateODataUriParameterResourceSetWriter(null, null);
                    WriteNestedResourceSetValue(writer, resourceSet);
                });
        }

        /// <summary>
        /// Converts the given string <paramref name="value"/> to an <see cref="ODataNestedValue"/> and returns it.
        /// </summary>
        /// <remarks>Does not handle primitive values.</remarks>
        /// <param name="value">Value to be deserialized.</param>
        /// <param name="model">Model to use for verification.</param>
        /// <param name="version">The OData version.</param>
        /// <param name="typeReference">Expected type reference from deserialization.</param>
        /// <returns>An <see cref="ODataNestedValue"/> that results from the deserialization of <paramref name="value"/>.</returns>
        internal static ODataNestedValue ConvertFromNestedValueLiteral(string value, IEdmModel model, ODataVersion version,
            IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            settings.Version = version;

            using (StringReader stringReader = new StringReader(value))
            {
                ODataMessageInfo messageInfo = new ODataMessageInfo
                {
                    MediaType = new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType),
                    Model = model,
                    IsResponse = false,
                    IsAsync = false,
                    MessageStream = null,
                };

                using (ODataJsonLightInputContext context = new ODataJsonLightInputContext(stringReader, messageInfo, settings))
                {
                    ODataReader reader;
                    if (typeReference.IsCollection())
                    {
                        IEdmCollectionTypeReference collectionType = typeReference.AsCollection();
                        reader = context.CreateUriParameterResourceSetReader(null, collectionType.ElementType().AsStructured().StructuredDefinition());
                    }
                    else
                    {
                        reader = context.CreateUriParameterResourceReader(null, typeReference.ToStructuredType());
                    }

                    return ReadNestedValue(reader);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="ODataResourceBase"/> to a string for use in a Url.
        /// </summary>
        /// <param name="resource">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <returns>A string representation of <paramref name="resource"/> to be added to a Url.</returns>
        internal static string ConvertToUriEntityLiteral(ODataResourceBase resource, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(resource, "resource");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            return ConverToJsonLightLiteral(
                model,
                context =>
            {
                ODataWriter writer = context.CreateODataUriParameterResourceWriter(null, null);
                WriteStartResource(writer, resource);
                writer.WriteEnd();
            });
        }

        /// <summary>
        /// Converts a list of <see cref="ODataResourceBase"/> to a string for use in a Url.
        /// </summary>
        /// <param name="entries">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <returns>A string representation of <paramref name="entries"/> to be added to a Url.</returns>
        internal static string ConvertToUriEntitiesLiteral(IEnumerable<ODataResourceBase> entries, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(entries, "entries");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            return ConverToJsonLightLiteral(
                model,
                context =>
            {
                ODataWriter writer = context.CreateODataUriParameterResourceSetWriter(null, null);
                writer.WriteStart(new ODataResourceSet());

                // TODO: Write Complex Properties in entry
                foreach (var resource in entries)
                {
                    WriteStartResource(writer, resource);
                    writer.WriteEnd();
                }

                writer.WriteEnd();
            });
        }

        /// <summary>
        /// Converts a <see cref="ODataEntityReferenceLink"/> to a string for use in a Url.
        /// </summary>
        /// <param name="link">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <returns>A string representation of <paramref name="link"/> to be added to a Url.</returns>
        internal static string ConvertToUriEntityReferenceLiteral(ODataEntityReferenceLink link, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(link, "link");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            return ConverToJsonLightLiteral(model, context => context.WriteEntityReferenceLink(link));
        }

        /// <summary>
        /// Converts a <see cref="ODataEntityReferenceLinks"/> to a string for use in a Url.
        /// </summary>
        /// <param name="links">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <returns>A string representation of <paramref name="links"/> to be added to a Url.</returns>
        internal static string ConvertToUriEntityReferencesLiteral(ODataEntityReferenceLinks links, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(links, "links");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            return ConverToJsonLightLiteral(model, context => context.WriteEntityReferenceLinks(links));
        }

        /// <summary>
        /// Converts a <see cref="ODataCollectionValue"/> to a string for use in a Url.
        /// </summary>
        /// <param name="collectionValue">Instance to convert.</param>
        /// <param name="model">Model to be used for validation. User model is optional. The EdmLib core model is expected as a minimum.</param>
        /// <param name="version">Version to be compliant with. Collection requires >= V3.</param>
        /// <returns>A string representation of <paramref name="collectionValue"/> to be added to a Url.</returns>
        internal static string ConvertToUriCollectionLiteral(ODataCollectionValue collectionValue, IEdmModel model, ODataVersion version)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionValue, "collectionValue");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            StringBuilder builder = new StringBuilder();
            using (TextWriter textWriter = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
                {
                    Version = version,
                    Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                };

                WriteJsonLightLiteral(
                    model,
                    messageWriterSettings,
                    textWriter,
                    (serializer) => serializer.WriteCollectionValue(
                        collectionValue,
                        null /*metadataTypeReference*/,
                        null /*valueTypeReference*/,
                        false /*isTopLevelProperty*/,
                        true /*isInUri*/,
                        false /*isOpenPropertyType*/));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Coerces the given <paramref name="primitiveValue"/> to the appropriate CLR type based on <paramref name="targetEdmType"/>.
        /// </summary>
        /// <param name="primitiveValue">Primitive value to coerce.</param>
        /// <param name="targetEdmType">Edm primitive type to check against.</param>
        /// <returns><paramref name="primitiveValue"/> as the corresponding CLR type indicated by <paramref name="targetEdmType"/>, or null if unable to coerce.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Centralized method for coercing numeric types in easiest to understand.")]
        internal static object CoerceNumericType(object primitiveValue, IEdmPrimitiveType targetEdmType)
        {
            // This is implemented to match TypePromotionUtils and MetadataUtilsCommon.CanConvertPrimitiveTypeTo()
            ExceptionUtils.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            ExceptionUtils.CheckArgumentNotNull(targetEdmType, "targetEdmType");

            EdmPrimitiveTypeKind targetPrimitiveKind = targetEdmType.PrimitiveKind;

            if (primitiveValue is SByte)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.SByte:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Int16:
                        return Convert.ToInt16((SByte)primitiveValue);
                    case EdmPrimitiveTypeKind.Int32:
                        return Convert.ToInt32((SByte)primitiveValue);
                    case EdmPrimitiveTypeKind.Int64:
                        return Convert.ToInt64((SByte)primitiveValue);
                    case EdmPrimitiveTypeKind.Single:
                        return Convert.ToSingle((SByte)primitiveValue);
                    case EdmPrimitiveTypeKind.Double:
                        return Convert.ToDouble((SByte)primitiveValue);
                    case EdmPrimitiveTypeKind.Decimal:
                        return Convert.ToDecimal((SByte)primitiveValue);
                }
            }

            if (primitiveValue is Byte)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Byte:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Int16:
                        return Convert.ToInt16((Byte)primitiveValue);
                    case EdmPrimitiveTypeKind.Int32:
                        return Convert.ToInt32((Byte)primitiveValue);
                    case EdmPrimitiveTypeKind.Int64:
                        return Convert.ToInt64((Byte)primitiveValue);
                    case EdmPrimitiveTypeKind.Single:
                        return Convert.ToSingle((Byte)primitiveValue);
                    case EdmPrimitiveTypeKind.Double:
                        return Convert.ToDouble((Byte)primitiveValue);
                    case EdmPrimitiveTypeKind.Decimal:
                        return Convert.ToDecimal((Byte)primitiveValue);
                }
            }

            if (primitiveValue is Int16)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Int16:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Int32:
                        return Convert.ToInt32((Int16)primitiveValue);
                    case EdmPrimitiveTypeKind.Int64:
                        return Convert.ToInt64((Int16)primitiveValue);
                    case EdmPrimitiveTypeKind.Single:
                        return Convert.ToSingle((Int16)primitiveValue);
                    case EdmPrimitiveTypeKind.Double:
                        return Convert.ToDouble((Int16)primitiveValue);
                    case EdmPrimitiveTypeKind.Decimal:
                        return Convert.ToDecimal((Int16)primitiveValue);
                }
            }

            if (primitiveValue is Int32)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Int32:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Int64:
                        return Convert.ToInt64((Int32)primitiveValue);
                    case EdmPrimitiveTypeKind.Single:
                        return Convert.ToSingle((Int32)primitiveValue);
                    case EdmPrimitiveTypeKind.Double:
                        return Convert.ToDouble((Int32)primitiveValue);
                    case EdmPrimitiveTypeKind.Decimal:
                        return Convert.ToDecimal((Int32)primitiveValue);
                }
            }

            if (primitiveValue is Int64)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Int64:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Single:
                        return Convert.ToSingle((Int64)primitiveValue);
                    case EdmPrimitiveTypeKind.Double:
                        return Convert.ToDouble((Int64)primitiveValue);
                    case EdmPrimitiveTypeKind.Decimal:
                        return Convert.ToDecimal((Int64)primitiveValue);
                }
            }

            if (primitiveValue is Single)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Single:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Double:
                        /*to string then to double, avoid losing precision like "(double)123.001f" which is 123.00099945068359, instead of 123.001d.*/
                        return double.Parse(((Single)primitiveValue).ToString("R", CultureInfo.InvariantCulture),
                            CultureInfo.InvariantCulture);
                    case EdmPrimitiveTypeKind.Decimal:
                        return Convert.ToDecimal((Single)primitiveValue);
                }
            }

            if (primitiveValue is Double)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Double:
                        return primitiveValue;
                    case EdmPrimitiveTypeKind.Decimal:
                        // TODO: extract these convertion steps to an individual function
                        decimal doubleToDecimalR;

                        // To keep the full presion of the current value, which if necessary is all 17 digits of precision supported by the Double type.
                        if (decimal.TryParse(((Double)primitiveValue).ToString("R", CultureInfo.InvariantCulture),
                            out doubleToDecimalR))
                        {
                            return doubleToDecimalR;
                        }

                        return Convert.ToDecimal((Double)primitiveValue);
                }
            }

            if (primitiveValue is Decimal)
            {
                switch (targetPrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Decimal:
                        return primitiveValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Coerces the given <paramref name="primitiveValue"/> to the appropriate CLR type based on <paramref name="targetEdmType"/>.
        /// </summary>
        /// <param name="primitiveValue">Primitive value to coerce.</param>
        /// <param name="targetEdmType">Edm primitive type to check against.</param>
        /// <returns><paramref name="primitiveValue"/> as the corresponding CLR type indicated by <paramref name="targetEdmType"/>, or null if unable to coerce.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Centralized method for coercing temporal types in easiest to understand.")]
        internal static object CoerceTemporalType(object primitiveValue, IEdmPrimitiveType targetEdmType)
        {
            // This is implemented to match TypePromotionUtils and MetadataUtilsCommon.CanConvertPrimitiveTypeTo()
            ExceptionUtils.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            ExceptionUtils.CheckArgumentNotNull(targetEdmType, "targetEdmType");

            EdmPrimitiveTypeKind targetPrimitiveKind = targetEdmType.PrimitiveKind;

            switch (targetPrimitiveKind)
            {
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    if (primitiveValue is Date)
                    {
                        var dateValue = (Date)primitiveValue;
                        return new DateTimeOffset(dateValue.Year, dateValue.Month, dateValue.Day, 0, 0, 0, new TimeSpan(0));
                    }

                    break;

                case EdmPrimitiveTypeKind.Date:
                    var stringValue = primitiveValue as string;
                    if (stringValue != null)
                    {
                        // Coerce to Date Type from String.
                        return PlatformHelper.ConvertStringToDate(stringValue);
                    }

                    break;
            }

            return null;
        }

        /// <summary>
        /// Writes an <see cref="ODataNestedResourceSetValue"/>.
        /// </summary>
        /// <param name="writer">The <see cref="ODataWriter"/> to use to write the (deleted) resource.</param>
        /// <param name="resource">The resource, or deleted resource, to write.</param>
        private static void WriteNestedResourceSetValue(ODataWriter writer, ODataNestedResourceSetValue nestedResourceSet)
        {
            WriteStartResourceSet(writer, nestedResourceSet.ResourceSet);

            if (nestedResourceSet.NestedResources != null)
            {
                foreach (var item in nestedResourceSet.NestedResources)
                {
                    WriteNestedResourceValue(writer, item);
                }
            }

            writer.WriteEnd();
        }

        /// <summary>
        /// Writes an <see cref="ODataNestedResourceValue"/>.
        /// </summary>
        /// <param name="writer">The <see cref="ODataWriter"/> to use to write the (deleted) resource.</param>
        /// <param name="nestedResourceValue">The nested resource to write.</param>
        private static void WriteNestedResourceValue(ODataWriter writer, ODataNestedResourceValue nestedResourceValue)
        {
            WriteStartResource(writer, nestedResourceValue.Resource);

            if (nestedResourceValue.NestedItems != null)
            {
                foreach (var item in nestedResourceValue.NestedItems)
                {
                    WriteNestedResourceItem(writer, item);
                }
            }

            writer.WriteEnd();
        }

        /// <summary>
        /// Writes an <see cref="ODataNestedItem"/>.
        /// </summary>
        /// <param name="writer">The <see cref="ODataWriter"/> to use to write the (deleted) resource.</param>
        /// <param name="nestedItem">The nested item to write.</param>
        private static void WriteNestedResourceItem(ODataWriter writer, ODataNestedItem nestedItem)
        {
            // skip the "null" nested item.
            if (nestedItem == null)
            {
                return;
            }

            writer.WriteStart(nestedItem.NestedResourceInfo);

            bool isCollection = nestedItem.NestedResourceInfo.IsCollection.HasValue && nestedItem.NestedResourceInfo.IsCollection.Value;
            if (nestedItem.NestedValue == null)
            {
                if (isCollection)
                {
                    writer.WriteStart(new ODataResourceSet());
                }
                else
                {
                    writer.WriteStart(resource: null);
                }

                writer.WriteEnd();
            }
            else
            {
                if (isCollection)
                {
                    Debug.Assert(nestedItem.NestedValue is ODataNestedResourceSetValue);
                    WriteNestedResourceSetValue(writer, (ODataNestedResourceSetValue)(nestedItem.NestedValue));
                }
                else
                {
                    Debug.Assert(nestedItem.NestedValue is ODataNestedResourceValue);
                    WriteNestedResourceValue(writer, (ODataNestedResourceValue)(nestedItem.NestedValue));
                }
            }

            writer.WriteEnd();
        }

        /// <summary>
        /// Writes an <see cref="ODataResourceBase"/> as either a resource or a deleted resource.
        /// </summary>
        /// <param name="writer">The <see cref="ODataWriter"/> to use to write the (deleted) resource.</param>
        /// <param name="resource">The resource, or deleted resource, to write.</param>
        private static void WriteStartResource(ODataWriter writer, ODataResourceBase resource)
        {
            ODataDeletedResource deletedResource = resource as ODataDeletedResource;
            if (deletedResource != null)
            {
                writer.WriteStart(deletedResource);
            }
            else
            {
                // will write a null resource if resource is not an ODataResource
                writer.WriteStart(resource as ODataResource);
            }
        }

        /// <summary>
        /// Writes an <see cref="ODataResourceSetBase"/> as either a resource or a deleted resource.
        /// </summary>
        /// <param name="writer">The <see cref="ODataWriter"/> to use to write the (deleted) resource.</param>
        /// <param name="resource">The resource, or deleted resource, to write.</param>
        private static void WriteStartResourceSet(ODataWriter writer, ODataResourceSetBase resourceSet)
        {
            ODataDeltaResourceSet deltaResourceSet = resourceSet as ODataDeltaResourceSet;
            if (deltaResourceSet != null)
            {
                writer.WriteStart(deltaResourceSet);
            }
            else
            {
                // will write a null resource if resource is not an ODataResource
                writer.WriteStart(resourceSet as ODataResourceSet);
            }
        }

        /// <summary>
        /// Write a literal value in JSON Light format.
        /// </summary>
        /// <param name="model">EDM Model to use for validation and type lookups.</param>
        /// <param name="messageWriterSettings">Settings to use when writing.</param>
        /// <param name="textWriter">TextWriter to use as the output for the value.</param>
        /// <param name="writeValue">Delegate to use to actually write the value.</param>
        private static void WriteJsonLightLiteral(IEdmModel model, ODataMessageWriterSettings messageWriterSettings, TextWriter textWriter, Action<ODataJsonLightValueSerializer> writeValue)
        {
            // Calling dispose since it's the right thing to do, but when created from a custom-built TextWriter
            // the output context Dispose will not actually dispose anything, it will just cleanup itself.
            // TODO: URI parser will also support DI container in the future but set the container to null at this moment.
            ODataMessageInfo messageInfo = new ODataMessageInfo
            {
                Model = model,
                IsAsync = false,
                IsResponse = false
            };

            using (ODataJsonLightOutputContext jsonOutputContext =
                new ODataJsonLightOutputContext(textWriter, messageInfo, messageWriterSettings))
            {
                ODataJsonLightValueSerializer jsonLightValueSerializer = new ODataJsonLightValueSerializer(jsonOutputContext);
                writeValue(jsonLightValueSerializer);
                jsonLightValueSerializer.AssertRecursionDepthIsZero();
            }
        }

        /// <summary>
        /// Convert to a literal value in JSON Light format.
        /// </summary>
        /// <param name="model">EDM Model to use for validation and type lookups.</param>
        /// <param name="writeAction">Delegate to use to actually write the value.</param>
        /// <returns>The literal value string.</returns>
        private static string ConverToJsonLightLiteral(IEdmModel model, Action<ODataOutputContext> writeAction)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ODataMessageWriterSettings messageWriterSettings = new ODataMessageWriterSettings()
                {
                    Version = ODataVersion.V4,
                    Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                };

                ODataMediaType mediaType = new ODataMediaType(MimeConstants.MimeApplicationType, MimeConstants.MimeJsonSubType);

                ODataMessageInfo messageInfo = new ODataMessageInfo
                {
                    MessageStream = stream,
                    Encoding = Encoding.UTF8,
                    IsAsync = false,
                    IsResponse = false,
                    MediaType = mediaType,
                    Model = model
                };

                // TODO: URI parser will also support DI container in the future but set the container to null at this moment.
                using (ODataJsonLightOutputContext jsonOutputContext =
                    new ODataJsonLightOutputContext(messageInfo, messageWriterSettings))
                {
                    writeAction(jsonOutputContext);
                    stream.Position = 0;
                    return new StreamReader(stream).ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Read and return the <see cref="ODataNestedValue"/>.
        /// </summary>
        /// <param name="reader">The OData reader.</param>
        /// <returns>The <see cref="ODataNestedValue"/>.</returns>
        private static ODataNestedValue ReadNestedValue(ODataReader reader)
        {
            if (reader == null)
            {
                throw Error.ArgumentNull("reader");
            }

            ODataNestedValue topLevelItem = null;
            Stack<object> itemsStack = new Stack<object>();

            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceStart:
                        ODataResource resource = (ODataResource)reader.Item;
                        ODataNestedResourceValue resourceWrapper = null;
                        if (resource != null)
                        {
                            resourceWrapper = new ODataNestedResourceValue(resource);
                        }

                        if (itemsStack.Count == 0)
                        {
                            topLevelItem = resourceWrapper;
                        }
                        else
                        {
                            object parentItem = itemsStack.Peek();
                            ODataNestedResourceSetValue parentResourceSet = parentItem as ODataNestedResourceSetValue;
                            if (parentResourceSet != null)
                            {
                                if (parentResourceSet.NestedResources == null)
                                {
                                    parentResourceSet.NestedResources = new List<ODataNestedResourceValue>();
                                }

                                parentResourceSet.NestedResources.Add(resourceWrapper);
                            }
                            else
                            {
                                ODataNestedItem parentNestedItem = parentItem as ODataNestedItem;
                                Debug.Assert(parentNestedItem != null);

                                if (parentNestedItem.NestedValue == null)
                                {
                                    parentNestedItem.NestedValue = resourceWrapper;
                                }
                                else
                                {
                                    ODataNestedResourceSetValue nestedResourceSet = parentNestedItem.NestedValue as ODataNestedResourceSetValue;
                                    if (nestedResourceSet != null)
                                    {
                                        if (nestedResourceSet.NestedResources == null)
                                        {
                                            nestedResourceSet.NestedResources = new List<ODataNestedResourceValue>();
                                        }

                                        nestedResourceSet.NestedResources.Add(resourceWrapper);
                                    }
                                    else
                                    {
                                        throw new Exception("Wrong");
                                    }
                                }
                            }
                        }

                        itemsStack.Push(resourceWrapper);
                        break;

                    case ODataReaderState.ResourceEnd:
                        itemsStack.Pop();
                        break;

                    case ODataReaderState.NestedResourceInfoStart:
                        ODataNestedResourceInfo nestedResourceInfo = (ODataNestedResourceInfo)reader.Item;

                        ODataNestedItem nestedResourceInfoWrapper = new ODataNestedItem(nestedResourceInfo);

                        ODataNestedResourceValue parentResource = itemsStack.Peek() as ODataNestedResourceValue;
                        if (parentResource != null)
                        {
                            if (parentResource.NestedItems == null)
                            {
                                parentResource.NestedItems = new List<ODataNestedItem>();
                            }

                            parentResource.NestedItems.Add(nestedResourceInfoWrapper);
                        }

                        itemsStack.Push(nestedResourceInfoWrapper);
                        break;

                    case ODataReaderState.NestedResourceInfoEnd:
                        itemsStack.Pop();
                        break;

                    case ODataReaderState.ResourceSetStart:
                        ODataResourceSet resourceSet = (ODataResourceSet)reader.Item;

                        ODataNestedResourceSetValue resourceSetWrapper = new ODataNestedResourceSetValue(resourceSet);

                        if (itemsStack.Count > 0)
                        {
                            ODataNestedItem parentNestedResourceInfo = itemsStack.Peek() as ODataNestedItem;
                            if (parentNestedResourceInfo != null)
                            {
                                parentNestedResourceInfo.NestedValue = resourceSetWrapper;
                            }
                        }
                        else
                        {
                            topLevelItem = resourceSetWrapper;
                        }

                        itemsStack.Push(resourceSetWrapper);
                        break;

                    case ODataReaderState.ResourceSetEnd:
                        itemsStack.Pop();
                        break;

                    case ODataReaderState.EntityReferenceLink:
                    default:
                        break;
                }
            }

            return topLevelItem;
        }
    }
}
