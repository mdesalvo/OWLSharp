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

using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWLSharp.Reasoner.Rules
{
    internal static class OWLHasKeyEntailmentRule
    {
        internal static List<OWLAxiom> ExecuteRule(OWLOntology ontology)
        {
            List<OWLAxiom> inferences = new List<OWLAxiom>();

            //Temporary working variables
			List<OWLSameIndividual> sameIdvs = ontology.GetAssertionAxiomsOfType<OWLSameIndividual>();
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
			Dictionary<string, List<OWLIndividualExpression>> objectKeyValueRegister = new Dictionary<string, List<OWLIndividualExpression>>();
			
            foreach (OWLHasKey hasKeyAxiom in ontology.KeyAxioms)
			{
				List<OWLIndividualExpression> hasKeyClassIdvs = ontology.GetIndividualsOf(hasKeyAxiom.ClassExpression);

				//HasKey(C, OP) ^ ObjectPropertyAssertion(OP, I1, IX) ^ ObjectPropertyAssertion(OP, I2, IX) -> SameIndividual(I1,I2)
				inferences.AddRange(AnalyzeObjectKeyValues(ontology, hasKeyAxiom, hasKeyClassIdvs, opAsns));

				//HasKey(C, DP) ^ DataPropertyAssertion(DP, I1, LIT) ^ DataPropertyAssertion(DP, I2, LIT) -> SameIndividual(I1,I2)
				inferences.AddRange(AnalyzeDataKeyValues(ontology, hasKeyAxiom, hasKeyClassIdvs, dpAsns));
			}

			//Remove inferences already stated in explicit knowledge
            inferences.RemoveAll(inf => sameIdvs.Any(asn => string.Equals(inf.GetXML(), asn.GetXML())));

            return OWLAxiomHelper.RemoveDuplicates(inferences);
        }

		private static List<OWLAxiom> AnalyzeObjectKeyValues(OWLOntology ontology, OWLHasKey hasKeyAxiom, List<OWLIndividualExpression> hasKeyClassIdvs, List<OWLObjectPropertyAssertion> opAsns)
		{
			List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			Dictionary<string, List<OWLIndividualExpression>> objectKeyValueRegister = new Dictionary<string, List<OWLIndividualExpression>>();
			OWLIndividualExpression swapIdvExpr;

			#region Compute Keys
			//Iterate individuals of the HasKey axiom's class in order to calculate their key values
			foreach (OWLIndividualExpression idvExpr in hasKeyClassIdvs)
			{
				//Calculate the key values of the current individual
                StringBuilder sb = new StringBuilder();
                foreach (OWLObjectPropertyExpression keyObjectProperty in hasKeyAxiom.ObjectPropertyExpressions)
                {
					OWLObjectProperty keyObjectPropertyInvOfValue = (keyObjectProperty as OWLObjectInverseOf)?.ObjectProperty;

					#region Calibration
					List<OWLObjectPropertyAssertion> keyObjectPropertyAsns = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, keyObjectProperty);
                    for (int i=0; i<keyObjectPropertyAsns.Count; i++)
					{
						//In case the object assertion works under inverse logic, we must swap source/target of the object assertion
						if (keyObjectPropertyAsns[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
						{   
							swapIdvExpr = keyObjectPropertyAsns[i].SourceIndividualExpression;
							keyObjectPropertyAsns[i].SourceIndividualExpression = keyObjectPropertyAsns[i].TargetIndividualExpression;
							keyObjectPropertyAsns[i].TargetIndividualExpression = swapIdvExpr;
							keyObjectPropertyAsns[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
						}

						//In case the key object property works under inverse logic, we must swap source/target of the object assertion
						if (keyObjectPropertyInvOfValue != null)
						{
							swapIdvExpr = keyObjectPropertyAsns[i].SourceIndividualExpression;
							keyObjectPropertyAsns[i].SourceIndividualExpression = keyObjectPropertyAsns[i].TargetIndividualExpression;
							keyObjectPropertyAsns[i].TargetIndividualExpression = swapIdvExpr;
							keyObjectPropertyAsns[i].ObjectPropertyExpression = keyObjectPropertyInvOfValue;
						}
					}
					keyObjectPropertyAsns = OWLAxiomHelper.RemoveDuplicates(keyObjectPropertyAsns);
					#endregion

					if (keyObjectPropertyAsns.Count > 0)
                        sb.Append(string.Join("§§", keyObjectPropertyAsns.Select(asn => asn.TargetIndividualExpression.GetIRI().ToString())));
                }

                //Collect the key values of the current individual into the register
                string sbValue = sb.ToString();
                if (!string.IsNullOrEmpty(sbValue))
                {
                    if (!objectKeyValueRegister.ContainsKey(sbValue))
                        objectKeyValueRegister.Add(sbValue, new List<OWLIndividualExpression>());
                    objectKeyValueRegister[sbValue].Add(idvExpr);
                }
			}
			#endregion

			#region Analyze Keys
			//Restrict analysis to keys which have been generated by 2+ individuals
            foreach (KeyValuePair<string, List<OWLIndividualExpression>> collisionKeyValueRegister in objectKeyValueRegister.Where(kvr => kvr.Value.Count > 1)
                                                                                                            				.ToDictionary(kv => kv.Key, kv => kv.Value))
				inferences.Add(new OWLSameIndividual(collisionKeyValueRegister.Value) { IsInference=true });
			#endregion

			return inferences;
		}
    
		private static List<OWLAxiom> AnalyzeDataKeyValues(OWLOntology ontology, OWLHasKey hasKeyAxiom, List<OWLIndividualExpression> hasKeyClassIdvs, List<OWLDataPropertyAssertion> dpAsns)
		{
			List<OWLAxiom> inferences = new List<OWLAxiom>();

			//Temporary working variables
			Dictionary<string, List<OWLIndividualExpression>> dataKeyValueRegister = new Dictionary<string, List<OWLIndividualExpression>>();

			#region Compute Keys
			//Iterate individuals of the HasKey axiom's class in order to calculate their key values
			foreach (OWLIndividualExpression idvExpr in hasKeyClassIdvs)
			{
				//Calculate the key values of the current individual
                StringBuilder sb = new StringBuilder();
                foreach (OWLDataProperty keyDataProperty in hasKeyAxiom.DataProperties)
                {
					List<OWLDataPropertyAssertion> keyDataPropertyAsns = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, keyDataProperty);
					if (keyDataPropertyAsns.Count > 0)
                        sb.Append(string.Join("§§", keyDataPropertyAsns.Select(asn => asn.Literal.GetLiteral().ToString())));
                }

                //Collect the key values of the current individual into the register
                string sbValue = sb.ToString();
                if (!string.IsNullOrEmpty(sbValue))
                {
                    if (!dataKeyValueRegister.ContainsKey(sbValue))
                        dataKeyValueRegister.Add(sbValue, new List<OWLIndividualExpression>());
                    dataKeyValueRegister[sbValue].Add(idvExpr);
                }
			}
			#endregion

			#region Analyze Keys
			//Restrict analysis to keys which have been generated by 2+ individuals
            foreach (KeyValuePair<string, List<OWLIndividualExpression>> collisionKeyValueRegister in dataKeyValueRegister.Where(kvr => kvr.Value.Count > 1)
                                                                                                            			  .ToDictionary(kv => kv.Key, kv => kv.Value))
				inferences.Add(new OWLSameIndividual(collisionKeyValueRegister.Value) { IsInference=true });
			#endregion

			return inferences;
		}
    
	}
}