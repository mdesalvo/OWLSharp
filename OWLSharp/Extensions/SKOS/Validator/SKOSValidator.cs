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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSValidator gives SKOS validation capabilities to standard validators
    /// </summary>
    public static class SKOSValidator
    {
        #region Methods
        /// <summary>
        /// Adds the given SKOS rule to the validator
        /// </summary>
        public static OWLValidator AddSKOSRule(this OWLValidator validator, SKOSEnums.SKOSValidatorRules skosRule)
        {
            if (validator != null)
            {
                //Activate SKOS extension on the validator
                validator.ActivateExtension<SKOSEnums.SKOSValidatorRules>("SKOS", ApplyToOntology);

                //Add SKOS rule to the validator
                if (!((List<SKOSEnums.SKOSValidatorRules>)validator.Rules["SKOS"]).Contains(skosRule))
                    ((List<SKOSEnums.SKOSValidatorRules>)validator.Rules["SKOS"]).Add(skosRule);
            }
            return validator;
        }

        /// <summary>
        /// Applies the SKOS validator on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLValidator validator, OWLOntology ontology, Dictionary<string, OWLValidatorReport> evidenceRegistry)
        {
            //Initialize evidence registry
            foreach (SKOSEnums.SKOSValidatorRules skosRule in (List<SKOSEnums.SKOSValidatorRules>)validator.Rules["SKOS"])
                evidenceRegistry.Add(skosRule.ToString(), null);

            //Execute rules
            Parallel.ForEach((List<SKOSEnums.SKOSValidatorRules>)validator.Rules["SKOS"],
                skosRule =>
                {
                    OWLEvents.RaiseInfo($"Launching SKOS validator rule '{skosRule}'");

                    switch (skosRule)
                    {
                        case SKOSEnums.SKOSValidatorRules.Broader:
                            evidenceRegistry[SKOSEnums.SKOSValidatorRules.Broader.ToString()] = SKOSBroaderRule.ExecuteRule(ontology);
                            break;
                        case SKOSEnums.SKOSValidatorRules.LiteralForm:
                            evidenceRegistry[SKOSEnums.SKOSValidatorRules.LiteralForm.ToString()] = SKOSXLLiteralFormRule.ExecuteRule(ontology);
                            break;
                        case SKOSEnums.SKOSValidatorRules.Notation:
                            evidenceRegistry[SKOSEnums.SKOSValidatorRules.Notation.ToString()] = SKOSNotationRule.ExecuteRule(ontology);
                            break;
                        case SKOSEnums.SKOSValidatorRules.PreferredLabel:
                            evidenceRegistry[SKOSEnums.SKOSValidatorRules.PreferredLabel.ToString()] = SKOSPreferredLabelRule.ExecuteRule(ontology);
                            break;
                        case SKOSEnums.SKOSValidatorRules.TopConcept:
                            evidenceRegistry[SKOSEnums.SKOSValidatorRules.TopConcept.ToString()] = SKOSTopConceptRule.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed SKOS validator rule '{skosRule}': found {evidenceRegistry[skosRule.ToString()].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}