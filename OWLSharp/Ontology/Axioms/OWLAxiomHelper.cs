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
    public static class OWLAxiomHelper 
    {
        #region Declarations
        public static List<OWLDeclaration> GetDeclarationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLExpression, IOWLEntity
            => ontology?.DeclarationAxioms.Where(ax => ax.Expression is T).ToList() ?? new List<OWLDeclaration>();
        #endregion

        #region ClassAxioms
        public static List<T> GetClassAxiomsOfType<T>(this OWLOntology ontology) where T : OWLClassAxiom
            => ontology?.ClassAxioms.OfType<T>().ToList() ?? new List<T>();

        public static bool CheckIsSubClassOf(this OWLOntology ontology, OWLClassExpression subClassExpr, OWLClassExpression superClassExpr, bool directOnly=false)
            => ontology != null && subClassExpr != null && superClassExpr != null && GetSubClassesOf(ontology, superClassExpr, directOnly).Any(cex => cex.GetIRI().Equals(subClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetSubClassesOf(this OWLOntology ontology, OWLClassExpression classExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLClassExpression> FindSubClassesOf(RDFResource classExprIRI, List<OWLSubClassOf> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> subResults = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return subResults;
                #endregion

				#region Discovery
				foreach (OWLSubClassOf axiom in axioms.Where(ax => ax.SuperClassExpression.GetIRI().Equals(classExprIRI)))
                    subResults.Add(axiom.SubClassExpression);

				if (!directOnly)
                {
					//SubClassOf(C1,C2) ^ SubClassOf(C2,C3) -> SubClassOf(C1,C3)
                    foreach (OWLClassExpression subClass in subResults.ToList())
                        subResults.AddRange(FindSubClassesOf(subClass.GetIRI(), axioms, visitContext));
                }
				#endregion

                return subResults;
            }
            #endregion

            List<OWLClassExpression> results = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
			{
				results.AddRange(FindSubClassesOf(classExpr.GetIRI(), GetClassAxiomsOfType<OWLSubClassOf>(ontology), new HashSet<long>()));

				if (!directOnly)
				{
					//DisjointUnionOf(C1,(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
                    foreach (OWLDisjointUnion disjointUnion in GetClassAxiomsOfType<OWLDisjointUnion>(ontology).Where(ax => ax.ClassIRI.GetIRI().Equals(classExpr.GetIRI())))
                        results.AddRange(disjointUnion.ClassExpressions);

					//EquivalentClass(C1,ObjectUnionOf(C2 C3)) -> SubClassOf(C2,C1) ^ SubClassOf(C3,C1)
					foreach (OWLObjectUnionOf objectUnion in GetEquivalentClasses(ontology, classExpr, directOnly).OfType<OWLObjectUnionOf>())
						results.AddRange(objectUnion.ClassExpressions);

                    //SubClassOf(C1,C2) ^ EquivalentClass(C2,C3) -> SubClassOf(C1,C3)
                    foreach (OWLClassExpression result in results.ToList())
						results.AddRange(GetEquivalentClasses(ontology, result, directOnly));
				}
			}
            return results;
        }

        public static bool CheckAreEquivalentClasses(this OWLOntology ontology, OWLClassExpression leftClassExpr, OWLClassExpression rightClassExpr, bool directOnly=false)
            => ontology != null && leftClassExpr != null && rightClassExpr != null && GetEquivalentClasses(ontology, leftClassExpr, directOnly).Any(cex => cex.GetIRI().Equals(rightClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetEquivalentClasses(this OWLOntology ontology, OWLClassExpression classExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLClassExpression> FindEquivalentClasses(RDFResource classExprIRI, List<OWLEquivalentClasses> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> subResults = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return subResults;
                #endregion

				#region Discovery
				foreach (OWLEquivalentClasses axiom in axioms.Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(classExprIRI))))
                    subResults.AddRange(axiom.ClassExpressions);

				if (!directOnly)
                {
					//EquivalentClass(C1,C2) ^ EquivalentClass(C2,C3) -> EquivalentClass(C1,C3)
                    foreach (OWLClassExpression equivalentClass in subResults.ToList())
                        subResults.AddRange(FindEquivalentClasses(equivalentClass.GetIRI(), axioms, visitContext));
                }
				#endregion

				subResults.RemoveAll(res => res.GetIRI().Equals(classExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(subResults);
            }
            #endregion

            List<OWLClassExpression> results = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
                results.AddRange(FindEquivalentClasses(classExpr.GetIRI(), GetClassAxiomsOfType<OWLEquivalentClasses>(ontology), new HashSet<long>()));
            return results;
        }

        public static bool CheckAreDisjointClasses(this OWLOntology ontology, OWLClassExpression leftClassExpr, OWLClassExpression rightClassExpr)
            => ontology != null && leftClassExpr != null && rightClassExpr != null && GetDisjointClasses(ontology, leftClassExpr).Any(cex => cex.GetIRI().Equals(rightClassExpr.GetIRI()));

        public static List<OWLClassExpression> GetDisjointClasses(this OWLOntology ontology, OWLClassExpression classExpr)
        {
            #region Utilities
            List<OWLClassExpression> FindDisjointClasses(RDFResource classExprIRI, List<OWLDisjointClasses> axioms, HashSet<long> visitContext)
            {
                List<OWLClassExpression> subResults = new List<OWLClassExpression>();

                #region VisitContext
                if (!visitContext.Contains(classExprIRI.PatternMemberID))
                    visitContext.Add(classExprIRI.PatternMemberID);
                else
                    return subResults;
                #endregion

                #region Discovery
                foreach (OWLDisjointClasses axiom in axioms.Where(ax => ax.ClassExpressions.Any(cex => cex.GetIRI().Equals(classExprIRI))))
                    subResults.AddRange(axiom.ClassExpressions);
                #endregion

                subResults.RemoveAll(res => res.GetIRI().Equals(classExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(subResults);
            }
            #endregion

            List<OWLClassExpression> results = new List<OWLClassExpression>();
            if (ontology != null && classExpr != null)
                results.AddRange(FindDisjointClasses(classExpr.GetIRI(), GetClassAxiomsOfType<OWLDisjointClasses>(ontology), new HashSet<long>()));
            return results;
        }
        #endregion

        #region DataPropertyAxioms
        public static List<T> GetDataPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLDataPropertyAxiom
            => ontology?.DataPropertyAxioms.OfType<T>().ToList() ?? new List<T>();
        #endregion

        #region ObjectPropertyAxioms
        public static List<T> GetObjectPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLObjectPropertyAxiom
            => ontology?.ObjectPropertyAxioms.OfType<T>().ToList() ?? new List<T>();
        #endregion

        #region AnnotationAxioms
        public static List<T> GetAnnotationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAnnotationAxiom
            => ontology?.AnnotationAxioms.OfType<T>().ToList() ?? new List<T>();
        #endregion

        #region AssertionAxioms
        public static List<T> GetAssertionAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAssertionAxiom
            => ontology?.AssertionAxioms.OfType<T>().ToList() ?? new List<T>();
        #endregion
    }
}