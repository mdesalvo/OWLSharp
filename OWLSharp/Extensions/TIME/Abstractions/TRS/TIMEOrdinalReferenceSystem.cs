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
        public OWLOntology THORSOntology { get; internal set; }
        #endregion

        #region Ctors
        public TIMEOrdinalReferenceSystem(RDFResource trsUri) : base(trsUri)
        {
            THORSOntology = new OWLOntology(URI);

            //Initialize TIME+THORS
            THORSOntology.InitializeTIMEAsync().GetAwaiter().GetResult();
        }

        public TIMEOrdinalReferenceSystem(RDFResource trsUri, TIMEOrdinalReferenceSystem ordinalTRS) : base(trsUri)
            => THORSOntology = ordinalTRS?.THORSOntology ?? throw new OWLException("Cannot create ordinal TRS because given \"ordinalTRS\" parameter is null");
        #endregion

        #region Methods

        #region Declarer
        public TIMEOrdinalReferenceSystem DeclareEra(RDFResource era, TIMEInstant eraBeginning, TIMEInstant eraEnd)
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
            THORSOntology.DeclareEntity(new OWLNamedIndividual(era));
            THORSOntology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(era)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.COMPONENT),
                new OWLNamedIndividual(this),
                new OWLNamedIndividual(era)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.SYSTEM),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(this))); //inference

            //Add knowledge to the A-BOX (begin)
            THORSOntology.DeclareInstantFeatureInternal(eraBeginning);
            THORSOntology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(eraBeginning)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(eraBeginning)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.NEXT_ERA),
                new OWLNamedIndividual(eraBeginning),
                new OWLNamedIndividual(era))); //inference

            //Add knowledge to the A-BOX (end)
            THORSOntology.DeclareInstantFeatureInternal(eraEnd);
            THORSOntology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                new OWLNamedIndividual(eraEnd)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.END),
                new OWLNamedIndividual(era),
                new OWLNamedIndividual(eraEnd)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.PREVIOUS_ERA),
                new OWLNamedIndividual(eraEnd),
                new OWLNamedIndividual(era))); //inference

            return this;
        }

        public TIMEOrdinalReferenceSystem DeclareSubEra(RDFResource subEra, RDFResource superEra)
        {
            #region Guards
            if (subEra == null)
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"subEra\" parameter is null");
            if (superEra == null)
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"superEra\" parameter is null");
            if (CheckIsSubEraOf(superEra, subEra, true))
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"superEra\" parameter is already defined as sub-era of the given \"subEra\" parameter!");
            #endregion

            //Add knowledge to the A-BOX
            THORSOntology.DeclareEntity(new OWLNamedIndividual(subEra));
            THORSOntology.DeclareEntity(new OWLNamedIndividual(superEra));
            THORSOntology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(subEra)));
            THORSOntology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(superEra)));
            THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER),
                new OWLNamedIndividual(superEra),
                new OWLNamedIndividual(subEra)));

            return this;
        }

        public TIMEOrdinalReferenceSystem DeclareReferencePoints(TIMEInstant[] referencePoints)
        {
            #region Guards
            if (referencePoints == null)
                throw new OWLException("Cannot declare reference points to ordinal TRS because given \"referencePoints\" parameter is null");
            if (referencePoints.Any(rp => rp == null))
                throw new OWLException("Cannot declare reference points to ordinal TRS because given \"referencePoints\" parameter contains null elements");
            if (referencePoints.Length < 2)
                throw new OWLException("Cannot declare reference points to ordinal TRS because given \"referencePoints\" parameter must contain at least 2 elements");
            #endregion

            //Add knowledge to the A-BOX
            for (int i=0; i<referencePoints.Length; i++)
            {
                THORSOntology.DeclareInstantFeatureInternal(referencePoints[i]);
                THORSOntology.DeclareAssertionAxiom(new OWLClassAssertion(
                    new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY),
                    new OWLNamedIndividual(referencePoints[i])));
                THORSOntology.DeclareAssertionAxiom(new OWLObjectPropertyAssertion(
                    new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT),
                    new OWLNamedIndividual(this),
                    new OWLNamedIndividual(referencePoints[i])));
            }

            return this;
        }
        #endregion

        #region Analyzer
        public bool CheckHasEra(RDFResource era)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot check if ordinal TRS has era because given \"era\" parameter is null");
            #endregion

            return THORSOntology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.THORS.ERA))
                    .Any(idv => idv.GetIRI().Equals(era));
        }

        public bool CheckHasEraBoundary(RDFResource eraBoundary)
        {
            #region Guards
            if (eraBoundary == null)
                throw new OWLException("Cannot check if ordinal TRS has era boundary because given \"eraBoundary\" parameter is null");
            #endregion

            return THORSOntology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY))
                    .Any(idv => idv.GetIRI().Equals(eraBoundary));
        }

        public bool CheckHasReferencePoint(RDFResource referencePoint)
        {
            #region Guards
            if (referencePoint == null)
                throw new OWLException("Cannot check if ordinal TRS has reference point because given \"referencePoint\" parameter is null");
            #endregion

            return THORSOntology.GetIndividualsOf(new OWLClass(RDFVocabulary.TIME.THORS.ERA_BOUNDARY))
                    .Any(idv => THORSOntology.CheckHasAssertionAxiom(new OWLObjectPropertyAssertion(new OWLObjectProperty(RDFVocabulary.TIME.THORS.REFERENCE_POINT), new OWLNamedIndividual(this), idv)) 
                                 && idv.GetIRI().Equals(referencePoint));
        }

        public bool CheckIsSubEraOf(RDFResource subEra, RDFResource superEra, bool enableReasoning=true)
            => subEra != null && superEra != null && GetSubErasOf(superEra, enableReasoning).Any(e => e.Equals(subEra));

        public bool CheckIsSuperEraOf(RDFResource superEra, RDFResource subEra, bool enableReasoning=true)
            => superEra != null && subEra != null && GetSuperErasOf(subEra, enableReasoning).Any(e => e.Equals(superEra));

        public List<RDFResource> GetSubErasOf(RDFResource era, bool enableReasoning=true)
        {
            List<RDFResource> subEras = new List<RDFResource>();

            if (era != null)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(THORSOntology);
                List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER));

                //Reason on the given era
                subEras.AddRange(FindSubErasOf(era, thorsMemberObjPropAsns, new Dictionary<long, RDFResource>(), enableReasoning));

                //We don't want to also enlist the given thors:Era
                subEras.RemoveAll(cls => cls.Equals(era));
            }

            return RDFQueryUtilities.RemoveDuplicates(subEras);
        }
        internal List<RDFResource> FindSubErasOf(RDFResource era, List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns, Dictionary<long, RDFResource> visitContext, bool enableReasoning)
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
                    subEras.AddRange(FindSubErasOf(subEra, thorsMemberObjPropAsns, visitContext, enableReasoning));
            }

            return subEras;
        }

        public List<RDFResource> GetSuperErasOf(RDFResource era, bool enableReasoning=true)
        {
            List<RDFResource> superEras = new List<RDFResource>();

            if (era != null)
            {
                //Temporary working variables
                List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(THORSOntology);
                List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.MEMBER));

                //Reason on the given era
                superEras.AddRange(FindSuperErasOf(era, thorsMemberObjPropAsns, new Dictionary<long, RDFResource>(), enableReasoning));

                //We don't want to also enlist the given thors:Era
                superEras.RemoveAll(cls => cls.Equals(era));
            }

            return RDFQueryUtilities.RemoveDuplicates(superEras);
        }
        internal List<RDFResource> FindSuperErasOf(RDFResource era, List<OWLObjectPropertyAssertion> thorsMemberObjPropAsns, Dictionary<long, RDFResource> visitContext, bool enableReasoning)
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
                    superEras.AddRange(FindSuperErasOf(superEra, thorsMemberObjPropAsns, visitContext, enableReasoning));
            }            

            return superEras;
        }

        public (TIMECoordinate,TIMECoordinate) GetEraCoordinates(RDFResource era, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot get coordinates of era because given \"era\" parameter is null");
            if (!CheckHasEra(era))
                throw new OWLException("Cannot get coordinates of era because given \"era\" parameter is not a component of this ordinal TRS");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Temporary working variables
            List<OWLObjectPropertyAssertion> objPropAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(THORSOntology);
            List<OWLObjectPropertyAssertion> thorsBeginObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.BEGIN));
            List<OWLObjectPropertyAssertion> thorsEndObjPropAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(objPropAsns, new OWLObjectProperty(RDFVocabulary.TIME.THORS.END));

            //Get begin boundary of era (if correctly declared to the ordinal TRS through THORS semantics)
            TIMECoordinate eraBeginBoundaryCoordinate = null;
            OWLObjectPropertyAssertion thorsBeginEraAsn = thorsBeginObjPropAsns.FirstOrDefault(asn => asn.SourceIndividualExpression.GetIRI().Equals(era)
                                                           && CheckHasEraBoundary(asn.TargetIndividualExpression.GetIRI()));
            if (thorsBeginEraAsn != null)
                eraBeginBoundaryCoordinate = THORSOntology.GetCoordinateOfInstant(thorsBeginEraAsn.TargetIndividualExpression.GetIRI(), calendarTRS);

            //Get end boundary of era (if correctly declared to the ordinal TRS through THORS semantics)
            TIMECoordinate eraEndBoundaryCoordinate = null;
            OWLObjectPropertyAssertion thorsEndEraAsn = thorsEndObjPropAsns.FirstOrDefault(asn => asn.SourceIndividualExpression.GetIRI().Equals(era)
                                                         && CheckHasEraBoundary(asn.TargetIndividualExpression.GetIRI()));
            if (thorsEndEraAsn != null)
                eraEndBoundaryCoordinate = THORSOntology.GetCoordinateOfInstant(thorsEndEraAsn.TargetIndividualExpression.GetIRI(), calendarTRS);

            return (eraBeginBoundaryCoordinate, eraEndBoundaryCoordinate);
        }

        public TIMEExtent GetEraExtent(RDFResource era, TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot get extent of era because given \"era\" parameter is null");
            if (!CheckHasEra(era))
                throw new OWLException("Cannot get extent of era because given \"era\" parameter is not a component of this ordinal TRS");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Get coordinates of era (if correctly declared to the ordinal TRS with THORS semantics)
            (TIMECoordinate,TIMECoordinate) eraCoordinates = GetEraCoordinates(era, calendarTRS);

            //Get extent of era
            if (eraCoordinates.Item1 != null && eraCoordinates.Item2 != null)
                return TIMEConverter.CalculateExtentBetweenCoordinates(eraCoordinates.Item1, eraCoordinates.Item2, calendarTRS);

            return null;
        }
        #endregion

        #endregion
    }
}