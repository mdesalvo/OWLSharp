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
using static OWLSharp.Extensions.TIME.TIMEEnums;

namespace OWLSharp.Test.Extensions.TIME;

[TestClass]
public class TIMEConverterTest
{
    #region Tests
    [TestMethod]
    public void ShouldThrowExceptionOnGettingCalendarFromPositionBecauseNullPositionTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.CoordinateFromPosition(25226354, null, TIMECalendarReferenceSystem.Gregorian));

    [TestMethod]
    public void ShouldThrowExceptionOnGettingCalendarFromPositionBecauseNullCalendarTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.CoordinateFromPosition(25226354, TIMEPositionReferenceSystem.UnixTime, null));

    [DataTestMethod]
    [DataRow(-413733671.4, 1956, 11, 21, 9, 58, 48)]
    [DataRow(-413733600, 1956, 11, 21, 10, 0, 0)]
    [DataRow(-3675, 1969, 12, 31, 22, 58, 45)]
    [DataRow(-625, 1969, 12, 31, 23, 49, 35)]
    [DataRow(-600, 1969, 12, 31, 23, 50, 0)]
    [DataRow(-117, 1969, 12, 31, 23, 58, 3)]
    [DataRow(-60, 1969, 12, 31, 23, 59, 0)]
    [DataRow(-34, 1969, 12, 31, 23, 59, 26)]
    [DataRow(-1, 1969, 12, 31, 23, 59, 59)]
    [DataRow(0, 1970, 1, 1, 0, 0, 0)]
    [DataRow(1, 1970, 1, 1, 0, 0, 1)]
    [DataRow(34, 1970, 1, 1, 0, 0, 34)]
    [DataRow(600, 1970, 1, 1, 0, 10, 0)]
    [DataRow(625, 1970, 1, 1, 0, 10, 25)]
    [DataRow(3675, 1970, 1, 1, 1, 1, 15)]
    [DataRow(413733685, 1983, 2, 10, 14, 1, 25)]
    [DataRow(413733671.4, 1983, 2, 10, 14, 1, 11)]
    [DataRow(1682520568, 2023, 4, 26, 14, 49, 28)]
    public void ShouldGetCalendarFromUnixTRS(double timePosition, int expectedYear, int expectedMonth,
        int expectedDay, int expectedHour, int expectedMinute, int expectedSecond)
    {
        TIMECoordinate tc = TIMEConverter.CoordinateFromPosition(timePosition, TIMEPositionReferenceSystem.UnixTime, TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }
        
    [DataTestMethod]
    [DataRow(-25.2502, TIMEUnitType.Year, 1, 1944, 10, 7, 16, 14, 52)] //6 leap years encountered (1968,1964,1960,1956,1952,1948)
    [DataRow(-61.25, TIMEUnitType.Day, 1, 1969, 10, 31, 18, 0, 0)]
    [DataRow(-1, TIMEUnitType.Hour, 26.5, 1969, 12, 30, 21, 30, 0)]
    [DataRow(-10.5, TIMEUnitType.Second, 30, 1969, 12, 31, 23, 54, 45)]
    [DataRow(1, TIMEUnitType.Hour, 0.5, 1970, 1, 1, 0, 30, 0)]
    [DataRow(610, TIMEUnitType.Minute, 0.1, 1970, 1, 1, 1, 1, 0)]
    [DataRow(61.25, TIMEUnitType.Day, 1, 1970, 3, 3, 6, 0, 0)]
    [DataRow(601.5, TIMEUnitType.Second, 30, 1970, 1, 1, 5, 0, 45)]
    [DataRow(10, TIMEUnitType.Year, 1, 1979, 12, 30, 0, 0, 0)] //2 leap years encountered (1972,1976)
    [DataRow(16, TIMEUnitType.Month, 1, 1971, 4, 26, 0, 0, 0)] //Gregorian offset (P5D)
    public void ShouldGetCalendarFromUnixTRSHavingDerivateUnit(double timePosition, TIMEUnitType unitType, double unitScale, int expectedYear, int expectedMonth,
        int expectedDay, int expectedHour, int expectedMinute, int expectedSecond)
    {
        TIMEPositionReferenceSystem unixModifiedTRS = new TIMEPositionReferenceSystem(
            new RDFResource("ex:CustomTRS"),
            TIMEPositionReferenceSystem.UnixTime.Origin,
            new TIMEUnit(new RDFResource("ex:CustomUnit"), unitType, unitScale));
        TIMECoordinate tc = TIMEConverter.CoordinateFromPosition(timePosition, unixModifiedTRS, TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [DataTestMethod]
    [DataRow(3, 1969, 13, 32, 49, 0, 0, TIMEUnitType.Hour, 0.5, 1970, 2, 3, 2, 30, 0)]
    [DataRow(3, 1969, 12, 31, 23, 0, 0, TIMEUnitType.Hour, 0.5, 1970, 1, 1, 0, 30, 0)]
    [DataRow(717, 2000, 1, 1, 0, 0, 0, TIMEUnitType.Day, 1, 2001, 12, 18, 0, 0, 0)]    //1 leap year encountered
    [DataRow(717.5, 2000, 1, 1, 0, 0, 0, TIMEUnitType.Day, 1, 2001, 12, 18, 12, 0, 0)] //1 leap year encountered
    [DataRow(0, 2000, 1, 1, 0, 0, 0, TIMEUnitType.Month, 1, 2000, 1, 1, 0, 0, 0)]
    [DataRow(-717, 2000, 1, 1, 0, 0, 0, TIMEUnitType.Day, 1, 1998, 1, 14, 0, 0, 0)]
    [DataRow(2, 2000, 11, 27, 0, 0, 0, TIMEUnitType.Month, 2, 2001, 3, 27, 0, 0, 0)]
    public void ShouldGetCalendarFromCustomTRSHavingDerivateUnit(double timePosition, double originYear, double originMonth, double originDay,
        double originHour, double originMinute, double originSecond, TIMEUnitType unitType, double unitScale,
        int expectedYear, int expectedMonth, int expectedDay, int expectedHour, int expectedMinute, int expectedSecond)
    {
        TIMEPositionReferenceSystem unixModifiedTRS = new TIMEPositionReferenceSystem(
            new RDFResource("ex:CustomTRS"),
            new TIMECoordinate(originYear, originMonth, originDay, originHour, originMinute, originSecond),
            new TIMEUnit(new RDFResource("ex:CustomUnit"), unitType, unitScale));
        TIMECoordinate tc = TIMEConverter.CoordinateFromPosition(timePosition, unixModifiedTRS, TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [DataTestMethod]
    [DataRow(1, TIMEUnitType.Hour, 0.5, 1970, 1, 1, 0, 30, 0)]
    [DataRow(-1, TIMEUnitType.Hour, 1, 1969, 12, 30, 23, 0, 0)]
    [DataRow(25, TIMEUnitType.Hour, 1, 1970, 1, 2, 1, 0, 0)]
    [DataRow(80.25, TIMEUnitType.Day, 2, 1970, 6, 11, 12, 0, 0)]
    [DataRow(-17, TIMEUnitType.Month, 1.5, 1967, 11, 16, 0, 0, 0)]
    [DataRow(17.35, TIMEUnitType.Month, 1.5, 1972, 3, 1, 18, 0, 0)]
    [DataRow(10, TIMEUnitType.Year, 1.5, 1985, 1, 1, 0, 0, 0)]
    [DataRow(10, TIMEUnitType.Year, 1.0675, 1980, 9, 3, 23, 59, 59)]
    [DataRow(10, TIMEUnitType.Year, 2, 1989, 12, 29, 0, 0, 0)]  //2 leap years encountered
    public void ShouldGetCalendarFromUnixTRSToCustomCalendarTRS(double timePosition, TIMEUnitType unitType, double unitScale, int expectedYear, int expectedMonth,
        int expectedDay, int expectedHour, int expectedMinute, int expectedSecond)
    {
        TIMEPositionReferenceSystem unixModifiedTRS = new TIMEPositionReferenceSystem(
            new RDFResource("ex:CustomPositionTRS"),
            TIMEPositionReferenceSystem.UnixTime.Origin,
            new TIMEUnit(new RDFResource("ex:CustomUnit"), unitType, unitScale));
        TIMECoordinate tc = TIMEConverter.CoordinateFromPosition(
            timePosition, 
            unixModifiedTRS,
            new TIMECalendarReferenceSystem(
                new RDFResource("https://en.wikipedia.org/wiki/360-day_calendar"),
                new TIMECalendarReferenceSystemMetrics(60, 60, 24, [30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30])
                    .SetLeapYearRule(year => year >= 1985 && year % 2 == 0
                        ? [31, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30]
                        : [30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30])));

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [DataTestMethod]
    [DataRow(-66.5, 66501950d, null, null, null, null, null)]
    [DataRow(-0.001, 2950d, null, null, null, null, null)]
    [DataRow(-0.000001, 1951d, null, null, null, null, null)]
    [DataRow(-0.00000025, 1950d, null, null, null, null, null)]
    [DataRow(0, 1950d, null, null, null, null, null)]
    [DataRow(0.000001, 1949d, null, null, null, null, null)]
    [DataRow(0.001, 950d, null, null, null, null, null)]
    [DataRow(66.5, -66498050d, null, null, null, null, null)]
    public void ShouldGetCalendarFromGeologicTRS(double timePosition, double? expectedYear, double? expectedMonth, double? expectedDay,
        double? expectedHour, double? expectedMinute, double? expectedSecond)
    {
        TIMECoordinate tc = TIMEConverter.CoordinateFromPosition(timePosition, TIMEPositionReferenceSystem.GeologicTime, TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [DataTestMethod]
    [DataRow(-9000, 1969, 12, 31, 23, 0, 0, TIMEUnitType.Month, 500, -373031d, null, null, null, null, null)]
    [DataRow(3700, 1969, 12, 31, 23, 0, 0, TIMEUnitType.Year, 5, 20469d, null, null, null, null, null)]
    [DataRow(819.46, -2000, 1, 1, 0, 0, 0, TIMEUnitType.Year, -1000000, -819462000d, null, null, null, null, null)]
    public void ShouldGetCalendarFromCustomLargeScaleTRSHavingDerivateUnit(double timePosition, double originYear, double originMonth, double originDay,
        double originHour, double originMinute, double originSecond, TIMEUnitType unitType, double unitScale,
        double? expectedYear, double? expectedMonth, double? expectedDay, double? expectedHour, double? expectedMinute, double? expectedSecond)
    {
        TIMEPositionReferenceSystem geologicModifiedTRS = new TIMEPositionReferenceSystem(
            new RDFResource("ex:CustomLargeScaleTRS"),
            new TIMECoordinate(originYear, originMonth, originDay, originHour, originMinute, originSecond),
            new TIMEUnit(new RDFResource("ex:CustomUnit"), unitType, unitScale), true);
        TIMECoordinate tc = TIMEConverter.CoordinateFromPosition(timePosition, geologicModifiedTRS, TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnNormalizingCoordinateBecauseNullCoordinate()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.NormalizeCoordinate(null, TIMECalendarReferenceSystem.Gregorian));

    [TestMethod]
    public void ShouldThrowExceptionOnNormalizingCoordinateBecauseNullCalendarTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.NormalizeCoordinate(new TIMECoordinate(), null));

    [DataTestMethod]
    [DataRow(1983, 2, 10, 15, 30, 30, 1983, 2, 10, 15, 30, 30)]
    [DataRow(1983, 1, 31, 15, 30, 30, 1983, 1, 31, 15, 30, 30)]
    [DataRow(1983, 12, 31, 15, 30, 30, 1983, 12, 31, 15, 30, 30)]
    [DataRow(1983, 12, 31, 23, 59, 59, 1983, 12, 31, 23, 59, 59)]
    [DataRow(1983, 2, 10, 15, 30, 75, 1983, 2, 10, 15, 31, 15)]
    [DataRow(1983, 2, 10, 15, 75, 30, 1983, 2, 10, 16, 15, 30)]
    [DataRow(1983, 2, 10, 25, 30, 30, 1983, 2, 11, 1, 30, 30)]
    [DataRow(1983, 2, 29, 15, 30, 30, 1983, 3, 1, 15, 30, 30)]
    [DataRow(1983, 14, 10, 15, 30, 30, 1984, 2, 10, 15, 30, 30)]
    [DataRow(1983, 2, 10, 47, 75, 75, 1983, 2, 12, 0, 16, 15)]
    [DataRow(1983, 12, 54, 50, 75, 75, 1984, 1, 25, 3, 16, 15)]
    [DataRow(1983, 27, 54, 50, 75, 75, 1985, 4, 25, 3, 16, 15)]
    [DataRow(1983, 12, 31, 23, 59, 60, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 12, 31, 23, 60, 0, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 12, 31, 24, 0, 0, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 12, 32, 0, 0, 0, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 13, 1, 0, 0, 0, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 13, 32, 24, 60, 60, 1984, 2, 2, 1, 1, 0)]
    [DataRow(1983, 1, 1, 0, 0, 86400, 1983, 1, 2, 0, 0, 0)]
    [DataRow(1983, 1, 1, 0, 1440, 0, 1983, 1, 2, 0, 0, 0)]
    [DataRow(1983, 12, 30, 0, 1440, 86400, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 23, 34, 25, 0, 0, 1984, 12, 5, 1, 0, 0)]
    [DataRow(1983, 24, 34, 25, 0, 0, 1985, 1, 4, 1, 0, 0)]
    [DataRow(1983, 11, 31, 0, 0, 0, 1983, 12, 1, 0, 0, 0)]
    [DataRow(1983, 11, 61, 0, 0, 0, 1984, 1, 1, 0, 0, 0)]
    [DataRow(1983, 48, 42, 0, 0, 84.5, 1987, 1, 11, 0, 1, 24.5)]
    [DataRow(1983, 28, 400, 0, 0, 0, 1986, 5, 10, 0, 0, 0)]
    [DataRow(1983, 2, 29, 0, 0, 0, 1983, 3, 1, 0, 0, 0)]
    [DataRow(1983, 1, 60, 0, 0, 0, 1983, 3, 1, 0, 0, 0)]
    [DataRow(1580, 2, 29, 0, 0, 0, 1580, 3, 1, 0, 0, 0)]  //before 1582 no leap year applicable
    [DataRow(1984, 2, 29, 0, 0, 0, 1984, 2, 29, 0, 0, 0)] //leap year
    [DataRow(1984, 1, 60, 0, 0, 0, 1984, 2, 29, 0, 0, 0)] //leap year
    [DataRow(1984, 2, 28, 23, 59, 60, 1984, 2, 29, 0, 0, 0)] //leap year
    [DataRow(0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0, 1, 1, 0, 0, 0.5)] //month and day initialized at 1, others at 0 (seconds untouched)
    public void ShouldNormalizeCoordinateToGregorianCalendar(double originYear, double originMonth, double originDay,
        double originHour, double originMinute, double originSecond, double expectedYear, double expectedMonth, double expectedDay,
        double expectedHour, double expectedMinute, double expectedSecond)
    {
        TIMECoordinate tc = TIMEConverter.NormalizeCoordinate(
            new TIMECoordinate(originYear, originMonth, originDay, originHour, originMinute, originSecond), 
            TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [DataTestMethod]
    [DataRow(2000, 2, 10, 8, 7, 9, 2000, 2, 10, 8, 7, 9)]
    [DataRow(2000, 2, 10, 8, 7, 10, 2000, 2, 10, 8, 8, 0)]
    [DataRow(2000, 2, 10, 8, 10, 9, 2000, 2, 10, 9, 0, 9)]
    [DataRow(2000, 2, 11, 8, 7, 9, 2000, 3, 1, 8, 7, 9)]
    [DataRow(1999, 6, 11, 8, 7, 9, 2000, 1, 1, 8, 7, 9)]
    [DataRow(1999, 6, 70, 0, 0, 0, 2000, 6, 10, 0, 0, 0)]
    [DataRow(1999, 6, 12, 0, 0, 0, 2000, 1, 2, 0, 0, 0)]
    [DataRow(1999, 6, 10, 10, 0, 0, 2000, 1, 1, 0, 0, 0)]
    [DataRow(1999, 60, 1, 0, 0, 0, 2008, 6, 1, 0, 0, 0)]
    [DataRow(1999, 61, 1, 0, 0, 0, 2009, 1, 1, 0, 0, 0)]
    [DataRow(2000, 6, 70, 0, 0, 0, 2001, 6, 4, 0, 0, 0)]   //leap year
    [DataRow(2000, 6, 11, 8, 7, 9, 2000, 6, 11, 8, 7, 9)]  //leap year
    [DataRow(2000, 6, 12, 0, 0, 0, 2001, 1, 1, 0, 0, 0)]   //leap year
    [DataRow(2000, 6, 10, 10, 0, 0, 2000, 6, 11, 0, 0, 0)] //leap year
    public void ShouldNormalizeCoordinateToCustomCalendar(double originYear, double originMonth, double originDay,
        double originHour, double originMinute, double originSecond, double expectedYear, double expectedMonth, double expectedDay,
        double expectedHour, double expectedMinute, double expectedSecond)
    {
        TIMECoordinate tc = TIMEConverter.NormalizeCoordinate(
            new TIMECoordinate(originYear, originMonth, originDay, originHour, originMinute, originSecond),
            new TIMECalendarReferenceSystem(
                new RDFResource("ex:MyStrangeCalendar"),
                new TIMECalendarReferenceSystemMetrics(10, 10, 10, [10, 10, 10, 10, 10, 10])
                    .SetLeapYearRule(year => year % 2 == 0 ? [10, 10, 10, 10, 10, 11] : [10, 10, 10, 10, 10, 10])));

        Assert.IsNotNull(tc);
        Assert.AreEqual(expectedYear, tc.Year);
        Assert.AreEqual(expectedMonth, tc.Month);
        Assert.AreEqual(expectedDay, tc.Day);
        Assert.AreEqual(expectedHour, tc.Hour);
        Assert.AreEqual(expectedMinute, tc.Minute);
        Assert.AreEqual(expectedSecond, tc.Second);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnGettingExtentFromDurationBecauseNegativeDuration()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.ExtentFromDuration(-25226354, TIMEUnit.Second, TIMECalendarReferenceSystem.Gregorian));

    [TestMethod]
    public void ShouldThrowExceptionOnGettingExtentFromDurationBecauseNullUnitType()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.ExtentFromDuration(25226354, null, TIMECalendarReferenceSystem.Gregorian));

    [TestMethod]
    public void ShouldThrowExceptionOnGettingExtentFromDurationBecauseNullCalendarTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.ExtentFromDuration(25226354, TIMEUnit.MarsSol, null));

    [DataTestMethod]
    //seconds
    [DataRow(0, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(59, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 0d, 59d)]
    [DataRow(60, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 1d, 0d)]
    [DataRow(61, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 1d, 1d)]
    [DataRow(3599, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 59d, 59d)]
    [DataRow(3600, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(3660, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(3661, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 1d, 1d, 1d)]
    [DataRow(86399, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 23d, 59d, 59d)]
    [DataRow(86400, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(90000, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 1d, 0d, 0d)]
    [DataRow(90060, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 1d, 1d, 0d)]
    [DataRow(90061, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 1d, 1d, 1d)]
    [DataRow(2591999, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 29d, 23d, 59d, 59d)]
    [DataRow(2592000, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 30d, 0d, 0d, 0d)]
    [DataRow(2595600, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 30d, 1d, 0d, 0d)]
    [DataRow(2595660, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 30d, 1d, 1d, 0d)]
    [DataRow(2595661, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 30d, 1d, 1d, 1d)]
    [DataRow(2678400, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 31d, 0d, 0d, 0d)]
    [DataRow(26784000, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 310d, 0d, 0d, 0d)]
    //minutes
    [DataRow(0, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 0d, 0d, 30d)]
    [DataRow(59, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 0d, 59d, 0d)]
    [DataRow(60, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(60.25, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 1d, 0d, 15d)]
    [DataRow(61, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(1439, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 23d, 59d, 0d)]
    [DataRow(1440, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(1440.75, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 0d, 0d, 45d)]
    [DataRow(1441, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 0d, 1d, 0d)]
    [DataRow(1500, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 1d, 0d, 0d)]
    [DataRow(1501, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 1d, 1d, 0d)]
    [DataRow(43199, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 29d, 23d, 59d, 0d)]
    [DataRow(43200, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 30d, 0d, 0d, 0d)]
    [DataRow(43201, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 30d, 0d, 1d, 0d)]
    [DataRow(43260, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 30d, 1d, 0d, 0d)]
    [DataRow(43267, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 30d, 1d, 7d, 0d)]
    [DataRow(43201.1, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 30d, 0d, 1d, 6d)]
    [DataRow(432600, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 300d, 10d, 0d, 0d)]
    //hours
    [DataRow(0, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 30d, 0d)]
    [DataRow(0.55, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 33d, 0d)]
    [DataRow(0.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 33d, 1d)]
    [DataRow(1, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(23.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 23d, 30d, 0d)]
    [DataRow(23.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 23d, 33d, 1d)]
    [DataRow(24, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(24.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 1d, 0d, 30d, 0d)]
    [DataRow(35.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 1d, 11d, 33d, 1d)]
    [DataRow(719, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 29d, 23d, 0d, 0d)]
    [DataRow(720, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 30d, 0d, 0d, 0d)]
    [DataRow(744, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 31d, 0d, 0d, 0d)]
    [DataRow(745, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 31d, 1d, 0d, 0d)]
    [DataRow(745.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 31d, 1d, 30d, 0d)]
    [DataRow(745.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 31d, 1d, 33d, 1d)]
    [DataRow(8759, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 364d, 23d, 0d, 0d)]
    [DataRow(8760, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 365d, 0d, 0d, 0d)]
    [DataRow(87602, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 3650d, 2d, 0d, 0d)]
    //days
    [DataRow(0, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.005, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 0d, 7d, 12d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 12d, 0d, 0d)]
    [DataRow(0.55, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 13d, 12d, 0d)]
    [DataRow(0.550285, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 13d, 12d, 24d)]
    [DataRow(1, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(29.95, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 29d, 22d, 48d, 0d)]
    [DataRow(30, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 30d, 0d, 0d, 0d)]
    [DataRow(365.2422, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 365d, 5d, 48d, 46d)]
    //weeks (Gregorian calendar TRS has not exact metric, so this component maps to equivalent days)
    [DataRow(0, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 7, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 7, 0d, 0d, 0d, 3d, 12d, 0d, 0d)]
    [DataRow(1, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 7, 0d, 0d, 0d, 7d, 0d, 0d, 0d)]
    [DataRow(52.142857, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 7, 0d, 0d, 0d, 364d, 23d, 59d, 59d)]
    //months (Gregorian calendar TRS has not exact metric, so this component maps to equivalent days)
    [DataRow(0, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 15d, 0d, 0d, 0d)]
    [DataRow(1, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 30d, 0d, 0d, 0d)]
    [DataRow(2.5, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 75d, 0d, 0d, 0d)]
    [DataRow(8.2425, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 247d, 6d, 35d, 59d)]
    [DataRow(45.965, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 1378d, 22d, 48d, 0d)]
    //years (Gregorian calendar TRS has not exact metric, so this component maps to equivalent days)
    [DataRow(0, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 0d, 0d, 182d, 12d, 0d, 0d)]
    [DataRow(1, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 0d, 0d, 365d, 0d, 0d, 0d)]
    [DataRow(9.272501, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 0d, 0d, 3384d, 11d, 6d, 31d)]
    public void ShouldGetExtentFromDuration(double timeDuration, string unitTypeURI, TIMEUnitType unitTypeEnum, double scaleFactor, 
        double? expectedYears, double? expectedMonths, double? expectedWeeks, double? expectedDays, double? expectedHours, double? expectedMinutes, double? expectedSeconds)
    {
        TIMEExtent te = TIMEConverter.ExtentFromDuration(timeDuration, new TIMEUnit(new RDFResource(unitTypeURI), unitTypeEnum, scaleFactor), TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(te);
        Assert.AreEqual(expectedYears, te.Years);
        Assert.AreEqual(expectedMonths, te.Months);
        Assert.AreEqual(expectedWeeks, te.Weeks);
        Assert.AreEqual(expectedDays, te.Days);
        Assert.AreEqual(expectedHours, te.Hours);
        Assert.AreEqual(expectedMinutes, te.Minutes);
        Assert.AreEqual(expectedSeconds, te.Seconds);
    }

    [DataTestMethod]
    //seconds
    [DataRow(0, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(59, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 0d, 59d)]
    [DataRow(60, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 1d, 0d)]
    [DataRow(61, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 1d, 1d)]
    [DataRow(3599, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 0d, 59d, 59d)]
    [DataRow(3600, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(3660, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(3661, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 1d, 1d, 1d)]
    [DataRow(86399, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 0d, 23d, 59d, 59d)]
    [DataRow(86400, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(90000, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 1d, 0d, 0d)]
    [DataRow(90060, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 1d, 1d, 0d)]
    [DataRow(90061, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 1d, 1d, 1d, 1d)]
    [DataRow(2591999, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 0d, 0d, 29d, 23d, 59d, 59d)]
    [DataRow(2592000, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 1d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(2595600, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 1d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(2595660, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 1d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(2595661, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 1d, 0d, 0d, 1d, 1d, 1d)]
    [DataRow(31103999, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 0d, 11d, 0d, 29d, 23d, 59d, 59d)]
    [DataRow(31104000, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(142399112.6, "http://www.w3.org/2006/time#second", TIMEUnitType.Second, 1, 4d, 6d, 0d, 28d, 3d, 18d, 32d)]
    //minutes
    [DataRow(0, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 0d, 0d, 30d)]
    [DataRow(59, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 0d, 59d, 0d)]
    [DataRow(60, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(60.25, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 1d, 0d, 15d)]
    [DataRow(61, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(1439, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 0d, 23d, 59d, 0d)]
    [DataRow(1440, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(1440.75, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 0d, 0d, 45d)]
    [DataRow(1441, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 0d, 1d, 0d)]
    [DataRow(1500, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 1d, 0d, 0d)]
    [DataRow(1501, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 1d, 1d, 1d, 0d)]
    [DataRow(43199, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 0d, 0d, 29d, 23d, 59d, 0d)]
    [DataRow(43200, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(43201, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 0d, 0d, 1d, 0d)]
    [DataRow(43201.1, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 0d, 0d, 1d, 6d)]
    [DataRow(43260, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(43261, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(43261.2, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 0d, 1d, 1d, 12d)]
    [DataRow(84960, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 1d, 0d, 29d, 0d, 0d, 0d)]
    [DataRow(518399, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 0d, 11d, 0d, 29d, 23d, 59d, 0d)]
    [DataRow(518400, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(518401, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 1d, 0d, 0d, 0d, 0d, 1d, 0d)]
    [DataRow(518461, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 1d, 0d, 0d, 0d, 1d, 1d, 0d)]
    [DataRow(519841, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 1d, 0d, 0d, 1d, 0d, 1d, 0d)]
    [DataRow(519841.5, "http://www.w3.org/2006/time#minute", TIMEUnitType.Minute, 1, 1d, 0d, 0d, 1d, 0d, 1d, 30d)]
    //hours
    [DataRow(0, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 30d, 0d)]
    [DataRow(0.55, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 33d, 0d)]
    [DataRow(0.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 0d, 33d, 1d)]
    [DataRow(1, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(23.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 23d, 30d, 0d)]
    [DataRow(23.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 0d, 23d, 33d, 1d)]
    [DataRow(24, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(24.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 1d, 0d, 30d, 0d)]
    [DataRow(35.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 1d, 11d, 33d, 1d)]
    [DataRow(719, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 0d, 0d, 29d, 23d, 0d, 0d)]
    [DataRow(720, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 1d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(744, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 1d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(745, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 1d, 0d, 1d, 1d, 0d, 0d)]
    [DataRow(745.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 1d, 0d, 1d, 1d, 30d, 0d)]
    [DataRow(745.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 1d, 0d, 1d, 1d, 33d, 1d)]
    [DataRow(8639, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 0d, 11d, 0d, 29d, 23d, 0d, 0d)]
    [DataRow(8640, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(8641, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 1d, 0d, 0d, 0d, 1d, 0d, 0d)]
    [DataRow(8664, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 1d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(8665, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 1d, 0d, 0d, 1d, 1d, 0d, 0d)]
    [DataRow(8665.5, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 1d, 0d, 0d, 1d, 1d, 30d, 0d)]
    [DataRow(8665.550285, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 1d, 0d, 0d, 1d, 1d, 33d, 1d)]
    [DataRow(34560, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 4d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(43200, "http://www.w3.org/2006/time#hour", TIMEUnitType.Hour, 1, 5d, 0d, 0d, 0d, 0d, 0d, 0d)]
    //days
    [DataRow(0, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.005, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 0d, 7d, 12d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 12d, 0d, 0d)]
    [DataRow(0.55, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 13d, 12d, 0d)]
    [DataRow(0.550285, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 0d, 13d, 12d, 24d)]
    [DataRow(1, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 1d, 0d, 0d, 0d)]
    [DataRow(29.95, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 0d, 0d, 29d, 22d, 48d, 0d)]
    [DataRow(30, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 1d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(75, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 0d, 2d, 0d, 15d, 0d, 0d, 0d)]
    [DataRow(360, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(3677.550285, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 1, 10d, 2d, 0d, 17d, 13d, 12d, 24d)]
    //weeks (modeled as multiple of Day, in this calendar TRS it is comfortable to have 5 days per week)
    [DataRow(0, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 5, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 5, 0d, 0d, 0d, 2d, 12d, 0d, 0d)]
    [DataRow(1, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 5, 0d, 0d, 0d, 5d, 0d, 0d, 0d)]
    [DataRow(6, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 5, 0d, 1d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(72, "http://www.w3.org/2006/time#day", TIMEUnitType.Day, 5, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    //months
    [DataRow(0, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.5, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 15d, 0d, 0d, 0d)]
    [DataRow(0.005, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 0d, 3d, 36d, 0d)]
    [DataRow(0.005004, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 0d, 0d, 0d, 3d, 36d, 10d)]
    [DataRow(1, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 1d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(1.25, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 0d, 1d, 0d, 7d, 12d, 0d, 0d)]
    [DataRow(12, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(37, "http://www.w3.org/2006/time#month", TIMEUnitType.Month, 1, 3d, 1d, 0d, 0d, 0d, 0d, 0d)]
    //years
    [DataRow(0, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(0.2, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 2d, 0d, 12d, 0d, 0d, 0d)]
    [DataRow(0.205, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 0d, 2d, 0d, 13d, 19d, 11d, 59d)]
    [DataRow(1, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 1d, 0d, 0d, 0d, 0d, 0d, 0d)]
    [DataRow(25.575, "http://www.w3.org/2006/time#year", TIMEUnitType.Year, 1, 25d, 6d, 0d, 27d, 0d, 0d, 0d)]
    public void ShouldGetExtentFromDurationWithExactMetricCalendar(double timeDuration, string unitTypeURI, TIMEUnitType unitTypeEnum, double scaleFactor,
        double? expectedYears, double? expectedMonths, double? expectedWeeks, double? expectedDays, double? expectedHours, double? expectedMinutes, double? expectedSeconds)
    {
        TIMEExtent te = TIMEConverter.ExtentFromDuration(timeDuration, new TIMEUnit(new RDFResource(unitTypeURI), unitTypeEnum, scaleFactor),
            new TIMECalendarReferenceSystem(
                new RDFResource("https://en.wikipedia.org/wiki/360-day_calendar"),
                new TIMECalendarReferenceSystemMetrics(60, 60, 24, [30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30])));

        Assert.IsNotNull(te);
        Assert.AreEqual(expectedYears, te.Years);
        Assert.AreEqual(expectedMonths, te.Months);
        Assert.AreEqual(expectedWeeks, te.Weeks);
        Assert.AreEqual(expectedDays, te.Days);
        Assert.AreEqual(expectedHours, te.Hours);
        Assert.AreEqual(expectedMinutes, te.Minutes);
        Assert.AreEqual(expectedSeconds, te.Seconds);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnNormalizingExtentBecauseNullExtent()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.NormalizeExtent(null, TIMECalendarReferenceSystem.Gregorian));

    [TestMethod]
    public void ShouldThrowExceptionOnNormalizingExtentBecauseNullCalendarTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEConverter.NormalizeExtent(new TIMEExtent(), null));

    [DataTestMethod]
    //seconds
    [DataRow(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 60, 0, 0, 0, 0, 0, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 61, 0, 0, 0, 0, 0, 1, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 3599, 0, 0, 0, 0, 0, 59, 59)]
    [DataRow(0, 0, 0, 0, 0, 0, 3600, 0, 0, 0, 0, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 3601, 0, 0, 0, 0, 1, 0, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 3660, 0, 0, 0, 0, 1, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 3661, 0, 0, 0, 0, 1, 1, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 86399, 0, 0, 0, 0, 23, 59, 59)]
    [DataRow(0, 0, 0, 0, 0, 0, 86400, 0, 0, 0, 1, 0, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 86401, 0, 0, 0, 1, 0, 0, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 86460, 0, 0, 0, 1, 0, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 86461, 0, 0, 0, 1, 0, 1, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 90000, 0, 0, 0, 1, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 90060, 0, 0, 0, 1, 1, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 0, 90061, 0, 0, 0, 1, 1, 1, 1)]
    [DataRow(0, 0, 0, 0, 0, 0, 3456000, 0, 0, 0, 40, 0, 0, 0)]
    //minutes
    [DataRow(0, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 0, 0, 30)]
    [DataRow(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 1.5, 0, 0, 0, 0, 0, 0, 1, 30)]
    [DataRow(0, 0, 0, 0, 0, 60, 0, 0, 0, 0, 0, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 60.5, 0, 0, 0, 0, 0, 1, 0, 30)]
    [DataRow(0, 0, 0, 0, 0, 61, 0, 0, 0, 0, 0, 1, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 61.5, 0, 0, 0, 0, 0, 1, 1, 30)]
    [DataRow(0, 0, 0, 0, 0, 1439, 0, 0, 0, 0, 0, 23, 59, 0)]
    [DataRow(0, 0, 0, 0, 0, 1440, 0, 0, 0, 0, 1, 0, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 1440.5, 0, 0, 0, 0, 1, 0, 0, 30)]
    [DataRow(0, 0, 0, 0, 0, 1441, 0, 0, 0, 0, 1, 0, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 1441.25, 0, 0, 0, 0, 1, 0, 1, 15)]
    [DataRow(0, 0, 0, 0, 0, 1500, 0, 0, 0, 0, 1, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 1500.75, 0, 0, 0, 0, 1, 1, 0, 45)]
    [DataRow(0, 0, 0, 0, 0, 1501, 0, 0, 0, 0, 1, 1, 1, 0)]
    [DataRow(0, 0, 0, 0, 0, 1501.5, 0, 0, 0, 0, 1, 1, 1, 30)]
    [DataRow(0, 0, 0, 0, 0, 57600, 0, 0, 0, 0, 40, 0, 0, 0)]
    //hours
    [DataRow(0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0, 0, 0, 30, 0)]
    [DataRow(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 1.25, 0, 0, 0, 0, 0, 0, 1, 15, 0)]
    [DataRow(0, 0, 0, 0, 24, 0, 0, 0, 0, 0, 1, 0, 0, 0)]
    [DataRow(0, 0, 0, 0, 24.5, 0, 0, 0, 0, 0, 1, 0, 30, 0)]
    [DataRow(0, 0, 0, 0, 25, 0, 0, 0, 0, 0, 1, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 25.75, 0, 0, 0, 0, 0, 1, 1, 45, 0)]
    [DataRow(0, 0, 0, 0, 8759, 0, 0, 0, 0, 0, 364, 23, 0, 0)]
    [DataRow(0, 0, 0, 0, 8760, 0, 0, 0, 0, 0, 365, 0, 0, 0)]
    [DataRow(0, 0, 0, 0, 8761.25, 0, 0, 0, 0, 0, 365, 1, 15, 0)]
    //days
    [DataRow(0, 0, 0, 0.005, 0, 0, 0, 0, 0, 0, 0, 0, 7, 12)]
    [DataRow(0, 0, 0, 0.05, 0, 0, 0, 0, 0, 0, 0, 1, 12, 0)]
    [DataRow(0, 0, 0, 0.5, 0, 0, 0, 0, 0, 0, 0, 12, 0, 0)]
    [DataRow(0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)]
    [DataRow(0, 0, 0, 1.25, 0, 0, 0, 0, 0, 0, 1, 6, 0, 0)]
    [DataRow(0, 0, 0, 1.255, 0, 0, 0, 0, 0, 0, 1, 6, 7, 12)]
    [DataRow(0, 0, 0, 40, 0, 0, 0, 0, 0, 0, 40, 0, 0, 0)]
    //weeks
    [DataRow(0, 0, 0.05, 0, 0, 0, 0, 0, 0, 0, 0, 8, 24, 0)]
    [DataRow(0, 0, 0.5, 0, 0, 0, 0, 0, 0, 0, 3, 12, 0, 0)]
    [DataRow(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0)]
    [DataRow(0, 0, 1.25, 0, 0, 0, 0, 0, 0, 0, 8, 18, 0, 0)]
    [DataRow(0, 0, 1.255, 0, 0, 0, 0, 0, 0, 0, 8, 18, 50, 24)]
    [DataRow(0, 0, 36, 0, 0, 0, 0, 0, 0, 0, 252, 0, 0, 0)]
    [DataRow(0, 0, 52.1, 0, 0, 0, 0, 0, 0, 0, 364, 16, 48, 0)]
    //months
    [DataRow(0, 0.00505, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 38, 9)]
    [DataRow(0, 0.005, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 36, 0)]
    [DataRow(0, 0.05, 0, 0, 0, 0, 0, 0, 0, 0, 1, 12, 0, 0)]
    [DataRow(0, 0.5, 0, 0, 0, 0, 0, 0, 0, 0, 15, 0, 0, 0)]
    [DataRow(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 30, 0, 0, 0)]
    [DataRow(0, 1.2, 0, 0, 0, 0, 0, 0, 0, 0, 36, 0, 0, 0)]
    [DataRow(0, 1.25, 0, 0, 0, 0, 0, 0, 0, 0, 37, 12, 0, 0)]
    [DataRow(0, 1.255, 0, 0, 0, 0, 0, 0, 0, 0, 37, 15, 36, 0)]
    [DataRow(0, 1.25502, 0, 0, 0, 0, 0, 0, 0, 0, 37, 15, 36, 51)]
    [DataRow(0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 360, 0, 0, 0)]
    //years
    [DataRow(0.052, 0, 0, 0, 0, 0, 0, 0, 0, 0, 18, 23, 31, 11)]
    [DataRow(0.5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 182, 12, 0, 0)]
    [DataRow(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 365, 0, 0, 0)]
    [DataRow(1.2425, 0, 0, 0, 0, 0, 0, 0, 0, 0, 453, 12, 18, 0)]
    //mixed components
    [DataRow(1, 1, 1, 40, 25, 64, 62, 0, 0, 0, 443, 2, 5, 2)]
    public void ShouldNormalizeExtentToGregorianCalendar(double originYears, double originMonths, double originWeeks, 
        double originDays, double originHours, double originMinutes, double originSeconds, double expectedYears, 
        double expectedMonths, double expectedWeeks, double expectedDays, double expectedHours, 
        double expectedMinutes, double expectedSeconds)
    {
        TIMEExtent te = TIMEConverter.NormalizeExtent(
            new TIMEExtent(originYears, originMonths, originWeeks, originDays, originHours, originMinutes, originSeconds),
            TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(te);
        Assert.AreEqual(expectedYears, te.Years);
        Assert.AreEqual(expectedMonths, te.Months);
        Assert.AreEqual(expectedWeeks, te.Weeks);
        Assert.AreEqual(expectedDays, te.Days);
        Assert.AreEqual(expectedHours, te.Hours);
        Assert.AreEqual(expectedMinutes, te.Minutes);
        Assert.AreEqual(expectedSeconds, te.Seconds);
    }

    [DataTestMethod]
    [DataRow(-2023, null, null, null, null, null, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 1476908, 11, 38, 10)]
    [DataRow(null, null, null, null, null, null, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 738513, 11, 38, 10)]
    [DataRow(2023, 4, 29, 11, 38, 10, null, null, null, null, null, null, 0, 0, 0, 738513, 11, 38, 10)] //swap
    [DataRow(2023, null, null, null, null, null, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 118, 11, 38, 10)]
    [DataRow(2023, 4, 29, 11, 38, 10, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 0, 0, 0, 0)]
    [DataRow(2023, 4, 29, 11, 38, 11, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 0, 0, 0, 1)]
    [DataRow(2023, 4, 29, 11, 39, 11, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 0, 0, 1, 1)]
    [DataRow(2023, 4, 29, 12, 39, 11, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 0, 1, 1, 1)]
    [DataRow(2023, 4, 30, 12, 39, 11, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 1, 1, 1, 1)]
    [DataRow(2023, 5, 30, 12, 39, 11, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 31, 1, 1, 1)]
    [DataRow(2024, 5, 30, 12, 39, 11, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 396, 1, 1, 1)]
    [DataRow(1983, 2, 10, 15, 30, 30, 2023, 4, 29, 11, 38, 10, 0, 0, 0, 14678, 20, 7, 40)]
    [DataRow(2023, 4, 29, 11, 38, 10, 1983, 2, 10, 15, 30, 30, 0, 0, 0, 14678, 20, 7, 40)] //swap
    [DataRow(1983, 2, 10, 15, 30, 30, 1997, 7, 14, 10, 30, 00, 0, 0, 0, 5263, 18, 59, 30)]
    [DataRow(2025, 1, 1, 0, 0, 0, 2024, 12, 31, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [DataRow(2025, 1, 31, 0, 0, 0, 2024, 10, 7, 24, 0, 0, 0, 0, 0, 118, 0, 0, 0)]
    public void ShouldCalculateExtentBetweenCoordinates(double startYear, double startMonth, double startDay, double startHour, double startMinute, double startSecond, 
        double endYear, double endMonth, double endDay, double endHour, double endMinute, double endSecond, 
        double expectedYears, double expectedMonths, double expectedWeeks, double expectedDays, double expectedHours, double expectedMinutes, double expectedSeconds)
    {
        TIMECoordinate startCoordinate = new TIMECoordinate(startYear, startMonth, startDay, startHour, startMinute, startSecond, 
            new TIMECoordinateMetadata(TIMECalendarReferenceSystem.Gregorian, RDFVocabulary.TIME.UNIT_SECOND));
        TIMECoordinate endCoordinate = new TIMECoordinate(endYear, endMonth, endDay, endHour, endMinute, endSecond,
            new TIMECoordinateMetadata(TIMECalendarReferenceSystem.Gregorian, RDFVocabulary.TIME.UNIT_SECOND));
        TIMEExtent te = TIMEConverter.ExtentBetweenCoordinates(startCoordinate, endCoordinate, TIMECalendarReferenceSystem.Gregorian);

        Assert.IsNotNull(te);
        Assert.AreEqual(expectedYears, te.Years);
        Assert.AreEqual(expectedMonths, te.Months);
        Assert.AreEqual(expectedWeeks, te.Weeks);
        Assert.AreEqual(expectedDays, te.Days);
        Assert.AreEqual(expectedHours, te.Hours);
        Assert.AreEqual(expectedMinutes, te.Minutes);
        Assert.AreEqual(expectedSeconds, te.Seconds);
    }
    #endregion
}