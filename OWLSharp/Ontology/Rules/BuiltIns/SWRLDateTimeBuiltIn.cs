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

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLDateTimeBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#dateTime built-in
    /// </summary>
    internal static class SWRLDateTimeBuiltIn
    {
        internal static readonly RDFDatatype XSD_DATETIME = RDFDatatypeRegister.GetDatatype(RDFModelEnums.RDFDatatypes.XSD_DATETIME);

        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 8)
                throw new ArgumentException("it requires exactly 8 arguments");
            #endregion

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember leftPatternMember = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[0]);
            if (leftPatternMember == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberYEAR = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]);
            if (rightPatternMemberYEAR == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberMONTH = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[2]);
            if (rightPatternMemberMONTH == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberDAY = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[3]);
            if (rightPatternMemberDAY == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberHOUR = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[4]);
            if (rightPatternMemberHOUR == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberMINUTE = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[5]);
            if (rightPatternMemberMINUTE == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberSECOND = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[6]);
            if (rightPatternMemberSECOND == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMemberTZ = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[7]);
            if (rightPatternMemberTZ == null)
                return false;

            //This is a datetime builtIn, so ensure to have information compatible with "date/datetime" semantic
            bool isDateTimeLeftPM = leftPatternMember is RDFTypedLiteral leftPMTLit
                                     && leftPMTLit.HasDatetimeDatatype();
            bool isNumericRightPMYEAR = rightPatternMemberYEAR is RDFTypedLiteral rightPMTLitYEAR
                                         && rightPMTLitYEAR.HasDecimalDatatype();
            bool isNumericRightPMMONTH = rightPatternMemberMONTH is RDFTypedLiteral rightPMTLitMONTH
                                          && rightPMTLitMONTH.HasDecimalDatatype();
            bool isNumericRightPMDAY = rightPatternMemberDAY is RDFTypedLiteral rightPMTLitDAY
                                        && rightPMTLitDAY.HasDecimalDatatype();
            bool isNumericRightPMHOUR = rightPatternMemberHOUR is RDFTypedLiteral rightPMTLitHOUR
                                         && rightPMTLitHOUR.HasDecimalDatatype();
            bool isNumericRightPMMINUTE = rightPatternMemberMINUTE is RDFTypedLiteral rightPMTLitMINUTE
                                          && rightPMTLitMINUTE.HasDecimalDatatype();
            bool isNumericRightPMSECOND = rightPatternMemberSECOND is RDFTypedLiteral rightPMTLitSECOND
                                        && rightPMTLitSECOND.HasDecimalDatatype();
            bool isStringRightPMTZ = rightPatternMemberTZ is RDFPlainLiteral
                                      || (rightPatternMemberTZ is RDFTypedLiteral rightPMTLitTZ && rightPMTLitTZ.HasStringDatatype());
            if (isDateTimeLeftPM
                 && isNumericRightPMYEAR
                 && isNumericRightPMMONTH
                 && isNumericRightPMDAY
                 && isNumericRightPMHOUR
                 && isNumericRightPMMINUTE
                 && isNumericRightPMSECOND
                 && isStringRightPMTZ
                 && XSD_DATETIME.Validate(((RDFLiteral)leftPatternMember).Value).Item1)
            {
                string targetTZ = ((RDFLiteral)rightPatternMemberTZ).Value;
                TimeZoneInfo rightDateTimeTZ = TimeZoneInfo.FindSystemTimeZoneById(
                                                 string.IsNullOrWhiteSpace(targetTZ) ? "UTC" : targetTZ); //Fallback to "UTC" in absence of input

                //Get left datetime and convert to destination timezone
                DateTime leftDateTime = DateTime.Parse(((RDFLiteral)leftPatternMember).Value);
                DateTime convertedLeftDateTime = TimeZoneInfo.ConvertTime(leftDateTime, rightDateTimeTZ);

                //Get right datetime and convert to destination timezone
                DateTime rightDateTime = new DateTime(
                    Convert.ToInt32(((RDFLiteral)rightPatternMemberYEAR).Value),
                    Convert.ToInt32(((RDFLiteral)rightPatternMemberMONTH).Value),
                    Convert.ToInt32(((RDFLiteral)rightPatternMemberDAY).Value),
                    Convert.ToInt32(((RDFLiteral)rightPatternMemberHOUR).Value),
                    Convert.ToInt32(((RDFLiteral)rightPatternMemberMINUTE).Value),
                    Convert.ToInt32(((RDFLiteral)rightPatternMemberSECOND).Value),
                    DateTimeKind.Utc);
                DateTime convertedRightDateTime = TimeZoneInfo.ConvertTime(rightDateTime, rightDateTimeTZ);

                //Compare datetimes
                return convertedLeftDateTime.Equals(convertedRightDateTime);
            }
            return false;
        }
        #endregion
    }
}