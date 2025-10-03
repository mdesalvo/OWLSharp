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
    /// SWRLArgument represents a parameter of a SWRL atom or built-in
    /// </summary>
    [XmlInclude(typeof(SWRLIndividualArgument))]
    [XmlInclude(typeof(SWRLLiteralArgument))]
    [XmlInclude(typeof(SWRLVariableArgument))]
    public abstract class SWRLArgument
    {
        #region Ctors
        internal SWRLArgument() { }
        #endregion
    }

    /// <summary>
    /// SWRLIndividualArgument represents an OWL named individual acting as SWRL parameter
    /// </summary>
    [XmlRoot("NamedIndividual")]
    public sealed class SWRLIndividualArgument : SWRLArgument
    {
        #region Properties
        /// <summary>
        /// The IRI of the named individual
        /// </summary>
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal SWRLIndividualArgument() { }

        /// <summary>
        /// Builds an individual parameter from the given RDFResource
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public SWRLIndividualArgument(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new SWRLException($"Cannot create SWRLIndividualArgument because given '{nameof(iri)}' parameter is null");
            if (iri.IsBlank)
                throw new SWRLException($"Cannot create SWRLIndividualArgument because given '{nameof(iri)}' parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gets the string representation of the individual parameter
        /// </summary>
        public override string ToString()
            => GetResource().ToString();
        #endregion

        #region Methods
        /// <summary>
        /// Gets the RDFResource representation of the individual parameter
        /// </summary>
        public RDFResource GetResource()
            => new RDFResource(IRI);
        #endregion
    }

    /// <summary>
    /// SWRLLiteralArgument represents an OWL literal acting as SWRL parameter
    /// </summary>
    [XmlRoot("Literal")]
    public sealed class SWRLLiteralArgument : SWRLArgument
    {
        #region Properties
        /// <summary>
        /// The eventual datatype IRI of the literal argument (in case it is RDFTypedLiteral)
        /// </summary>
        [XmlAttribute("datatypeIRI", DataType="anyURI")]
        public string DatatypeIRI { get; set; }

        /// <summary>
        /// The eventual language of the literal argument (in case it is RDFPlainLiteral)
        /// </summary>
        [XmlAttribute("xml:lang", Namespace="http://www.w3.org/XML/1998/namespace#")]
        public string Language { get; set; }

        /// <summary>
        /// The value of the literal argument
        /// </summary>
        [XmlText(DataType="string")]
        public string Value { get; set; } = string.Empty;
        #endregion

        #region Ctors
        internal SWRLLiteralArgument() { }

        /// <summary>
        /// Builds a literal parameter from the given RDFLiteral
        /// </summary>
        public SWRLLiteralArgument(RDFLiteral literal)
        {
            Value = literal?.Value ?? string.Empty;
            DatatypeIRI = literal is RDFTypedLiteral tLit ? tLit.Datatype.ToString() : null;
            Language = literal is RDFPlainLiteral pLit && pLit.HasLanguage() ? pLit.Language : null;
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gets the string representation of the literal parameter
        /// </summary>
        public override string ToString()
            => GetLiteral().ToString();
        #endregion

        #region Methods
        /// <summary>
        /// Gets the RDFLiteral representation of the literal parameter
        /// </summary>
        public RDFLiteral GetLiteral()
        {
            if (DatatypeIRI != null)
                return new RDFTypedLiteral(Value, RDFDatatypeRegister.GetDatatype(DatatypeIRI));

            return new RDFPlainLiteral(Value, Language);
        }
        #endregion
    }

    /// <summary>
    /// SWRLVariableArgument represents an OWL variable acting as SWRL parameter
    /// </summary>
    [XmlRoot("Variable")]
    public sealed class SWRLVariableArgument : SWRLArgument
    {
        #region Properties
        /// <summary>
        /// The IRI of the variable (always prefixed with "urn:swrl:var#")
        /// </summary>
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal SWRLVariableArgument() { }

        /// <summary>
        /// Builds a variable parameter from the given RDFVariable
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public SWRLVariableArgument(RDFVariable variable)
        {
            #region Guards
            if (variable == null)
                throw new SWRLException($"Cannot create SWRLVariableArgument because given '{nameof(variable)}' parameter is null");
            #endregion

            IRI = new RDFResource($"urn:swrl:var#{variable.VariableName.Substring(1)}").ToString();
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gets the string representation of the variable parameter
        /// </summary>
        public override string ToString()
            => GetVariable().ToString();
        #endregion

        #region Methods
        /// <summary>
        /// Gets the RDFVariable representation of the variable parameter
        /// </summary>
        public RDFVariable GetVariable()
            => new RDFVariable($"?{IRI.Replace("urn:swrl:var#", string.Empty)}");
        #endregion
    }
}