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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Ontology.Axioms
{
    public static class OWLClassAxiomHelper 
    {
		#region Methods
        public static List<T> GetClassAxiomsOfType<T>(this OWLOntology ontology) where T : OWLClassAxiom
            => ontology?.ClassAxioms.OfType<T>().ToList() ?? new List<T>();

        public static bool CheckIsSubClassOf(this OWLOntology ontology, OWLClassExpression subClassExpr, OWLClassExpression superClassExpr, bool directOnly=false)
            => ontology != null && subClassExpr != null && superClassExpr != null && GetSubClassesOf(ontology, superClassExpr, directOnly).Any(cex => cex.GetIRI().Equals(subClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetSubClassesOf(this OWLOntology ontology, OWLClassExpression classExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLClassExpression> FindSubClassesOf(RDFResource classExprIRI, List<OWLSubClassOf> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundSubClasses = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundSubClasses;
                #endregion

				#region Discovery
				foreach (OWLSubClassOf axiom in axioms.Where(ax => ax.SuperClassExpression.GetIRI().Equals(classExprIRI)))
                    foundSubClasses.Add(axiom.SubClassExpression);

				if (!directOnly)
                {
					//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
                    foreach (OWLClassExpression subClass in foundSubClasses.ToList())
                        foundSubClasses.AddRange(FindSubClassesOf(subClass.GetIRI(), axioms, visitContext));
                }
				#endregion

                return foundSubClasses;
            }
            #endregion

            List<OWLClassExpression> subClasses = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				RDFResource clsExprIRI = classExpr.GetIRI();
				subClasses.AddRange(FindSubClassesOf(clsExprIRI, GetClassAxiomsOfType<OWLSubClassOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//SubClassOf(C1,C2) ^ EquivalentClass(C2,C3) -> SubClassOf(C1,C3)
                    foreach (OWLClassExpression subClass in subClasses.ToList())
						subClasses.AddRange(GetEquivalentClasses(ontology, subClass, directOnly));

					//DisjointUnionOf(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
                    foreach (OWLDisjointUnion disjointUnion in GetClassAxiomsOfType<OWLDisjointUnion>(ontology).Where(ax => ax.ClassIRI.GetIRI().Equals(clsExprIRI)))
                        subClasses.AddRange(disjointUnion.ClassExpressions);

					//EquivalentClass(C1,ObjectUnionOf(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
					foreach (OWLObjectUnionOf objectUnion in GetEquivalentClasses(ontology, classExpr, directOnly).OfType<OWLObjectUnionOf>())
						subClasses.AddRange(objectUnion.ClassExpressions);
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(subClasses);
        }

		public static bool CheckIsSuperClassOf(this OWLOntology ontology, OWLClassExpression superClassExpr, OWLClassExpression subClassExpr, bool directOnly=false)
            => ontology != null && superClassExpr != null && subClassExpr != null && GetSuperClassesOf(ontology, subClassExpr, directOnly).Any(cex => cex.GetIRI().Equals(superClassExpr.GetIRI()));

		public static List<OWLClassExpression> GetSuperClassesOf(this OWLOntology ontology, OWLClassExpression classExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLClassExpression> FindSuperClassesOf(RDFResource classExprIRI, List<OWLSubClassOf> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundSuperClasses = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundSuperClasses;
                #endregion

				#region Discovery
				foreach (OWLSubClassOf axiom in axioms.Where(ax => ax.SubClassExpression.GetIRI().Equals(classExprIRI)))
                    foundSuperClasses.Add(axiom.SuperClassExpression);

				if (!directOnly)
                {
					//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
                    foreach (OWLClassExpression superClass in foundSuperClasses.ToList())
                        foundSuperClasses.AddRange(FindSuperClassesOf(superClass.GetIRI(), axioms, visitContext));
                }
				#endregion

                return foundSuperClasses;
            }
            #endregion

            List<OWLClassExpression> superClasses = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				RDFResource clsExprIRI = classExpr.GetIRI();
				superClasses.AddRange(FindSuperClassesOf(clsExprIRI, GetClassAxiomsOfType<OWLSubClassOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//SubClassOf(C1,C2) ^ EquivalentClass(C2,C3) -> SubClassOf(C1,C3)
                    foreach (OWLClassExpression superClass in superClasses.ToList())
						superClasses.AddRange(GetEquivalentClasses(ontology, superClass, directOnly));

					//DisjointUnionOf(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
                    foreach (OWLDisjointUnion disjointUnion in GetClassAxiomsOfType<OWLDisjointUnion>(ontology).Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(clsExprIRI))))
						superClasses.Add(disjointUnion.ClassIRI);
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(superClasses);
        }

        public static bool CheckAreEquivalentClasses(this OWLOntology ontology, OWLClassExpression leftClassExpr, OWLClassExpression rightClassExpr, bool directOnly=false)
            => ontology != null && leftClassExpr != null && rightClassExpr != null && GetEquivalentClasses(ontology, leftClassExpr, directOnly).Any(cex => cex.GetIRI().Equals(rightClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetEquivalentClasses(this OWLOntology ontology, OWLClassExpression classExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLClassExpression> FindEquivalentClasses(RDFResource classExprIRI, List<OWLEquivalentClasses> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundEquivalentClasses = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundEquivalentClasses;
                #endregion

				#region Discovery
				foreach (OWLEquivalentClasses axiom in axioms.Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(classExprIRI))))
                    foundEquivalentClasses.AddRange(axiom.ClassExpressions);

				if (!directOnly)
                {
					//EquivalentClass(C1,C2) ^ EquivalentClass(C2,C3) -> EquivalentClass(C1,C3)
                    foreach (OWLClassExpression equivalentClass in foundEquivalentClasses.ToList())
                        foundEquivalentClasses.AddRange(FindEquivalentClasses(equivalentClass.GetIRI(), axioms, visitContext));
                }
				#endregion

				foundEquivalentClasses.RemoveAll(res => res.GetIRI().Equals(classExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundEquivalentClasses);
            }
            #endregion

            List<OWLClassExpression> equivalentClasses = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
                equivalentClasses.AddRange(FindEquivalentClasses(classExpr.GetIRI(), GetClassAxiomsOfType<OWLEquivalentClasses>(ontology), new HashSet<long>()));
            return equivalentClasses;
        }

        public static bool CheckAreDisjointClasses(this OWLOntology ontology, OWLClassExpression leftClassExpr, OWLClassExpression rightClassExpr, bool directOnly=false)
            => ontology != null && leftClassExpr != null && rightClassExpr != null && GetDisjointClasses(ontology, leftClassExpr, directOnly).Any(cex => cex.GetIRI().Equals(rightClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetDisjointClasses(this OWLOntology ontology, OWLClassExpression classExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLClassExpression> FindDisjointClasses(RDFResource classExprIRI, List<OWLDisjointClasses> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> foundDisjointClasses = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return foundDisjointClasses;
                #endregion

                #region Discovery
                foreach (OWLDisjointClasses axiom in axioms.Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(classExprIRI))))
                    foundDisjointClasses.AddRange(axiom.ClassExpressions);
                #endregion

                foundDisjointClasses.RemoveAll(res => res.GetIRI().Equals(classExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundDisjointClasses);
            }
            #endregion

            List<OWLClassExpression> disjointClasses = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				HashSet<long> visitContext = new HashSet<long>();
				List<OWLDisjointClasses> disjointClassAxioms = GetClassAxiomsOfType<OWLDisjointClasses>(ontology);
				disjointClasses.AddRange(FindDisjointClasses(classExpr.GetIRI(), disjointClassAxioms, visitContext));

				if (!directOnly)
				{
					//SubClassOf(C1,C2) ^ DisjointWith(C2,C3) -> DisjointWith(C1,C3)
					foreach (OWLClassExpression superClass in GetSuperClassesOf(ontology, classExpr, directOnly))
						disjointClasses.AddRange(FindDisjointClasses(superClass.GetIRI(), disjointClassAxioms, visitContext));
				}
			}
            return OWLExpressionHelper.RemoveDuplicates(disjointClasses);
        }
        #endregion
    }
}