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
using RDFSharp.Query;
using System.Collections.Generic;
using System.Globalization;
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
            RDFGraph aboxObjectAssertions = OWLOntologyDataLoader.GetObjectAssertions(ontology, ontology.Data.ABoxGraph);
            RDFGraph aboxDataAssertions = OWLOntologyDataLoader.GetDatatypeAssertions(ontology, ontology.Data.ABoxGraph);

            //Iterate individuals of the ontology
            IEnumerator<RDFResource> individualsEnumerator = ontology.Data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext())
            {
                //Extract classes assigned to the current individual
                List<RDFResource> individualClasses = classType[individualsEnumerator.Current, null, null, null]
                                                        .Select(t => t.Object)
                                                        .OfType<RDFResource>()
                                                        .ToList();

                //Iterate discovered classes to check for eventual 'owl:disjointWith' or 'owl:complementOf' or local cardinality clashes
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

                    #region Local Cardinality Analysis
                    //There should not be [Q][Exact|Max|MinMax] local cardinality constraints violated by the individual
                    bool proceedWithValidation = default, isQualifiedRestriction = default;
                    RDFResource onProperty = default, onClass = default; 
                    RDFTypedLiteral maxAllowedCardinality = default;
                    if (ontology.Model.ClassModel.CheckHasCardinalityRestrictionClass(individualClass))
                    {
                        proceedWithValidation = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionCardinality(ontology.Model.ClassModel.TBoxGraph, individualClass);
                        onProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, individualClass);
                    }
                    else if (ontology.Model.ClassModel.CheckHasMaxCardinalityRestrictionClass(individualClass)
                              || ontology.Model.ClassModel.CheckHasMinMaxCardinalityRestrictionClass(individualClass))
                    {
                        proceedWithValidation = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionMaxCardinality(ontology.Model.ClassModel.TBoxGraph, individualClass);
                        onProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, individualClass);
                    }                        
                    else if (ontology.Model.ClassModel.CheckHasQualifiedCardinalityRestrictionClass(individualClass))
                    {
                        proceedWithValidation = true;
                        isQualifiedRestriction = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionQualifiedCardinality(ontology.Model.ClassModel.TBoxGraph, individualClass);
                        onProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, individualClass);
                        onClass = OWLOntologyClassModelLoader.GetRestrictionClass(ontology.Model.ClassModel.TBoxGraph, individualClass);
                    }
                    else if (ontology.Model.ClassModel.CheckHasMaxQualifiedCardinalityRestrictionClass(individualClass) 
                              ||ontology.Model.ClassModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(individualClass))
                    {
                        proceedWithValidation = true;
                        isQualifiedRestriction = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionMaxQualifiedCardinality(ontology.Model.ClassModel.TBoxGraph, individualClass);
                        onProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, individualClass);
                        onClass = OWLOntologyClassModelLoader.GetRestrictionClass(ontology.Model.ClassModel.TBoxGraph, individualClass);
                    }

                    //Proceed with validation of the current individual
                    if (proceedWithValidation
                         && onProperty != null
                         && uint.TryParse(maxAllowedCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxAllowedCardinalityValue)
                         && (!isQualifiedRestriction || onClass != null))
                    {
                        //Fetch assertions of the current individual using the restricted property
                        RDFGraph individualAssertions = aboxObjectAssertions[individualsEnumerator.Current, onProperty, null, null]
                                                         .UnionWith(aboxDataAssertions[individualsEnumerator.Current, onProperty, null, null]);
                        
                        //Determine if maximum allowed cardinality is violated or not
                        if (!isQualifiedRestriction)
                        {
                            bool violatesRestriction = default;

                            //Isolate target individuals in order to perform owl:differentFrom analysis (since
                            //under OWA we must take in consideration only *explicitly different* individuals)
                            if (ontology.Model.PropertyModel.CheckHasObjectProperty(onProperty))
                            {
                                List<RDFResource> targetIndividuals = RDFQueryUtilities.RemoveDuplicates(individualAssertions.Select(t => t.Object)
                                                                                                                             .OfType<RDFResource>()
                                                                                                                             .ToList());
                                violatesRestriction = CountDifferentIndividuals(ontology, targetIndividuals, maxAllowedCardinalityValue) >= maxAllowedCardinalityValue;
                            }

                            //Isolate target literals in order to perform inequality analysis
                            else if (ontology.Model.PropertyModel.CheckHasDatatypeProperty(onProperty))
                            {
                                List<RDFLiteral> targetLiterals = RDFQueryUtilities.RemoveDuplicates(individualAssertions.Select(t => t.Object)
                                                                                                                         .OfType<RDFLiteral>()
                                                                                                                         .ToList());
                                violatesRestriction = targetLiterals.Count > maxAllowedCardinalityValue;
                            }

                            //Raise violation error
                            if (violatesRestriction)
                                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                                    nameof(OWLClassTypeRule),
                                    $"Violation of local cardinality constraint on individual '{individualsEnumerator.Current}'",
                                    $"Revise your data: you have a local cardinality constraint (valued {maxAllowedCardinalityValue}) violated by the targets of assertion property {onProperty}"));
                        }                            

                        //Determine if maximum allowed qualified cardinality is violated or not
                        //(we cannot verify qualified cardinalities working on data properties)
                        else
                        {
                            bool violatesQRestriction = default;

                            //Isolate target individuals in order to perform owl:differentFrom analysis (since
                            //under OWA we must take in consideration only *explicitly different* individuals)
                            if (ontology.Model.PropertyModel.CheckHasObjectProperty(onProperty))
                            {
                                List<RDFResource> targetQIndividuals = RDFQueryUtilities.RemoveDuplicates(individualAssertions.Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO
                                                                                                                                            && ontology.Data.CheckIsIndividualOf(ontology.Model, (RDFResource)t.Object, onClass))
                                                                                                                              .Select(t => t.Object)
                                                                                                                              .OfType<RDFResource>()
                                                                                                                              .ToList());
                                violatesQRestriction = CountDifferentIndividuals(ontology, targetQIndividuals, maxAllowedCardinalityValue) >= maxAllowedCardinalityValue;
                            }
                            
                            if (violatesQRestriction)
                                validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                    OWLEnums.OWLValidatorEvidenceCategory.Error,
                                    nameof(OWLClassTypeRule),
                                    $"Violation of local qualified cardinality constraint on individual '{individualsEnumerator.Current}'",
                                    $"Revise your data: you have a local qualified cardinality constraint (valued {maxAllowedCardinalityValue}) violated by the targets of assertion property {onProperty}"));
                        }
                    }
                    #endregion
                }                
            }

            return validatorRuleReport;
        }

        //Counts the distinct pairs of individuals related by owl:differentFrom in the given ontology 
        private static int CountDifferentIndividuals(OWLOntology ontology, List<RDFResource> targetIndividuals, uint maxAllowedCardinalityValue)
        {
            //In case the maximum allowed number of occurrences is zero we can safely exit 
            //by directly reporting the number of target individuals (we are not interested
            //in computing their owl:differentFrom relations)
            if (maxAllowedCardinalityValue == 0)
                return targetIndividuals.Count;

            //Otherwise we must compute the distinct pairs of individuals related by an
            //owl:differentFrom relation. In fact, under OWA this is the only violation
            //trigger: having N>threshold explicitly different individuals as targets of
            //the same [Q][Exact|Max|MinMax] local cardinality constraint 
            int differentTargetIndividuals = 0;
            for (int i = 0; i < targetIndividuals.Count - 1; i++)
                for (int j = i + 1; j < targetIndividuals.Count; j++)
                    if (ontology.Data.CheckIsDifferentIndividual(targetIndividuals[i], targetIndividuals[j]))
                        differentTargetIndividuals++;
            return differentTargetIndividuals;
        }
    }
}