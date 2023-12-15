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
    /// Given temporal intervals I1, I2 and I3: AFTER(?I1,?I2) ^ FINISHES(?I2,?I3) -> AFTER(?I1,?I3)
    /// </summary>
    internal static class TIMEAfterFinishesEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            SWRLRule swrlRule = new SWRLRule(
                nameof(TIMEAfterFinishesEntailmentRule),
                "AFTER(?I1,?I2) ^ FINISHES(?I2,?I3) -> AFTER(?I1,?I3)",
                new SWRLAntecedent()
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.TIME.INTERVAL, new RDFVariable("?I1")))
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.TIME.INTERVAL, new RDFVariable("?I2")))
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.TIME.INTERVAL, new RDFVariable("?I3")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.TIME.INTERVAL_AFTER, new RDFVariable("?I1"), new RDFVariable("?I2")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.TIME.INTERVAL_FINISHES, new RDFVariable("?I2"), new RDFVariable("?I3")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?I1"), new RDFVariable("?I2")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?I1"), new RDFVariable("?I3")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?I2"), new RDFVariable("?I3"))),
                new SWRLConsequent()
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.TIME.INTERVAL_AFTER, new RDFVariable("?I1"), new RDFVariable("?I3"))));

            OWLReasonerReport reasonerRuleReport = swrlRule.ApplyToOntology(ontology);
            return reasonerRuleReport;
        }
    }
}