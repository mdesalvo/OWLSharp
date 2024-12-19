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
    public static class OWLDatatypeDefinitionHelper
    {
        #region Methods
        public static bool CheckHasDatatypeDefinition(this OWLOntology ontology, OWLDatatypeDefinition datatypeDefinition)
            => ontology?.DatatypeDefinitionAxioms.Any(ax => string.Equals(ax.GetXML(), datatypeDefinition?.GetXML())) ?? false;

        public static void DeclareDatatypeDefinition(this OWLOntology ontology, OWLDatatypeDefinition datatypeDefinition)
        {
            #region Guards
            if (datatypeDefinition == null)
                throw new OWLException("Cannot declare datatype definition because given \"datatypeDefinition\" parameter is null");
            #endregion

            if (!CheckHasDatatypeDefinition(ontology, datatypeDefinition))
                ontology?.DatatypeDefinitionAxioms.Add(datatypeDefinition);
        }
        #endregion
    }
}