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
using System.Collections.Generic;

namespace OWLSharp.Ontology
{
    public static class OWLOntologyHelper
    {
        #region Properties
        internal static Dictionary<string, (OWLOntology Ontology,DateTime ExpireTimestamp)> ImportCache { get; set; }
        #endregion

        #region Ctors
        static OWLOntologyHelper()
        {
            if (ImportCache == null)
                ImportCache = new Dictionary<string, (OWLOntology,DateTime)>();
        }
        #endregion
    }
}