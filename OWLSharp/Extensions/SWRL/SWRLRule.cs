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
using System.Threading.Tasks;
using OWLSharp.Ontology;
using OWLSharp.Reasoner;

namespace OWLSharp.Extensions.SWRL
{
    public class SWRLRule
    {
        #region Properties
        public SWRLAntecedent Antecedent { get; internal set; }
		public SWRLConsequent Consequent { get; internal set; }
        #endregion

        #region Ctors
        public SWRLRule(SWRLAntecedent antecedent, SWRLConsequent consequent)
        {
            Antecedent = antecedent ?? throw new OWLException("Cannot create SWRL rule because given \"antecedent\" parameter is null");
            Consequent = consequent ?? throw new OWLException("Cannot create SWRL rule because given \"consequent\" parameter is null");
        }
        #endregion

        #region Methods
        public Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology)
			=> Task.Run(() => Consequent.Evaluate(
								Antecedent.Evaluate(ontology), ontology));
        #endregion
    }
}
