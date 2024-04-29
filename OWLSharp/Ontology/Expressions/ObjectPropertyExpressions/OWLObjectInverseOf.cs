﻿/*
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

using System.Xml;
using System.Xml.Serialization;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Expressions
{
    [XmlRoot("ObjectInverseOf")]
    public class OWLObjectInverseOf : OWLObjectPropertyExpression
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
		internal override RDFGraph GetGraph(RDFResource expressionIRI=null)
		{
			RDFGraph graph = new RDFGraph();

			graph.AddTriple(new RDFTriple(GetIRI(), RDFVocabulary.OWL.INVERSE_OF, ObjectProperty.GetIRI()));

			return graph;
		}
		#endregion
    }
}