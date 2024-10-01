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

using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules
{
    [XmlRoot("ClassAtom")]
    public class SWRLClassAtom : SWRLAtom
    {
        #region Ctors
        internal SWRLClassAtom() { }
        public SWRLClassAtom(OWLClassExpression classExpression, SWRLVariableArgument leftArgument)
            : base(classExpression, leftArgument, null) { }
        #endregion

        #region Methods
        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology)
        {
            string leftArgumentString = LeftArgument.ToString();

            //Initialize the structure of the atom result
            DataTable atomResult = new DataTable();
            RDFQueryEngine.AddColumn(atomResult, leftArgumentString);

            //Calculate individuals of the atom predicate
            List<OWLIndividualExpression> atomClassIndividuals = ontology.GetIndividualsOf((OWLClassExpression)Predicate);

            //Save them into the atom result
            Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
            foreach (OWLIndividualExpression atomClassIndividual in atomClassIndividuals)
            {
                atomResultBindings.Add(leftArgumentString, atomClassIndividual.GetIRI().ToString());

                RDFQueryEngine.AddRow(atomResult, atomResultBindings);

                atomResultBindings.Clear();
            }

            //Return the atom result
            return atomResult;
        }

        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();
            string leftArgumentString = LeftArgument.ToString();
            string classAtomString = this.ToString();

            #region Guards
            //The antecedent results table MUST have a column corresponding to the atom's left argument
            if (!antecedentResults.Columns.Contains(leftArgumentString))
                return inferences;
            #endregion

            //Iterate the antecedent results table to materialize the atom's reasoner evidences
            IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
            while (rowsEnum.MoveNext())
            {
                DataRow currentRow = (DataRow)rowsEnum.Current;

                #region Guards
                //The current row MUST have a BOUND value in the column corresponding to the atom's left argument
                if (currentRow.IsNull(leftArgumentString))
                    continue;
                #endregion

                //Parse the value of the column corresponding to the atom's left argument
                RDFPatternMember leftArgumentValue = RDFQueryUtilities.ParseRDFPatternMember(currentRow[leftArgumentString].ToString());
                if (leftArgumentValue is RDFResource leftArgumentValueResource)
                {
					//Build the inference individual
					OWLIndividualExpression clsAsnIdvExpr;
					if (leftArgumentValueResource.IsBlank)
						clsAsnIdvExpr = new OWLAnonymousIndividual(leftArgumentValueResource.ToString().Substring(6));
					else
						clsAsnIdvExpr = new OWLNamedIndividual(leftArgumentValueResource);

					//Create the inference
                    OWLClassAssertion inference = new OWLClassAssertion(
						(OWLClassExpression)Predicate) 
						{ 
							IndividualExpression = clsAsnIdvExpr, 
							IsInference = true
						};
					inference.GetXML();
					inferences.Add(new OWLInference(classAtomString, inference));
                }
            }

            return inferences;
        }

        internal override RDFGraph ToRDFGraph(RDFResource ruleBN, RDFResource antecedentOrConsequentBN, RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();
            
            RDFResource atomBN = new RDFResource();
            atomsList.AddItem(atomBN);

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, new RDFResource("http://www.w3.org/2003/11/swrl#ClassAtom")));
            graph.AddTriple(new RDFTriple(atomBN, new RDFResource("http://www.w3.org/2003/11/swrl#classPredicate"), Predicate.GetIRI()));
            graph = graph.UnionWith(((OWLClassExpression)Predicate).ToRDFGraph());

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