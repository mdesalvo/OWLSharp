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

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLInverseObjectPropertiesEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
			List<OWLInverseObjectProperties> invObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();

            //InverseObjectProperties(OP,IOP) ^ ObjectPropertyAssertion(OP,IDV1,IDV2) -> ObjectPropertyAssertion(IOP,IDV2,IDV1)
            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
                                                                     	 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				RDFResource declaredObjectPropertyIRI = declaredObjectProperty.GetIRI();

				//Extract inverse object properties of the current object property
				List<OWLObjectPropertyExpression> invsOfDeclaredObjectProperty = invObjProps.Where(ax => ax.LeftObjectPropertyExpression.GetIRI().Equals(declaredObjectPropertyIRI))
																						    .Select(ax => ax.RightObjectPropertyExpression)
																						    .Union(invObjProps.Where(ax => ax.RightObjectPropertyExpression.GetIRI().Equals(declaredObjectPropertyIRI))
																						   					  .Select(ax => ax.LeftObjectPropertyExpression)).ToList();
				invsOfDeclaredObjectProperty.RemoveAll(opex => opex.GetIRI().Equals(declaredObjectPropertyIRI));

				//Extract object assertions with the current object property
				foreach (OWLObjectPropertyAssertion opAsn in OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, declaredObjectProperty))
				{
					OWLIndividualExpression opAsnSourceIdvExpr = opAsn.SourceIndividualExpression;
                    OWLIndividualExpression opAsnTargetIdvExpr = opAsn.TargetIndividualExpression;

					//In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                    if (opAsn.ObjectPropertyExpression is OWLObjectInverseOf)
                    {
                        opAsnSourceIdvExpr = opAsn.TargetIndividualExpression;
                        opAsnTargetIdvExpr = opAsn.SourceIndividualExpression;
                    }

					//Iterate inverse object properties of the current object property in order to materialize the "swapped-assertion" inference
					foreach (OWLObjectPropertyExpression invOfDeclaredObjectProperty in invsOfDeclaredObjectProperty)
						inferences.Add(new OWLObjectPropertyAssertion(invOfDeclaredObjectProperty, opAsnTargetIdvExpr, opAsnSourceIdvExpr) { IsInference=true });
				}
			}

            return inferences;
        }
    }
}