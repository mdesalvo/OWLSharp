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

namespace OWLSharp.DataModel
{
    [XmlRoot("Declaration")]
    public partial class OWLDeclaration : OWLAxiom
    {
        #region Properties
        //Register here all derived types of OWLExpression allowed for declaration
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=2)]
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=2)]
        [XmlElement(typeof(OWLAnnotationProperty), ElementName="AnnotationProperty", Order=2)]
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual", Order=2)]
        public OWLExpression Expression { get; set; }
        #endregion

        #region Ctors
        public OWLDeclaration(OWLClass classIRI) : this()
            => Expression = classIRI ?? throw new OWLException("Cannot create OWLDeclaration because given \"classIRI\" parameter is null");
        public OWLDeclaration(OWLDatatype datatypeIRI) : this()
            => Expression = datatypeIRI ?? throw new OWLException("Cannot create OWLDeclaration because given \"datatypeIRI\" parameter is null");
        public OWLDeclaration(OWLObjectProperty objectPropertyIRI) : this()
            => Expression = objectPropertyIRI ?? throw new OWLException("Cannot create OWLDeclaration because given \"objectPropertyIRI\" parameter is null");
        public OWLDeclaration(OWLDataProperty dataPropertyIRI) : this()
            => Expression = dataPropertyIRI ?? throw new OWLException("Cannot create OWLDeclaration because given \"dataPropertyIRI\" parameter is null");
        public OWLDeclaration(OWLAnnotationProperty annotationPropertyIRI) : this()
            => Expression = annotationPropertyIRI ?? throw new OWLException("Cannot create OWLDeclaration because given \"annotationPropertyIRI\" parameter is null");
        public OWLDeclaration(OWLNamedIndividual namedIndividualIRI) : this()
            => Expression = namedIndividualIRI ?? throw new OWLException("Cannot create OWLDeclaration because given \"namedIndividualIRI\" parameter is null");
        #endregion
    }
}