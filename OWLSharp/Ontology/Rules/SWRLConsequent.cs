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

using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using OWLSharp.Reasoner;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    [XmlRoot("Head")]
    public sealed class SWRLConsequent
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
        #endregion

        #region Interfaces
        public override string ToString()
            => string.Join(" ^ ", Atoms);
        #endregion

        #region Methods
        internal List<OWLInference> Evaluate(DataTable antecedentResults, OWLOntology ontology)
        {
            List<OWLInference>  inferences = new List<OWLInference> ();

            //Execute the consequent atoms
            if (antecedentResults != null)
                Atoms.ForEach(atom => inferences.AddRange(atom.EvaluateOnConsequent(antecedentResults, ontology)));

            //Return the consequent result
            return inferences;
        }

        internal RDFGraph ToRDFGraph(RDFResource ruleBN)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource consequentBN = new RDFResource();
            graph.AddTriple(new RDFTriple(ruleBN, RDFVocabulary.SWRL.HEAD, consequentBN));

            RDFCollection consequentElements = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource) { InternalReificationSubject = consequentBN };
            foreach (SWRLAtom atom in Atoms)
                graph = graph.UnionWith(atom.ToRDFGraph(consequentElements));
            return graph.UnionWith(SWRLRule.ReifySWRLCollection(consequentElements, true));
        }
        #endregion
    }
}