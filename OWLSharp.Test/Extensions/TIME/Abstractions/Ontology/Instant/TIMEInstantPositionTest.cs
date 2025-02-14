﻿/*
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
public class TIMEInstantPositionTest
{
    #region Tests
    [TestMethod]
    public void ShouldCreateNumericInstantPosition()
    {
        TIMEInstantPosition timeInstantPosition = new TIMEInstantPosition(
            new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.UnixTime, 121977842);

        Assert.IsNotNull(timeInstantPosition);
        Assert.IsTrue(timeInstantPosition.URI.Equals(new Uri("ex:instPos")));
        Assert.AreEqual(121977842, timeInstantPosition.NumericValue);
        Assert.IsFalse(timeInstantPosition.IsNominal);
        Assert.IsNull(timeInstantPosition.PositionalUncertainty);
    }

    [TestMethod]
    public void ShouldCreateNumericInstantPositionWithUncertainty()
    {
        TIMEInstantPosition timeInstantPosition = new TIMEInstantPosition(
            new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.UnixTime, 121977842)
        {
            PositionalUncertainty = new TIMEIntervalDuration(
                new RDFResource("ex:posUnc"), TIMEUnit.Second, 4.04)
        };

        Assert.IsNotNull(timeInstantPosition);
        Assert.IsTrue(timeInstantPosition.URI.Equals(new Uri("ex:instPos")));
        Assert.AreEqual(121977842, timeInstantPosition.NumericValue);
        Assert.IsFalse(timeInstantPosition.IsNominal);
        Assert.IsNotNull(timeInstantPosition.PositionalUncertainty);
        Assert.IsTrue(timeInstantPosition.PositionalUncertainty.UnitType.Equals(TIMEUnit.Second));
        Assert.AreEqual(4.04, timeInstantPosition.PositionalUncertainty.Value);
    }

    [TestMethod]
    public void ShouldCreateNominalInstantPosition()
    {
        TIMEInstantPosition timeInstantPosition = new TIMEInstantPosition(
            new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.GeologicTime, new RDFResource("geol:Archean"));

        Assert.IsNotNull(timeInstantPosition);
        Assert.IsTrue(timeInstantPosition.URI.Equals(new Uri("ex:instPos")));
        Assert.IsTrue(timeInstantPosition.IsNominal);
        Assert.IsTrue(timeInstantPosition.NominalValue.Equals(new RDFResource("geol:Archean")));
        Assert.IsNull(timeInstantPosition.PositionalUncertainty);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNumericInstantPositionBecauseNullTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEInstantPosition(new RDFResource("ex:instPos"), null, 121977842));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNominalInstantPositionBecauseNullTRS()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEInstantPosition(new RDFResource("ex:instPos"), null, new RDFResource("ex:Archean")));

    [TestMethod]
    public void ShouldThrowExceptionOnCreatingNominalInstantPositionBecauseNullValue()
        => Assert.ThrowsExactly<OWLException>(() => _ = new TIMEInstantPosition(new RDFResource("ex:instPos"), TIMEPositionReferenceSystem.GeologicTime, null));
    #endregion
}