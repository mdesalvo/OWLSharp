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
    /// OWL2 validator rule checking for consistency of negative assertions [OWL2]
    /// </summary>
    internal static class OWLNegativeAssertionsRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();
            Dictionary<long, List<RDFResource>> sameIndividualsCache = new Dictionary<long, List<RDFResource>>();
            Dictionary<long, List<RDFResource>> compatiblePropertiesCache = new Dictionary<long, List<RDFResource>>();

            //owl:NegativeObjectProperty
            RDFGraph objectAssertions = OWLOntologyDataLoader.GetObjectAssertions(ontology, ontology.Data.ABoxGraph);
            foreach (RDFTriple negativeObjectAssertion in OWLOntologyDataLoader.GetNegativeObjectAssertions(ontology.Data.ABoxGraph))
            {
                //Enlist the individuals which are compatible with negative assertion subject
                if (!sameIndividualsCache.ContainsKey(negativeObjectAssertion.Subject.PatternMemberID))
                    sameIndividualsCache.Add(negativeObjectAssertion.Subject.PatternMemberID, ontology.Data.GetSameIndividuals((RDFResource)negativeObjectAssertion.Subject));
                List<RDFResource> compatibleSubjects = sameIndividualsCache[negativeObjectAssertion.Subject.PatternMemberID]
                                                        .Union(new List<RDFResource>() { (RDFResource)negativeObjectAssertion.Subject }).ToList();

                //Enlist the object properties which are compatible with negative assertion predicate
                if (!compatiblePropertiesCache.ContainsKey(negativeObjectAssertion.Predicate.PatternMemberID))
                    compatiblePropertiesCache.Add(negativeObjectAssertion.Predicate.PatternMemberID, ontology.Model.PropertyModel.GetEquivalentPropertiesOf((RDFResource)negativeObjectAssertion.Predicate)
                                                                                                        .Union(ontology.Model.PropertyModel.GetSubPropertiesOf((RDFResource)negativeObjectAssertion.Predicate)).ToList());
                List<RDFResource> compatibleProperties = compatiblePropertiesCache[negativeObjectAssertion.Predicate.PatternMemberID]
                                                           .Union(new List<RDFResource>() { (RDFResource)negativeObjectAssertion.Predicate })
                                                            .ToList();

                //Enlist the individuals which are compatible with negative assertion object
                if (!sameIndividualsCache.ContainsKey(negativeObjectAssertion.Object.PatternMemberID))
                    sameIndividualsCache.Add(negativeObjectAssertion.Object.PatternMemberID, ontology.Data.GetSameIndividuals((RDFResource)negativeObjectAssertion.Object));
                List<RDFResource> compatibleObjects = sameIndividualsCache[negativeObjectAssertion.Object.PatternMemberID]
                                                       .Union(new List<RDFResource>() { (RDFResource)negativeObjectAssertion.Object }).ToList();

                //There should not be any object assertion conflicting with negative object assertions
                if (objectAssertions.Any(objAsn => compatibleSubjects.Any(subj => subj.Equals(objAsn.Subject))
                                                     && compatibleProperties.Any(pred => pred.Equals(objAsn.Predicate))
                                                       && compatibleObjects.Any(obj => obj.Equals(objAsn.Object))))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLNegativeAssertionsRule),
                        $"Violation of negative object assertion '{negativeObjectAssertion}'",
                        "Revise your object assertions: there should not be any object assertion conflicting with negative object assertions!"));
            }

            //owl:NegativeDatatypeProperty
            RDFGraph datatypeAssertions = OWLOntologyDataLoader.GetDatatypeAssertions(ontology, ontology.Data.ABoxGraph);
            foreach (RDFTriple negativeDatatypeAssertion in OWLOntologyDataLoader.GetNegativeDatatypeAssertions(ontology.Data.ABoxGraph))
            {
                //Enlist the individuals which are compatible with negative assertion subject
                if (!sameIndividualsCache.ContainsKey(negativeDatatypeAssertion.Subject.PatternMemberID))
                    sameIndividualsCache.Add(negativeDatatypeAssertion.Subject.PatternMemberID, ontology.Data.GetSameIndividuals((RDFResource)negativeDatatypeAssertion.Subject));
                List<RDFResource> compatibleSubjects = sameIndividualsCache[negativeDatatypeAssertion.Subject.PatternMemberID]
                                                        .Union(new List<RDFResource>() { (RDFResource)negativeDatatypeAssertion.Subject }).ToList();

                //Enlist the datatype properties which are compatible with negative assertion predicate
                if (!compatiblePropertiesCache.ContainsKey(negativeDatatypeAssertion.Predicate.PatternMemberID))
                    compatiblePropertiesCache.Add(negativeDatatypeAssertion.Predicate.PatternMemberID, ontology.Model.PropertyModel.GetEquivalentPropertiesOf((RDFResource)negativeDatatypeAssertion.Predicate)
                                                                                                        .Union(ontology.Model.PropertyModel.GetSubPropertiesOf((RDFResource)negativeDatatypeAssertion.Predicate)).ToList());
                List<RDFResource> compatibleProperties = compatiblePropertiesCache[negativeDatatypeAssertion.Predicate.PatternMemberID]
                                                           .Union(new List<RDFResource>() { (RDFResource)negativeDatatypeAssertion.Predicate }).ToList();

                //There should not be any datatype assertion conflicting with negative datatype assertions
                if (datatypeAssertions.Any(dtAsn => compatibleSubjects.Any(subj => subj.Equals(dtAsn.Subject))
                                                      && compatibleProperties.Any(pred => pred.Equals(dtAsn.Predicate))
                                                        && negativeDatatypeAssertion.Object.Equals(dtAsn.Object)))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLNegativeAssertionsRule),
                        $"Violation of negative datatype assertion '{negativeDatatypeAssertion}'",
                        "Revise your datatype assertions: there should not be any datatype assertion conflicting with negative datatype assertions!"));
            }

            return validatorRuleReport;
        }
    }
}