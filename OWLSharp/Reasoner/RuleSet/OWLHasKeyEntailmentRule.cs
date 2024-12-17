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
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWLSharp.Reasoner
{
    internal static class OWLHasKeyEntailmentRule
    {
        internal static readonly string rulename = OWLEnums.OWLReasonerRules.HasKeyEntailment.ToString();

        internal static List<OWLInference> ExecuteRule(OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            //Temporary working variables
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
			
            foreach (OWLHasKey hasKeyAxiom in ontology.KeyAxioms)
			{
				//HasKey(C, OP) ^ ClassAssertion(C, I1) ^ ObjectPropertyAssertion(OP, I1, IX) ^ ClassAssertion(C, I2) ^  ObjectPropertyAssertion(OP, I2, IX) -> SameIndividual(I1,I2)
				//HasKey(C, DP) ^ ClassAssertion(C, I1) ^ DataPropertyAssertion(DP, I1, LIT)  ^ ClassAssertion(C, I2) ^  DataPropertyAssertion(DP, I2, LIT)  -> SameIndividual(I1,I2)
				inferences.AddRange(AnalyzeKeyValues(hasKeyAxiom, ontology.GetIndividualsOf(hasKeyAxiom.ClassExpression), opAsns, dpAsns));
			}

            return inferences;
        }

		private static List<OWLInference> AnalyzeKeyValues(OWLHasKey hasKeyAxiom, List<OWLIndividualExpression> hasKeyClassIdvs, 
			List<OWLObjectPropertyAssertion> opAsns, List<OWLDataPropertyAssertion> dpAsns)
		{
			List<OWLInference> inferences = new List<OWLInference>();

			//Temporary working variables
			Dictionary<string, List<OWLIndividualExpression>> objectKeyValueRegister = new Dictionary<string, List<OWLIndividualExpression>>();
			Dictionary<string, List<OWLIndividualExpression>> dataKeyValueRegister = new Dictionary<string, List<OWLIndividualExpression>>();
			OWLIndividualExpression swapIdvExpr;

			#region Compute Keys
			//Iterate individuals of the HasKey axiom's class in order to calculate their key values
			foreach (OWLIndividualExpression idvExpr in hasKeyClassIdvs)
			{
				RDFResource idvExprIRI = idvExpr.GetIRI();

				#region Object Keys
				if (hasKeyAxiom.ObjectPropertyExpressions.Count(opex => opex is OWLObjectProperty) > 0)
				{
					//Calculate the object key values of the current individual
					StringBuilder objSB = new StringBuilder();
					foreach (OWLObjectPropertyExpression keyObjectProperty in hasKeyAxiom.ObjectPropertyExpressions.Where(opex => opex is OWLObjectProperty))
					{
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
						}
						keyObjectPropertyAsns = OWLAxiomHelper.RemoveDuplicates(keyObjectPropertyAsns);
						#endregion

						if (keyObjectPropertyAsns.Count > 0)
							objSB.Append(string.Join("§§", keyObjectPropertyAsns.Where(asn => asn.SourceIndividualExpression.GetIRI().Equals(idvExprIRI))
																				.Select(asn => asn.TargetIndividualExpression.GetIRI().ToString())));
					}

					//Collect the object key values of the current individual into the register
					string objSBValue = objSB.ToString();
					if (!string.IsNullOrEmpty(objSBValue))
					{
						if (!objectKeyValueRegister.ContainsKey(objSBValue))
							objectKeyValueRegister.Add(objSBValue, new List<OWLIndividualExpression>());
						objectKeyValueRegister[objSBValue].Add(idvExpr);
					}
				}				
				#endregion

				#region Data Keys
				if (hasKeyAxiom.DataProperties.Count > 0)
				{
					//Calculate the data key values of the current individual
					StringBuilder dtSB = new StringBuilder();
					foreach (OWLDataProperty keyDataProperty in hasKeyAxiom.DataProperties)
					{
						List<OWLDataPropertyAssertion> keyDataPropertyAsns = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, keyDataProperty);
						if (keyDataPropertyAsns.Count > 0)
							dtSB.Append(string.Join("§§", keyDataPropertyAsns.Where(asn => asn.IndividualExpression.GetIRI().Equals(idvExprIRI))
																			 .Select(asn => asn.Literal.GetLiteral().ToString())));
					}

					//Collect the data key values of the current individual into the register
					string dtSBValue = dtSB.ToString();
					if (!string.IsNullOrEmpty(dtSBValue))
					{
						if (!dataKeyValueRegister.ContainsKey(dtSBValue))
							dataKeyValueRegister.Add(dtSBValue, new List<OWLIndividualExpression>());
						dataKeyValueRegister[dtSBValue].Add(idvExpr);
					}
				}
				#endregion
			}
			#endregion

			#region Analyze Keys
			//There will be inferences only if any object keys have been generated by 2+ individuals
			//HasKey(C, OP) ^ ClassAssertion(C, I1) ^ ObjectPropertyAssertion(OP, I1, IX) ^ ClassAssertion(C, I2) ^  ObjectPropertyAssertion(OP, I2, IX) -> SameIndividual(I1,I2)
			foreach (KeyValuePair<string, List<OWLIndividualExpression>> collisionObjKeyValueRegister in objectKeyValueRegister.Where(kvr => kvr.Value.Count > 1)
																															   .ToDictionary(kv => kv.Key, kv => kv.Value))
			{
				OWLSameIndividual inference = new OWLSameIndividual(collisionObjKeyValueRegister.Value) { IsInference=true };
				inference.GetXML();
                inferences.Add(new OWLInference(rulename, inference));
			}

			//There will be inferences only if any data keys have been generated by 2+ individuals
			//HasKey(C, DP) ^ ClassAssertion(C, I1) ^ DataPropertyAssertion(DP, I1, LIT)  ^ ClassAssertion(C, I2) ^  DataPropertyAssertion(DP, I2, LIT)  -> SameIndividual(I1,I2)
			foreach (KeyValuePair<string, List<OWLIndividualExpression>> collisionDtKeyValueRegister in dataKeyValueRegister.Where(kvr => kvr.Value.Count > 1)
																														    .ToDictionary(kv => kv.Key, kv => kv.Value))
			{
                OWLSameIndividual inference = new OWLSameIndividual(collisionDtKeyValueRegister.Value) { IsInference=true };
                inference.GetXML();
                inferences.Add(new OWLInference(rulename, inference));
            }
			#endregion

			return inferences;
		}
	}
}