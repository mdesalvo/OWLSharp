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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
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
                throw new SWRLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLObjectPropertyAtom(OWLObjectProperty objectProperty, SWRLVariableArgument leftArgument, SWRLIndividualArgument rightArgument)
            : base(objectProperty, leftArgument, rightArgument) 
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

            //Extract object property assertions of the atom predicate
            List<OWLObjectPropertyAssertion> opAsns = OWLAssertionAxiomHelper.CalibrateObjectAssertions(ontology);
            List<OWLObjectPropertyAssertion> atomPredicateAssertions = OWLAssertionAxiomHelper.SelectObjectAssertionsByOPEX(opAsns, (OWLObjectProperty)Predicate);
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
            string objectPropertyAtomString = ToString();

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

        internal override RDFGraph ToRDFGraph(RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();
            
            RDFResource atomBN = new RDFResource();
            atomsList.AddItem(atomBN);

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.INDIVIDUAL_PROPERTY_ATOM));
            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.PROPERTY_PREDICATE, Predicate.GetIRI()));

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