/*
  Copyright 2014-2026 Marco De Salvo
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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>OWLSharp extension: no direct W3C RL/RDF correspondent for owl:top/bottomObjectProperty and owl:top/bottomDataProperty root/bottom-position checks (analogous in spirit to cls-thing/cls-nothing1, but the property-level axiomatic triples are not part of the given RL/RDF table).
    /// T1/T2/B1/B2 (mispositioning of top/bottom properties within SubObjectPropertyOf/SubDataPropertyOf axioms) are a stylistic pitfall -- Warning, mirroring OWLThingNothingAnalysis' T1/N1.
    /// B3/B4 (an actual property assertion using owl:bottomObjectProperty/owl:bottomDataProperty as predicate) are a genuine contradiction, since the bottom property is defined to always be empty -- Error, mirroring OWLThingNothingAnalysis' N2</para>
    /// </summary>
    internal static class OWLTopBottomAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.TopBottomAnalysis);
        internal const string rulesuggT1 = "There should not be any direct or indirect SubObjectPropertyOf axioms having reserved owl:topObjectProperty property in position of subproperty: this object property should be the root object property!";
        internal const string rulesuggT2 = "There should not be any direct or indirect SubDataPropertyOf axioms having reserved owl:topDataProperty property in position of subproperty: this data property should be the root data property!";
        internal const string rulesuggB1 = "There should not be any direct or indirect SubObjectPropertyOf axioms having reserved owl:bottomObjectProperty property in position of superproperty: this object property should be the bottom object property!";
        internal const string rulesuggB2 = "There should not be any direct or indirect SubDataPropertyOf axioms having reserved owl:bottomDataProperty property in position of superproperty: this data property should be the bottom data property!";
        internal const string rulesuggB3 = "There should not be any individuals stating object assertions with owl:bottomObjectProperty!";
        internal const string rulesuggB4 = "There should not be any individuals stating data assertions with owl:bottomDataProperty!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //T1/T2/B1/B2: mispositioning the reserved top/bottom property within a SubObjectPropertyOf/SubDataPropertyOf hierarchy is a modeling
            //smell (it forces the involved property to collapse to top/bottom, which is legal OWL2 but almost certainly not what was intended),
            //not a strict logical contradiction by itself -- so these are reported as Warning, exactly like OWLThingNothingAnalysis' T1/N1
            //(owl:Thing/owl:Nothing mispositioning) which is the direct structural analogue of this check at the class level
            if (ontology.GetSuperObjectPropertiesOf(RDFVocabulary.OWL.TOP_OBJECT_PROPERTY.ToEntity<OWLObjectProperty>()).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected object property axioms causing reserved owl:topObjectProperty property to not be the root object property of the ontology",
                    rulesuggT1));

            if (ontology.GetSuperDataPropertiesOf(RDFVocabulary.OWL.TOP_DATA_PROPERTY.ToEntity<OWLDataProperty>()).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected data property axioms causing reserved owl:topDataProperty property to not be the root data property of the ontology",
                    rulesuggT2));

            if (ontology.GetSubObjectPropertiesOf(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY.ToEntity<OWLObjectProperty>()).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected object property axioms causing reserved owl:bottomObjectProperty property to not be the bottom object property of the ontology",
                    rulesuggB1));

            if (ontology.GetSubDataPropertiesOf(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY.ToEntity<OWLDataProperty>()).Count > 0)
                issues.Add(new OWLIssue(
                    OWLEnums.OWLIssueSeverity.Warning,
                    rulename,
                    "Detected data property axioms causing reserved owl:bottomDataProperty property to not be the bottom data property of the ontology",
                    rulesuggB2));

            //B3/B4: an actual assertion using the reserved bottom property as predicate IS a genuine contradiction (not just a modeling smell),
            //because owl:bottomObjectProperty/owl:bottomDataProperty are defined to always be empty -- so any instance stating otherwise is
            //logically false, exactly like OWLThingNothingAnalysis' N2 (individuals asserted to belong to owl:Nothing), which is also Error
            issues.AddRange(from opAsn
                            in OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology)
                            where opAsn.ObjectPropertyExpression.GetIRI().Equals(RDFVocabulary.OWL.BOTTOM_OBJECT_PROPERTY)
                            select new OWLIssue(
                                OWLEnums.OWLIssueSeverity.Error,
                                rulename,
                                "Detected object property assertion having owl:bottomObjectProperty as predicate: this is not allowed",
                                rulesuggB3));
            issues.AddRange(from dpAsn
                            in ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
                            where dpAsn.DataProperty.GetIRI().Equals(RDFVocabulary.OWL.BOTTOM_DATA_PROPERTY)
                            select new OWLIssue(
                                OWLEnums.OWLIssueSeverity.Error,
                                rulename,
                                "Detected data property assertion having owl:bottomDataProperty as predicate: this is not allowed",
                                rulesuggB4));

            return issues;
        }
    }
}