﻿//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseFinisherTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class SelectExpandClauseFinisherTests
    {
        [TestMethod]
        public void ExpandedNavigationPropertiesAreImplicitlyAddedAsPathSelectionItemsIfSelectIsPopulated()
        {
            IEdmNavigationProperty navigationProperty = ModelBuildingHelpers.BuildValidNavigationProperty();
            SelectExpandClause clause = new SelectExpandClause(new SelectItem[]
            {
                new PathSelectItem(new ODataSelectPath(new PropertySegment(ModelBuildingHelpers.BuildValidPrimitiveProperty()))),
                new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(navigationProperty, ModelBuildingHelpers.BuildValidEntitySet())), ModelBuildingHelpers.BuildValidEntitySet(), new SelectExpandClause(new List<SelectItem>(), false)), 
            },
            false /*allSelected*/);
            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);
            clause.SelectedItems.Should().HaveCount(3)
                .And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath.LastSegment is NavigationPropertySegment && x.As<PathSelectItem>().SelectedPath.LastSegment.As<NavigationPropertySegment>().NavigationProperty.Name == navigationProperty.Name);
        }

        [TestMethod]
        public void ExpandedNavigationPropertiesAreNotAddedAsPathSelectionItemsIfSelectIsNotPopulated()
        {
            SelectExpandClause clause = new SelectExpandClause(new SelectItem[]
            {
                new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), ModelBuildingHelpers.BuildValidEntitySet())), ModelBuildingHelpers.BuildValidEntitySet(), new SelectExpandClause(new List<SelectItem>(), false)), 
            },
            false /*allSelected*/);
            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);
            clause.SelectedItems.Should().HaveCount(1);
        }
    }
}
