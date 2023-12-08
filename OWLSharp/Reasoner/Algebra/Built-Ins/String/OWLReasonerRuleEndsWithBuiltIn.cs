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

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerRuleEndsWithBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:endsWith basis
    /// </summary>
    public class OWLReasonerRuleEndsWithBuiltIn : OWLReasonerRuleFilterBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:endsWith)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:endsWith");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:endsWith built-in with given arguments
        /// </summary>
        public OWLReasonerRuleEndsWithBuiltIn(RDFVariable leftArgument, string endString)
            : base(BuiltInUri, leftArgument, null)
        {
            if (endString == null)
                throw new OWLException("Cannot create built-in because given \"endString\" parameter is null");

            RightArgument = new RDFPlainLiteral(endString);
            BuiltInFilter = new RDFRegexFilter(leftArgument, new Regex($"{endString}$"));
        }
        #endregion
    }
}