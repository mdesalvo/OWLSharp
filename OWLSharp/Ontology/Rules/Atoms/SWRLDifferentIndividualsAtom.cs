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
    [XmlRoot("DifferentIndividualsAtom")]
    public sealed class SWRLDifferentIndividualsAtom : SWRLAtom
    {
        internal static readonly OWLExpression DifferentFrom = new OWLExpression { ExpressionIRI = RDFVocabulary.OWL.DIFFERENT_FROM };

        #region Ctors
        internal SWRLDifferentIndividualsAtom() { }
        public SWRLDifferentIndividualsAtom(SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            : base(DifferentFrom, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new SWRLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLDifferentIndividualsAtom(SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
            : base(DifferentFrom, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new SWRLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }
        #endregion

        #region Methods
        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology)
        {
            string leftArgumentString = LeftArgument.ToString();
            string rightArgumentString = RightArgument.ToString();

            //Initialize the structure of the atom result
            DataTable atomResult = new DataTable();
            RDFQueryEngine.AddColumn(atomResult, leftArgumentString);
            if (RightArgument is SWRLVariableArgument)
                RDFQueryEngine.AddColumn(atomResult, rightArgumentString);

            //In case the atom's right argument is an individual, directly enlist its different individuals
            if (RightArgument is SWRLIndividualArgument rightArgumentIndividual)
            {
                //Build the atom's right argument individual
                OWLIndividualExpression rightArgumentIdvExpr;
                if (rightArgumentIndividual.GetResource().IsBlank)
                    rightArgumentIdvExpr = new OWLAnonymousIndividual(rightArgumentIndividual.ToString()[6..]);
                else
                    rightArgumentIdvExpr = new OWLNamedIndividual(rightArgumentIndividual.GetResource());

                //Calculate different individuals of the atom's right argument
                List<OWLIndividualExpression> differentIndividuals = ontology.GetDifferentIndividuals(rightArgumentIdvExpr);

                //Save them into the atom result
                Dictionary<string, string> atomResultBindings = [];
                foreach (OWLIndividualExpression differentIndividual in differentIndividuals)
                {
                    atomResultBindings.Add(leftArgumentString, differentIndividual.GetIRI().ToString());

                    RDFQueryEngine.AddRow(atomResult, atomResultBindings);

                    atomResultBindings.Clear();
                }
            }

            //In case the atom's right argument is a variable, iterate the set of declared individuals
            //in order to enlist, foreach of them, their different individuals
            else
            {
                foreach (OWLNamedIndividual declaredIdv in ontology.GetDeclarationAxiomsOfType<OWLNamedIndividual>()
                                                                   .Select(ax => (OWLNamedIndividual)ax.Expression))
                {
                    //Calculate different individuals of the current
                    List<OWLIndividualExpression> differentIndividuals = ontology.GetDifferentIndividuals(declaredIdv);

                    //Save them into the atom result
                    string declaredIdvIRI = declaredIdv.GetIRI().ToString();
                    Dictionary<string, string> atomResultBindings = [];
                    foreach (OWLIndividualExpression differentIndividual in differentIndividuals)
                    {
                        atomResultBindings.Add(leftArgumentString, declaredIdvIRI);
                        atomResultBindings.Add(rightArgumentString, differentIndividual.GetIRI().ToString());

                        RDFQueryEngine.AddRow(atomResult, atomResultBindings);

                        atomResultBindings.Clear();
                    }
                }
            }

            //Return the atom result
            return atomResult;
        }

        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology)
        {
            List<OWLInference> inferences = [];
            string leftArgumentString = LeftArgument.ToString();
            string rightArgumentString = RightArgument.ToString();
            string differentFromAtomString = ToString();

            #region Guards
            //The antecedent results table MUST have a column corresponding to the atom's left argument
            if (!antecedentResults.Columns.Contains(leftArgumentString))
                return inferences;

            //The antecedent results table MUST have a column corresponding to the atom's right argument (if variable)
            if (RightArgument is SWRLVariableArgument
                    && !antecedentResults.Columns.Contains(rightArgumentString))
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
                if (RightArgument is SWRLVariableArgument
                        && currentRow.IsNull(rightArgumentString))
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
                        leftArgIdvExpr = new OWLAnonymousIndividual(leftArgumentValueResource.ToString()[6..]);
                    else
                        leftArgIdvExpr = new OWLNamedIndividual(leftArgumentValueResource);

                    //Build the inference individual (target)
                    OWLIndividualExpression rightArgIdvExpr;
                    if (rightArgumentValueResource.IsBlank)
                        rightArgIdvExpr = new OWLAnonymousIndividual(rightArgumentValueResource.ToString()[6..]);
                    else
                        rightArgIdvExpr = new OWLNamedIndividual(rightArgumentValueResource);

                    //Create the inference
                    OWLDifferentIndividuals inference = new OWLDifferentIndividuals([leftArgIdvExpr, rightArgIdvExpr]) { IsInference = true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(differentFromAtomString, inference));
                }
            }

            return inferences;
        }

        internal override RDFGraph ToRDFGraph(RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource atomBN = new RDFResource();
            atomsList.AddItem(atomBN);

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.DIFFERENT_INDIVIDUALS_ATOM));

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