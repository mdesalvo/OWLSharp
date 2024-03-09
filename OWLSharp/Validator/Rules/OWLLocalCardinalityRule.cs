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
    /// OWL-DL validator rule checking for consistency of local cardinality constraints
    /// </summary>
    internal static class OWLLocalCardinalityRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();
            RDFGraph classType = ontology.Data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, null, null];
            RDFGraph aboxObjectAssertions = OWLOntologyDataLoader.GetObjectAssertions(ontology, ontology.Data.ABoxGraph);
            RDFGraph aboxDataAssertions = OWLOntologyDataLoader.GetDatatypeAssertions(ontology, ontology.Data.ABoxGraph);

            //owl:Restriction
            IEnumerator<RDFResource> restrictionsEnumerator = ontology.Model.ClassModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                if (ontology.Model.ClassModel.CheckHasLocalCardinalityRestrictionClass(restrictionsEnumerator.Current)
                     || ontology.Model.ClassModel.CheckHasLocalQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current))
                {
                    RDFResource onProperty = OWLOntologyClassModelLoader.GetRestrictionProperty(ontology.Model.ClassModel.TBoxGraph, restrictionsEnumerator.Current);
                    RDFResource onClass = OWLOntologyClassModelLoader.GetRestrictionClass(ontology.Model.ClassModel.TBoxGraph, restrictionsEnumerator.Current);

                    #region Transitivity Analysis
                    //There should not be cardinality restrictions working on a transitive property, or any super properties or inverse properties being transitive
                    if (ontology.Model.PropertyModel.CheckHasTransitiveProperty(onProperty))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLLocalCardinalityRule),
                            $"Violation of OWL-DL integrity caused by 'owl:TransitiveProperty' used as 'owl:onProperty' of cardinality restriction '{restrictionsEnumerator.Current}'",
                            "Revise your class model: it is not allowed the direct use of transitive properties on cardinality restrictions"));
                    else
                    {
                        List<RDFResource> superProperties = ontology.Model.PropertyModel.GetSuperPropertiesOf(onProperty);
                        if (superProperties.Any(sp => ontology.Model.PropertyModel.CheckHasTransitiveProperty(sp)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLLocalCardinalityRule),
                                $"Violation of OWL-DL integrity caused by 'owl:TransitiveProperty' being super property of 'owl:onProperty' of cardinality restriction '{restrictionsEnumerator.Current}'",
                                "Revise your class model: it is not allowed the indirect use of transitive properties on cardinality restrictions"));

                        //Grab its inverse properties to check for presence of any transitive properties
                        List<RDFResource> inverseProperties = ontology.Model.PropertyModel.GetInversePropertiesOf(onProperty);
                        if (inverseProperties.Any(ip => ontology.Model.PropertyModel.CheckHasTransitiveProperty(ip)))
                            validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                                OWLEnums.OWLValidatorEvidenceCategory.Error,
                                nameof(OWLLocalCardinalityRule),
                                $"Violation of OWL-DL integrity caused by 'owl:TransitiveProperty' being inverse property of 'owl:onProperty' of cardinality restriction '{restrictionsEnumerator.Current}'",
                                "Revise your class model: it is not allowed the indirect use of transitive properties on cardinality restrictions"));
                    }
                    #endregion

                    #region Local Cardinality Analysis
                    //There should not be [Q][Exact|Max|MinMax] local cardinality constraints violated by the individual
                    bool proceedWithValidation = default, isQualifiedRestriction = default;
                    RDFTypedLiteral maxAllowedCardinality = default;
                    if (ontology.Model.ClassModel.CheckHasCardinalityRestrictionClass(restrictionsEnumerator.Current))
                    {
                        proceedWithValidation = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionCardinality(ontology.Model.ClassModel.TBoxGraph, restrictionsEnumerator.Current);
                    }
                    else if (ontology.Model.ClassModel.CheckHasMaxCardinalityRestrictionClass(restrictionsEnumerator.Current)
                              || ontology.Model.ClassModel.CheckHasMinMaxCardinalityRestrictionClass(restrictionsEnumerator.Current))
                    {
                        proceedWithValidation = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionMaxCardinality(ontology.Model.ClassModel.TBoxGraph, restrictionsEnumerator.Current);
                    }                        
                    else if (ontology.Model.ClassModel.CheckHasQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current))
                    {
                        proceedWithValidation = true;
                        isQualifiedRestriction = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionQualifiedCardinality(ontology.Model.ClassModel.TBoxGraph, restrictionsEnumerator.Current);
                    }
                    else if (ontology.Model.ClassModel.CheckHasMaxQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current) 
                              ||ontology.Model.ClassModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(restrictionsEnumerator.Current))
                    {
                        proceedWithValidation = true;
                        isQualifiedRestriction = true;
                        maxAllowedCardinality = OWLOntologyClassModelLoader.GetRestrictionMaxQualifiedCardinality(ontology.Model.ClassModel.TBoxGraph, restrictionsEnumerator.Current);
                    }

                    if (proceedWithValidation
                        && onProperty != null
                        && uint.TryParse(maxAllowedCardinality.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxAllowedCardinalityValue)
                        && (!isQualifiedRestriction || onClass != null))
                    {
                        //Fetch individuals explicitly assigned to the current local cardinality constraints
                        foreach(RDFResource restrictionIndividual in  classType[null, null, restrictionsEnumerator.Current, null]
                                                                        .Select(t => t.Subject)
                                                                        .OfType<RDFResource>())
                        {
                            //Fetch assertions of the current individual on the restricted property
                            RDFGraph individualAssertions = aboxObjectAssertions[restrictionIndividual, onProperty, null, null]
                                                             .UnionWith(aboxDataAssertions[restrictionIndividual, onProperty, null, null]);
                            
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
                                        $"Violation of local cardinality constraint on individual '{restrictionIndividual}'",
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
                                        $"Violation of local qualified cardinality constraint on individual '{restrictionIndividual}'",
                                        $"Revise your data: you have a local qualified cardinality constraint (valued {maxAllowedCardinalityValue}) violated by the targets of assertion property {onProperty}"));
                            }
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