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
    /// <summary>
    /// SWRLSubstringBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#substring built-in
    /// </summary>
    internal static class SWRLSubstringBuiltIn
    {
        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 3 && builtInArguments?.Count != 4)
                throw new ArgumentException("it requires exactly 3 or exactly 4 arguments");
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

            #region RightArgument (SOURCE)
            RDFPatternMember rightPatternMemberSRC = null;
            switch (builtInArguments[1])
            {
                case SWRLVariableArgument rightArgVarSRC:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarSRC.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberSRC = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitSRC:
                    rightPatternMemberSRC = rightArgLitSRC.GetLiteral();
                    break;
                case SWRLIndividualArgument rightArgIdvSRC:
                    rightPatternMemberSRC = rightArgIdvSRC.GetResource();
                    break;
            }
            #endregion

            #region RightArgument (INDEX)
            RDFPatternMember rightPatternMemberIDX = null;
            switch (builtInArguments[2])
            {
                case SWRLVariableArgument rightArgVarIDX:
                {
                    #region Guards
                    string rightArgVarName = rightArgVarIDX.GetVariable().ToString();
                    if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                        return true;
                    #endregion

                    rightPatternMemberIDX = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                    break;
                }
                case SWRLLiteralArgument rightArgLitIDX:
                    rightPatternMemberIDX = rightArgLitIDX.GetLiteral();
                    break;
            }
            #endregion

            #region RightArgument (LENGTH)
            RDFPatternMember rightPatternMemberLEN = null;
            if (builtInArguments.Count == 4)
                switch (builtInArguments[3])
                {
                    case SWRLVariableArgument rightArgVarLEN:
                    {
                        #region Guards
                        string rightArgVarName = rightArgVarLEN.GetVariable().ToString();
                        if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                            return true;
                        #endregion

                        rightPatternMemberLEN = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
                        break;
                    }
                    case SWRLLiteralArgument rightArgLitLEN:
                        rightPatternMemberLEN = rightArgLitLEN.GetLiteral();
                        break;
                }
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
                     && int.TryParse(leftTypedLiteralIDX.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int leftNumberIDX))
                {
                    //startIndex
                    if (builtInArguments.Count == 3)
                        return string.Equals(leftPMValue, rightPMValue.Substring(leftNumberIDX));

                    //startIndex+length
                    if (rightPatternMemberLEN is RDFTypedLiteral leftTypedLiteralLEN
                        && leftTypedLiteralLEN.HasDecimalDatatype()
                        && int.TryParse(leftTypedLiteralLEN.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int leftNumberLEN))
                    {
                        return string.Equals(leftPMValue, rightPMValue.Substring(leftNumberIDX, leftNumberLEN));
                    }
                }
            }
            return false;
        }
        #endregion
    }
}