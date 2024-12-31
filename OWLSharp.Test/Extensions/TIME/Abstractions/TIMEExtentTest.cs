/*
   Copyright 2014-2024 Marco De Salvo

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
using System.Xml;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEExtentTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateExtent()
        {
            TIMEExtent timeExtent = new TIMEExtent(1, 2, 3, 4, 5, 6, 7);

            Assert.IsNotNull(timeExtent);
            Assert.IsTrue(timeExtent.Years == 1);
            Assert.IsTrue(timeExtent.Months == 2);
            Assert.IsTrue(timeExtent.Weeks == 3);
            Assert.IsTrue(timeExtent.Days == 4);
            Assert.IsTrue(timeExtent.Hours == 5);
            Assert.IsTrue(timeExtent.Minutes == 6);
            Assert.IsTrue(timeExtent.Seconds == 7);
            Assert.IsTrue(timeExtent.ToString() == "1_2_3_4_5_6_7");
            Assert.IsNotNull(timeExtent.Metadata);
            Assert.IsNull(timeExtent.Metadata.TRS);
        }

        [TestMethod]
        public void ShouldCreateExtentWithPartialValues()
        {
            TIMEExtent timeExtent = new TIMEExtent(1, 2, 3, null, null, null, null);

            Assert.IsNotNull(timeExtent);
            Assert.IsTrue(timeExtent.Years == 1);
            Assert.IsTrue(timeExtent.Months == 2);
            Assert.IsTrue(timeExtent.Weeks == 3);
            Assert.IsNull(timeExtent.Days);
            Assert.IsNull(timeExtent.Hours);
            Assert.IsNull(timeExtent.Minutes);
            Assert.IsNull(timeExtent.Seconds);
            Assert.IsTrue(timeExtent.ToString() == "1_2_3_0_0_0_0");
            Assert.IsNotNull(timeExtent.Metadata);
            Assert.IsNull(timeExtent.Metadata.TRS);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeYears()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(-1, 1, 1, 1, 1, 1, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeMonths()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(1, -1, 1, 1, 1, 1, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeWeeks()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(1, 1, -1, 1, 1, 1, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeDays()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(1, 1, 1, -1, 1, 1, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeHours()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(1, 1, 1, 1, -1, 1, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeMinutes()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(1, 1, 1, 1, 1, -1, 1));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingExtentBecauseNegativeSeconds()
            => Assert.ThrowsException<OWLException>(() => new TIMEExtent(1, 1, 1, 1, 1, 1, -1));

        [TestMethod]
        public void ShouldCreateExtentFromTimeSpan()
        {
            TIMEExtent timeExtent = new TIMEExtent(XmlConvert.ToTimeSpan("P1Y2M3DT4H5M6S"));

            Assert.IsNotNull(timeExtent);
            Assert.IsNull(timeExtent.Years);
            Assert.IsNull(timeExtent.Months);
            Assert.IsNull(timeExtent.Weeks);
            Assert.IsTrue(timeExtent.Days == 428);
            Assert.IsTrue(timeExtent.Hours == 4);
            Assert.IsTrue(timeExtent.Minutes == 5);
            Assert.IsTrue(timeExtent.Seconds == 6);
            Assert.IsNotNull(timeExtent.Metadata);
            Assert.IsNotNull(timeExtent.Metadata.TRS);
            Assert.IsTrue(timeExtent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        }

        [DataTestMethod]
        [DataRow("P1Y", null, 1)]
        [DataRow("P1Y", "P11M", 1)]
        [DataRow("P11M", "P1Y", -1)]
        [DataRow("P1Y", "P1Y", 0)]
        public void ShouldCompareExtents(string leftTimeSpan, string rightTimeSpan, int expectedComparison)
        {
            TIMEExtent leftExtent = new TIMEExtent(XmlConvert.ToTimeSpan(leftTimeSpan));
            TIMEExtent rightExtent = rightTimeSpan == null ? null : new TIMEExtent(XmlConvert.ToTimeSpan(rightTimeSpan));

            Assert.IsTrue(leftExtent.CompareTo(rightExtent) == expectedComparison);
        }

        [DataTestMethod]
        [DataRow(2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]        
        [DataRow(1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]        
        [DataRow(1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 2d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 2d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, null, 1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, null, 1d, 1d, 1d, 1d, 1d, 1d, 1d, -1)]
        [DataRow(1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 1d, 0)]
        [DataRow(null, 0d, null, 0d, null, 0d, null, 0d, null, 0d, null, 0d, null, 0d, 0)]
        public void ShouldComparePartialExtents(double? years1, double? months1, double? weeks1, double? days1, double? hours1, double? minutes1, double? seconds1,
            double? years2, double? months2, double? weeks2, double? days2, double? hours2, double? minutes2, double? seconds2, int expectedComparison)
        {
            TIMEExtent leftExtent = new TIMEExtent(years1, months1, weeks1, days1, hours1, minutes1, seconds1);
            TIMEExtent rightExtent = new TIMEExtent(years2, months2, weeks2, days2, hours2, minutes2, seconds2);

            Assert.IsTrue(leftExtent.CompareTo(rightExtent) == expectedComparison);
        }
        #endregion
    }
}