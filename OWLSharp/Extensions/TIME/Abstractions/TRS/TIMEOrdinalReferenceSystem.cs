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

using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.TIME
{
    public class TIMEOrdinalReferenceSystem : TIMEReferenceSystem
    {
        #region Properties
        internal OWLOntology Ontology { get; set; }
        #endregion

        #region Ctors
        public TIMEOrdinalReferenceSystem(RDFResource trsUri) : base(trsUri)
            => Ontology = new OWLOntology(URI);
        #endregion

        #region Methods

        #region Declarer
        public TIMEOrdinalReferenceSystem DeclareTHORSEra(RDFResource era, TIMEInstant eraBeginning, TIMEInstant eraEnd)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot declare era to ordinal TRS because given \"era\" parameter is null");
            if (eraBeginning == null)
                throw new OWLException("Cannot declare era to ordinal TRS because given \"referencePoint\" parameter is null");
            if (eraEnd == null)
                throw new OWLException("Cannot declare era to ordinal TRS because given \"eraEnd\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX (era)
            Ontology.DeclareEntity(new OWLNamedIndividual(era));
            Ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.THORS.ERA));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(era)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                new OWLNamedIndividual(this),
                new OWLNamedIndividual(era)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(this))); //inference

            //Add knowledge to the A-BOX (begin)
            Ontology.DeclareTIMEInstantFeatureInternal(eraBeginning);
            Ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(eraBeginning)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(eraBeginning)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                new OWLNamedIndividual(eraBeginning),
                new OWLNamedIndividual(era))); //inference

            //Add knowledge to the A-BOX (end)
            Ontology.DeclareTIMEInstantFeatureInternal(eraEnd);
            Ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.END));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(eraEnd)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(eraEnd)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                new OWLNamedIndividual(eraEnd),
                new OWLNamedIndividual(era))); //inference

            return this;
        }

        public TIMEOrdinalReferenceSystem DeclareTHORSSubEra(RDFResource era, RDFResource subEra)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"era\" parameter is null");
            if (subEra == null)
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"subEra\" parameter is null");
            if (CheckHasTHORSSubEra(subEra, era, true))
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"era\" parameter is already defined as sub-era of the given \"subEra\" parameter, so the requested operation would generate an A-BOX inconsistency");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.DeclareEntity(new OWLNamedIndividual(era));
            Ontology.DeclareEntity(new OWLNamedIndividual(subEra));
            Ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.THORS.ERA));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(era)));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(subEra)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(subEra)));

            return this;
        }

        public TIMEOrdinalReferenceSystem DeclareTHORSReferencePoint(TIMEInstant referencePoint)
        {
            #region Guards
            if (referencePoint == null)
                throw new OWLException("Cannot declare reference point to ordinal TRS because given \"referencePoint\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX (referencePoint)
            Ontology.DeclareTIMEInstantFeatureInternal(referencePoint);
            Ontology.DeclareEntity(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY));
            Ontology.DeclareEntity(new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(referencePoint)));
            Ontology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT),
                new OWLNamedIndividual(this),
                new OWLNamedIndividual(referencePoint)));

            return this;
        }
        #endregion

        #region Analyzer
        public bool CheckHasTHORSEra(RDFResource era)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot check if ordinal TRS has era because given \"era\" parameter is null");
            #endregion

            return Ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.THORS.ERA))
                    .Any(idv => idv.Equals(era));
        }

        public bool CheckHasTHORSEraBoundary(RDFResource eraBoundary)
        {
            #region Guards
            if (eraBoundary == null)
                throw new OWLException("Cannot check if ordinal TRS has era boundary because given \"eraBoundary\" parameter is null");
            #endregion

            return Ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY))
                    .Any(idv => idv.Equals(eraBoundary));
        }

        public bool CheckHasTHORSReferencePoint(RDFResource referencePoint)
        {
            #region Guards
            if (referencePoint == null)
                throw new OWLException("Cannot check if ordinal TRS has reference point because given \"referencePoint\" parameter is null");
            #endregion

            return Ontology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY))
                    .Any(idv => Ontology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT), new OWLNamedIndividual(this), idv)) 
                                 && idv.GetIRI().Equals(referencePoint));
        }

        public bool CheckHasTHORSSubEra(RDFResource era, RDFResource subEra, bool enableReasoning=true)
            => era != null && subEra != null && GetTHORSSuperEras(subEra, enableReasoning).Any(e => e.Equals(era));

        public bool CheckHasTHORSSuperEra(RDFResource era, RDFResource superEra, bool enableReasoning=true)
            => era != null && superEra != null && GetTHORSSubEras(superEra, enableReasoning).Any(e => e.Equals(era));

        public List<RDFResource> GetTHORSSubEras(RDFResource era, bool enableReasoning=true)
        {
            List<RDFResource> subEras = new List<RDFResource>();

            if (era != null)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(Ontology);
                List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER));

                //Reason on the given era
                subEras.AddRange(FindTHORSSubEras(era, thorsMemberObjPropAsns, new Dictionary<long, RDFResource>(), enableReasoning));

                //We don't want to also enlist the given thors:Era
                subEras.RemoveAll(cls => cls.Equals(era));
            }

            return RDFQueryUtilities.RemoveDuplicates(subEras);
        }
        internal List<RDFResource> FindTHORSSubEras(RDFResource era, List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns, Dictionary<long, RDFResource> visitContext, bool enableReasoning)
        {
            List<RDFResource> subEras = new List<RDFResource>();

            #region VisitContext
            if (!visitContext.ContainsKey(era.PatternMemberID))
                visitContext.Add(era.PatternMemberID, era);
            else
                return subEras;
            #endregion

            // DIRECT: thors:member(A,B)
            subEras.AddRange(thorsMemberObjPropAsns.Where(asn  => asn.SourceIndividualExpression.GetIRI().Equals(era))
                                                   .Select(asn => asn.TargetIndividualExpression.GetIRI()));

            // INDIRECT: thors:member(A,B) ^ thors:member(B,C) -> thors:member(A,C)
            if (enableReasoning)
            {
                foreach (RDFResource subEra in subEras.ToList())
                    subEras.AddRange(FindTHORSSubEras(subEra, thorsMemberObjPropAsns, visitContext, enableReasoning));
            }

            return subEras;
        }

        public List<RDFResource> GetTHORSSuperEras(RDFResource era, bool enableReasoning=true)
        {
            List<RDFResource> superEras = new List<RDFResource>();

            if (era != null)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(Ontology);
                List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER));

                //Reason on the given era
                superEras.AddRange(FindTHORSSuperEras(era, thorsMemberObjPropAsns, new Dictionary<long, RDFResource>(), enableReasoning));

                //We don't want to also enlist the given thors:Era
                superEras.RemoveAll(cls => cls.Equals(era));
            }

            return RDFQueryUtilities.RemoveDuplicates(superEras);
        }
        internal List<RDFResource> FindTHORSSuperEras(RDFResource era, List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns, Dictionary<long, RDFResource> visitContext, bool enableReasoning)
        {
            List<RDFResource> superEras = new List<RDFResource>();

            #region VisitContext
            if (!visitContext.ContainsKey(era.PatternMemberID))
                visitContext.Add(era.PatternMemberID, era);
            else
                return superEras;
            #endregion

            // DIRECT: thors:member(A,B)
            superEras.AddRange(thorsMemberObjPropAsns.Where(asn  => asn.TargetIndividualExpression.GetIRI().Equals(era))
                                                     .Select(asn => asn.SourceIndividualExpression.GetIRI()));

            // INDIRECT: thors:member(A,B) ^ thors:member(B,C) -> thors:member(A,C)
            if (enableReasoning)
            {
                foreach (RDFResource superEra in superEras.ToList())
                    superEras.AddRange(FindTHORSSuperEras(superEra, thorsMemberObjPropAsns, visitContext, enableReasoning));
            }            

            return superEras;
        }

        public (TIMECoordinate,TIMECoordinate) GetTHORSEraCoordinates(RDFResource era, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot get coordinates of era because given \"era\" parameter is null");
            if (!CheckHasTHORSEra(era))
                throw new OWLException("Cannot get coordinates of era because given \"era\" parameter is not a component of this ordinal TRS");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Temporary working variables
            List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(Ontology);
            List<OWLObjectPropertyAssertion> thorsBeginObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN));
            List<OWLObjectPropertyAssertion> thorsEndObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.END));

            //Get begin boundary of era (if correctly declared to the ordinal TRS through THORS semantics)
            TIMECoordinate eraBeginBoundaryCoordinate = null;
            OWLObjectPropertyAssertion thorsBeginEraAsn = thorsBeginObjPropAsns.FirstOrDefault(asn => asn.SourceIndividualExpression.GetIRI().Equals(era)
                                                           && CheckHasTHORSEraBoundary(asn.TargetIndividualExpression.GetIRI()));
            if (thorsBeginEraAsn != null)
                eraBeginBoundaryCoordinate = Ontology.GetCoordinateOfTIMEInstant(thorsBeginEraAsn.TargetIndividualExpression.GetIRI(), calendarTRS);

            //Get end boundary of era (if correctly declared to the ordinal TRS through THORS semantics)
            TIMECoordinate eraEndBoundaryCoordinate = null;
            OWLObjectPropertyAssertion thorsEndEraAsn = thorsEndObjPropAsns.FirstOrDefault(asn => asn.SourceIndividualExpression.GetIRI().Equals(era)
                                                         && CheckHasTHORSEraBoundary(asn.TargetIndividualExpression.GetIRI()));
            if (thorsEndEraAsn != null)
                eraEndBoundaryCoordinate = Ontology.GetCoordinateOfTIMEInstant(thorsEndEraAsn.TargetIndividualExpression.GetIRI(), calendarTRS);

            return (eraBeginBoundaryCoordinate, eraEndBoundaryCoordinate);
        }

        public TIMEExtent GetTHORSEraExtent(RDFResource era, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot get extent of era because given \"era\" parameter is null");
            if (!CheckHasTHORSEra(era))
                throw new OWLException("Cannot get extent of era because given \"era\" parameter is not a component of this ordinal TRS");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Get coordinates of era (if correctly declared to the ordinal TRS with THORS semantics)
            (TIMECoordinate,TIMECoordinate) eraCoordinates = GetTHORSEraCoordinates(era, calendarTRS);

            //Get extent of era
            if (eraCoordinates.Item1 != null && eraCoordinates.Item2 != null)
                return TIMEConverter.CalculateExtent(eraCoordinates.Item1, eraCoordinates.Item2, calendarTRS);

            return null;
        }
        #endregion

        #endregion
    }
}