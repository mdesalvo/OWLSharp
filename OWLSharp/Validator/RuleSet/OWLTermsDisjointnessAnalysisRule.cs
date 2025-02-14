/*
  Copyright 2014-2025 Marco De Salvo
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

using OWLSharp.Ontology;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    internal static class OWLTermsDisjointnessAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.TermsDisjointnessAnalysis.ToString();
        internal const string rulesugg = "There should not be terms referring at the same time to classes, datatypes, properties or individuals: although 'punning' is supported, it is recommended to pursue terms disjointness for the sake of decidability";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<string> declaredClasses = ontology.GetDeclaredEntitiesOfType<OWLClass>().Select(cls => cls.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredDatatypes = ontology.GetDeclaredEntitiesOfType<OWLDatatype>().Select(dtt => dtt.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredDataProperties = ontology.GetDeclaredEntitiesOfType<OWLDataProperty>().Select(dtp => dtp.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredObjectProperties = ontology.GetDeclaredEntitiesOfType<OWLObjectProperty>().Select(obp => obp.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredAnnotationProperties = ontology.GetDeclaredEntitiesOfType<OWLAnnotationProperty>().Select(anp => anp.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredNamedIndividuals = ontology.GetDeclaredEntitiesOfType<OWLNamedIndividual>().Select(idv => idv.GetIRI().ToString()).Distinct().ToList();
            
            foreach (string clashingClass in declaredClasses.Where(cls => declaredDatatypes.Contains(cls)
                                                                            || declaredDataProperties.Contains(cls)
                                                                            || declaredObjectProperties.Contains(cls)
                                                                            || declaredAnnotationProperties.Contains(cls)
                                                                            || declaredNamedIndividuals.Contains(cls)))
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning, 
                    rulename, 
                    $"Detected clash on terms disjointness for class with IRI: '{clashingClass}'", 
                    rulesugg));

            foreach (string clashingDatatype in declaredDatatypes.Where(dtt => declaredDataProperties.Contains(dtt)
                                                                                || declaredObjectProperties.Contains(dtt)
                                                                                || declaredAnnotationProperties.Contains(dtt)
                                                                                || declaredNamedIndividuals.Contains(dtt)))
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning, 
                    rulename, 
                    $"Detected clash on terms disjointness for datatype with IRI: '{clashingDatatype}'", 
                    rulesugg));

            foreach (string clashingDataProperty in declaredDataProperties.Where(dtp => declaredObjectProperties.Contains(dtp)
                                                                                         || declaredAnnotationProperties.Contains(dtp)
                                                                                         || declaredNamedIndividuals.Contains(dtp)))
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning, 
                    rulename, 
                    $"Detected clash on terms disjointness for data property with IRI: '{clashingDataProperty}'", 
                    rulesugg));

            foreach (string clashingObjectProperty in declaredObjectProperties.Where(obp => declaredAnnotationProperties.Contains(obp)
                                                                                              || declaredNamedIndividuals.Contains(obp)))
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning, 
                    rulename, 
                    $"Detected clash on terms disjointness for object property with IRI: '{clashingObjectProperty}'", 
                    rulesugg));

            foreach (string clashingAnnotationProperty in declaredAnnotationProperties.Where(anp => declaredNamedIndividuals.Contains(anp)))
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning, 
                    rulename, 
                    $"Detected clash on terms disjointness for annotation property with IRI: '{clashingAnnotationProperty}'", 
                    rulesugg));

            return issues;
        }
    }
}