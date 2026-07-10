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

#if !NET8_0_OR_GREATER
using Dasync.Collections;
#endif
using System;
using OWLSharp.Ontology;
using OWLSharp.Profiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Reasoner
{
    /// <summary>
    /// OWLReasoner is an inference engine that processes an ontology by analyzing its
    /// T-BOX (terminological/schema knowledge about classes and properties),
    /// A-BOX (assertional knowledge about individuals and their relationships), and
    /// R-BOX (rules for additional inference patterns).
    /// The reasoner applies logical deduction to derive implicit knowledge that is entailed
    /// but not explicitly stated, such as inferring class memberships, property relationships,
    /// detecting inconsistencies, and computing class hierarchies, thereby materializing
    /// the full logical consequences of the ontology's axioms and rules.
    /// </summary>
    public sealed class OWLReasoner
    {
        #region Properties
        /// <summary>
        /// A predefined reasoner including all available OWL2 reasoner rules (Schema + Fact)
        /// </summary>
        public static readonly OWLReasoner Default = new OWLReasoner {
            Rules = Enum.GetValues(typeof(OWLEnums.OWLReasonerRules)).Cast<OWLEnums.OWLReasonerRules>().ToList() };

        /// <summary>
        /// The set of rules to be applied by the reasoner
        /// </summary>
        public List<OWLEnums.OWLReasonerRules> Rules { get; internal set; } = new List<OWLEnums.OWLReasonerRules>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given rule to the reasoner
        /// </summary>
        /// <returns>The reasoner itself</returns>
        public OWLReasoner AddRule(OWLEnums.OWLReasonerRules rule)
        {
            Rules.Add(rule);
            return this;
        }

        /// <summary>
        /// Applies the reasoner to the given ontology, using the eventually specified options.<br/><br/>
        /// Execution is organized in two sequential phases:<br/>
        /// 1) <b>Schema phase</b> - runs once, before anything else, applying only the selected Schema-tier rules and
        /// looping internally until no new Schema-tier inference is produced (a real fixpoint, not a capped approximation:
        /// no Reasoner rule derives a T-Box axiom from an A-Box fact, so the Schema tier is acyclic with respect to the
        /// Fact tier and therefore never needs to be revisited once closed).<br/>
        /// 2) <b>Fact phase</b> - runs afterwards, applying only the selected Fact-tier rules (plus any SWRL rules
        /// configured on the ontology) against the now-schema-stable ontology, under the existing iterative/capped
        /// fixpoint logic (see <see cref="OWLReasonerOptions"/>). If <see cref="OWLReasonerOptions.ForceRLFixpointConvergence"/>
        /// is explicitly opted into, see <see cref="DetermineMaxAllowedIterationsAsync"/> for how the Fact-phase cap
        /// is (or isn't) relaxed.
        /// </summary>
        /// <returns>The report collecting the discovered inferences and the Fact-phase fixpoint iteration metrics</returns>
        public async Task<OWLReasonerReport> ApplyToOntologyAsync(OWLOntology ontology, OWLReasonerOptions reasonerOptions=null)
        {
            if (reasonerOptions == null)
                reasonerOptions = new OWLReasonerOptions();

            // Initialize structures and counters for the two-phase reasoning: schema, facts
            List<OWLInference> schemaInferences = new List<OWLInference>();
            uint schemaClosureRounds = 0;
            List<OWLInference> factInferences = new List<OWLInference>();
            uint factIterationsPerformed = 0;
            bool factReachedFixpoint = true;

            #region Execute
            if (ontology != null)
            {
                OWLEvents.RaiseInfo($"Launching two-phase reasoner on ontology '{ontology.IRI}'...");

                // Deduplicate the rule set
                Rules = Rules.Distinct().ToList();
                // Separate the rule tiers
                List<OWLEnums.OWLReasonerRules> schemaRules = Rules.Where(rule => rule.ToString().StartsWith("Schema", StringComparison.Ordinal)).ToList();
                List<OWLEnums.OWLReasonerRules> factRules = Rules.Where(rule => rule.ToString().StartsWith("Fact", StringComparison.Ordinal)).ToList();

                #region Schema phase
                if (schemaRules.Count > 0)
                {
                    OWLEvents.RaiseInfo("Launching Schema phase (T-Box closure)...");

                    // Execute the first round of T-Box (schema closure) reasoning
                    List<OWLInference> roundInferences = await ExecuteRulesAsync(ontology, schemaRules, includeSWRLRules: false);
                    schemaClosureRounds = 1;
                    schemaInferences.AddRange(roundInferences);

                    // Iterate the subsequent rounds of T-Box (schema closure) reasoning, if enabled.
                    // At each round, schema inferences are phisically merged into the ontology
                    if (reasonerOptions.EnableIterativeReasoning)
                    {
                        while (roundInferences.Count > 0)
                        {
                            MergeInferences(ontology, roundInferences);

                            roundInferences = await ExecuteRulesAsync(ontology, schemaRules, includeSWRLRules: false);
                            schemaClosureRounds++;
                            schemaInferences.AddRange(roundInferences);
                        }
                    }

                    OWLEvents.RaiseInfo($"Completed Schema phase in {schemaClosureRounds} round(s) => {schemaInferences.Count} inferences");
                }
                #endregion

                #region Fact phase
                if (factRules.Count > 0 || ontology.Rules.Count > 0)
                {
                    OWLEvents.RaiseInfo("Launching Fact phase (assertion/individual-level propagation and SWRL rules)...");

                    // Adjust options for customizing the fact phase on the given preferences
                    OWLReasonerOptions factPhaseOptions = new OWLReasonerOptions
                    {
                        EnableIterativeReasoning = reasonerOptions.EnableIterativeReasoning,
                        ForceRLFixpointConvergence = reasonerOptions.ForceRLFixpointConvergence,
                        MaxAllowedIterations = await DetermineMaxAllowedIterationsAsync(ontology, reasonerOptions)
                    };

                    // Run the Fact-phase and capture its outcome: the inferences it produced, how many iterations
                    // it actually performed, and whether it stopped because it hit a real fixpoint or the cap
                    (List<OWLInference> Inferences, uint IterationsPerformed, bool ReachedFixpoint) factResult =
                        await IterateFactPhaseAsync(ontology, factPhaseOptions, factRules);
                    factInferences = factResult.Inferences;
                    factIterationsPerformed = factResult.IterationsPerformed;
                    factReachedFixpoint = factResult.ReachedFixpoint;

                    OWLEvents.RaiseInfo($"Completed Fact phase in {factIterationsPerformed} iteration(s) => {factInferences.Count} inferences");
                }
                #endregion

                OWLEvents.RaiseInfo($"Completed two-phase reasoner on ontology '{ontology.IRI}' => {schemaInferences.Count + factInferences.Count} total inferences");
            }
            #endregion

            // Collect inferences from the two-phase reasoning: schema, facts
            List<OWLInference> allInferences = new List<OWLInference>(schemaInferences.Count + factInferences.Count);
            allInferences.AddRange(schemaInferences);
            allInferences.AddRange(factInferences);

            // Give the report of the reasoning process
            return new OWLReasonerReport
            {
                Inferences = allInferences,
                IterationsPerformed = factIterationsPerformed,
                ReachedFixpoint = factReachedFixpoint,
                SchemaClosureRounds = schemaClosureRounds
            };
        }

        /// <summary>
        /// Determines the value of <see cref="OWLReasonerOptions.MaxAllowedIterations"/> that governs the Fact phase,
        /// checking OWL2-RL compliance and relaxing the cap only if the caller opted into <see cref="OWLReasonerOptions.ForceRLFixpointConvergence"/>.<br/>
        /// </summary>
        private static async Task<uint> DetermineMaxAllowedIterationsAsync(OWLOntology ontology, OWLReasonerOptions reasonerOptions)
        {
            // If we are not forced into RL-checking, we must honour the preference for maximum allowed iterations
            if (!reasonerOptions.ForceRLFixpointConvergence)
                return reasonerOptions.MaxAllowedIterations;

            // Otherwise, before effetively pursuing RL-compliance, we can directly honour an explicitly customized limit
            uint defaultMaxAllowedIterations = new OWLReasonerOptions().MaxAllowedIterations;
            if (reasonerOptions.MaxAllowedIterations != defaultMaxAllowedIterations)
                return reasonerOptions.MaxAllowedIterations;

            // At last we have to check for RL-compliance: if we detect this is an OWL2/RL ontology we can unset the limit
            return (await ontology.CheckProfileAsync(OWLEnums.OWLProfiles.RL)).IsCompliant ? uint.MaxValue : defaultMaxAllowedIterations;
        }

        /// <summary>
        /// Executes a single Fact-tier (+ SWRL) reasoning iteration and, if iterative reasoning is enabled and new
        /// knowledge was found, recurses until fixpoint or <see cref="OWLReasonerOptions.MaxAllowedIterations"/> is reached.
        /// </summary>
        private static async Task<(List<OWLInference> Inferences, uint IterationsPerformed, bool ReachedFixpoint)> IterateFactPhaseAsync(
            OWLOntology ontology, OWLReasonerOptions reasonerOptions, List<OWLEnums.OWLReasonerRules> factRules)
        {
            // Run one Fact-tier (+ SWRL) round against the ontology's current state (schema already closed, plus
            // whatever the previous Fact-phase rounds have merged in so far)
            List<OWLInference> inferences = await ExecuteRulesAsync(ontology, factRules, includeSWRLRules: true);

            //This iteration is the one currently in progress: record it as the (so far) last performed iteration
            uint iterationsPerformed = reasonerOptions.CurrentIteration;
            bool reachedFixpoint;

            // Keep recursing only if: (1) the caller allows iteration at all, (2) this round actually found
            // something new (no point recursing on an empty round), and (3) the cap hasn't been reached yet
            if (reasonerOptions.EnableIterativeReasoning
                 && inferences.Count > 0
                 && reasonerOptions.CurrentIteration < reasonerOptions.MaxAllowedIterations)
            {
                OWLEvents.RaiseInfo($"Merging inferences after Fact-phase iteration {reasonerOptions.CurrentIteration}...");

                // Fold this round's inferences into the ontology so the next round's rule execution can see them
                // as regular axioms (e.g. a freshly inferred ClassAssertion becoming usable by another Fact rule)
                MergeInferences(ontology, inferences);

                // Advance the iteration counter before recursing into the next round
                reasonerOptions.CurrentIteration++;

                // Recurse into the next round on the merged ontology; the whole chain of recursive calls
                // unwinds back into a single (Inferences, IterationsPerformed, ReachedFixpoint) result once it stops
                (List<OWLInference> Inferences, uint IterationsPerformed, bool ReachedFixpoint) nextIterationResult =
                    await IterateFactPhaseAsync(ontology, reasonerOptions, factRules);

                // Accumulate: the final Inferences list carries everything found across every round, not just this one
                inferences.AddRange(nextIterationResult.Inferences);

                //Propagate the deepest iteration's counters: they reflect where the recursion actually stopped
                iterationsPerformed = nextIterationResult.IterationsPerformed;
                reachedFixpoint = nextIterationResult.ReachedFixpoint;
            }
            else
            {
                //The loop stops here: either iterative reasoning is disabled, this iteration found nothing new
                //(genuine fixpoint), or the iteration cap was hit while inferences were still being produced
                reachedFixpoint = inferences.Count == 0;
            }

            return (inferences, iterationsPerformed, reachedFixpoint);
        }

        /// <summary>
        /// Executes a single round of the given OWL2 reasoner rules (a subset of either tier) against the ontology in
        /// its current state, optionally alongside the ontology's SWRL rules, and returns the deduplicated inferences
        /// discovered in that round (i.e. inferences not already asserted as axioms in the ontology). This is the unit
        /// of work shared by both the Schema phase's internal fixpoint loop and the Fact phase's iterative core: neither
        /// mutates the ontology itself here (that is the caller's responsibility, via <see cref="MergeInferences"/>),
        /// so this method is safe to call repeatedly without side effects beyond reading the ontology's current axioms.
        /// </summary>
        private static async Task<List<OWLInference>> ExecuteRulesAsync(OWLOntology ontology, List<OWLEnums.OWLReasonerRules> owl2Rules, bool includeSWRLRules)
        {
            List<OWLInference> inferences = new List<OWLInference>();

            #region Init registry & context
            // Initialize structures and counters for this round of reasoning (which may include SWRL)
            int swrlRuleCount = includeSWRLRules ? ontology.Rules.Count : 0;
            Dictionary<string, List<OWLInference>> inferenceRegistry = new Dictionary<string, List<OWLInference>>(owl2Rules.Count + swrlRuleCount);
            owl2Rules.ForEach(owl2Rule => inferenceRegistry.Add(owl2Rule.ToString(), null));
            if (includeSWRLRules)
                ontology.Rules.ForEach(swrlRule => inferenceRegistry.Add(swrlRule.ToString(), null));

            // Initialize axioms XML (required for inference deduplication)
            Task<HashSet<string>> clsAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> dtPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> opPropAsnAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> sameIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLSameIndividual>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> diffIdvsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetAssertionAxiomsOfType<OWLDifferentIndividuals>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> dsjClsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetClassAxiomsOfType<OWLDisjointClasses>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> eqvClsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetClassAxiomsOfType<OWLEquivalentClasses>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> subClsAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetClassAxiomsOfType<OWLSubClassOf>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> dsjDtPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLDisjointDataProperties>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> eqvDtPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLEquivalentDataProperties>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> subDtPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLSubDataPropertyOf>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> dsjOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLDisjointObjectProperties>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> eqvOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLEquivalentObjectProperties>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> subOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLSubObjectPropertyOf>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> invOpPropAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLInverseObjectProperties>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> opDomainAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyDomain>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> opRangeAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetObjectPropertyAxiomsOfType<OWLObjectPropertyRange>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> dtDomainAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyDomain>().Select(asn => asn.GetXML())));
            Task<HashSet<string>> dtRangeAxiomsTask = Task.Run(() => new HashSet<string>(ontology.GetDataPropertyAxiomsOfType<OWLDataPropertyRange>().Select(asn => asn.GetXML())));
            #endregion

            #region Process rules
            // Execute OWL2 reasoner rules (only the ones belonging to the tier/subset selected for this round)
            Parallel.ForEach(owl2Rules, rule =>
            {
                string ruleString = rule.ToString();
                OWLEvents.RaiseInfo($"Launching OWL2 rule {ruleString}...");

                switch (rule)
                {
                    // Schema tier (T-Box -> T-Box)
                    case OWLEnums.OWLReasonerRules.SchemaClassEntailment:
                        inferenceRegistry[OWLSchemaClassEntailment.rulename] = OWLSchemaClassEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaDisjointClassesEntailment:
                        inferenceRegistry[OWLSchemaDisjointClassesEntailment.rulename] = OWLSchemaDisjointClassesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaDisjointDataPropertiesEntailment:
                        inferenceRegistry[OWLSchemaDisjointDataPropertiesEntailment.rulename] = OWLSchemaDisjointDataPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaDisjointObjectPropertiesEntailment:
                        inferenceRegistry[OWLSchemaDisjointObjectPropertiesEntailment.rulename] = OWLSchemaDisjointObjectPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaEquivalentClassesEntailment:
                        inferenceRegistry[OWLSchemaEquivalentClassesEntailment.rulename] = OWLSchemaEquivalentClassesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaEquivalentDataPropertiesEntailment:
                        inferenceRegistry[OWLSchemaEquivalentDataPropertiesEntailment.rulename] = OWLSchemaEquivalentDataPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaEquivalentObjectPropertiesEntailment:
                        inferenceRegistry[OWLSchemaEquivalentObjectPropertiesEntailment.rulename] = OWLSchemaEquivalentObjectPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaHasValueEntailment:
                        inferenceRegistry[OWLSchemaHasValueEntailment.rulename] = OWLSchemaHasValueEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaObjectAllValuesFromEntailment:
                        inferenceRegistry[OWLSchemaObjectAllValuesFromEntailment.rulename] = OWLSchemaObjectAllValuesFromEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaObjectPropertyEntailment:
                        inferenceRegistry[OWLSchemaObjectPropertyEntailment.rulename] = OWLSchemaObjectPropertyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaObjectSomeValuesFromEntailment:
                        inferenceRegistry[OWLSchemaObjectSomeValuesFromEntailment.rulename] = OWLSchemaObjectSomeValuesFromEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaObjectUnionOfEntailment:
                        inferenceRegistry[OWLSchemaObjectUnionOfEntailment.rulename] = OWLSchemaObjectUnionOfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaPropertyDomainEntailment:
                        inferenceRegistry[OWLSchemaPropertyDomainEntailment.rulename] = OWLSchemaPropertyDomainEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaPropertyRangeEntailment:
                        inferenceRegistry[OWLSchemaPropertyRangeEntailment.rulename] = OWLSchemaPropertyRangeEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaDataPropertyEntailment:
                        inferenceRegistry[OWLSchemaDataPropertyEntailment.rulename] = OWLSchemaDataPropertyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaSubClassOfEntailment:
                        inferenceRegistry[OWLSchemaSubClassOfEntailment.rulename] = OWLSchemaSubClassOfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaSubDataPropertyOfEntailment:
                        inferenceRegistry[OWLSchemaSubDataPropertyOfEntailment.rulename] = OWLSchemaSubDataPropertyOfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.SchemaSubObjectPropertyOfEntailment:
                        inferenceRegistry[OWLSchemaSubObjectPropertyOfEntailment.rulename] = OWLSchemaSubObjectPropertyOfEntailment.ExecuteRule(ontology);
                        break;

                    // Fact tier (assertion/individual-level propagation)
                    case OWLEnums.OWLReasonerRules.FactClassAssertionEntailment:
                        inferenceRegistry[OWLFactClassAssertionEntailment.rulename] = OWLFactClassAssertionEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactDataPropertyDomainEntailment:
                        inferenceRegistry[OWLFactDataPropertyDomainEntailment.rulename] = OWLFactDataPropertyDomainEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactDifferentIndividualsEntailment:
                        inferenceRegistry[OWLFactDifferentIndividualsEntailment.rulename] = OWLFactDifferentIndividualsEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactEquivalentDataPropertiesEntailment:
                        inferenceRegistry[OWLFactEquivalentDataPropertiesEntailment.rulename] = OWLFactEquivalentDataPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactEquivalentObjectPropertiesEntailment:
                        inferenceRegistry[OWLFactEquivalentObjectPropertiesEntailment.rulename] = OWLFactEquivalentObjectPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactFunctionalObjectPropertyEntailment:
                        inferenceRegistry[OWLFactFunctionalObjectPropertyEntailment.rulename] = OWLFactFunctionalObjectPropertyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactHasKeyEntailment:
                        inferenceRegistry[OWLFactHasKeyEntailment.rulename] = OWLFactHasKeyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactHasSelfEntailment:
                        inferenceRegistry[OWLFactHasSelfEntailment.rulename] = OWLFactHasSelfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactHasValueEntailment:
                        inferenceRegistry[OWLFactHasValueEntailment.rulename] = OWLFactHasValueEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactInverseFunctionalObjectPropertyEntailment:
                        inferenceRegistry[OWLFactInverseFunctionalObjectPropertyEntailment.rulename] = OWLFactInverseFunctionalObjectPropertyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactInverseObjectPropertiesEntailment:
                        inferenceRegistry[OWLFactInverseObjectPropertiesEntailment.rulename] = OWLFactInverseObjectPropertiesEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectAllValuesFromEntailment:
                        inferenceRegistry[OWLFactObjectAllValuesFromEntailment.rulename] = OWLFactObjectAllValuesFromEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectMaxCardinalityEntailment:
                        inferenceRegistry[OWLFactObjectMaxCardinalityEntailment.rulename] = OWLFactObjectMaxCardinalityEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectOneOfEntailment:
                        inferenceRegistry[OWLFactObjectOneOfEntailment.rulename] = OWLFactObjectOneOfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectPropertyChainEntailment:
                        inferenceRegistry[OWLFactObjectPropertyChainEntailment.rulename] = OWLFactObjectPropertyChainEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectPropertyDomainEntailment:
                        inferenceRegistry[OWLFactObjectPropertyDomainEntailment.rulename] = OWLFactObjectPropertyDomainEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectPropertyRangeEntailment:
                        inferenceRegistry[OWLFactObjectPropertyRangeEntailment.rulename] = OWLFactObjectPropertyRangeEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactObjectSomeValuesFromEntailment:
                        inferenceRegistry[OWLFactObjectSomeValuesFromEntailment.rulename] = OWLFactObjectSomeValuesFromEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactReflexiveObjectPropertyEntailment:
                        inferenceRegistry[OWLFactReflexiveObjectPropertyEntailment.rulename] = OWLFactReflexiveObjectPropertyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactSameIndividualEntailment:
                        inferenceRegistry[OWLFactSameIndividualEntailment.rulename] = OWLFactSameIndividualEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactSubDataPropertyOfEntailment:
                        inferenceRegistry[OWLFactSubDataPropertyOfEntailment.rulename] = OWLFactSubDataPropertyOfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactSubObjectPropertyOfEntailment:
                        inferenceRegistry[OWLFactSubObjectPropertyOfEntailment.rulename] = OWLFactSubObjectPropertyOfEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactSymmetricObjectPropertyEntailment:
                        inferenceRegistry[OWLFactSymmetricObjectPropertyEntailment.rulename] = OWLFactSymmetricObjectPropertyEntailment.ExecuteRule(ontology);
                        break;
                    case OWLEnums.OWLReasonerRules.FactTransitiveObjectPropertyEntailment:
                        inferenceRegistry[OWLFactTransitiveObjectPropertyEntailment.rulename] = OWLFactTransitiveObjectPropertyEntailment.ExecuteRule(ontology);
                        break;
                }

                OWLEvents.RaiseInfo($"Completed OWL2 rule {ruleString} => {inferenceRegistry[ruleString].Count} candidate inferences");
            });

            // Execute SWRL reasoner rules (only when this round is a Fact-phase round: SWRL reasons over individuals,
            // so it is always Fact-tier and must never run as part of the Schema phase's closure loop)
            if (includeSWRLRules)
            {
#if !NET8_0_OR_GREATER
                await ontology.Rules.ParallelForEachAsync(async (swrlRule, _) =>
#else
                await Parallel.ForEachAsync(ontology.Rules, async (swrlRule, _) =>
#endif
                {
                    string swrlRuleString = swrlRule.ToString();
                    OWLEvents.RaiseInfo($"Launching SWRL rule {swrlRuleString}...");

                    inferenceRegistry[swrlRuleString] = await swrlRule.ApplyToOntologyAsync(ontology);

                    OWLEvents.RaiseInfo($"Completed SWRL rule {swrlRuleString} => {inferenceRegistry[swrlRuleString].Count} candidate inferences");
                });
            }
            #endregion

            #region Deduplicate & finalize inferences
            // Deduplicate inferences (in order to not state already known knowledge)
            await Task.WhenAll(clsAsnAxiomsTask, dtPropAsnAxiomsTask, opPropAsnAxiomsTask, sameIdvsAxiomsTask, diffIdvsAxiomsTask, dsjClsAxiomsTask, eqvClsAxiomsTask, subClsAxiomsTask,
                dsjDtPropAxiomsTask, eqvDtPropAxiomsTask, subDtPropAxiomsTask, dsjOpPropAxiomsTask, eqvOpPropAxiomsTask, subOpPropAxiomsTask, invOpPropAxiomsTask, opDomainAxiomsTask, opRangeAxiomsTask,
                dtDomainAxiomsTask, dtRangeAxiomsTask);
            foreach (KeyValuePair<string, List<OWLInference>> inferenceRegistryEntry in inferenceRegistry.Where(ir => ir.Value?.Count > 0))
                inferenceRegistryEntry.Value.RemoveAll(inf =>
                {
                    switch (inf.Axiom.GetType().Name)
                    {
                        case nameof(OWLClassAssertion):
                            return clsAsnAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDataPropertyAssertion):
                            return dtPropAsnAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLObjectPropertyAssertion):
                            return opPropAsnAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLSameIndividual):
                            return sameIdvsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDifferentIndividuals):
                            return diffIdvsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDisjointClasses):
                            return dsjClsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLEquivalentClasses):
                            return eqvClsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLSubClassOf):
                            return subClsAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDisjointDataProperties):
                            return dsjDtPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLEquivalentDataProperties):
                            return eqvDtPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLSubDataPropertyOf):
                            return subDtPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDisjointObjectProperties):
                            return dsjOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLEquivalentObjectProperties):
                            return eqvOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLSubObjectPropertyOf):
                            return subOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLInverseObjectProperties):
                            return invOpPropAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLObjectPropertyDomain):
                            return opDomainAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLObjectPropertyRange):
                            return opRangeAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDataPropertyDomain):
                            return dtDomainAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        case nameof(OWLDataPropertyRange):
                            return dtRangeAxiomsTask.Result.Contains(inf.Axiom.GetXML());
                        // Not explicitly handled inference type: just keep it
                        default: return true;
                    }
                });

            // Collect inferences
            IEnumerable<OWLInference> emptyInferenceSet = Enumerable.Empty<OWLInference>();
            inferences.AddRange(inferenceRegistry.SelectMany(ir => ir.Value ?? emptyInferenceSet).Distinct());
            #endregion

            return inferences;
        }

        /// <summary>
        /// Merges the given round's inferences back into the ontology's axiom lists (deduplicating per axiom type),
        /// so that the next round (whether the next Schema-closure round or the next Fact-phase iteration) reasons
        /// over the enriched ontology state. Shared by both phases since the merge logic itself does not depend on
        /// which tier produced the inferences -- only the axiom's own base type matters.
        /// </summary>
        private static void MergeInferences(OWLOntology ontology, List<OWLInference> inferences)
        {
            foreach (IGrouping<Type,OWLInference> inferenceGroupType in inferences.GroupBy(inf => inf.Axiom.GetType()))
            {
                switch (inferenceGroupType.Key.BaseType?.Name)
                {
                    case nameof(OWLAssertionAxiom):
                        ontology.AssertionAxioms.AddRange(inferenceGroupType.Select(g => (OWLAssertionAxiom)g.Axiom));
                        ontology.AssertionAxioms = OWLAxiomHelper.RemoveDuplicates(ontology.AssertionAxioms);
                        break;
                    case nameof(OWLClassAxiom):
                        ontology.ClassAxioms.AddRange(inferenceGroupType.Select(g => (OWLClassAxiom)g.Axiom));
                        ontology.ClassAxioms = OWLAxiomHelper.RemoveDuplicates(ontology.ClassAxioms);
                        break;
                    case nameof(OWLDataPropertyAxiom):
                        ontology.DataPropertyAxioms.AddRange(inferenceGroupType.Select(g => (OWLDataPropertyAxiom)g.Axiom));
                        ontology.DataPropertyAxioms = OWLAxiomHelper.RemoveDuplicates(ontology.DataPropertyAxioms);
                        break;
                    case nameof(OWLObjectPropertyAxiom):
                        ontology.ObjectPropertyAxioms.AddRange(inferenceGroupType.Select(g => (OWLObjectPropertyAxiom)g.Axiom));
                        ontology.ObjectPropertyAxioms = OWLAxiomHelper.RemoveDuplicates(ontology.ObjectPropertyAxioms);
                        break;
                    case nameof(OWLAnnotationAxiom):
                        ontology.AnnotationAxioms.AddRange(inferenceGroupType.Select(g => (OWLAnnotationAxiom)g.Axiom));
                        ontology.AnnotationAxioms = OWLAxiomHelper.RemoveDuplicates(ontology.AnnotationAxioms);
                        break;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// OWLReasonerOptions permits fine-tuning of the behavior of an OWLReasoner.<br/>
    /// Default configuration enables iterative reasoning with 3 maximum allowed iterations.
    /// </summary>
    public sealed class OWLReasonerOptions
    {
        #region Properties
        /// <summary>
        /// Enables the reasoner to iterate until no new inferences are discovered, or until the maximum number of iterations has been reached (default: true).<br/>
        /// Acts as a global switch for BOTH phases: when disabled, the Schema phase also stops after a single round
        /// (rather than looping to its own internal fixpoint), mirroring the pre-two-phase-refactor "single pass" behavior.
        /// </summary>
        public bool EnableIterativeReasoning { get; set; } = true;

        /// <summary>
        /// Allows the reasoner's Fact phase to execute at most this number of iterations (default: 3).<br/>
        /// This cap never applies to the Schema phase, which always closes to a real fixpoint by construction.
        /// </summary>
        public uint MaxAllowedIterations { get; set; } = 3;

        /// <summary>
        /// Opt-in switch (default: <b>false</b>): asks the reasoner to verify OWL2-RL compliance
        /// (<see cref="OWLProfiler.CheckProfileAsync"/>) and, only if compliant and <see cref="MaxAllowedIterations"/>
        /// was left at its default, run the Fact phase uncapped to its real fixpoint (RL/RDF closure is theoretically
        /// guaranteed to terminate in a finite number of rounds). Disabled by default: the compliance check has a
        /// real cost, and no ontology should be assumed RL for a caller who didn't ask.<br/>
        /// If the ontology turns out not RL-compliant, or <see cref="MaxAllowedIterations"/> was customized, the
        /// configured cap applies as-is and there is no guarantee that the reported fixpoint is the real one.
        /// </summary>
        public bool ForceRLFixpointConvergence { get; set; }

        /// <summary>
        /// Indicates the number of the current Fact-phase iteration (default: 1)
        /// </summary>
        internal uint CurrentIteration { get; set; } = 1;
        #endregion
    }
}