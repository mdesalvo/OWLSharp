﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("ObjectOneOf")]
    public sealed class OWLObjectOneOf : OWLClassExpression
    {
        #region Properties
        //Register here all derived types of OWLIndividualExpression
        [XmlElement(typeof(OWLNamedIndividual), ElementName="NamedIndividual")]
        [XmlElement(typeof(OWLAnonymousIndividual), ElementName="AnonymousIndividual")]
        public List<OWLIndividualExpression> IndividualExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLObjectOneOf() { }
        public OWLObjectOneOf(List<OWLIndividualExpression> individualExpressions)
        {
            #region Guards
            if (individualExpressions == null)
                throw new OWLException("Cannot create OWLObjectOneOf because given \"individualExpressions\" parameter is null");
            if (individualExpressions.Count == 0)
                throw new OWLException("Cannot create OWLObjectOneOf because given \"individualExpressions\" parameter must contain at least 1 element");
            if (individualExpressions.Any(iex => iex == null))
                throw new OWLException("Cannot create OWLObjectOneOf because given \"individualExpressions\" parameter contains a null element");
            #endregion

            IndividualExpressions = individualExpressions;
        }
        #endregion

        #region Methods
        public override string ToSWRLString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('(');
            sb.Append('{');
            sb.Append(string.Join(",", IndividualExpressions.Select(idvExpr => idvExpr.ToSWRLString())));
            sb.Append('}');
            sb.Append(')');

            return sb.ToString();
        }

        internal override RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
        {
            RDFGraph graph = new RDFGraph();
            expressionIRI = expressionIRI ?? GetIRI();

            RDFCollection objectOneOfCollection = new RDFCollection(RDFModelEnums.RDFItemTypes.Resource);
            foreach (OWLIndividualExpression objectOneOfIndividual in IndividualExpressions)
            {
                RDFResource idvExpressionIRI = objectOneOfIndividual.GetIRI();
                objectOneOfCollection.AddItem(idvExpressionIRI);
                graph = graph.UnionWith(objectOneOfIndividual.ToRDFGraph(idvExpressionIRI));
            }
            graph.AddCollection(objectOneOfCollection);
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(expressionIRI, RDFVocabulary.OWL.ONE_OF, objectOneOfCollection.ReificationSubject));

            return graph;
        }
        #endregion
    }
}