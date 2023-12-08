/*
   Copyright 2012-2024 Marco De Salvo
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
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 validator rule checking for consistency of assertions using irreflexive properties [OWL2]
    /// </summary>
    internal static class OWLIrreflexivePropertyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:IrreflexiveProperty
            IEnumerator<RDFResource> irreflexivePropertiesEnumerator = ontology.Model.PropertyModel.IrreflexivePropertiesEnumerator;
            while (irreflexivePropertiesEnumerator.MoveNext())
            {
                //There should not be irreflexive object assertions having the same subject as object
                RDFGraph irreflexiveObjectAssertions = ontology.Data.ABoxGraph[null, irreflexivePropertiesEnumerator.Current, null, null];
                if (irreflexiveObjectAssertions.Any(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO && asn.Subject.Equals(asn.Object)))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLIrreflexivePropertyRule),
                        $"Violation of 'owl:IrreflexiveProperty' behavior on property '{irreflexivePropertiesEnumerator.Current}'",
                        "Revise your object assertions: fix irreflexive property usage in order to not clash on subject/object irreflexivity"));
            }

            return validatorRuleReport;
        }
    }
}