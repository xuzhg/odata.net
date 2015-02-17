﻿//---------------------------------------------------------------------
// <copyright file="OasisRelationshipChangesAcceptanceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Edm.Strings;

    [TestClass]
    public class OasisRelationshipChangesAcceptanceTests
    {
        private const string RepresentativeEdmxDocument = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ForeignKeyId1"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyId2"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyProperty"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""navigation"" Type=""Collection(Test.EntityType)"" Partner=""NAVIGATION"" ContainsTarget=""true"">
          <OnDelete Action=""Cascade"" />
        </NavigationProperty>
        <NavigationProperty Name=""NAVIGATION"" Type=""Test.EntityType"" Partner=""navigation"">
          <ReferentialConstraint Property=""ForeignKeyId2"" ReferencedProperty=""ID2"" />
          <ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""ID1"" />
        </NavigationProperty>
        <NavigationProperty Name=""NonKeyPrincipalNavigation"" Type=""Test.EntityType"" Partner=""OtherNavigation"">
          <ReferentialConstraint Property=""ForeignKeyProperty"" ReferencedProperty=""ID1"" />
        </NavigationProperty>
        <NavigationProperty Name=""OtherNavigation"" Type=""Collection(Test.EntityType)"" Partner=""NonKeyPrincipalNavigation"" />
      </EntityType>
      <EntityType Name=""DerivedEntityType"" BaseType=""Test.EntityType"">
        <NavigationProperty Name=""DerivedNavigation"" Type=""Test.DerivedEntityType"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""EntitySet1"" EntityType=""Test.EntityType"">
          <NavigationPropertyBinding Path=""navigation"" Target=""EntitySet1"" />
          <NavigationPropertyBinding Path=""NAVIGATION"" Target=""EntitySet1"" />
          <NavigationPropertyBinding Path=""NonKeyPrincipalNavigation"" Target=""EntitySet1"" />
          <NavigationPropertyBinding Path=""Test.DerivedEntityType/DerivedNavigation"" Target=""EntitySet1"" />
        </EntitySet>
        <EntitySet Name=""EntitySet2"" EntityType=""Test.EntityType"">
          <NavigationPropertyBinding Path=""navigation"" Target=""EntitySet2"" />
          <NavigationPropertyBinding Path=""NAVIGATION"" Target=""EntitySet2"" />
          <NavigationPropertyBinding Path=""NonKeyPrincipalNavigation"" Target=""EntitySet2"" />
          <NavigationPropertyBinding Path=""Test.DerivedEntityType/DerivedNavigation"" Target=""EntitySet2"" />
        </EntitySet>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private readonly IEdmModel representativeModel;
        private readonly IEdmEntitySet entitySet1;
        private readonly IEdmEntitySet entitySet2;
        private readonly IEdmEntityType entityType;
        private readonly IEdmNavigationProperty navigation1;
        private readonly IEdmNavigationProperty navigation2;
        private readonly IEdmNavigationProperty nonKeyPrincipalNavigation;
        private readonly IEdmNavigationProperty derivedNavigation;

        public OasisRelationshipChangesAcceptanceTests()
        {
            this.representativeModel = EdmxReader.Parse(XElement.Parse(RepresentativeEdmxDocument).CreateReader());
            var container = this.representativeModel.EntityContainer;
            this.entitySet1 = container.FindEntitySet("EntitySet1");
            this.entitySet2 = container.FindEntitySet("EntitySet2");
            this.entityType = this.representativeModel.FindType("Test.EntityType") as IEdmEntityType;

            this.entitySet1.Should().NotBeNull();
            this.entitySet2.Should().NotBeNull();
            this.entityType.Should().NotBeNull();

            this.navigation1 = this.entityType.FindProperty("navigation") as IEdmNavigationProperty;
            this.navigation2 = this.entityType.FindProperty("NAVIGATION") as IEdmNavigationProperty;
            nonKeyPrincipalNavigation = this.entityType.FindProperty("NonKeyPrincipalNavigation") as IEdmNavigationProperty;

            var derivedType = this.representativeModel.FindType("Test.DerivedEntityType") as IEdmEntityType;
            derivedType.Should().NotBeNull();
            this.derivedNavigation = derivedType.FindProperty("DerivedNavigation") as IEdmNavigationProperty;

            this.navigation1.Should().NotBeNull();
            this.navigation2.Should().NotBeNull();
            this.derivedNavigation.Should().NotBeNull();
        }

        [TestMethod]
        public void RepresentativeModelShouldBeValid()
        {
            IEnumerable<EdmError> errors;
            this.representativeModel.Validate(out errors).Should().BeTrue();
            errors.Should().BeEmpty();
        }

        [TestMethod]
        public void FindNavigationTargetShouldUseBinding()
        {
            this.entitySet2.FindNavigationTarget(this.navigation2).Should().BeSameAs(this.entitySet2);
            this.entitySet1.FindNavigationTarget(this.derivedNavigation).Should().BeSameAs(this.entitySet1);
        }

        [TestMethod]
        public void ReferenceNavigationPropertyTypeShouldContinueToWork()
        {
            this.navigation2.Type.Definition.Should().BeSameAs(this.entityType);
        }

        [TestMethod]
        public void CollectionNavigationPropertyTypeShouldContinueToWork()
        {
            this.navigation1.Type.TypeKind().Should().Be(EdmTypeKind.Collection);
            this.navigation1.Type.AsCollection().ElementType().Definition.Should().BeSameAs(this.entityType);
        }

        [TestMethod]
        public void OnDeleteShouldContinueToWork()
        {
            this.navigation1.OnDelete.Should().Be(EdmOnDeleteAction.Cascade);
            this.navigation2.OnDelete.Should().Be(EdmOnDeleteAction.None);
        }

        [TestMethod]
        public void ReferentialConstraintShouldContinueToWork()
        {
            this.navigation1.DependentProperties().Should().BeNull();
            this.navigation2.DependentProperties().Should().Contain(this.entityType.FindProperty("ForeignKeyId1") as IEdmStructuralProperty);
            this.navigation2.DependentProperties().Should().Contain(this.entityType.FindProperty("ForeignKeyId2") as IEdmStructuralProperty);
        }

        [TestMethod]
        public void ReferentialConstraintShouldWorkForNonKeyPrincipalProperties()
        {
            this.nonKeyPrincipalNavigation.DependentProperties().Should().Contain(this.entityType.FindProperty("ForeignKeyProperty") as IEdmStructuralProperty);
        }

        [TestMethod]
        public void IsPrincipalShouldContinueToWork()
        {
            this.navigation1.IsPrincipal().Should().BeTrue();
            this.navigation2.IsPrincipal().Should().BeFalse();
        }

        [TestMethod]
        public void PartnerShouldContinueToWork()
        {
            this.navigation1.Partner.Should().BeSameAs(this.navigation2);
            this.navigation2.Partner.Should().BeSameAs(this.navigation1);
        }

        [TestMethod]
        public void ContainsTargetShouldContinueToWork()
        {
            this.navigation1.ContainsTarget.Should().BeTrue();
            this.navigation2.ContainsTarget.Should().BeFalse();
        }

        [TestMethod]
        public void NavigationTargetMappingsShouldContainAllBindings()
        {
            this.entitySet1.NavigationPropertyBindings.Should()
                .HaveCount(4)
                .And.Contain(m => m.NavigationProperty == this.navigation1 && m.Target == this.entitySet1)
                .And.Contain(m => m.NavigationProperty == this.navigation2 && m.Target == this.entitySet1)
                .And.Contain(m => m.NavigationProperty == this.derivedNavigation && m.Target == this.entitySet1);
            this.entitySet2.NavigationPropertyBindings.Should()
                .HaveCount(4)
                .And.Contain(m => m.NavigationProperty == this.navigation1 && m.Target == this.entitySet2)
                .And.Contain(m => m.NavigationProperty == this.navigation2 && m.Target == this.entitySet2)
                .And.Contain(m => m.NavigationProperty == this.derivedNavigation && m.Target == this.entitySet2);
        }

        [TestMethod]
        public void WriterShouldContinueToWork()
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                IEnumerable<EdmError> errors;
                EdmxWriter.TryWriteEdmx(this.representativeModel, writer, EdmxTarget.OData, out errors).Should().BeTrue();
                errors.Should().BeEmpty();
                writer.Flush();
            }

            string actual = builder.ToString();
            var actualXml = XElement.Parse(actual);
            var actualNormalized = actualXml.ToString();

            actualNormalized.Should().Be(RepresentativeEdmxDocument);
        }

        [TestMethod]
        public void ValidationShouldFailIfABindingToANonExistentPropertyIsFound()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""NonExistent"" Target=""EntitySet"" />",
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("NonExistent", "Test.EntityType"));
        }

        [TestMethod]
        public void ValidationShouldFailIfABindingToANonExistentSetIsFound()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" Target=""NonExistent"" />",
                EdmErrorCode.BadUnresolvedEntitySet,
                ErrorStrings.Bad_UnresolvedEntitySet("NonExistent"));
        }

        [TestMethod]
        public void ValidationShouldFailIfAContainerQualifiedNameIsUsedForTheTargetOfABinding()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" Target=""Container.EntitySet"" />",
                EdmErrorCode.BadUnresolvedEntitySet,
                ErrorStrings.Bad_UnresolvedEntitySet("Container.EntitySet"));
        }

        [TestMethod]
        public void ValidationShouldFailIfADerivedPropertyIsUsedWithoutATypeCast()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""DerivedNavigation"" Target=""EntitySet"" />",
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("DerivedNavigation", "Test.EntityType"));
        }

        [TestMethod]
        public void ValidationShouldFailIfATypeCastIsFollowedByANonExistentProperty()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Test.DerivedEntityType/NonExistent"" Target=""EntitySet"" />",
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("Test.DerivedEntityType/NonExistent", "Test.EntityType"));
        }

        [TestMethod]
        public void ParsingShouldFailIfABindingIsMissingTarget()
        {
            this.ParseBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Target", "NavigationPropertyBinding"));
        }

        [TestMethod]
        public void ParsingShouldFailIfABindingIsMissingPath()
        {
            this.ParseBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Target=""EntitySet"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Path", "NavigationPropertyBinding"));
        }

        [TestMethod]
        public void ParsingShouldFailIfABindingHasExtraAttributes()
        {
            this.ParseBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" Target=""EntitySet"" Something=""else"" Foo=""bar"" />",
                EdmErrorCode.UnexpectedXmlAttribute,
                ErrorStrings.XmlParser_UnexpectedAttribute("Something"),
                ErrorStrings.XmlParser_UnexpectedAttribute("Foo"));
        }

        [TestMethod]
        public void ParsingShouldFailIfABindingHasAnnotations()
        {
            const string invalidBinding = @"
              <NavigationPropertyBinding Path=""Navigation"" Target=""EntitySet"">
                <Annotation Term=""FQ.NS.Term""/>
              </NavigationPropertyBinding>";

            this.ParseBindingWithExpectedErrors(
                invalidBinding,
                EdmErrorCode.UnexpectedXmlElement, 
                ErrorStrings.XmlParser_UnexpectedElement("Annotation"));
        }

        [TestMethod]
        public void ParsingShouldFailIfAConstraintIsMissingProperty()
        {
            this.ParseReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint ReferencedProperty=""ID1"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Property", "ReferentialConstraint"));
        }

        [TestMethod]
        public void ParsingShouldFailIfAConstraintIsMissingReferencedProperty()
        {
            this.ParseReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint Property=""ForeignKeyId1"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("ReferencedProperty", "ReferentialConstraint"));
        }

        [TestMethod]
        public void ParsingShouldFailIfAConstraintHasExtraAttributes()
        {
            this.ParseReferentialConstraintWithExpectedErrors(
                @"
              <ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""ID1"" Something=""else"" Foo=""bar"" />",
                EdmErrorCode.UnexpectedXmlAttribute,
                ErrorStrings.XmlParser_UnexpectedAttribute("Something"),
                ErrorStrings.XmlParser_UnexpectedAttribute("Foo"));
        }

        [TestMethod]
        public void ParsingShouldFailIfAConstraintHasAnnotations()
        {
            const string invalidConstraint = @"
              <ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""ID1"">
                <Annotation Term=""FQ.NS.Term""/>
              </ReferentialConstraint>";

            this.ParseReferentialConstraintWithExpectedErrors(
                invalidConstraint,
                EdmErrorCode.UnexpectedXmlElement, 
                ErrorStrings.XmlParser_UnexpectedElement("Annotation"));
        }
        
        [TestMethod]
        public void ValidationShouldFailIfAConstraintOnANonExistentPropertyIsFound()
        {
            this.ValidateReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint Property=""NonExistent"" ReferencedProperty=""ID1"" />",
                EdmErrorCode.BadUnresolvedProperty,
                ErrorStrings.Bad_UnresolvedProperty("NonExistent")
                );
        }

        [TestMethod]
        public void ValidationShouldFailIfAConstraintOnANonExistentReferencedPropertyIsFound()
        {
            this.ValidateReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""NonExistent"" />",
                EdmErrorCode.BadUnresolvedProperty,
                ErrorStrings.Bad_UnresolvedProperty("NonExistent"));
        }

        [TestMethod]
        public void ParsingShouldFailIfANavigationHasMultipleOnDeleteElements()
        {
            this.ParseNavigationExpectedErrors(
                @"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"">
                    <OnDelete Action=""Cascade"" />
                    <OnDelete Action=""None"" />
                  </NavigationProperty>",
                EdmErrorCode.UnexpectedXmlElement,
                ErrorStrings.XmlParser_UnusedElement("OnDelete"));
        }

        [TestMethod]
        public void ParsingShouldFailIfANavigationHasAnInvalidOnDeleteAction()
        {
            this.ParseNavigationExpectedErrors(
                @"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"">
                    <OnDelete Action=""Foo"" />
                  </NavigationProperty>",
                EdmErrorCode.InvalidOnDelete,
                ErrorStrings.CsdlParser_InvalidDeleteAction("Foo"));
        }

        [TestMethod]
        public void ParsingShouldFailIfANavigationIsMissingType()
        {
            this.ParseNavigationExpectedErrors(
                @"<NavigationProperty Name=""Navigation"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Type", "NavigationProperty"));
        }

        [TestMethod]
        public void ParsingShouldNotFailIfANavigationIsMissingPartner()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Test.EntityType)"" />");
        }

        [TestMethod]
        public void ParsingShouldFailIfNavigationTypeIsEmpty()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type="""" />", 
                EdmErrorCode.InvalidTypeName,
                ErrorStrings.CsdlParser_InvalidTypeName(""));
        }

        [TestMethod]
        public void ParsingShouldFailIfNavigationNullableIsEmpty()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""""/>",
                EdmErrorCode.InvalidBoolean,
                ErrorStrings.ValueParser_InvalidBoolean(""));
        }

        [TestMethod]
        public void ParsingShouldFailIfNavigationNullableIsNotTrueOrFalse()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""foo""/>", 
                EdmErrorCode.InvalidBoolean,
                ErrorStrings.ValueParser_InvalidBoolean("foo"));
        }

        [TestMethod]
        public void ValidationShouldFailIfNavigationNullableIsSpecifiedOnCollection()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Test.EntityType)"" Nullable=""false""/>", 
                EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute,
                ErrorStrings.CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType);
        }

        [TestMethod]
        public void ValidationShouldFailIfNavigationTypeIsAPrimitiveType()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Edm.Int32"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Edm.Int32"));
        }

        [TestMethod]
        public void ValidationShouldFailIfNavigationTypeIsPrimitiveCollectionType()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Edm.Int32)"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Edm.Int32"));
        }

        [TestMethod]
        public void ValidationShouldFailIfNavigationTypeDoesNotExist()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Fake.Nonexistent"" />",
                EdmErrorCode.BadUnresolvedEntityType, 
                ErrorStrings.Bad_UnresolvedEntityType("Fake.Nonexistent"));
        }

        [TestMethod]
        public void ValidationShouldFailIfNavigationTypeIsACollectionButElementTypeDoesNotExist()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Fake.Nonexistent)"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Fake.Nonexistent"));
        }

        [TestMethod]
        public void ValidationShouldFailIfNavigationParterIsSpecifiedButCannotBeFound()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Partner=""Nonexistent"" />", 
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("Nonexistent", "Test.EntityType"));
        }

        private void ValidateBindingWithExpectedErrors(string bindingText, EdmErrorCode errorCode, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""EntitySet"" EntityType=""Test.EntityType"">
          {0}
        </EntitySet>
      </EntityContainer>
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Nullable=""false"" Type=""Edm.Int32""/>
        <NavigationProperty Name=""Navigation"" Type=""Collection(Test.EntityType)"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, bindingText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            EdmxReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();
            
            model.Validate(out errors).Should().BeFalse();
            errors.Should().HaveCount(messages.Length);
            foreach (var message in messages)
            {
                errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
            }
        }

        private void ValidateReferentialConstraintWithExpectedErrors(string referentialConstraintText, EdmErrorCode errorCode, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ForeignKeyId1"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyId2"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""true"">
          {0}
        </NavigationProperty>
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, referentialConstraintText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            EdmxReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();
            
            model.Validate(out errors).Should().BeFalse();
            errors.Should().HaveCount(messages.Length);
            foreach (var message in messages)
            {
                errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
            }
        }

        private void ValidateNavigationWithExpectedErrors(string navigationText, EdmErrorCode? errorCode = null, string message = null)
        {
            if (errorCode != null)
            {
                ValidateNavigationWithExpectedErrors(navigationText, new[] { errorCode.Value }, new[] { message });
            }
            else
            {
                ValidateNavigationWithExpectedErrors(navigationText, new EdmErrorCode[0], new string[0]); 
            }
        }

        private void ValidateNavigationWithExpectedErrors(string navigationText, EdmErrorCode[] errorCodes, string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {0}
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, navigationText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            EdmxReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();

            bool result = model.Validate(out errors);

            if (errorCodes.Length > 0)
            {
                result.Should().BeFalse();

                errors.Should().HaveCount(messages.Length);
                for (int i = 0; i < messages.Length; i++)
                {
                    errors.Should().Contain(e => e.ErrorCode == errorCodes[i] && e.ErrorMessage == messages[i]);
                }
            }
            else
            {
                result.Should().BeTrue();
                errors.Should().BeEmpty();
            }
        }

        private void ParseBindingWithExpectedErrors(string bindingText, EdmErrorCode errorCode, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""EntitySet"" EntityType=""Test.EntityType"">
          {0}
        </EntitySet>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, bindingText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            EdmxReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeFalse();
            
            errors.Should().HaveCount(messages.Length);
            foreach (var message in messages)
            {
                errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
            }
        }

        private void ParseReferentialConstraintWithExpectedErrors(string referentialConstraintText, EdmErrorCode errorCode, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ForeignKeyId1"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyId2"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""true"">
          {0}
        </NavigationProperty>
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, referentialConstraintText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            EdmxReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeFalse();
            
            errors.Should().HaveCount(messages.Length);
            foreach (var message in messages)
            {
                errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
            }
        }

        private void ParseNavigationExpectedErrors(string navigationText, EdmErrorCode[] errorCodes, string[] messages)
        {
            errorCodes.Length.Should().Be(messages.Length);
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {0}
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, navigationText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
           
            bool result = EdmxReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            if (errorCodes.Length > 0)
            {
                result.Should().BeFalse();

                errors.Should().HaveCount(messages.Length);
                for (int i = 0; i < messages.Length; i++)
                {
                    errors.Should().Contain(e => e.ErrorCode == errorCodes[i] && e.ErrorMessage == messages[i]);
                }
            }
            else
            {
                result.Should().BeTrue();
                errors.Should().BeEmpty();
            }
        }

        private void ParseNavigationExpectedErrors(string navigationText, EdmErrorCode? errorCode = null, string message = null)
        {
            if (errorCode != null)
            {
                ParseNavigationExpectedErrors(navigationText, new[] { errorCode.Value }, new[] { message });   
            }
            else
            {
                ParseNavigationExpectedErrors(navigationText, new EdmErrorCode[0], new string[0]);   
            }
        }
    }
}
