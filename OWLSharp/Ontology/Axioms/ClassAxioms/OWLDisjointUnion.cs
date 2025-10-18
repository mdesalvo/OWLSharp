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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLDisjointUnion axiom defines a class as the complete, non-overlapping union of two or more subclasses,
    /// asserting both that the subclasses are pairwise disjoint and that their union exactly covers the parent class.
    /// For example, DisjointUnion(Person Male Female) states that every Person is either Male or Female (but not both),
    /// providing both an exhaustive partition and mutual exclusivity, enabling reasoners to infer class membership
    /// and detect inconsistencies in classification.
    /// </summary>
    [XmlRoot("DisjointUnion")]
    public sealed class OWLDisjointUnion : OWLClassAxiom
    {
        #region Properties
        /// <summary>
        /// The class representing the union of the pairwise disjoint class expressions (e.g: http://example.org/Person)
        /// </summary>
        [XmlElement("Class", Order=2)]
        public OWLClass ClassIRI { get; set; }

        /// <summary>
        /// The set of class expressions asserted to be pairwise disjoint (e.g: http://example.org/Male, http://example.org/Female)
        /// </summary>
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=3)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=3)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=3)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=3)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=3)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=3)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=3)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=3)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=3)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=3)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=3)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=3)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=3)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=3)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=3)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=3)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=3)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=3)]
        public List<OWLClassExpression> ClassExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointUnion() { }

        /// <summary>
        /// Builds an OWLDisjointUnion with the given class and set of pairwise disjoint class expressions (must be at least 2)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLDisjointUnion(OWLClass classIRI, List<OWLClassExpression> classExpressions) : this()
        {
            #region Guards
            if (classExpressions == null)
                throw new OWLException($"Cannot create OWLDisjointUnion because given '{nameof(classExpressions)}' parameter is null");
            if (classExpressions.Count < 2)
                throw new OWLException($"Cannot create OWLDisjointUnion because given '{nameof(classExpressions)}' parameter must contain at least 2 elements");
            if (classExpressions.Any(cex => cex == null))
                throw new OWLException($"Cannot create OWLDisjointUnion because given '{nameof(classExpressions)}' parameter contains a null element");
            #endregion

            ClassIRI = classIRI ?? throw new OWLException($"Cannot create OWLDisjointUnion because given '{nameof(classIRI)}' parameter is null");
            ClassExpressions = classExpressions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLDisjointUnion to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            RDFResource classIRI = ClassIRI.GetIRI();
            graph = graph.UnionWith(ClassIRI.ToRDFGraph(classIRI));

            RDFCollection disjointUnionCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (OWLClassExpression classExpression in ClassExpressions)
            {
                RDFResource classExpressionIRI = classExpression.GetIRI();
                disjointUnionCollection.AddItem(classExpressionIRI);
                graph = graph.UnionWith(classExpression.ToRDFGraph(classExpressionIRI));
            }
            graph.AddCollection(disjointUnionCollection);

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(classIRI, RDFVocabulary.OWL.DISJOINT_UNION_OF, disjointUnionCollection.ReificationSubject);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}