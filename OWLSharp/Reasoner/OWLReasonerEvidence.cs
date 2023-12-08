﻿/*
   Copyright 2012-2024 Marco De Salvo
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

namespace OWLSharp
{
    /// <summary>
    /// OWLReasonerEvidence represents an evidence generated by an ontology reasoner rule
    /// </summary>
    public class OWLReasonerEvidence
    {
        #region Properties
        /// <summary>
        /// Category of this evidence
        /// </summary>
        public OWLEnums.OWLReasonerEvidenceCategory EvidenceCategory { get; internal set; }

        /// <summary>
        /// Rule which has generated this evidence
        /// </summary>
        public string EvidenceRule { get; internal set; }

        /// <summary>
        /// Content of the evidence
        /// </summary>
        public RDFTriple EvidenceContent { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a reasoner evidence with given category, provenance, message and suggestion
        /// </summary>
        public OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory evidenceCategory, string evidenceRule, RDFTriple evidenceContent)
        {
            if (string.IsNullOrEmpty(evidenceRule))
                throw new OWLException("Cannot create reasoner evidence because given \"evidenceRule\" parameter is null or empty");
            if (evidenceContent == null)
                throw new OWLException("Cannot create reasoner evidence because given \"evidenceContent\" parameter is null");

            EvidenceCategory = evidenceCategory;
            EvidenceRule = evidenceRule;
            EvidenceContent = evidenceContent;
        }
        #endregion
    }
}