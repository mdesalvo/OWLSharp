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

using OWLSharp.Extensions.SWRL;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// Given SKOS concepts C1, C2, C3: EXACT_MATCH(?C1,?C2) ^ EXACT_MATCH(?C2,?C3) -> EXACT_MATCH(?C1,?C3)
    /// </summary>
    internal static class SKOSExactMatchEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            SWRLRule swrlRule = new SWRLRule(
                nameof(SKOSExactMatchEntailmentRule),
                "EXACT_MATCH(?C1,?C2) ^ EXACT_MATCH(?C2,?C3) -> EXACT_MATCH(?C1,?C3)",
                new SWRLAntecedent()
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.SKOS.CONCEPT, new RDFVariable("?C1")))
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.SKOS.CONCEPT, new RDFVariable("?C2")))
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.SKOS.CONCEPT, new RDFVariable("?C3")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.SKOS.EXACT_MATCH, new RDFVariable("?C1"), new RDFVariable("?C2")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.SKOS.EXACT_MATCH, new RDFVariable("?C2"), new RDFVariable("?C3")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?C1"), new RDFVariable("?C2")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?C1"), new RDFVariable("?C3")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?C2"), new RDFVariable("?C3"))),
                new SWRLConsequent()
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.SKOS.EXACT_MATCH, new RDFVariable("?C1"), new RDFVariable("?C3")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.SKOS.EXACT_MATCH, new RDFVariable("?C3"), new RDFVariable("?C1"))));

            OWLReasonerReport reasonerRuleReport = swrlRule.ApplyToOntology(ontology);
            return reasonerRuleReport;
        }
    }
}