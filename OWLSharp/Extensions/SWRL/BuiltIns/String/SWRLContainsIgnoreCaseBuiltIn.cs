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
using System.Text.RegularExpressions;

namespace OWLSharp.Extensions.SWRL.BuiltIns
{
    public class SWRLContainsIgnoreCaseBuiltIn : SWRLFilterBuiltIn
    {
        #region Properties
        internal static readonly RDFResource BuiltInUri = new RDFResource("http://www.w3.org/2003/11/swrlb#containsIgnoreCase");
        #endregion

        #region Ctors
        public SWRLContainsIgnoreCaseBuiltIn(RDFVariable leftArgument, string containString)
            : base(new OWLExpression() { ExpressionIRI = BuiltInUri }, leftArgument, null)
        {
            #region Guards
            if (containString == null)
                throw new OWLException("Cannot create built-in because given \"containString\" parameter is null");
            #endregion

            RightArgument = new RDFPlainLiteral(containString);
            BuiltInFilter = new RDFRegexFilter(leftArgument, new Regex($"{containString}", RegexOptions.IgnoreCase));
        }
        #endregion
    }
}