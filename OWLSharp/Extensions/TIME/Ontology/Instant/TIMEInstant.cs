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
    public class TIMEInstant : TIMEEntity
    {
        #region Properties
        public DateTime? DateTime { get; internal set; }

        public TIMEInstantDescription Description { get; internal set; }

        public TIMEInstantPosition Position { get; internal set; }
        #endregion

        #region Ctors
        internal TIMEInstant(RDFResource timeInstantUri)
            : base(timeInstantUri) { }

        public TIMEInstant(RDFResource timeInstantUri, DateTime dateTime) : base(timeInstantUri)
            => DateTime = dateTime.ToUniversalTime();

        public TIMEInstant(RDFResource timeInstantUri, TIMEInstantDescription timeInstantDescription) : base(timeInstantUri)
            => Description = timeInstantDescription ?? throw new OWLException("Cannot create time instant because given \"timeInstantDescription\" parameter is null");

        public TIMEInstant(RDFResource timeInstantUri, TIMEInstantPosition timeInstantPosition) : base(timeInstantUri)
            => Position = timeInstantPosition ?? throw new OWLException("Cannot create time instant because given \"timeInstantPosition\" parameter is null");
        #endregion
    }
}