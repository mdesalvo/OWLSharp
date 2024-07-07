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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLEquivalentObjectPropertiesEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			OWLIndividualExpression swapIdvExpr;
			List<OWLEquivalentObjectProperties> equivObjPropsAxs = ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>();
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            foreach (OWLObjectProperty declaredObjectProperty in ontology.GetDeclarationAxiomsOfType<OWLObjectProperty>()
            															 .Select(ax => (OWLObjectProperty)ax.Expression))
			{
				//EquivalentObjectProperties(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> EquivalentObjectProperties(P1,P3)
				List<OWLObjectPropertyExpression> equivObjectPropertyExprs = ontology.GetEquivalentObjectProperties(declaredObjectProperty);
                foreach (OWLObjectProperty equivalentObjectProperty in equivObjectPropertyExprs.OfType<OWLObjectProperty>())
                    inferences.Add(new OWLEquivalentObjectProperties(new List<OWLObjectPropertyExpression>() { declaredObjectProperty, equivalentObjectProperty }) { IsInference=true });
                foreach (OWLObjectInverseOf equivalentObjectInverseOf in equivObjectPropertyExprs.OfType<OWLObjectInverseOf>())
                    inferences.Add(new OWLEquivalentObjectProperties(new List<OWLObjectPropertyExpression>() { declaredObjectProperty, equivalentObjectInverseOf }) { IsInference=true });
			
				//EquivalentObjectProperties(P1,P2) ^ ObjectPropertyAssertion(P1,I1,I2) -> ObjectPropertyAssertion(P2,I1,I2)
				List<OWLObjectPropertyAssertion> declaredObjectPropertyAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, declaredObjectProperty);
				for (int i = 0; i < declaredObjectPropertyAsns.Count; i++)
                    if (declaredObjectPropertyAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                    {   
                        swapIdvExpr = declaredObjectPropertyAsns[i].SourceIndividualExpression;
                        declaredObjectPropertyAsns[i].SourceIndividualExpression = declaredObjectPropertyAsns[i].TargetIndividualExpression;
                        declaredObjectPropertyAsns[i].TargetIndividualExpression = swapIdvExpr;
                        declaredObjectPropertyAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                    }
				foreach (OWLObjectPropertyAssertion declaredObjectPropertyAsn in OWLAxiomHelper.RemoveDuplicates(declaredObjectPropertyAsns))
					foreach (OWLObjectPropertyExpression equivObjectPropertyExpr in equivObjectPropertyExprs)
						inferences.Add(equivObjectPropertyExpr is OWLObjectInverseOf objInvOf 
							? new OWLObjectPropertyAssertion(objInvOf.ObjectProperty, declaredObjectPropertyAsn.TargetIndividualExpression, declaredObjectPropertyAsn.SourceIndividualExpression) { IsInference=true }
							: new OWLObjectPropertyAssertion(equivObjectPropertyExpr, declaredObjectPropertyAsn.SourceIndividualExpression, declaredObjectPropertyAsn.TargetIndividualExpression) { IsInference=true });
			}
			//Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => equivObjPropsAxs.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));
			inferences.RemoveAll(inf => opAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return inferences;
        }
    }
}