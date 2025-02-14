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
    internal static class SWRLTimeBuiltIn
    {
        internal static readonly RDFDatatype XSD_TIME = RDFDatatypeRegister.GetDatatype(RDFModelEnums.RDFDatatypes.XSD_TIME);

        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 5)
                throw new ArgumentException("it requires exactly 5 arguments");
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

            #region RightArgument (HOUR)
            RDFPatternMember rightPatternMemberHOUR = null;
            switch (builtInArguments[1])
            {
                case SWRLVariableArgument rightArgVarHOUR:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarHOUR.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberHOUR = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitHOUR:
                    rightPatternMemberHOUR = rightArgLitHOUR.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument (MINUTE)
            RDFPatternMember rightPatternMemberMINUTE = null;
            switch (builtInArguments[2])
            {
                case SWRLVariableArgument rightArgVarMINUTE:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarMINUTE.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberMINUTE = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitMINUTE:
                    rightPatternMemberMINUTE = rightArgLitMINUTE.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument (SECOND)
            RDFPatternMember rightPatternMemberSECOND = null;
            switch (builtInArguments[3])
            {
                case SWRLVariableArgument rightArgVarSECOND:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarSECOND.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberSECOND = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitSECOND:
                    rightPatternMemberSECOND = rightArgLitSECOND.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument (TZ)
            RDFPatternMember rightPatternMemberTZ = null;
            switch (builtInArguments[4])
            {
                case SWRLVariableArgument rightArgVarTZ:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarTZ.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberTZ = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitTZ:
                    rightPatternMemberTZ = rightArgLitTZ.GetLiteral();
                    break;
            }
            #endregion

            //This is a datetime builtIn, so ensure to have information compatible with "date/datetime" semantic
            bool isDateTimeLeftPM = leftPatternMember is RDFTypedLiteral leftPMTLit 
                                     && leftPMTLit.HasDatetimeDatatype();
            bool isNumericRightPMHOUR = rightPatternMemberHOUR is RDFTypedLiteral rightPMTLitHOUR 
                                         && rightPMTLitHOUR.HasDecimalDatatype();
            bool isNumericRightPMMINUTE = rightPatternMemberMINUTE is RDFTypedLiteral rightPMTLitMINUTE 
                                          && rightPMTLitMINUTE.HasDecimalDatatype();
            bool isNumericRightPMSECOND = rightPatternMemberSECOND is RDFTypedLiteral rightPMTLitSECOND 
                                        && rightPMTLitSECOND.HasDecimalDatatype();
            bool isStringRightPMTZ = rightPatternMemberTZ is RDFPlainLiteral
                                      || (rightPatternMemberTZ is RDFTypedLiteral rightPMTLitTZ && rightPMTLitTZ.HasStringDatatype());
            if (isDateTimeLeftPM && isNumericRightPMHOUR && isNumericRightPMMINUTE && isNumericRightPMSECOND && isStringRightPMTZ) 
                if (XSD_TIME.Validate(((RDFLiteral)leftPatternMember).Value).Item1)
                {
                    string targetTZ = ((RDFLiteral)rightPatternMemberTZ).Value;
                    TimeZoneInfo rightTimeTZ = TimeZoneInfo.FindSystemTimeZoneById(
                                                    string.IsNullOrWhiteSpace(targetTZ) ? "UTC" : targetTZ); //Fallback to "UTC" in absence of input

                    //Get left time and convert to destination timezone
                    DateTime leftTime = DateTime.Parse(((RDFLiteral)leftPatternMember).Value);
                    DateTime convertedLeftTime = TimeZoneInfo.ConvertTime(leftTime, rightTimeTZ);

                    //Get right time and convert to destination timezone
                    DateTime rightTime = new DateTime(
                        leftTime.Year,
                        leftTime.Month,
                        leftTime.Day,
                        Convert.ToInt32(((RDFLiteral)rightPatternMemberHOUR).Value),
                        Convert.ToInt32(((RDFLiteral)rightPatternMemberMINUTE).Value),
                        Convert.ToInt32(((RDFLiteral)rightPatternMemberSECOND).Value),
                        DateTimeKind.Utc);
                    DateTime convertedRightTime = TimeZoneInfo.ConvertTime(rightTime, rightTimeTZ);

                    //Compare times
                    return convertedLeftTime.Equals(convertedRightTime);
                }
            return false;
        }
        #endregion
    }
}