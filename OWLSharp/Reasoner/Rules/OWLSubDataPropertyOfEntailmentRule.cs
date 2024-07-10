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
    internal static class OWLSubDataPropertyOfEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			List<OWLSubDataPropertyOf> subDtPropOfAxs = ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>();
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();

            foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                     .Select(ax => (OWLDataProperty)ax.Expression))
			{
				//SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
            	//SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)
				List<OWLDataProperty> superDataProperties = ontology.GetSuperDataPropertiesOf(declaredDataProperty);
                foreach (OWLDataProperty superDataProperty in superDataProperties)
                    inferences.Add(new OWLSubDataPropertyOf(declaredDataProperty, superDataProperty) { IsInference=true });

				//SubDataPropertyOf(P1,P2) ^ DataPropertyAssertion(P1,I,LIT) -> DataPropertyAssertion(P2,I,LIT)
				foreach (OWLDataPropertyAssertion declaredDataPropertyAsn in OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, declaredDataProperty))
					foreach (OWLDataProperty superDataProperty in superDataProperties)
						inferences.Add(new OWLDataPropertyAssertion(superDataProperty, declaredDataPropertyAsn.Literal) { IndividualExpression=declaredDataPropertyAsn.IndividualExpression, IsInference=true });
			}
			//Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => subDtPropOfAxs.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));
			inferences.RemoveAll(inf => dpAsns.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }
    }
}