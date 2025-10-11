/*
   Copyright 2014-2025 Marco De Salvo

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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLAntecedent is a conjunction of atoms that specifies the conditions that must be satisfied for the rule to fire.
    /// When all atoms in the body are true for a particular variable binding, the rule is triggered and the consequent (head) is entailed,
    /// allowing you to express conditional inference patterns like "IF conditions in body THEN conclusions in head".
    /// </summary>
    [XmlRoot("Body")]
    public sealed class SWRLAntecedent
    {
        #region Properties
        /// <summary>
        /// The set of atoms responsible for analyzing the ontology knowledge
        /// </summary>
        [XmlElement(typeof(SWRLAnnotationPropertyAtom), ElementName="AnnotationPropertyAtom")]
        [XmlElement(typeof(SWRLClassAtom), ElementName="ClassAtom")]
        [XmlElement(typeof(SWRLDataPropertyAtom), ElementName="DataPropertyAtom")]
        [XmlElement(typeof(SWRLDataRangeAtom), ElementName="DataRangeAtom")]
        [XmlElement(typeof(SWRLDifferentIndividualsAtom), ElementName="DifferentIndividualsAtom")]
        [XmlElement(typeof(SWRLObjectPropertyAtom), ElementName="ObjectPropertyAtom")]
        [XmlElement(typeof(SWRLSameIndividualAtom), ElementName="SameIndividualAtom")]
        public List<SWRLAtom> Atoms { get; set; } = new List<SWRLAtom>();

        /// <summary>
        /// The set of built-ins responsible for filtering the ontology knowledge
        /// </summary>
        [XmlElement(ElementName="BuiltInAtom")]
        public List<SWRLBuiltIn> BuiltIns { get; set; } = new List<SWRLBuiltIn>();
        #endregion

        #region Interfaces
        /// <summary>
        /// Gets the string representation of this SWRL antecedent (atom1 ^ ...atomN ^ builtin1 ^ ...builtinN)
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join(" ^ ", Atoms))
              .Append(Atoms.Count > 0 && BuiltIns.Count > 0 ? " ^ " : string.Empty)
              .Append(string.Join(" ^ ", BuiltIns));
            return sb.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Analyzes and filters the ontology knowledge by sequentially applying the set of atoms and built-ins.<br/>
        /// The result of the process is a tabular binding of variables ready for evaluation by the consequent.
        /// </summary>
        internal DataTable Evaluate(OWLOntology ontology)
        {
            //Execute the antecedent atoms
            List<DataTable> atomResults = new List<DataTable>(Atoms.Count);
            Atoms.ForEach(atom => atomResults.Add(atom.EvaluateOnAntecedent(ontology)));

            //Join results of antecedent atoms
            DataTable antecedentResult = RDFQueryEngine.CombineTables(atomResults);

            //Execute the antecedent built-ins
            BuiltIns.ForEach(builtin => antecedentResult = builtin.EvaluateOnAntecedent(antecedentResult));

            //Return the antecedent result
            return antecedentResult;
        }

        /// <summary>
        /// Exports this SWRL antecedent to an equivalent RDFGraph object
        /// </summary>
        internal RDFGraph ToRDFGraph(RDFResource ruleBN)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource antecedentBN = new RDFResource();
            graph.AddTriple(new RDFTriple(ruleBN, RDFVocabulary.SWRL.BODY, antecedentBN));

            RDFCollection antecedentElements = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource) { InternalReificationSubject = antecedentBN };
            foreach (SWRLAtom atom in Atoms)
                graph = graph.UnionWith(atom.ToRDFGraph(antecedentElements));
            foreach (SWRLBuiltIn builtIn in BuiltIns)
                graph = graph.UnionWith(builtIn.ToRDFGraph(antecedentElements));
            return graph.UnionWith(SWRLRule.ReifySWRLCollection(antecedentElements, true));
        }
        #endregion
    }
}