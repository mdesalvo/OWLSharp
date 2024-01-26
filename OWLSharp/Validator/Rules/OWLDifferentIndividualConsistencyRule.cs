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

using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL validator rule checking for consistency of owl:differentFrom relations (useful after import/merge scenarios)
    /// </summary>
    internal static class OWLDifferentIndividualConsistencyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //Iterate the individuals of the data
            IEnumerator<RDFResource> individualsEnumerator = ontology.Data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext())
            {
                //Get the differentFrom individuals of the current individual
                foreach (RDFResource differentIndividual in ontology.Data.GetDifferentIndividuals(individualsEnumerator.Current))
                    if (ontology.Data.CheckIsSameIndividual(individualsEnumerator.Current, differentIndividual))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLDifferentIndividualConsistencyRule),
                            $"Violation of 'owl:differentFrom' hierarchy of individuals '{individualsEnumerator.Current}'",
                            $"Revise you data: after import/merge actions you have '{differentIndividual}' at the same time differentFrom and sameAs '{individualsEnumerator.Current}'"));
            }

            return validatorRuleReport;
        }
    }
}