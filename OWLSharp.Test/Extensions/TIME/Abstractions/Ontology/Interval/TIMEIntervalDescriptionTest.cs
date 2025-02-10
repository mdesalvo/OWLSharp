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
    public class TIMEIntervalDescriptionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIntervalDescriptionFromTimeSpan()
        {
            TIMEIntervalDescription timeIntervalDescription = new TIMEIntervalDescription(
                new RDFResource("ex:intvDesc"), XmlConvert.ToTimeSpan("PT5H7M"));

            Assert.IsNotNull(timeIntervalDescription);
            Assert.IsTrue(timeIntervalDescription.URI.Equals(new Uri("ex:intvDesc")));
            Assert.IsTrue(timeIntervalDescription.Extent.Equals(new TIMEExtent(0, 0, 0, 0, 5, 7, 0)));
        }

        [TestMethod]
        public void ShouldCreateIntervalDescriptionFromLength()
        {
            TIMEIntervalDescription timeIntervalDescription = new TIMEIntervalDescription(
                new RDFResource("ex:intvDesc"), 
                new TIMEExtent(0, 0, 0, 0, 5, 7, 0, new TIMEExtentMetadata(TIMECalendarReferenceSystem.Gregorian)));

            Assert.IsNotNull(timeIntervalDescription);
            Assert.IsTrue(timeIntervalDescription.URI.Equals(new Uri("ex:intvDesc")));
            Assert.IsTrue(timeIntervalDescription.Extent.Equals(new TIMEExtent(0, 0, 0, 0, 5, 7, 0)));
            Assert.IsTrue(timeIntervalDescription.Extent.Metadata.TRS.Equals(TIMECalendarReferenceSystem.Gregorian));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIntervalFromDescriptionBecauseNullDescription()
            => Assert.ThrowsException<OWLException>(() => new TIMEIntervalDescription(new RDFResource("ex:intvDesc"), null));

        [TestMethod]
        public void ShouldCompareIntervalDescriptions()
        {
            TIMEIntervalDescription timeIntervalDescriptionA = new TIMEIntervalDescription(
                new RDFResource("ex:intvDesc"),
                new TIMEExtent(0, 0, 0, 0, 5, 7, 0));
            TIMEIntervalDescription timeIntervalDescriptionB = new TIMEIntervalDescription(
                new RDFResource("ex:intvDesc"),
                new TIMEExtent(0, 0, 0, 0, 5, 2, 0));

            Assert.AreEqual(1, timeIntervalDescriptionA.CompareTo(timeIntervalDescriptionB));
        }

        [TestMethod]
        public void ShouldEqualIntervalDescriptions()
        {
            TIMEIntervalDescription timeIntervalDescriptionA = new TIMEIntervalDescription(
                new RDFResource("ex:intvDesc"),
                new TIMEExtent(0, 0, 0, 0, 5, 7, 0));
            TIMEIntervalDescription timeIntervalDescriptionB = new TIMEIntervalDescription(
                new RDFResource("ex:intvDesc"),
                new TIMEExtent(0, 0, 0, 0, 5, 7, 0));

            Assert.IsTrue(timeIntervalDescriptionA.Equals(timeIntervalDescriptionB));
        }
        #endregion
    }
}