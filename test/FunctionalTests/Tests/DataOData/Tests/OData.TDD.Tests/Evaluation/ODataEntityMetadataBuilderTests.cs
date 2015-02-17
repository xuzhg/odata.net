﻿//---------------------------------------------------------------------
// <copyright file="ODataEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataEntityMetadataBuilderTests
    {
        private readonly ODataEntityMetadataBuilder builder = new TestBuilder();

        [TestMethod]
        public void GetStreamEditLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetStreamEditLinkShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void GetStreamReadLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetStreamReadLinkShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void GetNavigationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetNavigationLinkUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void GetAssociationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetAssociationLinkUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void GetOperationTargetUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetOperationTargetUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void GetOperationTitleShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetOperationTitleShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullAssociationLink()
        {
            this.builder.GetAssociationLinkUri("Fake", null, false).Should().BeNull();
            this.builder.GetAssociationLinkUri("Fake", null, true).Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullNavigationLink()
        {
            this.builder.GetNavigationLinkUri("Fake", null, false).Should().BeNull();
            this.builder.GetNavigationLinkUri("Fake", null, true).Should().BeNull();
        }
        [TestMethod]
        public void BaseBuilderShouldReturnNullAssociationLinkEvenWhenNonComputedLinkIsAvailable()
        {
            this.builder.GetAssociationLinkUri("Fake", new Uri("http://example.com/override"), false).Should().BeNull();
            this.builder.GetAssociationLinkUri("Fake", new Uri("http://example.com/override"), true).Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullNavigationLinkEvenWhenNonComputedLinkIsAvailable()
        {
            this.builder.GetNavigationLinkUri("Fake", new Uri("http://example.com/override"), false).Should().BeNull();
            this.builder.GetNavigationLinkUri("Fake", new Uri("http://example.com/override"), true).Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullOperationTarget()
        {
            this.builder.GetOperationTargetUri("Fake", null, null).Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullOperationTitle()
        {
            this.builder.GetOperationTitle("Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullStreamEditLinkForStreamProperty()
        {
            this.builder.GetStreamEditLink("Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullStreamEditLinkForDefaultStream()
        {
            this.builder.GetStreamEditLink(null).Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullStreamReadLinkForStreamProperty()
        {
            this.builder.GetStreamReadLink("Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullStreamReadLinkForDefaultStream()
        {
            this.builder.GetStreamReadLink(null).Should().BeNull();
        }

        private class TestBuilder : ODataEntityMetadataBuilder
        {
            internal override System.Uri GetEditLink()
            {
                return new Uri("http://example.com/edit/link");
            }

            internal override System.Uri GetReadLink()
            {
                return new Uri("http://example.com/read/link");
            }

            internal override string GetETag()
            {
                return "etag value";
            }

            internal override Uri GetId()
            {
                return new Uri("http://idvalue");
            }

            internal override bool TryGetIdForSerialization(out Uri id)
            {
                id = null;
                return false;
            }
        }
    }
}
