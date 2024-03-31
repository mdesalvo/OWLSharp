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

using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OWLSharp
{
    [XmlRoot("Ontology")]
    public class OWLOntology
    {
        #region Properties
        [XmlAttribute("ontologyIRI", DataType="anyURI")]
        public string OntologyIRI { get; set; }

        [XmlAttribute("ontologyVersion", DataType="anyURI")]
        public string OntologyVersion { get; set; }

        [XmlElement("Prefix")]
        public List<OWLPrefix> Prefixes { get; set; }

        [XmlElement("Import")]
        public List<OWLImport> Imports { get; set; }

        [XmlElement("Annotation")]
        public List<OWLAnnotation> Annotations { get; set; }

        //Register here all derived types of OWLAxiom
        //OWLDeclarationAxiom
        [XmlElement(typeof(OWLDeclarationAxiom), ElementName="Declaration")]
        //OWLClassAxiom
        [XmlElement(typeof(OWLSubClassOfAxiom), ElementName="SubClassOf")]
        [XmlElement(typeof(OWLEquivalentClassesAxiom), ElementName="EquivalentClasses")]
        [XmlElement(typeof(OWLDisjointClassesAxiom), ElementName="DisjointClasses")]
        [XmlElement(typeof(OWLDisjointUnionAxiom), ElementName="DisjointUnion")]
        //OWLObjectPropertyAxiom
        [XmlElement(typeof(OWLSubObjectPropertyOfAxiom), ElementName="SubObjectPropertyOf")]
        [XmlElement(typeof(OWLEquivalentObjectPropertiesAxiom), ElementName="EquivalentObjectProperties")]
        [XmlElement(typeof(OWLDisjointObjectPropertiesAxiom), ElementName="DisjointObjectProperties")]
        [XmlElement(typeof(OWLInverseObjectPropertiesAxiom), ElementName="InverseObjectProperties")]
        [XmlElement(typeof(OWLObjectPropertyDomainAxiom), ElementName="ObjectPropertyDomain")]
        [XmlElement(typeof(OWLObjectPropertyRangeAxiom), ElementName="ObjectPropertyRange")]
        [XmlElement(typeof(OWLFunctionalObjectPropertyAxiom), ElementName="FunctionalObjectProperty")]
        [XmlElement(typeof(OWLInverseFunctionalObjectPropertyAxiom), ElementName="InverseFunctionalObjectProperty")]
        [XmlElement(typeof(OWLReflexiveObjectPropertyAxiom), ElementName="ReflexiveObjectProperty")]
        [XmlElement(typeof(OWLIrreflexiveObjectPropertyAxiom), ElementName="IrreflexiveObjectProperty")]
        [XmlElement(typeof(OWLSymmetricObjectPropertyAxiom), ElementName="SymmetricObjectProperty")]
        [XmlElement(typeof(OWLAsymmetricObjectPropertyAxiom), ElementName="AsymmetricObjectProperty")]
        [XmlElement(typeof(OWLTransitiveObjectPropertyAxiom), ElementName="TransitiveObjectProperty")]
        //OWLDataPropertyAxiom
        [XmlElement(typeof(OWLSubDataPropertyOfAxiom), ElementName="SubDataPropertyOf")]
        [XmlElement(typeof(OWLEquivalentDataPropertiesAxiom), ElementName="EquivalentDataProperties")]
        [XmlElement(typeof(OWLDisjointDataPropertiesAxiom), ElementName="DisjointDataProperties")]
        [XmlElement(typeof(OWLDataPropertyDomainAxiom), ElementName="DataPropertyDomain")]
        [XmlElement(typeof(OWLDataPropertyRangeAxiom), ElementName="DataPropertyRange")]
        [XmlElement(typeof(OWLFunctionalDataPropertyAxiom), ElementName="FunctionalDataProperty")]
        //OWLDatatypeDefinitionAxiom
        [XmlElement(typeof(OWLDatatypeDefinitionAxiom), ElementName="DatatypeDefinition")]
        //OWLHasKeyAxiom
        [XmlElement(typeof(OWLHasKeyAxiom), ElementName="HasKey")]
        //OWLAssertionAxiom
        [XmlElement(typeof(OWLSameIndividualAxiom), ElementName="SameIndividual")]
        [XmlElement(typeof(OWLDifferentIndividualsAxiom), ElementName="DifferentIndividuals")]
        [XmlElement(typeof(OWLClassAssertionAxiom), ElementName="ClassAssertion")]
        [XmlElement(typeof(OWLObjectPropertyAssertionAxiom), ElementName="ObjectPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeObjectPropertyAssertionAxiom), ElementName="NegativeObjectPropertyAssertion")]
        [XmlElement(typeof(OWLDataPropertyAssertionAxiom), ElementName="DataPropertyAssertion")]
        [XmlElement(typeof(OWLNegativeDataPropertyAssertionAxiom), ElementName="NegativeDataPropertyAssertion")]
        //OWLAnnotationAxiom
        [XmlElement(typeof(OWLAnnotationAssertionAxiom), ElementName="AnnotationAssertion")]
        [XmlElement(typeof(OWLSubAnnotationPropertyOfAxiom), ElementName="SubAnnotationPropertyOf")]
        [XmlElement(typeof(OWLAnnotationPropertyDomainAxiom), ElementName="AnnotationPropertyDomain")]
        [XmlElement(typeof(OWLAnnotationPropertyRangeAxiom), ElementName="AnnotationPropertyRange")]
        public List<OWLAxiom> Axioms { get; set; }
        #endregion

        #region Ctors
        internal OWLOntology() {}
        public OWLOntology(Uri ontologyIRI, Uri ontologyVersion=null)
        {
            OntologyIRI = ontologyIRI?.ToString();
            OntologyVersion = ontologyVersion?.ToString();
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
            Axioms = new List<OWLAxiom>();
        }
        #endregion
    }
}