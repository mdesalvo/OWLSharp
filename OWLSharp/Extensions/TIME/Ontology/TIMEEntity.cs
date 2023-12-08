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
    /// Represents the description of a temporal extension in terms of an instant or an interval
    /// </summary>
    public class TIMEEntity : RDFResource
    {
        #region Ctors
        /// <summary>
        /// Builds a time entity with given Uri
        /// </summary>
        internal TIMEEntity(RDFResource timeEntityUri)
            : base(timeEntityUri?.ToString()) { }
        #endregion
    }
}