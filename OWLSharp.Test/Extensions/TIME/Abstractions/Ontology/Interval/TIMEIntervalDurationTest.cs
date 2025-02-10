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
    public class TIMEIntervalDurationTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateIntervalDuration()
        {
            TIMEIntervalDuration timeIntervalDuration = new TIMEIntervalDuration(
                new RDFResource("ex:intvDur"), TIMEUnit.MillionYearsAgo, 12.23);

            Assert.IsNotNull(timeIntervalDuration);
            Assert.IsTrue(timeIntervalDuration.URI.Equals(new Uri("ex:intvDur")));
            Assert.IsTrue(timeIntervalDuration.UnitType.Equals(TIMEUnit.MillionYearsAgo));
            Assert.AreEqual(12.23, timeIntervalDuration.Value);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingIntervalDurationBecauseNullUnitType()
            => Assert.ThrowsException<OWLException>(() => new TIMEIntervalDuration(new RDFResource("ex:intvDur"), null, 12.23));
        #endregion
    }
}