//---------------------------------------------------------------------
// <copyright file="ODataUriUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using FluentAssertions;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using System.Linq;

namespace Microsoft.OData.Tests.Query
{
    public class ODataUriUtilsTests
    {
        [Fact]
        public void TestDecimalConvertToUriLiteral()
        {
            string decimalString = ODataUriUtils.ConvertToUriLiteral(decimal.MaxValue, ODataVersion.V4);
            decimalString.Should().Be("79228162514264337593543950335");

            decimalString = ODataUriUtils.ConvertToUriLiteral(decimal.MinValue, ODataVersion.V4);
            decimalString.Should().Be("-79228162514264337593543950335");

            decimalString = ODataUriUtils.ConvertToUriLiteral(112M, ODataVersion.V4);
            decimalString.Should().Be("112");
        }

        [Fact]
        public void TestDecimalConvertFromUriLiteral()
        {
            object dec = ODataUriUtils.ConvertFromUriLiteral("79228162514264337593543950335", ODataVersion.V4);
            (dec is decimal).Should().Be(true);

            dec = ODataUriUtils.ConvertFromUriLiteral("-79228162514264337593543950335", ODataVersion.V4);
            (dec is decimal).Should().Be(true);

            dec = ODataUriUtils.ConvertFromUriLiteral("112M", ODataVersion.V4);
            (dec is decimal).Should().Be(true);
        }

        [Fact]
        public void TestLongConvertToUriLiteral()
        {
            string longString = ODataUriUtils.ConvertToUriLiteral(long.MaxValue, ODataVersion.V4);
            longString.Should().Be("9223372036854775807");

            longString = ODataUriUtils.ConvertToUriLiteral(long.MinValue, ODataVersion.V4);
            longString.Should().Be("-9223372036854775808");

            longString = ODataUriUtils.ConvertToUriLiteral(123L, ODataVersion.V4);
            longString.Should().Be("123");
        }

        [Fact]
        public void TestLongConvertFromUriLiteral()
        {
            object longNumber = ODataUriUtils.ConvertFromUriLiteral("9223372036854775807", ODataVersion.V4);
            (longNumber is long).Should().Be(true);

            longNumber = ODataUriUtils.ConvertFromUriLiteral("-9223372036854775808", ODataVersion.V4);
            (longNumber is long).Should().Be(true);

            longNumber = ODataUriUtils.ConvertFromUriLiteral("123L", ODataVersion.V4);
            (longNumber is long).Should().Be(true);
        }

        [Fact]
        public void TestSingleConvertToUriLiteral()
        {
            string singleString = ODataUriUtils.ConvertToUriLiteral(float.MaxValue, ODataVersion.V4);
            singleString.Should().Be("3.40282347E+38");

            singleString = ODataUriUtils.ConvertToUriLiteral(float.MinValue, ODataVersion.V4);
            singleString.Should().Be("-3.40282347E+38");

            singleString = ODataUriUtils.ConvertToUriLiteral(1000000000000f, ODataVersion.V4);
            singleString.Should().Be("1E+12");
        }

        [Fact]
        public void TestSingleConvertFromUriLiteral()
        {
            object singleNumber = ODataUriUtils.ConvertFromUriLiteral("3.40282347E+38", ODataVersion.V4);
            (singleNumber is float).Should().Be(true);

            singleNumber = ODataUriUtils.ConvertFromUriLiteral("-3.40282347E+38", ODataVersion.V4);
            (singleNumber is float).Should().Be(true);

            singleNumber = ODataUriUtils.ConvertFromUriLiteral("1000000000000f", ODataVersion.V4);
            (singleNumber is float).Should().Be(true);
        }

        [Fact]
        public void TestDoubleConvertToUriLiteral()
        {
            string doubleString = ODataUriUtils.ConvertToUriLiteral(double.MaxValue, ODataVersion.V4);
            doubleString.Should().Be("1.7976931348623157E+308");

            doubleString = ODataUriUtils.ConvertToUriLiteral(double.MinValue, ODataVersion.V4);
            doubleString.Should().Be("-1.7976931348623157E+308");

            doubleString = ODataUriUtils.ConvertToUriLiteral(1000000000000D, ODataVersion.V4);
            doubleString.Should().Be("1000000000000.0");
        }

        [Fact]
        public void TestDoubleConvertFromUriLiteral()
        {
            object doubleNumber = ODataUriUtils.ConvertFromUriLiteral("1.7976931348623157E+308", ODataVersion.V4);
            (doubleNumber is double).Should().Be(true);

            doubleNumber = ODataUriUtils.ConvertFromUriLiteral("-1.7976931348623157E+308", ODataVersion.V4);
            (doubleNumber is double).Should().Be(true);

            doubleNumber = ODataUriUtils.ConvertFromUriLiteral("1000000000000D", ODataVersion.V4);
            (doubleNumber is double).Should().Be(true);
        }

        [Fact]
        public void TestBinaryConvertToUriLiteral()
        {
            byte[] value1 = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            string binaryString = ODataUriUtils.ConvertToUriLiteral(value1, ODataVersion.V4);
            binaryString.Should().Be("binary'AAECAwQFBgc='");

            byte[] value2 = new byte[] { 0x3, 0x1, 0x4, 0x1, 0x5, 0x9, 0x2, 0x6, 0x5, 0x3, 0x5, 0x9 };
            binaryString = ODataUriUtils.ConvertToUriLiteral(value2, ODataVersion.V4);
            binaryString.Should().Be("binary'AwEEAQUJAgYFAwUJ'");

            binaryString = ODataUriUtils.ConvertToUriLiteral(new byte[] { }, ODataVersion.V4);
            binaryString.Should().Be("binary''");
        }

        [Fact]
        public void TestBinaryConvertFromUriLiteral()
        {
            byte[] value1 = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7 };
            byte[] result = ODataUriUtils.ConvertFromUriLiteral("binary'AAECAwQFBgc='", ODataVersion.V4) as byte[];
            result.Should().BeEquivalentTo(value1);

            byte[] value2 = new byte[] { 0x3, 0x1, 0x4, 0x1, 0x5, 0x9, 0x2, 0x6, 0x5, 0x3, 0x5, 0x9 };
            result = ODataUriUtils.ConvertFromUriLiteral("binary'AwEEAQUJAgYFAwUJ'", ODataVersion.V4) as byte[];
            result.Should().BeEquivalentTo(value2);

            result = ODataUriUtils.ConvertFromUriLiteral("binary''", ODataVersion.V4) as byte[];
            result.Should().BeEmpty();

            // Invalid base64 value.
            Action action = () => { ODataUriUtils.ConvertFromUriLiteral("binary'AwEEAQUJAgYFAwUJ='", ODataVersion.V4); };
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_UnrecognizedLiteral("Edm.Binary", "binary'AwEEAQUJAgYFAwUJ='", "0", "binary'AwEEAQUJAgYFAwUJ='"));
        }

        #region enum testings
        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumName()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'Red'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            enumVal.Value.Should().Be(1L + "");
            enumVal.TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
        }

        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumName_Combined()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'Red,Solid,BlueYellowStriped'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            enumVal.Value.Should().Be(31L + "");
            enumVal.TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
        }

        [Fact]
        public void TestEnumConvertFromUriLiteral_EnumLong()
        {
            ODataEnumValue enumVal = (ODataEnumValue)ODataUriUtils.ConvertFromUriLiteral("Fully.Qualified.Namespace.ColorPattern'11'", ODataVersion.V4, HardCodedTestModel.TestModel, null);
            enumVal.Value.Should().Be(11L + "");
            enumVal.TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
        }

        [Fact]
        public void TestEnumConvertToUriLiteral_EnumValue()
        {
            var val = new ODataEnumValue(11L + "", "Fully.Qualified.Namespace.ColorPattern");
            string enumValStr = ODataUriUtils.ConvertToUriLiteral(val, ODataVersion.V4);
            enumValStr.Should().Be("Fully.Qualified.Namespace.ColorPattern'11'");
        }
        #endregion

        #region Date/DateTimeOffset
        [Fact]
        public void TestDateConvertFromUriLiteral()
        {
            Date dateValue = (Date)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetDate(false));
            dateValue.Should().Be(new Date(1997, 7, 1));

            DateTimeOffset dtoValue1 = (DateTimeOffset)ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetDateTimeOffset(false));
            dtoValue1.Should().Be(new DateTimeOffset(1997, 7, 1, 0, 0, 0, new TimeSpan(0)));

            var dtoValue2 = ODataUriUtils.ConvertFromUriLiteral("1997-07-01", ODataVersion.V4);
            dtoValue2.Should().Be(new Date(1997, 7, 1));
        }

        [Fact]
        public void TestDateTimeOffsetConvertFromUriLiteral()
        {
            // Date is not in right format
            Action action = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-1T12:12:12-11:00", ODataVersion.V4);
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-1T12:12:12-11:00"));

            // Time is not in right format
            Action action2 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:2-11:00", ODataVersion.V4);
            action2.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:2-11:00"));

            // Date and Time separator is incorrect
            // Call from DataUriUtils, it will parse till blank space which is a correct Date
            var dtoValue1 = ODataUriUtils.ConvertFromUriLiteral("1997-07-01 12:12:02-11:00", ODataVersion.V4);
            dtoValue1.Should().Be(new Date(1997, 7, 1));

            // Date is not with limit
            Action action4 = () => ODataUriUtils.ConvertFromUriLiteral("1997-13-01T12:12:12-11:00", ODataVersion.V4);
            action4.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-13-01T12:12:12-11:00"));

            // Time is not within limit
            Action action5 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:62-11:00", ODataVersion.V4);
            action5.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:62-11:00"));

            // Timezone separator is incorrect
            // Call from DataUriUtils, it will parse till blank space, so error message string is without timezone information.
            Action action6 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:02 11:00", ODataVersion.V4);
            action6.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:02"));

            // Timezone is not within limit
            Action action7 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:02-15:00", ODataVersion.V4);
            action7.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:02-15:00"));

            // Timezone is not specified
            Action action8 = () => ODataUriUtils.ConvertFromUriLiteral("1997-07-01T12:12:02", ODataVersion.V4);
            action8.ShouldThrow<ODataException>().WithMessage(Strings.UriUtils_DateTimeOffsetInvalidFormat("1997-07-01T12:12:02"));
        }

        [Fact]
        public void TestTimeOfDayConvertFromUriLiteral()
        {
            TimeOfDay timeValue1 = (TimeOfDay)ODataUriUtils.ConvertFromUriLiteral("12:13:14.015", ODataVersion.V4, HardCodedTestModel.TestModel, EdmCoreModel.Instance.GetTimeOfDay(false));
            timeValue1.Should().Be(new TimeOfDay(12, 13, 14, 15));

            TimeOfDay timeValue2 = (TimeOfDay)ODataUriUtils.ConvertFromUriLiteral("12:13:14.015", ODataVersion.V4);
            timeValue2.Should().Be(new TimeOfDay(12, 13, 14, 15));
        }

        [Fact]
        public void TestResourceConvertFromUriLiteral()
        {
            EdmModel model = new EdmModel();
            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(complex);
            var complexRef = new EdmComplexTypeReference(complex, false);
            string literal = @"{""@odata.type"":""#NS.Address"",""Street"":""Ave156"",""City"":""City11""}";

            var value = ODataUriUtils.ConvertFromUriLiteral(literal, ODataVersion.V4, model, complexRef);
            var resource = Assert.IsType<ODataNestedResourceValue>(value);
            Assert.NotNull(resource.Resource);
            Assert.Equal(2, resource.Resource.Properties.Count());
            ODataProperty property = resource.Resource.Properties.First();
            Assert.Equal("Street", property.Name);
            Assert.Equal("Ave156", property.Value);

            property = resource.Resource.Properties.Last();
            Assert.Equal("City", property.Name);
            Assert.Equal("City11", property.Value);
        }

        [Fact]
        public void TestResourceSetConvertFromUriLiteral()
        {
            EdmModel model = new EdmModel();
            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("Location", new EdmComplexTypeReference(complex, true));
            model.AddElement(complex);
            var complexRef = new EdmComplexTypeReference(complex, false);
            string literal = @"[{""Street"":""Ave156"",""City"":""City22""},{""Street"":""Ave156"",""City"":""Redmond"",""Location"":{""Street"":""Ave228"",""City"":""Sammamish""}}]";

            var collectionRef = new EdmCollectionTypeReference(new EdmCollectionType(complexRef));
            var value = ODataUriUtils.ConvertFromUriLiteral(literal, ODataVersion.V4, model, collectionRef);

            var setValue = Assert.IsType<ODataNestedResourceSetValue>(value);
            Assert.NotNull(setValue.ResourceSet);

            Assert.Equal(2, setValue.NestedResources.Count());
            ODataNestedResourceValue resourceValue = setValue.NestedResources[0];

            Assert.Equal(2, resourceValue.Resource.Properties.Count());
            ODataProperty property = resourceValue.Resource.Properties.First();
            Assert.Equal("Street", property.Name);
            Assert.Equal("Ave156", property.Value);

            property = resourceValue.Resource.Properties.Last();
            Assert.Equal("City", property.Name);
            Assert.Equal("City22", property.Value);

            // Item2
            resourceValue = setValue.NestedResources[1];

            // Street, City
            Assert.Equal(2, resourceValue.Resource.Properties.Count());
            property = resourceValue.Resource.Properties.First(c => c.Name == "Street");
            Assert.Equal("Ave156", property.Value);

            property = resourceValue.Resource.Properties.First(c => c.Name == "City");
            Assert.Equal("Redmond", property.Value);

            Assert.NotNull(resourceValue.NestedItems);
            var nestedItem = Assert.Single(resourceValue.NestedItems);
            Assert.NotNull(nestedItem.NestedResourceInfo);
            Assert.Equal("Location", nestedItem.NestedResourceInfo.Name);
            Assert.NotNull(nestedItem.NestedValue);
            var nestedResource = Assert.IsType<ODataNestedResourceValue>(nestedItem.NestedValue);

            Assert.Equal(2, nestedResource.Resource.Properties.Count());
            property = nestedResource.Resource.Properties.First(c => c.Name == "Street");
            Assert.Equal("Ave228", property.Value);

            property = nestedResource.Resource.Properties.First(c => c.Name == "City");
            Assert.Equal("Sammamish", property.Value);
        }

        [Fact]
        public void TestResourceConvertToUriLiteral()
        {
            EdmModel model = new EdmModel();
            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(complex);

            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Address"
            };

            resource.Properties = new[]
            {
                new ODataProperty { Name = "Street", Value = "Ave156" },
                new ODataProperty { Name = "City", Value = "Shanghai" }
            };

            var literal = ODataUriUtils.ConvertToUriLiteral(resource, ODataVersion.V4, model);
            Assert.Equal(@"{""@odata.type"":""#NS.Address"",""Street"":""Ave156"",""City"":""Shanghai""}", literal);
        }

        [Fact]
        public void TestResourceWithNestedResourceConvertFromUriLiteral()
        {
            EdmModel model = new EdmModel();
            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("Location", new EdmComplexTypeReference(complex, true));
            model.AddElement(complex);

            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "Ave156" },
                    new ODataProperty { Name = "City", Value = "Redmond" }
                }
            };

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Location",
                IsCollection = false
            };

            ODataResource nestedResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "Ave228" },
                    new ODataProperty { Name = "City", Value = "Samma" }
                }
            };

            ODataNestedResourceValue value = new ODataNestedResourceValue(resource)
            {
                NestedItems = new[] {
                    new ODataNestedItem(nestedResourceInfo)
                    {
                        NestedValue = new ODataNestedResourceValue(nestedResource)
                    }
                }
            };

            var literal = ODataUriUtils.ConvertToUriLiteral(value, ODataVersion.V4, model);
            Assert.Equal(@"{""@odata.type"":""#NS.Address"",""Street"":""Ave156"",""City"":""Redmond"",""Location"":{""Street"":""Ave228"",""City"":""Samma""}}", literal);
        }

        [Fact]
        public void TestResourceSetConvertToUriLiteral()
        {
            EdmModel model = new EdmModel();
            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            complex.AddStructuralProperty("Location", new EdmComplexTypeReference(complex, true));
            model.AddElement(complex);

            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "Ave156" },
                    new ODataProperty { Name = "City", Value = "Redmond" }
                }
            };

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Location",
                IsCollection = false
            };

            ODataResource nestedResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "Ave228" },
                    new ODataProperty { Name = "City", Value = "Samma" }
                }
            };

            ODataNestedResourceValue value = new ODataNestedResourceValue(resource)
            {
                NestedItems = new[] {
                    new ODataNestedItem(nestedResourceInfo)
                    {
                        NestedValue = new ODataNestedResourceValue(nestedResource)
                    }
                }
            };

            ODataResourceSet set = new ODataResourceSet()
            {
                TypeName = "Collection(NS.Address)"
            };
            ODataNestedResourceSetValue setValue = new ODataNestedResourceSetValue(set)
            {
                NestedResources = new[] { new ODataNestedResourceValue(resource), value }
            };

            var literal = ODataUriUtils.ConvertToUriLiteral(setValue, ODataVersion.V4, model);
            Assert.Equal(@"[{""Street"":""Ave156"",""City"":""Redmond""},{""Street"":""Ave156"",""City"":""Redmond"",""Location"":{""Street"":""Ave228"",""City"":""Samma""}}]",
                literal);
        }

        [Fact]
        public void TestCollectionConvertFromBracketCollection()
        {
            object collection = ODataUriUtils.ConvertFromUriLiteral("[1,2,3]", ODataVersion.V4, HardCodedTestModel.TestModel, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            IEnumerable items = collection.As<Microsoft.OData.ODataCollectionValue>().Items;
            items.Should().Equal(new int[] { 1, 2, 3 });
        }

        [Fact]
        public void TestCollectionConvertWithMismatchedBracket()
        {
            Action parse = () => ODataUriUtils.ConvertFromUriLiteral("[1,2,3)", ODataVersion.V4, HardCodedTestModel.TestModel, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            parse.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Strings.ExpressionLexer_UnbalancedBracketExpression);
        }
        #endregion
    }
}
