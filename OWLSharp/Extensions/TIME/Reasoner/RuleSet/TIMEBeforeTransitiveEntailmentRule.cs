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
using System.Threading.Tasks;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Extensions.TIME
{
    internal static class TIMEBeforeTransitiveEntailmentRule
    {
        internal static async Task<List<OWLInference>> ExecuteRuleAsync(OWLOntology ontology, Dictionary<string, List<OWLIndividualExpression>> cacheRegistry)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            SWRLRule swrlRuleForInstants = new SWRLRule(
                new RDFPlainLiteral(nameof(TIMEBeforeTransitiveEntailmentRule)),
                new RDFPlainLiteral("BEFORE(?I1,?I2) ^ BEFORE(?I3,?I1) -> BEFORE(?I3,?I2)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INSTANT), new SWRLVariableArgument(new RDFVariable("?I1"))) { IndividualsCache = cacheRegistry["INSTANTS"] },
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INSTANT), new SWRLVariableArgument(new RDFVariable("?I2"))) { IndividualsCache = cacheRegistry["INSTANTS"] },
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INSTANT), new SWRLVariableArgument(new RDFVariable("?I3"))) { IndividualsCache = cacheRegistry["INSTANTS"] },
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.BEFORE), new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.BEFORE), new SWRLVariableArgument(new RDFVariable("?I2")), new SWRLVariableArgument(new RDFVariable("?I3")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I3"))),
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I2")), new SWRLVariableArgument(new RDFVariable("?I3")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.BEFORE), new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I3")))
                    }
                });
            inferences.AddRange(await swrlRuleForInstants.ApplyToOntologyAsync(ontology));

            SWRLRule swrlRuleForIntervals = new SWRLRule(
                new RDFPlainLiteral(nameof(TIMEBeforeTransitiveEntailmentRule)),
                new RDFPlainLiteral("BEFORE(?I1,?I2) ^ BEFORE(?I3,?I1) -> BEFORE(?I3,?I2)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INTERVAL), new SWRLVariableArgument(new RDFVariable("?I1"))),
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INTERVAL), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INTERVAL), new SWRLVariableArgument(new RDFVariable("?I3"))),
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE), new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE), new SWRLVariableArgument(new RDFVariable("?I3")), new SWRLVariableArgument(new RDFVariable("?I1")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I3"))),
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I2")), new SWRLVariableArgument(new RDFVariable("?I3")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_BEFORE), new SWRLVariableArgument(new RDFVariable("?I3")), new SWRLVariableArgument(new RDFVariable("?I2")))
                    }
                });
            inferences.AddRange(await swrlRuleForIntervals.ApplyToOntologyAsync(ontology));

            return inferences;
        }
    }
}