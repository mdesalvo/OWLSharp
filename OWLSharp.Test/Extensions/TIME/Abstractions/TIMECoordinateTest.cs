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
public class TIMECoordinateTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateCoordinate()
    {
        TIMECoordinate timeCoordinate = new TIMECoordinate(2010, 5, 22, 22, 45, 30);

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(2010, timeCoordinate.Year);
        Assert.AreEqual(5, timeCoordinate.Month);
        Assert.AreEqual(22, timeCoordinate.Day);
        Assert.AreEqual(22, timeCoordinate.Hour);
        Assert.AreEqual(45, timeCoordinate.Minute);
        Assert.AreEqual(30, timeCoordinate.Second);
        Assert.AreEqual("2010_5_22_22_45_30", timeCoordinate.ToString());
        Assert.IsNotNull(timeCoordinate.Metadata);
        Assert.IsNull(timeCoordinate.Metadata.TRS);
        Assert.IsNull(timeCoordinate.Metadata.UnitType);
        Assert.IsNull(timeCoordinate.Metadata.MonthOfYear);
        Assert.IsNull(timeCoordinate.Metadata.DayOfWeek);
        Assert.IsNull(timeCoordinate.Metadata.DayOfYear);
    }

    [TestMethod]
    public void ShouldCreateCoordinateWithMetadata()
    {
        TIMECoordinate timeCoordinate = new TIMECoordinate(2010, 5, 22, 22, 45, 30,
            new TIMECoordinateMetadata(
                TIMECalendarReferenceSystem.Gregorian, 
                RDFVocabulary.TIME.UNIT_SECOND,
                RDFVocabulary.TIME.GREG.MAY,
                RDFVocabulary.TIME.SATURDAY,
                142));

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(2010, timeCoordinate.Year);
        Assert.AreEqual(5, timeCoordinate.Month);
        Assert.AreEqual(22, timeCoordinate.Day);
        Assert.AreEqual(22, timeCoordinate.Hour);
        Assert.AreEqual(45, timeCoordinate.Minute);
        Assert.AreEqual(30, timeCoordinate.Second);
        Assert.AreEqual("2010_5_22_22_45_30", timeCoordinate.ToString());
        Assert.IsNotNull(timeCoordinate.Metadata);
        Assert.IsNotNull(timeCoordinate.Metadata.TRS);
        Assert.IsTrue(timeCoordinate.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        Assert.IsNotNull(timeCoordinate.Metadata.UnitType);
        Assert.IsTrue(timeCoordinate.Metadata.UnitType.Equals(RDFVocabulary.TIME.UNIT_SECOND));
        Assert.IsNotNull(timeCoordinate.Metadata.MonthOfYear);
        Assert.IsTrue(timeCoordinate.Metadata.MonthOfYear.Equals(RDFVocabulary.TIME.GREG.MAY));
        Assert.IsNotNull(timeCoordinate.Metadata.DayOfWeek);
        Assert.IsTrue(timeCoordinate.Metadata.DayOfWeek.Equals(RDFVocabulary.TIME.SATURDAY));
        Assert.IsNotNull(timeCoordinate.Metadata.DayOfYear);
        Assert.AreEqual(142u, timeCoordinate.Metadata.DayOfYear);
    }

    [TestMethod]
    public void ShouldCreateCoordinateWithPartialValues()
    {
        TIMECoordinate timeCoordinate = new TIMECoordinate(2010, 5, 22, null, null, null);

        Assert.IsNotNull(timeCoordinate);
        Assert.AreEqual(2010, timeCoordinate.Year);
        Assert.AreEqual(5, timeCoordinate.Month);
        Assert.AreEqual(22, timeCoordinate.Day);
        Assert.IsNull(timeCoordinate.Hour);
        Assert.IsNull(timeCoordinate.Minute);
        Assert.IsNull(timeCoordinate.Second);
        Assert.AreEqual("2010_5_22_0_0_0", timeCoordinate.ToString());
        Assert.IsNotNull(timeCoordinate.Metadata);
        Assert.IsNull(timeCoordinate.Metadata.TRS);
        Assert.IsNull(timeCoordinate.Metadata.UnitType);
        Assert.IsNull(timeCoordinate.Metadata.MonthOfYear);
        Assert.IsNull(timeCoordinate.Metadata.DayOfWeek);
        Assert.IsNull(timeCoordinate.Metadata.DayOfYear);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingCoordinateBecauseNegativeMonth()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMECoordinate(2010, -5, 22, 22, 45, 30));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingCoordinateBecauseNegativeDay()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMECoordinate(2010, 5, -22, 22, 45, 30));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingCoordinateBecauseNegativeHour()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMECoordinate(2010, 5, 22, -22, 45, 30));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingCoordinateBecauseNegativeMinute()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMECoordinate(2010, 5, 22, 22, -45, 30));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingCoordinateBecauseNegativeSecond()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMECoordinate(2010, 5, 22, 22, 45, -30));

    [TestMethod]
    public void ShouldCreateCoordinateFromDateTime()
    {
        TIMECoordinate timeCoodinate = new TIMECoordinate(DateTime.Parse("2010-05-22T22:45:30Z"));

        Assert.IsNotNull(timeCoodinate);
        Assert.AreEqual(2010, timeCoodinate.Year);
        Assert.AreEqual(5, timeCoodinate.Month);
        Assert.AreEqual(22, timeCoodinate.Day);
        Assert.AreEqual(22, timeCoodinate.Hour);
        Assert.AreEqual(45, timeCoodinate.Minute);
        Assert.AreEqual(30, timeCoodinate.Second);
    }

    [DataTestMethod]
    [DataRow("2010-05-22T22:45:30Z", null, 1)]
    [DataRow("2010-05-22T22:45:30Z", "2009-05-22T22:45:30Z", 1)]
    [DataRow("2010-05-22T22:45:30Z", "2011-05-22T22:45:30Z", -1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-04-22T22:45:30Z", 1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-06-22T22:45:30Z", -1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-21T22:45:30Z", 1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-23T22:45:30Z", -1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T21:45:30Z", 1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T23:45:30Z", -1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T22:44:30Z", 1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T22:46:30Z", -1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T22:45:29Z", 1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T22:45:31Z", -1)]
    [DataRow("2010-05-22T22:45:30Z", "2010-05-22T22:45:30Z", 0)]
    public void ShouldCompareCoordinates(string leftDateTime, string rightDateTime, int expectedComparison)
    {
        TIMECoordinate leftCoordinate = new TIMECoordinate(DateTime.Parse(leftDateTime));
        TIMECoordinate rightCoordinate = rightDateTime == null ? null : new TIMECoordinate(DateTime.Parse(rightDateTime));

        Assert.AreEqual(expectedComparison, leftCoordinate.CompareTo(rightCoordinate));
    }

    [DataTestMethod]
    [DataRow(2011d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, 1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2011d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, null, 05d, 22d, 22d, 45d, 30d, 1)]
    [DataRow(null, 05d, 22d, 22d, 45d, 30d, 2011d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 04d, 22d, 22d, 45d, 30d, 1)]
    [DataRow(2010d, 04d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, null, 22d, 22d, 45d, 30d, 1)]
    [DataRow(2010d, null, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 21d, 22d, 45d, 30d, 1)]
    [DataRow(2010d, 05d, 21d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, null, 22d, 45d, 30d, 1)]
    [DataRow(2010d, 05d, null, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 21d, 45d, 30d, 1)]
    [DataRow(2010d, 05d, 22d, 21d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, null, 45d, 30d, 1)]
    [DataRow(2010d, 05d, 22d, null, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 44d, 30d, 1)]
    [DataRow(2010d, 05d, 22d, 22d, 44d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, null, 30d, 1)]
    [DataRow(2010d, 05d, 22d, 22d, null, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 31d, 2010d, 05d, 22d, 22d, 45d, 30d, 1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 29d, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, null, 1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, null, 2010d, 05d, 22d, 22d, 45d, 30d, -1)]
    [DataRow(2010d, 05d, 22d, 22d, 45d, 30d, 2010d, 05d, 22d, 22d, 45d, 30d, 0)]
    [DataRow(null, 0d, null, 0d, null, 0d, 0d, null, 0d, null, 0d, null, 0)]
    public void ShouldComparePartialCoordinates(double? year1, double? month1, double? day1, double? hour1, double? minute1, double? second1,
        double? year2, double? month2, double? day2, double? hour2, double? minute2, double? second2, int expectedComparison)
    {
        TIMECoordinate leftCoordinate = new TIMECoordinate(year1, month1, day1, hour1, minute1, second1);
        TIMECoordinate rightCoordinate = new TIMECoordinate(year2, month2, day2, hour2, minute2, second2);

        Assert.AreEqual(expectedComparison, leftCoordinate.CompareTo(rightCoordinate));
    }
    #endregion
}