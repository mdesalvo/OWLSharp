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
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLRule represents a DL-safe rule expressed in SWRL
    /// </summary>
    [XmlRoot("DLSafeRule")]
    public sealed class SWRLRule
    {
        #region Properties
        /// <summary>
        /// The set of annotations about this SWRL rule (e.g: comment, label, author)
        /// </summary>
        [XmlElement("Annotation")]
        public List<OWLAnnotation> Annotations { get; set; }

        /// <summary>
        /// The antecedent of this SWRL rule (also called "Body")
        /// </summary>
        [XmlElement("Body")]
        public SWRLAntecedent Antecedent { get; set; }

        /// <summary>
        /// The consequent of this SWRL rule (also called "Head")
        /// </summary>
        [XmlElement("Head")]
        public SWRLConsequent Consequent { get; set; }

        /// <summary>
        /// Indicates that this SWRL rule has been imported from another ontology
        /// </summary>
        [XmlIgnore]
        public bool IsImport { get; set; }
        #endregion

        #region Ctors
        internal SWRLRule()
            => Annotations = new List<OWLAnnotation>();

        /// <summary>
        /// Builds a SWRL rule with the given name, description, antecedent and consequent
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public SWRLRule(RDFLiteral ruleName, RDFLiteral ruleDescription, SWRLAntecedent antecedent, SWRLConsequent consequent) : this()
        {
            #region Guards
            if (ruleName == null)
                throw new SWRLException($"Cannot create SWRL rule because given '{nameof(ruleName)}' parameter is null");
            if (ruleDescription == null)
                throw new SWRLException($"Cannot create SWRL rule because given '{nameof(ruleDescription)}' parameter is null");
            #endregion

            Annotations.Add(
                new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.LABEL),
                    new OWLLiteral(ruleName)));
            Annotations.Add(
                new OWLAnnotation(
                    new OWLAnnotationProperty(RDFVocabulary.RDFS.COMMENT),
                    new OWLLiteral(ruleDescription)));
            Antecedent = antecedent ?? throw new SWRLException($"Cannot create SWRL rule because given '{nameof(antecedent)}' parameter is null");
            Consequent = consequent ?? throw new SWRLException($"Cannot create SWRL rule because given '{nameof(consequent)}' is null");
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gets the string representation of this SWRL rule (antecedent -> consequent)
        /// </summary>
        public override string ToString()
            => $"{Antecedent} -> {Consequent}";
        #endregion

        #region Methods
        /// <summary>
        /// Gets the OWL2/XML representation of this SWRL rule
        /// </summary>
        public string GetXML()
            => OWLSerializer.SerializeObject(this);

        /// <summary>
        /// Exports this SWRL rule to an equivalent RDFGraph object
        /// </summary>
        public RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            RDFResource ruleBN = new RDFResource();
            graph.AddTriple(new RDFTriple(ruleBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.IMP));

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraphInternal(ruleBN));

            //Antecedent
            graph = graph.UnionWith(Antecedent?.ToRDFGraph(ruleBN));

            //Consequent
            graph = graph.UnionWith(Consequent?.ToRDFGraph(ruleBN));

            return graph;
        }
        #endregion

        #region Utilities
        internal Task<List<OWLInference>> ApplyToOntologyAsync(OWLOntology ontology)
            => Task.Run(() => Consequent?.Evaluate(Antecedent?.Evaluate(ontology), ontology)
                                       ?? Enumerable.Empty<OWLInference>().ToList());

        internal static RDFGraph ReifySWRLCollection(RDFCollection collection, bool isAtomList)
        {
            RDFGraph reifColl = new RDFGraph();
            RDFResource reifSubj = collection.ReificationSubject;
            int itemCount = 0;

            //Conceptually equivalent to the reification of RDF lists, except for supporting
            //the emission of an additional "rdf:type swrl:AtomList" triple for each element
            foreach (RDFPatternMember collectionItem in collection)
            {
                itemCount++;

                reifColl.AddTriple(new RDFTriple(reifSubj, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
                if (isAtomList)
                    reifColl.AddTriple(new RDFTriple(reifSubj, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.ATOMLIST));

                if (collectionItem is RDFResource collectionItemResource)
                    reifColl.AddTriple(new RDFTriple(reifSubj, RDFVocabulary.RDF.FIRST, collectionItemResource));
                else
                    reifColl.AddTriple(new RDFTriple(reifSubj, RDFVocabulary.RDF.FIRST, (RDFLiteral)collectionItem));

                if (itemCount < collection.ItemsCount)
                {
                    RDFResource newSub = new RDFResource();
                    reifColl.AddTriple(new RDFTriple(reifSubj, RDFVocabulary.RDF.REST, newSub));
                    reifSubj = newSub;
                }
                else
                {
                    reifColl.AddTriple(new RDFTriple(reifSubj, RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
                }
            }

            return reifColl;
        }
        #endregion
    }
}