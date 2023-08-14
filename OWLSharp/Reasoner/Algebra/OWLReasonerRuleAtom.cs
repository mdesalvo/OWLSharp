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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Data;
using System.Text;

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerRuleAtom represents a SWRL atom contained in a reasoner rule's antecedent/consequent
    /// </summary>
    public abstract class OWLReasonerRuleAtom
    {
        #region Properties
        /// <summary>
        /// Checks if the atom represents a built-in
        /// </summary>
        public bool IsBuiltIn => this is OWLReasonerRuleBuiltIn;

        /// <summary>
        /// Represents the atom's predicate
        /// </summary>
        public RDFResource Predicate { get; internal set; }

        /// <summary>
        /// Represents the left argument given to the atom's predicate
        /// </summary>
        public RDFPatternMember LeftArgument { get; internal set; }

        /// <summary>
        /// Represents the (optional) right argument given to the atom's predicate
        /// </summary>
        public RDFPatternMember RightArgument { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an atom with given predicate and arguments
        /// </summary>
        public OWLReasonerRuleAtom(RDFResource predicate, RDFPatternMember leftArgument, RDFPatternMember rightArgument)
        {
            if (predicate == null)
                throw new OWLException("Cannot create atom because given \"predicate\" parameter is null");
            if (leftArgument == null)
                throw new OWLException("Cannot create atom because given \"leftArgument\" parameter is null");

            Predicate = predicate;
            LeftArgument = leftArgument;
            RightArgument = rightArgument;
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the atom
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Predicate
            sb.Append(RDFModelUtilities.GetShortUri(Predicate.URI));

            //Arguments
            sb.Append($"({LeftArgument}");
            if (RightArgument != null)
            {
                //When the right argument is a resource, it is printed in a SWRL-shortened form
                if (RightArgument is RDFResource rightArgumentResource)
                    sb.Append($",{RDFModelUtilities.GetShortUri(rightArgumentResource.URI)}");

                //Other cases of right argument (variable, literal) are printed in normal form
                else
                    sb.Append($",{RDFQueryPrinter.PrintPatternMember(RightArgument, RDFNamespaceRegister.Instance.Register)}");
            }
            sb.Append(")");

            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluates the atom in the context of an antecedent
        /// </summary>
        internal abstract DataTable EvaluateOnAntecedent(OWLOntology ontology);

        /// <summary>
        /// Evaluates the atom in the context of a consequent
        /// </summary>
        internal abstract OWLReasonerReport EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology);
        #endregion
    }
}