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
        private const string NormalizeSpacesPattern = @"\s{2,}";
        private static readonly Lazy<Regex> NormalizeSpacesRegex = new Lazy<Regex>(() => new Regex(NormalizeSpacesPattern, RegexOptions.Compiled));
        #endregion

        #region Methods
        /// <summary>
        /// Computes the second argument with every run of whitespace collapsed into a single space
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

            //Every run of whitespace becomes a single space
            return new RDFPlainLiteral(NormalizeSpacesRegex.Value.Replace(sourceValue, NormalizeSpacesReplacement));
        }
        #endregion
    }
}
