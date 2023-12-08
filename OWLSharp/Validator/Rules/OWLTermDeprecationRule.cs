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

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL validator rule checking for usage of deprecated classes and properties
    /// </summary>
    internal static class OWLTermDeprecationRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:DeprecatedClass
            IEnumerator<RDFResource> deprecatedClassesEnumerator = ontology.Model.ClassModel.DeprecatedClassesEnumerator;
            while (deprecatedClassesEnumerator.MoveNext())
            {
                //It should be avoided the assigment of deprecated classes as class types of individuals
                if (ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, deprecatedClassesEnumerator.Current, null].TriplesCount > 0)
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Warning,
                        nameof(OWLTermDeprecationRule),
                        $"Deprecated class '{deprecatedClassesEnumerator.Current}' is used by individuals through 'rdf:type' relation",
                        "Revise your 'rdf:type' relations: abandon usage of deprecated classes (which may be removed in future ontology editions)"));
            }

            //owl:DeprecatedProperty
            IEnumerator<RDFResource> deprecatedPropertiesEnumerator = ontology.Model.PropertyModel.DeprecatedPropertiesEnumerator;
            while (deprecatedPropertiesEnumerator.MoveNext())
            {
                //It should be avoided the usage of deprecated properties in assertions
                if (ontology.Data.ABoxGraph[null, deprecatedPropertiesEnumerator.Current, null, null].TriplesCount > 0)
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Warning,
                        nameof(OWLTermDeprecationRule),
                        $"Deprecated property '{deprecatedPropertiesEnumerator.Current}' is used by individuals through object or datatype assertions",
                        "Revise your object or datatype assertions: abandon usage of deprecated properties (which may be removed in future ontology editions)"));
            }

            return validatorRuleReport;
        }
    }
}