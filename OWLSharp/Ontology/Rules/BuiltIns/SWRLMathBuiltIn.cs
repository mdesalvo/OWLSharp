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

using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules.BuiltIns
{
    //Register here all derived types of SWRLMathBuiltIn
    [XmlInclude(typeof(SWRLAbsBuiltIn))]
    [XmlInclude(typeof(SWRLAddBuiltIn))]
    [XmlInclude(typeof(SWRLCeilingBuiltIn))]
    [XmlInclude(typeof(SWRLCosBuiltIn))]
    [XmlInclude(typeof(SWRLDivideBuiltIn))]
    [XmlInclude(typeof(SWRLFloorBuiltIn))]
    [XmlInclude(typeof(SWRLMultiplyBuiltIn))]
    [XmlInclude(typeof(SWRLPowBuiltIn))]
    [XmlInclude(typeof(SWRLRoundBuiltIn))]
    [XmlInclude(typeof(SWRLRoundHalfToEvenBuiltIn))]
    [XmlInclude(typeof(SWRLSinBuiltIn))]
    [XmlInclude(typeof(SWRLSubtractBuiltIn))]
    [XmlInclude(typeof(SWRLTanBuiltIn))]
    public abstract class SWRLMathBuiltIn : SWRLBuiltIn
    {
        #region Properties
        [XmlIgnore]
        internal double MathValue { get; set; }
        #endregion

        #region Ctors
        internal SWRLMathBuiltIn(OWLExpression predicate, SWRLArgument leftArgument, SWRLArgument rightArgument, double mathValue)
            : base(predicate, leftArgument, rightArgument) => MathValue = mathValue;
        #endregion

		#region Interfaces
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Predicate
            sb.Append(RDFModelUtilities.GetShortUri(Predicate.GetIRI().URI));

            //Arguments
            RDFTypedLiteral mathValueTypedLiteral = new RDFTypedLiteral(MathValue.ToString(), RDFModelEnums.RDFDatatypes.XSD_DOUBLE);
            sb.Append($"({LeftArgument},{RightArgument},{RDFQueryPrinter.PrintPatternMember(mathValueTypedLiteral, RDFNamespaceRegister.Instance.Register)})");

            return sb.ToString();
        }
        #endregion

        #region Methods
        internal override DataTable EvaluateOnAntecedent(DataTable antecedentResults, OWLOntology ontology)
        {
            DataTable filteredTable = antecedentResults.Clone();

            #region Guards
            //Preliminary checks for built-in's applicability (requires arguments to be known variables)
            string leftArgumentString = LeftArgument.ToString();
            if (!antecedentResults.Columns.Contains(leftArgumentString))
                return filteredTable;
            string rightArgumentString = RightArgument.ToString();
            if (!antecedentResults.Columns.Contains(rightArgumentString))
                return filteredTable;
            #endregion

            //Iterate the rows of the antecedent result table
            IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
            while (rowsEnum.MoveNext())
            {
                try
                {
                    //Fetch data corresponding to the built-in's arguments
                    DataRow currentRow = (DataRow)rowsEnum.Current;
                    string leftArgumentValue = currentRow[leftArgumentString].ToString();
                    string rightArgumentValue = currentRow[rightArgumentString].ToString();

                    //Transform fetched data to pattern members
                    RDFPatternMember leftArgumentPMember = RDFQueryUtilities.ParseRDFPatternMember(leftArgumentValue);
                    RDFPatternMember rightArgumentPMember = RDFQueryUtilities.ParseRDFPatternMember(rightArgumentValue);

                    //Check compatibility of pattern members with the built-in (requires numeric typed literals)
                    if (leftArgumentPMember is RDFTypedLiteral leftArgumentTypedLiteral
                         && leftArgumentTypedLiteral.HasDecimalDatatype()
                         && rightArgumentPMember is RDFTypedLiteral rightArgumentTypedLiteral
                         && rightArgumentTypedLiteral.HasDecimalDatatype())
                    {
                        if (double.TryParse(leftArgumentTypedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double leftArgumentNumericValue)
                             && double.TryParse(rightArgumentTypedLiteral.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double rightArgumentNumericValue))
                        {
                            //Execute the built-in's comparison logics
                            bool keepRow = false;
                            if (this is SWRLAddBuiltIn)
                                keepRow = (leftArgumentNumericValue == rightArgumentNumericValue + MathValue);
                            else if (this is SWRLSubtractBuiltIn)
                                keepRow = (leftArgumentNumericValue == rightArgumentNumericValue - MathValue);
                            else if (this is SWRLMultiplyBuiltIn)
                                keepRow = (leftArgumentNumericValue == rightArgumentNumericValue * MathValue);
                            else if (this is SWRLDivideBuiltIn)
                                keepRow = (leftArgumentNumericValue == rightArgumentNumericValue / MathValue);
                            else if (this is SWRLPowBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Pow(rightArgumentNumericValue, MathValue));
                            else if (this is SWRLAbsBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Abs(rightArgumentNumericValue));
                            else if (this is SWRLFloorBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Floor(rightArgumentNumericValue));
                            else if (this is SWRLCeilingBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Ceiling(rightArgumentNumericValue));
                            else if (this is SWRLRoundBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Round(rightArgumentNumericValue));
                            else if (this is SWRLRoundHalfToEvenBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Round(rightArgumentNumericValue, MidpointRounding.ToEven));
                            else if (this is SWRLSinBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Sin(rightArgumentNumericValue));
                            else if (this is SWRLTanBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Tan(rightArgumentNumericValue));
                            else if (this is SWRLCosBuiltIn)
                                keepRow = (leftArgumentNumericValue == Math.Cos(rightArgumentNumericValue));

                            //If the row has passed the built-in, keep it in the filtered result table
                            if (keepRow)
                            {
                                DataRow newRow = filteredTable.NewRow();
                                newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                                filteredTable.Rows.Add(newRow);
                            }
                        }
                    }
                }
                catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
            }

            return filteredTable;
        }
        #endregion
    }
}