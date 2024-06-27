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

using System.Collections.Generic;

namespace OWLSharp.Ontology.Axioms
{
	public static class OWLAxiomHelper
	{
		#region Methods
		public static List<T> RemoveDuplicates<T>(List<T> axioms) where T : OWLAxiom
        {
            List<T> deduplicatedAxioms = new List<T>();
            if (axioms?.Count > 0)
            {
                HashSet<string> lookup = new HashSet<string>();
                axioms.ForEach(axiom =>
                {
					string axiomID = axiom.GetXML();
                    if (!lookup.Contains(axiomID))
                    {
                        lookup.Add(axiomID);
                        deduplicatedAxioms.Add(axiom);
                    }
                });
            }
            return deduplicatedAxioms;
        }
		#endregion
	}
}