﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValueSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests and short-span integration tests for ODataJsonLightValueSerializer.
    /// </summary>
    [TestClass]
    public class ODataJsonLightValueSerializerTests
    {
        private EdmModel model;
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        [TestInitialize]
        public void Init()
        {
            model = new EdmModel();
            stream = new MemoryStream();
            settings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://example.com"));
        }

        // Regression for: Validates when no metadata is specified WriteCollection will fail
        [TestMethod()]
        public void WritingCollectionValueShouldFailWhenNoTypeSpecified()
        {
            var serializer = CreateODataJsonLightValueSerializer(false);

            var collectionValue = new ODataCollectionValue() {Items = new List<string>()};

            Action test = () => serializer.WriteCollectionValue(collectionValue, null, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest);
        }

        [TestMethod]
        public void WritingCollectionValueDifferingFromExpectedTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>(), TypeName = "Collection(Edm.Int32)" };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"));
        }

        [TestMethod]
        public void WritingCollectionValueDifferingFromExpectedTypeShouldFail_WithoutEdmPrefix()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue() { Items = new List<string>(), TypeName = "Collection(Int32)" };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"));
        }

        [TestMethod]
        public void WritingCollectionValueElementWithNullValue()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue()
                {
                    Items = new int?[] { null },
                    TypeName = "Collection(Edm.Int32)"
                };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
            serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);
        }

        [TestMethod]
        public void WritingCollectionValueWithNullValueForNonNullableElementTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var collectionValue = new ODataCollectionValue()
            {
                Items = new int?[] { null },
                TypeName = "Collection(Edm.Int32)"
            };

            var stringCollectionTypeRef = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false));
            Action test = () => serializer.WriteCollectionValue(collectionValue, stringCollectionTypeRef, false, false, false);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_NonNullableCollectionElementsMustNotBeNull);
        }

        [TestMethod]
        public void WritingTypeDefinitionValueWithValueOfIncompatibleTypeShouldFail()
        {
            var serializer = CreateODataJsonLightValueSerializer(true);

            var uint64 = model.GetUInt64("NS", true);

            Action test = () => serializer.WritePrimitiveValue("123", uint64);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.String", true, "NS.UInt64", true));
        }

        [TestMethod]
        public void WritingInstanceAnnotationInComplexValueShouldWrite()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var complexValue = new ODataComplexValue {TypeName = "TestNamespace.Address", InstanceAnnotations = new Collection<ODataInstanceAnnotation> {new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(true))}};

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteComplexValue(complexValue, complexTypeRef, false, false, new DuplicatePropertyNamesChecker(false, true));
            });

            result.Should().Contain("\"@Is.ReadOnly\":true");
        }

        [TestMethod]
        public void WritingMultipleInstanceAnnotationInComplexValueShouldWrite()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var complexValue = new ODataComplexValue
                {
                    TypeName = "TestNamespace.Address",
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)),
                        new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123)),
                        new ODataInstanceAnnotation("Annotation.3", new ODataPrimitiveValue("annotation"))
                    }
                };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteComplexValue(complexValue, complexTypeRef, false, false, new DuplicatePropertyNamesChecker(false, true));
            });

            result.Should().Contain("\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\"");
        }

        [TestMethod]
        public void WritingMultipleInstanceAnnotationInComplexValueShouldSkipBaseOnSettings()
        {
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var result = this.SetupSerializerAndRunTest(serializer =>
            {
                var complexValue = new ODataComplexValue
                {
                    TypeName = "TestNamespace.Address",
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)),
                        new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123)),
                        new ODataInstanceAnnotation("Annotation.3", new ODataPrimitiveValue("annotation"))
                    }
                };

                var complexTypeRef = new EdmComplexTypeReference(complexType, false);
                serializer.WriteComplexValue(complexValue, complexTypeRef, false, false, new DuplicatePropertyNamesChecker(false, true));
            });

            result.Should().NotContain("\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\"");
        }

        private ODataJsonLightValueSerializer CreateODataJsonLightValueSerializer(bool writingResponse)
        {
            var context = new ODataJsonLightOutputContext(ODataFormat.Json, stream, new ODataMediaType("application", "json"), Encoding.Default, settings, writingResponse, true, model, null);
            var serializer = new ODataJsonLightValueSerializer(context);
            return serializer;
        }

        /// <summary>
        /// Sets up a ODataJsonLightSerializer, runs the given test code, and then flushes and reads the stream back as a string for
        /// customized verification.
        /// </summary>
        private string SetupSerializerAndRunTest(Action<ODataJsonLightValueSerializer> action)
        {
            var serializer = CreateODataJsonLightValueSerializer(true);
            action(serializer);
            serializer.JsonWriter.Flush();
            stream.Position = 0;
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
