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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// SWRLBuiltIn represents a specific category of SWRL atoms filtering inferences of a rule's antecedent
    /// </summary>
    public abstract class SWRLBuiltIn : SWRLAtom
    {
        #region Ctors
        /// <summary>
        /// Default-ctor to build a built-in with given predicate and arguments
        /// </summary>
        internal SWRLBuiltIn(RDFResource predicate, RDFPatternMember leftArgument, RDFPatternMember rightArgument)
            : base(predicate, leftArgument, rightArgument) { }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of the given antecedent results
        /// </summary>
        internal abstract DataTable Evaluate(DataTable antecedentResults, OWLOntology ontology);

        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology) => null;
        internal override OWLReasonerReport EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology) => null;
        #endregion
    }
}