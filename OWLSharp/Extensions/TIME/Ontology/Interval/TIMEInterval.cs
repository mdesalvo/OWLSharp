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
using System;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// Represents a temporal entity with extent or duration (more precisely, a "period" in time)
    /// </summary>
    public class TIMEInterval : TIMEEntity
    {
        #region Properties
        /// <summary>
        /// Interval expressed using xsd:Duration
        /// </summary>
        public TimeSpan? TimeSpan { get; internal set; }

        /// <summary>
        /// Interval expressed using a structured 7-component description
        /// </summary>
        public TIMEIntervalDescription Description { get; internal set; }

        /// <summary>
        /// Interval expressed as a numeric scaled value
        /// </summary>
        public TIMEIntervalDuration Duration { get; internal set; }

        /// <summary>
        /// Instant representing the beginning of the interval
        /// </summary>
        public TIMEInstant Beginning { get; internal set; }

        /// <summary>
        /// Instant representing the end of the interval
        /// </summary>
        public TIMEInstant End { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a time interval with given Uri, whose temporal extent is not expressed
        /// </summary>
        internal TIMEInterval(RDFResource timeIntervalUri)
            : base(timeIntervalUri) { }

        /// <summary>
        /// Builds a time interval with the given Uri, whose temporal extent is expressed through by the given TimeSpan
        /// </summary>
        public TIMEInterval(RDFResource timeInstantUri, TimeSpan timeSpan)
            : base(timeInstantUri)
        {
            TimeSpan = timeSpan;
        }

        /// <summary>
        /// Builds a time interval with the given Uri, whose temporal extent is expressed through the given description
        /// </summary>
        public TIMEInterval(RDFResource timeInstantUri, TIMEIntervalDescription timeIntervalDescription)
            : base(timeInstantUri)
        {
            #region Guards
            if (timeIntervalDescription == null)
                throw new OWLException("Cannot create time interval because given \"timeIntervalDescription\" parameter is null");
            #endregion

            Description = timeIntervalDescription;
        }

        /// <summary>
        /// Builds a time interval with the given Uri, whose temporal extent is expressed through the given duration 
        /// </summary>
        public TIMEInterval(RDFResource timeInstantUri, TIMEIntervalDuration timeIntervalDuration)
            : base(timeInstantUri)
        {
            #region Guards
            if (timeIntervalDuration == null)
                throw new OWLException("Cannot create time interval because given \"timeIntervalDuration\" parameter is null");
            #endregion

            Duration = timeIntervalDuration;
        }

        /// <summary>
        /// Builds a time interval with the given Uri, whose temporal extent is expressed through the given boundary instants
        /// </summary>
        public TIMEInterval(RDFResource timeInstantUri, TIMEInstant timeInstantBeginning, TIMEInstant timeInstantEnd)
            : base(timeInstantUri)
        {
            #region Guards
            if (timeInstantBeginning == null && timeInstantEnd == null)
                throw new OWLException("Cannot create time interval because both \"timeInstantBeginning\" and \"timeInstantEnd\" parameters are null");
            #endregion

            Beginning = timeInstantBeginning;
            End = timeInstantEnd;
        }
        #endregion
    }
}