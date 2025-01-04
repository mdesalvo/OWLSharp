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

using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Ontology
{
    public static class OWLClassAxiomHelper 
    {
		#region Methods
        public static List<T> GetClassAxiomsOfType<T>(this OWLOntology ontology) where T : OWLClassAxiom
            => ontology?.ClassAxioms.OfType<T>().ToList() ?? new List<T>();

        public static bool CheckHasClassAxiom<T>(this OWLOntology ontology, T classAxiom) where T : OWLClassAxiom
            => GetClassAxiomsOfType<T>(ontology).Any(ax => string.Equals(ax.GetXML(), classAxiom?.GetXML()));

        public static void DeclareClassAxiom<T>(this OWLOntology ontology, T classAxiom) where T : OWLClassAxiom
        {
            #region Guards
            if (classAxiom == null)
                throw new OWLException("Cannot declare class axiom because given \"classAxiom\" parameter is null");
            #endregion

            if (!CheckHasClassAxiom(ontology, classAxiom))
                ontology?.ClassAxioms.Add(classAxiom);
        }

        public static bool CheckIsSubClassOf(this OWLOntology ontology, OWLClassExpression subClassExpr, OWLClassExpression superClassExpr)
            => ontology != null && subClassExpr != null && superClassExpr != null && GetSubClassesOf(ontology, superClassExpr).Any(cex => cex.GetIRI().Equals(subClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetSubClassesOf(this OWLOntology ontology, OWLClassExpression classExpr)
        {
            #region Utilities
            List<OWLClassExpression> FindSubClassesOf(RDFResource classExprIRI, List<OWLSubClassOf> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundSubClassExprs = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundSubClassExprs;
                #endregion

				#region Discovery
				foreach (OWLSubClassOf axiom in axioms.Where(ax => ax.SuperClassExpression.GetIRI().Equals(classExprIRI)))
                    foundSubClassExprs.Add(axiom.SubClassExpression);

				//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
				foreach (OWLClassExpression subClassExpr in foundSubClassExprs.ToList())
					foundSubClassExprs.AddRange(FindSubClassesOf(subClassExpr.GetIRI(), axioms, visitContext));
				#endregion

                return foundSubClassExprs;
            }
            #endregion

            List<OWLClassExpression> subClassExprs = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				RDFResource clsExprIRI = classExpr.GetIRI();
				HashSet<long> visitContext = new HashSet<long>();
				List<OWLSubClassOf> subClassOfAxs = GetClassAxiomsOfType<OWLSubClassOf>(ontology);
				List<OWLClassExpression> equivClassesOfClassExpr = GetEquivalentClasses(ontology, classExpr);

				//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
				subClassExprs.AddRange(FindSubClassesOf(clsExprIRI, subClassOfAxs, visitContext));

				//EquivalentClasses(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
				foreach (OWLClassExpression equivClassExpr in equivClassesOfClassExpr)
					subClassExprs.AddRange(FindSubClassesOf(equivClassExpr.GetIRI(), subClassOfAxs, visitContext));

				//SubClassOf(C1,C2) ^ EquivalentClasses(C2,C3) -> SubClassOf(C1,C3)
				foreach (OWLClassExpression subClassExpr in subClassExprs.ToList())
					subClassExprs.AddRange(GetEquivalentClasses(ontology, subClassExpr));

				//DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
				foreach (OWLDisjointUnion disjointUnion in GetClassAxiomsOfType<OWLDisjointUnion>(ontology).Where(ax => ax.ClassIRI.GetIRI().Equals(clsExprIRI)))
					subClassExprs.AddRange(disjointUnion.ClassExpressions);

				//EquivalentClasses(C1,ObjectUnionOf(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
				foreach (OWLObjectUnionOf objectUnionOf in equivClassesOfClassExpr.OfType<OWLObjectUnionOf>())
					subClassExprs.AddRange(objectUnionOf.ClassExpressions);
			}
            return OWLExpressionHelper.RemoveDuplicates(subClassExprs);
        }

		public static bool CheckIsSuperClassOf(this OWLOntology ontology, OWLClassExpression superClassExpr, OWLClassExpression subClassExpr)
            => ontology != null && superClassExpr != null && subClassExpr != null && GetSuperClassesOf(ontology, subClassExpr).Any(cex => cex.GetIRI().Equals(superClassExpr.GetIRI()));

		public static List<OWLClassExpression> GetSuperClassesOf(this OWLOntology ontology, OWLClassExpression classExpr)
        {
            #region Utilities
            List<OWLClassExpression> FindSuperClassesOf(RDFResource classExprIRI, List<OWLSubClassOf> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundSuperClassExprs = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundSuperClassExprs;
                #endregion

				#region Discovery
				foreach (OWLSubClassOf axiom in axioms.Where(ax => ax.SubClassExpression.GetIRI().Equals(classExprIRI)))
                    foundSuperClassExprs.Add(axiom.SuperClassExpression);

				//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
				foreach (OWLClassExpression superClassExpr in foundSuperClassExprs.ToList())
					foundSuperClassExprs.AddRange(FindSuperClassesOf(superClassExpr.GetIRI(), axioms, visitContext));
				#endregion

                return foundSuperClassExprs;
            }
            #endregion

			List<OWLClassExpression> superClassExprs = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				RDFResource clsExprIRI = classExpr.GetIRI();
				HashSet<long> visitContext = new HashSet<long>();
				List<OWLSubClassOf> subClassOfAxs = GetClassAxiomsOfType<OWLSubClassOf>(ontology);
				
				//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
				superClassExprs.AddRange(FindSuperClassesOf(clsExprIRI, subClassOfAxs, visitContext));

				//EquivalentClasses(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
				foreach (OWLClassExpression equivClassExpr in GetEquivalentClasses(ontology, classExpr))
					superClassExprs.AddRange(FindSuperClassesOf(equivClassExpr.GetIRI(), subClassOfAxs, visitContext));

				//SubClassOf(C1,C2) ^ EquivalentClasses(C2,C3) -> SubClassOf(C1,C3)
				foreach (OWLClassExpression superClassExpr in superClassExprs.ToList())
					superClassExprs.AddRange(GetEquivalentClasses(ontology, superClassExpr));

				//DisjointUnion(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
				foreach (OWLDisjointUnion disjointUnion in GetClassAxiomsOfType<OWLDisjointUnion>(ontology).Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(clsExprIRI))))
					superClassExprs.Add(disjointUnion.ClassIRI);
			}
            return OWLExpressionHelper.RemoveDuplicates(superClassExprs);
        }

        public static bool CheckAreEquivalentClasses(this OWLOntology ontology, OWLClassExpression leftClassExpr, OWLClassExpression rightClassExpr)
            => ontology != null && leftClassExpr != null && rightClassExpr != null && GetEquivalentClasses(ontology, leftClassExpr).Any(cex => cex.GetIRI().Equals(rightClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetEquivalentClasses(this OWLOntology ontology, OWLClassExpression classExpr)
        {
            #region Utilities
            List<OWLClassExpression> FindEquivalentClasses(RDFResource classExprIRI, List<OWLEquivalentClasses> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundEquivClassExprs = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundEquivClassExprs;
                #endregion

				#region Discovery
				foreach (OWLEquivalentClasses axiom in axioms.Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(classExprIRI))))
                    foundEquivClassExprs.AddRange(axiom.ClassExpressions);

				//EquivalentClass(C1,C2) ^ EquivalentClass(C2,C3) -> EquivalentClass(C1,C3)
				foreach (OWLClassExpression equivClassExpr in foundEquivClassExprs.ToList())
					foundEquivClassExprs.AddRange(FindEquivalentClasses(equivClassExpr.GetIRI(), axioms, visitContext));
				#endregion

				foundEquivClassExprs.RemoveAll(res => res.GetIRI().Equals(classExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundEquivClassExprs);
            }
            #endregion

            List<OWLClassExpression> equivClassExprs = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
                equivClassExprs.AddRange(FindEquivalentClasses(classExpr.GetIRI(), GetClassAxiomsOfType<OWLEquivalentClasses>(ontology), new HashSet<long>()));
            return equivClassExprs;
        }

        public static bool CheckAreDisjointClasses(this OWLOntology ontology, OWLClassExpression leftClassExpr, OWLClassExpression rightClassExpr)
            => ontology != null && leftClassExpr != null && rightClassExpr != null && GetDisjointClasses(ontology, leftClassExpr).Any(cex => cex.GetIRI().Equals(rightClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetDisjointClasses(this OWLOntology ontology, OWLClassExpression classExpr)
        {
            #region Utilities
            List<OWLClassExpression> FindDisjointClasses(RDFResource classExprIRI, List<OWLDisjointClasses> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundDisjClassExprs = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundDisjClassExprs;
                #endregion

                #region Discovery
                foreach (OWLDisjointClasses axiom in axioms.Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(classExprIRI))))
                    foundDisjClassExprs.AddRange(axiom.ClassExpressions);
                #endregion

                foundDisjClassExprs.RemoveAll(res => res.GetIRI().Equals(classExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundDisjClassExprs);
            }
            #endregion

            List<OWLClassExpression> disjClassExprs = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				HashSet<long> visitContext = new HashSet<long>();
				List<OWLDisjointClasses> disjointClassAxioms = GetClassAxiomsOfType<OWLDisjointClasses>(ontology);
				disjClassExprs.AddRange(FindDisjointClasses(classExpr.GetIRI(), disjointClassAxioms, visitContext));

				//EquivalentClasses(C1,C2) ^ DisjointWith(C2,C3) -> DisjointWith(C1,C3)
				foreach (OWLClassExpression equivClassExpr in GetEquivalentClasses(ontology, classExpr))
					disjClassExprs.AddRange(FindDisjointClasses(equivClassExpr.GetIRI(), disjointClassAxioms, visitContext));

				//SubClassOf(C1,C2) ^ DisjointWith(C2,C3) -> DisjointWith(C1,C3)
				foreach (OWLClassExpression superClassExpr in GetSuperClassesOf(ontology, classExpr))
					disjClassExprs.AddRange(FindDisjointClasses(superClassExpr.GetIRI(), disjointClassAxioms, visitContext));
			}
            return OWLExpressionHelper.RemoveDuplicates(disjClassExprs);
        }
        #endregion
    }
}