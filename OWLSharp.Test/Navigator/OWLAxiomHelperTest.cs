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

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Modeler;
using OWLSharp.Modeler.Axioms;
using OWLSharp.Modeler.Expressions;
using OWLSharp.Navigator;
using RDFSharp.Model;

namespace OWLSharp.Test.Navigator
{
    [TestClass]
    public class OWLAxiomHelperTest
    {
        #region Tests
        [TestMethod]
		public void ShouldGetSubClassesOf()
		{
			OWLOntology ont = new OWLOntology(new Uri("ex:ont"));
			ont.ClassAxioms.Add(new OWLSubClassOf(new OWLClass(new RDFResource("ex:ClsB")), new OWLClass(new RDFResource("ex:ClsA"))));
			ont.ClassAxioms.Add(new OWLSubClassOf(new OWLClass(new RDFResource("ex:ClsC")), new OWLClass(new RDFResource("ex:ClsB"))));
			List<RDFResource> subClassesOfClsA = ont.GetSubClassesOf(new RDFResource("ex:ClsA"));

			Assert.IsNotNull(subClassesOfClsA);
			Assert.IsTrue(subClassesOfClsA.Count == 2);
			Assert.IsTrue(subClassesOfClsA[0].Equals(new RDFResource("ex:ClsB")));
			Assert.IsTrue(subClassesOfClsA[1].Equals(new RDFResource("ex:ClsC")));
		}
        #endregion
    }
}