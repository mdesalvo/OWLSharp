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
public class TIMEIntervalHelperTest
{
    #region Methods
    //time:intervalAfter

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingAfterBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckAfter(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingAfterBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckAfter(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckAfter()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckAfterBecauseMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckAfterBecauseMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckAfter(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalBefore

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingBeforeBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckBefore(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingBeforeBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckBefore(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckBefore()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-23T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-04-27T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckBeforeBecauseMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckBeforeBecauseMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckBefore(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalContains

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalContainsIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckContains(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalContainsIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckContains(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalContainsInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-04-29T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalContainsIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-04-29T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalContainsIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-04-29T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalContainsIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-04-29T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalContainsIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-04-29T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckContains(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalDisjoint

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalIsDisjointWithIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckDisjoint(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalIsDisjointWithIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckDisjoint(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalIsDisjointWithInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2020-01-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2020-12-31T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2025-01-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2025-12-31T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-12-31T23:59:59Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2024-12-31T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2020-01-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-01-01T00:00:00Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-01-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-12-31T23:59:59Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckDisjoint(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsTrue(TIMEIntervalHelper.CheckDisjoint(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDisjoint(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDisjoint(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalDuring

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalDuringIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckDuring(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalDuringIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckDuring(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalDuringInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-03T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalDuringIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-03T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalDuringIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-03T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalDuringIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-03T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalDuringIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-03T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckDuring(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalEquals

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalEqualsIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckEquals(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalEqualsIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckEquals(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalEqualsInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalEqualsIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalEqualsIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalEqualsIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalEqualsIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:00:00Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckEquals(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalFinishedBy

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalFinishedByIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckFinishedBy(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalFinishedByIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckFinishedBy(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalFinishedByInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishedByIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishedByIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishedByIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishedByIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-28T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"))));


        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishedBy(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalFinishes

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalFinishesIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckFinishes(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalFinishesIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckFinishes(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalFinishesInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishesIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishesIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));


        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishesIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalFinishesIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckFinishes(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalIn

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalInIntervalBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckIn(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingIntervalInIntervalBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckIn(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckIntervalInInterval()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:23:59Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalInIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalInIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:23:59Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalInIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:23:59Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalInIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T00:23:59Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T23:59:59Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft4"),
            new TIMEInterval(new RDFResource("ex:timeIntvD"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningD"), DateTime.Parse("2023-04-30T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndD"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft5"),
            new TIMEInterval(new RDFResource("ex:timeIntvE"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningE"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndE"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvE")));
        Assert.IsFalse(TIMEIntervalHelper.CheckIn(timeOntology, new RDFResource("ex:timeIntvD"), new RDFResource("ex:timeIntvE")));
    }

    //time:intervalMeets

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingMeetsBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckMeets(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingMeetsBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckMeets(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckMeets()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-23T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckMeetsBecauseMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckMeetsBecauseMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMeets(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalMetBy

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingMetByBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckMetBy(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingMetByBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckMetBy(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckMetBy()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckMetByBecauseMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckMetByBecauseMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-08T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:14Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckMetBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalOverlaps

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingOverlapsBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckOverlaps(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingOverlapsBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckOverlaps(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckOverlaps()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-23T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlapsIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlapsIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-23T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlapsIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-23T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlapsIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-23T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-02T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlaps(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalOverlappedBy

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingOverlappedByBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckOverlappedBy(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingOverlappedByBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckOverlappedBy(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckOverlappedBy()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-06T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvC"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlappedByIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-06T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlappedByIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlappedByIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-06T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalOverlappedByIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-06T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:16Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckOverlappedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalStarts

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingStartsBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckStarts(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingStartsBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckStarts(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckStarts()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartsIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartsIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartsIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartsIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStarts(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    //time:intervalStartedBy

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingStartedByBecauseNullLeftIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckStartedBy(new OWLOntology(new Uri("ex:timeOnt")), null, new RDFResource()));

    [TestMethod]
    public void ShouldThrowExceptionOnCheckingStartedByBecauseNullRightIntervalURI()
        => Assert.ThrowsExactly<OWLException>(() => _ = TIMEIntervalHelper.CheckStartedBy(new OWLOntology(new Uri("ex:timeOnt")), new RDFResource(), null));

    [TestMethod]
    public void ShouldCheckStartedBy()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-05T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsTrue(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartedByIntervalMissingLeftIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartedByIntervalMissingLeftIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"))));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartedByIntervalMissingRightIntervalBeginningInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC")),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));

        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }

    [TestMethod]
    public void ShouldNotCheckIntervalStartedByIntervalMissingRightIntervalEndInformation()
    {
        OWLOntology timeOntology = new OWLOntology(new Uri("ex:timeOnt"));

        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft1"),
            new TIMEInterval(new RDFResource("ex:timeIntvA"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningA"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndA"), DateTime.Parse("2023-05-01T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft2"),
            new TIMEInterval(new RDFResource("ex:timeIntvB"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningB"), DateTime.Parse("2023-05-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndB"), DateTime.Parse("2023-05-04T20:47:15Z").ToUniversalTime())));
        timeOntology.DeclareIntervalFeature(new RDFResource("ex:ft3"),
            new TIMEInterval(new RDFResource("ex:timeIntvC"),
                new TIMEInstant(new RDFResource("ex:timeIntvBeginningC"), DateTime.Parse("2023-04-30T20:47:15Z").ToUniversalTime()),
                new TIMEInstant(new RDFResource("ex:timeIntvEndC"))));

        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvA"), new RDFResource("ex:timeIntvC")));
        Assert.IsFalse(TIMEIntervalHelper.CheckStartedBy(timeOntology, new RDFResource("ex:timeIntvB"), new RDFResource("ex:timeIntvC")));
    }
    #endregion
}