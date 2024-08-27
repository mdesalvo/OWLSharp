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
using RDFSharp.Query;
using System.Collections;
using System.Data;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules.BuiltIns
{
    //Register here all derived types of SWRLFilterBuiltIn
    [XmlInclude(typeof(SWRLContainsBuiltIn))]
    [XmlInclude(typeof(SWRLContainsIgnoreCaseBuiltIn))]
    [XmlInclude(typeof(SWRLEndsWithBuiltIn))]
    [XmlInclude(typeof(SWRLEqualBuiltIn))]
    [XmlInclude(typeof(SWRLGreaterThanBuiltIn))]
    [XmlInclude(typeof(SWRLGreaterThanOrEqualBuiltIn))]
    [XmlInclude(typeof(SWRLLessThanBuiltIn))]
    [XmlInclude(typeof(SWRLLessThanOrEqualBuiltIn))]
    [XmlInclude(typeof(SWRLMatchesBuiltIn))]
    [XmlInclude(typeof(SWRLNotEqualBuiltIn))]
    [XmlInclude(typeof(SWRLStartsWithBuiltIn))]
    public abstract class SWRLFilterBuiltIn : SWRLBuiltIn
    {
        #region Properties
        [XmlIgnore]
        internal RDFFilter BuiltInFilter { get; set; }
        #endregion

        #region Ctors
        internal SWRLFilterBuiltIn(OWLExpression predicate, SWRLArgument leftArgument, SWRLArgument rightArgument)
            : base(predicate, leftArgument, rightArgument) { }
        #endregion

        #region Methods
        internal override DataTable EvaluateOnAntecedent(DataTable antecedentResults, OWLOntology ontology)
        {
            DataTable filteredTable = antecedentResults.Clone();
            IEnumerator rowsEnum = antecedentResults.Rows.GetEnumerator();

            //Iterate the rows of the antecedent result table
            while (rowsEnum.MoveNext())
            {
                //Apply the built-in filter on the row
                bool keepRow = BuiltInFilter.ApplyFilter((DataRow)rowsEnum.Current, false);

                //If the row has passed the filter, keep it in the filtered result table
                if (keepRow)
                {
                    DataRow newRow = filteredTable.NewRow();
                    newRow.ItemArray = ((DataRow)rowsEnum.Current).ItemArray;
                    filteredTable.Rows.Add(newRow);
                }
            }

            return filteredTable;
        }
        #endregion
    }
}