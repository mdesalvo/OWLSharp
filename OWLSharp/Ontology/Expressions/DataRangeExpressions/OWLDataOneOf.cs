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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    [XmlRoot("DataOneOf")]
    public class OWLDataOneOf : OWLDataRangeExpression
    {
        #region Properties
        [XmlElement(ElementName="Literal")]
        public List<OWLLiteral> Literals { get; set; }
        #endregion

        #region Ctors
        internal OWLDataOneOf() { }
        public OWLDataOneOf(List<OWLLiteral> literals)
        {
            #region Guards
            if (literals == null)
                throw new OWLException("Cannot create OWLDataOneOf because given \"literals\" parameter is null");
            if (literals.Count == 0)
                throw new OWLException("Cannot create OWLDataOneOf because given \"literals\" parameter must contain at least 1 element");
            if (literals.Any(lit => lit == null))
                throw new OWLException("Cannot create OWLDataOneOf because given \"literals\" parameter contains a null element");
            #endregion

            Literals = literals;
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append('{');
            sb.Append(string.Join(",", Literals.Select(lit => lit.ToSWRLString())));
            sb.Append('}');
            sb.Append(')');

            return sb.ToString();
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFCollection dataOneOfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Literal);
            foreach (OWLLiteral dataOneOfLiteral in Literals)
                dataOneOfCollection.AddItem(dataOneOfLiteral.GetLiteral());
            graph.AddCollection(dataOneOfCollection);
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATA_RANGE));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ONE_OF, dataOneOfCollection.ReificationSubject));

            return graph;
        }
        #endregion
    }
}