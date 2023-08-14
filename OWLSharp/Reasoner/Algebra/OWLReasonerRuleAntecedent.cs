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

using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerRuleAntecedent represents the antecedent of a reasoner rule expressed in SWRL
    /// </summary>
    public class OWLReasonerRuleAntecedent
    {
        #region Properties
        /// <summary>
        /// Atoms composing the antecedent
        /// </summary>
        internal List<OWLReasonerRuleAtom> Atoms { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty antecedent
        /// </summary>
        public OWLReasonerRuleAntecedent()
            => Atoms = new List<OWLReasonerRuleAtom>();
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the antecedent
        /// </summary>
        public override string ToString()
            => string.Join(" ^ ", Atoms);
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given atom to the antecedent
        /// </summary>
        public OWLReasonerRuleAntecedent AddAtom(OWLReasonerRuleAtom atom)
        {
            if (atom != null && !Atoms.Any(x => string.Equals(x.ToString(), atom.ToString(), StringComparison.OrdinalIgnoreCase)))
                Atoms.Add(atom);
            return this;
        }

        /// <summary>
        /// Adds the given built-in to the antecedent
        /// </summary>
        public OWLReasonerRuleAntecedent AddBuiltIn(OWLReasonerRuleBuiltIn builtIn)
            => AddAtom(builtIn);

        /// <summary>
        /// Evaluates the antecedent in the context of the given ontology
        /// </summary>
        internal DataTable Evaluate(OWLOntology ontology)
        {
            //Execute the antecedent atoms
            List<DataTable> atomResults = new List<DataTable>();
            Atoms.Where(atom => !atom.IsBuiltIn)
                 .ToList()
                 .ForEach(atom => atomResults.Add(atom.EvaluateOnAntecedent(ontology)));

            //Join results of antecedent atoms
            DataTable antecedentResult = RDFQueryEngine.CombineTables(atomResults, false);

            //Execute the antecedent built-ins
            Atoms.Where(atom => atom.IsBuiltIn)
                 .OfType<OWLReasonerRuleBuiltIn>()
                 .ToList()
                 .ForEach(builtin => antecedentResult = builtin.Evaluate(antecedentResult, ontology));

            //Return the antecedent result
            return antecedentResult;
        }
        #endregion
    }
}