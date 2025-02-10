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
    public class OWLDatatypeDefinitionHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldDeclareDatatypeDefinition()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareDatatypeDefinition(new OWLDatatypeDefinition(
                new OWLDatatype(new RDFResource("ex:length6to10")),
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                     new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)])));

            Assert.AreEqual(1, ontology.DatatypeDefinitionAxioms.Count);
            Assert.IsTrue(ontology.CheckHasDatatypeDefinition(new OWLDatatypeDefinition(
                new OWLDatatype(new RDFResource("ex:length6to10")),
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                     new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]))));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareDatatypeDefinition(null));

            ontology.DeclareDatatypeDefinition(new OWLDatatypeDefinition(
                new OWLDatatype(new RDFResource("ex:length6to10")),
                new OWLDatatypeRestriction(
                    new OWLDatatype(RDFVocabulary.XSD.STRING),
                    [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MIN_LENGTH),
                     new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), RDFVocabulary.XSD.MAX_LENGTH)]))); //will be discarded, since duplicates are not allowed
            Assert.AreEqual(1, ontology.DatatypeDefinitionAxioms.Count);
        }
        #endregion
    }
}