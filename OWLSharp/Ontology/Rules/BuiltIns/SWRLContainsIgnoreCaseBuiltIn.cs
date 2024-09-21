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
    internal static class SWRLContainsIgnoreCaseBuiltIn
    {
        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
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

            #region RightArgument
            RDFPatternMember rightPatternMember = null;
            if (builtInArguments[1] is SWRLVariableArgument rightArgVar)
            {
                #region Guards
                string rightArgVarName = rightArgVar.GetVariable().ToString();
                if (!antecedentResultsRow.Table.Columns.Contains(rightArgVarName))
                    return true;
                #endregion

                rightPatternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[rightArgVarName].ToString());
            }
            else if (builtInArguments[1] is SWRLLiteralArgument rightArgLit)
                rightPatternMember = rightArgLit.GetLiteral();
            else if (builtInArguments[1] is SWRLIndividualArgument rightArgIdv)
                rightPatternMember = rightArgIdv.GetResource();
            #endregion

            //This is a string builtIn, so ensure to have information compatible with "string" semantic
            bool isStringLeftPM = leftPatternMember is RDFResource 
                                   || leftPatternMember is RDFPlainLiteral 
                                   || (leftPatternMember is RDFTypedLiteral leftPMTLit && leftPMTLit.HasStringDatatype());
            bool isStringRightPM = rightPatternMember is RDFResource 
                                    || rightPatternMember is RDFPlainLiteral 
                                    || (rightPatternMember is RDFTypedLiteral rightPMTLit && rightPMTLit.HasStringDatatype());
            if (isStringLeftPM && isStringRightPM) 
            {
                string leftPMValue = leftPatternMember is RDFLiteral leftPMLit ? leftPMLit.Value : leftPatternMember.ToString();
                string rightPMValue = rightPatternMember is RDFLiteral rightPMLit ? rightPMLit.Value : rightPatternMember.ToString();
                return leftPMValue.IndexOf(rightPMValue, StringComparison.OrdinalIgnoreCase) > -1;
            }
            return false;
        }
        #endregion
    }
}