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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator.RuleSet
{
	internal static class OWLSubClassOfAnalysisRule
	{
		internal static readonly string rulename = OWLEnums.OWLValidatorRules.SubClassOfAnalysis.ToString();
		internal static readonly string rulesugg1 = "There should not be class expressions belonging at the same time to SubClassOf and EquivalentClasses/DisjointClasses axioms!";
		internal static readonly string rulesugg2 = "There should not be individuals violating ObjectExactCardinality or ObjectMaxCardinality constraints!";
		internal static readonly string rulesugg3 = "There should not be individuals violating DataExactCardinality or DataMaxCardinality constraints!";

		internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
		{
			#region Utilities
            List<OWLObjectPropertyAssertion> CalibrateObjectAssertions(List<OWLObjectPropertyAssertion> objectPropertyAssertions)
            {
                OWLIndividualExpression swapIdvExpr;
                for (int i = 0; i < objectPropertyAssertions.Count; i++)
                    if (objectPropertyAssertions[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {
                        swapIdvExpr = objectPropertyAssertions[i].SourceIndividualExpression;
                        objectPropertyAssertions[i].SourceIndividualExpression = objectPropertyAssertions[i].TargetIndividualExpression;
                        objectPropertyAssertions[i].TargetIndividualExpression = swapIdvExpr;
                        objectPropertyAssertions[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }
                return OWLAxiomHelper.RemoveDuplicates(objectPropertyAssertions);
            }
            #endregion

			List<OWLIssue> issues = new List<OWLIssue>();

			//Temporary working variables
			List<OWLObjectPropertyAssertion> opAsns = CalibrateObjectAssertions(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>());
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
			Dictionary<string, List<OWLIndividualExpression>> individualsCache = new Dictionary<string, List<OWLIndividualExpression>>();

			foreach (OWLSubClassOf subClassOf in ontology.GetClassAxiomsOfType<OWLSubClassOf>())
			{
				//SubClassOf(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
				//SubClassOf(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> ERROR
				//SubClassOf(CLS1,CLS2) ^ DisjointClasses(CLS1,CLS2) -> ERROR
				if (ontology.CheckIsSubClassOf(subClassOf.SuperClassExpression, subClassOf.SubClassExpression)
					 || ontology.CheckAreEquivalentClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression)
					 || ontology.CheckAreDisjointClasses(subClassOf.SubClassExpression, subClassOf.SuperClassExpression))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
						rulesugg1));

				//SubClassOf(CLS,ObjectExactCardinality(OP,N)
				//SubClassOf(CLS,DataExactCardinality(DP,N)
				//SubClassOf(CLS,ObjectMaxCardinality(OP,N)
				//SubClassOf(CLS,DataMaxCardinality(DP,N)
				if (subClassOf.SuperClassExpression is OWLObjectExactCardinality
					 || subClassOf.SuperClassExpression is OWLObjectMaxCardinality
					 || subClassOf.SuperClassExpression is OWLDataExactCardinality
					 || subClassOf.SuperClassExpression is OWLDataMaxCardinality)
				{
					//Materialize individuals of the subclass
					string subClassIRI = subClassOf.SubClassExpression.GetIRI().ToString();
					if (!individualsCache.ContainsKey(subClassIRI))
						individualsCache.Add(subClassIRI, ontology.GetIndividualsOf(subClassOf.SubClassExpression, false));
					
					//Filter assertions of the current individual, depending on the nature of the superclass
					foreach (OWLIndividualExpression individual in individualsCache[subClassIRI])
					{
						RDFResource individualIRI = individual.GetIRI();
						if (subClassOf.SuperClassExpression is OWLObjectExactCardinality objExactCardinality)
						{
							int asnsCount = opAsns.Count(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(individualIRI)
																	&& opAsn.ObjectPropertyExpression.GetIRI().Equals(objExactCardinality.ObjectPropertyExpression.GetIRI()));
							if (asnsCount > int.Parse(objExactCardinality.Cardinality))
								issues.Add(new OWLIssue(
									OWLEnums.OWLIssueSeverity.Error, 
									rulename, 
									$"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
									rulesugg2));
						}
						else if (subClassOf.SuperClassExpression is OWLObjectMaxCardinality objMaxCardinality)
						{
							int asnsCount = opAsns.Count(opAsn => opAsn.SourceIndividualExpression.GetIRI().Equals(individualIRI)
																	&& opAsn.ObjectPropertyExpression.GetIRI().Equals(objMaxCardinality.ObjectPropertyExpression.GetIRI()));
							if (asnsCount > int.Parse(objMaxCardinality.Cardinality))
								issues.Add(new OWLIssue(
									OWLEnums.OWLIssueSeverity.Error, 
									rulename, 
									$"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
									rulesugg2));
						}
						else if (subClassOf.SuperClassExpression is OWLDataExactCardinality dtExactCardinality)
						{
							int asnsCount = dpAsns.Count(dpAsn => dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
																	&& dpAsn.DataProperty.GetIRI().Equals(dtExactCardinality.DataProperty.GetIRI()));
							if (asnsCount > int.Parse(dtExactCardinality.Cardinality))
								issues.Add(new OWLIssue(
									OWLEnums.OWLIssueSeverity.Error, 
									rulename, 
									$"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
									rulesugg3));
						}
						else if (subClassOf.SuperClassExpression is OWLDataMaxCardinality dtMaxCardinality)
						{
							int asnsCount = dpAsns.Count(dpAsn => dpAsn.IndividualExpression.GetIRI().Equals(individualIRI)
																	&& dpAsn.DataProperty.GetIRI().Equals(dtMaxCardinality.DataProperty.GetIRI()));
							if (asnsCount > int.Parse(dtMaxCardinality.Cardinality))
								issues.Add(new OWLIssue(
									OWLEnums.OWLIssueSeverity.Error, 
									rulename, 
									$"Violated SubClassOf axiom with signature: '{subClassOf.GetXML()}'", 
									rulesugg3));
						}
					}
				}
			}

			return issues;
		}
	}
}