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
using System.Linq;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLAddBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#add built-in
    /// </summary>
    internal static class SWRLAddBuiltIn
    {
        #region Methods
        /// <summary>
        /// Computes the sum of the arguments after the first
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static RDFLiteral EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count < 2)
                throw new ArgumentException("it requires at least 2 arguments");
            #endregion

            //Only numbers take part in the computation: an unbound or non-numeric operand leaves the result undefined
            if (!SWRLHelper.TryGetNumericOperands(antecedentResultsRow, builtInArguments, out SWRLNumericFamily numericFamily, out decimal[] exactOperands, out double[] approximateOperands))
                return null;

            //A float operand gives a double sum, otherwise the sum is exact (beyond the decimal range it overflows and the row is dropped)
            return numericFamily == SWRLNumericFamily.Float
                ? SWRLHelper.ToNumericLiteral(approximateOperands.Sum())
                : SWRLHelper.ToNumericLiteral(exactOperands.Sum(), numericFamily);
        }
        #endregion
    }
}
