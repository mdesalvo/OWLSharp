/*
   Copyright 2012-2023 Marco De Salvo
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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL2 validator rule checking for consistency of assertions using disjoint properties [OWL2]
    /// </summary>
    internal static class OWLPropertyDisjointRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            #region DetectRuleViolation
            bool DetectObjectRuleViolation(RDFTriple objectAssertion, RDFTriple disjointObjectAssertion)
            {
                bool isSameSubject = objectAssertion.Subject.Equals(disjointObjectAssertion.Subject);
                bool isSameObject = objectAssertion.Object.Equals(disjointObjectAssertion.Object);
                if (isSameSubject && isSameObject)
                    return true; //exact violation
                
                bool isSynonimSubject = ontology.Data.CheckIsSameIndividual((RDFResource)objectAssertion.Subject, (RDFResource)disjointObjectAssertion.Subject);
                if (isSynonimSubject && isSameObject)
                    return true; //inferred violation on synonim subject

                bool isSynonimObject = ontology.Data.CheckIsSameIndividual((RDFResource)objectAssertion.Object, (RDFResource)disjointObjectAssertion.Object);
                if (isSameSubject && isSynonimObject)
                    return true; //inferred violation on synonim object

                if (isSynonimSubject && isSynonimObject)
                    return true; //inferred violation on synonim subject and synonim object

                return false; //no violation
            }
            bool DetectDatatypeRuleViolation(RDFTriple datatypeAssertion, RDFTriple disjointDatatypeAssertion)
            {
                bool isSameSubject = datatypeAssertion.Subject.Equals(disjointDatatypeAssertion.Subject);
                bool isSameObject = datatypeAssertion.Object.Equals(disjointDatatypeAssertion.Object);
                if (isSameSubject && isSameObject)
                    return true; //exact violation

                bool isSynonimSubject = ontology.Data.CheckIsSameIndividual((RDFResource)datatypeAssertion.Subject, (RDFResource)disjointDatatypeAssertion.Subject);
                if (isSynonimSubject && isSameObject)
                    return true; //inferred violation on synonim subject

                return false; //no violation
            }
            #endregion

            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:ObjectProperty
            IEnumerator<RDFResource> objectProperties = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectProperties.MoveNext())
            {
                //Consider object assertions using the current object property
                RDFGraph objectAssertionsGraph = ontology.Data.ABoxGraph[null, objectProperties.Current, null, null];

                //Calculate object assertions using its 'owl:disjointPropertyWith' related properties
                RDFGraph disjointObjectAssertionsGraph = new RDFGraph();
                foreach (RDFResource disjointObjectProperty in ontology.Model.PropertyModel.GetDisjointPropertiesWith(objectProperties.Current))
                    disjointObjectAssertionsGraph = disjointObjectAssertionsGraph.UnionWith(ontology.Data.ABoxGraph[null, disjointObjectProperty, null, null]);

                //There should not be disjoint object assertions connecting the same subject individual with the same object individual
                foreach (RDFTriple objectAssertion in objectAssertionsGraph)
                    if (disjointObjectAssertionsGraph.Any(disjointObjectAssertion => DetectObjectRuleViolation(objectAssertion, disjointObjectAssertion)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLPropertyDisjointRule),
                            $"Violation of 'owl:propertyDisjointWith' behavior on object property '{objectProperties.Current}'",
                            "Revise your object assertions: there should not be disjoint object assertions connecting the same subject individual with the same object individual"));
            }

            //owl:DatatypeProperty
            IEnumerator<RDFResource> datatypeProperties = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypeProperties.MoveNext())
            {
                //Consider datatype assertions using the current object property
                RDFGraph datatypeAssertionsGraph = ontology.Data.ABoxGraph[null, datatypeProperties.Current, null, null];

                //Calculate datatype assertions using its 'owl:disjointPropertyWith' related properties
                RDFGraph disjointDatatypeAssertionsGraph = new RDFGraph();
                foreach (RDFResource disjointDatatypeProperty in ontology.Model.PropertyModel.GetDisjointPropertiesWith(datatypeProperties.Current))
                    disjointDatatypeAssertionsGraph = disjointDatatypeAssertionsGraph.UnionWith(ontology.Data.ABoxGraph[null, disjointDatatypeProperty, null, null]);

                //There should not be disjoint datatype assertions connecting the same subject individual with the same literal
                foreach (RDFTriple datatypeAssertion in datatypeAssertionsGraph)
                    if (disjointDatatypeAssertionsGraph.Any(disjointDatatypeAssertion => DetectDatatypeRuleViolation(datatypeAssertion, disjointDatatypeAssertion)))
                        validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                            OWLEnums.OWLValidatorEvidenceCategory.Error,
                            nameof(OWLPropertyDisjointRule),
                            $"Violation of 'owl:propertyDisjointWith' behavior on datatype property '{datatypeProperties.Current}'",
                            "Revise your datatype assertions: there should not be disjoint datatype assertions connecting the same subject individual with the same literal"));
            }

            return validatorRuleReport;
        }
    }
}