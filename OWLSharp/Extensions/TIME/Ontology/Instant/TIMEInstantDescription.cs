/*
   Copyright 2012-2023 Marco De Salvo
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
    /// Represents the description of date and time structured with separate values for the various elements of a calendar-clock system
    /// </summary>
    public class TIMEInstantDescription : RDFResource,
        IEquatable<TIMEInstantDescription>, IComparable<TIMEInstantDescription>
    {
        #region Properties
        /// <summary>
        /// Encodes the temporal extent of the time instant in standard 6-dimensions
        /// </summary>
        public TIMECoordinate Coordinate { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a time instant description with the given Uri, whose temporal extent is not expressed
        /// </summary>
        internal TIMEInstantDescription(RDFResource timeInstantDescriptionUri)
            : base(timeInstantDescriptionUri?.ToString()) { }

        /// <summary>
        /// Builds a time instant description with the given Uri, whose temporal extent is expressed through the given Gregorian DateTime
        /// </summary>
        public TIMEInstantDescription(RDFResource timeInstantDescriptionUri, DateTime dateTime)
            : this(timeInstantDescriptionUri)
        {
            Coordinate = new TIMECoordinate(dateTime.ToUniversalTime());
        }

        /// <summary>
        /// Builds a time instant description with the given Uri, whose temporal extent is expressed through the given instant coordinate
        /// </summary>
        public TIMEInstantDescription(RDFResource timeInstantDescriptionUri, TIMECoordinate timeInstantCoordinate)
            : this(timeInstantDescriptionUri)
        {
            #region Guards
            if (timeInstantCoordinate == null)
                throw new OWLException("Cannot create description of time instant because given \"timeInstantCoordinate\" parameter is null");
            #endregion

            Coordinate = timeInstantCoordinate;
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Compares this instant description with the given one
        /// </summary>
        public int CompareTo(TIMEInstantDescription other)
            => Coordinate.CompareTo(other?.Coordinate);

        /// <summary>
        /// Checks if this instant description is the same as the given one
        /// </summary>
        public bool Equals(TIMEInstantDescription other)
            => CompareTo(other) == 0;
        #endregion
    }
}