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

using System.Collections.Generic;
using System.Linq;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAnnotationAxiomHelper simplifies annotation axiom modeling and analysis with a set of facilities
    /// </summary>
    public static class OWLAnnotationAxiomHelper
    {
        #region Methods
        /// <summary>
        /// Enlists the given type of annotation axiom from the T-BOX/A-BOX of the given ontology
        /// </summary>
        public static List<T> GetAnnotationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAnnotationAxiom
            => ontology?.AnnotationAxioms.OfType<T>().ToList() ?? new List<T>();

        /// <summary>
        /// Checks if the given ontology has the given assertion axiom in its T-BOX/A-BOX
        /// </summary>
        public static bool CheckHasAnnotationAxiom<T>(this OWLOntology ontology, T annotationAxiom) where T : OWLAnnotationAxiom
            => GetAnnotationAxiomsOfType<T>(ontology).Any(ax => string.Equals(ax.GetXML(), annotationAxiom?.GetXML()));

        /// <summary>
        /// Declares the given annotation axiom to the T-BOX/A-BOX of the given ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static void DeclareAnnotationAxiom<T>(this OWLOntology ontology, T annotationAxiom) where T : OWLAnnotationAxiom
        {
            #region Guards
            if (annotationAxiom == null)
                throw new OWLException($"Cannot declare annotation axiom because given '{nameof(annotationAxiom)}' parameter is null");
            #endregion

            if (!CheckHasAnnotationAxiom(ontology, annotationAxiom))
                ontology?.AnnotationAxioms.Add(annotationAxiom);
        }

        /// <summary>
        /// Checks for the existence of an OWLSubAnnotationPropertyOf axiom directly or indirectly relating the given child->mother annotation properties in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckIsSubAnnotationPropertyOf(this OWLOntology ontology, OWLAnnotationProperty childAnnProp, OWLAnnotationProperty motherAnnProp)
            => ontology != null && childAnnProp != null && motherAnnProp != null && GetSubAnnotationPropertiesOf(ontology, motherAnnProp).Any(ap => ap.GetIRI().Equals(childAnnProp.GetIRI()));

        /// <summary>
        /// Enlists the annotation properties which are directly or indirectly related as children to the given mother annotation property in the T-BOX of the given ontology
        /// </summary>
        public static List<OWLAnnotationProperty> GetSubAnnotationPropertiesOf(this OWLOntology ontology, OWLAnnotationProperty annotationProperty)
        {
            #region Utilities
            //Facility for recursive traversal of the OWLSubAnnotationPropertyOf hierarchy
            List<OWLAnnotationProperty> FindSubAnnotationPropertiesOf(RDFResource annPropIRI, List<OWLSubAnnotationPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLAnnotationProperty> foundSubAnnotationProperties = new List<OWLAnnotationProperty>();

                #region VisitContext
                if (!visitContext.Add(annPropIRI.PatternMemberID))
                    return foundSubAnnotationProperties;
                #endregion

                #region Discovery
                foreach (OWLSubAnnotationPropertyOf axiom in axioms.Where(ax => ax.SuperAnnotationProperty.GetIRI().Equals(annPropIRI)))
                    foundSubAnnotationProperties.Add(axiom.SubAnnotationProperty);

                //SubAnnotationPropertyOf(P1,P2) ^ SubAnnotationPropertyOf(P2,P3) -> SubAnnotationPropertyOf(P1,P3)
                foreach (OWLAnnotationProperty subAnnotationProperty in foundSubAnnotationProperties.ToList())
                    foundSubAnnotationProperties.AddRange(FindSubAnnotationPropertiesOf(subAnnotationProperty.GetIRI(), axioms, visitContext));
                #endregion

                return foundSubAnnotationProperties;
            }
            #endregion

            List<OWLAnnotationProperty> subAnnotationProperties = new List<OWLAnnotationProperty>();
            if (ontology != null && annotationProperty != null)
            {
                RDFResource dtPropIRI = annotationProperty.GetIRI();
                subAnnotationProperties.AddRange(FindSubAnnotationPropertiesOf(dtPropIRI, GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>(ontology), new HashSet<long>()));
            }
            return OWLExpressionHelper.RemoveDuplicates(subAnnotationProperties);
        }

        /// <summary>
        /// Checks for the existence of an OWLSubAnnotationPropertyOf axiom directly or indirectly relating the given mother->child annotation properties in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckIsSuperAnnotationPropertyOf(this OWLOntology ontology, OWLAnnotationProperty motherAnnProp, OWLAnnotationProperty childAnnProp)
            => ontology != null && motherAnnProp != null && childAnnProp != null && GetSuperAnnotationPropertiesOf(ontology, childAnnProp).Any(ap => ap.GetIRI().Equals(motherAnnProp.GetIRI()));

        /// <summary>
        /// Enlists the annotation properties which are directly or indirectly related as mother to the given child annotationProperty in the T-BOX of the given ontology
        /// </summary>
        public static List<OWLAnnotationProperty> GetSuperAnnotationPropertiesOf(this OWLOntology ontology, OWLAnnotationProperty annotationProperty)
        {
            #region Utilities
            //Facility for recursive traversal of the OWLSubAnnotationPropertyOf hierarchy
            List<OWLAnnotationProperty> FindSuperAnnotationPropertiesOf(RDFResource annPropIRI, List<OWLSubAnnotationPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLAnnotationProperty> foundSuperAnnotationProperties = new List<OWLAnnotationProperty>();

                #region VisitContext
                if (!visitContext.Add(annPropIRI.PatternMemberID))
                    return foundSuperAnnotationProperties;
                #endregion

                #region Discovery
                foreach (OWLSubAnnotationPropertyOf axiom in axioms.Where(ax => ax.SubAnnotationProperty.GetIRI().Equals(annPropIRI)))
                    foundSuperAnnotationProperties.Add(axiom.SuperAnnotationProperty);

                //SubAnnotationPropertyOf(P1,P2) ^ SubAnnotationPropertyOf(P2,P3) -> SubAnnotationPropertyOf(P1,P3)
                foreach (OWLAnnotationProperty superAnnotationProperty in foundSuperAnnotationProperties.ToList())
                    foundSuperAnnotationProperties.AddRange(FindSuperAnnotationPropertiesOf(superAnnotationProperty.GetIRI(), axioms, visitContext));
                #endregion

                return foundSuperAnnotationProperties;
            }
            #endregion

            List<OWLAnnotationProperty> superAnnotationProperties = new List<OWLAnnotationProperty>();
            if (ontology != null && annotationProperty != null)
            {
                RDFResource dtPropIRI = annotationProperty.GetIRI();
                superAnnotationProperties.AddRange(FindSuperAnnotationPropertiesOf(dtPropIRI, GetAnnotationAxiomsOfType<OWLSubAnnotationPropertyOf>(ontology), new HashSet<long>()));
            }
            return OWLExpressionHelper.RemoveDuplicates(superAnnotationProperties);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Filters the given set of OWLAnnotationAssertion by searching those using the given annotation property
        /// </summary>
        internal static List<OWLAnnotationAssertion> SelectAnnotationAssertionsByAPEX(List<OWLAnnotationAssertion> annAsnAxioms, OWLAnnotationProperty annProp)
            => annAsnAxioms.Where(ax => ax.AnnotationProperty.GetIRI().Equals(annProp.GetIRI())).ToList();
        #endregion
    }
}