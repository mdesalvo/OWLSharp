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
using System.Globalization;

namespace OWLSharp.Ontology.Rules
{
    internal static class SWRLStringConcatBuiltIn
    {
        #region Methods
        internal static bool EvaluateOnAntecedent(DataRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count < 2)
                throw new ArgumentException("it requires at least 2 arguments");
            #endregion

            #region Arguments
            string leftValue = null, rightValue = null;
            RDFPatternMember patternMember;
            for (int i=0; i<builtInArguments.Count; i++)
            {
                //Parse current argument
                patternMember = null;
                if (builtInArguments[i] is SWRLVariableArgument argVar && antecedentResultsRow.Table.Columns.Contains(argVar.GetVariable().ToString()))
                    patternMember = RDFQueryUtilities.ParseRDFPatternMember(antecedentResultsRow[argVar.GetVariable().ToString()].ToString());
                else if (builtInArguments[i] is SWRLLiteralArgument argLit)
                    patternMember = argLit.GetLiteral();
                else if (builtInArguments[i] is SWRLIndividualArgument argIdv)
                    patternMember = argIdv.GetResource();

                //This is a string builtIn, so ensure to have information compatible with "string" semantic
                if (patternMember is RDFResource 
                     || patternMember is RDFPlainLiteral 
                     || (patternMember is RDFTypedLiteral pmTLit && pmTLit.HasStringDatatype()))
                {
                    string argValue = patternMember is RDFLiteral pmLit ? pmLit.Value : patternMember.ToString();

                    //First argument concurs to the left value
                    if (i==0)
                        leftValue = argValue;
                    //Other arguments concur to the right value
                    else if (rightValue == null)
                        rightValue = argValue;
                    else
                        rightValue = string.Concat(rightValue,argValue);
                }
                //In case of type error just discard the current row
                else
                    return false;
            };
            return string.Equals(leftValue, rightValue);
            #endregion
        }
        #endregion
    }
}