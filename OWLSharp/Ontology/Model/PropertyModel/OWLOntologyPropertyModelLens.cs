/*
   Copyright 2012-2024 Marco De Salvo

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
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyPropertyModelLens represents a magnifying glass on the knowledge available for a given owl:Property instance within an ontology
    /// </summary>
    public class OWLOntologyPropertyModelLens
    {
        #region Properties
        /// <summary>
        /// Property observed by the lens
        /// </summary>
        public RDFResource Property { get; internal set; }

        /// <summary>
        /// Ontology observed by the lens
        /// </summary>
        public OWLOntology Ontology { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a property model lens for the given owl:Property instance on the given ontology
        /// </summary>
        public OWLOntologyPropertyModelLens(RDFResource owlProperty, OWLOntology ontology)
        {
            if (owlProperty == null)
                throw new OWLException("Cannot create property model lens because given \"owlProperty\" parameter is null");
            if (ontology == null)
                throw new OWLException("Cannot create property model lens because given \"ontology\" parameter is null");

            Property = owlProperty;
            Ontology = ontology;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enlists the properties which are related with the lens property by rdfs:subPropertyOf
        /// </summary>
        public List<RDFResource> SubProperties()
            => Ontology.Model.PropertyModel.GetSubPropertiesOf(Property);

        /// <summary>
        /// Asynchronously enlists the properties which are related with the lens property by rdfs:subPropertyOf
        /// </summary>
        public Task<List<RDFResource>> SubPropertiesAsync()
            => Task.Run(() => SubProperties());

        /// <summary>
        /// Enlists the properties to which the lens property is related by rdfs:subPropertyOf
        /// </summary>
        public List<RDFResource> SuperProperties()
            => Ontology.Model.PropertyModel.GetSuperPropertiesOf(Property);

        /// <summary>
        /// Asynchronously enlists the properties to which the lens property is related by rdfs:subPropertyOf
        /// </summary>
        public Task<List<RDFResource>> SuperPropertiesAsync()
            => Task.Run(() => SuperProperties());

        /// <summary>
        /// Enlists the properties which are related with the lens property by owl:equivalentProperty
        /// </summary>
        public List<RDFResource> EquivalentProperties()
            => Ontology.Model.PropertyModel.GetEquivalentPropertiesOf(Property);

        /// <summary>
        /// Asynchronously enlists the properties which are related with the lens property by owl:equivalentProperty
        /// </summary>
        public Task<List<RDFResource>> EquivalentPropertiesAsync()
            => Task.Run(() => EquivalentProperties());

        /// <summary>
        /// Enlists the properties which are related with the lens property by owl:propertyDisjointWith [OWL2]
        /// </summary>
        public List<RDFResource> DisjointProperties()
            => Ontology.Model.PropertyModel.GetDisjointPropertiesWith(Property);

        /// <summary>
        /// Asynchronously enlists the properties which are related with the lens property owl:propertyDisjointWith [OWL2]
        /// </summary>
        public Task<List<RDFResource>> DisjointPropertiesAsync()
            => Task.Run(() => DisjointProperties());

        /// <summary>
        /// Enlists the properties which are related with the lens property by owl:inverseOf
        /// </summary>
        public List<RDFResource> InverseProperties()
            => Ontology.Model.PropertyModel.GetInversePropertiesOf(Property);

        /// <summary>
        /// Asynchronously enlists the properties which are related with the lens property by owl:inverseOf
        /// </summary>
        public Task<List<RDFResource>> InversePropertiesAsync()
            => Task.Run(() => InverseProperties());

        /// <summary>
        /// Enlists the properties which are related with the lens property by owl:propertyChainAxiom [OWL2]
        /// </summary>
        public List<RDFResource> ChainAxiomProperties()
            => Ontology.Model.PropertyModel.GetChainAxiomPropertiesOf(Property);

        /// <summary>
        /// Asynchronously enlists the properties which are related with the lens property by owl:propertyChainAxiom [OWL2]
        /// </summary>
        public Task<List<RDFResource>> ChainAxiomPropertiesAsync()
            => Task.Run(() => ChainAxiomProperties());

        /// <summary>
        /// Enlists the object annotations to which the lens property is related as subject
        /// </summary>
        public List<RDFTriple> ObjectAnnotations()
        {
            List<RDFTriple> result = new List<RDFTriple>();

            result.AddRange(Ontology.Model.PropertyModel.OBoxGraph.Where(ann => ann.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO && ann.Subject.Equals(Property)));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the object annotations to which the lens property is related as subject
        /// </summary>
        public Task<List<RDFTriple>> ObjectAnnotationsAsync()
            => Task.Run(() => ObjectAnnotations());

        /// <summary>
        /// Enlists the data annotations to which the lens property is related as subject
        /// </summary>
        public List<RDFTriple> DataAnnotations()
        {
            List<RDFTriple> result = new List<RDFTriple>();

            result.AddRange(Ontology.Model.PropertyModel.OBoxGraph.Where(ann => ann.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL && ann.Subject.Equals(Property)));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the data annotations to which the lens property is related as subject
        /// </summary>
        public Task<List<RDFTriple>> DataAnnotationsAsync()
            => Task.Run(() => DataAnnotations());
        #endregion
    }
}