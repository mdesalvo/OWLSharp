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

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// SWRLGreaterThanBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:greaterThan basis
    /// </summary>
    public class SWRLGreaterThanBuiltIn : SWRLFilterBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:greaterThan)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:greaterThan");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:greaterThan built-in with given arguments
        /// </summary>
        public SWRLGreaterThanBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        /// <summary>
        /// Default-ctor to build a swrlb:greaterThan built-in with given arguments
        /// </summary>
        public SWRLGreaterThanBuiltIn(RDFVariable leftArgument, RDFResource rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        /// <summary>
        /// Default-ctor to build a swrlb:greaterThan built-in with given arguments
        /// </summary>
        public SWRLGreaterThanBuiltIn(RDFVariable leftArgument, RDFLiteral rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        /// <summary>
        /// Internal-ctor to build a swrlb:greaterThan built-in with given arguments
        /// </summary>
        internal SWRLGreaterThanBuiltIn(RDFVariable leftArgument, RDFPatternMember rightArgument)
            : base(BuiltInUri, leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");

            BuiltInFilter = new RDFComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.GreaterThan, leftArgument, rightArgument);
        }
        #endregion
    }
}