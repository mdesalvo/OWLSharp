/*
   Copyright 2012-2024 Marco De Salvo

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
using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMECalendarReferenceSystemTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateCalendarTRS()
        {
            TIMECalendarReferenceSystem ModifiedGregorianTRS = new TIMECalendarReferenceSystem(
                new RDFResource("ex:ModifiedGregorian"),
                new TIMECalendarReferenceSystemMetrics(60, 60, 24, new uint[] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 }));

            Assert.IsNotNull(ModifiedGregorianTRS);
            Assert.IsTrue(ModifiedGregorianTRS.Equals(new RDFResource("ex:ModifiedGregorian")));
            Assert.IsNotNull(ModifiedGregorianTRS.Metrics);
            Assert.IsTrue(ModifiedGregorianTRS.Metrics.HasExactMetric);
            Assert.IsTrue(ModifiedGregorianTRS.Metrics.SecondsInMinute == 60);
            Assert.IsTrue(ModifiedGregorianTRS.Metrics.MinutesInHour == 60);
            Assert.IsTrue(ModifiedGregorianTRS.Metrics.HoursInDay == 24);
            Assert.IsTrue(ModifiedGregorianTRS.Metrics.MonthsInYear == 12);
            Assert.IsTrue(ModifiedGregorianTRS.Metrics.DaysInYear == 360);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseNullMetrics()
            => Assert.ThrowsException<OWLException>(() =>
                new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseInvalidMetrics1()
            => Assert.ThrowsException<OWLException>(() =>
                new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"),
                new TIMECalendarReferenceSystemMetrics(0, 60, 24, new uint[] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 })));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseInvalidMetrics2()
            => Assert.ThrowsException<OWLException>(() =>
                new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"),
                new TIMECalendarReferenceSystemMetrics(60, 0, 24, new uint[] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 })));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseInvalidMetrics3()
            => Assert.ThrowsException<OWLException>(() =>
                new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"),
                new TIMECalendarReferenceSystemMetrics(60, 60, 0, new uint[] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 })));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseInvalidMetrics4()
           => Assert.ThrowsException<OWLException>(() =>
               new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"),
               new TIMECalendarReferenceSystemMetrics(60, 60, 24, null)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseInvalidMetrics5()
           => Assert.ThrowsException<OWLException>(() =>
               new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"),
               new TIMECalendarReferenceSystemMetrics(60, 60, 24, new uint[] { })));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseInvalidMetrics6()
           => Assert.ThrowsException<OWLException>(() =>
               new TIMECalendarReferenceSystem(new RDFResource("ex:ModifiedGregorian"),
               new TIMECalendarReferenceSystemMetrics(60, 60, 24, new uint[] { 30, 30, 0, 31 })));
        #endregion
    }
}