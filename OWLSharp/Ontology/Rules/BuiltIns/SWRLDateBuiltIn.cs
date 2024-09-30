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

namespace OWLSharp.Ontology.Rules
{
    internal static class SWRLDateBuiltIn
    {
        internal static RDFDatatype XSD_DATE = RDFDatatypeRegister.GetDatatype(RDFModelEnums.RDFDatatypes.XSD_DATE);

        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 5)
                throw new ArgumentException("it requires exactly 5 arguments");
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
            #endregion

            #region RightArgument (YEAR)
            RDFPatternMember rightPatternMemberYEAR = null;
            if (builtInArguments[1] is SWRLVariableArgument rightArgVarYEAR)
            {
                #region Guards
                string rightArgVarName = rightArgVarYEAR.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberYEAR = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[1] is SWRLLiteralArgument rightArgLitYEAR)
                rightPatternMemberYEAR = rightArgLitYEAR.GetLiteral();
            #endregion

            #region RightArgument (MONTH)
            RDFPatternMember rightPatternMemberMONTH = null;
            if (builtInArguments[2] is SWRLVariableArgument rightArgVarMONTH)
            {
                #region Guards
                string rightArgVarName = rightArgVarMONTH.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberMONTH = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[2] is SWRLLiteralArgument rightArgLitMONTH)
                rightPatternMemberMONTH = rightArgLitMONTH.GetLiteral();
            #endregion

            #region RightArgument (DAY)
            RDFPatternMember rightPatternMemberDAY = null;
            if (builtInArguments[3] is SWRLVariableArgument rightArgVarDAY)
            {
                #region Guards
                string rightArgVarName = rightArgVarDAY.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberDAY = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[3] is SWRLLiteralArgument rightArgLitDAY)
                rightPatternMemberDAY = rightArgLitDAY.GetLiteral();
            #endregion

            #region RightArgument (TZ)
            RDFPatternMember rightPatternMemberTZ = null;
            if (builtInArguments[4] is SWRLVariableArgument rightArgVarTZ)
            {
                #region Guards
                string rightArgVarName = rightArgVarTZ.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMemberTZ = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[4] is SWRLLiteralArgument rightArgLitTZ)
                rightPatternMemberTZ = rightArgLitTZ.GetLiteral();
            #endregion

            //This is a datetime builtIn, so ensure to have information compatible with "date/datetime" semantic
            bool isDateTimeLeftPM = leftPatternMember is RDFTypedLiteral leftPMTLit 
                                     && leftPMTLit.HasDatetimeDatatype();
            bool isNumericRightPMYEAR = rightPatternMemberYEAR is RDFTypedLiteral rightPMTLitYEAR 
                                         && rightPMTLitYEAR.HasDecimalDatatype();
            bool isNumericRightPMMONTH = rightPatternMemberMONTH is RDFTypedLiteral rightPMTLitMONTH 
                                          && rightPMTLitMONTH.HasDecimalDatatype();
            bool isNumericRightPMDAY = rightPatternMemberDAY is RDFTypedLiteral rightPMTLitDAY 
                                        && rightPMTLitDAY.HasDecimalDatatype();
            bool isStringRightPMTZ = rightPatternMemberTZ is RDFPlainLiteral
                                      || (rightPatternMemberTZ is RDFTypedLiteral rightPMTLitTZ && rightPMTLitTZ.HasStringDatatype());
            if (isDateTimeLeftPM && isNumericRightPMYEAR && isNumericRightPMMONTH && isNumericRightPMDAY && isStringRightPMTZ) 
                if (XSD_DATE.Validate(((RDFLiteral)leftPatternMember).Value).Item1)
                {
                    string targetTZ = ((RDFLiteral)rightPatternMemberTZ).Value;
                    TimeZoneInfo rightDateTZ = TimeZoneInfo.FindSystemTimeZoneById(
                                                    string.IsNullOrWhiteSpace(targetTZ) ? "UTC" : targetTZ); //Fallback to "UTC" in absence of input

                    //Get left date and convert to destination timezone
                    DateTime leftDate = DateTime.Parse(((RDFLiteral)leftPatternMember).Value);
                    DateTime convertedLeftDate = TimeZoneInfo.ConvertTime(leftDate, rightDateTZ);

                    //Get right date and convert to destination timezone
                    DateTime rightDate = new DateTime(
                        Convert.ToInt32(((RDFLiteral)rightPatternMemberYEAR).Value),
                        Convert.ToInt32(((RDFLiteral)rightPatternMemberMONTH).Value),
                        Convert.ToInt32(((RDFLiteral)rightPatternMemberDAY).Value),
                        0,0,0,DateTimeKind.Utc);
                    DateTime convertedRightDate = TimeZoneInfo.ConvertTime(rightDate, rightDateTZ);

                    //Compare dates
                    return convertedLeftDate.Equals(convertedRightDate);
                }
            return false;
        }
        #endregion
    }
}