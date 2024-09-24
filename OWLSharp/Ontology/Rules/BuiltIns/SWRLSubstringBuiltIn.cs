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
using System.Text.RegularExpressions;
using System.Globalization;

namespace OWLSharp.Ontology.Rules
{
    internal static class SWRLSubstringBuiltIn
    {
        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3 && builtInArguments?.Count != 4)
                throw new ArgumentException("it requires exactly 3 or exactly 4 arguments");
            #endregion

            #region LeftArgument
            RDFPatternMember leftPatternMember = null;
            if (builtInArguments[0] is SWRLVariableArgument leftArgVar)
            {
                #region Guards
                string leftArgVarName = leftArgVar.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(leftArgVarName))
                    return true;
                #endregion

                leftPatternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[leftArgVarName].ToString());
            }
            else if (builtInArguments[0] is SWRLLiteralArgument leftArgLit)
                leftPatternMember = leftArgLit.GetLiteral();
            else if (builtInArguments[0] is SWRLIndividualArgument leftArgIdv)
                leftPatternMember = leftArgIdv.GetResource();
            #endregion

            #region RightArgument (SOURCE)
            RDFPatternMember rightPatternMemberSRC = null;
            if (builtInArguments[1] is SWRLVariableArgument rightArgVarSRC)
            {
                #region Guards
                string rightArgVarName = rightArgVarSRC.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberSRC = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[1] is SWRLLiteralArgument rightArgLitSRC)
                rightPatternMemberSRC = rightArgLitSRC.GetLiteral();
            else if (builtInArguments[1] is SWRLIndividualArgument rightArgIdvSRC)
                rightPatternMemberSRC = rightArgIdvSRC.GetResource();
            #endregion

            #region RightArgument (INDEX)
            RDFPatternMember rightPatternMemberIDX = null;
            if (builtInArguments[2] is SWRLVariableArgument rightArgVarIDX)
            {
                #region Guards
                string rightArgVarName = rightArgVarIDX.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberIDX = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[2] is SWRLLiteralArgument rightArgLitIDX)
                rightPatternMemberIDX = rightArgLitIDX.GetLiteral();
            #endregion

            #region RightArgument (LENGTH)
            RDFPatternMember rightPatternMemberLEN = null;
            if (builtInArguments.Count == 4)
                if (builtInArguments[3] is SWRLVariableArgument rightArgVarLEN)
                {
                    #region Guards
                    string rightArgVarName = rightArgVarLEN.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberLEN = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                }
                else if (builtInArguments[3] is SWRLLiteralArgument rightArgLitLEN)
                    rightPatternMemberLEN = rightArgLitLEN.GetLiteral();
            #endregion

            //This is a string builtIn, so ensure to have information compatible with "string" semantic
            bool isStringLeftPM = leftPatternMember is RDFResource 
                                   || leftPatternMember is RDFPlainLiteral 
                                   || (leftPatternMember is RDFTypedLiteral leftPMTLit && leftPMTLit.HasStringDatatype());
            bool isStringRightPMSRC = rightPatternMemberSRC is RDFResource 
                                        || rightPatternMemberSRC is RDFPlainLiteral 
                                        || (rightPatternMemberSRC is RDFTypedLiteral rightPMTLit && rightPMTLit.HasStringDatatype());
            if (isStringLeftPM && isStringRightPMSRC) 
            {
                string leftPMValue = leftPatternMember is RDFLiteral leftPMLit ? leftPMLit.Value : leftPatternMember.ToString();
                string rightPMValue = rightPatternMemberSRC is RDFLiteral rightPMLit ? rightPMLit.Value : rightPatternMemberSRC.ToString();

                //This builtIn also requires numeric literals in 3rd (and optional 4th) arguments to execute a substring operation on the 2nd
                if (rightPatternMemberIDX is RDFTypedLiteral leftTypedLiteralIDX
                     && leftTypedLiteralIDX.HasDecimalDatatype()
                     && double.TryParse(leftTypedLiteralIDX.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftNumberIDX))
                {
                    //startIndex
                    if (builtInArguments.Count == 3)
                        return string.Equals(leftPMValue, rightPMValue.Substring(Convert.ToInt32(leftNumberIDX)));

                    //startIndex+length
                    else if (rightPatternMemberLEN is RDFTypedLiteral leftTypedLiteralLEN
                              && leftTypedLiteralLEN.HasDecimalDatatype()
                              && double.TryParse(leftTypedLiteralLEN.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftNumberLEN))
                        return string.Equals(leftPMValue, rightPMValue.Substring(Convert.ToInt32(leftNumberIDX), Convert.ToInt32(leftNumberLEN)));
                }                    
            }
            return false;
        }
        #endregion
    }
}