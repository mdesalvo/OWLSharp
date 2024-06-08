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
	public static class OWLDataPropertyAxiomHelper
	{
		#region Methods
		public static List<T> GetDataPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLDataPropertyAxiom
            => ontology?.DataPropertyAxioms.OfType<T>().ToList() ?? new List<T>();
		
		public static bool CheckIsSubDataPropertyOf(this OWLOntology ontology, OWLDataProperty subDataProperty, OWLDataProperty superDataProperty, bool directOnly=false)
            => ontology != null && subDataProperty != null && superDataProperty != null && GetSubDataPropertiesOf(ontology, superDataProperty, directOnly).Any(dp => dp.GetIRI().Equals(subDataProperty.GetIRI()));

        public static List<OWLDataProperty> GetSubDataPropertiesOf(this OWLOntology ontology, OWLDataProperty dataProperty, bool directOnly=false)
        {
            #region Utilities
            List<OWLDataProperty> FindSubDataPropertiesOf(RDFResource dataPropertyIRI, List<OWLSubDataPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundSubDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Contains(dataPropertyIRI.PatternMemberID))
                    visitContext.Add(dataPropertyIRI.PatternMemberID);
                else
                    return foundSubDataProperties;
                #endregion

				#region Discovery
				foreach (OWLSubDataPropertyOf axiom in axioms.Where(ax => ax.SuperDataProperty.GetIRI().Equals(dataPropertyIRI)))
                    foundSubDataProperties.Add(axiom.SubDataProperty);

				if (!directOnly)
                {
					//SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                    foreach (OWLDataProperty subDataProperty in foundSubDataProperties.ToList())
                        foundSubDataProperties.AddRange(FindSubDataPropertiesOf(subDataProperty.GetIRI(), axioms, visitContext));
                }
				#endregion

                return foundSubDataProperties;
            }
            #endregion

            List<OWLDataProperty> subDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
			{
				RDFResource dtPropIRI = dataProperty.GetIRI();
				subDataProperties.AddRange(FindSubDataPropertiesOf(dtPropIRI, GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//SubDataPropertyOf(P1,P2) ^ EquivalentDataProperty(P2,P3) -> SubDataPropertyOf(P1,P3)
                    foreach (OWLDataProperty subDataProperty in subDataProperties.ToList())
						subDataProperties.AddRange(GetEquivalentDataProperties(ontology, subDataProperty, directOnly));
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(subDataProperties);
        }

		public static bool CheckIsSuperDataPropertyOf(this OWLOntology ontology, OWLDataProperty superDataProperty, OWLDataProperty subDataProperty, bool directOnly=false)
            => ontology != null && superDataProperty != null && subDataProperty != null && GetSuperDataPropertiesOf(ontology, subDataProperty, directOnly).Any(dp => dp.GetIRI().Equals(superDataProperty.GetIRI()));

		public static List<OWLDataProperty> GetSuperDataPropertiesOf(this OWLOntology ontology, OWLDataProperty dataProperty, bool directOnly=false)
        {
            #region Utilities
            List<OWLDataProperty> FindSuperDataPropertiesOf(RDFResource dtPropIRI, List<OWLSubDataPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundSuperDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Contains(dtPropIRI.PatternMemberID))
                    visitContext.Add(dtPropIRI.PatternMemberID);
                else
                    return foundSuperDataProperties;
                #endregion

				#region Discovery
				foreach (OWLSubDataPropertyOf axiom in axioms.Where(ax => ax.SubDataProperty.GetIRI().Equals(dtPropIRI)))
                    foundSuperDataProperties.Add(axiom.SuperDataProperty);

				if (!directOnly)
                {
					//SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                    foreach (OWLDataProperty superDataProperty in foundSuperDataProperties.ToList())
                        foundSuperDataProperties.AddRange(FindSuperDataPropertiesOf(superDataProperty.GetIRI(), axioms, visitContext));
                }
				#endregion

                return foundSuperDataProperties;
            }
            #endregion

            List<OWLDataProperty> superDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
			{
				RDFResource dtPropIRI = dataProperty.GetIRI();
				superDataProperties.AddRange(FindSuperDataPropertiesOf(dtPropIRI, GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)
                    foreach (OWLDataProperty superDataProperty in superDataProperties.ToList())
						superDataProperties.AddRange(GetEquivalentDataProperties(ontology, superDataProperty, directOnly));
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(superDataProperties);
        }

		public static bool CheckAreEquivalentDataProperties(this OWLOntology ontology, OWLDataProperty leftDataProperty, OWLDataProperty rightDataProperty, bool directOnly=false)
            => ontology != null && leftDataProperty != null && rightDataProperty != null && GetEquivalentDataProperties(ontology, leftDataProperty, directOnly).Any(dex => dex.GetIRI().Equals(rightDataProperty.GetIRI()));

        public static List<OWLDataProperty> GetEquivalentDataProperties(this OWLOntology ontology, OWLDataProperty dataProperty, bool directOnly=false)
        {
            #region Utilities
            List<OWLDataProperty> FindEquivalentDataProperties(RDFResource dtPropIRI, List<OWLEquivalentDataProperties> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundEquivalentDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Contains(dtPropIRI.PatternMemberID))
                    visitContext.Add(dtPropIRI.PatternMemberID);
                else
                    return foundEquivalentDataProperties;
                #endregion

				#region Discovery
				foreach (OWLEquivalentDataProperties axiom in axioms.Where(ax => ax.DataProperties.Any(dp => dp.GetIRI().Equals(dtPropIRI))))
                    foundEquivalentDataProperties.AddRange(axiom.DataProperties);

				if (!directOnly)
                {
					//EquivalentDataProperty(P1,P2) ^ EquivalentDataProperty(P2,P3) -> EquivalentDataProperty(P1,P3)
                    foreach (OWLDataProperty equivalentDataProperty in foundEquivalentDataProperties.ToList())
                        foundEquivalentDataProperties.AddRange(FindEquivalentDataProperties(equivalentDataProperty.GetIRI(), axioms, visitContext));
                }
				#endregion

				foundEquivalentDataProperties.RemoveAll(res => res.GetIRI().Equals(dtPropIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundEquivalentDataProperties);
            }
            #endregion

            List<OWLDataProperty> equivalentDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
                equivalentDataProperties.AddRange(FindEquivalentDataProperties(dataProperty.GetIRI(), GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>(ontology), new HashSet<long>()));
            return equivalentDataProperties;
        }

		public static bool CheckAreDisjointDataProperties(this OWLOntology ontology, OWLDataProperty leftDataProperty, OWLDataProperty rightDataProperty, bool directOnly=false)
            => ontology != null && leftDataProperty != null && rightDataProperty != null && GetDisjointDataProperties(ontology, leftDataProperty, directOnly).Any(dex => dex.GetIRI().Equals(rightDataProperty.GetIRI()));

        public static List<OWLDataProperty> GetDisjointDataProperties(this OWLOntology ontology, OWLDataProperty dataProperty, bool directOnly=false)
        {
            #region Utilities
            List<OWLDataProperty> FindDisjointDataProperties(RDFResource dtPropIRI, List<OWLDisjointDataProperties> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundDisjointDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Contains(dtPropIRI.PatternMemberID))
                    visitContext.Add(dtPropIRI.PatternMemberID);
                else
                    return foundDisjointDataProperties;
                #endregion

                #region Discovery
                foreach (OWLDisjointDataProperties axiom in axioms.Where(ax => ax.DataProperties.Any(dp => dp.GetIRI().Equals(dtPropIRI))))
                    foundDisjointDataProperties.AddRange(axiom.DataProperties);
                #endregion

                foundDisjointDataProperties.RemoveAll(res => res.GetIRI().Equals(dtPropIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundDisjointDataProperties);
            }
            #endregion

            List<OWLDataProperty> disjointDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
			{
				HashSet<long> visitContext = new HashSet<long>();
				List<OWLDisjointDataProperties> disjointDataPropertyAxioms = GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>(ontology);
				disjointDataProperties.AddRange(FindDisjointDataProperties(dataProperty.GetIRI(), disjointDataPropertyAxioms, visitContext));

				if (!directOnly)
				{
					//EquivalentDataProperties(P1,P2) ^ PropertyDisjointWith(P2,P3) -> PropertyDisjointWith(P1,P3)
					foreach (OWLDataProperty equivalentDataProperty in GetEquivalentDataProperties(ontology, dataProperty, directOnly))
						disjointDataProperties.AddRange(FindDisjointDataProperties(equivalentDataProperty.GetIRI(), disjointDataPropertyAxioms, visitContext));

					//SubDataPropertyOf(P1,P2) ^ PropertyDisjointWith(P2,P3) -> PropertyDisjointWith(P1,P3)
					foreach (OWLDataProperty superDataProperty in GetSuperDataPropertiesOf(ontology, dataProperty, directOnly))
						disjointDataProperties.AddRange(FindDisjointDataProperties(superDataProperty.GetIRI(), disjointDataPropertyAxioms, visitContext));
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(disjointDataProperties);
        }
		
		public static bool CheckHasFunctionalDataProperty(this OWLOntology ontology, OWLDataProperty dataProperty)
            => ontology != null && dataProperty != null && GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>(ontology).Any(fdp => fdp.DataProperty.GetIRI().Equals(dataProperty.GetIRI()));
		#endregion
	}
}