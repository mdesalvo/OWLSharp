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
using System.Xml;

namespace OWLSharp.Ontology
{
    internal static class SWRLDayTimeDurationBuiltIn
    {
        internal static readonly RDFDatatype XSD_DURATION = RDFDatatypeRegister.GetDatatype(RDFModelEnums.RDFDatatypes.XSD_DURATION);

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

            #region RightArgument (DAY)
            RDFPatternMember rightPatternMemberDAY = null;
            switch (builtInArguments[1])
            {
                case SWRLVariableArgument rightArgVarDAY:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarDAY.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberDAY = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitDAY:
                    rightPatternMemberDAY = rightArgLitDAY.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument (HOUR)
            RDFPatternMember rightPatternMemberHOUR = null;
            switch (builtInArguments[2])
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
            switch (builtInArguments[3])
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
            switch (builtInArguments[4])
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

            //This is a datetime builtIn, so ensure to have information compatible with "date/datetime" semantic
            bool isDurationLeftPM = leftPatternMember is RDFTypedLiteral leftPMTLit
                                     && leftPMTLit.HasTimespanDatatype();
            bool isNumericRightPMDAY = rightPatternMemberDAY is RDFTypedLiteral rightPMTLitDAY
                                         && rightPMTLitDAY.HasDecimalDatatype();
            bool isNumericRightPMHOUR = rightPatternMemberHOUR is RDFTypedLiteral rightPMTLitHOUR
                                          && rightPMTLitHOUR.HasDecimalDatatype();
            bool isNumericRightPMMINUTE = rightPatternMemberHOUR is RDFTypedLiteral rightPMTLitMINUTE
                                          && rightPMTLitMINUTE.HasDecimalDatatype();
            bool isNumericRightPMSECOND = rightPatternMemberHOUR is RDFTypedLiteral rightPMTLitSECOND
                                          && rightPMTLitSECOND.HasDecimalDatatype();
            if (isDurationLeftPM && isNumericRightPMDAY && isNumericRightPMHOUR && isNumericRightPMMINUTE && isNumericRightPMSECOND)
                if (XSD_DURATION.Validate(((RDFLiteral)leftPatternMember).Value).Item1)
                {
                    //Get left duration
                    TimeSpan leftDuration = XmlConvert.ToTimeSpan(((RDFLiteral)leftPatternMember).Value);

                    //Get right duration
                    int rightDurationDay = Convert.ToInt32(((RDFLiteral)rightPatternMemberDAY).Value);
                    int rightDurationHour = Convert.ToInt32(((RDFLiteral)rightPatternMemberHOUR).Value);
                    int rightDurationMinute = Convert.ToInt32(((RDFLiteral)rightPatternMemberMINUTE).Value);
                    int rightDurationSecond = Convert.ToInt32(((RDFLiteral)rightPatternMemberSECOND).Value);
                    TimeSpan rightDuration = XmlConvert.ToTimeSpan($"P{rightDurationDay}DT{rightDurationHour}H{rightDurationMinute}M{rightDurationSecond}S");

                    //Compare dates
                    return leftDuration.Equals(rightDuration);
                }
            return false;
        }
        #endregion
    }
}