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
    public static class SWRLHelper
    {
        #region Methods
        public static bool CheckHasRule(this OWLOntology ontology, SWRLRule rule)
            => ontology?.Rules.Any(rl => string.Equals(rl.GetXML(), rule?.GetXML())) ?? false;

        public static void AddRule(this OWLOntology ontology, SWRLRule rule)
        {
            #region Guards
            if (rule == null)
                throw new OWLException("Cannot declare SWRL rule because given \"rule\" parameter is null");
            #endregion

            if (!CheckHasRule(ontology, rule))
                ontology?.Rules.Add(rule);
        }
        #endregion
    }
}