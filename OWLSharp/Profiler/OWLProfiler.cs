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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWLSharp.Ontology;

namespace OWLSharp.Profiler
{
    /// <summary>
    /// OWLProfiler checks an ontology's T-BOX and R-BOX against the syntactic grammars of the OWL2 profiles
    /// (EL, QL, RL) defined at https://www.w3.org/TR/owl2-profiles/, reporting the constructs that fall outside
    /// a given profile's restricted expressivity. Unlike OWLReasoner and OWLValidator, profiling performs no
    /// semantic reasoning: it is a purely syntactic classification of the ontology's axioms and expressions.
    /// </summary>
    public static class OWLProfiler
    {
        #region Methods
        /// <summary>
        /// Checks the given ontology against the given OWL2 profile
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static async Task<OWLProfileReport> CheckProfileAsync(this OWLOntology ontology, OWLEnums.OWLProfiles profile)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException($"Cannot check profile because given '{nameof(ontology)}' parameter is null");
            #endregion

            OWLEvents.RaiseInfo($"Launching profiler on ontology '{ontology.IRI}' for profile {profile}...");

            //If Violations stays empty, the ontology is compliant with the profile's grammar (OWLProfileReport.IsCompliant).
            OWLProfileReport report = new OWLProfileReport(profile)
            {
                Violations = await ExecuteProfileRuleAsync(profile, ontology)
            };

            OWLEvents.RaiseInfo($"Completed profiler on ontology '{ontology.IRI}' for profile {profile} => {report.Violations.Count} violations");

            return report;
        }

        /// <summary>
        /// Checks the given ontology against all the OWL2 profiles, sharing a single walk of its axioms
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public static async Task<Dictionary<OWLEnums.OWLProfiles, OWLProfileReport>> CheckProfilesAsync(this OWLOntology ontology)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException($"Cannot check profiles because given '{nameof(ontology)}' parameter is null");
            #endregion

            //Iterate every enum value of OWLEnums.OWLProfiles (currently EL, QL, RL) instead of hardcoding
            //the three cases, so a future profile addition to the enum is picked up here automatically.
            Dictionary<OWLEnums.OWLProfiles, OWLProfileReport> reports = new Dictionary<OWLEnums.OWLProfiles, OWLProfileReport>();
            foreach (OWLEnums.OWLProfiles profile in (OWLEnums.OWLProfiles[])Enum.GetValues(typeof(OWLEnums.OWLProfiles)))
                reports[profile] = await ontology.CheckProfileAsync(profile);

            return reports;
        }

        //Dispatches to the RuleSet class implementing the requested profile's grammar, returning its Task
        //directly (no Task.FromResult wrapping here) so CheckProfileAsync's "await" above is a real await
        //all the way down to OWL{EL,QL,RL}Profile.ExecuteRuleAsync, not just a cosmetic one at the boundary.
        //Mirrors the switch-based dispatch used by OWLValidator/OWLReasoner, but keyed on a single
        //profile value per call (CheckProfileAsync) instead of a Parallel.ForEach over a rule list,
        //since here there is exactly one grammar to check per call, not many independent rules.
        private static Task<List<OWLProfileViolation>> ExecuteProfileRuleAsync(OWLEnums.OWLProfiles profile, OWLOntology ontology)
        {
            switch (profile)
            {
                case OWLEnums.OWLProfiles.EL:
                    return OWLELProfile.ExecuteRuleAsync(ontology);
                case OWLEnums.OWLProfiles.QL:
                    return OWLQLProfile.ExecuteRuleAsync(ontology);
                case OWLEnums.OWLProfiles.RL:
                    return OWLRLProfile.ExecuteRuleAsync(ontology);
                default:
                    return Task.FromResult(new List<OWLProfileViolation>());
            }
        }
        #endregion
    }
}