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
    /// RDFS validator rule checking for consistency of rdfs:domain and rdfs:range behavior of properties 
    /// </summary>
    internal static class OWLDomainRangeRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();
            Dictionary<long, HashSet<long>> individualsCache = new Dictionary<long, HashSet<long>>();

            //owl:ObjectAssertion
            IEnumerator<RDFResource> objectProperties = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectProperties.MoveNext())
            {
                List<RDFResource> domainClasses = ontology.Model.PropertyModel.GetDomainOf(objectProperties.Current);
                List<RDFResource> rangeClasses = ontology.Model.PropertyModel.GetRangeOf(objectProperties.Current);
                if (domainClasses.Any() || rangeClasses.Any())
                {
                    //Materialize cache of individuals belonging to domain/range classes
                    foreach (RDFResource domainClass in domainClasses)
                    { 
                        if (!individualsCache.ContainsKey(domainClass.PatternMemberID))
                            individualsCache.Add(domainClass.PatternMemberID, new HashSet<long>(ontology.Data.GetIndividualsOf(ontology.Model, domainClass).Select(idv => idv.PatternMemberID)));
                    }
                    foreach (RDFResource rangeClass in rangeClasses)
                    { 
                        if (!individualsCache.ContainsKey(rangeClass.PatternMemberID))
                            individualsCache.Add(rangeClass.PatternMemberID, new HashSet<long>(ontology.Data.GetIndividualsOf(ontology.Model, rangeClass).Select(idv => idv.PatternMemberID)));
                    }

                    //Analyze A-BOX object assertions using the current object property
                    RDFGraph objectPropertyAssertions = ontology.Data.ABoxGraph[null, objectProperties.Current, null, null];
                    foreach (RDFTriple objectPropertyAssertion in objectPropertyAssertions)
                    {
                        //Subject of object property assertion should be compatible with specified rdfs:domain
                        if (domainClasses.Any(domainClass => individualsCache[domainClass.PatternMemberID].Count > 0 && !individualsCache[domainClass.PatternMemberID].Contains(objectPropertyAssertion.Subject.PatternMemberID)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLDomainRangeRule),
                                $"Violation of 'rdfs:domain' behavior on property '{objectProperties.Current}'",
                                $"Revise 'rdf:type' relations of '{objectPropertyAssertion.Subject}' individual: it is not compatible with expected 'rdfs:domain' constraint"));

                        //Object of object property assertion should be compatible with specified rdfs:range
                        if (rangeClasses.Any(rangeClass => individualsCache[rangeClass.PatternMemberID].Count > 0 && !individualsCache[rangeClass.PatternMemberID].Contains(objectPropertyAssertion.Object.PatternMemberID)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLDomainRangeRule),
                                $"Violation of 'rdfs:range' behavior on property '{objectProperties.Current}'",
                                $"Revise 'rdf:type' relations of '{objectPropertyAssertion.Object}' individual: it is not compatible with expected 'rdfs:range' constraint"));
                    }
                }
            }

            //owl:DatatypeAssertion
            IEnumerator<RDFResource> datatypeProperties = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypeProperties.MoveNext())
            {
                List<RDFResource> domainClasses = ontology.Model.PropertyModel.GetDomainOf(datatypeProperties.Current);
                if (domainClasses.Any())
                {
                    //Materialize cache of individuals belonging to domain class
                    foreach (RDFResource domainClass in domainClasses)
                    { 
                        if (!individualsCache.ContainsKey(domainClass.PatternMemberID))
                            individualsCache.Add(domainClass.PatternMemberID, new HashSet<long>(ontology.Data.GetIndividualsOf(ontology.Model, domainClass).Select(idv => idv.PatternMemberID)));
                    }

                    //Analyze A-BOX datatype assertions using the current datatype property
                    RDFGraph datatypePropertyAssertions = ontology.Data.ABoxGraph[null, datatypeProperties.Current, null, null];
                    foreach (RDFTriple datatypePropertyAssertion in datatypePropertyAssertions)
                    {
                        //Subject of datatype property assertion should be compatible with specified rdfs:domain
                        if (domainClasses.Any(domainClass => individualsCache[domainClass.PatternMemberID].Count > 0 && !individualsCache[domainClass.PatternMemberID].Contains(datatypePropertyAssertion.Subject.PatternMemberID)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLDomainRangeRule),
                                $"Violation of 'rdfs:domain' behavior on property '{datatypeProperties.Current}'",
                                $"Revise 'rdf:type' relations of '{datatypePropertyAssertion.Subject}' individual: it is not compatible with expected 'rdfs:domain' constraint"));
                    }
                }
            }

            return validatorRuleReport;
        }
    }
}