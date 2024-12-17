/*
   Copyright 2014-2024 Marco De Salvo

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
    internal static class SWRLPowBuiltIn
    {
        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3)
                throw new ArgumentException("it requires exactly 3 arguments");
            #endregion

            #region Arguments
            double? leftValue = null, rightValue = null;
            RDFPatternMember patternMember;
            for (int i=0; i<builtInArguments.Count; i++)
            {
                //Parse current argument
                patternMember = null;
                if (builtInArguments[i] is SWRLVariableArgument argVar && antecedentResultsRow.Table.Columns.Contains(argVar.GetVariable().ToString()))
                    patternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[argVar.GetVariable().ToString()].ToString());
                else if (builtInArguments[i] is SWRLLiteralArgument argLit)
                    patternMember = argLit.GetLiteral();

                //Collect current argument
                if (patternMember is RDFTypedLiteral typedLiteral
                     && typedLiteral.HasDecimalDatatype()
                     && double.TryParse(typedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double argValue))
                {
                    //First argument concurs to the left value
                    if (i==0)
                        leftValue = argValue;
                    //Other arguments concur to the right value
                    else if (rightValue == null)
                        rightValue = argValue;
                    else
                        rightValue = Math.Pow(rightValue.Value, argValue);
                }
                //In case of type error just discard the current row
                else
                    return false;
            };
            return leftValue.HasValue && rightValue.HasValue && leftValue.Value == rightValue.Value;
            #endregion
        }
        #endregion
    }
}