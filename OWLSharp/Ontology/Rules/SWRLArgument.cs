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
    //Register here all derived types of SWRLArgument
    [XmlInclude(typeof(SWRLIndividualArgument))]
    [XmlInclude(typeof(SWRLLiteralArgument))]
    [XmlInclude(typeof(SWRLVariableArgument))]    
    public abstract class SWRLArgument
    {
        #region Ctors
        internal SWRLArgument() { }
        #endregion
    }

    //Derived

    [XmlRoot("NamedIndividual")]
    public class SWRLIndividualArgument : SWRLArgument
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal SWRLIndividualArgument() { }
        public SWRLIndividualArgument(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new SWRLException("Cannot create SWRLIndividualArgument because given \"iri\" parameter is null");
            if (iri.IsBlank)
                throw new SWRLException("Cannot create SWRLIndividualArgument because given \"iri\" parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
        }
        #endregion

        #region Interfaces
        public override string ToString()
            => GetResource().ToString();
        #endregion

        #region Methods
        public RDFResource GetResource()
            => new RDFResource(IRI);
        #endregion
    }

    [XmlRoot("Literal")]
    public class SWRLLiteralArgument : SWRLArgument
    {
        #region Properties
        [XmlAttribute("datatypeIRI", DataType="anyURI")]
        public string DatatypeIRI { get; set; }

        [XmlAttribute("xml:lang", Namespace="http://www.w3.org/XML/1998/namespace#")]
        public string Language { get; set; }

        [XmlText(DataType="string")]
        public string Value { get; set; } = string.Empty;
        #endregion

        #region Ctors
        internal SWRLLiteralArgument() { }
        public SWRLLiteralArgument(RDFLiteral literal)
        {
            Value = literal?.Value ?? string.Empty;
            DatatypeIRI = literal is RDFTypedLiteral tLit ? tLit.Datatype.ToString() : null;
            Language = literal is RDFPlainLiteral pLit && pLit.HasLanguage() ? pLit.Language : null;
        }
        #endregion

        #region Interfaces
        public override string ToString()
            => GetLiteral().ToString();
        #endregion

        #region Methods
        public RDFLiteral GetLiteral()
        {
            if (DatatypeIRI != null)
                return new RDFTypedLiteral(Value, RDFDatatypeRegister.GetDatatype(DatatypeIRI));
            else
                return new RDFPlainLiteral(Value, Language);
        }
        #endregion
    }

    [XmlRoot("Variable")]
    public class SWRLVariableArgument : SWRLArgument
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal SWRLVariableArgument() { }
        public SWRLVariableArgument(RDFVariable variable)
        {
            #region Guards
            if (variable == null)
                throw new SWRLException("Cannot create SWRLVariableArgument because given \"variable\" parameter is null");
            #endregion

            IRI = new RDFResource($"urn:swrl:var#{variable.VariableName.Substring(1)}").ToString();
        }
        #endregion

        #region Interfaces
        public override string ToString()
            => GetVariable().ToString();
        #endregion

        #region Methods
        public RDFVariable GetVariable()
            => new RDFVariable($"?{IRI.Replace("urn:swrl:var#", string.Empty)}");
        #endregion
    }
}