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
    /// OWL-DL validator rule checking for consistency of rdf:type relations characterizing individuals
    /// </summary>
    internal static class OWLClassTypeRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //This rule requires a couple of caches for performance reasons
            Dictionary<long, HashSet<long>> disjointWithCache = new Dictionary<long, HashSet<long>>();
            RDFGraph classType = ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, null, null];
            RDFGraph complementOf = ontology.Model.ClassModel.TBoxGraph[null, RDFVocabulary.OWL.COMPLEMENT_OF, null, null];
            
            //Iterate individuals of the ontology
            IEnumerator<RDFResource> individualsEnumerator = ontology.Data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext())
            {
                //Extract classes assigned to the current individual
                List<RDFResource> individualClasses = classType[individualsEnumerator.Current, null, null, null]
                                                        .Select(t => t.Object)
                                                        .OfType<RDFResource>()
                                                        .ToList();

                //Iterate discovered classes to check for eventual 'owl:disjointWith' or 'owl:complementOf' clashes
                foreach (RDFResource individualClass in individualClasses)
                {
                    #region Complement Analysis
                    //There should not be complement classes assigned as class types of the same individual
                    List<RDFResource> complementClasses = complementOf[individualClass, null, null, null]
                                                            .Select(t => t.Object)
                                                            .OfType<RDFResource>()
                                                            .ToList();
                    if (individualClasses.Any(idvClass => complementClasses.Any(cclClass => cclClass.Equals(idvClass))))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLClassTypeRule),
                            $"Violation of 'rdf:type' relations on individual '{individualsEnumerator.Current}'",
                            "Revise your class model: you have complement classes to which this individual belongs at the same time!"));
                    #endregion

                    #region Disjoint Analysis
                    //Calculate disjoint classes of the current class
                    if (!disjointWithCache.ContainsKey(individualClass.PatternMemberID))
                        disjointWithCache.Add(individualClass.PatternMemberID, new HashSet<long>(ontology.Model.ClassModel.GetDisjointClassesWith(individualClass).Select(cls => cls.PatternMemberID)));

                    //There should not be disjoint classes assigned as class types of the same individual
                    if (individualClasses.Any(idvClass => !idvClass.Equals(individualClass) && disjointWithCache[individualClass.PatternMemberID].Contains(idvClass.PatternMemberID)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLClassTypeRule),
                            $"Violation of 'rdf:type' relations on individual '{individualsEnumerator.Current}'",
                            "Revise your class model: you have disjoint classes to which this individual belongs at the same time!"));
                    #endregion
                }                
            }

            return validatorRuleReport;
        }
    }
}