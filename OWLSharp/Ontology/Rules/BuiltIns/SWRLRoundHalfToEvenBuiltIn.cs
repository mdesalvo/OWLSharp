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
    /// SWRLRoundHalfToEvenBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#roundHalfToEven built-in
    /// </summary>
    internal static class SWRLRoundHalfToEvenBuiltIn
    {
        #region Methods
        /// <summary>
        /// Rounds the second argument to the closest integral value with halves rounded to the even neighbour
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            //Only numbers take part in the computation: an unbound or non-numeric operand leaves the result undefined
            if (!SWRLHelper.TryGetNumericOperands(antecedentResultsRow, builtInArguments, out SWRLNumericFamily numericFamily, out decimal[] exactOperands, out double[] approximateOperands))
                return null;

            //Halves round to the even neighbour as in XQuery fn:round-half-to-even and the result keeps the family of the operand
            return numericFamily == SWRLNumericFamily.Float
                ? SWRLHelper.ToNumericLiteral(Math.Round(approximateOperands[0], MidpointRounding.ToEven))
                : SWRLHelper.ToNumericLiteral(Math.Round(exactOperands[0], MidpointRounding.ToEven), numericFamily);
        }
        #endregion
    }
}
