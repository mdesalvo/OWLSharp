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

namespace OWLSharp.Ontology.Rules
{
    internal static class SWRLReplaceBuiltIn
    {
        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 4)
                throw new ArgumentException("it requires exactly 4 arguments");
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

            #region RightArgument (STRING)
            RDFPatternMember rightPatternMemberSTR = null;
            if (builtInArguments[1] is SWRLVariableArgument rightArgVarSTR)
            {
                #region Guards
                string rightArgVarName = rightArgVarSTR.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberSTR = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[1] is SWRLLiteralArgument rightArgLitSTR)
                rightPatternMemberSTR = rightArgLitSTR.GetLiteral();
            else if (builtInArguments[1] is SWRLIndividualArgument rightArgIdvSTR)
                rightPatternMemberSTR = rightArgIdvSTR.GetResource();
            #endregion

            #region RightArgument (REGEX)
            RDFPatternMember rightPatternMemberRGX = null;
            if (builtInArguments[2] is SWRLVariableArgument rightArgVarRGX)
            {
                #region Guards
                string rightArgVarName = rightArgVarRGX.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberRGX = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[2] is SWRLLiteralArgument rightArgLitRGX)
                rightPatternMemberRGX = rightArgLitRGX.GetLiteral();
            else if (builtInArguments[2] is SWRLIndividualArgument rightArgIdvRGX)
                rightPatternMemberRGX = rightArgIdvRGX.GetResource();
            #endregion

            #region RightArgument (REPLACE)
            RDFPatternMember rightPatternMemberRPL = null;
            if (builtInArguments[3] is SWRLVariableArgument rightArgVarRPL)
            {
                #region Guards
                string rightArgVarName = rightArgVarRPL.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberRPL = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[3] is SWRLLiteralArgument rightArgLitRPL)
                rightPatternMemberRPL = rightArgLitRPL.GetLiteral();
            else if (builtInArguments[3] is SWRLIndividualArgument rightArgIdvRPL)
                rightPatternMemberRPL = rightArgIdvRPL.GetResource();
            #endregion

            //This is a string builtIn, so ensure to have information compatible with "string" semantic
            bool isStringLeftPM = leftPatternMember is RDFResource 
                                   || leftPatternMember is RDFPlainLiteral 
                                   || (leftPatternMember is RDFTypedLiteral leftPMTLit && leftPMTLit.HasStringDatatype());
            bool isStringRightPMSTR = rightPatternMemberSTR is RDFResource 
                                        || rightPatternMemberSTR is RDFPlainLiteral 
                                        || (rightPatternMemberSTR is RDFTypedLiteral rightPMTLitSTR && rightPMTLitSTR.HasStringDatatype());
            bool isStringRightPMRGX = rightPatternMemberRGX is RDFResource 
                                        || rightPatternMemberRGX is RDFPlainLiteral 
                                        || (rightPatternMemberRGX is RDFTypedLiteral rightPMTLitRGX && rightPMTLitRGX.HasStringDatatype());
            bool isStringRightPMRPL = rightPatternMemberRPL is RDFResource 
                                        || rightPatternMemberRPL is RDFPlainLiteral 
                                        || (rightPatternMemberRPL is RDFTypedLiteral rightPMTLitRPL && rightPMTLitRPL.HasStringDatatype());
            if (isStringLeftPM && isStringRightPMSTR && isStringRightPMRGX && isStringRightPMRPL) 
            {
                string leftPMValue = leftPatternMember is RDFLiteral leftPMLit ? leftPMLit.Value : leftPatternMember.ToString();
                string rightPMSTRValue = rightPatternMemberSTR is RDFLiteral rightPMLitSTR ? rightPMLitSTR.Value : rightPatternMemberSTR.ToString();
                string rightPMRGXValue = rightPatternMemberRGX is RDFLiteral rightPMLitRGX ? rightPMLitRGX.Value : rightPatternMemberRGX.ToString();
                string rightPMRPLValue = rightPatternMemberRPL is RDFLiteral rightPMLitRPL ? rightPMLitRPL.Value : rightPatternMemberRPL.ToString();
                return string.Equals(leftPMValue, Regex.Replace(rightPMSTRValue, rightPMRGXValue, rightPMRPLValue));
            }
            return false;
        }
        #endregion
    }
}