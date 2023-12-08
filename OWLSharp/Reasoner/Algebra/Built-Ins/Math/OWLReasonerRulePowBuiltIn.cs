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
    /// OWLReasonerRulePowBuiltIn represents a SWRL built-in filtering inferences of a rule's antecedent on a swrlb:pow basis
    /// </summary>
    public class OWLReasonerRulePowBuiltIn : OWLReasonerRuleMathBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the Uri of the built-in (swrlb:pow)
        /// </summary>
        private static readonly RDFResource BuiltInUri = new RDFResource("swrlb:pow");
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a swrlb:pow built-in with given arguments
        /// </summary>
        public OWLReasonerRulePowBuiltIn(RDFVariable leftArgument, RDFVariable rightArgument, double expValue)
            : base(BuiltInUri, leftArgument, rightArgument, expValue)
        {
            if (rightArgument == null)
                throw new OWLException("Cannot create built-in because given \"rightArgument\" parameter is null");
        }
        #endregion
    }
}