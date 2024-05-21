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

namespace OWLSharp.Ontology.Expressions
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
            DatatypeIRI = literal is RDFTypedLiteral tLit ? RDFModelUtilities.GetDatatypeFromEnum(tLit.Datatype) : null;
            Language = literal is RDFPlainLiteral pLit && pLit.HasLanguage() ? pLit.Language : null;
        }
        #endregion

		#region Methods
		public RDFLiteral GetLiteral()
		{
			if (DatatypeIRI != null)
				return new RDFTypedLiteral(Value, RDFModelUtilities.GetDatatypeFromString(DatatypeIRI));
			else
				return new RDFPlainLiteral(Value, Language);
		}
		#endregion
    }
}