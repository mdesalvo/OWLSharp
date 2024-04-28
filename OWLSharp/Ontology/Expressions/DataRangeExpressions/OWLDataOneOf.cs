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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Expressions
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
		internal override RDFGraph ToRDFGraph()
		{
			RDFGraph graph = new RDFGraph();

			RDFCollection dataoneofCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Literal);
			foreach (OWLLiteral dataoneofLiteral in Literals)
				dataoneofCollection.AddItem(dataoneofLiteral.ToRDFLiteral());
			graph.AddCollection(dataoneofCollection);

			return graph;
		}
		#endregion
    }
}