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
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyClassModelLens represents a magnifying glass on the knowledge available for a given owl:Class instance within an ontology
    /// </summary>
    public class OWLOntologyClassModelLens
    {
        #region Properties
        /// <summary>
        /// Class observed by the lens
        /// </summary>
        public RDFResource Class { get; internal set; }

        /// <summary>
        /// Ontology observed by the lens
        /// </summary>
        public OWLOntology Ontology { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a class model lens for the given owl:Class instance on the given ontology
        /// </summary>
        public OWLOntologyClassModelLens(RDFResource owlClass, OWLOntology ontology)
        {
            #region Guards
            if (owlClass == null)
                throw new OWLException("Cannot create class model lens because given \"owlClass\" parameter is null");
            if (ontology == null)
                throw new OWLException("Cannot create class model lens because given \"ontology\" parameter is null");
            #endregion

            Class = owlClass;
            Ontology = ontology;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Enlists the classes which are related with the lens class by rdfs:subClassOf
        /// </summary>
        public List<RDFResource> SubClasses()
            => Ontology.Model.ClassModel.GetSubClassesOf(Class);

        /// <summary>
        /// Asynchronously enlists the classes which are related with the lens class by rdfs:subClassOf
        /// </summary>
        public Task<List<RDFResource>> SubClassesAsync()
            => Task.Run(() => SubClasses());

        /// <summary>
        /// Enlists the classes to which the lens class is related by rdfs:subClassOf
        /// </summary>
        public List<RDFResource> SuperClasses()
            => Ontology.Model.ClassModel.GetSuperClassesOf(Class);

        /// <summary>
        /// Asynchronously enlists the classes to which the lens class is related by rdfs:subClassOf
        /// </summary>
        public Task<List<RDFResource>> SuperClassesAsync()
            => Task.Run(() => SuperClasses());

        /// <summary>
        /// Enlists the classes which are related with the lens class by owl:equivalentClass
        /// </summary>
        public List<RDFResource> EquivalentClasses()
            => Ontology.Model.ClassModel.GetEquivalentClassesOf(Class);

        /// <summary>
        /// Asynchronously enlists the classes which are related with the lens class by owl:equivalentClass
        /// </summary>
        public Task<List<RDFResource>> EquivalentClassesAsync()
            => Task.Run(() => EquivalentClasses());

        /// <summary>
        /// Enlists the classes which are related with the lens class by owl:disjointWith
        /// </summary>
        public List<RDFResource> DisjointClasses()
            => Ontology.Model.ClassModel.GetDisjointClassesWith(Class);

        /// <summary>
        /// Asynchronously enlists the classes which are related with the lens class by owl:disjointWith
        /// </summary>
        public Task<List<RDFResource>> DisjointClassesAsync()
            => Task.Run(() => DisjointClasses());

        /// <summary>
        /// Enlists the properties which are related with the lens class by owl:hasKey [OWL2]
        /// </summary>
        public List<RDFResource> KeyProperties()
            => Ontology.Model.ClassModel.GetKeyPropertiesOf(Class);

        /// <summary>
        /// Asynchronously enlists the properties which are related with the lens class by owl:hasKey [OWL2]
        /// </summary>
        public Task<List<RDFResource>> KeyPropertiesAsync()
            => Task.Run(() => KeyProperties());

        /// <summary>
        /// Enlists the individuals which are related with the lens class by rdf:type
        /// </summary>
        public List<RDFResource> Individuals()
            => Ontology.Data.GetIndividualsOf(Ontology.Model, Class);

        /// <summary>
        /// Asynchronously enlists the individuals which are related with the lens class by rdf:type
        /// </summary>
        public Task<List<RDFResource>> IndividualsAsync()
            => Task.Run(() => Individuals());

        /// <summary>
        /// Enlists the individuals which are related with the lens class by a negative rdf:type [OWL2]
        /// </summary>
        public List<RDFResource> NegativeIndividuals()
        {
            List<RDFResource> result = new List<RDFResource>();

            foreach (RDFResource individual in Ontology.Data)
                if (Ontology.Data.CheckIsNegativeIndividualOf(Ontology.Model, individual, Class))
                    result.Add(individual);

            return RDFQueryUtilities.RemoveDuplicates(result);
        }

        /// <summary>
        /// Asynchronously enlists the individuals which are related with the lens class by a negative rdf:type [OWL2]
        /// </summary>
        public Task<List<RDFResource>> NegativeIndividualsAsync()
            => Task.Run(() => NegativeIndividuals());

        /// <summary>
        /// Enlists the object annotations to which the lens class is related as subject
        /// </summary>
        public List<RDFTriple> ObjectAnnotations()
        {
            List<RDFTriple> result = new List<RDFTriple>();

            result.AddRange(Ontology.Model.ClassModel.OBoxGraph.Where(ann => ann.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO && ann.Subject.Equals(Class)));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the object annotations to which the lens class is related as subject
        /// </summary>
        public Task<List<RDFTriple>> ObjectAnnotationsAsync()
            => Task.Run(() => ObjectAnnotations());

        /// <summary>
        /// Enlists the data annotations to which the lens class is related as subject
        /// </summary>
        public List<RDFTriple> DataAnnotations()
        {
            List<RDFTriple> result = new List<RDFTriple>();

            result.AddRange(Ontology.Model.ClassModel.OBoxGraph.Where(ann => ann.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL && ann.Subject.Equals(Class)));

            return result;
        }

        /// <summary>
        /// Asynchronously enlists the data annotations to which the lens class is related as subject
        /// </summary>
        public Task<List<RDFTriple>> DataAnnotationsAsync()
            => Task.Run(() => DataAnnotations());
        #endregion
    }
}