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
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL validator rule checking for consistency of assertions using symmetric properties
    /// </summary>
    internal static class OWLSymmetricPropertyRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();
            Dictionary<long, HashSet<long>> individualsCache = new Dictionary<long, HashSet<long>>();

            //owl:SymmetricProperty
            IEnumerator<RDFResource> symmetricPropertiesEnumerator = ontology.Model.PropertyModel.SymmetricPropertiesEnumerator;
            while (symmetricPropertiesEnumerator.MoveNext())
            {
                List<RDFResource> domainClasses = ontology.Model.PropertyModel.GetDomainOf(symmetricPropertiesEnumerator.Current);
                List<RDFResource> rangeClasses = ontology.Model.PropertyModel.GetRangeOf(symmetricPropertiesEnumerator.Current);
                if (domainClasses.Any() || rangeClasses.Any())
                {
                    //Materialize cache of individuals belonging to domain/range classes
                    foreach (RDFResource domainClass in domainClasses)
                        if (!individualsCache.ContainsKey(domainClass.PatternMemberID))
                            individualsCache.Add(domainClass.PatternMemberID, new HashSet<long>(ontology.Data.GetIndividualsOf(ontology.Model, domainClass).Select(idv => idv.PatternMemberID)));
                    foreach (RDFResource rangeClass in rangeClasses)
                        if (!individualsCache.ContainsKey(rangeClass.PatternMemberID))
                            individualsCache.Add(rangeClass.PatternMemberID, new HashSet<long>(ontology.Data.GetIndividualsOf(ontology.Model, rangeClass).Select(idv => idv.PatternMemberID)));
                    
                    //Analyze A-BOX object assertions using the current symmetric property
                    RDFGraph symmetricObjectAssertions = ontology.Data.ABoxGraph[null, symmetricPropertiesEnumerator.Current, null, null];
                    foreach (RDFTriple symmetricObjectAssertion in symmetricObjectAssertions)
                    {
                        //Object of symmetric assertions should be compatible with specified rdfs:domain
                        if (domainClasses.Any(domainClass => individualsCache[domainClass.PatternMemberID].Count > 0 && !individualsCache[domainClass.PatternMemberID].Contains(symmetricObjectAssertion.Object.PatternMemberID)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLSymmetricPropertyRule),
                                $"Violation of 'owl:SymmetricProperty' behavior on property '{symmetricPropertiesEnumerator.Current}'",
                                "Revise your object assertions: fix symmetric property usage in order to not tamper rdfs:domain constraints of this property"));

                        //Subject of symmetric property assertion should be compatible with specified rdfs:range
                        if (rangeClasses.Any(rangeClass => individualsCache[rangeClass.PatternMemberID].Count > 0 && !individualsCache[rangeClass.PatternMemberID].Contains(symmetricObjectAssertion.Subject.PatternMemberID)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLSymmetricPropertyRule),
                                $"Violation of 'owl:SymmetricProperty' behavior on property '{symmetricPropertiesEnumerator.Current}'",
                                "Revise your object assertions: fix symmetric property usage in order to not tamper rdfs:range constraints of this property"));
                    }
                }
            }

            return validatorRuleReport;
        }
    }
}