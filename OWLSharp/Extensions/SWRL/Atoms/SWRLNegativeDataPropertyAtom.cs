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

namespace OWLSharp.Extensions.SWRL.Atoms
{
    public class SWRLNegativeDataPropertyAtom : SWRLAtom
    {
        #region Ctors
        public SWRLNegativeDataPropertyAtom(OWLDataProperty dataProperty, RDFVariable leftArgument, RDFVariable rightArgument)
            : base(dataProperty, leftArgument, rightArgument) 
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLNegativeDataPropertyAtom(OWLDataProperty dataProperty, RDFVariable leftArgument, RDFLiteral rightArgument)
            : base(dataProperty, leftArgument, rightArgument) 
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

            //Extract negative data property assertions of the atom predicate
			List<OWLNegativeDataPropertyAssertion> ndpAsns = ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>();
            List<OWLNegativeDataPropertyAssertion> atomPredicateNegativeAssertions = OWLAssertionAxiomHelper.SelectNegativeDataAssertionsByDPEX(ndpAsns, (OWLDataProperty)Predicate);
            if (RightArgument is RDFLiteral rightArgumentLiteral)
                atomPredicateNegativeAssertions = atomPredicateNegativeAssertions.Where(asn => asn.Literal.GetLiteral().Equals(rightArgumentLiteral))
																                 .ToList();

            //Save them into the atom result
            Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
            foreach (OWLNegativeDataPropertyAssertion atomPredicateNegativeAssertion in atomPredicateNegativeAssertions)
            {   
                atomResultBindings.Add(leftArgumentString, atomPredicateNegativeAssertion.IndividualExpression.GetIRI().ToString());
                if (RightArgument is RDFVariable)
                    atomResultBindings.Add(rightArgumentString, atomPredicateNegativeAssertion.Literal.GetLiteral().ToString());

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
            string rightArgumentString = RightArgument.ToString();
            string negativeDataPropertyAtomString = this.ToString();

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
                if (RightArgument is RDFVariable && currentRow.IsNull(rightArgumentString))
                    continue;
                #endregion

                //Parse the value of the column corresponding to the atom's left argument
                RDFPatternMember leftArgumentValue = RDFQueryUtilities.ParseRDFPatternMember(currentRow[leftArgumentString].ToString());

                //Parse the value of the column corresponding to the atom's right argument
                RDFPatternMember rightArgumentValue = RightArgument is RDFVariable ? RDFQueryUtilities.ParseRDFPatternMember(currentRow[rightArgumentString].ToString())
                                                                                   : RightArgument; //Literal

                if (leftArgumentValue is RDFResource leftArgumentValueResource
                     && rightArgumentValue is RDFLiteral rightArgumentValueLiteral)
                {
					//Build the inference individual
					OWLIndividualExpression dpAsnIdvExpr;
					if (leftArgumentValueResource.IsBlank)
						dpAsnIdvExpr = new OWLAnonymousIndividual(leftArgumentValueResource.ToString().Substring(6));
					else
						dpAsnIdvExpr = new OWLNamedIndividual(leftArgumentValueResource);

					//Create the inference
                    OWLNegativeDataPropertyAssertion inference = new OWLNegativeDataPropertyAssertion(
						(OWLDataProperty)Predicate, 
						new OWLLiteral(rightArgumentValueLiteral)) 
						{ 
							IndividualExpression = dpAsnIdvExpr, 
							IsInference = true 
						};
					inference.GetXML();
					inferences.Add(new OWLInference(negativeDataPropertyAtomString, inference));
                }
            }

            return inferences;
        }
        #endregion
    }
}