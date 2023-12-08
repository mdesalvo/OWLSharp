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
    /// OWL2 validator rule checking for consistency of assertions using asymmetric properties [OWL2]
    /// </summary>
    internal static class OWLAsymmetricPropertyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:AsymmetricProperty
            IEnumerator<RDFResource> asymmetricPropertiesEnumerator = ontology.Model.PropertyModel.AsymmetricPropertiesEnumerator;
            while (asymmetricPropertiesEnumerator.MoveNext())
            {
                //There should not be asymmetric object assertions switching subject/object
                RDFGraph asymmetricObjectAssertions = ontology.Data.ABoxGraph[null, asymmetricPropertiesEnumerator.Current, null, null];
                if (asymmetricObjectAssertions.Any(asn => asn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO && ontology.Data.CheckHasObjectAssertion((RDFResource)asn.Object, asymmetricPropertiesEnumerator.Current, (RDFResource)asn.Subject)))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLAsymmetricPropertyRule),
                        $"Violation of 'owl:AsymmetricProperty' behavior on property '{asymmetricPropertiesEnumerator.Current}'",
                        "Revise your object assertions: fix asymmetric property usage in order to not clash on subject/object asimmetry"));
            }

            return validatorRuleReport;
        }
    }
}