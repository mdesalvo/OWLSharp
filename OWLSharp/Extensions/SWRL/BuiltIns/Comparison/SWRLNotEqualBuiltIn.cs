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

namespace OWLSharp.Extensions.SWRL.BuiltIns
{
    public class SWRLNotEqualBuiltIn : SWRLFilterBuiltIn
    {
        #region Properties
        internal static readonly RDFResource BuiltInUri = new RDFResource("http://www.w3.org/2003/11/swrlb#notEqual");
        #endregion

        #region Ctors
        public SWRLNotEqualBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        public SWRLNotEqualBuiltIn(RDFVariable leftArgument, RDFResource rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        public SWRLNotEqualBuiltIn(RDFVariable leftArgument, RDFLiteral rightArgument)
            : this(leftArgument, rightArgument as RDFPatternMember) { }

        internal SWRLNotEqualBuiltIn(RDFVariable leftArgument, RDFPatternMember rightArgument)
            : base(new OWLExpression() { ExpressionIRI = BuiltInUri }, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion

            BuiltInFilter = new RDFComparisonFilter(RDFQueryEnums.RDFComparisonFlavors.NotEqualTo, leftArgument, rightArgument);
        }
        #endregion
    }
}