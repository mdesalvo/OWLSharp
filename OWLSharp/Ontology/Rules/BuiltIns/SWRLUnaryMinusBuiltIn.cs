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
    internal static class SWRLUnaryMinusBuiltIn
    {
        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            #region LeftArgument
            RDFPatternMember leftPatternMember = null;
            switch (builtInArguments[0])
            {
                case SWRLVariableArgument leftArgVar:
                {
                    #region Guards
                    string leftArgVarName = leftArgVar.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(leftArgVarName))
                        return true;
                    #endregion

                    leftPatternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[leftArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument leftArgLit:
                    leftPatternMember = leftArgLit.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument
            RDFPatternMember rightPatternMember = null;
            switch (builtInArguments[1])
            {
                case SWRLVariableArgument rightArgVar:
                {
                    #region Guards
                    string rightArgVarName = rightArgVar.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLit:
                    rightPatternMember = rightArgLit.GetLiteral();
                    break;
            }
            #endregion

            if (leftPatternMember is RDFTypedLiteral leftTypedLiteral
                 && leftTypedLiteral.HasDecimalDatatype()
                 && double.TryParse(leftTypedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftNumber)
                 && rightPatternMember is RDFTypedLiteral rightTypedLiteral
                 && rightTypedLiteral.HasDecimalDatatype()
                 && double.TryParse(rightTypedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double rightNumber))
            {
                return leftNumber == -rightNumber;
            }

            return false;
        }
        #endregion
    }
}