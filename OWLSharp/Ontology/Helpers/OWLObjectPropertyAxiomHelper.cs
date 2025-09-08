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
    public static class OWLObjectPropertyAxiomHelper
    {
        #region Methods
        public static List<T> GetObjectPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLObjectPropertyAxiom
            => ontology?.ObjectPropertyAxioms.OfType<T>().ToList() ?? [];

        public static bool CheckHasObjectPropertyAxiom<T>(this OWLOntology ontology, T objectPropertyAxiom) where T : OWLObjectPropertyAxiom
            => GetObjectPropertyAxiomsOfType<T>(ontology).Any(ax => string.Equals(ax.GetXML(), objectPropertyAxiom?.GetXML()));

        public static void DeclareObjectPropertyAxiom<T>(this OWLOntology ontology, T objectPropertyAxiom) where T : OWLObjectPropertyAxiom
        {
            #region Guards
            if (objectPropertyAxiom == null)
                throw new OWLException("Cannot declare object property axiom because given \"objectPropertyAxiom\" parameter is null");
            #endregion

            if (!CheckHasObjectPropertyAxiom(ontology, objectPropertyAxiom))
                ontology?.ObjectPropertyAxioms.Add(objectPropertyAxiom);
        }

        public static bool CheckIsSubObjectPropertyOf(this OWLOntology ontology, OWLObjectPropertyExpression subObjPropExpr, OWLObjectPropertyExpression superObjPropExpr)
            => ontology != null && subObjPropExpr != null && superObjPropExpr != null && GetSubObjectPropertiesOf(ontology, superObjPropExpr).Any(opex => opex.GetIRI().Equals(subObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetSubObjectPropertiesOf(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindSubObjectPropertiesOf(RDFResource objPropExprIRI, List<OWLSubObjectPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundSubObjPropExprs = [];

                #region VisitContext
                if (!visitContext.Add(objPropExprIRI.PatternMemberID))
                    return foundSubObjPropExprs;
                #endregion

                #region Discovery
                foreach (OWLSubObjectPropertyOf axiom in axioms.Where(ax => ax.SuperObjectPropertyExpression.GetIRI().Equals(objPropExprIRI)
                                                                              && ax.SubObjectPropertyExpression != null))
                    foundSubObjPropExprs.Add(axiom.SubObjectPropertyExpression);

                //SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                foreach (OWLObjectPropertyExpression subObjPropExpr in foundSubObjPropExprs.ToList())
                    foundSubObjPropExprs.AddRange(FindSubObjectPropertiesOf(subObjPropExpr.GetIRI(), axioms, visitContext));
                #endregion

                return foundSubObjPropExprs;
            }
            #endregion

            List<OWLObjectPropertyExpression> subObjPropExprs = [];
            if (ontology != null && objPropExpr != null)
            {
                RDFResource objPropIRI = objPropExpr.GetIRI();
                HashSet<long> visitContext = [];
                List<OWLSubObjectPropertyOf> subObjPropOfAxs = GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>(ontology);
                List<OWLObjectPropertyExpression> equivObjPropsOfObjPropExpr = GetEquivalentObjectProperties(ontology, objPropExpr);

                //SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                subObjPropExprs.AddRange(FindSubObjectPropertiesOf(objPropIRI, subObjPropOfAxs, visitContext));

                //EquivalentObjectProperties(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                foreach (OWLObjectPropertyExpression equivObjPropExpr in equivObjPropsOfObjPropExpr)
                    subObjPropExprs.AddRange(FindSubObjectPropertiesOf(equivObjPropExpr.GetIRI(), subObjPropOfAxs, visitContext));

                //SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
                foreach (OWLObjectPropertyExpression subObjPropExpr in subObjPropExprs.ToList())
                    subObjPropExprs.AddRange(GetEquivalentObjectProperties(ontology, subObjPropExpr));
            }
            return OWLExpressionHelper.RemoveDuplicates(subObjPropExprs);
        }

        public static bool CheckIsSuperObjectPropertyOf(this OWLOntology ontology, OWLObjectPropertyExpression superObjPropExpr, OWLObjectPropertyExpression subObjPropExpr)
            => ontology != null && superObjPropExpr != null && subObjPropExpr != null && GetSuperObjectPropertiesOf(ontology, subObjPropExpr).Any(opex => opex.GetIRI().Equals(superObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetSuperObjectPropertiesOf(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindSuperObjectPropertiesOf(RDFResource objPropExprIRI, List<OWLSubObjectPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundSuperObjPropExprs = [];

                #region VisitContext
                if (!visitContext.Add(objPropExprIRI.PatternMemberID))
                    return foundSuperObjPropExprs;
                #endregion

                #region Discovery
                foreach (OWLSubObjectPropertyOf axiom in axioms.Where(ax => ax.SubObjectPropertyExpression.GetIRI().Equals(objPropExprIRI)))
                    foundSuperObjPropExprs.Add(axiom.SuperObjectPropertyExpression);

                //SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                foreach (OWLObjectPropertyExpression superObjPropExpr in foundSuperObjPropExprs.ToList())
                    foundSuperObjPropExprs.AddRange(FindSuperObjectPropertiesOf(superObjPropExpr.GetIRI(), axioms, visitContext));
                #endregion

                return foundSuperObjPropExprs;
            }
            #endregion

            List<OWLObjectPropertyExpression> superObjPropExprs = [];
            if (ontology != null && objPropExpr != null)
            {
                RDFResource objPropExprIRI = objPropExpr.GetIRI();
                HashSet<long> visitContext = [];
                List<OWLSubObjectPropertyOf> subObjPropOfAxs = GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>(ontology);

                //SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                superObjPropExprs.AddRange(FindSuperObjectPropertiesOf(objPropExprIRI, subObjPropOfAxs, visitContext));

                //EquivalentObjectProperties(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                foreach (OWLObjectPropertyExpression equivObjPropExpr in GetEquivalentObjectProperties(ontology, objPropExpr))
                    superObjPropExprs.AddRange(FindSuperObjectPropertiesOf(equivObjPropExpr.GetIRI(), subObjPropOfAxs, visitContext));

                //SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
                foreach (OWLObjectPropertyExpression superObjPropExpr in superObjPropExprs.ToList())
                    superObjPropExprs.AddRange(GetEquivalentObjectProperties(ontology, superObjPropExpr));
            }
            return OWLExpressionHelper.RemoveDuplicates(superObjPropExprs);
        }

        public static bool CheckAreEquivalentObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression leftObjPropExpr, OWLObjectPropertyExpression rightObjPropExpr)
            => ontology != null && leftObjPropExpr != null && rightObjPropExpr != null && GetEquivalentObjectProperties(ontology, leftObjPropExpr).Any(oex => oex.GetIRI().Equals(rightObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetEquivalentObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindEquivalentObjectProperties(RDFResource objPropExprIRI, List<OWLEquivalentObjectProperties> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundEquivObjPropExprs = [];

                #region VisitContext
                if (!visitContext.Add(objPropExprIRI.PatternMemberID))
                    return foundEquivObjPropExprs;
                #endregion

                #region Discovery
                foreach (OWLEquivalentObjectProperties axiom in axioms.Where(ax => ax.ObjectPropertyExpressions.Any(opex => opex.GetIRI().Equals(objPropExprIRI))))
                    foundEquivObjPropExprs.AddRange(axiom.ObjectPropertyExpressions);

                //EquivalentObjectProperties(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> EquivalentObjectProperties(P1,P3)
                foreach (OWLObjectPropertyExpression equivObjPropExpr in foundEquivObjPropExprs.ToList())
                    foundEquivObjPropExprs.AddRange(FindEquivalentObjectProperties(equivObjPropExpr.GetIRI(), axioms, visitContext));
                #endregion

                foundEquivObjPropExprs.RemoveAll(res => res.GetIRI().Equals(objPropExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundEquivObjPropExprs);
            }
            #endregion

            List<OWLObjectPropertyExpression> equivObjPropExprs = [];
            if (ontology != null && objPropExpr != null)
                equivObjPropExprs.AddRange(FindEquivalentObjectProperties(objPropExpr.GetIRI(), GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>(ontology), []));
            return equivObjPropExprs;
        }

        public static bool CheckAreDisjointObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression leftObjPropExpr, OWLObjectPropertyExpression rightObjPropExpr)
            => ontology != null && leftObjPropExpr != null && rightObjPropExpr != null && GetDisjointObjectProperties(ontology, leftObjPropExpr).Any(opex => opex.GetIRI().Equals(rightObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetDisjointObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
        {
            List<OWLObjectPropertyExpression> disjointObjPropExprs = [];

            if (ontology != null && objPropExpr != null)
            {
                RDFResource objPropExprIRI = objPropExpr.GetIRI();

                //There is no reasoning on object property disjointness (apart simmetry), being this totally under OWA domain
                foreach (OWLDisjointObjectProperties axiom in GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>(ontology).Where(ax => ax.ObjectPropertyExpressions.Any(dp => dp.GetIRI().Equals(objPropExprIRI))))
                    disjointObjPropExprs.AddRange(axiom.ObjectPropertyExpressions);

                disjointObjPropExprs.RemoveAll(res => res.GetIRI().Equals(objPropExprIRI));
            }

            return OWLExpressionHelper.RemoveDuplicates(disjointObjPropExprs);
        }

        public static bool CheckHasFunctionalObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLFunctionalObjectProperty>(ontology).Any(fop => fop.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));

        public static bool CheckHasInverseFunctionalObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLInverseFunctionalObjectProperty>(ontology).Any(ifop => ifop.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));

        public static bool CheckHasSymmetricObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLSymmetricObjectProperty>(ontology).Any(sop => sop.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));

        public static bool CheckHasAsymmetricObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLAsymmetricObjectProperty>(ontology).Any(asop => asop.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));

        public static bool CheckHasReflexiveObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLReflexiveObjectProperty>(ontology).Any(rop => rop.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));

        public static bool CheckHasIrreflexiveObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLIrreflexiveObjectProperty>(ontology).Any(irop => irop.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));

        public static bool CheckHasTransitiveObjectProperty(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr)
            => ontology != null && objPropExpr != null && GetObjectPropertyAxiomsOfType<OWLTransitiveObjectProperty>(ontology).Any(top => top.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()));
        #endregion
    }
}