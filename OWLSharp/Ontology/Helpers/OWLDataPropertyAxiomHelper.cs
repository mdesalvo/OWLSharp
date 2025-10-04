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
    /// OWLDataPropertyAxiomHelper simplifies data property axiom modeling and analysis with a set of facilities
    /// </summary>
    public static class OWLDataPropertyAxiomHelper
    {
        #region Methods
        /// <summary>
        /// Enlists the given type of data property axiom from the T-BOX of the given ontology
        /// </summary>
        public static List<T> GetDataPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLDataPropertyAxiom
            => ontology?.DataPropertyAxioms.OfType<T>().ToList() ?? new List<T>();

        /// <summary>
        /// Checks if the given ontology has the given data property axiom in its T-BOX
        /// </summary>
        public static bool CheckHasDataPropertyAxiom<T>(this OWLOntology ontology, T dataPropertyAxiom) where T : OWLDataPropertyAxiom
            => GetDataPropertyAxiomsOfType<T>(ontology).Any(ax => string.Equals(ax.GetXML(), dataPropertyAxiom?.GetXML()));

        /// <summary>
        /// Declares the given data property axiom to the T-BOX of the given ontology
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static void DeclareDataPropertyAxiom<T>(this OWLOntology ontology, T dataPropertyAxiom) where T : OWLDataPropertyAxiom
        {
            #region Guards
            if (dataPropertyAxiom == null)
                throw new OWLException($"Cannot declare data property axiom because given '{nameof(dataPropertyAxiom)}' parameter is null");
            #endregion

            if (!CheckHasDataPropertyAxiom(ontology, dataPropertyAxiom))
                ontology?.DataPropertyAxioms.Add(dataPropertyAxiom);
        }

        /// <summary>
        /// Checks for the existence of an OWLSubDataPropertyOf axiom directly or indirectly relating the given child->mother data properties in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckIsSubDataPropertyOf(this OWLOntology ontology, OWLDataProperty childDataProperty, OWLDataProperty motherDataProperty)
            => ontology != null && childDataProperty != null && motherDataProperty != null && GetSubDataPropertiesOf(ontology, motherDataProperty).Any(dp => dp.GetIRI().Equals(childDataProperty.GetIRI()));

        /// <summary>
        /// Enlists the data properties which are directly or indirectly related as children to the given mother data property in the T-BOX of the given ontology
        /// </summary>
        public static List<OWLDataProperty> GetSubDataPropertiesOf(this OWLOntology ontology, OWLDataProperty dataProperty)
        {
            #region Utilities
            //Facility for recursive traversal of the OWLSubDataPropertyOf hierarchy
            List<OWLDataProperty> FindSubDataPropertiesOf(RDFResource dataPropertyIRI, List<OWLSubDataPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundSubDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Add(dataPropertyIRI.PatternMemberID))
                    return foundSubDataProperties;
                #endregion

                #region Discovery
                foreach (OWLSubDataPropertyOf axiom in axioms.Where(ax => ax.SuperDataProperty.GetIRI().Equals(dataPropertyIRI)))
                    foundSubDataProperties.Add(axiom.SubDataProperty);

                //SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                foreach (OWLDataProperty subDataProperty in foundSubDataProperties.ToList())
                    foundSubDataProperties.AddRange(FindSubDataPropertiesOf(subDataProperty.GetIRI(), axioms, visitContext));
                #endregion

                return foundSubDataProperties;
            }
            #endregion

            List<OWLDataProperty> subDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
            {
                HashSet<long> visitContext = new HashSet<long>();
                List<OWLSubDataPropertyOf> subDtPropOfAxs = GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>(ontology);

                //SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                subDataProperties.AddRange(FindSubDataPropertiesOf(dataProperty.GetIRI(), subDtPropOfAxs, visitContext));

                //EquivalentDataProperties(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                foreach (OWLDataProperty equivDtProp in GetEquivalentDataProperties(ontology, dataProperty))
                    subDataProperties.AddRange(FindSubDataPropertiesOf(equivDtProp.GetIRI(), subDtPropOfAxs, visitContext));

                //SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)
                foreach (OWLDataProperty subDataProperty in subDataProperties.ToList())
                    subDataProperties.AddRange(GetEquivalentDataProperties(ontology, subDataProperty));

                return OWLExpressionHelper.RemoveDuplicates(subDataProperties);
            }
            return subDataProperties;
        }

        /// <summary>
        /// Checks for the existence of an OWLSubDataPropertyOf axiom directly or indirectly relating the given mother->child data properties in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckIsSuperDataPropertyOf(this OWLOntology ontology, OWLDataProperty motherDataProperty, OWLDataProperty childDataProperty)
            => ontology != null && motherDataProperty != null && childDataProperty != null && GetSuperDataPropertiesOf(ontology, childDataProperty).Any(dp => dp.GetIRI().Equals(motherDataProperty.GetIRI()));

        /// <summary>
        /// Enlists the data properties which are directly or indirectly related as mother to the given child data property in the T-BOX of the given ontology
        /// </summary>
        public static List<OWLDataProperty> GetSuperDataPropertiesOf(this OWLOntology ontology, OWLDataProperty dataProperty)
        {
            #region Utilities
            //Facility for recursive traversal of the OWLSubDataPropertyOf hierarchy
            List<OWLDataProperty> FindSuperDataPropertiesOf(RDFResource dtPropIRI, List<OWLSubDataPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundSuperDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Add(dtPropIRI.PatternMemberID))
                    return foundSuperDataProperties;
                #endregion

                #region Discovery
                foreach (OWLSubDataPropertyOf axiom in axioms.Where(ax => ax.SubDataProperty.GetIRI().Equals(dtPropIRI)))
                    foundSuperDataProperties.Add(axiom.SuperDataProperty);

                //SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                foreach (OWLDataProperty superDataProperty in foundSuperDataProperties.ToList())
                    foundSuperDataProperties.AddRange(FindSuperDataPropertiesOf(superDataProperty.GetIRI(), axioms, visitContext));
                #endregion

                return foundSuperDataProperties;
            }
            #endregion

            List<OWLDataProperty> superDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
            {
                HashSet<long> visitContext = new HashSet<long>();
                List<OWLSubDataPropertyOf> subDtPropOfAxs = GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>(ontology);

                //SubDataPropertyOf(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                superDataProperties.AddRange(FindSuperDataPropertiesOf(dataProperty.GetIRI(), subDtPropOfAxs, visitContext));

                //EquivalentDataProperties(P1,P2) ^ SubDataPropertyOf(P2,P3) -> SubDataPropertyOf(P1,P3)
                foreach (OWLDataProperty equivDtProp in GetEquivalentDataProperties(ontology, dataProperty))
                    superDataProperties.AddRange(FindSuperDataPropertiesOf(equivDtProp.GetIRI(), subDtPropOfAxs, visitContext));

                //SubDataPropertyOf(P1,P2) ^ EquivalentDataProperties(P2,P3) -> SubDataPropertyOf(P1,P3)
                foreach (OWLDataProperty superDataProperty in superDataProperties.ToList())
                    superDataProperties.AddRange(GetEquivalentDataProperties(ontology, superDataProperty));

                return OWLExpressionHelper.RemoveDuplicates(superDataProperties);
            }
            return superDataProperties;
        }

        /// <summary>
        /// Checks for the existence of an OWLEquivalentDataProperties axiom directly or indirectly relating the given data properties in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckAreEquivalentDataProperties(this OWLOntology ontology, OWLDataProperty leftDataProperty, OWLDataProperty rightDataProperty)
            => ontology != null && leftDataProperty != null && rightDataProperty != null && GetEquivalentDataProperties(ontology, leftDataProperty).Any(dex => dex.GetIRI().Equals(rightDataProperty.GetIRI()));

        /// <summary>
        /// Enlists the data properties which are directly or indirectly related as equivalent data properties to the given data property in the T-BOX of the given ontology
        /// </summary>
        public static List<OWLDataProperty> GetEquivalentDataProperties(this OWLOntology ontology, OWLDataProperty dataProperty)
        {
            #region Utilities
            //Facility for recursive traversal of the OWLEquivalentDataProperties relations
            List<OWLDataProperty> FindEquivalentDataProperties(RDFResource dtPropIRI, List<OWLEquivalentDataProperties> axioms, HashSet<long> visitContext)
            {
                List<OWLDataProperty> foundEquivalentDataProperties = new List<OWLDataProperty>();

                #region VisitContext
                if (!visitContext.Add(dtPropIRI.PatternMemberID))
                    return foundEquivalentDataProperties;
                #endregion

                #region Discovery
                foreach (OWLEquivalentDataProperties axiom in axioms.Where(ax => ax.DataProperties.Any(dp => dp.GetIRI().Equals(dtPropIRI))))
                    foundEquivalentDataProperties.AddRange(axiom.DataProperties);

                //EquivalentDataProperties(P1,P2) ^ EquivalentDataProperties(P2,P3) -> EquivalentDataProperties(P1,P3)
                foreach (OWLDataProperty equivalentDataProperty in foundEquivalentDataProperties.ToList())
                    foundEquivalentDataProperties.AddRange(FindEquivalentDataProperties(equivalentDataProperty.GetIRI(), axioms, visitContext));
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

        /// <summary>
        /// Checks for the existence of an OWLDisjointDataProperties axiom directly or indirectly relating the given data properties in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckAreDisjointDataProperties(this OWLOntology ontology, OWLDataProperty leftDataProperty, OWLDataProperty rightDataProperty)
            => ontology != null && leftDataProperty != null && rightDataProperty != null && GetDisjointDataProperties(ontology, leftDataProperty).Any(dex => dex.GetIRI().Equals(rightDataProperty.GetIRI()));

        /// <summary>
        /// Enlists the data properties which are directly or indirectly related as disjoint data properties to the given data property in the T-BOX of the given ontology
        /// </summary>
        public static List<OWLDataProperty> GetDisjointDataProperties(this OWLOntology ontology, OWLDataProperty dataProperty)
        {
            List<OWLDataProperty> disjointDataProperties = new List<OWLDataProperty>();
            if (ontology != null && dataProperty != null)
            {
                RDFResource dtPropIRI = dataProperty.GetIRI();

                //We can only enlist explicitly declared disjoint data properties: no other kind
                //of reasoning is possible, since this relation is protected under OWA behavior!
                foreach (OWLDisjointDataProperties axiom in GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>(ontology).Where(ax => ax.DataProperties.Any(dp => dp.GetIRI().Equals(dtPropIRI))))
                    disjointDataProperties.AddRange(axiom.DataProperties);

                disjointDataProperties.RemoveAll(res => res.GetIRI().Equals(dtPropIRI));
                return OWLExpressionHelper.RemoveDuplicates(disjointDataProperties);
            }
            return disjointDataProperties;
        }

        /// <summary>
        /// Checks for the existence of an OWLFunctionalDataProperty axiom about the given data property in the T-BOX of the given ontology
        /// </summary>
        public static bool CheckHasFunctionalDataProperty(this OWLOntology ontology, OWLDataProperty dataProperty)
            => ontology != null && dataProperty != null && GetDataPropertyAxiomsOfType<OWLFunctionalDataProperty>(ontology).Any(fdp => fdp.DataProperty.GetIRI().Equals(dataProperty.GetIRI()));
        #endregion
    }
}