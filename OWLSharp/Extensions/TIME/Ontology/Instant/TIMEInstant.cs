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
    /// Represents a temporal entity without extent or duration (more precisely, a "moment" in time)
    /// </summary>
    public class TIMEInstant : TIMEEntity
    {
        #region Properties
        /// <summary>
        /// Instant expressed using xsd:dateTimeStamp
        /// </summary>
        public DateTime? DateTime { get; internal set; }

        /// <summary>
        /// Instant expressed using a structured 6-component description
        /// </summary>
        public TIMEInstantDescription Description { get; internal set; }

        /// <summary>
        /// Instant expressed as a numeric temporal coordinate
        /// </summary>
        public TIMEInstantPosition Position { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a time instant with the given Uri, whose temporal extent is not expressed
        /// </summary>
        internal TIMEInstant(RDFResource timeInstantUri)
            : base(timeInstantUri) { }

        /// <summary>
        /// Builds a time instant with the given Uri, whose temporal extent is expressed through the given DateTime
        /// </summary>
        public TIMEInstant(RDFResource timeInstantUri, DateTime dateTime)
            : base(timeInstantUri)
        {
            DateTime = dateTime.ToUniversalTime();
        }

        /// <summary>
        /// Builds a time instant with the given Uri, whose temporal extent is expressed through the given description
        /// </summary>
        public TIMEInstant(RDFResource timeInstantUri, TIMEInstantDescription timeInstantDescription)
            : base(timeInstantUri)
        {
            #region Guards
            if (timeInstantDescription == null)
                throw new OWLException("Cannot create time instant because given \"timeInstantDescription\" parameter is null");
            #endregion

            Description = timeInstantDescription;
        }

        /// <summary>
        /// Builds a time instant with the given Uri, whose temporal extent is expressed through the given position
        /// </summary>
        public TIMEInstant(RDFResource timeInstantUri, TIMEInstantPosition timeInstantPosition)
            : base(timeInstantUri)
        {
            #region Guards
            if (timeInstantPosition == null)
                throw new OWLException("Cannot create time instant because given \"timeInstantPosition\" parameter is null");
            #endregion

            Position = timeInstantPosition;
        }
        #endregion
    }
}