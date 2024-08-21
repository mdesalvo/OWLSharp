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

namespace OWLSharp.Extensions.SWRL.BuiltIns
{
    public class SWRLTanBuiltIn : SWRLMathBuiltIn
    {
        #region Properties
        internal static readonly RDFResource BuiltInUri = new RDFResource("http://www.w3.org/2003/11/swrlb#tan");
        #endregion

        #region Ctors
        public SWRLTanBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument)
            : base(new OWLExpression() { ExpressionIRI = BuiltInUri }, leftArgument, rightArgument, double.NaN)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            #endregion
        }
        #endregion
    }
}