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
    internal static class OWLDisjointDataPropertiesEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //Temporary working variables
            List<OWLDisjointDataProperties> dsjDataProps = ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>();

            //EquivalentDataProperties(P1,P2) ^ DisjointDataProperties(P2,P3) -> DisjointDataProperties(P1,P3)
            //SubDataPropertyOf(P1,P2) ^ DisjointDataProperties(P2,P3) -> DisjointClasses(P1,P3)
            foreach (OWLDataProperty declaredDataProperty in ontology.GetDeclarationAxiomsOfType<OWLDataProperty>()
                                                                     .Select(ax => (OWLDataProperty)ax.Expression))
			{
				List<OWLDataProperty> disjointDataProperties = ontology.GetDisjointDataProperties(declaredDataProperty);
                foreach (OWLDataProperty disjointDataProperty in disjointDataProperties)
                    inferences.Add(new OWLDisjointDataProperties(new List<OWLDataProperty>() { declaredDataProperty, disjointDataProperty }) { IsInference=true });
			}
            //Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => dsjDataProps.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return inferences;
        }
    }
}