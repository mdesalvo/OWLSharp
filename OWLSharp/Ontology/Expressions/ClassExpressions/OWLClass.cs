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
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Expressions
{
    [XmlRoot("Class")]
    public class OWLClass : OWLClassExpression
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        [XmlAttribute("abbreviatedIRI", DataType="QName")]
        public XmlQualifiedName AbbreviatedIRI { get; set; }

		[XmlIgnore]
		public override RDFResource ExpressionIRI 
		{
			get 
			{
				string iri = IRI;
				if (string.IsNullOrEmpty(iri))
					iri = string.Concat(AbbreviatedIRI.Namespace, AbbreviatedIRI.Name);
				return new RDFResource(iri);
			}
		}
        #endregion

        #region Ctors
        internal OWLClass() { }
        public OWLClass(RDFResource iri)
        {
            #region Guards
            if (iri == null)
                throw new OWLException("Cannot create OWLClass because given \"iri\" parameter is null");
            if (iri.IsBlank)
                throw new OWLException("Cannot create OWLClass because given \"iri\" parameter is a blank resource");
            #endregion

            IRI = iri.ToString();
        }
        public OWLClass(XmlQualifiedName abbreviatedIri)
            => AbbreviatedIRI = abbreviatedIri ?? throw new OWLException("Cannot create OWLClass because given \"abbreviatedIri\" parameter is null");
		#endregion

		#region Methods
		internal override RDFGraph ToRDFGraph()
		{
			RDFGraph graph = new RDFGraph();

			graph.AddTriple(new RDFTriple(ExpressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));

			return graph;
		}
		#endregion
	}
}