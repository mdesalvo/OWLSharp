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
using System;

namespace OWLSharp.Extensions.TIME
{
    public class TIMEIntervalDescription : RDFResource, IEquatable<TIMEIntervalDescription>, IComparable<TIMEIntervalDescription>
    {
        #region Properties
        public TIMEExtent Extent { get; internal set; }
        #endregion

        #region Ctors
        internal TIMEIntervalDescription(RDFResource timeIntervalDescriptionUri) : base(timeIntervalDescriptionUri?.ToString()) { }

        public TIMEIntervalDescription(RDFResource timeIntervalDescriptionUri, TimeSpan timeSpan) : this(timeIntervalDescriptionUri)
            => Extent = new TIMEExtent(timeSpan);

        public TIMEIntervalDescription(RDFResource timeInstantDescriptionUri, TIMEExtent timeIntervalExtent) : this(timeInstantDescriptionUri)
            => Extent = timeIntervalExtent?? throw new OWLException("Cannot create description of time interval because given \"timeIntervalExtent\" parameter is null");
        #endregion

        #region Interfaces
        public int CompareTo(TIMEIntervalDescription other)
            => Extent.CompareTo(other?.Extent);

        public bool Equals(TIMEIntervalDescription other)
            => CompareTo(other) == 0;
        #endregion
    }
}