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
    /// SKOSValidator analyzes a skos:ConceptScheme in order to discover errors and inconsistencies affecting its taxonomies
    /// </summary>
    public class SKOSValidator
    {
        #region Properties
        /// <summary>
        /// List of rules applied by the SKOS validator
        /// </summary>
        internal List<SKOSEnums.SKOSValidatorRules> Rules { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty SKOS validator
        /// </summary>
        public SKOSValidator()
            => Rules = new List<SKOSEnums.SKOSValidatorRules>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given rule to the SKOS validator
        /// </summary>
        public SKOSValidator AddRule(SKOSEnums.SKOSValidatorRules validatorRule)
        {
            if (!Rules.Contains(validatorRule))
                Rules.Add(validatorRule);
            return this;
        }

        /// <summary>
        /// Applies the SKOS validator on the given skos:ConceptScheme
        /// </summary>
        public OWLValidatorReport ApplyToConceptScheme(SKOSConceptScheme conceptScheme)
        {
            OWLValidatorReport validatorReport = new OWLValidatorReport();

            if (conceptScheme != null)
            {
                OWLEvents.RaiseInfo($"SKOS Validator is going to be applied on skos:ConceptScheme '{conceptScheme.URI}'");

                //Initialize validator registry
                Dictionary<string, OWLValidatorReport> validatorRegistry = new Dictionary<string, OWLValidatorReport>();
                foreach (SKOSEnums.SKOSValidatorRules standardRule in Rules)
                    validatorRegistry.Add(standardRule.ToString(), null);

                //Execute rules
                Parallel.ForEach(Rules,
                    standardRule =>
                    {
                        OWLEvents.RaiseInfo($"Launching SKOS validator rule '{standardRule}'");

                        switch (standardRule)
                        {
                            case SKOSEnums.SKOSValidatorRules.TopConcept:
                                validatorRegistry[SKOSEnums.SKOSValidatorRules.TopConcept.ToString()] = SKOSTopConceptRule.ExecuteRule(conceptScheme);
                                break;
                            case SKOSEnums.SKOSValidatorRules.LiteralForm:
                                validatorRegistry[SKOSEnums.SKOSValidatorRules.LiteralForm.ToString()] = SKOSXLLiteralFormRule.ExecuteRule(conceptScheme);
                                break;
                        }

                        OWLEvents.RaiseInfo($"Completed SKOS validator rule '{standardRule}': found {validatorRegistry[standardRule.ToString()].EvidencesCount} evidences");
                    });

                //Process validator registry
                foreach (OWLValidatorReport validatorRegistryReport in validatorRegistry.Values)
                    validatorReport.MergeEvidences(validatorRegistryReport);

                OWLEvents.RaiseInfo($"SKOS Validator has been applied on skos:ConceptScheme '{conceptScheme.URI}': found {validatorReport.EvidencesCount} evidences");
            }

            return validatorReport;
        }

        /// <summary>
        /// Asynchronously applies the SKOS validator on the given skos:ConceptScheme
        /// </summary>
        public Task<OWLValidatorReport> ApplyToConceptSchemeAsync(SKOSConceptScheme conceptScheme)
            => Task.Run(() => ApplyToConceptScheme(conceptScheme));
        #endregion
    }
}