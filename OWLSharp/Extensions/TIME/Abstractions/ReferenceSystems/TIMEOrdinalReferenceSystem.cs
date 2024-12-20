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
            Ontology.DeclareEntity(new OWLNamedIndividual(era));
            Ontology.DeclareAssertionAxiom(new OWLClassAssertion(
                new OWLClass(RDFVocabulary.TIME.THORS.ERA),
                new OWLNamedIndividual(era)));
            Ontology.Data.DeclareObjectAssertion(this, RDFVocabulary.TIME.THORS.COMPONENT, era);
            Ontology.Data.DeclareObjectAssertion(era, RDFVocabulary.TIME.THORS.SYSTEM, this); //inference

            //Add knowledge to the A-BOX (begin)
            Ontology.DeclareInstantFeatureInternal(eraBeginning);
            Ontology.Data.DeclareIndividualType(eraBeginning, RDFVocabulary.TIME.THORS.ERA_BOUNDARY);
            Ontology.Data.DeclareObjectAssertion(era, RDFVocabulary.TIME.THORS.BEGIN, eraBeginning);
            Ontology.Data.DeclareObjectAssertion(eraBeginning, RDFVocabulary.TIME.THORS.NEXT_ERA, era); //inference

            //Add knowledge to the A-BOX (end)
            Ontology.DeclareInstantFeatureInternal(eraEnd);
            Ontology.Data.DeclareIndividualType(eraEnd, RDFVocabulary.TIME.THORS.ERA_BOUNDARY);
            Ontology.Data.DeclareObjectAssertion(era, RDFVocabulary.TIME.THORS.END, eraEnd);
            Ontology.Data.DeclareObjectAssertion(eraEnd, RDFVocabulary.TIME.THORS.PREVIOUS_ERA, era); //inference

            return this;
        }

        /// <summary>
        /// Declares the existence of the given "thors:member(era,subEra)" relation to the ordinal TRS
        /// </summary>
        public TIMEOrdinalReferenceSystem DeclareSubEra(RDFResource era, RDFResource subEra)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"era\" parameter is null");
            if (subEra == null)
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"subEra\" parameter is null");
            if (CheckHasSubEra(subEra, era, true))
                throw new OWLException("Cannot declare sub-era to ordinal TRS because given \"era\" parameter is already defined as sub-era of the given \"subEra\" parameter, so the requested operation would generate an A-BOX inconsistency");
            #endregion

            //Add knowledge to the A-BOX
            Ontology.Data.DeclareObjectAssertion(era, RDFVocabulary.TIME.THORS.MEMBER, subEra);

            return this;
        }

        /// <summary>
        /// Declares the existence of the given reference point to the ordinal TRS
        /// </summary>
        public TIMEOrdinalReferenceSystem DeclareReferencePoint(TIMEInstant referencePoint)
        {
            #region Guards
            if (referencePoint == null)
                throw new OWLException("Cannot declare reference point to ordinal TRS because given \"referencePoint\" parameter is null");
            #endregion

            //Add knowledge to the A-BOX (referencePoint)
            Ontology.DeclareInstantFeatureInternal(referencePoint);
            Ontology.Data.DeclareIndividualType(referencePoint, RDFVocabulary.TIME.THORS.ERA_BOUNDARY);
            Ontology.Data.DeclareObjectAssertion(this, RDFVocabulary.TIME.THORS.REFERENCE_POINT, referencePoint);

            return this;
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// Checks if this ordinal TRS contains a definition for the given era
        /// </summary>
        public bool CheckHasEra(RDFResource era)
        {
            #region Guards
            if (era == null)
                throw new OWLException("Cannot check if ordinal TRS has era because given \"era\" parameter is null");
            #endregion

            return Ontology.Data.GetIndividualsOf(Ontology.Model, RDFVocabulary.TIME.THORS.ERA)
                    .Any(idv => idv.Equals(era));
        }

        /// <summary>
        /// Checks if this ordinal TRS contains a definition for the given era boundary
        /// </summary>
        public bool CheckHasEraBoundary(RDFResource eraBoundary)
        {
            #region Guards
            if (eraBoundary == null)
                throw new OWLException("Cannot check if ordinal TRS has era boundary because given \"eraBoundary\" parameter is null");
            #endregion

            return Ontology.Data.GetIndividualsOf(Ontology.Model, RDFVocabulary.TIME.THORS.ERA_BOUNDARY)
                    .Any(idv => idv.Equals(eraBoundary));
        }

        /// <summary>
        /// Checks if this ordinal TRS contains a definition for the given reference point
        /// </summary>
        public bool CheckHasReferencePoint(RDFResource referencePoint)
        {
            #region Guards
            if (referencePoint == null)
                throw new OWLException("Cannot check if ordinal TRS has reference point because given \"referencePoint\" parameter is null");
            #endregion

            return Ontology.Data.GetIndividualsOf(Ontology.Model, RDFVocabulary.TIME.THORS.ERA_BOUNDARY)
                    .Any(idv => Ontology.Data.CheckHasObjectAssertion(this, RDFVocabulary.TIME.THORS.REFERENCE_POINT, idv) && idv.Equals(referencePoint));
        }

        //thors:member

        /// <summary>
        /// Checks if the given thors:Era has the given child thors:Era within this ordinal TRS
        /// </summary>
        public bool CheckHasSubEra(RDFResource era, RDFResource subEra, bool enableReasoning=true)
            => era != null && subEra != null && GetSuperErasOf(subEra, enableReasoning).Any(e => e.Equals(era));

        /// <summary>
        /// Checks if the given thors:Era has the given parent thors:Era within this ordinal TRS
        /// </summary>
        public bool CheckHasSuperEra(RDFResource era, RDFResource superEra, bool enableReasoning=true)
            => era != null && superEra != null && GetSubErasOf(superEra, enableReasoning).Any(e => e.Equals(era));

        /// <summary>
        /// Analyzes "thors:member(era,X)" relations of this ordinal TRS to answer the sub eras of the given thors:Era
        /// </summary>
        public List<RDFResource> GetSubErasOf(RDFResource era, bool enableReasoning=true)
        {
            List<RDFResource> subEras = new List<RDFResource>();

            if (era != null)
            {
                Dictionary<long, RDFResource> visitContext = new Dictionary<long, RDFResource>();

                //Reason on the given era
                subEras.AddRange(FindSubErasOf(era, visitContext, enableReasoning));

                //We don't want to also enlist the given thors:Era
                subEras.RemoveAll(cls => cls.Equals(era));
            }

            return RDFQueryUtilities.RemoveDuplicates(subEras);
        }

        /// <summary>
        /// Analyzes "thors:member(X,era)" relations of this ordinal TRS to answer the super eras of the given thors:Era
        /// </summary>
        public List<RDFResource> GetSuperErasOf(RDFResource era, bool enableReasoning=true)
        {
            List<RDFResource> superEras = new List<RDFResource>();

            if (era != null)
            {
                Dictionary<long, RDFResource> visitContext = new Dictionary<long, RDFResource>();

                //Reason on the given era
                superEras.AddRange(FindSuperErasOf(era, visitContext, enableReasoning));

                //We don't want to also enlist the given thors:Era
                superEras.RemoveAll(cls => cls.Equals(era));
            }

            return RDFQueryUtilities.RemoveDuplicates(superEras);
        }

        /// <summary>
        /// Finds "thors:member(era,X)" relations of this ordinal TRS to answer the sub eras of the given thors:Era
        /// </summary>
        internal List<RDFResource> FindSubErasOf(RDFResource era, Dictionary<long, RDFResource> visitContext, bool enableReasoning)
        {
            List<RDFResource> subEras = new List<RDFResource>();

            #region VisitContext
            if (!visitContext.ContainsKey(era.PatternMemberID))
                visitContext.Add(era.PatternMemberID, era);
            else
                return subEras;
            #endregion

            // DIRECT: thors:member(A,B)
            subEras.AddRange(Ontology.Data.ABoxGraph[era, RDFVocabulary.TIME.THORS.MEMBER, null, null]
                   .Select(t => t.Object)
                   .OfType<RDFResource>());

            // INDIRECT: thors:member(A,B) ^ thors:member(B,C) -> thors:member(A,C)
            if (enableReasoning)
            {
                foreach (RDFResource subEra in subEras.ToList())
                    subEras.AddRange(FindSubErasOf(subEra, visitContext, enableReasoning));
            }

            return subEras;
        }

        /// <summary>
        /// Finds "thors:member(X,era)" relations of this ordinal TRS to answer the super eras of the given thors:Era
        /// </summary>
        internal List<RDFResource> FindSuperErasOf(RDFResource era, Dictionary<long, RDFResource> visitContext, bool enableReasoning)
        {
            List<RDFResource> superEras = new List<RDFResource>();

            #region VisitContext
            if (!visitContext.ContainsKey(era.PatternMemberID))
                visitContext.Add(era.PatternMemberID, era);
            else
                return superEras;
            #endregion

            // DIRECT: thors:member(A,B)
            superEras.AddRange(Ontology.Data.ABoxGraph[null, RDFVocabulary.TIME.THORS.MEMBER, era, null]
                     .Select(t => t.Subject)
                     .OfType<RDFResource>());

            // INDIRECT: thors:member(A,B) ^ thors:member(B,C) -> thors:member(A,C)
            if (enableReasoning)
            {
                foreach (RDFResource superEra in superEras.ToList())
                    superEras.AddRange(FindSuperErasOf(superEra, visitContext, enableReasoning));
            }            

            return superEras;
        }

        /// <summary>
        /// Gets the (beginning,end) temporal coordinates of the given era within this ordinal TRS
        /// </summary>
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

            //Get begin boundary of era (if correctly declared to the ordinal TRS through THORS semantics)
            TIMECoordinate eraBeginBoundaryCoordinate = null;
            if (Ontology.Data.ABoxGraph[era, RDFVocabulary.TIME.THORS.BEGIN, null, null]
                            ?.Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                            ?.FirstOrDefault()
                            ?.Object is RDFResource eraBeginBoundary && CheckHasEraBoundary(eraBeginBoundary))
                eraBeginBoundaryCoordinate = Ontology.GetCoordinateOfInstant(eraBeginBoundary, calendarTRS);

            //Get end boundary of era (if correctly declared to the ordinal TRS through THORS semantics)
            TIMECoordinate eraEndBoundaryCoordinate = null;
            if (Ontology.Data.ABoxGraph[era, RDFVocabulary.TIME.THORS.END, null, null]
                            ?.Where(t => t.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                            ?.FirstOrDefault()
                            ?.Object is RDFResource eraEndBoundary && CheckHasEraBoundary(eraEndBoundary))
                eraEndBoundaryCoordinate = Ontology.GetCoordinateOfInstant(eraEndBoundary, calendarTRS);

            return (eraBeginBoundaryCoordinate, eraEndBoundaryCoordinate);
        }

        /// <summary>
        /// Gets the temporal extent of the given era within this ordinal TRS
        /// </summary>
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
                return TIMEConverter.CalculateExtent(eraCoordinates.Item1, eraCoordinates.Item2, calendarTRS);

            return null;
        }
        #endregion

        #endregion
    }
}