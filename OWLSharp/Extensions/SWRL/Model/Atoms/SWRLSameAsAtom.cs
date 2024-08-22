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

using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Ontology.Helpers;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OWLSharp.Extensions.SWRL.Model.Atoms
{
    public class SWRLSameAsAtom : SWRLAtom
    {
        internal static readonly OWLObjectProperty SameAs = new OWLObjectProperty(RDFVocabulary.OWL.SAME_AS);

        #region Ctors
        public SWRLSameAsAtom(RDFVariable leftArgument, RDFVariable rightArgument)
            : base(SameAs, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLSameAsAtom(RDFVariable leftArgument, RDFResource rightArgument)
            : base(SameAs, leftArgument, rightArgument)
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create atom because given \"rightArgument\" parameter is null");
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
            if (RightArgument is RDFVariable)
                RDFQueryEngine.AddColumn(atomResult, rightArgumentString);

            //In case the atom's right argument is an individual, directly enlist its same individuals
            if (RightArgument is RDFResource rightArgumentIndividual)
            {
                //Build the atom's right argument individual
                OWLIndividualExpression rightArgumentIdvExpr;
                if (rightArgumentIndividual.IsBlank)
                    rightArgumentIdvExpr = new OWLAnonymousIndividual(rightArgumentIndividual.ToString().Substring(6));
                else
                    rightArgumentIdvExpr = new OWLNamedIndividual(rightArgumentIndividual);

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

        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology)
        {
            List<OWLInference> inferences = new List<OWLInference>();
            string leftArgumentString = LeftArgument.ToString();
            string rightArgumentString = RightArgument.ToString();
            string sameAsAtomString = this.ToString();

            #region Guards
            //The antecedent results table MUST have a column corresponding to the atom's left argument
            if (!antecedentResults.Columns.Contains(leftArgumentString))
                return inferences;

            //The antecedent results table MUST have a column corresponding to the atom's right argument (if variable)
            if (RightArgument is RDFVariable && !antecedentResults.Columns.Contains(rightArgumentString))
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

                //The current row MUST have a BOUND value in the column corresponding to the atom's right argument (if variable)
                if (this.RightArgument is RDFVariable && currentRow.IsNull(rightArgumentString))
                    continue;
                #endregion

                //Parse the value of the column corresponding to the atom's left argument
                RDFPatternMember leftArgumentValue = RDFQueryUtilities.ParseRDFPatternMember(currentRow[leftArgumentString].ToString());

                //Parse the value of the column corresponding to the atom's right argument
                RDFPatternMember rightArgumentValue = RightArgument is RDFVariable ? RDFQueryUtilities.ParseRDFPatternMember(currentRow[rightArgumentString].ToString())
                                                                                   : RightArgument; //Resource

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
                    OWLSameIndividual inference = new OWLSameIndividual(new List<OWLIndividualExpression>() { leftArgIdvExpr, rightArgIdvExpr }) { IsInference = true };
                    inference.GetXML();
                    inferences.Add(new OWLInference(sameAsAtomString, inference));
                }
            }

            return inferences;
        }
        #endregion
    }
}