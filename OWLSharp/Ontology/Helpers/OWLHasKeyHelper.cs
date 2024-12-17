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

using System.Linq;

namespace OWLSharp.Ontology
{
    public static class OWLHasKeyHelper
    {
        #region Methods
        public static bool CheckHasKey(this OWLOntology ontology, OWLHasKey hasKey)
            => ontology?.KeyAxioms.Any(ax => string.Equals(ax.GetXML(), hasKey?.GetXML())) ?? false;

        public static void DeclareHasKey(this OWLOntology ontology, OWLHasKey hasKey)
        {
            #region Guards
            if (hasKey == null)
                throw new OWLException("Cannot declare has key because given \"hasKey\" parameter is null");
            #endregion

            if (!CheckHasKey(ontology, hasKey))
                ontology?.KeyAxioms.Add(hasKey);
        }
        #endregion
    }
}