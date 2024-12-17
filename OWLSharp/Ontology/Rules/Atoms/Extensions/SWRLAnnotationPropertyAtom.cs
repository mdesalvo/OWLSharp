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
    [XmlRoot("AnnotationPropertyAtom")]
    public class SWRLAnnotationPropertyAtom : SWRLAtom
    {
        #region Properties
        internal static RDFResource AnnotationPropertyAtomIRI = new RDFResource($"{RDFVocabulary.SWRL.BASE_URI}AnnotationPropertyAtom");
        #endregion

        #region Ctors
        internal SWRLAnnotationPropertyAtom() { }
        public SWRLAnnotationPropertyAtom(OWLAnnotationProperty annotationProperty, SWRLVariableArgument leftArgument, SWRLVariableArgument rightArgument)
            : base(annotationProperty, leftArgument, rightArgument) 
        {
            #region Guards
            if (rightArgument == null)
                throw new SWRLException("Cannot create atom because given \"rightArgument\" parameter is null");
            #endregion
        }

        public SWRLAnnotationPropertyAtom(OWLAnnotationProperty annotationProperty, SWRLVariableArgument leftArgument, SWRLLiteralArgument rightArgument)
            : base(annotationProperty, leftArgument, rightArgument) 
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

            //Extract annotation property assertions of the atom predicate
			List<OWLAnnotationAssertion> annAsns = ontology.GetAnnotationAxiomsOfType<OWLAnnotationAssertion>();
            List<OWLAnnotationAssertion> atomPredicateAssertions = OWLAnnotationAxiomHelper.SelectAnnotationAssertionsByAPEX(annAsns, (OWLAnnotationProperty)Predicate);
            if (RightArgument is SWRLLiteralArgument rightArgumentLiteral)
                atomPredicateAssertions = atomPredicateAssertions.Where(asn => asn.ValueLiteral?.GetLiteral().Equals(rightArgumentLiteral.GetLiteral()) ?? false)
																 .ToList();

            //Save them into the atom result
            Dictionary<string, string> atomResultBindings = new Dictionary<string, string>();
            foreach (OWLAnnotationAssertion atomPredicateAssertion in atomPredicateAssertions)
            {   
                atomResultBindings.Add(leftArgumentString, atomPredicateAssertion.SubjectIRI);
                if (RightArgument is SWRLVariableArgument)
                    atomResultBindings.Add(rightArgumentString, atomPredicateAssertion.ValueLiteral.GetLiteral().ToString());

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
            string annotationPropertyAtomString = this.ToString();

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
					//Create the inference
                    OWLAnnotationAssertion inference = new OWLAnnotationAssertion(
						(OWLAnnotationProperty)Predicate,
                        leftArgumentValueResource,
                        new OWLLiteral(rightArgumentValueLiteral)) 
						{ 
							IsInference=true 
						};
					inference.GetXML();
					inferences.Add(new OWLInference(annotationPropertyAtomString, inference));
                }
            }

            return inferences;
        }

        internal override RDFGraph ToRDFGraph(RDFCollection atomsList)
        {
            RDFGraph graph = new RDFGraph();

            RDFResource atomBN = new RDFResource();
            atomsList.AddItem(atomBN);

            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.RDF.TYPE, AnnotationPropertyAtomIRI));
            graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.PROPERTY_PREDICATE, Predicate.GetIRI()));

            if (LeftArgument is SWRLVariableArgument leftArgVar)
            {
                RDFResource leftArgVarIRI = new RDFResource(leftArgVar.IRI);
                graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT1, leftArgVarIRI));
                graph.AddTriple(new RDFTriple(leftArgVarIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE));
            }
            else if (LeftArgument is SWRLIndividualArgument leftArgIdv)
                graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT1, leftArgIdv.GetResource()));
            else if (LeftArgument is SWRLLiteralArgument leftArgLit)
                graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT1, leftArgLit.GetLiteral()));

            if (RightArgument is SWRLVariableArgument rightArgVar)
            {
                RDFResource rightArgVarIRI = new RDFResource(rightArgVar.IRI);
                graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT2, rightArgVarIRI));
                graph.AddTriple(new RDFTriple(rightArgVarIRI, RDFVocabulary.RDF.TYPE, RDFVocabulary.SWRL.VARIABLE));
            }
            else if (RightArgument is SWRLIndividualArgument rightArgIdv)
                graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT2, rightArgIdv.GetResource()));
            else if (RightArgument is SWRLLiteralArgument rightArgLit)
                graph.AddTriple(new RDFTriple(atomBN, RDFVocabulary.SWRL.ARGUMENT2, rightArgLit.GetLiteral()));

            return graph;
        }
        #endregion
    }
}