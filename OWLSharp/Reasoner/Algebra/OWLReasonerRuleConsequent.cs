/*
   Copyright 2012-2023 Marco De Salvo

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
using System.Data;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerRuleConsequent represents the consequent of a reasoner rule expressed in SWRL
    /// </summary>
    public class OWLReasonerRuleConsequent
    {
        #region Properties
        /// <summary>
        /// Atoms composing the consequent
        /// </summary>
        internal List<OWLReasonerRuleAtom> Atoms { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty consequent
        /// </summary>
        public OWLReasonerRuleConsequent()
            => Atoms = new List<OWLReasonerRuleAtom>();
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the consequent
        /// </summary>
        public override string ToString()
            => string.Join(" ^ ", Atoms);
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given atom to the consequent
        /// </summary>
        public OWLReasonerRuleConsequent AddAtom(OWLReasonerRuleAtom atom)
        {
            if (atom != null && !Atoms.Any(x => string.Equals(x.ToString(), atom.ToString(), StringComparison.OrdinalIgnoreCase)))
                Atoms.Add(atom);
            return this;
        }

        /// <summary>
        /// Evaluates the consequent in the context of the given antecedent results
        /// </summary>
        internal OWLReasonerReport Evaluate(DataTable antecedentResults, OWLOntology ontology)
        {
            OWLReasonerReport report = new OWLReasonerReport();

            //Execute the consequent atoms
            Atoms.ForEach(atom => report.MergeEvidences(atom.EvaluateOnConsequent(antecedentResults, ontology)));

            //Return the consequent result
            return report;
        }
        #endregion
    }
}