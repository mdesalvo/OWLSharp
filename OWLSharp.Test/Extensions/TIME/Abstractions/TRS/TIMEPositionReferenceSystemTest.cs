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
            Assert.AreEqual(2000, MillenniumTimeTRS.Origin.Year);
            Assert.AreEqual(1, MillenniumTimeTRS.Origin.Month);
            Assert.AreEqual(1, MillenniumTimeTRS.Origin.Day);
            Assert.AreEqual(0, MillenniumTimeTRS.Origin.Hour);
            Assert.AreEqual(0, MillenniumTimeTRS.Origin.Minute);
            Assert.AreEqual(0, MillenniumTimeTRS.Origin.Second);
            Assert.IsNotNull(MillenniumTimeTRS.Unit);
            Assert.AreEqual(TIMEEnums.TIMEUnitType.Day, MillenniumTimeTRS.Unit.UnitType);
            Assert.AreEqual(1, MillenniumTimeTRS.Unit.ScaleFactor);
            Assert.IsFalse(MillenniumTimeTRS.HasLargeScaleSemantic);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseNullOrigin()
            => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEPositionReferenceSystem(new RDFResource("ex:ModifiedUnixTime"), null, TIMEUnit.Second, false));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatimeCalendarTRSBecauseNullUnit()
            => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEPositionReferenceSystem(new RDFResource("ex:ModifiedUnixTime"), TIMECoordinate.UnixTime, null, false));
        #endregion
    }
}