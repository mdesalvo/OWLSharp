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

namespace OWLSharp.Validator.Rules
{
    internal static class OWLDisjointClassesAnalysisRule
    {
        internal static readonly string rulename = OWLEnums.OWLValidatorRules.DisjointClassesAnalysis.ToString();
		internal static readonly string rulesugg = "There should not be class expressions belonging at the same time to DisjointClasses and SubClassOf/EquivalentClasses axioms!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS1,CLS2) -> ERROR
			//DisjointClasses(CLS1,CLS2) ^ SubClassOf(CLS2,CLS1) -> ERROR
			//DisjointClasses(CLS1,CLS2) ^ EquivalentClasses(CLS1,CLS2) -> ERROR
            foreach (OWLDisjointClasses disjClasses in ontology.GetClassAxiomsOfType<OWLDisjointClasses>())
				if (disjClasses.ClassExpressions.Any(outerClass => 
					  disjClasses.ClassExpressions.Any(innerClass => !outerClass.GetIRI().Equals(innerClass.GetIRI())
					  													&& (ontology.CheckIsSubClassOf(outerClass, innerClass)
																			 || ontology.CheckIsSubClassOf(innerClass, outerClass)
																			 || ontology.CheckAreEquivalentClasses(outerClass, innerClass)))))
					issues.Add(new OWLIssue(
						OWLEnums.OWLIssueSeverity.Error, 
						rulename, 
						$"Violated DisjointClasses axiom with signature: '{disjClasses.GetXML()}'", 
						rulesugg));

            return issues;
        }
    }
}