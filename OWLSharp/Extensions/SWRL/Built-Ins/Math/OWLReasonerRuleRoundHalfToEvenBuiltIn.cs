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
using System.Text;

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// OWLReasonerRuleRoundHalfToEvenBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:roundHalfToEven basis
    /// </summary>
    public class OWLReasonerRuleRoundHalfToEvenBuiltIn : OWLReasonerRuleMathBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:roundHalfToEven)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:roundHalfToEven");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:roundHalfToEven built-in with given arguments
        /// </summary>
        public OWLReasonerRuleRoundHalfToEvenBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument)
            : base(BuiltInUri, leftArgument, rightArgument, double.NaN)
        {
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the built-in
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Predicate
            sb.Append(RDFModelUtilities.GetShortUri(Predicate.URI));

            //Arguments
            sb.Append($"({LeftArgument},{RightArgument})");

            return sb.ToString();
        }
        #endregion
    }
}