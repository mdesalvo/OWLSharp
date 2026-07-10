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
using System.Collections.Generic;

namespace OWLSharp.Reasoner
{
    internal static class OWLSchemaPropertyDomainEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaPropertyDomainEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            #region ObjectPropertyDomain
            foreach (OWLObjectPropertyDomain objectPropertyDomain in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>())
            {
                //scm-dom1: ObjectPropertyDomain(OP,C1) ^ SubClassOf(C1,C2) -> ObjectPropertyDomain(OP,C2)
                foreach (OWLClassExpression superClassExpression in ontology.GetSuperClassesOf(objectPropertyDomain.ClassExpression))
                {
                    OWLObjectPropertyDomain inference = new OWLObjectPropertyDomain(objectPropertyDomain.ObjectPropertyExpression, superClassExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

                //scm-dom2: ObjectPropertyDomain(OP2,C) ^ SubObjectPropertyOf(OP1,OP2) -> ObjectPropertyDomain(OP1,C)
                foreach (OWLObjectPropertyExpression subObjectPropertyExpression in ontology.GetSubObjectPropertiesOf(objectPropertyDomain.ObjectPropertyExpression))
                {
                    OWLObjectPropertyDomain inference = new OWLObjectPropertyDomain(subObjectPropertyExpression, objectPropertyDomain.ClassExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }
            #endregion

            #region DataPropertyDomain
            foreach (OWLDataPropertyDomain dataPropertyDomain in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>())
            {
                //scm-dom1: DataPropertyDomain(DP,C1) ^ SubClassOf(C1,C2) -> DataPropertyDomain(DP,C2)
                foreach (OWLClassExpression superClassExpression in ontology.GetSuperClassesOf(dataPropertyDomain.ClassExpression))
                {
                    OWLDataPropertyDomain inference = new OWLDataPropertyDomain(dataPropertyDomain.DataProperty, superClassExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

                //scm-dom2: DataPropertyDomain(DP2,C) ^ SubDataPropertyOf(DP1,DP2) -> DataPropertyDomain(DP1,C)
                foreach (OWLDataProperty subDataProperty in ontology.GetSubDataPropertiesOf(dataPropertyDomain.DataProperty))
                {
                    OWLDataPropertyDomain inference = new OWLDataPropertyDomain(subDataProperty, dataPropertyDomain.ClassExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }
            #endregion

            return inferences;
        }
    }
}
