﻿/*
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEOntologyLoaderTest
    {
        #region Tests
        [TestMethod]
        public void ShouldInitializeTIME()
        {
            OWLOntology ontology = new OWLOntology("ex:timeOnt");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:MyClass"));

            Assert.IsTrue(ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:MyClass")));
            Assert.IsFalse(ontology.Model.ClassModel.CheckHasSimpleClass(RDFVocabulary.TIME.INSTANT));

            ontology.InitializeTIME();

            Assert.IsTrue(ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:MyClass")));
            Assert.IsFalse(ontology.Model.ClassModel.TBoxGraph[new RDFResource("ex:MyClass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Single().TripleMetadata.HasValue);
                        
            Assert.IsTrue(ontology.Model.ClassModel.CheckHasSimpleClass(RDFVocabulary.TIME.INSTANT));
            Assert.IsTrue(ontology.Model.ClassModel.TBoxGraph[RDFVocabulary.TIME.INSTANT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Single().TripleMetadata.HasValue
                && ontology.Model.ClassModel.TBoxGraph[RDFVocabulary.TIME.INSTANT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Single().IsImport());
        }

        [TestMethod]
        public void ShouldThrowExceptionOnInitializingTIMEBecauseNullOntology()
        {
            OWLOntology ontology = default;
            Assert.ThrowsException<OWLException>(() => ontology.InitializeTIME());
        }
        #endregion
    }
}