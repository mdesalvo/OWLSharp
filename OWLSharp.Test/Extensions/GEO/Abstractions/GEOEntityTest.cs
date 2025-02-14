/*
   Copyright 2014-2025 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.GEO;
using RDFSharp.Model;
using System;

namespace OWLSharp.Test.Extensions.GEO
{
    [TestClass]
    public class GEOEntityTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateGEOPoint()
        {
            GEOPoint geom = new GEOPoint(new RDFResource("ex:MilanGM"), (9.188540, 45.464664));

            Assert.IsNotNull(geom);
            Assert.IsTrue(geom.URI.Equals(new Uri("ex:MilanGM")));
            Assert.IsNotNull(geom.WGS84Geometry);
            Assert.AreEqual(4326, geom.WGS84Geometry.SRID);
            Assert.IsTrue(string.Equals(geom.ToWKT(), "POINT (9.18854 45.464664)"));
            Assert.IsTrue(string.Equals(geom.ToGML(), "<gml:Point xmlns:gml=\"http://www.opengis.net/gml\"><gml:coord><gml:X>9.18854</gml:X><gml:Y>45.464664</gml:Y></gml:coord></gml:Point>"));

            Assert.ThrowsExactly<OWLException>(() => _ = new GEOPoint(new RDFResource("ex:MilanGM"), (-182.0, 45.464664)));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOPoint(new RDFResource("ex:MilanGM"), (182.0, 45.464664)));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOPoint(new RDFResource("ex:MilanGM"), (9.188540, -92.0)));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOPoint(new RDFResource("ex:MilanGM"), (9.188540, 92.0)));
        }

        [TestMethod]
        public void ShouldCreateGEOLine()
        {
            GEOLine geom = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664), (9.198540, 45.474664)]);

            Assert.IsNotNull(geom);
            Assert.IsTrue(geom.URI.Equals(new Uri("ex:MilanGM")));
            Assert.IsNotNull(geom.WGS84Geometry);
            Assert.AreEqual(4326, geom.WGS84Geometry.SRID);
            Assert.IsTrue(string.Equals(geom.ToWKT(), "LINESTRING (9.18854 45.464664, 9.19854 45.474664)"));
            Assert.IsTrue(string.Equals(geom.ToGML(), "<gml:LineString xmlns:gml=\"http://www.opengis.net/gml\"><gml:coordinates>9.18854,45.464664 9.19854,45.474664</gml:coordinates></gml:LineString>"));

            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), null));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(-182.188540, 45.464664), (9.198540, 45.474664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(182.188540, 45.464664), (9.198540, 45.474664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, -92.464664), (9.198540, 45.474664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 92.464664), (9.198540, 45.474664)]));
        }

        [TestMethod]
        public void ShouldCreateGEOArea()
        {
            GEOArea geom = new GEOArea(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664), (9.198540, 45.474664), (9.208540, 45.484664)]); //will be automatically closed

            Assert.IsNotNull(geom);
            Assert.IsTrue(geom.URI.Equals(new Uri("ex:MilanGM")));
            Assert.IsNotNull(geom.WGS84Geometry);
            Assert.AreEqual(4326, geom.WGS84Geometry.SRID);
            Assert.IsTrue(string.Equals(geom.ToWKT(), "POLYGON ((9.18854 45.464664, 9.19854 45.474664, 9.20854 45.484664, 9.18854 45.464664))"));
            Assert.IsTrue(string.Equals(geom.ToGML(), "<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml\"><gml:outerBoundaryIs><gml:LinearRing><gml:coordinates>9.18854,45.464664 9.19854,45.474664 9.20854,45.484664 9.18854,45.464664</gml:coordinates></gml:LinearRing></gml:outerBoundaryIs></gml:Polygon>"));

            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), null));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 45.464664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(-182.188540, 45.464664), (9.198540, 45.474664), (-82.188540, 45.464664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(182.188540, 45.464664), (9.198540, 45.474664), (82.188540, 45.464664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, -92.464664), (9.198540, 45.474664), (9.188540, -72.464664)]));
            Assert.ThrowsExactly<OWLException>(() => _ = new GEOLine(new RDFResource("ex:MilanGM"), [(9.188540, 92.464664), (9.198540, 45.474664), (9.188540, -72.464664)]));
        }
        #endregion
    }
}