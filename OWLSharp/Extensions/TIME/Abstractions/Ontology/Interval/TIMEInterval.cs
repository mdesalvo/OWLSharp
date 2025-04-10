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
    public sealed class TIMEInterval : TIMEEntity
    {
        #region Properties
        public TimeSpan? TimeSpan { get; internal set; }

        public TIMEIntervalDescription Description { get; internal set; }

        public TIMEIntervalDuration Duration { get; internal set; }

        public TIMEInstant Beginning { get; internal set; }

        public TIMEInstant End { get; internal set; }
        #endregion

        #region Ctors
        internal TIMEInterval(RDFResource timeIntervalUri)
            : base(timeIntervalUri) { }

        public TIMEInterval(RDFResource timeInstantUri, TimeSpan timeSpan) : base(timeInstantUri)
            => TimeSpan = timeSpan;

        public TIMEInterval(RDFResource timeInstantUri, TIMEIntervalDescription timeIntervalDescription) : base(timeInstantUri)
            => Description = timeIntervalDescription ?? throw new OWLException("Cannot create time interval because given \"timeIntervalDescription\" parameter is null");

        public TIMEInterval(RDFResource timeInstantUri, TIMEIntervalDuration timeIntervalDuration) : base(timeInstantUri)
            => Duration = timeIntervalDuration ?? throw new OWLException("Cannot create time interval because given \"timeIntervalDuration\" parameter is null");

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