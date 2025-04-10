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

namespace OWLSharp
{
    public sealed class OWLException : Exception
    {
        #region Ctors
        public OWLException() { }

        public OWLException(string message) : base(message) { }

        public OWLException(string message, Exception innerException) : base(message, innerException) { }
        #endregion
    }
}