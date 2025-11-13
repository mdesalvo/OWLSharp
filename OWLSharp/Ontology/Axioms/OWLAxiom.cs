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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAxiom is a fundamental statement or assertion that expresses a piece of knowledge about the domain,
    /// defining the properties, relationships, and constraints of entities within the ontology.
    /// Axioms are the basic building blocks of ontological knowledge and include declarations, class axioms
    /// (like subclass relationships or equivalences), property axioms (such as domain, range, or characteristics),
    /// individual assertions (facts about specific instances), and annotations, collectively forming the logical
    /// content from which reasoners derive inferences.
    /// </summary>
    [XmlInclude(typeof(OWLAnnotationAssertion))]
    [XmlInclude(typeof(OWLAnnotationPropertyDomain))]
    [XmlInclude(typeof(OWLAnnotationPropertyRange))]
    [XmlInclude(typeof(OWLAsymmetricObjectProperty))]
    [XmlInclude(typeof(OWLClassAssertion))]
    [XmlInclude(typeof(OWLDataPropertyAssertion))]
    [XmlInclude(typeof(OWLDataPropertyDomain))]
    [XmlInclude(typeof(OWLDataPropertyRange))]
    [XmlInclude(typeof(OWLDatatypeDefinition))]
    [XmlInclude(typeof(OWLDeclaration))]
    [XmlInclude(typeof(OWLDifferentIndividuals))]
    [XmlInclude(typeof(OWLDisjointClasses))]
    [XmlInclude(typeof(OWLDisjointDataProperties))]
    [XmlInclude(typeof(OWLDisjointObjectProperties))]
    [XmlInclude(typeof(OWLDisjointUnion))]
    [XmlInclude(typeof(OWLEquivalentClasses))]
    [XmlInclude(typeof(OWLEquivalentDataProperties))]
    [XmlInclude(typeof(OWLEquivalentObjectProperties))]
    [XmlInclude(typeof(OWLFunctionalDataProperty))]
    [XmlInclude(typeof(OWLFunctionalObjectProperty))]
    [XmlInclude(typeof(OWLHasKey))]
    [XmlInclude(typeof(OWLInverseFunctionalObjectProperty))]
    [XmlInclude(typeof(OWLInverseObjectProperties))]
    [XmlInclude(typeof(OWLIrreflexiveObjectProperty))]
    [XmlInclude(typeof(OWLNegativeDataPropertyAssertion))]
    [XmlInclude(typeof(OWLNegativeObjectPropertyAssertion))]
    [XmlInclude(typeof(OWLObjectPropertyAssertion))]
    [XmlInclude(typeof(OWLObjectPropertyDomain))]
    [XmlInclude(typeof(OWLObjectPropertyRange))]
    [XmlInclude(typeof(OWLReflexiveObjectProperty))]
    [XmlInclude(typeof(OWLSameIndividual))]
    [XmlInclude(typeof(OWLSubAnnotationPropertyOf))]
    [XmlInclude(typeof(OWLSubClassOf))]
    [XmlInclude(typeof(OWLSubDataPropertyOf))]
    [XmlInclude(typeof(OWLSubObjectPropertyOf))]
    [XmlInclude(typeof(OWLSymmetricObjectProperty))]
    [XmlInclude(typeof(OWLTransitiveObjectProperty))]
    public class OWLAxiom
    {
        #region Properties
        /// <summary>
        /// The set of annotations about this axiom (e.g: comment, label, author)
        /// </summary>
        [XmlElement("Annotation", Order=1)]
        public List<OWLAnnotation> Annotations { get; set; }

        /// <summary>
        /// Indicates that this axiom has been emitted by a reasoner (so that it represents inferred knowledge)
        /// </summary>
        [XmlIgnore]
        public bool IsInference { get; set; }

        /// <summary>
        /// Indicates that this axiom has been imported from another ontology (so that it represents imported knowledge)
        /// </summary>
        [XmlIgnore]
        public bool IsImport { get; set; }

        /// <summary>
        /// The OWL2/XML representation of this axiom
        /// </summary>
        [XmlIgnore]
        internal string AxiomXML { get; set; }
        #endregion

        #region Ctors
        internal OWLAxiom()
            => Annotations = new List<OWLAnnotation>();
        #endregion

        #region Methods
        /// <summary>
        /// Gets the OWL2/XML representation of this axiom
        /// </summary>
        public string GetXML()
            => AxiomXML ?? (AxiomXML = OWLSerializer.SerializeObject(this));

        /// <summary>
        /// Exports this axiom to an equivalent RDFGraph object
        /// </summary>
        public virtual RDFGraph ToRDFGraph()
            => new RDFGraph();

        /// <summary>
        /// Adds the given annotation to the set of this axiom's annotations
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public void Annotate(OWLAnnotation annotation)
            => Annotations.Add(annotation ?? throw new OWLException($"Cannot annotate axiom because given '{nameof(annotation)}' parameter is null"));
        #endregion
    }

    /// <summary>
    /// OWLClassAxiom represents a statement that defines or constrains the characteristics of a class.
    /// It establishes logical relationships such as subclass hierarchies, class equivalences,
    /// or disjointness constraints, thereby shaping the semantic definition and reasoning rules applicable
    /// to that class within an ontology.
    /// </summary>
    public class OWLClassAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLClassAxiom() { }
        #endregion
    }

    /// <summary>
    /// OWLObjectPropertyAxiom represents a statement that defines or constrains the characteristics of an object property
    /// (a relationship between individuals). It establishes logical relationships such as property hierarchies, inverses,
    /// domains, ranges, or functional/transitive constraints, thereby governing how that property can be used and reasoned
    /// about within an ontology.
    /// </summary>
    public class OWLObjectPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLObjectPropertyAxiom() { }
        #endregion
    }

    /// <summary>
    /// OWLDataPropertyAxiom represents a statement that defines or constrains the characteristics of a data property
    /// (a relationship between an individual and a literal value). It establishes logical relationships such as property hierarchies,
    /// domains, ranges, or functional constraints, thereby governing how that property associates individuals with
    /// typed data values within an ontology.
    /// </summary>
    public class OWLDataPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLDataPropertyAxiom() { }
        #endregion
    }

    /// <summary>
    /// OWLAssertionAxiom represents a statement that asserts facts about individuals in the ontology.
    /// It includes positive assertions (like class membership or property relationships between specific individuals)
    /// and negative assertions (like disjointness between individuals), thereby populating the ontology with
    /// concrete instance-level data that can be reasoned over.
    /// </summary>
    public class OWLAssertionAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAssertionAxiom() { }
        #endregion
    }

    /// <summary>
    /// OWLAnnotationAxiom represents a statement that attaches metadata or descriptive information to ontology elements
    /// (classes, properties, individuals, or other axioms). It includes annotations like labels, comments, documentation,
    /// or custom metadata, thereby providing human-readable and machine-processable auxiliary information without affecting
    /// the logical semantics of the ontology.
    /// </summary>
    public class OWLAnnotationAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAnnotationAxiom() { }
        #endregion
    }
}