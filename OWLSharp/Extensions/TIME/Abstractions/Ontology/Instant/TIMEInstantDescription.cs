/*
   Copyright 2014-2025 Marco De Salvo
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
    public class TIMEInstantDescription : RDFResource, IEquatable<TIMEInstantDescription>, IComparable<TIMEInstantDescription>
    {
        #region Properties
        public TIMECoordinate Coordinate { get; internal set; }
        #endregion

        #region Ctors
        internal TIMEInstantDescription(RDFResource timeInstantDescriptionUri) : base(timeInstantDescriptionUri?.ToString()) { }

        public TIMEInstantDescription(RDFResource timeInstantDescriptionUri, DateTime dateTime) : this(timeInstantDescriptionUri)
            => Coordinate = new TIMECoordinate(dateTime.ToUniversalTime());

        public TIMEInstantDescription(RDFResource timeInstantDescriptionUri, TIMECoordinate timeInstantCoordinate) : this(timeInstantDescriptionUri)
            => Coordinate = timeInstantCoordinate ?? throw new OWLException("Cannot create description of time instant because given \"timeInstantCoordinate\" parameter is null");
        #endregion

        #region Interfaces
        public int CompareTo(TIMEInstantDescription other)
            => Coordinate.CompareTo(other?.Coordinate);

        public bool Equals(TIMEInstantDescription other)
            => CompareTo(other) == 0;
        #endregion
    }
}