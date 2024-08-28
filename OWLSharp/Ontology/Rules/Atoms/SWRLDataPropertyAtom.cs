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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules.Atoms
{
    [XmlRoot("DataPropertyAtom")]
    public class SWRLDataPropertyAtom : SWRLAtom
    {
        #region Ctors
        internal SWRLDataPropertyAtom() { }
        public SWRLDataPropertyAtom(OWLDataProperty dataProperty, SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            : base(dataProperty, leftArgument, rightArgument) 
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLDataPropertyAtom(OWLDataProperty dataProperty, SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
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
            if (RightArgument is SWRLVariableArgument)
                RDFQueryEngine.AddColumn(atomResult, rightArgumentString);

            //Extract data property assertions of the atom predicate
			List<OWLDataPropertyAssertion> dpAsns = ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>();
            List<OWLDataPropertyAssertion> atomPredicateAssertions = OWLAssertionAxiomHelper.SelectDataAssertionsByDPEX(dpAsns, (OWLDataProperty)Predicate);
            if (RightArgument is SWRLLiteralArgument rightArgumentLiteral)
                atomPredicateAssertions = atomPredicateAssertions.Where(asn => asn.Literal.GetLiteral().Equals(rightArgumentLiteral.GetLiteral()))
																 .ToList();

            //Save them into the atom result
            Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
            foreach (OWLDataPropertyAssertion atomPredicateAssertion in atomPredicateAssertions)
            {   
                atomResultBindings.Add(leftArgumentString, atomPredicateAssertion.IndividualExpression.GetIRI().ToString());
                if (RightArgument is SWRLVariableArgument)
                    atomResultBindings.Add(rightArgumentString, atomPredicateAssertion.Literal.GetLiteral().ToString());

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
            string dataPropertyAtomString = this.ToString();

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
            IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
            while (rowsEnum.MoveNext())
            {
                DataRow currentRow = (DataRow)rowsEnum.Current;

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
                                                                                            : ((SWRLLiteralArgument)RightArgument).GetLiteral(); //Literal

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
                    OWLDataPropertyAssertion inference = new OWLDataPropertyAssertion(
						(OWLDataProperty)Predicate, 
						new OWLLiteral(rightArgumentValueLiteral)) 
						{ 
							IndividualExpression = dpAsnIdvExpr, 
							IsInference = true 
						};
					inference.GetXML();
					inferences.Add(new OWLInference(dataPropertyAtomString, inference));
                }
            }

            return inferences;
        }
        #endregion
    }
}