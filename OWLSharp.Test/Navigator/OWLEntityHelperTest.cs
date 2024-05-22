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
using OWLSharp.Modeler;
using OWLSharp.Modeler.Axioms;
using OWLSharp.Modeler.Expressions;
using OWLSharp.Navigator;
using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Test.Navigator
{
    [TestClass]
    public class OWLEntityHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetDeclaredClasses()
        {
            OWLOntology ont = new OWLOntology(new Uri("ex:ont"))
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.ORGANIZATION)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON))
                ]
            };
            List<RDFResource> cls = ont.GetDeclaredClasses();

            Assert.IsTrue(cls.Count == 3);
            Assert.IsTrue(cls.Any(c => c.Equals(RDFVocabulary.FOAF.AGENT)));
            Assert.IsTrue(cls.Any(c => c.Equals(RDFVocabulary.FOAF.ORGANIZATION)));
            Assert.IsTrue(cls.Any(c => c.Equals(RDFVocabulary.FOAF.PERSON)));
        }
        #endregion
    }
}