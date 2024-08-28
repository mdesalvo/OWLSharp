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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules.Atoms
{
    [XmlRoot("BuiltInAtom")]
    public class SWRLBuiltInAtom : SWRLAtom
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }

        [XmlIgnore]
        internal double MathValue { get; set; }

        [XmlIgnore]
        public bool IsMathBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#abs")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#add")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#ceiling")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#cos")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#divide")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#floor")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#multiply")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#pow")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#round")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#roundHalfToEven")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#sin")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#subtract")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#tan");

        [XmlIgnore]
        internal RDFFilter FilterValue { get; set; }

        [XmlIgnore]
        internal bool IsComparisonFilterBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#equal")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#greaterThan")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#greaterThanOrEqual")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#lessThan")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#lessThanOrEqual")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#notEqual");

        [XmlIgnore]
        internal bool IsStringFilterBuiltIn
            => string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#contains")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#containsIgnoreCase")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#endsWith")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#matches")
                || string.Equals(IRI, "http://www.w3.org/2003/11/swrlb#startsWith");
        #endregion

        #region Ctors
        internal SWRLBuiltInAtom() { }
        #endregion

        #region Methods
        internal DataTable EvaluateOnAntecedent(DataTable antecedentResults, OWLOntology ontology)
        {
            if (IsMathBuiltIn)
            {
                #region MathBuiltIn
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
                                //Execute the built-in's math logics
                                bool keepRow = false;
                                switch (IRI)
                                {
                                    case "http://www.w3.org/2003/11/swrlb#abs":
                                        keepRow = (leftArgumentNumericValue == Math.Abs(rightArgumentNumericValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#add":
                                        keepRow = (leftArgumentNumericValue == rightArgumentNumericValue + MathValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#ceiling":
                                        keepRow = (leftArgumentNumericValue == Math.Ceiling(rightArgumentNumericValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#cos":
                                        keepRow = (leftArgumentNumericValue == Math.Cos(rightArgumentNumericValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#divide":
                                        keepRow = (leftArgumentNumericValue == rightArgumentNumericValue / MathValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#floor":
                                        keepRow = (leftArgumentNumericValue == Math.Floor(rightArgumentNumericValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#multiply":
                                        keepRow = (leftArgumentNumericValue == rightArgumentNumericValue * MathValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#pow":
                                        keepRow = (leftArgumentNumericValue == Math.Pow(rightArgumentNumericValue, MathValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#round":
                                        keepRow = (leftArgumentNumericValue == Math.Round(rightArgumentNumericValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#roundHalfToEven":
                                        keepRow = (leftArgumentNumericValue == Math.Round(rightArgumentNumericValue, MidpointRounding.ToEven));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#sin":
                                        keepRow = (leftArgumentNumericValue == Math.Sin(rightArgumentNumericValue));
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#subtract":
                                        keepRow = (leftArgumentNumericValue == rightArgumentNumericValue - MathValue);
                                        break;
                                    case "http://www.w3.org/2003/11/swrlb#tan":
                                        keepRow = (leftArgumentNumericValue == Math.Tan(rightArgumentNumericValue));
                                        break;
                                }

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
                #endregion
            }
            else if (IsComparisonFilterBuiltIn || IsStringFilterBuiltIn)
            {
                #region FilterBuiltIn
                DataTable filteredTable = antecedentResults.Clone();

                //Iterate the rows of the antecedent result table
                IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();
                while (rowsEnum.MoveNext())
                {
                    //Apply the built-in filter on the row
                    bool keepRow = FilterValue.ApplyFilter((DataRow)rowsEnum.Current, false);

                    //If the row has passed the filter, keep it in the filtered result table
                    if (keepRow)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                        filteredTable.Rows.Add(newRow);
                    }
                }

                return filteredTable;
                #endregion
            }
            throw new OWLException($"Cannot evaluate unsupported configuration of SWRL built-in: unknown predicate '{Predicate}'");
        }

        //Derived from SRWLAtom
        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology) => null;
        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology) => null;
        #endregion
    }
}