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
    /// SWRLNormalizeSpaceBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#normalizeSpace built-in
    /// </summary>
    internal static class SWRLNormalizeSpaceBuiltIn
    {
        #region Statics
        private const string NormalizeSpacesReplacement = " ";
        //Whitespace is exactly the four characters of the XML production S (#x20, #x9, #xD, #xA): XPath 3.1 fn:normalize-space (5.4.5) leaves Unicode spaces such as U+00A0 alone
        private const string NormalizeSpacesPattern = @"[\x20\x09\x0D\x0A]+";
        //The same four characters are the only ones fn:normalize-space strips at both ends of the string
        private static readonly char[] NormalizeSpacesTrimCharacters = { ' ', '\t', '\r', '\n' };
        private static readonly Lazy<Regex> NormalizeSpacesRegex = new Lazy<Regex>(() => new Regex(NormalizeSpacesPattern, RegexOptions.Compiled));
        #endregion

        #region Methods
        /// <summary>
        /// Computes the second argument without leading and trailing whitespace and with every inner run of whitespace collapsed into a single space
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            //Only strings can be normalized: an unbound or non-string argument leaves the result undefined
            string sourceValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]));
            if (sourceValue == null)
                return null;

            //Leading and trailing whitespace is dropped first (only the four XML whitespace characters) then every inner run becomes a single space
            return new RDFPlainLiteral(NormalizeSpacesRegex.Value.Replace(sourceValue.Trim(NormalizeSpacesTrimCharacters), NormalizeSpacesReplacement));
        }
        #endregion
    }
}
