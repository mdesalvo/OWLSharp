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

namespace OWLSharp.Ontology.Rules
{
    [XmlRoot("ObjectPropertyAtom")]
    public class SWRLObjectPropertyAtom : SWRLAtom
    {
        #region Ctors
        internal SWRLObjectPropertyAtom() { }
        public SWRLObjectPropertyAtom(OWLObjectProperty objectProperty, SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            : base(objectProperty, leftArgument, rightArgument) 
        {
            #region Guards
            if (rightArgument == null)
                throw new OWLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLObjectPropertyAtom(OWLObjectProperty objectProperty, SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
            : base(objectProperty, leftArgument, rightArgument) 
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
            OWLIndividualExpression swapIdvExpr;

            //Initialize the structure of the atom result
            DataTable atomResult = new DataTable();
            RDFQueryEngine.AddColumn(atomResult, leftArgumentString);
            if (RightArgument is SWRLVariableArgument)
                RDFQueryEngine.AddColumn(atomResult, rightArgumentString);

            //Extract object property assertions of the atom predicate
			List<OWLObjectPropertyAssertion> opAsns = ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>();
            List<OWLObjectPropertyAssertion> atomPredicateAssertions = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, (OWLObjectProperty)Predicate);
            #region Calibration
            for (int i = 0; i < atomPredicateAssertions.Count; i++)
            {
                //In case the object assertion works under inverse logic, we must swap source/target of the object assertion
                if (atomPredicateAssertions[i].ObjectPropertyExpression is OWLObjectInverseOf objInvOf)
                {
                    swapIdvExpr = atomPredicateAssertions[i].SourceIndividualExpression;
                    atomPredicateAssertions[i].SourceIndividualExpression = atomPredicateAssertions[i].TargetIndividualExpression;
                    atomPredicateAssertions[i].TargetIndividualExpression = swapIdvExpr;
                    atomPredicateAssertions[i].ObjectPropertyExpression = objInvOf.ObjectProperty;
                }
            }
            atomPredicateAssertions = OWLAxiomHelper.RemoveDuplicates(atomPredicateAssertions);
            #endregion
            if (RightArgument is SWRLIndividualArgument rightArgumentIndividual)
                atomPredicateAssertions = atomPredicateAssertions.Where(asn => asn.TargetIndividualExpression.GetIRI().Equals(rightArgumentIndividual.GetResource()))
																 .ToList();

            //Save them into the atom result
            Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
            foreach (OWLObjectPropertyAssertion atomPredicateAssertion in atomPredicateAssertions)
            {   
                atomResultBindings.Add(leftArgumentString, atomPredicateAssertion.SourceIndividualExpression.GetIRI().ToString());
                if (RightArgument is SWRLVariableArgument)
                    atomResultBindings.Add(rightArgumentString, atomPredicateAssertion.TargetIndividualExpression.GetIRI().ToString());

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
            string objectPropertyAtomString = this.ToString();

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
                                                                                            : ((SWRLIndividualArgument)RightArgument).GetResource(); //Resource

                if (leftArgumentValue is RDFResource leftArgumentValueResource
                     && rightArgumentValue is RDFResource rightArgumentValueResource)
                {
					//Build the inference individual (source)
					OWLIndividualExpression opAsnSrcIdvExpr;
					if (leftArgumentValueResource.IsBlank)
						opAsnSrcIdvExpr = new OWLAnonymousIndividual(leftArgumentValueResource.ToString().Substring(6));
					else
						opAsnSrcIdvExpr = new OWLNamedIndividual(leftArgumentValueResource);

                    //Build the inference individual (target)
                    OWLIndividualExpression opAsnTgtIdvExpr;
                    if (rightArgumentValueResource.IsBlank)
                        opAsnTgtIdvExpr = new OWLAnonymousIndividual(rightArgumentValueResource.ToString().Substring(6));
                    else
                        opAsnTgtIdvExpr = new OWLNamedIndividual(rightArgumentValueResource);

                    //Create the inference
                    OWLObjectPropertyAssertion inference = new OWLObjectPropertyAssertion((OWLObjectProperty)Predicate, opAsnSrcIdvExpr, opAsnTgtIdvExpr) { IsInference = true };
					inference.GetXML();
					inferences.Add(new OWLInference(objectPropertyAtomString, inference));
                }
            }

            return inferences;
        }

        internal override RDFGraph ToRDFGraph(RDFResource ruleBN, RDFResource antecedentOrConsequentBN, RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            //TODO

            return graph;
        }
        #endregion
    }
}