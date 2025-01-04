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

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEPositionReferenceSystemTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreatePositionTRS()
        {
            TIMEPositionReferenceSystem MillenniumTimeTRS = new TIMEPositionReferenceSystem(
                new RDFResource("ex:MillenniumTime"),
                new TIMECoordinate(2000, 1, 1, 0, 0, 0),
                TIMEUnit.Day);

            Assert.IsNotNull(MillenniumTimeTRS);
            Assert.IsTrue(MillenniumTimeTRS.Equals(new RDFResource("ex:MillenniumTime")));
            Assert.IsNotNull(MillenniumTimeTRS.Origin);
            Assert.IsTrue(MillenniumTimeTRS.Origin.Year == 2000);
            Assert.IsTrue(MillenniumTimeTRS.Origin.Month == 1);
            Assert.IsTrue(MillenniumTimeTRS.Origin.Day == 1);
            Assert.IsTrue(MillenniumTimeTRS.Origin.Hour == 0);
            Assert.IsTrue(MillenniumTimeTRS.Origin.Minute == 0);
            Assert.IsTrue(MillenniumTimeTRS.Origin.Second == 0);
            Assert.IsNotNull(MillenniumTimeTRS.Unit);
            Assert.IsTrue(MillenniumTimeTRS.Unit.UnitType == TIMEEnums.TIMEUnitType.Day);
            Assert.IsTrue(MillenniumTimeTRS.Unit.ScaleFactor == 1);
            Assert.IsFalse(MillenniumTimeTRS.HasLargeScaleSemantic);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseNullOrigin()
            => Assert.ThrowsException<OWLException>(() => 
                new TIMEPositionReferenceSystem(new RDFResource("ex:ModifiedUnixTime"), null, TIMEUnit.Second, false));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseNullUnit()
            => Assert.ThrowsException<OWLException>(() =>
                new TIMEPositionReferenceSystem(new RDFResource("ex:ModifiedUnixTime"), TIMECoordinate.UnixTime, null, false));
        #endregion
    }
}