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
using System.Linq;

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

        public static SWRLBuiltIn Abs(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#abs",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:abs builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:abs builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Add(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#add",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:add builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:add builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn BooleanNot(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#booleanNot",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:booleanNot builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:booleanNot builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Ceiling(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#ceiling",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:ceiling builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:ceiling builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Cos(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#cos",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:cos builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:cos builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Floor(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#floor",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:floor builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:floor builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Multiply(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#multiply",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:multiply builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:multiply builtIn because: right arguments are null")).ToList()
                };

        public static SWRLBuiltIn Sin(SWRLArgument leftArg, SWRLArgument rightArg)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#sin",
                    Arguments = new List<SWRLArgument>()
                    {
                        leftArg ?? throw new OWLException("Cannot create swrlb:sin builtIn because: left argument is null"),
                        rightArg ?? throw new OWLException("Cannot create swrlb:sin builtIn because: right argument is null")
                    }
                };

        public static SWRLBuiltIn Subtract(SWRLArgument leftArg, params SWRLArgument[] rightArgs)
            =>  new SWRLBuiltIn()
                {
                    IRI = "http://www.w3.org/2003/11/swrlb#subtract",
                    Arguments = Enumerable.Concat(
                        new List<SWRLArgument>() { leftArg ?? throw new OWLException("Cannot create swrlb:subtract builtIn because: left argument is null") },
                        rightArgs?.ToList() ?? throw new OWLException("Cannot create swrlb:subtract builtIn because: right arguments are null")).ToList()
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
                        case "http://www.w3.org/2003/11/swrlb#add":
                            keepRow = SWRLAddBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#booleanNot":
                            keepRow = SWRLBooleanNotBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#ceiling":
                            keepRow = SWRLCeilingBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#cos":
                            keepRow = SWRLCosBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#floor":
                            keepRow = SWRLFloorBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#multiply":
                            keepRow = SWRLMultiplyBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#sin":
                            keepRow = SWRLSinBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
                            break;
                        case "http://www.w3.org/2003/11/swrlb#subtract":
                            keepRow = SWRLSubtractBuiltIn.EvaluateOnAntecedent(currentRow, Arguments);
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
                    /* This exception is for unsupported (or not yet implemented) builtIns */
                    if (ex is NotImplementedException nex)
                        throw new OWLException($"Cannot evaluate SWRL builtIn because: {nex.Message}", nex);
                    /* This exception is for builtIns violating required n-arity of arguments */
                    if (ex is ArgumentException aex)
                        throw new OWLException($"Cannot evaluate SWRL builtIn with IRI {IRI} because: {ex.Message}", aex);

                    /* NO-OP for every other recoverable situations */
                }

            return filteredTable;
        }
        #endregion
    }
}