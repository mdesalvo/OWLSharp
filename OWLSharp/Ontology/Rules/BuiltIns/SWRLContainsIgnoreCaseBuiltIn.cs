/*
   Copyright 2014-2026 Marco De Salvo

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
using System;
using RDFSharp.Query;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLContainsIgnoreCaseBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#containsIgnoreCase built-in
    /// </summary>
    internal static class SWRLContainsIgnoreCaseBuiltIn
    {
        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember leftPatternMember = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[0]);
            if (leftPatternMember == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMember = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]);
            if (rightPatternMember == null)
                return false;

            //Only strings take part in the test: a value that is not a string never satisfies the built-in
            string leftPMValue = SWRLHelper.GetStringValue(leftPatternMember);
            string rightPMValue = SWRLHelper.GetStringValue(rightPatternMember);
            return leftPMValue != null && rightPMValue != null && leftPMValue.IndexOf(rightPMValue, StringComparison.OrdinalIgnoreCase) > -1;
        }
        #endregion
    }
}