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
using System.Text;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLStringConcatBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#stringConcat built-in
    /// </summary>
    internal static class SWRLStringConcatBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the concatenation of the arguments after the first
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count < 2)
                throw new ArgumentException("it requires at least 2 arguments");
            #endregion

            //The concatenation grows with every operand from the second argument on
            StringBuilder concatenation = new StringBuilder();
            for (int i = 1; i < builtInArguments.Count; i++)
            {
                //Only strings can be concatenated: an unbound or non-string operand leaves the result undefined
                string operandValue = SWRLHelper.GetStringValue(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[i]));
                if (operandValue == null)
                    return null;

                //Operands are joined in the order of the arguments
                concatenation.Append(operandValue);
            }

            //The concatenated text is a plain literal without language
            return new RDFPlainLiteral(concatenation.ToString());
        }
        #endregion
    }
}
