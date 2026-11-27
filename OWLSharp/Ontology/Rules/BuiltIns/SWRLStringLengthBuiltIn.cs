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
    /// SWRLStringLengthBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#stringLength built-in
    /// </summary>
    internal static class SWRLStringLengthBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the length of the second argument
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            //Only strings can be measured: an unbound or non-string argument leaves the result undefined
            string sourceValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]));
            if (sourceValue == null)
                return null;

            //The length is a count of characters, hence an integer
            return new RDFTypedLiteral(sourceValue.Length.ToString(CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_INTEGER);
        }
        #endregion
    }
}
