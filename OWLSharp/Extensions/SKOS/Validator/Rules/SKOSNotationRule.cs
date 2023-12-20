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
    /// SKOS validator rule checking for consistency of skos:notation relations
    /// </summary>
    internal class SKOSNotationRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //This validator rule is suitable to be modeled as a reasoner rule producing violation inferences.
            //(Since relying on SWRL, it requires SKOS T-BOX to be already initialized on the working ontology)
            SWRLRule swrlRule = new SWRLRule(
                nameof(SKOSNotationRule),
                "CONCEPT_SCHEME(?CS) ^ CONCEPT(?C1) ^ CONCEPT(?C2) ^ IN_SCHEME(?C1,?CS) ^ IN_SCHEME(?C2,?CS) ^ NOTATION(?C1,?N) ^ NOTATION(?C2,?N) -> error",
                new SWRLAntecedent()
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.SKOS.CONCEPT_SCHEME, new RDFVariable("?CS")))
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.SKOS.CONCEPT, new RDFVariable("?C1")))
                    .AddAtom(new SWRLClassAtom(RDFVocabulary.SKOS.CONCEPT, new RDFVariable("?C2")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.SKOS.IN_SCHEME, new RDFVariable("?C1"), new RDFVariable("?CS")))
                    .AddAtom(new SWRLObjectPropertyAtom(RDFVocabulary.SKOS.IN_SCHEME, new RDFVariable("?C2"), new RDFVariable("?CS")))
                    .AddAtom(new SWRLDataPropertyAtom(RDFVocabulary.SKOS.NOTATION, new RDFVariable("?C1"), new RDFVariable("?N")))
                    .AddAtom(new SWRLDataPropertyAtom(RDFVocabulary.SKOS.NOTATION, new RDFVariable("?C2"), new RDFVariable("?N")))
                    .AddBuiltIn(new SWRLNotEqualBuiltIn(new RDFVariable("?C1"), new RDFVariable("?C2"))),
                new SWRLConsequent()
                    .AddAtom(new SWRLObjectPropertyAtom(new RDFResource("ex:skosNotationFailure"), new RDFVariable("?C1"), new RDFVariable("?C2"))));

            //Then every violation inference is transferred to the validator
            OWLReasonerReport reasonerReport = swrlRule.ApplyToOntology(ontology);
            foreach (OWLReasonerEvidence reasonerEvidence in reasonerReport)
                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                    nameof(SKOSNotationRule),
                    $"Violation of 'skos:notation' behavior for concepts '{reasonerEvidence.EvidenceContent.Subject}' and '{reasonerEvidence.EvidenceContent.Object}'",
                    "An instance of 'skos:notation' should not be shared between concepts belonging to the same 'skos:ConceptScheme'"));

            return validatorRuleReport;
        }
    }
}