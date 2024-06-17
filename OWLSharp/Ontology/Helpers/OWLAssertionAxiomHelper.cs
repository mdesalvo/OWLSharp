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

        public static List<OWLIndividualExpression> GetIndividualsOf(this OWLOntology ontology, OWLClass owlClass, bool directOnly=false)
        {
			List<OWLClassAssertion> classAssertions = GetAssertionAxiomsOfType<OWLClassAssertion>(ontology);
			List<OWLObjectPropertyAssertion> objectPropertyAssertions = GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>(ontology);
			List<OWLDataPropertyAssertion> dataPropertyAssertions = GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology);

            #region Utilities
            List<OWLIndividualExpression> FindIndividualsOf(OWLClassExpression visitingClsExpr, HashSet<long> visitContext)
			{
				List<OWLIndividualExpression> foundVisitingClsExprIndividuals = new List<OWLIndividualExpression>();
				RDFResource visitingClsExprIRI = visitingClsExpr.GetIRI();

				#region VisitContext
				if (!visitContext.Contains(visitingClsExprIRI.PatternMemberID))
                    visitContext.Add(visitingClsExprIRI.PatternMemberID);
                else
                    return foundVisitingClsExprIndividuals;
                #endregion

				#region Discovery
				foundVisitingClsExprIndividuals.AddRange(classAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(visitingClsExprIRI))
																		.Select(ax => ax.IndividualExpression));
            
                if (!directOnly)
                {
					//Type(IDV,C1) ^ SubClassOf(C1,C2) -> Type(IDV,C2)
					foreach (OWLClassExpression subClsExpr in ontology.GetSubClassesOf(visitingClsExpr, directOnly))
						foundVisitingClsExprIndividuals.AddRange(classAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(subClsExpr.GetIRI()))
																				.Select(ax => ax.IndividualExpression));

					//Type(IDV,C1) ^ EquivalentClasses(C1,C2) -> Type(IDV,C2)
					foreach (OWLClassExpression equivClsExpr in ontology.GetEquivalentClasses(visitingClsExpr, directOnly))
					{
						RDFResource equivClsExprIRI = equivClsExpr.GetIRI();

						#region Class
						if (equivClsExpr.IsClass)
							foundVisitingClsExprIndividuals.AddRange(FindIndividualsOf(equivClsExpr, visitContext));
						#endregion
						
						#region Enumerate
						else if (equivClsExpr.IsEnumerate)
							foundVisitingClsExprIndividuals.AddRange(((OWLObjectOneOf)equivClsExpr).IndividualExpressions);
						#endregion

						#region Composite
						else if (equivClsExpr.IsComposite)
						{
							if (equivClsExpr is OWLObjectUnionOf objUnionOf)
							{
								foreach (OWLClassExpression objUnionOfElement in objUnionOf.ClassExpressions)
									foundVisitingClsExprIndividuals.AddRange(FindIndividualsOf(objUnionOfElement, visitContext));
							}
							else if (equivClsExpr is OWLObjectIntersectionOf objIntersectionOf)
							{
								bool isFirstIntersectionElement = true;
								foreach (OWLClassExpression objIntersectionOfElement in objIntersectionOf.ClassExpressions)
								{
									List<OWLIndividualExpression> objIntersectionOfElementIdvExprs = FindIndividualsOf(objIntersectionOfElement, visitContext);
									if (isFirstIntersectionElement)
									{
										isFirstIntersectionElement = false;
										foundVisitingClsExprIndividuals.AddRange(objIntersectionOfElementIdvExprs);
									}
									else
									{
										foundVisitingClsExprIndividuals.RemoveAll(iex => !objIntersectionOfElementIdvExprs.Any(objIntOfElmIdv => objIntOfElmIdv.GetIRI().Equals(iex.GetIRI())));
									}
								}
							}
						}
						#endregion

						#region ObjectRestriction
						else if (equivClsExpr.IsObjectRestriction)
						{
							foundVisitingClsExprIndividuals.AddRange(classAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(equivClsExprIRI))
																	   	   			.Select(ax => ax.IndividualExpression));

							//Object[Exact|Max]Cardinality and ObjectAllValuesFrom can only be answered with their assigned individuals (OWA)
							if (equivClsExpr is OWLObjectExactCardinality || equivClsExpr is OWLObjectMaxCardinality || equivClsExpr is OWLObjectAllValuesFrom)
								continue;	
							else
							{
								#region ObjectHasValue
								if (equivClsExpr is OWLObjectHasValue objHasValue)
								{
									RDFResource objHasValueIdvExprIRI = objHasValue.IndividualExpression.GetIRI();

									//Compute same individuals of OHV restriction value
									List<OWLIndividualExpression> sameIndividuals = ontology.GetSameIndividuals(objHasValue.IndividualExpression);

									//Compute object property assertions in scope of OHV restriction
									bool shouldSwitchObjPropIdvs = objHasValue.ObjectPropertyExpression is OWLObjectInverseOf;
									List<OWLObjectPropertyAssertion> inScopeObjPropAssertions = SelectObjectAssertionsByOPEX(objectPropertyAssertions, objHasValue.ObjectPropertyExpression);

                                    //Compute individuals satisfying OHV restriction
                                    foreach (OWLObjectPropertyAssertion inScopeObjPropAssertion in inScopeObjPropAssertions)
									{
										OWLIndividualExpression inScopeObjPropAssertionSourceIdvExpr = inScopeObjPropAssertion.SourceIndividualExpression;
										OWLIndividualExpression inScopeObjPropAssertionTargetIdvExpr = inScopeObjPropAssertion.TargetIndividualExpression;
										if (inScopeObjPropAssertion.ObjectPropertyExpression is OWLObjectInverseOf)
										{
											inScopeObjPropAssertionSourceIdvExpr = inScopeObjPropAssertion.TargetIndividualExpression;
											inScopeObjPropAssertionTargetIdvExpr = inScopeObjPropAssertion.SourceIndividualExpression;
										}

										if (shouldSwitchObjPropIdvs)
										{
											if (inScopeObjPropAssertionSourceIdvExpr.GetIRI().Equals(objHasValueIdvExprIRI)
												 || sameIndividuals.Any(idv => idv.GetIRI().Equals(inScopeObjPropAssertionSourceIdvExpr.GetIRI())))
												 foundVisitingClsExprIndividuals.Add(inScopeObjPropAssertionTargetIdvExpr);
										}
										else
										{
											if (inScopeObjPropAssertionTargetIdvExpr.GetIRI().Equals(objHasValueIdvExprIRI)
												 || sameIndividuals.Any(idv => idv.GetIRI().Equals(inScopeObjPropAssertionTargetIdvExpr.GetIRI())))
												 foundVisitingClsExprIndividuals.Add(inScopeObjPropAssertionSourceIdvExpr);
										}
									}
									continue;
								}
								#endregion

								#region ObjectHasSelf
								if (equivClsExpr is OWLObjectHasSelf objHasSelf)
								{
                                    //Compute object property assertions in scope of OHS restriction
                                    List<OWLObjectPropertyAssertion> inScopeObjPropAssertions = SelectObjectAssertionsByOPEX(objectPropertyAssertions, objHasSelf.ObjectPropertyExpression);

                                    //Compute individuals satisfying OHS restriction
                                    foreach (OWLObjectPropertyAssertion inScopeObjPropAssertion in inScopeObjPropAssertions)
									{
										if (inScopeObjPropAssertion.SourceIndividualExpression.GetIRI().Equals(inScopeObjPropAssertion.TargetIndividualExpression.GetIRI()))
											foundVisitingClsExprIndividuals.Add(inScopeObjPropAssertion.SourceIndividualExpression);
									}
									continue;
								}
								#endregion

								/*								
								 this is OWLObjectSomeValuesFrom
								|| this is OWLObjectMinCardinality
								*/
							}
						}
						#endregion

						#region DataRestriction
						else if (equivClsExpr.IsDataRestriction)
						{
							foundVisitingClsExprIndividuals.AddRange(classAssertions.Where(ax => ax.ClassExpression.GetIRI().Equals(equivClsExprIRI))
																	   	   			.Select(ax => ax.IndividualExpression));

							//Data[Exact|Max]Cardinality and DataAllValuesFrom can only be answered with their assigned individuals (OWA)
							if (equivClsExpr is OWLDataExactCardinality || equivClsExpr is OWLDataMaxCardinality || equivClsExpr is OWLDataAllValuesFrom)
								continue;	
							else
							{
                                #region DataHasValue
                                if (equivClsExpr is OWLDataHasValue dtHasValue)
                                {
                                    RDFLiteral dtHasValueLiteral = dtHasValue.Literal.GetLiteral();

                                    //Compute object property assertions in scope of OHV restriction
                                    List<OWLDataPropertyAssertion> inScopeDtPropAssertions = SelectDataAssertionsByDPEX(dataPropertyAssertions, dtHasValue.DataProperty);

                                    //Compute individuals satisfying OHV restriction
                                    foreach (OWLDataPropertyAssertion inScopeDtPropAssertion in inScopeDtPropAssertions)
                                    {
                                        if (inScopeDtPropAssertion.Literal.GetLiteral().Equals(dtHasValueLiteral))
                                            foundVisitingClsExprIndividuals.Add(inScopeDtPropAssertion.IndividualExpression);
                                    }
                                    continue;
                                }
                                #endregion

                                /*								
								 this is OWLDataSomeValuesFrom
								|| this is OWLDataMinCardinality
								*/
                            }
                        }
						#endregion
					}
                }
				#endregion

				return foundVisitingClsExprIndividuals;
			}
			#endregion

            List<OWLIndividualExpression> classIndividuals = new List<OWLIndividualExpression>();
            if (ontology != null && owlClass != null)
			{
				classIndividuals.AddRange(FindIndividualsOf(owlClass, new HashSet<long>()));

				if (!directOnly)
				{
					//Type(IDV1,C) ^ SameAs(IDV1,IDV2) -> Type(IDV2,C)
					foreach (OWLIndividualExpression classIndividual in classIndividuals.ToList())
						classIndividuals.AddRange(ontology.GetSameIndividuals(classIndividual));
				}
			}				
            return OWLExpressionHelper.RemoveDuplicates(classIndividuals);
        }
        #endregion

        #region Utilities
        internal static List<OWLObjectPropertyAssertion> SelectObjectAssertionsByOPEX(List<OWLObjectPropertyAssertion> objPropAsnAxioms, OWLObjectPropertyExpression objPropExpr)
        {
            if (objPropExpr is OWLObjectInverseOf objPropExprInvOf)
                return objPropAsnAxioms.Where(ax =>
                        (ax.ObjectPropertyExpression is OWLObjectInverseOf asnObjInvOf && asnObjInvOf.ObjectProperty.GetIRI().Equals(objPropExprInvOf.ObjectProperty.GetIRI()))
                     || (ax.ObjectPropertyExpression is OWLObjectProperty asnObjProp && asnObjProp.GetIRI().Equals(objPropExprInvOf.ObjectProperty.GetIRI()))).ToList();
            else
                return objPropAsnAxioms.Where(ax =>
                        (ax.ObjectPropertyExpression is OWLObjectInverseOf asnObjInvOf && asnObjInvOf.ObjectProperty.GetIRI().Equals(objPropExpr.GetIRI()))
                     || (ax.ObjectPropertyExpression is OWLObjectProperty asnObjProp && asnObjProp.GetIRI().Equals(objPropExpr.GetIRI()))).ToList();
        }

        internal static List<OWLDataPropertyAssertion> SelectDataAssertionsByDPEX(List<OWLDataPropertyAssertion> dtPropAsnAxioms, OWLDataProperty dtProp)
			=> dtPropAsnAxioms.Where(ax => ax.DataProperty.GetIRI().Equals(dtProp.GetIRI())).ToList();
        #endregion
    }
}