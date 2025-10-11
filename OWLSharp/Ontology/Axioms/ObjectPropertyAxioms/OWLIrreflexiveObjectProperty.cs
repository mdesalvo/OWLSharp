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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLIrreflexiveObjectProperty axiom asserts that an object property cannot relate any individual to itself,
    /// prohibiting reflexive relationships. For example, IrreflexiveObjectProperty(isParentOf) states that
    /// no individual can be their own parent, allowing reasoners to detect inconsistencies when self-referential
    /// assertions would violate irreflexivity, ensuring the property never holds between an individual and itself.
    /// </summary>
    [XmlRoot("IrreflexiveObjectProperty")]
    public sealed class OWLIrreflexiveObjectProperty : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public OWLObjectPropertyExpression ObjectPropertyExpression { get; set; }
        #endregion

        #region Ctors
        internal OWLIrreflexiveObjectProperty()
        { }
        public OWLIrreflexiveObjectProperty(OWLObjectProperty objectProperty) : this()
            => ObjectPropertyExpression = objectProperty ?? throw new OWLException("Cannot create OWLIrreflexiveObjectProperty because given \"objectProperty\" parameter is null");
        public OWLIrreflexiveObjectProperty(OWLObjectInverseOf objectInverseOf) : this()
            => ObjectPropertyExpression = objectInverseOf ?? throw new OWLException("Cannot create OWLIrreflexiveObjectProperty because given \"objectInverseOf\" parameter is null");
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();
            RDFResource objPropExpressionIRI = ObjectPropertyExpression.GetIRI();
            graph = graph.UnionWith(ObjectPropertyExpression.ToRDFGraph(objPropExpressionIRI));

            //Axiom Triple
            RDFTriple axiomTriple = new RDFTriple(objPropExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY);
            graph.AddTriple(axiomTriple);

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}