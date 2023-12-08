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

using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerRuleDivideBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:divide basis
    /// </summary>
    public class OWLReasonerRuleDivideBuiltIn : OWLReasonerRuleMathBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:divide)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:divide");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:divide built-in with given arguments
        /// </summary>
        public OWLReasonerRuleDivideBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument, double divideValue)
            : base(BuiltInUri, leftArgument, rightArgument, divideValue)
        {
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
            if (divideValue == 0d)
                throw new OWLException("Cannot create built-in because given \"divideValue\" is zero!");
        }
        #endregion
    }
}