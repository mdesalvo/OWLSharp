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

using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules
{
    [XmlRoot("DataRangeAtom")]
    public class SWRLDataRangeAtom : SWRLAtom
    {
        #region Ctors
        internal SWRLDataRangeAtom() { }
        public SWRLDataRangeAtom(OWLDataRangeExpression datarangeExpression, SWRLVariableArgument leftArgument)
            : base(datarangeExpression, leftArgument, null) { }
        #endregion

        #region Methods
        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology)
        {
            string leftArgumentString = LeftArgument.ToString();

            //Initialize the structure of the atom result
            DataTable atomResult = new DataTable();
            RDFQueryEngine.AddColumn(atomResult, leftArgumentString);

            //Extract data property assertions of the atom predicate
            List<OWLLiteral> dpAsnLiterals = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>()
                                                     .Where(dpAsn => ontology.CheckIsLiteralOf((OWLDataRangeExpression)Predicate, dpAsn.Literal))
                                                     .Select(dpAsn => dpAsn.Literal)
                                                     .ToList();

            //Save them into the atom result
            Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
            foreach (OWLLiteral dpAsnLiteral in dpAsnLiterals)
            {
                atomResultBindings.Add(leftArgumentString, dpAsnLiteral.GetLiteral().ToString());

                RDFQueryEngine.AddRow(atomResult, atomResultBindings);

                atomResultBindings.Clear();
            }

            //Return the atom result
            return atomResult;
        }

        //This kind of atom does not emit inferences
        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology) 
            => new List<OWLInference>();

        internal override RDFGraph ToRDFGraph(RDFResource ruleBN, RDFResource antecedentOrConsequentBN, RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource atomBN = new RDFResource();
            atomsList.AddItem(atomBN);

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, new RDFResource("http://www.w3.org/2003/11/swrl#DataRangeAtom")));
            graph.AddTriple(new RDFTriple(atomBN, new RDFResource("http://www.w3.org/2003/11/swrl#dataRange"), Predicate.GetIRI()));

            if (LeftArgument is SWRLVariableArgument leftArgVar)
                graph.AddTriple(new RDFTriple(atomBN, new RDFResource("http://www.w3.org/2003/11/swrl#argument1"), new RDFResource(leftArgVar.IRI)));
            else if (LeftArgument is SWRLIndividualArgument leftArgIdv)
                graph.AddTriple(new RDFTriple(atomBN, new RDFResource("http://www.w3.org/2003/11/swrl#argument1"), leftArgIdv.GetResource()));
            else if (LeftArgument is SWRLLiteralArgument leftArgLit)
                graph.AddTriple(new RDFTriple(atomBN, new RDFResource("http://www.w3.org/2003/11/swrl#argument1"), leftArgLit.GetLiteral()));

            return graph;
        }
        #endregion
    }
}