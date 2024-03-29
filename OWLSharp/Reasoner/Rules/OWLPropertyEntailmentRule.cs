﻿/*
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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL reasoner rule targeting data knowledge (A-BOX) to infer assertions from rdfs:subPropertyOf and owl:equivalentProperty hierarchies
    /// </summary>
    internal static class OWLPropertyEntailmentRule
    {
        internal static OWLReasonerReport ExecuteRule(OWLOntology ontology)
        {
            #region RuleBody
            void InferAssertionsFromPropertyHierarchy(RDFResource currentProperty, OWLReasonerReport report)
            {
                //Get assertions of the current property
                RDFGraph propertyAssertions = ontology.Data.ABoxGraph[null, currentProperty, null, null];

                //Calculate properties compatible with the current properties
                List<RDFResource> compatibleProperties = ontology.Model.PropertyModel.GetSuperPropertiesOf(currentProperty)
                                                            .Union(ontology.Model.PropertyModel.GetEquivalentPropertiesOf(currentProperty)).ToList();

                //Extend current property assertions to each of the compatible properties (exclude blanks)
                foreach (RDFResource compatibleProperty in compatibleProperties.Where(p => !p.IsBlank))
                    foreach (RDFTriple propertyAssertion in propertyAssertions)
                    {
                        //Create the inferences
                        OWLReasonerEvidence evidence = propertyAssertion.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO 
                            ? new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, nameof(OWLPropertyEntailmentRule), new RDFTriple((RDFResource)propertyAssertion.Subject, compatibleProperty, (RDFResource)propertyAssertion.Object).SetInference())
                            : new OWLReasonerEvidence(OWLEnums.OWLReasonerEvidenceCategory.Data, nameof(OWLPropertyEntailmentRule), new RDFTriple((RDFResource)propertyAssertion.Subject, compatibleProperty, (RDFLiteral)propertyAssertion.Object).SetInference());

                        //Add the inferences to the report
                        if (!ontology.Data.ABoxGraph.ContainsTriple(evidence.EvidenceContent))
                            report.AddEvidence(evidence);
                    }
            }
            #endregion

            OWLReasonerReport reasonerRuleReport = new OWLReasonerReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectPropertiesEnumerator = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectPropertiesEnumerator.MoveNext())
                InferAssertionsFromPropertyHierarchy(objectPropertiesEnumerator.Current, reasonerRuleReport);

            //owl:DatatypeProperty
            IEnumerator<RDFResource> datatypePropertiesEnumerator = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypePropertiesEnumerator.MoveNext())
                InferAssertionsFromPropertyHierarchy(datatypePropertiesEnumerator.Current, reasonerRuleReport);

            return reasonerRuleReport;
        }
    }
}