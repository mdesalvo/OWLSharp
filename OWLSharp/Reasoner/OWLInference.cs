/*
   Copyright 2014-2025 Marco De Salvo
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

using OWLSharp.Ontology;
using System;

namespace OWLSharp.Reasoner
{
    /// <summary>
    /// OWLInference represents any kind of knowledge emitted by a reasoner when applied over an ontology
    /// </summary>
    public sealed class OWLInference : IEquatable<OWLInference>
    {
        #region Properties
        /// <summary>
        /// The name of the reasoner rule which emitted the inference
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        /// The axiom representing the inferred knowledge
        /// </summary>
        public OWLAxiom Axiom { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an inference with the given characteristics
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLInference(string ruleName, OWLAxiom axiom)
        {
            RuleName = ruleName?.Trim() ?? throw new OWLException("Cannot create inference because given \"ruleName\" parameter is null");
            Axiom = axiom ?? throw new OWLException("Cannot create inference because given \"axiom\" parameter is null");
        }
        #endregion

        #region Interfaces
        public bool Equals(OWLInference other)
            => string.Equals(ToString(), other?.ToString());

        public override bool Equals(object other)
            => other is OWLInference otherInference && Equals(otherInference);

        public override string ToString()
            => Axiom.GetXML();

        public override int GetHashCode()
            => ToString().GetHashCode();
        #endregion
    }
}