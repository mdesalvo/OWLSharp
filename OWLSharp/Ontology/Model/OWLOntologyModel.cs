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

using System;
using System.Threading.Tasks;
using RDFSharp.Model;

namespace OWLSharp
{
    /// <summary>
    /// OWLOntologyModel represents the T-BOX of the application domain formalized by the ontology
    /// </summary>
    public class OWLOntologyModel: IDisposable
    {
        #region Properties
        /// <summary>
        /// Model of the entities contained within the application domain
        /// </summary>
        public OWLOntologyClassModel ClassModel { get; internal set; }

        /// <summary>
        /// Model of the properties linking the entities of the application domain
        /// </summary>
        public OWLOntologyPropertyModel PropertyModel { get; internal set; }

        /// <summary>
        /// Flag indicating that the ontology model has already been disposed
        /// </summary>
        internal bool Disposed { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an empty ontology model
        /// </summary>
        public OWLOntologyModel()
        {
            ClassModel = new OWLOntologyClassModel();
            PropertyModel = new OWLOntologyPropertyModel();
        }

        /// <summary>
        /// Builds an ontology model having the given T-BOX knowledge
        /// </summary>
        public OWLOntologyModel(OWLOntologyClassModel classModel, OWLOntologyPropertyModel propertyModel) : this()
        {
            ClassModel = classModel ?? new OWLOntologyClassModel();
            PropertyModel = propertyModel ?? new OWLOntologyPropertyModel();
        }

        /// <summary>
        /// Destroys the ontology model instance
        /// </summary>
        ~OWLOntologyModel() => Dispose(false);
        #endregion

        #region Interfaces
        /// <summary>
        /// Disposes the ontology model (IDisposable)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the ontology model 
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                ClassModel.Dispose();
                PropertyModel.Dispose();
                ClassModel = null;
                PropertyModel = null;
            }

            Disposed = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a graph representation of the model
        /// </summary>
        public RDFGraph ToRDFGraph(bool includeInferences=true)
            => ClassModel.ToRDFGraph(includeInferences)
                  .UnionWith(PropertyModel.ToRDFGraph(includeInferences));

        /// <summary>
        /// Asynchronously gets a graph representation of the model
        /// </summary>
        public Task<RDFGraph> ToRDFGraphAsync(bool includeInferences=true)
            => Task.Run(() => ToRDFGraph(includeInferences));
        #endregion
    }
}