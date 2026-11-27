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
using System.Text.RegularExpressions;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLReplaceBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#replace built-in
    /// </summary>
    internal static class SWRLReplaceBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the second argument with every match of the third one replaced by the fourth one
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 4)
                throw new ArgumentException("it requires exactly 4 arguments");
            #endregion

            //Only strings can be searched: an unbound or non-string argument leaves the result undefined
            string sourceValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]));
            //Only strings can be used as patterns: an unbound or non-string argument leaves the result undefined
            string patternValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[2]));
            //Only strings can be used as replacements: an unbound or non-string argument leaves the result undefined
            string replacementValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[3]));
            if (sourceValue == null || patternValue == null || replacementValue == null)
                return null;

            //Every match of the pattern is replaced: an invalid pattern raises an error that aborts the evaluation of the rule
            return new RDFPlainLiteral(Regex.Replace(sourceValue, patternValue, replacementValue));
        }
        #endregion
    }
}
