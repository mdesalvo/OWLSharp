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
using System.Linq;

namespace OWLSharp.Reasoner
{
    internal static class OWLSchemaHasValueEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaHasValueEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
            List<OWLSubClassOf> scofAxms = ontology.GetClassAxiomsOfType<OWLSubClassOf>();
            List<OWLEquivalentClasses> eqClsAxms = ontology.GetClassAxiomsOfType<OWLEquivalentClasses>();

            #region ObjectHasValue
            //Collect all ObjectHasValue expressions referenced anywhere in the T-Box (SubClassOf, EquivalentClasses)
            List<OWLObjectHasValue> referencedObjHasValues = new List<OWLObjectHasValue>();
            referencedObjHasValues.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLObjectHasValue).Select(ax => (OWLObjectHasValue)ax.SuperClassExpression));
            referencedObjHasValues.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLObjectHasValue).Select(ax => (OWLObjectHasValue)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedObjHasValues.AddRange(eqAxm.ClassExpressions.OfType<OWLObjectHasValue>());
            referencedObjHasValues = OWLExpressionHelper.RemoveDuplicates(referencedObjHasValues);

            //scm-hv: ObjectHasValue(OP1,I) [referenced] ^ ObjectHasValue(OP2,I) [referenced] ^ SubObjectPropertyOf(OP1,OP2) -> SubClassOf(ObjectHasValue(OP1,I),ObjectHasValue(OP2,I))
            foreach (OWLObjectHasValue objHasValue1 in referencedObjHasValues)
                foreach (OWLObjectHasValue objHasValue2 in referencedObjHasValues)
                {
                    if (objHasValue1 == objHasValue2)
                        continue;

                    //Both restrictions must be pinned on the very same individual value
                    if (!objHasValue1.IndividualExpression.GetIRI().Equals(objHasValue2.IndividualExpression.GetIRI()))
                        continue;

                    //OP1 must be the same property as OP2, or a (transitive) sub-property of it
                    bool isSameOrSubProperty = objHasValue1.ObjectPropertyExpression.GetIRI().Equals(objHasValue2.ObjectPropertyExpression.GetIRI())
                                                 || ontology.CheckIsSubObjectPropertyOf(objHasValue1.ObjectPropertyExpression, objHasValue2.ObjectPropertyExpression);
                    if (!isSameOrSubProperty)
                        continue;

                    OWLSubClassOf inference = new OWLSubClassOf(objHasValue1, objHasValue2) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            #endregion

            #region DataHasValue
            //Collect all DataHasValue expressions referenced anywhere in the T-Box (SubClassOf, EquivalentClasses)
            List<OWLDataHasValue> referencedDataHasValues = new List<OWLDataHasValue>();
            referencedDataHasValues.AddRange(scofAxms.Where(ax => ax.SuperClassExpression is OWLDataHasValue).Select(ax => (OWLDataHasValue)ax.SuperClassExpression));
            referencedDataHasValues.AddRange(scofAxms.Where(ax => ax.SubClassExpression is OWLDataHasValue).Select(ax => (OWLDataHasValue)ax.SubClassExpression));
            foreach (OWLEquivalentClasses eqAxm in eqClsAxms)
                referencedDataHasValues.AddRange(eqAxm.ClassExpressions.OfType<OWLDataHasValue>());
            referencedDataHasValues = OWLExpressionHelper.RemoveDuplicates(referencedDataHasValues);

            //scm-hv: DataHasValue(DP1,LIT) [referenced] ^ DataHasValue(DP2,LIT) [referenced] ^ SubDataPropertyOf(DP1,DP2) -> SubClassOf(DataHasValue(DP1,LIT),DataHasValue(DP2,LIT))
            foreach (OWLDataHasValue dataHasValue1 in referencedDataHasValues)
                foreach (OWLDataHasValue dataHasValue2 in referencedDataHasValues)
                {
                    if (dataHasValue1 == dataHasValue2)
                        continue;

                    //Both restrictions must be pinned on the very same literal value
                    if (!dataHasValue1.Literal.GetLiteral().Equals(dataHasValue2.Literal.GetLiteral()))
                        continue;

                    //DP1 must be the same property as DP2, or a (transitive) sub-property of it
                    bool isSameOrSubProperty = dataHasValue1.DataProperty.GetIRI().Equals(dataHasValue2.DataProperty.GetIRI())
                                                 || ontology.CheckIsSubDataPropertyOf(dataHasValue1.DataProperty, dataHasValue2.DataProperty);
                    if (!isSameOrSubProperty)
                        continue;

                    OWLSubClassOf inference = new OWLSubClassOf(dataHasValue1, dataHasValue2) { IsInference=true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(rulename, inference));
                }
            #endregion

            return inferences;
        }
    }
}
