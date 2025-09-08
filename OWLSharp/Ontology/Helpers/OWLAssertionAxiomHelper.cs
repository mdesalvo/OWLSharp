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
using System.Globalization;
using System.Linq;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    public static class OWLAssertionAxiomHelper
    {
        #region Methods
        public static List<T> GetAssertionAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAssertionAxiom
            => ontology?.AssertionAxioms.OfType<T>().ToList() ?? [];

        public static bool CheckHasAssertionAxiom<T>(this OWLOntology ontology, T assertionAxiom) where T : OWLAssertionAxiom
            => GetAssertionAxiomsOfType<T>(ontology).Any(ax => string.Equals(ax.GetXML(), assertionAxiom?.GetXML()));

        public static void DeclareAssertionAxiom<T>(this OWLOntology ontology, T assertionAxiom) where T : OWLAssertionAxiom
        {
            #region Guards
            if (assertionAxiom == null)
                throw new OWLException("Cannot declare assertion axiom because given \"assertionAxiom\" parameter is null");
            #endregion

            if (!CheckHasAssertionAxiom(ontology, assertionAxiom))
                ontology?.AssertionAxioms.Add(assertionAxiom);
        }

        public static bool CheckIsSameIndividual(this OWLOntology ontology, OWLIndividualExpression leftIdvExpr, OWLIndividualExpression rightIdvExpr)
            => ontology != null && leftIdvExpr != null && rightIdvExpr != null && GetSameIndividuals(ontology, leftIdvExpr).Any(iex => iex.GetIRI().Equals(rightIdvExpr.GetIRI()));

        public static List<OWLIndividualExpression> GetSameIndividuals(this OWLOntology ontology, OWLIndividualExpression idvExpr)
        {
            #region Utilities
            List<OWLIndividualExpression> FindSameIndividuals(RDFResource idvExprIRI, List<OWLSameIndividual> axioms, HashSet<long> visitContext)
            {
                List<OWLIndividualExpression> foundSameIndividuals = [];

                #region VisitContext
                if (!visitContext.Add(idvExprIRI.PatternMemberID))
                    return foundSameIndividuals;
                #endregion

                #region Discovery
                foreach (OWLSameIndividual axiom in axioms.Where(ax => ax.IndividualExpressions.Any(iex => iex.GetIRI().Equals(idvExprIRI))))
                    foundSameIndividuals.AddRange(axiom.IndividualExpressions);

                //SameIndividual(I1,I2) ^ SameIndividual(I2,I3) -> SameIndividual(I1,I3)
                foreach (OWLIndividualExpression sameIndividual in foundSameIndividuals.ToList())
                    foundSameIndividuals.AddRange(FindSameIndividuals(sameIndividual.GetIRI(), axioms, visitContext));
                #endregion

                foundSameIndividuals.RemoveAll(res => res.GetIRI().Equals(idvExprIRI));
                return OWLExpressionHelper.RemoveDuplicates(foundSameIndividuals);
            }
            #endregion

            List<OWLIndividualExpression> sameIndividuals = [];
            if (ontology != null && idvExpr != null)
                sameIndividuals.AddRange(FindSameIndividuals(idvExpr.GetIRI(), GetAssertionAxiomsOfType<OWLSameIndividual>(ontology), []));
            return sameIndividuals;
        }

        public static bool CheckAreDifferentIndividuals(this OWLOntology ontology, OWLIndividualExpression leftIdvExpr, OWLIndividualExpression rightIdvExpr)
            => ontology != null && leftIdvExpr != null && rightIdvExpr != null && GetDifferentIndividuals(ontology, leftIdvExpr).Any(iex => iex.GetIRI().Equals(rightIdvExpr.GetIRI()));

        public static List<OWLIndividualExpression> GetDifferentIndividuals(this OWLOntology ontology, OWLIndividualExpression idvExpr)
        {
            List<OWLIndividualExpression> differentIndividuals = [];

            //There is no reasoning on individual difference (apart simmetry), being this totally under OWA domain
            if (ontology != null && idvExpr != null)
            {
                RDFResource idvExprIRI = idvExpr.GetIRI();
                foreach (OWLDifferentIndividuals axiom in GetAssertionAxiomsOfType<OWLDifferentIndividuals>(ontology).Where(ax => ax.IndividualExpressions.Any(iex => iex.GetIRI().Equals(idvExprIRI))))
                    differentIndividuals.AddRange(axiom.IndividualExpressions);
                differentIndividuals.RemoveAll(res => res.GetIRI().Equals(idvExprIRI));
            }

            return OWLExpressionHelper.RemoveDuplicates(differentIndividuals);
        }

        public static bool CheckIsIndividualOf(this OWLOntology ontology, OWLClassExpression clsExpr, OWLIndividualExpression idvExpr, bool enableSameAsEntailment=true)
            => ontology != null && clsExpr != null && idvExpr != null && GetIndividualsOf(ontology, clsExpr, enableSameAsEntailment).Any(iex => iex.GetIRI().Equals(idvExpr.GetIRI()));

        public static List<OWLIndividualExpression> GetIndividualsOf(this OWLOntology ontology, OWLClassExpression clsExpr, bool enableSameAsEntailment=true)
            => GetIndividualsOf(ontology, clsExpr, GetAssertionAxiomsOfType<OWLClassAssertion>(ontology), enableSameAsEntailment);
        internal static List<OWLIndividualExpression> GetIndividualsOf(this OWLOntology ontology, OWLClassExpression clsExpr, List<OWLClassAssertion> clsAsns, bool enableSameAsEntailment=true)
        {
            List<OWLObjectPropertyAssertion> objectPropertyAssertions = GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>(ontology);
            List<OWLDataPropertyAssertion> dataPropertyAssertions = GetAssertionAxiomsOfType<OWLDataPropertyAssertion>(ontology);

            #region Utilities
            List<OWLIndividualExpression> FindIndividualsOf(OWLClassExpression visitingClsExpr, HashSet<long> visitContext)
            {
                List<OWLIndividualExpression> foundVisitingClsExprIndividuals = [];
                RDFResource visitingClsExprIRI = visitingClsExpr.GetIRI();

                #region VisitContext
                if (!visitContext.Add(visitingClsExprIRI.PatternMemberID))
                    return foundVisitingClsExprIndividuals;
                #endregion

                #region Discovery
                foundVisitingClsExprIndividuals.AddRange(clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(visitingClsExprIRI))
                                                                .Select(ax => ax.IndividualExpression));

                //ClassAssertion(C1,I) ^ SubClassOf(C1,C2) -> ClassAssertion(C2,I)
                foreach (OWLClassExpression subClsExpr in ontology.GetSubClassesOf(visitingClsExpr))
                {
                    RDFResource subClsExprIRI = subClsExpr.GetIRI();
                    foundVisitingClsExprIndividuals.AddRange(clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(subClsExprIRI))
                                                                    .Select(ax => ax.IndividualExpression));
                }

                //ClassAssertion(C1,I) ^ EquivalentClasses(C1,C2) -> ClassAssertion(C2,I)
                foreach (OWLClassExpression equivClsExpr in ontology.GetEquivalentClasses(visitingClsExpr))
                {
                    RDFResource equivClsExprIRI = equivClsExpr.GetIRI();

                    #region Class
                    if (equivClsExpr.IsClass)
                    {
                        foundVisitingClsExprIndividuals.AddRange(FindIndividualsOf(equivClsExpr, visitContext));
                        continue;
                    }
                    #endregion

                    #region Enumerate
                    if (equivClsExpr.IsEnumerate)
                    {
                        foundVisitingClsExprIndividuals.AddRange(((OWLObjectOneOf)equivClsExpr).IndividualExpressions);
                        continue;
                    }
                    #endregion

                    #region Composite
                    if (equivClsExpr.IsComposite)
                    {
                        switch (equivClsExpr)
                        {
                            case OWLObjectUnionOf objUnionOf:
                            {
                                foreach (OWLClassExpression objUnionOfElement in objUnionOf.ClassExpressions)
                                    foundVisitingClsExprIndividuals.AddRange(FindIndividualsOf(objUnionOfElement, visitContext));
                                break;
                            }
                            case OWLObjectIntersectionOf objIntersectionOf:
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

                                break;
                            }
                        }

                        continue;
                    }
                    #endregion

                    #region ObjectRestriction
                    if (equivClsExpr.IsObjectRestriction)
                    {
                        foundVisitingClsExprIndividuals.AddRange(clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(equivClsExprIRI))
                                                                        .Select(ax => ax.IndividualExpression));

                        //Object[Exact|Max]Cardinality and ObjectAllValuesFrom can only be answered with their assigned individuals (OWA)
                        if (equivClsExpr is OWLObjectExactCardinality || equivClsExpr is OWLObjectMaxCardinality || equivClsExpr is OWLObjectAllValuesFrom)
                            continue;

                        #region ObjectHasValue/ObjectHasSelf/ObjectMinCardinality/ObjectSomeValuesFrom
                        switch (equivClsExpr)
                        {
                            case OWLObjectHasValue objHasValue:
                            {
                                RDFResource objHasValueIdvExprIRI = objHasValue.IndividualExpression.GetIRI();

                                //Compute same individuals of OHV restriction value
                                List<OWLIndividualExpression> sameIndividuals = ontology.GetSameIndividuals(objHasValue.IndividualExpression);

                                //Compute object property assertions in scope of OHV restriction
                                bool shouldSwitchObjPropIdvs = objHasValue.ObjectPropertyExpression is OWLObjectInverseOf;

                                //Compute individuals satisfying OHV restriction
                                foreach (OWLObjectPropertyAssertion inScopeObjPropAssertion in SelectObjectAssertionsByOPEX(objectPropertyAssertions, objHasValue.ObjectPropertyExpression))
                                {
                                    OWLIndividualExpression inScopeObjPropAsnSourceIdvExpr = inScopeObjPropAssertion.SourceIndividualExpression;
                                    OWLIndividualExpression inScopeObjPropAsnTargetIdvExpr = inScopeObjPropAssertion.TargetIndividualExpression;
                                    if (inScopeObjPropAssertion.ObjectPropertyExpression is OWLObjectInverseOf)
                                    {
                                        inScopeObjPropAsnSourceIdvExpr = inScopeObjPropAssertion.TargetIndividualExpression;
                                        inScopeObjPropAsnTargetIdvExpr = inScopeObjPropAssertion.SourceIndividualExpression;
                                    }

                                    if (shouldSwitchObjPropIdvs)
                                    {
                                        if (inScopeObjPropAsnSourceIdvExpr.GetIRI().Equals(objHasValueIdvExprIRI) || sameIndividuals.Any(idv => idv.GetIRI().Equals(inScopeObjPropAsnSourceIdvExpr.GetIRI())))
                                            foundVisitingClsExprIndividuals.Add(inScopeObjPropAsnTargetIdvExpr);
                                    }
                                    else
                                    {
                                        if (inScopeObjPropAsnTargetIdvExpr.GetIRI().Equals(objHasValueIdvExprIRI) || sameIndividuals.Any(idv => idv.GetIRI().Equals(inScopeObjPropAsnTargetIdvExpr.GetIRI())))
                                            foundVisitingClsExprIndividuals.Add(inScopeObjPropAsnSourceIdvExpr);
                                    }
                                }
                                continue;
                            }
                            case OWLObjectHasSelf objHasSelf:
                            {
                                //Compute object property assertions in scope of OHS restriction


                                //Compute individuals satisfying OHS restriction
                                foreach (OWLObjectPropertyAssertion inScopeObjPropAssertion in SelectObjectAssertionsByOPEX(objectPropertyAssertions, objHasSelf.ObjectPropertyExpression))
                                {
                                    if (inScopeObjPropAssertion.SourceIndividualExpression.GetIRI().Equals(inScopeObjPropAssertion.TargetIndividualExpression.GetIRI()))
                                        foundVisitingClsExprIndividuals.Add(inScopeObjPropAssertion.SourceIndividualExpression);
                                }
                                continue;
                            }
                            case OWLObjectMinCardinality _:
                            case OWLObjectSomeValuesFrom _:
                            {
                                //ObjectSomeValuesFrom is an OWL-DL syntactic shortcut for qualified ObjectMinCardinality(1)
                                //so we threat them the same way, fetching restricted object property and qualified class
                                OWLObjectPropertyExpression onPropExpr =
                                    (equivClsExpr as OWLObjectMinCardinality)?.ObjectPropertyExpression ??
                                    (equivClsExpr as OWLObjectSomeValuesFrom)?.ObjectPropertyExpression;
                                OWLClassExpression onClassExpr =
                                    (equivClsExpr as OWLObjectMinCardinality)?.ClassExpression ??
                                    (equivClsExpr as OWLObjectSomeValuesFrom)?.ClassExpression;
                                int objMinCardValue = 1;
                                if (equivClsExpr is OWLObjectMinCardinality objMNC && !int.TryParse(objMNC.Cardinality, NumberStyles.Integer, CultureInfo.InvariantCulture, out objMinCardValue))
                                    throw new OWLException($"Cannot get individuals of class expression {clsExpr.GetIRI()} because it is equivalent to an ObjectMinCardinality class expression specifying an invalid Cardinality value!");

                                //Compute object property assertions in scope of OMNC/OSVF restriction
                                bool shouldSwitchObjPropIdvs = onPropExpr is OWLObjectInverseOf;
                                List<OWLObjectPropertyAssertion> inScopeObjPropAssertions = SelectObjectAssertionsByOPEX(objectPropertyAssertions, onPropExpr);

                                //Compute qualified individuals eventually in scope of OMNC/OSVF restriction
                                bool isQualified = onClassExpr != null;
                                List<OWLIndividualExpression> qualifiedIdvExprs = isQualified ? ontology.GetIndividualsOf(onClassExpr)
                                    : Enumerable.Empty<OWLIndividualExpression>().ToList();

                                //Compute individuals participating to OMNC/OSVF restriction
                                var occurrenceRegistry = new Dictionary<long, (OWLIndividualExpression, long)>();
                                foreach (OWLObjectPropertyAssertion inScopeObjPropAssertion in inScopeObjPropAssertions)
                                {
                                    OWLIndividualExpression inScopeObjPropAsnSourceIdvExpr = inScopeObjPropAssertion.SourceIndividualExpression;
                                    OWLIndividualExpression inScopeObjPropAsnTargetIdvExpr = inScopeObjPropAssertion.TargetIndividualExpression;
                                    if (inScopeObjPropAssertion.ObjectPropertyExpression is OWLObjectInverseOf)
                                    {
                                        inScopeObjPropAsnSourceIdvExpr = inScopeObjPropAssertion.TargetIndividualExpression;
                                        inScopeObjPropAsnTargetIdvExpr = inScopeObjPropAssertion.SourceIndividualExpression;
                                    }
                                    if (shouldSwitchObjPropIdvs)
                                        (inScopeObjPropAsnTargetIdvExpr, inScopeObjPropAsnSourceIdvExpr) = (inScopeObjPropAsnSourceIdvExpr, inScopeObjPropAsnTargetIdvExpr);

                                    //Initialize individual counter
                                    RDFResource inScopeObjPropAsnSourceIdvExprIRI = inScopeObjPropAsnSourceIdvExpr.GetIRI();
                                    if (!occurrenceRegistry.ContainsKey(inScopeObjPropAsnSourceIdvExprIRI.PatternMemberID))
                                        occurrenceRegistry.Add(inScopeObjPropAsnSourceIdvExprIRI.PatternMemberID, (inScopeObjPropAsnSourceIdvExpr, 0));
                                    long occurrencyCounter = occurrenceRegistry[inScopeObjPropAsnSourceIdvExprIRI.PatternMemberID].Item2;

                                    //Collect occurrence of individual
                                    if (!isQualified  || qualifiedIdvExprs.Any(qiex => qiex.GetIRI().Equals(inScopeObjPropAsnTargetIdvExpr.GetIRI())))
                                        occurrenceRegistry[inScopeObjPropAsnSourceIdvExprIRI.PatternMemberID] = (inScopeObjPropAsnSourceIdvExpr, occurrencyCounter + 1);
                                }

                                //Filter individuals satisfying OMNC/OSVF restriction
                                var occurrenceRegistryEnumerator = occurrenceRegistry.Values.GetEnumerator();
                                while (occurrenceRegistryEnumerator.MoveNext())
                                {
                                    if (occurrenceRegistryEnumerator.Current.Item2 >= objMinCardValue)
                                        foundVisitingClsExprIndividuals.Add(occurrenceRegistryEnumerator.Current.Item1);
                                }
                                continue;
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region DataRestriction
                    if (equivClsExpr.IsDataRestriction)
                    {
                        foundVisitingClsExprIndividuals.AddRange(clsAsns.Where(ax => ax.ClassExpression.GetIRI().Equals(equivClsExprIRI))
                                                                        .Select(ax => ax.IndividualExpression));

                        //Data[Exact|Max]Cardinality and DataAllValuesFrom can only be answered with their assigned individuals (OWA)
                        if (equivClsExpr is OWLDataExactCardinality || equivClsExpr is OWLDataMaxCardinality || equivClsExpr is OWLDataAllValuesFrom)
                            continue;

                        #region DataHasValue/DataMinCardinality/DataSomeValuesFrom
                        switch (equivClsExpr)
                        {
                            case OWLDataHasValue dtHasValue:
                            {
                                RDFLiteral dtHasValueLiteral = dtHasValue.Literal.GetLiteral();

                                //Compute object property assertions in scope of OHV restriction


                                //Compute individuals satisfying OHV restriction
                                foreach (OWLDataPropertyAssertion inScopeDtPropAssertion in SelectDataAssertionsByDPEX(dataPropertyAssertions, dtHasValue.DataProperty))
                                {
                                    if (inScopeDtPropAssertion.Literal.GetLiteral().Equals(dtHasValueLiteral))
                                        foundVisitingClsExprIndividuals.Add(inScopeDtPropAssertion.IndividualExpression);
                                }
                                continue;
                            }
                            case OWLDataMinCardinality _:
                            case OWLDataSomeValuesFrom _:
                            {
                                //DataSomeValuesFrom is an OWL-DL syntactic shortcut for qualified DataMinCardinality(1)
                                //so we threat them the same way, fetching restricted data property and qualified datarange
                                List<OWLDataProperty> onProps = [];
                                switch (equivClsExpr)
                                {
                                    case OWLDataMinCardinality dtMinCard:
                                        onProps.Add(dtMinCard.DataProperty);
                                        break;
                                    case OWLDataSomeValuesFrom dtSVF:
                                        onProps.Add(dtSVF.DataProperty);
                                        break;
                                }
                                OWLDataRangeExpression onDataRangeExpr =
                                    (equivClsExpr as OWLDataMinCardinality)?.DataRangeExpression ??
                                    (equivClsExpr as OWLDataSomeValuesFrom)?.DataRangeExpression;
                                int dtMinCardValue = 1;
                                if (equivClsExpr is OWLDataMinCardinality dtMNC && !int.TryParse(dtMNC.Cardinality, NumberStyles.Integer, CultureInfo.InvariantCulture, out dtMinCardValue))
                                    throw new OWLException($"Cannot get individuals of class expression {clsExpr.GetIRI()} because it is equivalent to a DataMinCardinality class expression specifying an invalid Cardinality value!");

                                //Compute data property assertions in scope of DMNC/DSVF restriction
                                bool isQualified = onDataRangeExpr != null;
                                List<OWLDataPropertyAssertion> inScopeDtPropAssertions = [];
                                foreach (OWLDataProperty onProp in onProps)
                                    inScopeDtPropAssertions.AddRange(SelectDataAssertionsByDPEX(dataPropertyAssertions, onProp));

                                //Compute individuals participating to DMNC/DSVF restriction
                                var occurrenceRegistry = new Dictionary<long, (OWLIndividualExpression, long)>();
                                foreach (OWLDataPropertyAssertion inScopeDtPropAssertion in inScopeDtPropAssertions)
                                {
                                    //Initialize individual counter
                                    RDFResource inScopeDtPropAsnIdvExprIRI = inScopeDtPropAssertion.IndividualExpression.GetIRI();
                                    if (!occurrenceRegistry.ContainsKey(inScopeDtPropAsnIdvExprIRI.PatternMemberID))
                                        occurrenceRegistry.Add(inScopeDtPropAsnIdvExprIRI.PatternMemberID, (inScopeDtPropAssertion.IndividualExpression, 0));
                                    long occurrencyCounter = occurrenceRegistry[inScopeDtPropAsnIdvExprIRI.PatternMemberID].Item2;

                                    //Collect occurrence of individual
                                    if (!isQualified || CheckIsLiteralOf(ontology, onDataRangeExpr, inScopeDtPropAssertion.Literal))
                                        occurrenceRegistry[inScopeDtPropAsnIdvExprIRI.PatternMemberID] = (inScopeDtPropAssertion.IndividualExpression, occurrencyCounter + 1);
                                }

                                //Filter individuals satisfying DMNC/DSVF restriction
                                var occurrenceRegistryEnumerator = occurrenceRegistry.Values.GetEnumerator();
                                while (occurrenceRegistryEnumerator.MoveNext())
                                {
                                    if (occurrenceRegistryEnumerator.Current.Item2 >= dtMinCardValue)
                                        foundVisitingClsExprIndividuals.Add(occurrenceRegistryEnumerator.Current.Item1);
                                }
                                continue;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                return foundVisitingClsExprIndividuals;
            }
            #endregion

            List<OWLIndividualExpression> classIndividuals = [];
            if (ontology != null && clsExpr != null)
            {
                classIndividuals.AddRange(FindIndividualsOf(clsExpr, []));

                //This additional entailment can be quite expensive, so it is on-demand
                if (enableSameAsEntailment)
                {
                    //ClassAssertion(C,I1) ^ SameIndividual(I1,I2) -> ClassAssertion(C,I2)
                    foreach (OWLIndividualExpression classIndividual in classIndividuals.ToList())
                        classIndividuals.AddRange(ontology.GetSameIndividuals(classIndividual));
                }
            }
            return OWLExpressionHelper.RemoveDuplicates(classIndividuals);
        }

        public static bool CheckIsNegativeIndividualOf(this OWLOntology ontology, OWLClassExpression clsExpr, OWLIndividualExpression idvExpr)
            => CheckIsNegativeIndividualOf(ontology, clsExpr, idvExpr, GetAssertionAxiomsOfType<OWLClassAssertion>(ontology));
        internal static bool CheckIsNegativeIndividualOf(this OWLOntology ontology, OWLClassExpression clsExpr, OWLIndividualExpression idvExpr, List<OWLClassAssertion> clsAsnAxioms)
        {
            bool answer = false;

            if (ontology != null && clsExpr != null && idvExpr != null)
            {
                RDFResource idvExprIRI = idvExpr.GetIRI();
                RDFResource clsExprIRI = clsExpr.GetIRI();

                foreach (OWLClassAssertion idvExprClassAsn in clsAsnAxioms.Where(ax => ax.IndividualExpression.GetIRI().Equals(idvExprIRI)))
                {
                    //Direct
                    if (idvExprClassAsn.ClassExpression is OWLObjectComplementOf directObjComplOf
                        && directObjComplOf.ClassExpression.GetIRI().Equals(clsExprIRI))
                    {
                        answer = true;
                        break;
                    }

                    //Indirect
                    if (ontology.GetSuperClassesOf(idvExprClassAsn.ClassExpression)
                                .Union(ontology.GetEquivalentClasses(idvExprClassAsn.ClassExpression))
                                .Any(cex => cex is OWLObjectComplementOf indirectObjComplOf && indirectObjComplOf.ClassExpression.GetIRI().Equals(clsExprIRI)))
                    {
                        answer = true;
                        break;
                    }
                }
            }

            return answer;
        }

        public static bool CheckIsLiteralOf(this OWLOntology ontology, OWLDataRangeExpression drExpr, OWLLiteral literal)
        {
            if (ontology != null && drExpr != null && literal != null)
            {
                RDFLiteral rdfLiteral = literal.GetLiteral();
                RDFResource drExprIRI = drExpr.GetIRI();

                #region Datatype
                if (drExpr.IsDatatype)
                {
                    //Literals are instances of rdfs:Literal
                    if (drExprIRI.Equals(RDFVocabulary.RDFS.LITERAL))
                        return true;

                    //Plain literals are instances of rdf:langString when having language,
                    //are instances of rdf:dirLangString when having direction language,
                    //otherwise they are instances of rdf:PlainLiteral
                    if (drExprIRI.Equals(RDFVocabulary.RDF.LANG_STRING))
                        return rdfLiteral is RDFPlainLiteral rdfPlainLiteral && rdfPlainLiteral.HasLanguage();
                    if (drExprIRI.Equals(RDFVocabulary.RDF.DIR_LANG_STRING))
                        return rdfLiteral is RDFPlainLiteral rdfPlainLiteral && rdfPlainLiteral.HasDirection();
                    if (drExprIRI.Equals(RDFVocabulary.RDF.PLAIN_LITERAL))
                        return rdfLiteral is RDFPlainLiteral;
                    if (rdfLiteral is RDFPlainLiteral)
                        return false;

                    //Typed literals are instances of explicit datatype
                    return string.Equals(((RDFTypedLiteral)rdfLiteral).Datatype.ToString(), drExprIRI.ToString());
                }
                #endregion

                #region Enumerate
                if (drExpr.IsEnumerate)
                    return ((OWLDataOneOf)drExpr).Literals.Any(lit => lit.GetLiteral().Equals(rdfLiteral));
                #endregion

                #region Composite
                if (drExpr.IsComposite)
                {
                    switch (drExpr)
                    {
                        case OWLDataUnionOf dtUnionOf:
                            return dtUnionOf.DataRangeExpressions.Any(drex => ontology.CheckIsLiteralOf(drex, literal));
                        case OWLDataIntersectionOf dtIntersectionOf:
                            return dtIntersectionOf.DataRangeExpressions.All(drex => ontology.CheckIsLiteralOf(drex, literal));
                        case OWLDataComplementOf dtComplementOf:
                            return !ontology.CheckIsLiteralOf(dtComplementOf.DataRangeExpression, literal);
                    }
                }
                #endregion

                #region DatatypeRestriction
                if (drExpr.IsDatatypeRestriction)
                {
                    //No way to check a plain literal against a datarange expression
                    if (rdfLiteral is RDFPlainLiteral)
                        return false;

                    OWLDatatypeRestriction dtRestr = (OWLDatatypeRestriction)drExpr;
                    RDFTypedLiteral rdfTypedLiteral = (RDFTypedLiteral)rdfLiteral;

                    //We must build the RDF datatype corresponding to the OWL datatype restriction
                    RDFDatatype drExprDatatype = new RDFDatatype(drExprIRI.URI, dtRestr.Datatype.GetIRI().ToString().GetEnumFromDatatype(), null);
                    foreach (OWLFacetRestriction dtRestrFacet in dtRestr.FacetRestrictions ?? Enumerable.Empty<OWLFacetRestriction>())
                    {
                        //Numeric
                        if ((string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.LENGTH.ToString())
                             || string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MIN_LENGTH.ToString())
                             || string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MAX_LENGTH.ToString())
                             || string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MIN_EXCLUSIVE.ToString())
                             || string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MIN_INCLUSIVE.ToString())
                             || string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MAX_EXCLUSIVE.ToString())
                             || string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MAX_INCLUSIVE.ToString()))
                               && dtRestrFacet.Literal.GetLiteral() is RDFTypedLiteral dtRestrFacetTypedLiteralNF
                               && dtRestrFacetTypedLiteralNF.HasDecimalDatatype()
                               && uint.TryParse(dtRestrFacetTypedLiteralNF.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint dtRestrFacetTypedLiteralValue))
                        {
                            if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.LENGTH.ToString()))
                                drExprDatatype.Facets.Add(new RDFLengthFacet(dtRestrFacetTypedLiteralValue));
                            else if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MIN_LENGTH.ToString()))
                                drExprDatatype.Facets.Add(new RDFMinLengthFacet(dtRestrFacetTypedLiteralValue));
                            else if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MAX_LENGTH.ToString()))
                                drExprDatatype.Facets.Add(new RDFMaxLengthFacet(dtRestrFacetTypedLiteralValue));
                            else if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MIN_EXCLUSIVE.ToString()))
                                drExprDatatype.Facets.Add(new RDFMinExclusiveFacet(dtRestrFacetTypedLiteralValue));
                            else if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MIN_INCLUSIVE.ToString()))
                                drExprDatatype.Facets.Add(new RDFMinInclusiveFacet(dtRestrFacetTypedLiteralValue));
                            else if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MAX_EXCLUSIVE.ToString()))
                                drExprDatatype.Facets.Add(new RDFMaxExclusiveFacet(dtRestrFacetTypedLiteralValue));
                            else if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.MAX_INCLUSIVE.ToString()))
                                drExprDatatype.Facets.Add(new RDFMaxInclusiveFacet(dtRestrFacetTypedLiteralValue));
                            continue;
                        }

                        //Pattern
                        if (string.Equals(dtRestrFacet.FacetIRI, RDFVocabulary.XSD.PATTERN.ToString())
                             && dtRestrFacet.Literal.GetLiteral() is RDFTypedLiteral dtRestrFacetTypedLiteralPF
                             && dtRestrFacetTypedLiteralPF.Datatype.URI.Equals(RDFVocabulary.XSD.STRING.URI))
                        {
                            drExprDatatype.Facets.Add(new RDFPatternFacet(dtRestrFacetTypedLiteralPF.Value));
                        }
                    }

                    //Then we try validate the given literal against the reconstructed RDF datatype
                    try
                    {
                        _ = new RDFTypedLiteral(rdfTypedLiteral.Value, drExprDatatype);
                        return true;
                    }
                    catch { /* NO-OP */ }
                }
                #endregion
            }
            return false;
        }
        #endregion

        #region Utilities
        internal static List<OWLObjectPropertyAssertion> SelectObjectAssertionsByOPEX(List<OWLObjectPropertyAssertion> objPropAsnAxioms, OWLObjectPropertyExpression objPropExpr)
        {
            RDFResource opexIRI = objPropExpr is OWLObjectInverseOf objPropExprInvOf ? objPropExprInvOf.ObjectProperty.GetIRI() : objPropExpr.GetIRI();
            return [.. objPropAsnAxioms.Where(ax => (ax.ObjectPropertyExpression is OWLObjectInverseOf asnObjInvOf && asnObjInvOf.ObjectProperty.GetIRI().Equals(opexIRI))
                                                  || (ax.ObjectPropertyExpression is OWLObjectProperty asnObjProp && asnObjProp.GetIRI().Equals(opexIRI)))];
        }

        internal static List<OWLDataPropertyAssertion> SelectDataAssertionsByDPEX(List<OWLDataPropertyAssertion> dtPropAsnAxioms, OWLDataProperty dtProp)
            => [.. dtPropAsnAxioms.Where(ax => ax.DataProperty.GetIRI().Equals(dtProp.GetIRI()))];

        internal static List<OWLNegativeObjectPropertyAssertion> SelectNegativeObjectAssertionsByOPEX(List<OWLNegativeObjectPropertyAssertion> negObjPropAsnAxioms, OWLObjectPropertyExpression objPropExpr)
        {
            RDFResource opexIRI = objPropExpr is OWLObjectInverseOf objPropExprInvOf ? objPropExprInvOf.ObjectProperty.GetIRI() : objPropExpr.GetIRI();
            return [.. negObjPropAsnAxioms.Where(ax => (ax.ObjectPropertyExpression is OWLObjectInverseOf asnObjInvOf && asnObjInvOf.ObjectProperty.GetIRI().Equals(opexIRI))
                                                       || (ax.ObjectPropertyExpression is OWLObjectProperty asnObjProp && asnObjProp.GetIRI().Equals(opexIRI)))];
        }

        internal static List<OWLNegativeDataPropertyAssertion> SelectNegativeDataAssertionsByDPEX(List<OWLNegativeDataPropertyAssertion> negDtPropAsnAxioms, OWLDataProperty dtProp)
            => [.. negDtPropAsnAxioms.Where(ax => ax.DataProperty.GetIRI().Equals(dtProp.GetIRI()))];

        internal static List<OWLObjectPropertyAssertion> CalibrateObjectAssertions(OWLOntology ontology)
        {
            List<OWLObjectPropertyAssertion> objPropAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            foreach (OWLObjectPropertyAssertion objPropAsn in objPropAsns)
                if (objPropAsn.ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                {
                    (objPropAsn.SourceIndividualExpression, objPropAsn.TargetIndividualExpression) = (objPropAsn.TargetIndividualExpression, objPropAsn.SourceIndividualExpression);
                    objPropAsn.ObjectPropertyExpression = objInvOf.ObjectProperty;
                }
            return OWLAxiomHelper.RemoveDuplicates(objPropAsns);
        }

        internal static List<OWLNegativeObjectPropertyAssertion> CalibrateNegativeObjectAssertions(OWLOntology ontology)
        {
            List<OWLNegativeObjectPropertyAssertion> negObjPropAsns = ontology.GetAssertionAxiomsOfType<OWLNegativeObjectPropertyAssertion>();
            foreach (OWLNegativeObjectPropertyAssertion negObjPropAsn in negObjPropAsns)
                if (negObjPropAsn.ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                {
                    (negObjPropAsn.SourceIndividualExpression, negObjPropAsn.TargetIndividualExpression) = (negObjPropAsn.TargetIndividualExpression, negObjPropAsn.SourceIndividualExpression);
                    negObjPropAsn.ObjectPropertyExpression = objInvOf.ObjectProperty;
                }
            return OWLAxiomHelper.RemoveDuplicates(negObjPropAsns);
        }
        #endregion
    }
}