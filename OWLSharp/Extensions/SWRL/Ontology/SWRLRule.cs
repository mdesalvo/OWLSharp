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

using System.Data;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// SWRLRule represents a reasoner rule expressed in SWRL
    /// </summary>
    public class SWRLRule
    {
        #region Properties
        /// <summary>
        /// Name of the reasoner rule
        /// </summary>
        public string RuleName { get; internal set; }

        /// <summary>
        /// Description of the reasoner rule
        /// </summary>
        public string RuleDescription { get; internal set; }

        /// <summary>
        /// Antecedent of the reasoner rule
        /// </summary>
        public SWRLAntecedent Antecedent { get; internal set; }

        /// <summary>
        /// Consequent of the reasoner rule
        /// </summary>
        public SWRLConsequent Consequent { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a rule with given antecedent and consequent
        /// </summary>
        public SWRLRule(string ruleName, string ruleDescription, SWRLAntecedent antecedent, SWRLConsequent consequent)
        {
            #region Guards
            if (string.IsNullOrEmpty(ruleName))
                throw new OWLException("Cannot create rule because given \"ruleName\" parameter is null or empty");
            #endregion

            RuleName = ruleName;
            RuleDescription = ruleDescription;
            Antecedent = antecedent ?? throw new OWLException("Cannot create rule because given \"antecedent\" parameter is null");
            Consequent = consequent ?? throw new OWLException("Cannot create rule because given \"consequent\" parameter is null");
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the rule
        /// </summary>
        public override string ToString()
            => string.Concat(Antecedent, " -> ", Consequent);
        #endregion

        #region Methods
        /// <summary>
        /// Applies the rule to the given ontology
        /// </summary>
        public OWLReasonerReport ApplyToOntology(OWLOntology ontology)
        {
            //Materialize results of the rule's antecedent
            DataTable antecedentResults = Antecedent.Evaluate(ontology);

            //Materialize results of the rule's consequent
            OWLReasonerReport consequentResults = Consequent.Evaluate(antecedentResults, ontology);
            return consequentResults;
        }

        /// <summary>
        /// Asynchronously applies the rule to the given ontology
        /// </summary>
        public Task<OWLReasonerReport> ApplyToOntologyAsync(OWLOntology ontology)
            => Task.Run(() => ApplyToOntology(ontology));
        #endregion
    }
}