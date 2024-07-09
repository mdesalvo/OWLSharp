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

using System.Collections.Generic;
using System.Linq;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Helpers
{
	public static class OWLAnnotationAxiomHelper
	{
		#region Methods
		public static List<T> GetAnnotationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAnnotationAxiom
            => ontology?.AnnotationAxioms.OfType<T>().ToList() ?? new List<T>();
		
		public static bool CheckIsSubAnnotationPropertyOf(this OWLOntology ontology, OWLAnnotationProperty subAnnotationProperty, OWLAnnotationProperty superAnnotationProperty)
            => ontology != null && subAnnotationProperty != null && superAnnotationProperty != null && GetSubAnnotationPropertiesOf(ontology, superAnnotationProperty).Any(ap => ap.GetIRI().Equals(subAnnotationProperty.GetIRI()));

        public static List<OWLAnnotationProperty> GetSubAnnotationPropertiesOf(this OWLOntology ontology, OWLAnnotationProperty annotationProperty)
        {
            #region Utilities
            List<OWLAnnotationProperty> FindSubAnnotationPropertiesOf(RDFResource annPropIRI, List<OWLSubAnnotationPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLAnnotationProperty> foundSubAnnotationProperties = new List<OWLAnnotationProperty>();

                #region VisitContext
                if (!visitContext.Contains(annPropIRI.PatternMemberID))
                    visitContext.Add(annPropIRI.PatternMemberID);
                else
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

		public static bool CheckIsSuperAnnotationPropertyOf(this OWLOntology ontology, OWLAnnotationProperty superAnnotationProperty, OWLAnnotationProperty subAnnotationProperty)
            => ontology != null && superAnnotationProperty != null && subAnnotationProperty != null && GetSuperAnnotationPropertiesOf(ontology, subAnnotationProperty).Any(ap => ap.GetIRI().Equals(superAnnotationProperty.GetIRI()));

		public static List<OWLAnnotationProperty> GetSuperAnnotationPropertiesOf(this OWLOntology ontology, OWLAnnotationProperty annotationProperty)
        {
            #region Utilities
            List<OWLAnnotationProperty> FindSuperAnnotationPropertiesOf(RDFResource annPropIRI, List<OWLSubAnnotationPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLAnnotationProperty> foundSuperAnnotationProperties = new List<OWLAnnotationProperty>();

                #region VisitContext
                if (!visitContext.Contains(annPropIRI.PatternMemberID))
                    visitContext.Add(annPropIRI.PatternMemberID);
                else
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
		internal static List<OWLAnnotationAssertion> SelectAnnotationAssertionsByAPEX(List<OWLAnnotationAssertion> annAsnAxioms, OWLAnnotationProperty annProp)
			=> annAsnAxioms.Where(ax => ax.AnnotationProperty.GetIRI().Equals(annProp.GetIRI())).ToList();
		#endregion
	}
}