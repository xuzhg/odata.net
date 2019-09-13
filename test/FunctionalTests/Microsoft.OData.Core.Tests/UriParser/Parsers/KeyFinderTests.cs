﻿//---------------------------------------------------------------------
// <copyright file="KeyFinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    /// <summary>
    /// Unit tests for the KeyFinder class.
    /// </summary>
    public class KeyFinderTests
    {
        [Fact]
        public void CurrentNavigaitionPropertyMustBePopulated()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0,1", out key, false);
            Action callWithNullNavProp = () => KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty()
                },
                null,
                new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null));
            Assert.Throws<ArgumentNullException>("currentNavigationProperty", callWithNullNavProp);
        }

        [Fact]
        public void RawKeyValuesMustBePopulated()
        {

            Action callWithNullRawKey = () => KeyFinder.FindAndUseKeysFromRelatedSegment(
                null,
                new List<IEdmStructuralProperty>()
                {
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty()
                },
                ModelBuildingHelpers.BuildValidNavigationProperty(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null));
            Assert.Throws<ArgumentNullException>("rawKeyValuesFromUri", callWithNullRawKey);
        }

        [Fact]
        public void RawKeyWithMoreThanOnePositionalValueIsUnchanged()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0,1", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty(),
                    ModelBuildingHelpers.BuildValidPrimitiveProperty()
                },
                ModelBuildingHelpers.BuildValidNavigationProperty(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>(),
                    ModelBuildingHelpers.BuildValidEntityType(),
                    null));
            Assert.Same(key, newKey);
        }

        [Fact]
        public void IfValueExistsInTargetPropertiesAndNotExistingKeysItIsNotWritten()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property(),
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("Name", "Stuff")
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Same(key, newKey);
        }

        [Fact]
        public void IfValueExistsinExistingKeysButNotTargetPropertiesItIsNotWritten()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("0", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId2Property(),
                    HardCodedTestModel.GetLionAttackDatesProp()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Same(key, newKey);
        }

        [Fact]
        public void IfValueAlreadySpecifiedInRawKeyItIsNotOverwritten()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("ID1=6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Contains("ID1", newKey.NamedValues.Keys);
            Assert.Contains("6", newKey.NamedValues.Values);
        }

        [Fact]
        public void PositionalValueAddedAsMissingValueIfOnlyOneMissingValueExists()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Contains(newKey.NamedValues, kvp => kvp.Key == "ID1" && kvp.Value == "32");
            Assert.Contains(newKey.NamedValues, kvp => kvp.Key == "ID2" && kvp.Value == "6");
        }

        [Fact]
        public void PositionalValueNotAddedIfMoreThanOneMissingValueExists()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property(),
                    HardCodedTestModel.GetLionAttackDatesProp()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Same(key, newKey);
        }

        [Fact]
        public void PositionalValuesArrayIsCleared()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Empty(newKey.PositionalValues);
        }

        [Fact]
        public void AreValuesNamedIsAlwaysSet()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetPersonMyLionsNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.True(newKey.AreValuesNamed);
        }

        [Fact]
        public void LookForKeysOnBothCurrentNavPropAndPartnerIfItExists()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("6", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetDogLionWhoAteMeNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));

            Assert.Contains(newKey.NamedValues, kvp => kvp.Key == "ID1" && kvp.Value == "32");
            Assert.Contains(newKey.NamedValues, kvp => kvp.Key == "ID2" && kvp.Value == "6");
        }

        [Fact]
        public void LookForKeysOnBothCurrentNavPropAndPartnerIfItExistsWorksForTemplate()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("{6}", out key, true);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetLionId1Property(),
                    HardCodedTestModel.GetLionId2Property()
                },
                HardCodedTestModel.GetDogLionWhoAteMeNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));

            Assert.Contains(newKey.NamedValues, kvp => kvp.Key == "ID1" && kvp.Value == "32");
            Assert.Contains(newKey.NamedValues, kvp => kvp.Key == "ID2" && kvp.Value == "{6}");
        }

        [Fact]
        public void IfNoKeyExistsOnNavPropAndNoPartnerExistsKeyIsUnchanged()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetDogIdProp()
                },
                HardCodedTestModel.GetPersonMyDogNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Same(key, newKey);
        }

        [Fact]
        public void IfNoReferentialIntegrityConstraintExistsOnPartnerKeyIsUnchanged()
        {
            SegmentArgumentParser key;
            SegmentArgumentParser.TryParseKeysFromUri("", out key, false);
            var newKey = KeyFinder.FindAndUseKeysFromRelatedSegment(
                key,
                new List<IEdmStructuralProperty>()
                {
                    HardCodedTestModel.GetDogIdProp()
                },
                HardCodedTestModel.GetEmployeeOfficeDogNavProp(),
                new KeySegment(
                    new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>("ID", 32)
                    },
                    HardCodedTestModel.GetPersonType(),
                    HardCodedTestModel.GetPeopleSet()));
            Assert.Same(key, newKey);
        }
    }
}
