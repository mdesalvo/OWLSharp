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
    /// SWRLDivideBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#divide built-in
    /// </summary>
    internal static class SWRLDivideBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the quotient of the second and the third argument
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3)
                throw new ArgumentException("it requires exactly 3 arguments");
            #endregion

            //Only numbers take part in the computation: an unbound or non-numeric operand leaves the result undefined
            if (!SWRLHelper.TryGetNumericOperands(antecedentResultsRow, builtInArguments, out SWRLNumericFamily numericFamily, out decimal[] exactOperands, out double[] approximateOperands))
                return null;

            //A division by zero has no quotient
            if (approximateOperands[1] == 0d)
                return null;

            //A float operand gives a double quotient, otherwise the quotient is exact and is a decimal even between integers
            return numericFamily == SWRLNumericFamily.Float
                ? SWRLHelper.ToNumericLiteral(approximateOperands[0] / approximateOperands[1])
                : SWRLHelper.ToNumericLiteral(exactOperands[0] / exactOperands[1], SWRLNumericFamily.Decimal);
        }
        #endregion
    }
}
