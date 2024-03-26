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

using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLDeclarationAxiom : OWLAxiom
    {
        #region Properties
        //Register here derived types of (in-scope) OWLExpression
        [XmlElement(typeof(OWLClass), ElementName="Class")]
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype")]
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty")]
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty")]
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty")]
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual")]
        public OWLExpression Expression { get; set; }
        #endregion

        #region Ctors
        internal OWLDeclarationAxiom() { }
        public OWLDeclarationAxiom(OWLClass classIRI) 
            => Expression = classIRI ?? throw new OWLException("Cannot create OWLDeclarationAxiom because given \"classIRI\" parameter is null");
        public OWLDeclarationAxiom(OWLDatatype datatypeIRI) 
            => Expression = datatypeIRI ?? throw new OWLException("Cannot create OWLDeclarationAxiom because given \"datatypeIRI\" parameter is null");
        public OWLDeclarationAxiom(OWLObjectProperty objectPropertyIRI) 
            => Expression = objectPropertyIRI ?? throw new OWLException("Cannot create OWLDeclarationAxiom because given \"objectPropertyIRI\" parameter is null");
        public OWLDeclarationAxiom(OWLDataProperty dataPropertyIRI) 
            => Expression = dataPropertyIRI ?? throw new OWLException("Cannot create OWLDeclarationAxiom because given \"dataPropertyIRI\" parameter is null");
        public OWLDeclarationAxiom(OWLAnnotationProperty annotationPropertyIRI) 
            => Expression = annotationPropertyIRI ?? throw new OWLException("Cannot create OWLDeclarationAxiom because given \"annotationPropertyIRI\" parameter is null");
        public OWLDeclarationAxiom(OWLNamedIndividual namedIndividualIRI) 
            => Expression = namedIndividualIRI ?? throw new OWLException("Cannot create OWLDeclarationAxiom because given \"namedIndividualIRI\" parameter is null");
        #endregion
    }
}