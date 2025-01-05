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
    [XmlRoot("DisjointObjectProperties")]
    public class OWLDisjointObjectProperties : OWLObjectPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLObjectPropertyExpression
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointObjectProperties() : base() { }
        public OWLDisjointObjectProperties(List<OWLObjectPropertyExpression> objectPropertyExpressions) : this()
        {
            #region Guards
            if (objectPropertyExpressions == null)
                throw new OWLException("Cannot create OWLDisjointObjectProperties because given \"objectPropertyExpressions\" parameter is null");
            if (objectPropertyExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDisjointObjectProperties because given \"objectPropertyExpressions\" parameter must contain at least 2 elements");
            if (objectPropertyExpressions.Any(ope => ope == null))
                throw new OWLException("Cannot create OWLDisjointObjectProperties because given \"objectPropertyExpressions\" parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion

        #region Methods
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            //propertyDisjointWith
            if (ObjectPropertyExpressions.Count == 2)
            {
                RDFResource leftObjPropExpressionIRI = ObjectPropertyExpressions[0].GetIRI();
                RDFResource rightObjPropExpressionIRI = ObjectPropertyExpressions[1].GetIRI();
                graph = graph.UnionWith(ObjectPropertyExpressions[0].ToRDFGraph(leftObjPropExpressionIRI))
                             .UnionWith(ObjectPropertyExpressions[1].ToRDFGraph(rightObjPropExpressionIRI));

                //Axiom Triple
                RDFTriple axiomTriple = new RDFTriple(leftObjPropExpressionIRI, RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, rightObjPropExpressionIRI);
                graph.AddTriple(axiomTriple);

                //Annotations
                foreach (OWLAnnotation annotation in Annotations)
                    graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));
            }

            //AllDisjointProperties
            else
            {
                RDFResource allDisjointPropertiesIRI = new RDFResource(); 
                RDFCollection disjointObjectPropertiesCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
                foreach (OWLObjectPropertyExpression objectPropertyExpression in ObjectPropertyExpressions)
                {
                    RDFResource objectPropertyExpressionIRI = objectPropertyExpression.GetIRI();
                    disjointObjectPropertiesCollection.AddItem(objectPropertyExpressionIRI);
                    graph = graph.UnionWith(objectPropertyExpression.ToRDFGraph(objectPropertyExpressionIRI));
                }
                graph.AddCollection(disjointObjectPropertiesCollection);
                graph.AddTriple(new RDFTriple(allDisjointPropertiesIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES));
                graph.AddTriple(new RDFTriple(allDisjointPropertiesIRI, RDFVocabulary.OWL.MEMBERS, disjointObjectPropertiesCollection.ReificationSubject));

                //Annotations
                foreach (OWLAnnotation annotation in Annotations)
                    graph = graph.UnionWith(annotation.ToRDFGraphInternal(allDisjointPropertiesIRI));
            }

            return graph;
        }
        #endregion
    }
}