/*
   Copyright 2014-2025 Marco De Salvo

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

using System.Data;
using System.Collections.Generic;
using System;
using RDFSharp.Query;
using RDFSharp.Model;
using System.Globalization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLMultiplyBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#multiply built-in
    /// </summary>
    internal static class SWRLMultiplyBuiltIn
    {
        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count < 2)
                throw new ArgumentException("it requires at least 2 arguments");
            #endregion

            #region Arguments
            double? leftValue = null, rightValue = null;
            for (int i=0; i<builtInArguments.Count; i++)
            {
                //Parse current argument
                RDFPatternMember patternMember = null;
                switch (builtInArguments[i])
                {
                    case SWRLVariableArgument argVar when antecedentResultsRow.Table.Columns.Contains(argVar.GetVariable().ToString()):
                        patternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[argVar.GetVariable().ToString()].ToString());
                        break;
                    case SWRLLiteralArgument argLit:
                        patternMember = argLit.GetLiteral();
                        break;
                }

                //Collect current argument
                if (patternMember is RDFTypedLiteral typedLiteral
                     && typedLiteral.HasDecimalDatatype()
                     && double.TryParse(typedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double argValue))
                {
                    //First argument concurs to the left value
                    if (i == 0)
                        leftValue = argValue;
                    //Other arguments concur to the right value
                    else if (rightValue == null)
                        rightValue = argValue;
                    else
                        rightValue *= argValue;
                }
                //In case of type error just discard the current row
                else
                {
                    return false;
                }
            }

            return leftValue.HasValue && rightValue.HasValue && leftValue.Value == rightValue.Value;
            #endregion
        }
        #endregion
    }
}