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

using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLLessThanBuiltIn implements the standard http://www.w3.org/2003/11/swrlb#lessThan built-in
    /// </summary>
    internal static class SWRLLessThanBuiltIn
    {
        #region Methods
        /// <summary>
        /// Evaluates the built-in in the context of being part of a SWRL antecedent
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        internal static bool EvaluateOnAntecedent(RDFTableRow antecedentResultsRow, List<SWRLArgument> builtInArguments)
        {
            #region Guards
            if (builtInArguments?.Count != 2)
                throw new ArgumentException("it requires exactly 2 arguments");
            #endregion

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember leftPatternMember = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[0]);
            if (leftPatternMember == null)
                return false;

            //An argument that no atom has bound never satisfies the built-in
            RDFPatternMember rightPatternMember = SWRLHelper.GetArgumentValue(antecedentResultsRow, builtInArguments[1]);
            if (rightPatternMember == null)
                return false;

            #region ComparisonExpression
            RDFComparisonExpression comparisonExpression;
            switch (leftPatternMember)
            {
                case RDFResource leftPatternMemberRes:
                    {
                        switch (rightPatternMember)
                        {
                            case RDFResource rightPatternMemberRes:
                                comparisonExpression = new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.LessThan,
                                                        new RDFConstantExpression(leftPatternMemberRes),
                                                        new RDFConstantExpression(rightPatternMemberRes));
                                break;
                            default:
                                comparisonExpression = new RDFComparisonExpression(
                                        RDFQueryEnums.RDFComparisonFlavors.LessThan,
                                        new RDFConstantExpression(leftPatternMemberRes),
                                        new RDFConstantExpression((RDFLiteral)rightPatternMember));
                                break;
                        }
                        break;
                    }
                default:
                    {
                        switch (rightPatternMember)
                        {
                            case RDFResource rightPatternMemberRes:
                                comparisonExpression = new RDFComparisonExpression(
                                                        RDFQueryEnums.RDFComparisonFlavors.LessThan,
                                                        new RDFConstantExpression((RDFLiteral)leftPatternMember),
                                                        new RDFConstantExpression(rightPatternMemberRes));

                                break;
                            default:
                                comparisonExpression = new RDFComparisonExpression(
                                        RDFQueryEnums.RDFComparisonFlavors.LessThan,
                                        new RDFConstantExpression((RDFLiteral)leftPatternMember),
                                        new RDFConstantExpression((RDFLiteral)rightPatternMember));
                                break;
                        }
                        break;
                    }
            }
            #endregion

            return new RDFFilter(comparisonExpression).ApplyFilter(antecedentResultsRow, false);
        }
        #endregion
    }
}