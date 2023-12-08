/*
   Copyright 2012-2024 Marco De Salvo
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

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerRuleNotEqualBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:notEqual basis
    /// </summary>
    public class OWLReasonerRuleNotEqualBuiltIn : OWLReasonerRuleFilterBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:notEqual)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:notEqual");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:notEqual built-in with given arguments
        /// </summary>
        public OWLReasonerRuleNotEqualBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        /// <summary>
        /// Default-ctor to build a swrlb:notEqual built-in with given arguments
        /// </summary>
        public OWLReasonerRuleNotEqualBuiltIn(RDFVariable leftArgument, RDFResource rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        /// <summary>
        /// Default-ctor to build a swrlb:notEqual built-in with given arguments
        /// </summary>
        public OWLReasonerRuleNotEqualBuiltIn(RDFVariable leftArgument, RDFLiteral rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        /// <summary>
        /// Internal-ctor to build a swrlb:notEqual built-in with given arguments
        /// </summary>
        internal OWLReasonerRuleNotEqualBuiltIn(RDFVariable leftArgument, RDFPatternMember rightArgument)
            : base(BuiltInUri, leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");

            BuiltInFilter = new RDFComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.NotEqualTo, leftArgument, rightArgument);
        }
        #endregion
    }
}