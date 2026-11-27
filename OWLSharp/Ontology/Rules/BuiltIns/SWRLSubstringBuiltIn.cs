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
using System.Globalization;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLSubstringBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#substring built-in
    /// </summary>
    internal static class SWRLSubstringBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the part of the second argument that starts at the position given by the third one with the optional length of the fourth one
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3 && builtInArguments?.Count != 4)
                throw new ArgumentException("it requires exactly 3 or exactly 4 arguments");
            #endregion

            //Only strings can be cut: an unbound or non-string argument leaves the result undefined
            string sourceValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]));
            if (sourceValue == null)
                return null;

            //The start position is a 1-based integer, as in XPath fn:substring: it may point just after the end of the source
            if (!(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[2]) is RDFTypedLiteral startLiteral)
                 || !startLiteral.HasDecimalDatatype()
                 || !int.TryParse(startLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int startPosition)
                 || startPosition < 1 || startPosition > sourceValue.Length + 1)
                return null;

            //Without a length the result runs up to the end of the source
            if (builtInArguments.Count == 3)
                return new RDFPlainLiteral(sourceValue.Substring(startPosition - 1));

            //The length is a non-negative integer that must fit within the source after the start position
            if (!(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[3]) is RDFTypedLiteral lengthLiteral)
                 || !lengthLiteral.HasDecimalDatatype()
                 || !int.TryParse(lengthLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int length)
                 || length < 0 || length > sourceValue.Length - startPosition + 1)
                return null;

            //The result is the requested slice of the source
            return new RDFPlainLiteral(sourceValue.Substring(startPosition - 1, length));
        }
        #endregion
    }
}
