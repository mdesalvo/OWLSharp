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

using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using OWLSharp.Validator;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OWLSharp.Extensions.SKOS.Validator.RuleSet
{
    internal class SKOSAlternativeLabelAnalysisRule
    {
        internal static readonly string rulename = SKOSEnums.SKOSValidatorRules.AlternativeLabelAnalysis.ToString();
		internal static readonly string rulesugg1 = "There should not be SKOS concepts having the same annotation value for altLabel, prefLabel, hiddenLabel properties.";
		internal static readonly string rulesugg2 = "There should not be SKOS-XL concepts having the same annotation value for altLabel, prefLabel, hiddenLabel properties";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Temporary working variables (SKOS)
            List<OWLAnnotationAssertion> annAsns = ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>();
            List<OWLAnnotationAssertion> altLabels = annAsns.Where(annAsn => annAsn.AnnotationProperty.GetIRI().Equals(RDFVocabulary.SKOS.ALT_LABEL)).ToList();
            List<OWLAnnotationAssertion> prefLabels = annAsns.Where(annAsn => annAsn.AnnotationProperty.GetIRI().Equals(RDFVocabulary.SKOS.PREF_LABEL)).ToList();
            List<OWLAnnotationAssertion> hiddenLabels = annAsns.Where(annAsn => annAsn.AnnotationProperty.GetIRI().Equals(RDFVocabulary.SKOS.HIDDEN_LABEL)).ToList();

            //Temporary working variables (SKOS-XL)
            List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            #region Calibration
            OWLIndividualExpression swapIdvExpr;
            for (int i = 0; i < opAsns.Count; i++)
            {
                //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                if (opAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                {
                    swapIdvExpr = opAsns[i].SourceIndividualExpression;
                    opAsns[i].SourceIndividualExpression = opAsns[i].TargetIndividualExpression;
                    opAsns[i].TargetIndividualExpression = swapIdvExpr;
                    opAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                }
            }
            opAsns = OWLAxiomHelper.RemoveDuplicates(opAsns);
            #endregion
            List<OWLObjectPropertyAssertion> altXLabels = opAsns.Where(opAsn => opAsn.ObjectPropertyExpression.GetIRI().Equals(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL)).ToList();
            List<OWLObjectPropertyAssertion> prefXLabels = opAsns.Where(opAsn => opAsn.ObjectPropertyExpression.GetIRI().Equals(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL)).ToList();
            List<OWLObjectPropertyAssertion> hiddenXLabels = opAsns.Where(opAsn => opAsn.ObjectPropertyExpression.GetIRI().Equals(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL)).ToList();
            List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            List<OWLDataPropertyAssertion> litFormLabels = dpAsns.Where(dpAsn => dpAsn.DataProperty.GetIRI().Equals(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM)).ToList();
            
            foreach (OWLIndividualExpression concept in ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.SKOS.CONCEPT), false))
            {
                string conceptIRI = concept.GetIRI().ToString();

                //skos:altLabel
                foreach (OWLAnnotationAssertion altLabel in altLabels.Where(al => string.Equals(al.SubjectIRI, conceptIRI)
                                                                                   && al.ValueIRI == null 
                                                                                   && al.ValueLiteral?.GetLiteral() is RDFPlainLiteral))
                {
                    //Clash with skos:prefLabel
                    if (prefLabels.Any(pl => string.Equals(pl.SubjectIRI, conceptIRI)
                                              && pl.ValueIRI == null 
                                              && pl.ValueLiteral?.GetLiteral() is RDFPlainLiteral prefLabelValue
                                              && prefLabelValue.Equals(altLabel.ValueLiteral.GetLiteral())))
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error, 
                            rulename, 
                            $"Violated skos:altLabel with signature: '{altLabel.GetXML()}'", 
                            rulesugg1));

                    //Clash with skos:hiddenLabel
                    if (hiddenLabels.Any(hl => string.Equals(hl.SubjectIRI, conceptIRI)
                                                && hl.ValueIRI == null 
                                                && hl.ValueLiteral?.GetLiteral() is RDFPlainLiteral hiddenLabelValue
                                                && hiddenLabelValue.Equals(altLabel.ValueLiteral.GetLiteral())))
                        issues.Add(new OWLIssue(
                            OWLEnums.OWLIssueSeverity.Error, 
                            rulename, 
                            $"Violated skos:altLabel with signature: '{altLabel.GetXML()}'", 
                            rulesugg1));
                }                    

                //skosxl:altLabel
                foreach (OWLObjectPropertyAssertion altXLabel in altXLabels.Where(al => string.Equals(al.SourceIndividualExpression.GetIRI().ToString(), conceptIRI)))
                    foreach (OWLDataPropertyAssertion altXLabelLitForm in litFormLabels.Where(dpAsn => dpAsn.IndividualExpression.GetIRI().Equals(altXLabel.TargetIndividualExpression.GetIRI())))
                    {
                        //Clash with skosxl:prefLabel
                        if (prefXLabels.Any(pl => string.Equals(pl.SourceIndividualExpression.GetIRI().ToString(), conceptIRI)
                                                   && litFormLabels.Any(litFormLabel => litFormLabel.IndividualExpression.GetIRI().Equals(pl.TargetIndividualExpression.GetIRI())
                                                                                         && litFormLabel.Literal.GetLiteral().Equals(altXLabelLitForm.Literal.GetLiteral()))))
                            issues.Add(new OWLIssue(
                                OWLEnums.OWLIssueSeverity.Error, 
                                rulename, 
                                $"Violated skosxl:altLabel with signature: '{altXLabel.GetXML()}'", 
                                rulesugg2));

                        //Clash with skosxl:hiddenLabel
                        if (hiddenXLabels.Any(hl => string.Equals(hl.SourceIndividualExpression.GetIRI().ToString(), conceptIRI)
                                                     && litFormLabels.Any(litFormLabel => litFormLabel.IndividualExpression.GetIRI().Equals(hl.TargetIndividualExpression.GetIRI())
                                                                                           && litFormLabel.Literal.GetLiteral().Equals(altXLabelLitForm.Literal.GetLiteral()))))
                            issues.Add(new OWLIssue(
                                OWLEnums.OWLIssueSeverity.Error, 
                                rulename, 
                                $"Violated skosxl:altLabel with signature: '{altXLabel.GetXML()}'", 
                                rulesugg2));
                    }
            }

            return issues;
        }
    }
}