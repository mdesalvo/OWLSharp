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
using OWLSharp.Extensions.TIME;
using RDFSharp.Model;
using System;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEInstantTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateInstantFromDateTime()
        {
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:inst"), DateTime.Parse("2010-05-22T22:45:30Z").ToUniversalTime());

            Assert.IsNotNull(timeInstant);
            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:inst")));
            Assert.IsTrue(timeInstant.DateTime.HasValue && timeInstant.DateTime.Equals(DateTime.Parse("2010-05-22T22:45:30Z").ToUniversalTime()));
            Assert.IsNull(timeInstant.Description);
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldCreateInstantFromDescription()
        {
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:inst"), 
                new TIMEInstantDescription(new RDFResource("ex:instDesc"), DateTime.Parse("2010-05-22T22:45:30Z").ToUniversalTime()));

            Assert.IsNotNull(timeInstant);
            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:inst")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNotNull(timeInstant.Description);
            Assert.IsTrue(timeInstant.Description.URI.Equals(new Uri("ex:instDesc")));
            Assert.IsTrue(timeInstant.Description.Coordinate.Equals(new TIMECoordinate(2010, 5, 22, 22, 45, 30)));
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldCreateInstantFromDescriptionWithCoordinate()
        {
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:inst"),
                new TIMEInstantDescription(
                    new RDFResource("ex:instDesc"), 
                    new TIMECoordinate(2010, 5, 22, 22, 45, 30,
                        new TIMECoordinateMetadata(
                            TIMECalendarReferenceSystem.Gregorian,
                            RDFVocabulary.TIME.UNIT_SECOND,
                            RDFVocabulary.TIME.GREG.MAY,
                            RDFVocabulary.TIME.SATURDAY,
                            142))));

            Assert.IsNotNull(timeInstant);
            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:inst")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNotNull(timeInstant.Description);
            Assert.IsTrue(timeInstant.Description.URI.Equals(new Uri("ex:instDesc")));
            Assert.IsTrue(timeInstant.Description.Coordinate.Equals(new TIMECoordinate(2010, 5, 22, 22, 45, 30)));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.UnitType.Equals(RDFVocabulary.TIME.UNIT_SECOND));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.MAY));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.SATURDAY));
            Assert.IsTrue(timeInstant.Description.Coordinate.Metadata.DayOfYear == 142);
            Assert.IsNull(timeInstant.Position);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingInstantFromDescriptionBecauseNullDescription()
            => Assert.ThrowsException<OWLException>(() => new TIMEInstant(new RDFResource("ex:inst"), null as TIMEInstantDescription));

        [TestMethod]
        public void ShouldCreateInstantFromPosition()
        {
            TIMEInstant timeInstant = new TIMEInstant(new RDFResource("ex:inst"),
                new TIMEInstantPosition(new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.GeologicTime, 141.25));

            Assert.IsNotNull(timeInstant);
            Assert.IsTrue(timeInstant.URI.Equals(new Uri("ex:inst")));
            Assert.IsFalse(timeInstant.DateTime.HasValue);
            Assert.IsNull(timeInstant.Description);
            Assert.IsNotNull(timeInstant.Position);
            Assert.IsTrue(timeInstant.Position.URI.Equals(new Uri("ex:instPos")));
            Assert.IsTrue(timeInstant.Position.TRS.Equals(TIMEPositionReferenceSystem.GeologicTime));
            Assert.IsTrue(timeInstant.Position.NumericValue == 141.25);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingInstantFromDescriptionBecauseNullPosition()
            => Assert.ThrowsException<OWLException>(() => new TIMEInstant(new RDFResource("ex:inst"), null as TIMEInstantPosition));
        #endregion
    }
}