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
using RDFSharp.Query;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLLiteral is an entity suitable for modeling literals of any kind
    /// </summary>
    [XmlRoot("Literal")]
    public sealed class OWLLiteral : OWLLiteralExpression
    {
        #region Properties
        /// <summary>
        /// The IRI of the literal's datatype, in case of typed literal (e.g: http://www.w3.org/2001/XMLSchema#integer)
        /// </summary>
        [XmlAttribute("datatypeIRI", DataType="anyURI")]
        public string DatatypeIRI { get; set; }

        /// <summary>
        /// The language of the literal's value, in case of plain literal with language (e.g: en-US)
        /// </summary>
        [XmlAttribute("xml:lang", Namespace="http://www.w3.org/XML/1998/namespace#")]
        public string Language { get; set; }

        /// <summary>
        /// The value of the literal (e.g: "42")
        /// </summary>
        [XmlText(DataType="string")]
        public string Value { get; set; } = string.Empty;
        #endregion

        #region Ctors
        internal OWLLiteral() { }

        /// <summary>
        /// Builds a literal from the given RDFLiteral
        /// </summary>
        public OWLLiteral(RDFLiteral literal)
        {
            Value = literal?.Value ?? string.Empty;
            DatatypeIRI = literal is RDFTypedLiteral tLit ? tLit.Datatype.ToString() : null;
            Language = literal is RDFPlainLiteral pLit && pLit.HasLanguage() ? pLit.Language : null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the SWRL representation of this literal
        /// </summary>
        public override string ToSWRLString()
            => RDFQueryPrinter.PrintPatternMember(GetLiteral(), RDFNamespaceRegister.Instance.Register);

        /// <summary>
        /// Gets the RDFLiteral representation of this literal
        /// </summary>
        public RDFLiteral GetLiteral()
        {
            if (DatatypeIRI != null)
                return new RDFTypedLiteral(Value, RDFDatatypeRegister.GetDatatype(DatatypeIRI));

            return new RDFPlainLiteral(Value, Language);
        }
        #endregion
    }
}