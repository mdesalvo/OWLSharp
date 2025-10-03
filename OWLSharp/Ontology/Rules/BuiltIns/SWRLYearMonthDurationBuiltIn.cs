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
    /// <summary>
    /// SWRLYearMonthDurationBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#yearMonthDuration built-in
    /// </summary>
    internal static class SWRLYearMonthDurationBuiltIn
    {
        internal static readonly RDFDatatype XSD_DURATION = RDFDatatypeRegister.GetDatatype(RDFModelEnums.RDFDatatypes.XSD_DURATION);

        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
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
            }
            #endregion

            #region RightArgument (YEAR)
            RDFPatternMember rightPatternMemberYEAR = null;
            switch (builtInArguments[1])
            {
                case SWRLVariableArgument rightArgVarYEAR:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarYEAR.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberYEAR = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitYEAR:
                    rightPatternMemberYEAR = rightArgLitYEAR.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument (MONTH)
            RDFPatternMember rightPatternMemberMONTH = null;
            switch (builtInArguments[2])
            {
                case SWRLVariableArgument rightArgVarMONTH:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarMONTH.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberMONTH = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitMONTH:
                    rightPatternMemberMONTH = rightArgLitMONTH.GetLiteral();
                    break;
            }
            #endregion

            //This is a datetime builtIn, so ensure to have information compatible with "date/datetime" semantic
            bool isDurationLeftPM = leftPatternMember is RDFTypedLiteral leftPMTLit
                                     && leftPMTLit.HasTimespanDatatype();
            bool isNumericRightPMYEAR = rightPatternMemberYEAR is RDFTypedLiteral rightPMTLitYEAR
                                         && rightPMTLitYEAR.HasDecimalDatatype();
            bool isNumericRightPMMONTH = rightPatternMemberMONTH is RDFTypedLiteral rightPMTLitMONTH
                                          && rightPMTLitMONTH.HasDecimalDatatype();
            if (isDurationLeftPM
                 && isNumericRightPMYEAR
                 && isNumericRightPMMONTH
                 && XSD_DURATION.Validate(((RDFLiteral)leftPatternMember).Value).Item1)
            {
                //Get left duration
                TimeSpan leftDuration = XmlConvert.ToTimeSpan(((RDFLiteral)leftPatternMember).Value);

                //Get right duration
                int rightDurationYear = Convert.ToInt32(((RDFLiteral)rightPatternMemberYEAR).Value);
                int rightDurationMonth = Convert.ToInt32(((RDFLiteral)rightPatternMemberMONTH).Value);
                TimeSpan rightDuration = XmlConvert.ToTimeSpan($"P{rightDurationYear}Y{rightDurationMonth}M");

                //Compare dates
                return leftDuration.Equals(rightDuration);
            }
            return false;
        }
        #endregion
    }
}