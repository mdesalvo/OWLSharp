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
    internal static class TIMEEqualsEntailmentRule
    {
        internal static async Task<List<OWLInference>> ExecuteRuleAsync(OWLOntology ontology, Dictionary<string, List<OWLIndividualExpression>> cacheRegistry)
        {
            SWRLRule swrlRule = new SWRLRule(
                new RDFPlainLiteral(nameof(TIMEEqualsEntailmentRule)),
                new RDFPlainLiteral("STARTS(?I1,?I2) ^ FINISHES(?I1,?I2) -> EQUALS(?I1,?I2)"),
                new SWRLAntecedent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INTERVAL), new SWRLVariableArgument(new RDFVariable("?I1"))) { IndividualsCache = cacheRegistry["INTERVALS"] },
                        new SWRLClassAtom(new OWLClass(RDFVocabulary.TIME.INTERVAL), new SWRLVariableArgument(new RDFVariable("?I2"))) { IndividualsCache = cacheRegistry["INTERVALS"] },
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_STARTS), new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_FINISHES), new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2")))
                    },
                    BuiltIns = new List<SWRLBuiltIn>
                    {
                        SWRLBuiltIn.NotEqual(new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2")))
                    }
                },
                new SWRLConsequent
                {
                    Atoms = new List<SWRLAtom>
                    {
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS), new SWRLVariableArgument(new RDFVariable("?I1")), new SWRLVariableArgument(new RDFVariable("?I2"))),
                        new SWRLObjectPropertyAtom(new OWLObjectProperty(RDFVocabulary.TIME.INTERVAL_EQUALS), new SWRLVariableArgument(new RDFVariable("?I2")), new SWRLVariableArgument(new RDFVariable("?I1")))
                    }
                });

            return await swrlRule.ApplyToOntologyAsync(ontology);
        }
    }
}