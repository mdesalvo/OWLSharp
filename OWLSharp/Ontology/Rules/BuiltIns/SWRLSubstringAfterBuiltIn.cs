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

namespace OWLSharp.Ontology
{
    internal static class SWRLSubstringAfterBuiltIn
    {
        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3)
                throw new ArgumentException("it requires exactly 3 arguments");
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
                case SWRLIndividualArgument leftArgIdv:
                    leftPatternMember = leftArgIdv.GetResource();
                    break;
            }
            #endregion

            #region RightArgument (STRING)
            RDFPatternMember rightPatternMemberSTR = null;
            switch (builtInArguments[1])
            {
                case SWRLVariableArgument rightArgVarSTR:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarSTR.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberSTR = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitSTR:
                    rightPatternMemberSTR = rightArgLitSTR.GetLiteral();
                    break;
                case SWRLIndividualArgument rightArgIdvSTR:
                    rightPatternMemberSTR = rightArgIdvSTR.GetResource();
                    break;
            }
            #endregion

            #region RightArgument (SEPARATOR)
            RDFPatternMember rightPatternMemberSEP = null;
            switch (builtInArguments[2])
            {
                case SWRLVariableArgument rightArgVarSEP:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarSEP.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberSEP = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitSEP:
                    rightPatternMemberSEP = rightArgLitSEP.GetLiteral();
                    break;
                case SWRLIndividualArgument rightArgIdvSEP:
                    rightPatternMemberSEP = rightArgIdvSEP.GetResource();
                    break;
            }
            #endregion

            //This is a string builtIn, so ensure to have information compatible with "string" semantic
            bool isStringLeftPM = leftPatternMember is RDFResource 
                                   || leftPatternMember is RDFPlainLiteral 
                                   || (leftPatternMember is RDFTypedLiteral leftPMTLit && leftPMTLit.HasStringDatatype());
            bool isStringRightPMSTR = rightPatternMemberSTR is RDFResource 
                                        || rightPatternMemberSTR is RDFPlainLiteral 
                                        || (rightPatternMemberSTR is RDFTypedLiteral rightPMTLitSTR && rightPMTLitSTR.HasStringDatatype());
            bool isStringRightPMSEP = rightPatternMemberSEP is RDFResource 
                                        || rightPatternMemberSEP is RDFPlainLiteral 
                                        || (rightPatternMemberSEP is RDFTypedLiteral rightPMTLitSEP && rightPMTLitSEP.HasStringDatatype());
            if (isStringLeftPM && isStringRightPMSTR && isStringRightPMSEP) 
            {
                string leftPMValue = leftPatternMember is RDFLiteral leftPMLit ? leftPMLit.Value : leftPatternMember.ToString();
                string rightPMSTRValue = rightPatternMemberSTR is RDFLiteral rightPMLitSTR ? rightPMLitSTR.Value : rightPatternMemberSTR.ToString();
                string rightPMSEPValue = rightPatternMemberSEP is RDFLiteral rightPMLitSEP ? rightPMLitSEP.Value : rightPatternMemberSEP.ToString();

                //Short-Circuit
                if (rightPMSTRValue == null || rightPMSTRValue.Length == 0)
                    return string.Equals(leftPMValue, rightPMSTRValue);
                if (rightPMSEPValue.Length == 0)
                    return string.Equals(leftPMValue, string.Empty);
                
                //Split
                int sepIndex = rightPMSTRValue.IndexOf(rightPMSEPValue);
                return sepIndex == -1 ? string.Equals(leftPMValue, string.Empty, StringComparison.Ordinal)
                                      : string.Equals(leftPMValue, rightPMSTRValue.Substring(sepIndex+rightPMSEPValue.Length), StringComparison.Ordinal);              
            }
            return false;
        }
        #endregion
    }
}