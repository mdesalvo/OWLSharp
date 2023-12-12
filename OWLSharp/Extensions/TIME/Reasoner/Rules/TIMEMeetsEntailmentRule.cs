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

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// Given temporal intervals I1, I2 and I3: MEETS(?I1,?I2) ^ STARTS(?I2,?I3) -> MEETS(?I1,?I3)
    /// </summary>
    internal static class TIMEMeetsEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            OWLReasonerRule swrlRule = new OWLReasonerRule(
                nameof(TIMEMeetsEntailmentRule),
                "MEETS(?I1,?I2) ^ STARTS(?I2,?I3) -> MEETS(?I1,?I3)",
                new OWLReasonerRuleAntecedent()
                    .AddAtom(new OWLReasonerRuleClassAtom(RDFVocabulary.TIME.INTERVAL, new RDFVariable("?I1")))
                    .AddAtom(new OWLReasonerRuleClassAtom(RDFVocabulary.TIME.INTERVAL, new RDFVariable("?I2")))
                    .AddAtom(new OWLReasonerRuleClassAtom(RDFVocabulary.TIME.INTERVAL, new RDFVariable("?I3")))
                    .AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.TIME.INTERVAL_MEETS, new RDFVariable("?I1"), new RDFVariable("?I2")))
                    .AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.TIME.INTERVAL_STARTS, new RDFVariable("?I2"), new RDFVariable("?I3")))
                    .AddBuiltIn(new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?I1"), new RDFVariable("?I2")))
                    .AddBuiltIn(new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?I1"), new RDFVariable("?I3")))
                    .AddBuiltIn(new OWLReasonerRuleNotEqualBuiltIn(new RDFVariable("?I2"), new RDFVariable("?I3"))),
                new OWLReasonerRuleConsequent()
                    .AddAtom(new OWLReasonerRuleObjectPropertyAtom(RDFVocabulary.TIME.INTERVAL_MEETS, new RDFVariable("?I1"), new RDFVariable("?I3"))));

            OWLReasonerReport reasonerRuleReport = swrlRule.ApplyToOntology(ontology);
            return reasonerRuleReport;
        }
    }
}