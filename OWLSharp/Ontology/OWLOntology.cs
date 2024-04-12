/*
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

using OWLSharp.Ontology.Axioms;
using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("Ontology")]
    public class OWLOntology
    {
        #region Properties
        [XmlAttribute("ontologyIRI", DataType="anyURI")]
        public string IRI { get; set; }

        [XmlAttribute("ontologyVersion", DataType="anyURI")]
        public string Version { get; set; }

        [XmlElement("Prefix")]
        public List<OWLPrefix> Prefixes { get; internal set; }

        [XmlElement("Import")]
        public List<OWLImport> Imports { get; internal set; }

        [XmlElement("Annotation")]
        public List<OWLAnnotation> Annotations { get; internal set; }

        //Axioms

        [XmlElement("Declaration")]
        public List<OWLDeclaration> DeclarationAxioms { get; internal set; }

        [XmlElement(typeof(OWLSubClassOf), ElementName="SubClassOf")]
        [XmlElement(typeof(OWLEquivalentClasses), ElementName="EquivalentClasses")]
        [XmlElement(typeof(OWLDisjointClasses), ElementName="DisjointClasses")]
        [XmlElement(typeof(OWLDisjointUnion), ElementName="DisjointUnion")]
        public List<OWLClassAxiom> ClassAxioms { get; internal set; }

        [XmlElement(typeof(OWLSubObjectPropertyOf), ElementName="SubObjectPropertyOf")]
        [XmlElement(typeof(OWLEquivalentObjectProperties), ElementName="EquivalentObjectProperties")]
        [XmlElement(typeof(OWLDisjointObjectProperties), ElementName="DisjointObjectProperties")]
        [XmlElement(typeof(OWLInverseObjectProperties), ElementName="InverseObjectProperties")]
        [XmlElement(typeof(OWLObjectPropertyDomain), ElementName="ObjectPropertyDomain")]
        [XmlElement(typeof(OWLObjectPropertyRange), ElementName="ObjectPropertyRange")]
        [XmlElement(typeof(OWLFunctionalObjectProperty), ElementName="FunctionalObjectProperty")]
        [XmlElement(typeof(OWLInverseFunctionalObjectProperty), ElementName="InverseFunctionalObjectProperty")]
        [XmlElement(typeof(OWLReflexiveObjectProperty), ElementName="ReflexiveObjectProperty")]
        [XmlElement(typeof(OWLIrreflexiveObjectProperty), ElementName="IrreflexiveObjectProperty")]
        [XmlElement(typeof(OWLSymmetricObjectProperty), ElementName="SymmetricObjectProperty")]
        [XmlElement(typeof(OWLAsymmetricObjectProperty), ElementName="AsymmetricObjectProperty")]
        [XmlElement(typeof(OWLTransitiveObjectProperty), ElementName="TransitiveObjectProperty")]
        public List<OWLObjectPropertyAxiom> ObjectPropertyAxioms { get; internal set; }

        [XmlElement(typeof(OWLSubDataPropertyOf), ElementName="SubDataPropertyOf")]
        [XmlElement(typeof(OWLEquivalentDataProperties), ElementName="EquivalentDataProperties")]
        [XmlElement(typeof(OWLDisjointDataProperties), ElementName="DisjointDataProperties")]
        [XmlElement(typeof(OWLDataPropertyDomain), ElementName="DataPropertyDomain")]
        [XmlElement(typeof(OWLDataPropertyRange), ElementName="DataPropertyRange")]
        [XmlElement(typeof(OWLFunctionalDataProperty), ElementName="FunctionalDataProperty")]
        public List<OWLDataPropertyAxiom> DataPropertyAxioms { get; internal set; }

        [XmlElement(ElementName="DatatypeDefinition")]
        public List<OWLDatatypeDefinition> DatatypeDefinitionAxioms { get; internal set; }

        [XmlElement(ElementName="HasKey")]
        public List<OWLHasKey> HasKeyAxioms { get; internal set; }

        [XmlElement(typeof(OWLSameIndividual), ElementName="SameIndividual")]
        [XmlElement(typeof(OWLDifferentIndividuals), ElementName="DifferentIndividuals")]
        [XmlElement(typeof(OWLClassAssertion), ElementName="ClassAssertion")]
        [XmlElement(typeof(OWLObjectPropertyAssertion), ElementName="ObjectPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeObjectPropertyAssertion), ElementName="NegativeObjectPropertyAssertion")]
        [XmlElement(typeof(OWLDataPropertyAssertion), ElementName="DataPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeDataPropertyAssertion), ElementName="NegativeDataPropertyAssertion")]
        public List<OWLAssertionAxiom> AssertionAxioms { get; internal set; }

        [XmlElement(typeof(OWLAnnotationAssertion), ElementName="AnnotationAssertion")]
        [XmlElement(typeof(OWLSubAnnotationPropertyOf), ElementName="SubAnnotationPropertyOf")]
        [XmlElement(typeof(OWLAnnotationPropertyDomain), ElementName="AnnotationPropertyDomain")]
        [XmlElement(typeof(OWLAnnotationPropertyRange), ElementName="AnnotationPropertyRange")]
        public List<OWLAnnotationAxiom> AnnotationAxioms { get; internal set; }
        #endregion

        #region Ctors
        internal OWLOntology()
        {
            Prefixes = new List<OWLPrefix>()
            {
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.OWL.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDFS.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XSD.PREFIX)),
                new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX))
            };
            Imports = new List<OWLImport>();
            Annotations = new List<OWLAnnotation>();

            //Axioms
            DeclarationAxioms = new List<OWLDeclaration>();
            ClassAxioms = new List<OWLClassAxiom>();
            ObjectPropertyAxioms = new List<OWLObjectPropertyAxiom>();
            DataPropertyAxioms = new List<OWLDataPropertyAxiom>();
            DatatypeDefinitionAxioms = new List<OWLDatatypeDefinition>();
            HasKeyAxioms = new List<OWLHasKey>();
            AssertionAxioms = new List<OWLAssertionAxiom>();
            AnnotationAxioms = new List<OWLAnnotationAxiom>();
        }
        public OWLOntology(Uri ontologyIRI, Uri ontologyVersion=null) : this()
        {
            IRI = ontologyIRI?.ToString();
            Version = ontologyVersion?.ToString();
        }
        #endregion
    }
}