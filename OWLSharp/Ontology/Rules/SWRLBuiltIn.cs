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

using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Data;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections;

namespace OWLSharp.Ontology.Rules
{
    [XmlRoot("BuiltInAtom")]
    public class SWRLBuiltIn
    {
        #region Properties
        [XmlAttribute(DataType="anyURI")]
        public string IRI { get; set; }

        [XmlElement(typeof(SWRLIndividualArgument), ElementName="NamedIndividual")]
        [XmlElement(typeof(SWRLLiteralArgument), ElementName="Literal")]
        [XmlElement(typeof(SWRLVariableArgument), ElementName="Variable")]
        public List<SWRLArgument> Arguments { get; set; }
        #endregion

        #region Ctors
        internal SWRLBuiltIn()
            => Arguments = new List<SWRLArgument>();

        public static SWRLBuiltIn Abs(SWRLVariableArgument leftArg, SWRLVariableArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#abs",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:abs builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:abs builtIn because: right argument is null")
                    }
                };
        #endregion

        #region Interfaces
        public override string ToString()
        {
            #region Guards
            if (Arguments?.Count == 0)
                return string.Empty;
            #endregion

            StringBuilder sb = new StringBuilder();

            //Predicate
            sb.Append("swrlb:");
            sb.Append(RDFModelUtilities.GetShortUri(new Uri(IRI)));
            sb.Append("(");

            //Arguments
            for (int i=0; i<Arguments.Count; i++)
            {
                if (i>0)
                    sb.Append(',');

                if (Arguments[i] is SWRLIndividualArgument leftArgumentIndividual)
                    sb.Append($"{RDFModelUtilities.GetShortUri(leftArgumentIndividual.GetResource().URI)}");
                else if (Arguments[i] is SWRLLiteralArgument leftArgumentLiteral)
                    sb.Append($"{RDFQueryPrinter.PrintPatternMember(leftArgumentLiteral.GetLiteral(), RDFNamespaceRegister.Instance.Register)}");
                else if (Arguments[i] is SWRLVariableArgument leftArgumentVariable)
                    sb.Append($"{RDFQueryPrinter.PrintPatternMember(leftArgumentVariable.GetVariable(), RDFNamespaceRegister.Instance.Register)}");
            }

            sb.Append(")");
            return sb.ToString();
        }
        #endregion

        #region Methods
        internal DataTable EvaluateOnAntecedent(DataTable antecedentResults)
        {
            DataTable filteredTable = antecedentResults.Clone();

            //Iterate the rows of the antecedent results table
            bool keepRow;
            IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
            while (rowsEnum.MoveNext())
                try
                {
                    DataRow currentRow = (DataRow)rowsEnum.Current;
                    
                    //Determine the builtIn to be engaged for current row's evaluation
                    switch (IRI)
                    {
                        case "http://www.w3.org/2003/11/swrlb#abs":
                            keepRow = SWRLAbsBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;

                        //Unsupported builtIns must generate an explicit exception
                        default:
                            throw new NotImplementedException($"unsupported IRI {IRI}");
                    }

                    //If current row has satisfied the builtIn, keep it in the filtered result table
                    if (keepRow)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                        filteredTable.Rows.Add(newRow);
                    }
                }
                catch (Exception ex) 
                { 
                    if (ex is NotImplementedException nex)
                        throw new OWLException($"Cannot evaluate SWRL builtIn because: {nex.Message}", nex);
                    if (ex is ArgumentException aex)
                        throw new OWLException($"Cannot evaluate SWRL builtIn with IRI {IRI} because: {ex.Message}", aex);

                    /* NO-OP for every other recoverable situations */
                }

            return filteredTable;
        }
        #endregion
    }
}