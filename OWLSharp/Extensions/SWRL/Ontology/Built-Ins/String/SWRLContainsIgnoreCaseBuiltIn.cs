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
using System.Text.RegularExpressions;

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// SWRLContainsIgnoreCaseBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:containsIgnoreCase basis
    /// </summary>
    public class SWRLContainsIgnoreCaseBuiltIn : SWRLFilterBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:containsIgnoreCase)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:containsIgnoreCase");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:containsIgnoreCase built-in with given arguments
        /// </summary>
        public SWRLContainsIgnoreCaseBuiltIn(RDFVariable leftArgument, string containString)
            : base(BuiltInUri, leftArgument, null)
        {
            #region Guards
            if (containString == null)
                throw new OWLException("Cannot create built-in because given \"containString\" parameter is null");
            #endregion

            RightArgument = new RDFPlainLiteral(containString);
            BuiltInFilter = new RDFRegexFilter(leftArgument, new Regex($"{containString}", RegexOptions.IgnoreCase));
        }
        #endregion
    }
}