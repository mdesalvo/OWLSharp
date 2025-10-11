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

using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLClassAtom is an atom that asserts or tests whether an individual argument is an instance of a specified class from the ontology.
    /// For example, Person(?x) in a rule body checks whether the individual bound to variable ?x is a member of the Person class,
    /// while in the head it would entail that the individual belongs to that class, allowing rules to pattern-match based on class membership
    /// and derive new class assertions.
    /// </summary>
    [XmlRoot("ClassAtom")]
    public sealed class SWRLClassAtom : SWRLAtom
    {
        #region Ctors
        internal SWRLClassAtom() { }

        /// <summary>
        /// Builds a class atom with the given predicate and unary argument
        /// </summary>
        public SWRLClassAtom(OWLClassExpression classExpression, SWRLVariableArgument leftArgument)
            : base(classExpression, leftArgument, null) { }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluates the atom in the context of being part of a SWRL antecedent
        /// </summary>
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

        /// <summary>
        /// Evaluates the atom in the context of being part of a SWRL consequent
        /// </summary>
        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();
            string leftArgumentString = LeftArgument.ToString();
            string classAtomString = ToString();

            #region Guards
            //The antecedent results table MUST have a column corresponding to the atom's left argument
            if (!antecedentResults.Columns.Contains(leftArgumentString))
                return inferences;
            #endregion

            //Iterate the antecedent results table to materialize the atom's reasoner evidences
            foreach (DataRow currentRow in antecedentResults.Rows)
            {
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

        /// <summary>
        /// Exports this SWRL atom to an equivalent RDFGraph object
        /// </summary>
        internal override RDFGraph ToRDFGraph(RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource atomBN = new RDFResource();
            atomsList.AddItem(atomBN);

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.CLASS_ATOM));
            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.CLASS_PREDICATE, Predicate.GetIRI()));
            graph = graph.UnionWith(Predicate.ToRDFGraph());

            switch (LeftArgument)
            {
                case SWRLVariableArgument leftArgVar:
                {
                    RDFResource leftArgVarIRI = new RDFResource(leftArgVar.IRI);
                    graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT1, leftArgVarIRI));
                    graph.AddTriple(new RDFTriple(leftArgVarIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE));
                    break;
                }
                case SWRLIndividualArgument leftArgIdv:
                    graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT1, leftArgIdv.GetResource()));
                    break;
                case SWRLLiteralArgument leftArgLit:
                    graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT1, leftArgLit.GetLiteral()));
                    break;
            }

            return graph;
        }
        #endregion
    }
}