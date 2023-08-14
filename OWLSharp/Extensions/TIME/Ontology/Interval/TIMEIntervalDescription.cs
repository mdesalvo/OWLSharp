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
    /// Represents the description of a temporal extent structured with separate values for the various elements of a calendar-clock system 
    /// </summary>
    public class TIMEIntervalDescription : RDFResource,
        IEquatable<TIMEIntervalDescription>, IComparable<TIMEIntervalDescription>
    {
        #region Properties
        /// <summary>
        /// Encodes the temporal extent of the time interval in standard 7-dimensions
        /// </summary>
        public TIMEExtent Extent { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a time interval description with the given Uri, whose temporal extent is not expressed
        /// </summary>
        internal TIMEIntervalDescription(RDFResource timeIntervalDescriptionUri)
            : base(timeIntervalDescriptionUri?.ToString()) { }

        /// <summary>
        /// Builds a time interval description with the given Uri, whose temporal extent is expressed through the given TimeSpan
        /// </summary>
        public TIMEIntervalDescription(RDFResource timeIntervalDescriptionUri, TimeSpan timeSpan)
            : this(timeIntervalDescriptionUri)
        {
            Extent = new TIMEExtent(timeSpan);
        }

        /// <summary>
        /// Builds a time interval description with the given Uri, whose temporal extent is expressed through the given interval length
        /// </summary>
        public TIMEIntervalDescription(RDFResource timeInstantDescriptionUri, TIMEExtent timeIntervalExtent)
            : this(timeInstantDescriptionUri)
        {
            #region Guards
            if (timeIntervalExtent == null)
                throw new OWLException("Cannot create description of time interval because given \"timeIntervalExtent\" parameter is null");
            #endregion

            Extent = timeIntervalExtent;
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Compares this interval description with the given one
        /// </summary>
        public int CompareTo(TIMEIntervalDescription other)
            => Extent.CompareTo(other?.Extent);

        /// <summary>
        /// Checks if this interval description is the same as the given one
        /// </summary>
        public bool Equals(TIMEIntervalDescription other)
            => CompareTo(other) == 0;
        #endregion
    }
}