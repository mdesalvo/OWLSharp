﻿/*
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

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// TIMEValidator gives TIME validation capabilities to standard validators
    /// </summary>
    public static class TIMEValidator
    {
        #region Methods
        /// <summary>
        /// Adds the given TIME rule to the validator
        /// </summary>
        public static OWLValidator AddTIMERule(this OWLValidator validator, TIMEEnums.TIMEValidatorRules timeRule)
        {
            if (validator != null)
            {
                //Activate TIME extension on the validator
                validator.ActivateExtension<TIMEEnums.TIMEValidatorRules>("TIME", ApplyToOntology);

                //Add TIME rule to the validator
                if (!((List<TIMEEnums.TIMEValidatorRules>)validator.Rules["TIME"]).Contains(timeRule))
                    ((List<TIMEEnums.TIMEValidatorRules>)validator.Rules["TIME"]).Add(timeRule);
            }
            return validator;
        }

        /// <summary>
        /// Applies the TIME validator on the given ontology
        /// </summary>
        internal static void ApplyToOntology(this OWLValidator validator, OWLOntology ontology, Dictionary<string, OWLValidatorReport> evidenceRegistry)
        {
            //Initialize evidence registry
            foreach (TIMEEnums.TIMEValidatorRules timeRule in (List<TIMEEnums.TIMEValidatorRules>)validator.Rules["TIME"])
                evidenceRegistry.Add(timeRule.ToString(), null);

            //Execute rules
            Parallel.ForEach((List<TIMEEnums.TIMEValidatorRules>)validator.Rules["TIME"],
                timeRule =>
                {
                    OWLEvents.RaiseInfo($"Launching TIME validator rule '{timeRule}'");

                    switch (timeRule)
                    {
                        case TIMEEnums.TIMEValidatorRules.TIME_InstantAfter:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_InstantAfter.ToString()] = TIMEInstantAfterRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_InstantBefore:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_InstantBefore.ToString()] = TIMEInstantBeforeRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalAfter.ToString()] = TIMEIntervalAfterRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalBefore:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalBefore.ToString()] = TIMEIntervalBeforeRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalContains:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalContains.ToString()] = TIMEIntervalContainsRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalDisjoint:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalDisjoint.ToString()] = TIMEIntervalDisjointRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalDuring:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalDuring.ToString()] = TIMEIntervalDuringRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalEquals:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalEquals.ToString()] = TIMEIntervalEqualsRule.ExecuteRule(ontology);
                            break;
                        case TIMEEnums.TIMEValidatorRules.TIME_IntervalFinishes:
                            evidenceRegistry[TIMEEnums.TIMEValidatorRules.TIME_IntervalFinishes.ToString()] = TIMEIntervalFinishesRule.ExecuteRule(ontology);
                            break;
                    }

                    OWLEvents.RaiseInfo($"Completed TIME validator rule '{timeRule}': found {evidenceRegistry[timeRule.ToString()].EvidencesCount} evidences");
                });
        }
        #endregion
    }
}