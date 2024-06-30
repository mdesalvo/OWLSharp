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
	public static class OWLObjectPropertyAxiomHelper
	{
		#region Methods
		public static List<T> GetObjectPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLObjectPropertyAxiom
            => ontology?.ObjectPropertyAxioms.OfType<T>().ToList() ?? new List<T>();
		
		public static bool CheckIsSubObjectPropertyOf(this OWLOntology ontology, OWLObjectPropertyExpression subObjPropExpr, OWLObjectPropertyExpression superObjPropExpr, bool directOnly=false)
            => ontology != null && subObjPropExpr != null && superObjPropExpr != null && GetSubObjectPropertiesOf(ontology, superObjPropExpr, directOnly).Any(opex => opex.GetIRI().Equals(subObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetSubObjectPropertiesOf(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindSubObjectPropertiesOf(RDFResource objPropExprIRI, List<OWLSubObjectPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundSubObjPropExprs = new List<OWLObjectPropertyExpression>();

                #region VisitContext
                if (!visitContext.Contains(objPropExprIRI.PatternMemberID))
                    visitContext.Add(objPropExprIRI.PatternMemberID);
                else
                    return foundSubObjPropExprs;
                #endregion

				#region Discovery
				foreach (OWLSubObjectPropertyOf axiom in axioms.Where(ax => ax.SuperObjectPropertyExpression.GetIRI().Equals(objPropExprIRI)
																			  && ax.SubObjectPropertyExpression != null))
					foundSubObjPropExprs.Add(axiom.SubObjectPropertyExpression);

				if (!directOnly)
                {
					//SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                    foreach (OWLObjectPropertyExpression subObjPropExpr in foundSubObjPropExprs.ToList())
                        foundSubObjPropExprs.AddRange(FindSubObjectPropertiesOf(subObjPropExpr.GetIRI(), axioms, visitContext));
                }
				#endregion

                return foundSubObjPropExprs;
            }
            #endregion

            List<OWLObjectPropertyExpression> subObjPropExprs = new List<OWLObjectPropertyExpression>();
            if (ontology != null && objPropExpr != null)
			{
				RDFResource objPropIRI = objPropExpr.GetIRI();
				subObjPropExprs.AddRange(FindSubObjectPropertiesOf(objPropIRI, GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
                    foreach (OWLObjectPropertyExpression subObjPropExpr in subObjPropExprs.ToList())
						subObjPropExprs.AddRange(GetEquivalentObjectProperties(ontology, subObjPropExpr, directOnly));
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(subObjPropExprs);
        }

		public static bool CheckIsSuperObjectPropertyOf(this OWLOntology ontology, OWLObjectPropertyExpression superObjPropExpr, OWLObjectPropertyExpression subObjPropExpr, bool directOnly=false)
            => ontology != null && superObjPropExpr != null && subObjPropExpr != null && GetSuperObjectPropertiesOf(ontology, subObjPropExpr, directOnly).Any(opex => opex.GetIRI().Equals(superObjPropExpr.GetIRI()));

		public static List<OWLObjectPropertyExpression> GetSuperObjectPropertiesOf(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindSuperObjectPropertiesOf(RDFResource objPropExprIRI, List<OWLSubObjectPropertyOf> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundSuperObjPropExprs = new List<OWLObjectPropertyExpression>();

                #region VisitContext
                if (!visitContext.Contains(objPropExprIRI.PatternMemberID))
                    visitContext.Add(objPropExprIRI.PatternMemberID);
                else
                    return foundSuperObjPropExprs;
                #endregion

				#region Discovery
				foreach (OWLSubObjectPropertyOf axiom in axioms.Where(ax => ax.SubObjectPropertyExpression.GetIRI().Equals(objPropExprIRI)))
                    foundSuperObjPropExprs.Add(axiom.SuperObjectPropertyExpression);

				if (!directOnly)
                {
					//SubObjectPropertyOf(P1,P2) ^ SubObjectPropertyOf(P2,P3) -> SubObjectPropertyOf(P1,P3)
                    foreach (OWLObjectPropertyExpression superObjPropExpr in foundSuperObjPropExprs.ToList())
                        foundSuperObjPropExprs.AddRange(FindSuperObjectPropertiesOf(superObjPropExpr.GetIRI(), axioms, visitContext));
                }
				#endregion

                return foundSuperObjPropExprs;
            }
            #endregion

            List<OWLObjectPropertyExpression> superObjPropExprs = new List<OWLObjectPropertyExpression>();
            if (ontology != null && objPropExpr != null)
			{
				RDFResource objPropExprIRI = objPropExpr.GetIRI();
				superObjPropExprs.AddRange(FindSuperObjectPropertiesOf(objPropExprIRI, GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//SubObjectPropertyOf(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> SubObjectPropertyOf(P1,P3)
                    foreach (OWLObjectPropertyExpression superObjPropExpr in superObjPropExprs.ToList())
						superObjPropExprs.AddRange(GetEquivalentObjectProperties(ontology, superObjPropExpr, directOnly));
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(superObjPropExprs);
        }

		public static bool CheckAreEquivalentObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression leftObjPropExpr, OWLObjectPropertyExpression rightObjPropExpr, bool directOnly=false)
            => ontology != null && leftObjPropExpr != null && rightObjPropExpr != null && GetEquivalentObjectProperties(ontology, leftObjPropExpr, directOnly).Any(oex => oex.GetIRI().Equals(rightObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetEquivalentObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindEquivalentObjectProperties(RDFResource objPropExprIRI, List<OWLEquivalentObjectProperties> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundEquivObjPropExprs = new List<OWLObjectPropertyExpression>();

                #region VisitContext
                if (!visitContext.Contains(objPropExprIRI.PatternMemberID))
                    visitContext.Add(objPropExprIRI.PatternMemberID);
                else
                    return foundEquivObjPropExprs;
                #endregion

				#region Discovery
				foreach (OWLEquivalentObjectProperties axiom in axioms.Where(ax => ax.ObjectPropertyExpressions.Any(opex => opex.GetIRI().Equals(objPropExprIRI))))
                    foundEquivObjPropExprs.AddRange(axiom.ObjectPropertyExpressions);

				if (!directOnly)
                {
					//EquivalentObjectProperties(P1,P2) ^ EquivalentObjectProperties(P2,P3) -> EquivalentObjectProperties(P1,P3)
                    foreach (OWLObjectPropertyExpression equivObjPropExpr in foundEquivObjPropExprs.ToList())
                        foundEquivObjPropExprs.AddRange(FindEquivalentObjectProperties(equivObjPropExpr.GetIRI(), axioms, visitContext));
                }
				#endregion

				foundEquivObjPropExprs.RemoveAll(res => res.GetIRI().Equals(objPropExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundEquivObjPropExprs);
            }
            #endregion

            List<OWLObjectPropertyExpression> equivObjPropExprs = new List<OWLObjectPropertyExpression>();
            if (ontology != null && objPropExpr != null)
                equivObjPropExprs.AddRange(FindEquivalentObjectProperties(objPropExpr.GetIRI(), GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>(ontology), new HashSet<long>()));
            return equivObjPropExprs;
        }

        public static bool CheckAreDisjointObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression leftObjPropExpr, OWLObjectPropertyExpression rightObjPropExpr, bool directOnly = false)
            => ontology != null && leftObjPropExpr != null && rightObjPropExpr != null && GetDisjointObjectProperties(ontology, leftObjPropExpr, directOnly).Any(opex => opex.GetIRI().Equals(rightObjPropExpr.GetIRI()));

        public static List<OWLObjectPropertyExpression> GetDisjointObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLObjectPropertyExpression> FindDisjointObjectProperties(RDFResource objPropExprIRI, List<OWLDisjointObjectProperties> axioms, HashSet<long> visitContext)
            {
                List<OWLObjectPropertyExpression> foundDisjObjPropExprs = new List<OWLObjectPropertyExpression>();

                #region VisitContext
                if (!visitContext.Contains(objPropExprIRI.PatternMemberID))
                    visitContext.Add(objPropExprIRI.PatternMemberID);
                else
                    return foundDisjObjPropExprs;
                #endregion

                #region Discovery
                foreach (OWLDisjointObjectProperties axiom in axioms.Where(ax => ax.ObjectPropertyExpressions.Any(opex => opex.GetIRI().Equals(objPropExprIRI))))
                    foundDisjObjPropExprs.AddRange(axiom.ObjectPropertyExpressions);
                #endregion

                foundDisjObjPropExprs.RemoveAll(res => res.GetIRI().Equals(objPropExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundDisjObjPropExprs);
            }
            #endregion

            List<OWLObjectPropertyExpression> disjObjPropExprs = new List<OWLObjectPropertyExpression>();
            if (ontology != null && objPropExpr != null)
            {
                HashSet<long> visitContext = new HashSet<long>();
                List<OWLDisjointObjectProperties> disjointObjectPropertyAxioms = GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>(ontology);
                disjObjPropExprs.AddRange(FindDisjointObjectProperties(objPropExpr.GetIRI(), disjointObjectPropertyAxioms, visitContext));

                if (!directOnly)
                {
                    //EquivalentObjectProperties(P1,P2) ^ DisjointObjectProperties(P2,P3) -> DisjointObjectProperties(P1,P3)
                    foreach (OWLObjectPropertyExpression equivObjPropExpr in GetEquivalentObjectProperties(ontology, objPropExpr, directOnly))
                        disjObjPropExprs.AddRange(FindDisjointObjectProperties(equivObjPropExpr.GetIRI(), disjointObjectPropertyAxioms, visitContext));

                    //SubObjectPropertyOf(P1,P2) ^ DisjointObjectProperties(P2,P3) -> DisjointObjectProperties(P1,P3)
                    foreach (OWLObjectPropertyExpression superObjPropExpr in GetSuperObjectPropertiesOf(ontology, objPropExpr, directOnly))
                        disjObjPropExprs.AddRange(FindDisjointObjectProperties(superObjPropExpr.GetIRI(), disjointObjectPropertyAxioms, visitContext));
                }
            }
            return OWLExpressionHelper.RemoveDuplicates(disjObjPropExprs);
        }

		public static bool CheckAreInverseObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression leftObjPropExpr, OWLObjectPropertyExpression rightObjPropExpr, bool directOnly=false)
            => ontology != null && leftObjPropExpr != null && rightObjPropExpr != null && GetInverseObjectProperties(ontology, leftObjPropExpr, directOnly).Any(oex => oex.GetIRI().Equals(rightObjPropExpr.GetIRI()));

		public static List<OWLObjectPropertyExpression> GetInverseObjectProperties(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, bool directOnly=false)
        {
			List<OWLObjectPropertyExpression> invObjPropExprs = new List<OWLObjectPropertyExpression>();
			if (ontology != null && objPropExpr != null)
            {
				RDFResource objPropExprIRI = objPropExpr.GetIRI();
				List<OWLInverseObjectProperties> invObjProps = ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>();
            
				if (objPropExpr is OWLObjectInverseOf objInvOfPropExpr)
					invObjPropExprs.Add(objInvOfPropExpr.ObjectProperty);
				invObjPropExprs.AddRange(invObjProps.Where(ax => ax.LeftObjectPropertyExpression.GetIRI().Equals(objPropExprIRI))
													.Select(ax => ax.RightObjectPropertyExpression));
				invObjPropExprs.AddRange(invObjProps.Where(ax => ax.RightObjectPropertyExpression.GetIRI().Equals(objPropExprIRI))
												    .Select(ax => ax.LeftObjectPropertyExpression));
				
				if (!directOnly)
				{
					//InverseObjectProperties(OP,IOP) ^ EquivalentObjectProperties(IOP,OP2) -> InverseObjectProperties(OP,OP2)
					foreach (OWLObjectPropertyExpression iop in invObjPropExprs)
					{
						RDFResource iopIRI = iop.GetIRI();
						List<OWLObjectPropertyExpression> equivsIOP = ontology.GetEquivalentObjectProperties(iop, directOnly);
						equivsIOP.RemoveAll(opex => opex.GetIRI().Equals(iopIRI));

						foreach (OWLObjectPropertyExpression equivIOP in equivsIOP)
							invObjPropExprs.Add(equivIOP);						
					}
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(invObjPropExprs);
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