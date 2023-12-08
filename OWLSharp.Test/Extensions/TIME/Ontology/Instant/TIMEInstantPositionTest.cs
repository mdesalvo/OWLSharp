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
using RDFSharp.Model;
using System;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEInstantPositionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateNumericInstantPosition()
        {
            TIMEInstantPosition timeInstantPosition = new TIMEInstantPosition(
                new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.UnixTRS, 121977842);

            Assert.IsNotNull(timeInstantPosition);
            Assert.IsTrue(timeInstantPosition.URI.Equals(new Uri("ex:instPos")));
            Assert.IsTrue(timeInstantPosition.NumericValue == 121977842);
            Assert.IsFalse(timeInstantPosition.IsNominal);
            Assert.IsNull(timeInstantPosition.PositionalUncertainty);
        }

        [TestMethod]
        public void ShouldCreateNumericInstantPositionWithUncertainty()
        {
            TIMEInstantPosition timeInstantPosition = new TIMEInstantPosition(
                new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.UnixTRS, 121977842);
            timeInstantPosition.SetPositionalUncertainty(new TIMEIntervalDuration(
                new RDFResource("ex:posUnc"), TIMEUnit.Second, 4.04));

            Assert.IsNotNull(timeInstantPosition);
            Assert.IsTrue(timeInstantPosition.URI.Equals(new Uri("ex:instPos")));
            Assert.IsTrue(timeInstantPosition.NumericValue == 121977842);
            Assert.IsFalse(timeInstantPosition.IsNominal);
            Assert.IsNotNull(timeInstantPosition.PositionalUncertainty);
            Assert.IsTrue(timeInstantPosition.PositionalUncertainty.UnitType.Equals(TIMEUnit.Second));
            Assert.IsTrue(timeInstantPosition.PositionalUncertainty.Value == 4.04);
        }

        [TestMethod]
        public void ShouldCreateNominalInstantPosition()
        {
            TIMEInstantPosition timeInstantPosition = new TIMEInstantPosition(
                new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.GeologicTRS, new RDFResource("geol:Archean"));

            Assert.IsNotNull(timeInstantPosition);
            Assert.IsTrue(timeInstantPosition.URI.Equals(new Uri("ex:instPos")));
            Assert.IsTrue(timeInstantPosition.IsNominal);
            Assert.IsTrue(timeInstantPosition.NominalValue.Equals(new RDFResource("geol:Archean")));
            Assert.IsNull(timeInstantPosition.PositionalUncertainty);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNumericInstantPositionBecauseNullTRS()
            => Assert.ThrowsException<OWLException>(() => new TIMEInstantPosition(new RDFResource("ex:instPos"), null, 121977842));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNominalInstantPositionBecauseNullTRS()
            => Assert.ThrowsException<OWLException>(() => new TIMEInstantPosition(new RDFResource("ex:instPos"), null, new RDFResource("ex:Archean")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingNominalInstantPositionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new TIMEInstantPosition(new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.GeologicTRS, null));
        #endregion
    }
}