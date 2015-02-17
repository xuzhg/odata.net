﻿//---------------------------------------------------------------------
// <copyright file="GeographyBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Spatial;
    using Microsoft.Data.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Utils;

    /// <summary>
    /// Testing Geography Builder
    /// Integration with GeoDataBuilder and Validators
    /// We do not test the actual GeoData here, see GeoDataBuilderTests for GeoData building validation
    /// </summary>
    [TestClass]
    public class GeographyBuilderTests
    {
        private GeographyBuilderImplementation builder;
        private List<Geography> constructedInstances;
        private SpatialImplementation creator;

        [TestMethod]
        public void BuildCollection()
        {
            this.builder.BeginGeography(SpatialType.Collection);

            this.builder.BeginGeography(SpatialType.Point);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.BeginGeography(SpatialType.Polygon);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsCollection(
                g => g.VerifyAsPoint(new PositionData(10, 20, 30, 40)),
                g => g.VerifyAsPolygon(new[]
                                             {
                                                 new PositionData(10, 20, 30, 40),
                                                 new PositionData(10, 30, 30, 40),
                                                 new PositionData(10, 40, 30, 40),
                                                 new PositionData(10, 20, 30, 40),
                                             }));
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildFullGlobe()
        {
            this.builder.BeginGeography(SpatialType.FullGlobe);
            this.builder.EndGeography();

            var g = this.builder.ConstructedGeography as GeographyFullGlobe;
            Assert.IsNotNull(g);
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildLineString()
        {
            this.builder.BeginGeography(SpatialType.LineString);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(10, 30, 30, 40));
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildMultiLineString()
        {
            this.builder.BeginGeography(SpatialType.MultiLineString);

            this.builder.BeginGeography(SpatialType.LineString);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.BeginGeography(SpatialType.LineString);
            this.builder.BeginFigure(new GeographyPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsMultiLineString(new[]
                                                                          {
                                                                              new PositionData(10, 20, 30, 40),
                                                                              new PositionData(10, 30, 30, 40)
                                                                          },
                                                                      new[]
                                                                          {
                                                                              new PositionData(10, 30, 30, 40),
                                                                              new PositionData(10, 20, 30, 40)
                                                                          });
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildMultiPoint()
        {
            this.builder.BeginGeography(SpatialType.MultiPoint);

            this.builder.BeginGeography(SpatialType.Point);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.BeginGeography(SpatialType.Point);
            this.builder.BeginFigure(new GeographyPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsMultiPoint(new PositionData(10, 20, 30, 40), new PositionData(10, 30, 30, 40));
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildMultiPolygon()
        {
            this.builder.BeginGeography(SpatialType.MultiPolygon);

            this.builder.BeginGeography(SpatialType.Polygon);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.BeginGeography(SpatialType.Polygon);
            this.builder.BeginFigure(new GeographyPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 30, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsMultiPolygon(new[]
                                                                       {
                                                                           new[]
                                                                               {
                                                                                   new PositionData(10, 20, 30, 40),
                                                                                   new PositionData(10, 30, 30, 40),
                                                                                   new PositionData(10, 40, 30, 40),
                                                                                   new PositionData(10, 20, 30, 40),
                                                                               },
                                                                       },
                                                                   new[]
                                                                       {
                                                                           new[]
                                                                               {
                                                                                   new PositionData(10, 30, 30, 40),
                                                                                   new PositionData(10, 20, 30, 40),
                                                                                   new PositionData(10, 40, 30, 40),
                                                                                   new PositionData(10, 30, 30, 40),
                                                                               },
                                                                       });
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildPoint()
        {
            this.builder.BeginGeography(SpatialType.Point);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsPoint(new PositionData(10, 20, 30, 40));
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuildPolygon()
        {
            this.builder.BeginGeography(SpatialType.Polygon);
            this.builder.BeginFigure(new GeographyPosition(10, 20, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 30, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 40, 30, 40));
            this.builder.LineTo(new GeographyPosition(10, 20, 30, 40));
            this.builder.EndFigure();
            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsPolygon(new[]
                                                                  {
                                                                      new PositionData(10, 20, 30, 40),
                                                                      new PositionData(10, 30, 30, 40),
                                                                      new PositionData(10, 40, 30, 40),
                                                                      new PositionData(10, 20, 30, 40)
                                                                  });
            this.VerifyBuiltOnly(this.builder.ConstructedGeography);
        }

        [TestMethod]
        public void BuilderAccessBeforeEnd()
        {
            Geography g;
            this.builder.BeginGeography(SpatialType.Collection); // c1

            var ex = SpatialTestUtils.RunCatching<InvalidOperationException>(() => g = this.builder.ConstructedGeography);
            Assert.IsNotNull(ex);
            Assert.AreEqual(ex.Message, Strings.SpatialBuilder_CannotCreateBeforeDrawn);

            this.builder.BeginGeography(SpatialType.Collection); // c2
            this.builder.BeginGeography(SpatialType.Point);
            this.builder.BeginFigure(new GeographyPosition(10, 10, 10, 10));
            this.builder.EndFigure();
            this.builder.EndGeography();

            ex = SpatialTestUtils.RunCatching<InvalidOperationException>(() => g = this.builder.ConstructedGeography);
            Assert.IsNotNull(ex);
            Assert.AreEqual(ex.Message, Strings.SpatialBuilder_CannotCreateBeforeDrawn);

            this.builder.EndGeography(); // c2
            this.builder.BeginGeography(SpatialType.Point);
            this.builder.EndGeography();
            ex = SpatialTestUtils.RunCatching<InvalidOperationException>(() => g = this.builder.ConstructedGeography);
            Assert.IsNotNull(ex);
            Assert.AreEqual(ex.Message, Strings.SpatialBuilder_CannotCreateBeforeDrawn);
            this.builder.EndGeography();

            this.builder.ConstructedGeography.VerifyAsCollection(
                (c2) => c2.VerifyAsCollection((p) => p.VerifyAsPoint(new PositionData(10, 10, 10, 10))),
                (p) => p.VerifyAsPoint(null));
        }

        [TestMethod]
        public void BuildTwoPoints()
        {
            var shapes = new List<Geography>();
            for (int i = 1; i < 4; i++)
            {
                this.builder.BeginGeography(SpatialType.Point);
                this.builder.BeginFigure(new GeographyPosition(10 * i, 20, 30, 40));
                this.builder.EndFigure();
                this.builder.EndGeography();
                shapes.Add(this.builder.ConstructedGeography);
            }

            // ConstructedGeometry only keeps track of the last one written.
            this.builder.ConstructedGeography.VerifyAsPoint(new PositionData(30, 20, 30, 40));

            // But the event should have seen them all.
            this.VerifyBuiltOnly(shapes.ToArray());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.creator = new DataServicesSpatialImplementation();
            this.builder = new GeographyBuilderImplementation(this.creator);
            this.builder.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            this.constructedInstances = new List<Geography>();
            this.builder.ProduceGeography += this.constructedInstances.Add;
        }

        private void VerifyBuiltOnly(params Geography[] shapes)
        {
            SpatialTestUtils.AssertEqualContents(this.constructedInstances, shapes);
            foreach (var instance in constructedInstances)
            {
                Assert.AreSame(this.creator, instance.Creator);
            }
        }
    }
}