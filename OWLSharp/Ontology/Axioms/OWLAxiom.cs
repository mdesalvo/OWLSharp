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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    //Register here all derived types of OWLAxiom
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
        [XmlElement("Annotation", Order=1)]
        public List<OWLAnnotation> Annotations { get; set; }

        [XmlIgnore]
        public bool IsInference { get; set; }

        [XmlIgnore]
        public bool IsImport { get; set; }

        [XmlIgnore]
        internal string AxiomXML { get; set; }
        #endregion

        #region Ctors
        internal OWLAxiom() 
            => Annotations = new List<OWLAnnotation>();
        #endregion

        #region Methods
        internal virtual string GetXML(XmlSerializerNamespaces xmlSerializerNamespaces=null)
        {
            if (AxiomXML == null)
                AxiomXML = OWLSerializer.SerializeObject(this, xmlSerializerNamespaces);
            return AxiomXML;
        }

        public virtual RDFGraph ToRDFGraph()
            => new RDFGraph();
        #endregion
    }

    //Derived

    public class OWLClassAxiom : OWLAxiom
    {
		#region Ctors
        internal OWLClassAxiom() : base() { }
		#endregion	
	}

    public class OWLObjectPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLObjectPropertyAxiom() : base() { }
        #endregion
    }

    public class OWLDataPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLDataPropertyAxiom() : base() { }
        #endregion
    }

    public class OWLAssertionAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAssertionAxiom() : base() { }
        #endregion
    };

    public class OWLAnnotationAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAnnotationAxiom() : base() { }
        #endregion
    }
}