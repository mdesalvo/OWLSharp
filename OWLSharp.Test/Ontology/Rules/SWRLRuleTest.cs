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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Rules;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLRuleTest
    {
        #region Methods
        [TestMethod]
        public void ShouldCreateSWRLRule()
        {
            SWRLRule rule = new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent(),
                new SWRLConsequent());

            Assert.IsNotNull(rule);
            Assert.IsNotNull(rule.Annotations);
            Assert.IsTrue(rule.Annotations.Count == 2);
            Assert.IsNotNull(rule.Antecedent);
            Assert.IsNotNull(rule.Consequent);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullRuleName()
            => Assert.ThrowsException<OWLException>(() => 
                new SWRLRule(
                    null,
                    new RDFPlainLiteral("This is a test SWRL rule"),
                    new SWRLAntecedent(),
                    new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullRuleDescription()
            => Assert.ThrowsException<OWLException>(() =>
                new SWRLRule(
                    new RDFPlainLiteral("SWRL1"),
                    null,
                    new SWRLAntecedent(),
                    new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullAntecedent()
           => Assert.ThrowsException<OWLException>(() =>
               new SWRLRule(
                   new RDFPlainLiteral("SWRL1"),
                   new RDFPlainLiteral("This is a test SWRL rule"),
                   null,
                   new SWRLConsequent()));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSWRLRuleBecauseNullConsequent()
           => Assert.ThrowsException<OWLException>(() =>
               new SWRLRule(
                   new RDFPlainLiteral("SWRL1"),
                   new RDFPlainLiteral("This is a test SWRL rule"),
                   new SWRLAntecedent(),
                   null));

        [TestMethod]
        public void ShouldGetStringRepresentationOfSWRLRule()
        {
            SWRLRule rule = new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent() { 
                    Atoms = [ 
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.FOAF.PERSON), 
                            new SWRLVariableArgument(new RDFVariable("?P")))
                    ],
                    BuiltIns = [
                        SWRLBuiltIn.Add(
                            new SWRLVariableArgument(new RDFVariable("?A")),
                            new SWRLVariableArgument(new RDFVariable("?B")),
                            44.57),
                        SWRLBuiltIn.Matches(
                            new SWRLVariableArgument(new RDFVariable("?P")),
                            "Mark", 
                            RegexOptions.IgnoreCase)
                    ]
                },
                new SWRLConsequent() {
                    Atoms = [
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.FOAF.AGENT),
                            new SWRLVariableArgument(new RDFVariable("?P")))
                    ] });

            Assert.IsTrue(string.Equals("Person(?P) ^ swrlb:add(?A,?B,\"44.57\"^^xsd:double) ^ swrlb:matches(?P,\"Mark\",\"i\") -> Agent(?P)", rule.ToString()));
        }

        [TestMethod]
        public void ShouldGetXMLRepresentationOfSWRLRule()
        {
            SWRLRule rule = new SWRLRule(
                new RDFPlainLiteral("SWRL1"),
                new RDFPlainLiteral("This is a test SWRL rule"),
                new SWRLAntecedent()
                {
                    Atoms = [
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.FOAF.PERSON),
                            new SWRLVariableArgument(new RDFVariable("?P"))),
                        new SWRLDataPropertyAtom(
                            new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                            new SWRLVariableArgument(new RDFVariable("?P")),
                            new SWRLVariableArgument(new RDFVariable("?N")))
                    ],
                    BuiltIns = [
                        SWRLBuiltIn.ContainsIgnoreCase(
                            new SWRLVariableArgument(new RDFVariable("?N")), "mark")
                    ]
                },
                new SWRLConsequent()
                {
                    Atoms = [
                        new SWRLClassAtom(
                            new OWLClass(RDFVocabulary.FOAF.AGENT),
                            new SWRLVariableArgument(new RDFVariable("?P")))
                    ]
                });

            Assert.IsTrue(string.Equals(
@"<DLSafeRule><Annotation><AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" /><Literal>SWRL1</Literal></Annotation><Annotation><AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" /><Literal>This is a test SWRL rule</Literal></Annotation><Body><ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom><DataPropertyAtom><DataProperty IRI=""http://xmlns.com/foaf/0.1/name"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#N"" /></DataPropertyAtom><BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#containsIgnoreCase""><Variable IRI=""urn:swrl:var#N"" /><Literal>mark</Literal></BuiltInAtom></Body><Head><ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom></Head></DLSafeRule>", rule.GetXML()));
        }

        [TestMethod]
        public void ShouldGetSWRLRuleFromXMLRepresentation()
        {
            SWRLRule rule = OWLSerializer.DeserializeObject<SWRLRule>(
@"<DLSafeRule><Annotation><AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#label"" /><Literal>SWRL1</Literal></Annotation><Annotation><AnnotationProperty IRI=""http://www.w3.org/2000/01/rdf-schema#comment"" /><Literal>This is a test SWRL rule</Literal></Annotation><Body><ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Person"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom><DataPropertyAtom><DataProperty IRI=""http://xmlns.com/foaf/0.1/name"" /><Variable IRI=""urn:swrl:var#P"" /><Variable IRI=""urn:swrl:var#N"" /></DataPropertyAtom><BuiltInAtom IRI=""http://www.w3.org/2003/11/swrlb#containsIgnoreCase""><Variable IRI=""urn:swrl:var#N"" /><Literal>mark</Literal></BuiltInAtom></Body><Head><ClassAtom><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><Variable IRI=""urn:swrl:var#P"" /></ClassAtom></Head></DLSafeRule>");

            Assert.IsNotNull(rule);
            Assert.IsNotNull(rule.Annotations);
            Assert.IsTrue(rule.Annotations.Count == 2);
            Assert.IsNotNull(rule.Antecedent);
            Assert.IsTrue(rule.Antecedent.Atoms.Count == 2);
            Assert.IsTrue(rule.Antecedent.BuiltIns.Count == 1);
            Assert.IsNotNull(rule.Consequent);
            Assert.IsTrue(rule.Consequent.Atoms.Count == 1);
            Assert.IsTrue(string.Equals("Person(?P) ^ name(?P,?N) ^ swrlb:containsIgnoreCase(?N,\"mark\") -> Agent(?P)", rule.ToString()));
        }

        [TestMethod]
        public async Task ShouldApplySWRLRuleToOntologyAsync()
        {
            OWLOntology ontology = new OWLOntology()
            {
                DeclarationAxioms = [
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.PERSON)),
                    new OWLDeclaration(new OWLClass(RDFVocabulary.FOAF.AGENT)),
                    new OWLDeclaration(new OWLDataProperty(RDFVocabulary.FOAF.NAME)),
                    new OWLDeclaration(new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                ],
                AssertionAxioms = [
                    new OWLClassAssertion(
                        new OWLClass(RDFVocabulary.FOAF.PERSON),
                        new OWLNamedIndividual(new RDFResource("ex:Mark"))),
                    new OWLDataPropertyAssertion(
                        new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                        new OWLNamedIndividual(new RDFResource("ex:Mark")),
                        new OWLLiteral(new RDFPlainLiteral("Mark", "en-US")))
                ],
                Rules = [
                    new SWRLRule(
                        new RDFPlainLiteral("SWRL1"),
                        new RDFPlainLiteral("This is a test SWRL rule"),
                        new SWRLAntecedent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.PERSON),
                                    new SWRLVariableArgument(new RDFVariable("?P"))),
                                new SWRLDataPropertyAtom(
                                    new OWLDataProperty(RDFVocabulary.FOAF.NAME),
                                    new SWRLVariableArgument(new RDFVariable("?P")),
                                    new SWRLVariableArgument(new RDFVariable("?N")))
                            ],
                            BuiltIns = [
                                SWRLBuiltIn.ContainsIgnoreCase(
                                    new SWRLVariableArgument(new RDFVariable("?N")), "mark")
                            ]
                        },
                        new SWRLConsequent()
                        {
                            Atoms = [
                                new SWRLClassAtom(
                                    new OWLClass(RDFVocabulary.FOAF.AGENT),
                                    new SWRLVariableArgument(new RDFVariable("?P")))
                            ]
                        })
                ]
            };
            List<OWLInference> inferences = await ontology.Rules[0].ApplyToOntologyAsync(ontology);

            Assert.IsNotNull(inferences);
            Assert.IsTrue(inferences.Count == 1);
            Assert.IsTrue(inferences[0].Axiom is OWLClassAssertion clsAsnInf
                            && clsAsnInf.ClassExpression.GetIRI().Equals(RDFVocabulary.FOAF.AGENT)
                            && clsAsnInf.IndividualExpression.GetIRI().Equals(new RDFResource("ex:Mark")));
        }
        #endregion
    }
}