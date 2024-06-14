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
	public static class OWLAssertionAxiomHelper
	{
		#region Methods
		public static List<T> GetAssertionAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAssertionAxiom
            => ontology?.AssertionAxioms.OfType<T>().ToList() ?? new List<T>();

		public static bool CheckIsSameIndividual(this OWLOntology ontology, OWLIndividualExpression leftIdvExpr, OWLIndividualExpression rightIdvExpr, bool directOnly=false)
            => ontology != null && leftIdvExpr != null && rightIdvExpr != null && GetSameIndividuals(ontology, leftIdvExpr, directOnly).Any(iex => iex.GetIRI().Equals(rightIdvExpr.GetIRI()));

        public static List<OWLIndividualExpression> GetSameIndividuals(this OWLOntology ontology, OWLIndividualExpression idvExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLIndividualExpression> FindSameIndividuals(RDFResource idvExprIRI, List<OWLSameIndividual> axioms, HashSet<long> visitContext)
            {
                List<OWLIndividualExpression> foundSameIndividuals = new List<OWLIndividualExpression>();

                #region VisitContext
                if (!visitContext.Contains(idvExprIRI.PatternMemberID))
                    visitContext.Add(idvExprIRI.PatternMemberID);
                else
                    return foundSameIndividuals;
                #endregion

				#region Discovery
				foreach (OWLSameIndividual axiom in axioms.Where(ax => ax.IndividualExpressions.Any(iex => iex.GetIRI().Equals(idvExprIRI))))
                    foundSameIndividuals.AddRange(axiom.IndividualExpressions);

				if (!directOnly)
                {
					//SameAs(I1,I2) ^ SameAs(I2,I3) -> SameAs(I1,I3)
                    foreach (OWLIndividualExpression sameIndividual in foundSameIndividuals.ToList())
                        foundSameIndividuals.AddRange(FindSameIndividuals(sameIndividual.GetIRI(), axioms, visitContext));
                }
				#endregion

				foundSameIndividuals.RemoveAll(res => res.GetIRI().Equals(idvExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundSameIndividuals);
            }
            #endregion

            List<OWLIndividualExpression> sameIndividuals = new List<OWLIndividualExpression>();
            if (ontology != null && idvExpr != null)
                sameIndividuals.AddRange(FindSameIndividuals(idvExpr.GetIRI(), GetAssertionAxiomsOfType<OWLSameIndividual>(ontology), new HashSet<long>()));
            return sameIndividuals;
        }

        public static bool CheckAreDifferentIndividuals(this OWLOntology ontology, OWLIndividualExpression leftIdvExpr, OWLIndividualExpression rightIdvExpr, bool directOnly=false)
            => ontology != null && leftIdvExpr != null && rightIdvExpr != null && GetDifferentIndividuals(ontology, leftIdvExpr, directOnly).Any(iex => iex.GetIRI().Equals(rightIdvExpr.GetIRI()));

        public static List<OWLIndividualExpression> GetDifferentIndividuals(this OWLOntology ontology, OWLIndividualExpression idvExpr, bool directOnly=false)
        {
            #region Utilities
            List<OWLIndividualExpression> FindDifferentIndividuals(RDFResource idvExprIRI, List<OWLDifferentIndividuals> axioms, HashSet<long> visitContext)
            {
                List<OWLIndividualExpression> foundDifferentIndividuals = new List<OWLIndividualExpression>();

                #region VisitContext
                if (!visitContext.Contains(idvExprIRI.PatternMemberID))
                    visitContext.Add(idvExprIRI.PatternMemberID);
                else
                    return foundDifferentIndividuals;
                #endregion

                #region Discovery
                foreach (OWLDifferentIndividuals axiom in axioms.Where(ax => ax.IndividualExpressions.Any(iex => iex.GetIRI().Equals(idvExprIRI))))
                    foundDifferentIndividuals.AddRange(axiom.IndividualExpressions);
                #endregion

                foundDifferentIndividuals.RemoveAll(res => res.GetIRI().Equals(idvExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundDifferentIndividuals);
            }
            #endregion

            List<OWLIndividualExpression> differentIndividuals = new List<OWLIndividualExpression>();
            if (ontology != null && idvExpr != null)
                differentIndividuals.AddRange(FindDifferentIndividuals(idvExpr.GetIRI(), GetAssertionAxiomsOfType<OWLDifferentIndividuals>(ontology), new HashSet<long>()));
            return differentIndividuals;
        }

        public static List<OWLDataPropertyAssertion> GetDataPropertyAssertions(this OWLOntology ontology, OWLDataProperty dataProperty, OWLIndividualExpression idvExpr, OWLLiteral literal)
            => GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology).Where(ax =>
                   ((dataProperty == null || ax.DataProperty.GetIRI().Equals(dataProperty.GetIRI()))
                      && (idvExpr == null || ax.IndividualExpression.GetIRI().Equals(idvExpr.GetIRI())) 
                      && (literal == null || ax.Literal.GetLiteral().Equals(literal.GetLiteral())))).ToList();

        public static List<OWLNegativeDataPropertyAssertion> GetNegativeDataPropertyAssertions(this OWLOntology ontology, OWLDataProperty dataProperty, OWLIndividualExpression idvExpr, OWLLiteral literal)
            => GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>(ontology).Where(ax =>
                   ((dataProperty == null || ax.DataProperty.GetIRI().Equals(dataProperty.GetIRI()))
                      && (idvExpr == null || ax.IndividualExpression.GetIRI().Equals(idvExpr.GetIRI()))
                      && (literal == null || ax.Literal.GetLiteral().Equals(literal.GetLiteral())))).ToList();

        public static List<OWLObjectPropertyAssertion> GetObjectPropertyAssertions(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, OWLIndividualExpression sourceIdvExpr, OWLIndividualExpression targetIdvExpr)
            => GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>(ontology).Where(ax =>
                    ((objPropExpr == null || ax.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()))
                     && (sourceIdvExpr == null || ax.SourceIndividualExpression.GetIRI().Equals(sourceIdvExpr.GetIRI()))
                     && (targetIdvExpr == null || ax.TargetIndividualExpression.GetIRI().Equals(targetIdvExpr.GetIRI())))).ToList();

        public static List<OWLNegativeObjectPropertyAssertion> GetNegativeObjectPropertyAssertions(this OWLOntology ontology, OWLObjectPropertyExpression objPropExpr, OWLIndividualExpression sourceIdvExpr, OWLIndividualExpression targetIdvExpr)
            => GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>(ontology).Where(ax =>
                    ((objPropExpr == null || ax.ObjectPropertyExpression.GetIRI().Equals(objPropExpr.GetIRI()))
                     && (sourceIdvExpr == null || ax.SourceIndividualExpression.GetIRI().Equals(sourceIdvExpr.GetIRI()))
                     && (targetIdvExpr == null || ax.TargetIndividualExpression.GetIRI().Equals(targetIdvExpr.GetIRI())))).ToList();
        
        public static List<OWLIndividualExpression> GetIndividualsOf(this OWLOntology ontology, OWLClassExpression clsExpr, bool directOnly=false)
        {
            List<OWLIndividualExpression> clsExprIndividuals = new List<OWLIndividualExpression>();
            if (ontology != null && clsExpr != null)
            {
                //Direct
                clsExprIndividuals.AddRange(GetAssertionAxiomsOfType<OWLClassAssertion>(ontology).Where(ax => ax.ClassExpression.GetIRI().Equals(clsExpr.GetIRI()))
                                                                                                 .Select(ax => ax.IndividualExpression));
            
                //Indirect
                if (!directOnly)
                {
                    #region Indirect
                    if (clsExpr.IsObjectRestriction)
                    {
                        //TODO
                    }
                    else if (clsExpr.IsDataRestriction)
                    {
                        //TODO
                    }
                    else if (clsExpr.IsComposite)
                    {
                        //TODO
                    }
                    else if (clsExpr.IsEnumerate)
                        clsExprIndividuals.AddRange(((OWLObjectOneOf)clsExpr).IndividualExpressions);
                    else if (clsExpr.IsClass)
                    {
                        //TODO
                    }
                    #endregion
                }
            }
            return OWLExpressionHelper.RemoveDuplicates(clsExprIndividuals);
        }
        #endregion
    }
}