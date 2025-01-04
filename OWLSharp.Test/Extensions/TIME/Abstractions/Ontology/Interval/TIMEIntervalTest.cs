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
using System.Xml;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEIntervalTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIntervalFromTimeSpan()
        {
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:intv"), XmlConvert.ToTimeSpan("P1Y"));

            Assert.IsNotNull(timeInterval);
            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:intv")));
            Assert.IsTrue(timeInterval.TimeSpan.HasValue && timeInterval.TimeSpan.Equals(XmlConvert.ToTimeSpan("P1Y")));
            Assert.IsNull(timeInterval.Description);
            Assert.IsNull(timeInterval.Duration);
            Assert.IsNull(timeInterval.Beginning);
            Assert.IsNull(timeInterval.End);
        }

        [TestMethod]
        public void ShouldCreateIntervalFromDescription()
        {
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:intv"), 
                new TIMEIntervalDescription(new RDFResource("ex:intvDesc"), XmlConvert.ToTimeSpan("P1Y")));

            Assert.IsNotNull(timeInterval);
            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:intv")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNotNull(timeInterval.Description);
            Assert.IsTrue(timeInterval.Description.URI.Equals(new Uri("ex:intvDesc")));
            Assert.IsTrue(timeInterval.Description.Extent.Equals(new TIMEExtent(0, 0, 0, 365, 0, 0, 0)));
            Assert.IsNull(timeInterval.Duration);
        }

        [TestMethod]
        public void ShouldCreateIntervalFromDescriptionWithLength()
        {
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:intv"),
                new TIMEIntervalDescription(
                    new RDFResource("ex:intvDesc"),
                    new TIMEExtent(1, 0, 0, 0, 0, 0, 0, new TIMEExtentMetadata(TIMECalendarReferenceSystem.Gregorian))));

            Assert.IsNotNull(timeInterval);
            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:intv")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNotNull(timeInterval.Description);
            Assert.IsTrue(timeInterval.Description.URI.Equals(new Uri("ex:intvDesc")));
            Assert.IsTrue(timeInterval.Description.Extent.Equals(new TIMEExtent(1, 0, 0, 0, 0, 0, 0)));
            Assert.IsTrue(timeInterval.Description.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsNull(timeInterval.Duration);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIntervalFromDescriptionBecauseNullDescription()
            => Assert.ThrowsException<OWLException>(() => new TIMEInterval(new RDFResource("ex:intv"), null as TIMEIntervalDescription));

        [TestMethod]
        public void ShouldCreateIntervalFromDuration()
        {
            TIMEInterval timeInterval = new TIMEInterval(new RDFResource("ex:intv"),
                new TIMEIntervalDuration(new RDFResource("ex:intvDur"), TIMEUnit.Century, 8));

            Assert.IsNotNull(timeInterval);
            Assert.IsTrue(timeInterval.URI.Equals(new Uri("ex:intv")));
            Assert.IsFalse(timeInterval.TimeSpan.HasValue);
            Assert.IsNull(timeInterval.Description);
            Assert.IsNotNull(timeInterval.Duration);
            Assert.IsTrue(timeInterval.Duration.URI.Equals(new Uri("ex:intvDur")));
            Assert.IsTrue(timeInterval.Duration.UnitType.Equals(TIMEUnit.Century));
            Assert.IsTrue(timeInterval.Duration.Value == 8);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIntervalFromDurationBecauseNullDuration()
            => Assert.ThrowsException<OWLException>(() => new TIMEInterval(new RDFResource("ex:intv"), null as TIMEIntervalDuration));
        #endregion
    }
}