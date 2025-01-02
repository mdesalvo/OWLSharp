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
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLImport represents the IRI of a remote ontology declared for use by an ontology
    /// </summary>
    [XmlRoot("Import")]
    public class OWLImport
    {
        #region Properties
        /// <summary>
        /// IRI of the remote ontology (e.g: http://xmlns.com/foaf/0.1/)
        /// </summary>
        [XmlText(DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an empty ontology import (for internal serialization purposes)
        /// </summary>
        internal OWLImport() { }

        /// <summary>
        /// Builds an ontology import from the given resource
        /// </summary>
        /// <exception cref="OWLException">Thrown when the given resource is null or blank</exception>
        public OWLImport(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException("Cannot create OWLImport because given \"iri\" parameter is null");
            if (iri.IsBlank)
                throw new OWLException("Cannot create OWLImport because given \"iri\" parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
        }
        #endregion
    }
}