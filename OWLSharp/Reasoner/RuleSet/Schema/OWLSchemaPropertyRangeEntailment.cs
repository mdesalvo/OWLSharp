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
    internal static class OWLSchemaPropertyRangeEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaPropertyRangeEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            #region ObjectPropertyRange
            foreach (OWLObjectPropertyRange objectPropertyRange in ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>())
            {
                //scm-rng1: ObjectPropertyRange(OP,C1) ^ SubClassOf(C1,C2) -> ObjectPropertyRange(OP,C2)
                foreach (OWLClassExpression superClassExpression in ontology.GetSuperClassesOf(objectPropertyRange.ClassExpression))
                {
                    OWLObjectPropertyRange inference = new OWLObjectPropertyRange(objectPropertyRange.ObjectPropertyExpression, superClassExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }

                //scm-rng2: ObjectPropertyRange(OP2,C) ^ SubObjectPropertyOf(OP1,OP2) -> ObjectPropertyRange(OP1,C)
                foreach (OWLObjectPropertyExpression subObjectPropertyExpression in ontology.GetSubObjectPropertiesOf(objectPropertyRange.ObjectPropertyExpression))
                {
                    OWLObjectPropertyRange inference = new OWLObjectPropertyRange(subObjectPropertyExpression, objectPropertyRange.ClassExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            }
            #endregion

            #region DataPropertyRange
            //NOTE: only the scm-rng2 (subPropertyOf propagation) branch applies here: scm-rng1's SubClassOf premise does not make sense for a
            //DataPropertyRange, whose range is a data range expression (datatype) rather than an OWLClassExpression subject to a class hierarchy
            foreach (OWLDataPropertyRange dataPropertyRange in ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>())
                foreach (OWLDataProperty subDataProperty in ontology.GetSubDataPropertiesOf(dataPropertyRange.DataProperty))
                {
                    OWLDataPropertyRange inference = new OWLDataPropertyRange(subDataProperty, dataPropertyRange.DataRangeExpression) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            #endregion

            return inferences;
        }
    }
}
