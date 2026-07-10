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

namespace OWLSharp.Reasoner
{
    internal static class OWLSchemaClassEntailment
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLReasonerRules.SchemaClassEntailment);

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables (owl:Thing/owl:Nothing are always implicitly typed as classes by cls-thing/cls-nothing1)
            OWLClass owlThing = new OWLClass(RDFVocabulary.OWL.THING);
            OWLClass owlNothing = new OWLClass(RDFVocabulary.OWL.NOTHING);

            foreach (OWLClass declaredClass in ontology.GetDeclarationAxiomsOfType<OWLClass>()
                                                       .Select(ax => (OWLClass)ax.Entity))
            {
                //Skip materializing the reflexivity/root/bottom pattern on owl:Thing and owl:Nothing themselves:
                //it would only produce meaningless self-loops (SubClassOf(owl:Thing,owl:Thing), SubClassOf(owl:Nothing,owl:Thing), ...)
                RDFResource declaredClassIRI = declaredClass.GetIRI();
                if (declaredClassIRI.Equals(RDFVocabulary.OWL.THING) || declaredClassIRI.Equals(RDFVocabulary.OWL.NOTHING))
                    continue;

                //scm-cls: Class(C) -> SubClassOf(C,C)
                OWLSubClassOf selfSubClassOf = new OWLSubClassOf(declaredClass, declaredClass) { IsInference=true };
                selfSubClassOf.GetXML();
                inferences.Add(new OWLInference(rulename, selfSubClassOf));

                //scm-cls: Class(C) -> EquivalentClasses(C,C)
                OWLEquivalentClasses selfEquivalentClasses = new OWLEquivalentClasses(new List<OWLClassExpression> { declaredClass, declaredClass }) { IsInference=true };
                selfEquivalentClasses.GetXML();
                inferences.Add(new OWLInference(rulename, selfEquivalentClasses));

                //scm-cls: Class(C) -> SubClassOf(C,owl:Thing)
                OWLSubClassOf thingSubClassOf = new OWLSubClassOf(declaredClass, owlThing) { IsInference=true };
                thingSubClassOf.GetXML();
                inferences.Add(new OWLInference(rulename, thingSubClassOf));

                //scm-cls: Class(C) -> SubClassOf(owl:Nothing,C)
                OWLSubClassOf nothingSubClassOf = new OWLSubClassOf(owlNothing, declaredClass) { IsInference=true };
                nothingSubClassOf.GetXML();
                inferences.Add(new OWLInference(rulename, nothingSubClassOf));
            }

            return inferences;
        }
    }
}
