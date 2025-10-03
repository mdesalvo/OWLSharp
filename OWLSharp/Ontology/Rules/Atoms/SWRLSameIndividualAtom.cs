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
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// SWRLSameIndividualAtom is a SWRL atom suitable for filtering and reasoning on same individual assertions
    /// </summary>
    [XmlRoot("SameIndividualAtom")]
    public sealed class SWRLSameIndividualAtom : SWRLAtom
    {
        internal static readonly OWLExpression SameAs = new OWLExpression { ExpressionIRI = RDFVocabulary.OWL.SAME_AS };

        #region Ctors
        internal SWRLSameIndividualAtom() { }

        /// <summary>
        /// Builds a same indvidual atom with the given arguments
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public SWRLSameIndividualAtom(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            : base(SameAs, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new SWRLException($"Cannot create atom because given '{nameof(rightArgument)}' parameter is null");
            #endregion
        }

        /// <summary>
        /// Builds a same indvidual atom with the given arguments
        /// </summary>
        /// <exception cref="SWRLException"></exception>
        public SWRLSameIndividualAtom(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
            : base(SameAs, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new SWRLException($"Cannot create atom because given '{nameof(rightArgument)}' parameter is null");
            #endregion
        }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluates the atom in the context of being part of a SWRL antecedent
        /// </summary>
        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology)
        {
            string leftArgumentString = LeftArgument.ToString();
            string rightArgumentString = RightArgument.ToString();

            //Initialize the structure of the atom result
            DataTable atomResult = new DataTable();
            RDFQueryEngine.AddColumn(atomResult, leftArgumentString);
            if (RightArgument is SWRLVariableArgument)
                RDFQueryEngine.AddColumn(atomResult, rightArgumentString);

            //In case the atom's right argument is an individual, directly enlist its same individuals
            if (RightArgument is SWRLIndividualArgument rightArgumentIndividual)
            {
                //Build the atom's right argument individual
                OWLIndividualExpression rightArgumentIdvExpr;
                if (rightArgumentIndividual.GetResource().IsBlank)
                    rightArgumentIdvExpr = new OWLAnonymousIndividual(rightArgumentIndividual.ToString().Substring(6));
                else
                    rightArgumentIdvExpr = new OWLNamedIndividual(rightArgumentIndividual.GetResource());

                //Calculate same individuals of the atom's right argument
                List<OWLIndividualExpression> sameIndividuals = ontology.GetSameIndividuals(rightArgumentIdvExpr);

                //Save them into the atom result
                Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
                foreach (OWLIndividualExpression sameIndividual in sameIndividuals)
                {
                    atomResultBindings.Add(leftArgumentString, sameIndividual.GetIRI().ToString());

                    RDFQueryEngine.AddRow(atomResult, atomResultBindings);

                    atomResultBindings.Clear();
                }
            }

            //In case the atom's right argument is a variable, iterate the set of declared individuals
            //in order to enlist, foreach of them, their same individuals
            else
            {
                foreach (OWLNamedIndividual declaredIdv in ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
                                                                   .Select(ax => (OWLNamedIndividual)ax.Expression))
                {
                    //Calculate same individuals of the current
                    List<OWLIndividualExpression> sameIndividuals = ontology.GetSameIndividuals(declaredIdv);

                    //Save them into the atom result
                    string declaredIdvIRI = declaredIdv.GetIRI().ToString();
                    Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
                    foreach (OWLIndividualExpression sameIndividual in sameIndividuals)
                    {
                        atomResultBindings.Add(leftArgumentString, declaredIdvIRI);
                        atomResultBindings.Add(rightArgumentString, sameIndividual.GetIRI().ToString());

                        RDFQueryEngine.AddRow(atomResult, atomResultBindings);

                        atomResultBindings.Clear();
                    }
                }
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
            string rightArgumentString = RightArgument.ToString();
            string sameAsAtomString = ToString();

            #region Guards
            //The antecedent results table MUST have a column corresponding to the atom's left argument
            if (!antecedentResults.Columns.Contains(leftArgumentString))
                return inferences;

            //The antecedent results table MUST have a column corresponding to the atom's right argument (if variable)
            if (RightArgument is SWRLVariableArgument && !antecedentResults.Columns.Contains(rightArgumentString))
                return inferences;
            #endregion

            //Iterate the antecedent results table to materialize the atom's reasoner evidences
            foreach (DataRow currentRow in antecedentResults.Rows)
            {
                #region Guards
                //The current row MUST have a BOUND value in the column corresponding to the atom's left argument
                if (currentRow.IsNull(leftArgumentString))
                    continue;

                //The current row MUST have a BOUND value in the column corresponding to the atom's right argument (if variable)
                if (RightArgument is SWRLVariableArgument && currentRow.IsNull(rightArgumentString))
                    continue;
                #endregion

                //Parse the value of the column corresponding to the atom's left argument
                RDFPatternMember leftArgumentValue = RDFQueryUtilities.ParseRDFPatternMember(currentRow[leftArgumentString].ToString());

                //Parse the value of the column corresponding to the atom's right argument
                RDFPatternMember rightArgumentValue = RightArgument is SWRLVariableArgument ? RDFQueryUtilities.ParseRDFPatternMember(currentRow[rightArgumentString].ToString())
                                                                                            : ((SWRLIndividualArgument)RightArgument).GetResource(); //Resource

                if (leftArgumentValue is RDFResource leftArgumentValueResource
                     && rightArgumentValue is RDFResource rightArgumentValueResource)
                {
                    //Build the inference individual (source)
                    OWLIndividualExpression leftArgIdvExpr;
                    if (leftArgumentValueResource.IsBlank)
                        leftArgIdvExpr = new OWLAnonymousIndividual(leftArgumentValueResource.ToString().Substring(6));
                    else
                        leftArgIdvExpr = new OWLNamedIndividual(leftArgumentValueResource);

                    //Build the inference individual (target)
                    OWLIndividualExpression rightArgIdvExpr;
                    if (rightArgumentValueResource.IsBlank)
                        rightArgIdvExpr = new OWLAnonymousIndividual(rightArgumentValueResource.ToString().Substring(6));
                    else
                        rightArgIdvExpr = new OWLNamedIndividual(rightArgumentValueResource);

                    //Create the inference
                    OWLSameIndividual inference = new OWLSameIndividual(new List<OWLIndividualExpression> { leftArgIdvExpr, rightArgIdvExpr }) { IsInference = true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(sameAsAtomString, inference));
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

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.SAME_INDIVIDUAL_ATOM));

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

            switch (RightArgument)
            {
                case SWRLVariableArgument rightArgVar:
                {
                    RDFResource rightArgVarIRI = new RDFResource(rightArgVar.IRI);
                    graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT2, rightArgVarIRI));
                    graph.AddTriple(new RDFTriple(rightArgVarIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE));
                    break;
                }
                case SWRLIndividualArgument rightArgIdv:
                    graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT2, rightArgIdv.GetResource()));
                    break;
                case SWRLLiteralArgument rightArgLit:
                    graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT2, rightArgLit.GetLiteral()));
                    break;
            }

            return graph;
        }
        #endregion
    }
}