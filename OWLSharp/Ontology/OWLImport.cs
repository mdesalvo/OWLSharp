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
    /// OWLImport represents a directive stating that the ontology uses term definitions from the given ontology IRI
    /// </summary>
    [XmlRoot("Import")]
    public sealed class OWLImport
    {
        #region Properties
        /// <summary>
        /// The IRI of the referenced ontology (e.g: http://www.w3.org/2000/01/rdf-schema#)
        /// </summary>
        [XmlText(DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal OWLImport() { }

        /// <summary>
        /// Builds a new import directive with the given IRI
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLImport(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException($"Cannot create OWLImport because given '{nameof(iri)}' parameter is null");
            if (iri.IsBlank)
                throw new OWLException($"Cannot create OWLImport because given '{nameof(iri)}' parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
        }
        #endregion
    }
}