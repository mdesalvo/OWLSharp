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

namespace OWLSharp.Test.Extensions.TIME;

[TestClass]
public class TIMEInstantDescriptionTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateInstantDescriptionFromDateTime()
    {
        TIMEInstantDescription timeInstantDescription = new TIMEInstantDescription(
            new RDFResource("ex:instDesc"), DateTime.Parse("2010-05-22T22:45:30Z").ToUniversalTime());

        Assert.IsNotNull(timeInstantDescription);
        Assert.IsTrue(timeInstantDescription.URI.Equals(new Uri("ex:instDesc")));
        Assert.IsTrue(timeInstantDescription.Coordinate.Equals(new TIMECoordinate(2010, 5, 22, 22, 45, 30)));
    }

    [TestMethod]
    public void ShouldCreateInstantDescriptionFromCoordinate()
    {
        TIMEInstantDescription timeInstantDescription = new TIMEInstantDescription(
            new RDFResource("ex:instDesc"),
            new TIMECoordinate(2010, 5, 22, 22, 45, 30,
                new TIMECoordinateMetadata(
                    TIMECalendarReferenceSystem.Gregorian,
                    TIMEUnit.Second,
                    RDFVocabulary.TIME.GREG.MAY,
                    RDFVocabulary.TIME.SATURDAY,
                    142)));

        Assert.IsNotNull(timeInstantDescription);
        Assert.IsTrue(timeInstantDescription.URI.Equals(new Uri("ex:instDesc")));
        Assert.IsTrue(timeInstantDescription.Coordinate.Equals(new TIMECoordinate(2010, 5, 22, 22, 45, 30)));
        Assert.IsTrue(timeInstantDescription.Coordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsTrue(timeInstantDescription.Coordinate.Metadata.UnitType.Equals(TIMEUnit.Second));
        Assert.IsTrue(timeInstantDescription.Coordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.MAY));
        Assert.IsTrue(timeInstantDescription.Coordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.SATURDAY));
        Assert.AreEqual(142u, timeInstantDescription.Coordinate.Metadata.DayOfYear);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingInstantFromDescriptionBecauseNullDescription()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEInstantDescription(new RDFResource("ex:instDesc"), null));

    [TestMethod]
    public void ShouldCompareInstantDescriptions()
    {
        TIMEInstantDescription timeInstantDescriptionA = new TIMEInstantDescription(
            new RDFResource("ex:instDescA"),
            new TIMECoordinate(2010, 5, 22, 22, 45, 30));
        TIMEInstantDescription timeInstantDescriptionB = new TIMEInstantDescription(
            new RDFResource("ex:instDescB"),
            new TIMECoordinate(2010, 5, 22, 22, 55, 30));

        Assert.AreEqual(-1, timeInstantDescriptionA.CompareTo(timeInstantDescriptionB));
    }

    [TestMethod]
    public void ShouldEqualInstantDescriptions()
    {
        TIMEInstantDescription timeInstantDescriptionA = new TIMEInstantDescription(
            new RDFResource("ex:instDescA"),
            new TIMECoordinate(2010, 5, 22, 22, 45, 30));
        TIMEInstantDescription timeInstantDescriptionB = new TIMEInstantDescription(
            new RDFResource("ex:instDescB"),
            new TIMECoordinate(2010, 5, 22, 22, 45, 30));

        Assert.IsTrue(timeInstantDescriptionA.Equals(timeInstantDescriptionB));
    }
    #endregion
}