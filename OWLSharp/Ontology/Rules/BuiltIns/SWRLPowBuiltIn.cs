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
    /// SWRLPowBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#pow built-in
    /// </summary>
    internal static class SWRLPowBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the second argument raised to the power of the third one
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

            //An integer base raised to a non-negative integer exponent stays an exact integer
            if (numericFamily == SWRLNumericFamily.Integer && exactOperands[1] >= 0m)
            {
                //Square-and-multiply keeps the cost logarithmic in the exponent (beyond the decimal range the power overflows and the row is dropped)
                decimal power = 1m, factor = exactOperands[0];
                for (decimal exponent = exactOperands[1]; exponent > 0m; exponent = decimal.Truncate(exponent / 2m))
                {
                    //An odd exponent takes the current factor into the power
                    if (decimal.Remainder(exponent, 2m) == 1m)
                        power *= factor;
                    //The factor is squared only while exponent bits remain, so that a power within range never overflows on its last square
                    if (exponent > 1m)
                        factor *= factor;
                }
                return SWRLHelper.ToNumericLiteral(power, SWRLNumericFamily.Integer);
            }

            //A decimal or negative exponent, or a non-integer base, gives a power computed in double precision
            return SWRLHelper.ToNumericLiteral(Math.Pow(approximateOperands[0], approximateOperands[1]));
        }
        #endregion
    }
}
