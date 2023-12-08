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

using RDFSharp.Model;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// TIMEIntervalHelper contains methods for analyzing Allen relations involving temporal intervals
    /// </summary>
    public static class TIMEIntervalHelper
    {
        #region Methods
        /// <summary>
        /// Checks if the given aInterval is time:intervalAfter the given bInterval
        /// </summary>
        public static bool CheckAfter(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI, 
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval is after bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval is after bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalAfter
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalBefore the given bInterval
        /// </summary>
        public static bool CheckBefore(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI, 
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval is before bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval is before bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //time:intervalBefore
            return aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == -1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalContains the given bInterval
        /// </summary>
        public static bool CheckContains(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI, 
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval contains bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval contains bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalContains
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == -1
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalDisjoint the given bInterval
        /// </summary>
        public static bool CheckDisjoint(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI, 
            TIMECalendarReferenceSystem calendarTRS=null)
            => CheckBefore(timeOntology, aTimeIntervalURI, bTimeIntervalURI, calendarTRS)
                 || CheckAfter(timeOntology, aTimeIntervalURI, bTimeIntervalURI, calendarTRS);

        /// <summary>
        /// Checks if the given aInterval is time:intervalDuring the given bInterval
        /// </summary>
        public static bool CheckDuring(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval during bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval during bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalDuring
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 1
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == -1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalEquals the given bInterval
        /// </summary>
        public static bool CheckEquals(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval equals bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval equals bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalEquals
            return aTimeIntervalBeginningCoordinate.Equals(bTimeIntervalBeginningCoordinate)
                     && aTimeIntervalEndCoordinate.Equals(bTimeIntervalEndCoordinate);
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalFinishedBy the given bInterval
        /// </summary>
        public static bool CheckFinishedBy(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval finishedBy bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval finishedBy bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalFinishedBy
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == -1
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 0;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalFinishes the given bInterval
        /// </summary>
        public static bool CheckFinishes(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval finishes bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval finishes bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalFinishes
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 1
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 0;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalIn the given bInterval
        /// </summary>
        public static bool CheckIn(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval in bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval in bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalIn
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) >= 0
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) <= 0
                      && !CheckEquals(timeOntology, aTimeIntervalURI, bTimeIntervalURI);
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalMeets the given bInterval
        /// </summary>
        public static bool CheckMeets(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval meets bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval meets bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //time:intervalMeets
            return aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 0;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalMetBy the given bInterval
        /// </summary>
        public static bool CheckMetBy(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval is metBy bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval is metBy bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalAfter
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 0;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalOverlaps the given bInterval
        /// </summary>
        public static bool CheckOverlaps(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval overlaps bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval overlaps bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalOverlaps
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == -1
                    && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 1
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == -1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalOverlappedBy the given bInterval
        /// </summary>
        public static bool CheckOverlappedBy(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval overlappedBy bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval overlappedBy bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalOverlappedBy
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 1
                    && aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalEndCoordinate) == -1
                     && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalStarts the given bInterval
        /// </summary>
        public static bool CheckStarts(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval starts bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval starts bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalStarts
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 0
                    && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == -1;
        }

        /// <summary>
        /// Checks if the given aInterval is time:intervalStartedBy the given bInterval
        /// </summary>
        public static bool CheckStartedBy(OWLOntology timeOntology, RDFResource aTimeIntervalURI, RDFResource bTimeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS = null)
        {
            #region Guards
            if (aTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval startedBy bInterval because given \"aTimeIntervalURI\" parameter is null");
            if (bTimeIntervalURI == null)
                throw new OWLException("Cannot check if aInterval startedBy bInterval because given \"bTimeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given aInterval's beginning instant
            TIMECoordinate aTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given aInterval's end instant
            TIMECoordinate aTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(aTimeIntervalURI, calendarTRS);
            if (aTimeIntervalEndCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's beginning instant
            TIMECoordinate bTimeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given bInterval's end instant
            TIMECoordinate bTimeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(bTimeIntervalURI, calendarTRS);
            if (bTimeIntervalEndCoordinate == null)
                return false;

            //time:intervalStarts
            return aTimeIntervalBeginningCoordinate.CompareTo(bTimeIntervalBeginningCoordinate) == 0
                    && aTimeIntervalEndCoordinate.CompareTo(bTimeIntervalEndCoordinate) == 1;
        }
        #endregion
    }
}