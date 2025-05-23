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
    [XmlRoot("Body")]
    public sealed class SWRLAntecedent
    {
        #region Properties
        [XmlElement(typeof(SWRLAnnotationPropertyAtom), ElementName="AnnotationPropertyAtom")]
        [XmlElement(typeof(SWRLClassAtom), ElementName="ClassAtom")]
        [XmlElement(typeof(SWRLDataPropertyAtom), ElementName="DataPropertyAtom")]
        [XmlElement(typeof(SWRLDataRangeAtom), ElementName="DataRangeAtom")]
        [XmlElement(typeof(SWRLDifferentIndividualsAtom), ElementName="DifferentIndividualsAtom")]
        [XmlElement(typeof(SWRLObjectPropertyAtom), ElementName="ObjectPropertyAtom")]
        [XmlElement(typeof(SWRLSameIndividualAtom), ElementName="SameIndividualAtom")]
        public List<SWRLAtom> Atoms { get; set; } = new List<SWRLAtom>();

        [XmlElement(ElementName="BuiltInAtom")]
        public List<SWRLBuiltIn> BuiltIns { get; set; } = new List<SWRLBuiltIn>();
        #endregion

        #region Interfaces
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join(" ^ ", Atoms));
            sb.Append(Atoms.Count > 0 && BuiltIns.Count > 0 ? " ^ " : string.Empty);
            sb.Append(string.Join(" ^ ", BuiltIns));
            return sb.ToString();
        }
        #endregion

        #region Methods
        internal DataTable Evaluate(OWLOntology ontology)
        {
            //Execute the antecedent atoms
            List<DataTable> atomResults = new List<DataTable>();
            Atoms.ForEach(atom => atomResults.Add(atom.EvaluateOnAntecedent(ontology)));

            //Join results of antecedent atoms
            DataTable antecedentResult = RDFQueryEngine.CombineTables(atomResults);

            //Execute the antecedent built-ins
            BuiltIns.ForEach(builtin => antecedentResult = builtin.EvaluateOnAntecedent(antecedentResult));

            //Return the antecedent result
            return antecedentResult;
        }

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