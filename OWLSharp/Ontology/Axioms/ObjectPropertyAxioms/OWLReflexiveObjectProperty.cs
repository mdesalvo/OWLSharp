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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Axioms
{
    [XmlRoot("ReflexiveObjectProperty")]
    public class OWLReflexiveObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLReflexiveObjectProperty() : base() { }
        public OWLReflexiveObjectProperty(OWLObjectProperty objectProperty) : this()
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException("Cannot create OWLReflexiveObjectProperty because given \"objectProperty\" parameter is null");
        public OWLReflexiveObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException("Cannot create OWLReflexiveObjectProperty because given \"objectInverseOf\" parameter is null");
        #endregion

        #region Methods
        internal override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
			RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();

			graph.AddTriple(new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY));
			graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            return graph;
        }
        #endregion
    }
}