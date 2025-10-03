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

using System;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLException represents an exception thrown during manipulation of SWRL rules
    /// </summary>
    [Serializable]
    public sealed class SWRLException : Exception
    {
        #region Ctors
        /// <summary>
        /// Builds an empty SWRLException
        /// </summary>
        public SWRLException() { }

        /// <summary>
        /// Builds a SWRLException with the given message
        /// </summary>
        public SWRLException(string message) : base(message) { }

        /// <summary>
        /// Builds a SWRLException with the given message and inner exception
        /// </summary>
        public SWRLException(string message, Exception innerException) : base(message, innerException) { }
        #endregion
    }
}