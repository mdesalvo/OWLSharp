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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp
{
    /// <summary>
    /// OWL-DL validator rule checking for consistency of owl:inverseOf behavior of properties 
    /// </summary>
    internal static class OWLInverseOfRule
    {
        internal static OWLValidatorReport ExecuteRule(OWLOntology ontology)
        {
            #region CheckClassCompatibility
            bool CheckClassCompatibility(RDFResource leftClass, RDFResource rightClass)
                => leftClass.Equals(rightClass)
                     || ontology.Model.ClassModel.CheckIsEquivalentClassOf(leftClass, rightClass)
                       || ontology.Model.ClassModel.CheckIsSubClassOf(leftClass, rightClass);
            #endregion

            OWLValidatorReport validatorRuleReport = new OWLValidatorReport();

            //owl:inverseOf
            foreach (RDFTriple inverseOfTriple in ontology.Model.PropertyModel.TBoxGraph[null, RDFVocabulary.OWL.INVERSE_OF, null, null])
            {
                //Check rdfs:domain of the subject property against rdfs:range of object property
                List<RDFResource> subjectDomainClasses = ontology.Model.PropertyModel.GetDomainOf((RDFResource)inverseOfTriple.Subject);
                List<RDFResource> objectRangeClasses = ontology.Model.PropertyModel.GetRangeOf((RDFResource)inverseOfTriple.Object);
                if (subjectDomainClasses.Any() && objectRangeClasses.Any() && subjectDomainClasses.Any(domainClass => !objectRangeClasses.Any(rangeClass => CheckClassCompatibility(domainClass, rangeClass))))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLInverseOfRule),
                        $"Violation of 'owl:inverseOf' behavior between properties '{inverseOfTriple.Subject}' and '{inverseOfTriple.Object}'",
                        $"Revise 'rdfs:domain' definition of property '{inverseOfTriple.Subject}' in order to be compatible with 'rdfs:range' definition of property '{inverseOfTriple.Object}'"));

                //Check rdfs:range of the subject property against rdfs:domain of object property
                List<RDFResource> subjectRangeClasses = ontology.Model.PropertyModel.GetRangeOf((RDFResource)inverseOfTriple.Subject);
                List<RDFResource> objectDomainClasses = ontology.Model.PropertyModel.GetDomainOf((RDFResource)inverseOfTriple.Object);
                if (subjectRangeClasses.Any() && objectDomainClasses.Any() && subjectRangeClasses.Any(rangeClass => !objectDomainClasses.Any(domainClass => CheckClassCompatibility(rangeClass, domainClass))))
                    validatorRuleReport.AddEvidence(new OWLValidatorEvidence(
                        OWLEnums.OWLValidatorEvidenceCategory.Error,
                        nameof(OWLInverseOfRule),
                        $"Violation of 'owl:inverseOf' behavior between properties '{inverseOfTriple.Subject}' and '{inverseOfTriple.Object}'",
                        $"Revise 'rdfs:range' definition of property '{inverseOfTriple.Subject}' in order to be compatible with 'rdfs:domain' definition of property '{inverseOfTriple.Object}'"));
            }

            return validatorRuleReport;
        }
    }
}