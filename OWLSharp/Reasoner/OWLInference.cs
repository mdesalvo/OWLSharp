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

using OWLSharp.Ontology.Axioms;
using System;

namespace OWLSharp.Reasoner
{
    public class OWLInference : IEquatable<OWLInference>
    {
        #region Properties
        public string RuleName { get; internal set; }
        public OWLAxiom Axiom { get; internal set; }
        #endregion

        #region Ctors
        public OWLInference(string ruleName, OWLAxiom axiom)
        {
            RuleName = ruleName?.Trim() ?? throw new OWLException("Cannot create inference because given \"ruleName\" parameter is null");
            Axiom = axiom ?? throw new OWLException("Cannot create inference because given \"axiom\" parameter is null");
        }
        #endregion

        #region Interfaces
        public bool Equals(OWLInference other)
            => string.Equals(ToString(), other?.ToString());

        public override string ToString()
            => Axiom.GetXML();

        public override int GetHashCode()
            => ToString().GetHashCode();
        #endregion
    }
}