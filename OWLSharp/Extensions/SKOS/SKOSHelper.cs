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
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    public static class SKOSHelper
    {
        #region Methods
        public static bool CheckHasConcept(this OWLOntology ontology, RDFResource conceptScheme, RDFResource concept)
            => conceptScheme != null && concept != null && ontology != null && ontology.GetConceptsInScheme(conceptScheme).Any(c => c.Equals(concept));
        public static List<RDFResource> GetConceptsInScheme(this OWLOntology ontology, RDFResource skosConceptScheme)
        {
            List<RDFResource> conceptsInScheme = new List<RDFResource>();

            if (skosConceptScheme != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosInSchemeAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.IN_SCHEME));
                List<OWLObjectPropertyAssertion> skosHasTopConceptAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT));
                List<OWLObjectPropertyAssertion> skosTopConceptOfAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF));
                
                //skos:inScheme
                foreach (OWLObjectPropertyAssertion skosInSchemeAsn in skosInSchemeAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(skosConceptScheme)))
                    conceptsInScheme.Add(skosInSchemeAsn.SourceIndividualExpression.GetIRI());
                //skos:hasTopConcept
                foreach (OWLObjectPropertyAssertion skosHasTopConceptAsn in skosHasTopConceptAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConceptScheme)))
                    conceptsInScheme.Add(skosHasTopConceptAsn.TargetIndividualExpression.GetIRI());
                //skos:topConceptOf
                foreach (OWLObjectPropertyAssertion skosTopConceptOfAsn in skosTopConceptOfAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(skosConceptScheme)))
                    conceptsInScheme.Add(skosTopConceptOfAsn.SourceIndividualExpression.GetIRI());
            }

            return RDFQueryUtilities.RemoveDuplicates(conceptsInScheme);
        }

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
                broaderConcepts.AddRange(ontology.SubsumeBroaderTransitivity(skosConcept, skosBroaderTransitiveAsns, new Dictionary<long, RDFResource>()));
            }

            broaderConcepts.RemoveAll(c => c.Equals(skosConcept));
            return broaderConcepts;
        }
        internal static List<RDFResource> SubsumeBroaderTransitivity(this OWLOntology ontology, RDFResource skosConcept, List<OWLObjectPropertyAssertion> skosBroaderTransitiveAsns, Dictionary<long, RDFResource> visitContext)
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
                broaderTransitiveConcepts.AddRange(ontology.SubsumeBroaderTransitivity(broaderTransitiveConcept, skosBroaderTransitiveAsns, visitContext));

            return broaderTransitiveConcepts;
        }

        public static bool CheckHasNarrowerConcept(this OWLOntology ontology, RDFResource parentConcept, RDFResource childConcept)
            => parentConcept != null && childConcept != null && ontology != null && ontology.GetNarrowerConcepts(parentConcept).Any(concept => concept.Equals(childConcept));
        public static List<RDFResource> GetNarrowerConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> narrowerConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosNarrowerAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER));
                List<OWLObjectPropertyAssertion> skosNarrowerTransitiveAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE));

                //skos:narrower
                foreach (OWLObjectPropertyAssertion skosNarrowerAsn in skosNarrowerAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                    narrowerConcepts.Add(skosNarrowerAsn.TargetIndividualExpression.GetIRI());

                //skos:narrowerTransitive
                narrowerConcepts.AddRange(ontology.SubsumeNarrowerTransitivity(skosConcept, skosNarrowerTransitiveAsns, new Dictionary<long, RDFResource>()));
            }

            narrowerConcepts.RemoveAll(c => c.Equals(skosConcept));
            return narrowerConcepts;
        }
        internal static List<RDFResource> SubsumeNarrowerTransitivity(this OWLOntology ontology, RDFResource skosConcept, List<OWLObjectPropertyAssertion> skosNarrowerTransitiveAsns, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> narrowerTransitiveConcepts = new List<RDFResource>();

            #region visitContext
            if (!visitContext.ContainsKey(skosConcept.PatternMemberID))
                visitContext.Add(skosConcept.PatternMemberID, skosConcept);
            else
                return narrowerTransitiveConcepts;
            #endregion

            //skos:narrowerTransitive
            foreach (OWLObjectPropertyAssertion skosNarrowerTransitiveAsn in skosNarrowerTransitiveAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                narrowerTransitiveConcepts.Add(skosNarrowerTransitiveAsn.TargetIndividualExpression.GetIRI());

            foreach (RDFResource narrowerTransitiveConcept in narrowerTransitiveConcepts.ToList())
                narrowerTransitiveConcepts.AddRange(ontology.SubsumeNarrowerTransitivity(narrowerTransitiveConcept, skosNarrowerTransitiveAsns, visitContext));

            return narrowerTransitiveConcepts;
        }

        public static bool CheckHasBroadMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => leftConcept != null && rightConcept != null && ontology != null && ontology.GetBroadMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));
        public static List<RDFResource> GetBroadMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> broadMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosBroadMatchAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH));
                List<OWLObjectPropertyAssertion> skosNarrowMatchAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH));

                //skos:broadMatch
                foreach (OWLObjectPropertyAssertion skosBroadMatchAsn in skosBroadMatchAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                    broadMatchConcepts.Add(skosBroadMatchAsn.TargetIndividualExpression.GetIRI());
                //skos:narrowMatch
                foreach (OWLObjectPropertyAssertion skosNarrowMatchAsn in skosNarrowMatchAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(skosConcept)))
                    broadMatchConcepts.Add(skosNarrowMatchAsn.SourceIndividualExpression.GetIRI());
            }

            broadMatchConcepts.RemoveAll(c => c.Equals(skosConcept));
            return RDFQueryUtilities.RemoveDuplicates(broadMatchConcepts);
        }

        public static bool CheckHasNarrowMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => leftConcept != null && rightConcept != null && ontology != null && ontology.GetNarrowMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));
        public static List<RDFResource> GetNarrowMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> narrowMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosNarrowMatchAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH));
                List<OWLObjectPropertyAssertion> skosBroadMatchAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH));

                //skos:narrowMatch
                foreach (OWLObjectPropertyAssertion skosNarrowMatchAsn in skosNarrowMatchAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                    narrowMatchConcepts.Add(skosNarrowMatchAsn.TargetIndividualExpression.GetIRI());
                //skos:broadMatch
                foreach (OWLObjectPropertyAssertion skosBroadMatchAsn in skosBroadMatchAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(skosConcept)))
                    narrowMatchConcepts.Add(skosBroadMatchAsn.SourceIndividualExpression.GetIRI());
            }

            narrowMatchConcepts.RemoveAll(c => c.Equals(skosConcept));
            return RDFQueryUtilities.RemoveDuplicates(narrowMatchConcepts);
        }

        public static bool CheckHasCloseMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => leftConcept != null && rightConcept != null && ontology != null && ontology.GetCloseMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));
        public static List<RDFResource> GetCloseMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> closeMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosCloseMatchAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH));

                //skos:closeMatch
                foreach (OWLObjectPropertyAssertion skosCloseMatchAsn in skosCloseMatchAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                    closeMatchConcepts.Add(skosCloseMatchAsn.TargetIndividualExpression.GetIRI());
                foreach (OWLObjectPropertyAssertion skosCloseMatchAsn in skosCloseMatchAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(skosConcept)))
                    closeMatchConcepts.Add(skosCloseMatchAsn.SourceIndividualExpression.GetIRI());
            }

            closeMatchConcepts.RemoveAll(c => c.Equals(skosConcept));
            return RDFQueryUtilities.RemoveDuplicates(closeMatchConcepts);
        }

        public static bool CheckHasExactMatchConcept(this OWLOntology ontology, RDFResource leftConcept, RDFResource rightConcept)
            => leftConcept != null && rightConcept != null && ontology != null && ontology.GetExactMatchConcepts(leftConcept).Any(concept => concept.Equals(rightConcept));
        public static List<RDFResource> GetExactMatchConcepts(this OWLOntology ontology, RDFResource skosConcept)
        {
            List<RDFResource> exactMatchConcepts = new List<RDFResource>();

            if (skosConcept != null && ontology != null)
            {
                List<OWLObjectPropertyAssertion> objPropAsns = CalibrateObjectAssertions(ontology);
                List<OWLObjectPropertyAssertion> skosExactMatchAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH));

                //skos:exactMatch
                exactMatchConcepts.AddRange(ontology.SubsumeExactMatchTransitivity(skosConcept, skosExactMatchAsns, new Dictionary<long, RDFResource>()));
            }

            exactMatchConcepts.RemoveAll(c => c.Equals(skosConcept));
            return RDFQueryUtilities.RemoveDuplicates(exactMatchConcepts);
        }
        internal static List<RDFResource> SubsumeExactMatchTransitivity(this OWLOntology ontology, RDFResource skosConcept, List<OWLObjectPropertyAssertion> skosExactMatchAsns, Dictionary<long, RDFResource> visitContext)
        {
            List<RDFResource> exactMatchConcepts = new List<RDFResource>();

            #region visitContext
            if (!visitContext.ContainsKey(skosConcept.PatternMemberID))
                visitContext.Add(skosConcept.PatternMemberID, skosConcept);
            else
                return exactMatchConcepts;
            #endregion

            //skos:exactMatch
            foreach (OWLObjectPropertyAssertion skosExactMatchAsn in skosExactMatchAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(skosConcept)))
                exactMatchConcepts.Add(skosExactMatchAsn.TargetIndividualExpression.GetIRI());
            foreach (OWLObjectPropertyAssertion skosExactMatchAsn in skosExactMatchAsns.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(skosConcept)))
                exactMatchConcepts.Add(skosExactMatchAsn.SourceIndividualExpression.GetIRI());

            foreach (RDFResource exactMatchConcept in exactMatchConcepts.ToList())
                exactMatchConcepts.AddRange(ontology.SubsumeExactMatchTransitivity(exactMatchConcept, skosExactMatchAsns, visitContext));

            return exactMatchConcepts;
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