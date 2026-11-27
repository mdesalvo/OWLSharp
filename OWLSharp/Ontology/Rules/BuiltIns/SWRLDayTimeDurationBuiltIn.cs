/*
   Copyright 2014-2026 Marco De Salvo

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

using System.Collections.Generic;
using System;
using RDFSharp.Query;
using RDFSharp.Model;
using System.Xml;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLDayTimeDurationBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#dayTimeDuration built-in
    /// </summary>
    internal static class SWRLDayTimeDurationBuiltIn
    {
        internal static readonly RDFDatatype XSD_DURATION = RDFDatatypeRegister.GetDatatype(RDFModelEnums.RDFDatatypes.XSD_DURATION);

        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 5)
                throw new ArgumentException("it requires exactly 5 arguments");
            #endregion

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember leftPatternMember = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[0]);
            if (leftPatternMember == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberDAY = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]);
            if (rightPatternMemberDAY == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberHOUR = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[2]);
            if (rightPatternMemberHOUR == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberMINUTE = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[3]);
            if (rightPatternMemberMINUTE == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberSECOND = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[4]);
            if (rightPatternMemberSECOND == null)
                return false;

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
            if (isDurationLeftPM
                 && isNumericRightPMDAY
                 && isNumericRightPMHOUR
                 && isNumericRightPMMINUTE
                 && isNumericRightPMSECOND
                 && XSD_DURATION.Validate(((RDFLiteral)leftPatternMember).Value).Item1)
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