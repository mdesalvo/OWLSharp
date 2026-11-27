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
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLSubstringAfterBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#substringAfter built-in
    /// </summary>
    internal static class SWRLSubstringAfterBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the part of the second argument that follows the first occurrence of the third one
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3)
                throw new ArgumentException("it requires exactly 3 arguments");
            #endregion

            //Only strings can be split: an unbound or non-string argument leaves the result undefined
            string sourceValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]));
            //Only strings can separate: an unbound or non-string argument leaves the result undefined
            string separatorValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[2]));
            if (sourceValue == null || separatorValue == null)
                return null;

            //An empty separator gives an empty result, as this built-in has always done (XPath fn:substring-after would give the whole source)
            if (separatorValue.Length == 0)
                return new RDFPlainLiteral(string.Empty);

            //The text after the first separator is the result: it is empty when the separator does not occur
            int separatorIndex = sourceValue.IndexOf(separatorValue, StringComparison.Ordinal);
            return new RDFPlainLiteral(separatorIndex == -1 ? string.Empty : sourceValue.Substring(separatorIndex + separatorValue.Length));
        }
        #endregion
    }
}
