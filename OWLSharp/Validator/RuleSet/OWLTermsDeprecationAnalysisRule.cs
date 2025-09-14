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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    internal static class OWLTermsDeprecationAnalysisRule
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.TermsDeprecationAnalysis);
        internal const string rulesugg = "There should not be presence of deprecated classes, datatypes and properties: it is recommended to migrate ontology to newer term definitions if available";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology, OWLValidatorContext validatorContext)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables
            List<string> declaredClasses = ontology.GetDeclaredEntitiesOfType<OWLClass>().Select(cls => cls.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredDatatypes = ontology.GetDeclaredEntitiesOfType<OWLDatatype>().Select(dtt => dtt.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredDataProperties = ontology.GetDeclaredEntitiesOfType<OWLDataProperty>().Select(dtp => dtp.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredObjectProperties = ontology.GetDeclaredEntitiesOfType<OWLObjectProperty>().Select(obp => obp.GetIRI().ToString()).Distinct().ToList();
            List<string> declaredAnnotationProperties = ontology.GetDeclaredEntitiesOfType<OWLAnnotationProperty>().Select(anp => anp.GetIRI().ToString()).Distinct().ToList();

            foreach (OWLAnnotationAssertion annAsn in ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>()
                                                              .Where(asn => asn.AnnotationProperty.GetIRI().Equals(RDFVocabulary.OWL.DEPRECATED)
                                                                             && (asn.ValueLiteral?.GetLiteral().Equals(RDFTypedLiteral.True) ?? false)))
            {
                if (declaredClasses.Any(cls => string.Equals(cls, annAsn.SubjectIRI)))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Detected presence of deprecated class with IRI: '{annAsn.SubjectIRI}'",
                        rulesugg));

                if (declaredDatatypes.Any(dtt => string.Equals(dtt, annAsn.SubjectIRI)))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Detected presence of deprecated datatype with IRI: '{annAsn.SubjectIRI}'",
                        rulesugg));

                if (declaredDataProperties.Any(dtp => string.Equals(dtp, annAsn.SubjectIRI)))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Detected presence of deprecated data property with IRI: '{annAsn.SubjectIRI}'",
                        rulesugg));

                if (declaredObjectProperties.Any(obp => string.Equals(obp, annAsn.SubjectIRI)))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Detected presence of deprecated object property with IRI: '{annAsn.SubjectIRI}'",
                        rulesugg));

                if (declaredAnnotationProperties.Any(anp => string.Equals(anp, annAsn.SubjectIRI)))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Warning,
                        rulename,
                        $"Detected presence of deprecated annotation property with IRI: '{annAsn.SubjectIRI}'",
                        rulesugg));
            }

            return issues;
        }
    }
}