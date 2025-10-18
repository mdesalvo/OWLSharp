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
    /// OWLEquivalentObjectProperties axiom asserts that two or more object properties have exactly the same set of individual pairs,
    /// meaning they are logically equivalent and interchangeable. For example, EquivalentObjectProperties(hasMother hasFemaleParent)
    /// states that the properties hasMother and hasFemaleParent relate individuals in exactly the same way, allowing reasoners to infer
    /// that any assertion using one property implies the corresponding assertion with the other property.
    /// </summary>
    [XmlRoot("EquivalentObjectProperties")]
    public sealed class OWLEquivalentObjectProperties : OWLObjectPropertyAxiom
    {
        #region Properties
        /// <summary>
        /// The set of object property expressions asserted to be equivalent (e.g: http://example.org/hasMother, http://example.org/hasFemaleParent)
        /// </summary>
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=2)]
        [XmlElement(typeof(OWLObjectInverseOf), ElementName="ObjectInverseOf", Order=2)]
        public List<OWLObjectPropertyExpression> ObjectPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLEquivalentObjectProperties() { }

        /// <summary>
        /// Builds an OWLEquivalentObjectProperties with the given set of equivalent object property expressions (must be at least 2)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLEquivalentObjectProperties(List<OWLObjectPropertyExpression> objectPropertyExpressions) : this()
        {
            #region Guards
            if (objectPropertyExpressions == null)
                throw new OWLException($"Cannot create OWLEquivalentObjectProperties because given '{nameof(objectPropertyExpressions)}' parameter is null");
            if (objectPropertyExpressions.Count < 2)
                throw new OWLException($"Cannot create OWLEquivalentObjectProperties because given '{nameof(objectPropertyExpressions)}' parameter must contain at least 2 elements");
            if (objectPropertyExpressions.Any(ope => ope == null))
                throw new OWLException($"Cannot create OWLEquivalentObjectProperties because given '{nameof(objectPropertyExpressions)}' parameter contains a null element");
            #endregion

            ObjectPropertyExpressions = objectPropertyExpressions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Exports this OWLEquivalentObjectProperties to an equivalent RDFGraph object
        /// </summary>
        public override RDFGraph ToRDFGraph()
        {
            RDFGraph graph = new RDFGraph();

            List<RDFResource> objPropIRIs = new List<RDFResource>();
            foreach (OWLObjectPropertyExpression objectPropertyExpression in ObjectPropertyExpressions)
            {
                RDFResource objPropIRI = objectPropertyExpression.GetIRI();
                objPropIRIs.Add(objPropIRI);
                graph = graph.UnionWith(objectPropertyExpression.ToRDFGraph(objPropIRI));
            }

            //Axiom Triple(s)
            List<RDFTriple> axiomTriples = new List<RDFTriple>();
            for (int i = 0; i < ObjectPropertyExpressions.Count - 1; i++)
                for (int j = i+1; j < ObjectPropertyExpressions.Count; j++)
                {
                    RDFTriple axiomTriple = new RDFTriple(objPropIRIs[i], RDFVocabulary.OWL.EQUIVALENT_PROPERTY, objPropIRIs[j]);
                    axiomTriples.Add(axiomTriple);
                    graph.AddTriple(axiomTriple);
                }

            //Annotations
            foreach (OWLAnnotation annotation in Annotations)
                foreach (RDFTriple axiomTriple in axiomTriples)
                    graph = graph.UnionWith(annotation.ToRDFGraph(axiomTriple));

            return graph;
        }
        #endregion
    }
}