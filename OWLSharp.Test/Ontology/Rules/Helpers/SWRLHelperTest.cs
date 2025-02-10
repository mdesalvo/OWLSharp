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
using OWLSharp.Ontology;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class SWRLHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldDeclareRule()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareRule(new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent(),
                new SWRLConsequent()));

            Assert.AreEqual(1, ontology.Rules.Count);
            Assert.IsTrue(ontology.CheckHasRule(new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent(),
                new SWRLConsequent())));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareRule(null));

            ontology.DeclareRule(new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent(),
                new SWRLConsequent())); //will be discarded, since duplicates are not allowed
            Assert.AreEqual(1, ontology.Rules.Count);
        }
        #endregion
    }
}