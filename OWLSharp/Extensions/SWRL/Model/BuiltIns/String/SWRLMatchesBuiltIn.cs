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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Text;
using System.Text.RegularExpressions;

namespace OWLSharp.Extensions.SWRL.Model.BuiltIns
{
    public class SWRLMatchesBuiltIn : SWRLFilterBuiltIn
    {
        #region Properties
        internal static readonly RDFResource BuiltInUri = new RDFResource("swrlb:matches");
        #endregion

        #region Ctors
        public SWRLMatchesBuiltIn(RDFVariable leftArgument, Regex matchesRegex)
            : base(new OWLExpression() { ExpressionIRI = BuiltInUri }, leftArgument, null)
        {
            #region Guards
            if (matchesRegex == null)
                throw new OWLException("Cannot create built-in because given \"matchesRegex\" parameter is null");
            #endregion

            StringBuilder regexFlags = new StringBuilder();
            if (matchesRegex.Options.HasFlag(RegexOptions.IgnoreCase))
                regexFlags.Append('i');
            if (matchesRegex.Options.HasFlag(RegexOptions.Singleline))
                regexFlags.Append('s');
            if (matchesRegex.Options.HasFlag(RegexOptions.Multiline))
                regexFlags.Append('m');
            if (matchesRegex.Options.HasFlag(RegexOptions.IgnorePatternWhitespace))
                regexFlags.Append('x');
            RightArgument = string.IsNullOrEmpty(regexFlags.ToString()) ? new RDFPlainLiteral($"{matchesRegex}") 
                                                                        : new RDFPlainLiteral($"{matchesRegex}\",\"{regexFlags}");
            BuiltInFilter = new RDFRegexFilter(leftArgument, matchesRegex);
        }
        #endregion
    }
}