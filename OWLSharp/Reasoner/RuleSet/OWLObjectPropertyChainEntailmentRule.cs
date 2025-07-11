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

using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLObjectPropertyChainEntailmentRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.ObjectPropertyChainEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Prepare subgraph for analysis of property chain
            Lazy<RDFGraph> lazyOPAsnsGraph = new Lazy<RDFGraph>(() =>
            {
                RDFGraph opAsnsGraph = new RDFGraph();
                foreach (OWLObjectPropertyAssertion opAsn in ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>())
                    foreach (RDFTriple opAsnTriple in opAsn.ToRDFGraph())
                        opAsnsGraph.AddTriple(opAsnTriple);
                return opAsnsGraph;
            });

            //SubObjectPropertyOf(PC,OP) ^ ObjectPropertyChain(PC,(OP1..OPN)) -> ObjectPropertyAssertion(OP,OP1,OPN)
            foreach (OWLSubObjectPropertyOf subObjectPropertyOf in ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>()
                                                                           .Where(ax => ax.SubObjectPropertyChain != null))
            {
                //Transform OWL2 property chain into equivalent SPARQL property path
                RDFPropertyPath propertyPath = new RDFPropertyPath(new RDFVariable("?PROPERTY_CHAIN_AXIOM_START"), new RDFVariable("?PROPERTY_CHAIN_AXIOM_END"));
                foreach (OWLObjectPropertyExpression propertyPathStep in subObjectPropertyOf.SubObjectPropertyChain.ObjectPropertyExpressions)
                    propertyPath.AddSequenceStep(propertyPathStep is OWLObjectInverseOf objInvOfPropertyPathStep ? new RDFPropertyPathStep(objInvOfPropertyPathStep.ObjectProperty.GetIRI()).Inverse()
                                                                                                                 : new RDFPropertyPathStep(propertyPathStep.GetIRI()));

                //Execute SPARQL construct query to materialize property chain inferences
                RDFPattern templatePattern = subObjectPropertyOf.SuperObjectPropertyExpression is OWLObjectInverseOf objInvOfSuperObjPropExpr
                    ? new RDFPattern(new RDFVariable("?PROPERTY_CHAIN_AXIOM_END"), objInvOfSuperObjPropExpr.ObjectProperty.GetIRI(), new RDFVariable("?PROPERTY_CHAIN_AXIOM_START"))
                    : new RDFPattern(new RDFVariable("?PROPERTY_CHAIN_AXIOM_START"), subObjectPropertyOf.SuperObjectPropertyExpression.GetIRI(), new RDFVariable("?PROPERTY_CHAIN_AXIOM_END"));
                RDFConstructQueryResult materializedChainTriples =
                    new RDFConstructQuery()
                        .AddPatternGroup(new RDFPatternGroup()
                            .AddPropertyPath(propertyPath)
                            .AddFilter(new RDFExpressionFilter(new RDFIsUriExpression(new RDFVariable("?PROPERTY_CHAIN_AXIOM_START"))))
                            .AddFilter(new RDFExpressionFilter(new RDFIsUriExpression(new RDFVariable("?PROPERTY_CHAIN_AXIOM_END")))))
                        .AddTemplate(templatePattern)
                        .ApplyToGraph(lazyOPAsnsGraph.Value);

                //Populate result with corresponding inference assertions
                foreach (RDFTriple materializedChainTriple in materializedChainTriples.ToRDFGraph())
                {
                    //Rebuild source individual (preserve support for anonymous individuals)
                    OWLIndividualExpression infSrcIdvExpr;
                    if (((RDFResource)materializedChainTriple.Subject).IsBlank)
                        infSrcIdvExpr = new OWLAnonymousIndividual(materializedChainTriple.Subject.ToString().Replace("bnode:", string.Empty));
                    else
                        infSrcIdvExpr = new OWLNamedIndividual((RDFResource)materializedChainTriple.Subject);

                    //Rebuild target individual (preserve support for anonymous individuals)
                    OWLIndividualExpression infTgtIdvExpr;
                    if (((RDFResource)materializedChainTriple.Object).IsBlank)
                        infTgtIdvExpr = new OWLAnonymousIndividual(materializedChainTriple.Object.ToString().Replace("bnode:", string.Empty));
                    else
                        infTgtIdvExpr = new OWLNamedIndividual((RDFResource)materializedChainTriple.Object);

                    OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion(new OWLObjectProperty((RDFResource)materializedChainTriple.Predicate), infSrcIdvExpr, infTgtIdvExpr) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }

            return inferences;
        }
    }
}