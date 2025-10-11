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
using System;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLAnonymousIndividual represents an instance of a class that exists in the ontology but lacks an explicit IRI identifier,
    /// similar to blank nodes in RDF. Anonymous individuals are useful for describing entities whose identity is not important or unknown,
    /// allowing them to participate in relationships and assertions without requiring a globally unique name.
    /// </summary>
    [XmlRoot("AnonymousIndividual")]
    public sealed class OWLAnonymousIndividual : OWLIndividualExpression
    {
        #region Properties
        /// <summary>
        /// The xsd:NCName of this anonymous individual (e.g: Anon2447)
        /// </summary>
        [XmlAttribute("nodeID", DataType="NCName")]
        public string NodeID { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds an anonymous individual (e.g: ANONe8f9f06f6fb4451b84ff6c90540d9dee)
        /// </summary>
        public OWLAnonymousIndividual()
            => NodeID = $"ANON{Guid.NewGuid():N}";

        /// <summary>
        /// Builds an anonymous individual with the given xsd:NCName (e.g: ANON2447)
        /// </summary>
        /// <exception cref="OWLException"></exception>
        public OWLAnonymousIndividual(string xsdNCName)
        {
            try
            {
                RDFTypedLiteral xsdNCNameLiteral = new RDFTypedLiteral(xsdNCName, RDFModelEnums.RDFDatatypes.XSD_NCNAME);
                NodeID = xsdNCNameLiteral.Value;
            }
            catch { throw new OWLException($"Cannot create OWLAnonymousIndividual because given '{nameof(xsdNCName)}' parameter is null or is not a valid xsd:NCName"); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the IRI representation of this anonymous individual (e.g: bnode:ANON2447)
        /// </summary>
        public override RDFResource GetIRI()
            => new RDFResource(string.Concat($"bnode:{NodeID}"));
        #endregion
    }
}