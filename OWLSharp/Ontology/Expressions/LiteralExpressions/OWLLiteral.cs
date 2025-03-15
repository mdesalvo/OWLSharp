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
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("Literal")]
    public class OWLLiteral : OWLLiteralExpression
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
        internal OWLLiteral() { }
        public OWLLiteral(RDFLiteral literal)
        {
            Value = literal?.Value ?? string.Empty;
            DatatypeIRI = literal is RDFTypedLiteral tLit ? tLit.Datatype.ToString() : null;
            Language = literal is RDFPlainLiteral pLit && pLit.HasLanguage() ? pLit.Language : null;
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(RDFQueryPrinter.PrintPatternMember(GetLiteral(), RDFNamespaceRegister.Instance.Register));

            return sb.ToString();
        }

        public RDFLiteral GetLiteral()
        {
            if (DatatypeIRI != null)
                return new RDFTypedLiteral(Value, RDFDatatypeRegister.GetDatatype(DatatypeIRI));

            return new RDFPlainLiteral(Value, Language);
        }
        #endregion
    }
}