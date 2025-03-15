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
using OWLSharp.Ontology;
using RDFSharp.Model;
using System;

namespace OWLSharp.Test.Extensions.TIME;

[TestClass]
public class TIMEInstantHelperTest
{
    #region Methods
    //time:inside

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingInsideBecauseNullInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckInsideInterval(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingInsideBecauseNullIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckInsideInterval(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckInside()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft4"),
            new TIMEInstant(new RDFResource("ex:timeInstD"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA"))); //boundaries are excluded
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstD"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckInsideBecauseMissingInstantInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft4"),
            new TIMEInstant(new RDFResource("ex:timeInstD"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstD"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckInsideBecauseMissingIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft4"),
            new TIMEInstant(new RDFResource("ex:timeInstD"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstD"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckInsideBecauseMissingIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft4"),
            new TIMEInstant(new RDFResource("ex:timeInstD"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));

        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckInsideInterval(timeOntology, new RDFResource("ex:timeInstD"), new RDFResource("ex:timeIntvA")));
    }

    //time:after

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingAfterBecauseNullAInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckAfter(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingAfterBecauseNullBInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckAfter(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckAfter()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));

        Assert.IsTrue(TIMEInstantHelper.CheckAfter(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeInstD")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfter(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeInstD")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfter(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeInstD")));
    }

    [TestMethod]
    public void ShouldNotCheckAfterBecauseMissingInstantInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA")));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));

        Assert.IsFalse(TIMEInstantHelper.CheckAfter(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeInstB")));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingAfterIntervalBecauseNullInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckAfterInterval(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingAfterIntervalBecauseNullIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckAfterInterval(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckAfterInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckAfterIntervalBecauseMissingInstantInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckAfterIntervalBecauseMissingIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));

        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckAfterInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
    }

    //time:before

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingBeforeBecauseNullAInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckBefore(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingBeforeBecauseNullBInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckBefore(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckBefore()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-05-09T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstD"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime()));

        Assert.IsTrue(TIMEInstantHelper.CheckBefore(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeInstD")));
        Assert.IsFalse(TIMEInstantHelper.CheckBefore(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeInstD")));
        Assert.IsFalse(TIMEInstantHelper.CheckBefore(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeInstD")));
    }

    [TestMethod]
    public void ShouldNotCheckBeforeBecauseMissingInstantInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA")));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime()));

        Assert.IsFalse(TIMEInstantHelper.CheckBefore(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeInstB")));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingBeforeIntervalBecauseNullInstantURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckBeforeInterval(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingBeforeIntervalBecauseNullIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEInstantHelper.CheckBeforeInterval(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckBeforeInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckBeforeIntervalBecauseMissingInstantInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
    }

    [TestMethod]
    public void ShouldNotCheckBeforeIntervalBecauseMissingIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft1"),
            new TIMEInstant(new RDFResource("ex:timeInstA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft2"),
            new TIMEInstant(new RDFResource("ex:timeInstB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareInstantFeature(new RDFResource("ex:ft3"),
            new TIMEInstant(new RDFResource("ex:timeInstC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstA"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstB"), new RDFResource("ex:timeIntvA")));
        Assert.IsFalse(TIMEInstantHelper.CheckBeforeInterval(timeOntology, new RDFResource("ex:timeInstC"), new RDFResource("ex:timeIntvA")));
    }
    #endregion
}