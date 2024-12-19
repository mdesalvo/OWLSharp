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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    public static class SKOSHelper
    {
        #region Methods
        public static bool CheckHasBroaderConcept(this OWLOntology ontology, RDFResource childConcept, RDFResource parentConcept)
            => childConcept != null && parentConcept != null && ontology != null && ontology.GetBroaderConcepts(childConcept).Any(concept => concept.Equals(parentConcept));

        public static List<RDFResource> GetBroaderConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> broaderConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosBroaderAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.BROADER));
                List<OWLObjectPropertyAssertion> skosBroaderTransitiveAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE));

                //skos:broader
                foreach (OWLObjectPropertyAssertion skosBroaderAsn in skosBroaderAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                    broaderConcepts.Add(skosBroaderAsn.TargetIndividualExpression.GetIRI());

                //skos:broaderTransitive
                broaderConcepts.AddRange(ontology.SubsumeBroaderConceptTransitivity(skosConcept, skosBroaderTransitiveAsns, new Dictionary<long, RDFResource>()));
            }

            return broaderConcepts;
        }
        internal static List<RDFResource> SubsumeBroaderConceptTransitivity(this OWLOntology ontology, RDFResource skosConcept, List<OWLObjectPropertyAssertion> skosBroaderTransitiveAsns, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> broaderTransitiveConcepts = new List<RDFResource>();

            #region visitContext
            if (!visitContext.ContainsKey(skosConcept.PatternMemberID))
                visitContext.Add(skosConcept.PatternMemberID, skosConcept);
            else
                return broaderTransitiveConcepts;
            #endregion

            //skos:broaderTransitive
            foreach (OWLObjectPropertyAssertion skosBroaderTransitiveAsn in skosBroaderTransitiveAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                broaderTransitiveConcepts.Add(skosBroaderTransitiveAsn.TargetIndividualExpression.GetIRI());

            foreach (RDFResource broaderTransitiveConcept in broaderTransitiveConcepts.ToList())
                broaderTransitiveConcepts.AddRange(ontology.SubsumeBroaderConceptTransitivity(broaderTransitiveConcept, skosBroaderTransitiveAsns, visitContext));

            return broaderTransitiveConcepts;
        }
        #endregion

        #region Utilities
        internal static List<OWLObjectPropertyAssertion> CalibrateObjectAssertions(OWLOntology ontology)
        {
            List<OWLObjectPropertyAssertion> objPropAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();

            OWLIndividualExpression swapIdvExpr;
            for (int i = 0; i < objPropAsns.Count; i++)
                if (objPropAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                {
                    swapIdvExpr = objPropAsns[i].SourceIndividualExpression;
                    objPropAsns[i].SourceIndividualExpression = objPropAsns[i].TargetIndividualExpression;
                    objPropAsns[i].TargetIndividualExpression = swapIdvExpr;
                    objPropAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                }
            return OWLAxiomHelper.RemoveDuplicates(objPropAsns);
        }
        #endregion
    }
}