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

using System.Text;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    [XmlRoot("ObjectInverseOf")]
    public sealed class OWLObjectInverseOf : OWLObjectPropertyExpression
    {
        #region Properties
        [XmlElement]
        public OWLObjectProperty ObjectProperty { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectInverseOf() { }
        public OWLObjectInverseOf(OWLObjectProperty objectProperty)
            => ObjectProperty = objectProperty ?? throw new OWLException("Cannot create OWLObjectInverseOf because given \"objectProperty\" parameter is null");
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("inverse");
            sb.Append('(');
            sb.Append(ObjectProperty.ToSWRLString());
            sb.Append(')');

            return sb.ToString();
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI ??= GetIRI();

            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.INVERSE_OF, ObjectProperty.GetIRI()));
            return graph.UnionWith(ObjectProperty.ToRDFGraph());
        }
        #endregion
    }
}