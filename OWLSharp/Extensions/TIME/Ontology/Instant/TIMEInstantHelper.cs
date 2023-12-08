/*
   Copyright 2012-2024 Marco De Salvo
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
    /// TIMEInstantHelper contains methods for analyzing Allen relations involving temporal instants
    /// </summary>
    public static class TIMEInstantHelper
    {
        #region Methods
        /// <summary>
        /// Checks if the given instant is time:inside the given interval (boundaries excluded)
        /// </summary>
        public static bool CheckInsideInterval(OWLOntology timeOntology, RDFResource timeInstantURI, RDFResource timeIntervalURI, 
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeInstantURI == null)
                throw new OWLException("Cannot check if instant is inside interval because given \"timeInstantURI\" parameter is null");
            if (timeIntervalURI == null)
                throw new OWLException("Cannot check if instant is inside interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given time instant
            TIMECoordinate timeInstantCoordinate = timeOntology?.GetInstantCoordinate(timeInstantURI, calendarTRS);
            if (timeInstantCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given time interval's beginning instant
            TIMECoordinate timeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(timeIntervalURI, calendarTRS);
            if (timeIntervalBeginningCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given time interval's end instant
            TIMECoordinate timeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(timeIntervalURI, calendarTRS);
            if (timeIntervalEndCoordinate == null)
                return false;

            //time:inside
            return timeInstantCoordinate.CompareTo(timeIntervalBeginningCoordinate) == 1
                     && timeInstantCoordinate.CompareTo(timeIntervalEndCoordinate) == -1;
        }

        /// <summary>
        /// Checks if the given aInstant is time:after the given bInstant
        /// </summary>
        public static bool CheckAfter(OWLOntology timeOntology, RDFResource aTimeInstantURI, RDFResource bTimeInstantURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeInstantURI == null)
                throw new OWLException("Cannot check if aInstant is after bInstant because given \"aTimeInstantURI\" parameter is null");
            if (bTimeInstantURI == null)
                throw new OWLException("Cannot check if aInstant is after bInstant because given \"bTimeInstantURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given time aInstant
            TIMECoordinate aTimeInstantCoordinate = timeOntology?.GetInstantCoordinate(aTimeInstantURI, calendarTRS);
            if (aTimeInstantCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given time bInstant
            TIMECoordinate bTimeInstantCoordinate = timeOntology?.GetInstantCoordinate(bTimeInstantURI, calendarTRS);
            if (bTimeInstantCoordinate == null)
                return false;

            //time:after
            return aTimeInstantCoordinate.CompareTo(bTimeInstantCoordinate) == 1;
        }

        /// <summary>
        /// Checks if the given instant is time:after the given interval
        /// </summary>
        public static bool CheckAfterInterval(OWLOntology timeOntology, RDFResource timeInstantURI, RDFResource timeIntervalURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeInstantURI == null)
                throw new OWLException("Cannot check if instant is after interval because given \"timeInstantURI\" parameter is null");
            if (timeIntervalURI == null)
                throw new OWLException("Cannot check if instant is after interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given time instant
            TIMECoordinate timeInstantCoordinate = timeOntology?.GetInstantCoordinate(timeInstantURI, calendarTRS);
            if (timeInstantCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given time interval's end instant
            TIMECoordinate timeIntervalEndCoordinate = timeOntology?.GetEndOfInterval(timeIntervalURI, calendarTRS);
            if (timeIntervalEndCoordinate == null)
                return false;

            //time:after
            return timeInstantCoordinate.CompareTo(timeIntervalEndCoordinate) == 1;
        }

        /// <summary>
        /// Checks if the given aInstant is time:before the given bInstant
        /// </summary>
        public static bool CheckBefore(OWLOntology timeOntology, RDFResource aTimeInstantURI, RDFResource bTimeInstantURI,
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (aTimeInstantURI == null)
                throw new OWLException("Cannot check if aInstant is before bInstant because given \"aTimeInstantURI\" parameter is null");
            if (bTimeInstantURI == null)
                throw new OWLException("Cannot check if aInstant is before bInstant because given \"bTimeInstantURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given time aInstant
            TIMECoordinate aTimeInstantCoordinate = timeOntology?.GetInstantCoordinate(aTimeInstantURI, calendarTRS);
            if (aTimeInstantCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given time bInstant
            TIMECoordinate bTimeInstantCoordinate = timeOntology?.GetInstantCoordinate(bTimeInstantURI, calendarTRS);
            if (bTimeInstantCoordinate == null)
                return false;

            //time:after
            return aTimeInstantCoordinate.CompareTo(bTimeInstantCoordinate) == -1;
        }

        /// <summary>
        /// Checks if the given instant is time:before the given interval
        /// </summary>
        public static bool CheckBeforeInterval(OWLOntology timeOntology, RDFResource timeInstantURI, RDFResource timeIntervalURI, 
            TIMECalendarReferenceSystem calendarTRS=null)
        {
            #region Guards
            if (timeInstantURI == null)
                throw new OWLException("Cannot check if instant is before interval because given \"timeInstantURI\" parameter is null");
            if (timeIntervalURI == null)
                throw new OWLException("Cannot check if instant is before interval because given \"timeIntervalURI\" parameter is null");
            if (calendarTRS == null)
                calendarTRS = TIMECalendarReferenceSystem.Gregorian;
            #endregion

            //Retrieve the temporal coordinate of the given time instant
            TIMECoordinate timeInstantCoordinate = timeOntology?.GetInstantCoordinate(timeInstantURI, calendarTRS);
            if (timeInstantCoordinate == null)
                return false;

            //Retrieve the temporal coordinate of the given time interval's beginning instant
            TIMECoordinate timeIntervalBeginningCoordinate = timeOntology?.GetBeginningOfInterval(timeIntervalURI, calendarTRS);
            if (timeIntervalBeginningCoordinate == null)
                return false;

            //time:before
            return timeInstantCoordinate.CompareTo(timeIntervalBeginningCoordinate) == -1;
        }
        #endregion
    }
}