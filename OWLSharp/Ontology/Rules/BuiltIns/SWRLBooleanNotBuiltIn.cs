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
    /// SWRLBooleanNotBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#booleanNot built-in
    /// </summary>
    internal static class SWRLBooleanNotBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the logical negation of the second argument
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            //Only booleans can be negated: an unbound or non-boolean operand leaves the result undefined
            if (!(SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]) is RDFTypedLiteral operandLiteral)
                 || !operandLiteral.HasBooleanDatatype()
                 || !bool.TryParse(operandLiteral.Value, out bool operandTruth))
                return null;

            //The negation is the opposite truth value
            return operandTruth ? RDFTypedLiteral.False : RDFTypedLiteral.True;
        }
        #endregion
    }
}
