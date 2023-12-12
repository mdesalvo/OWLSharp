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
using System.Collections;
using System.Data;

namespace OWLSharp.Extensions.SWRL
{
    /// <summary>
    /// SWRLFilterBuiltIn represents a specific category of SWRL built-in filtering inferences of a rule's antecedent on a string basis
    /// </summary>
    public abstract class SWRLFilterBuiltIn : SWRLBuiltIn
    {
        #region Properties
        /// <summary>
        /// Represents the SWRL built-in equivalent SPARQL filter
        /// </summary>
        internal RDFFilter BuiltInFilter { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a SWRL built-in with given predicate and arguments
        /// </summary>
        internal SWRLFilterBuiltIn(RDFResource predicate, RDFPatternMember leftArgument, RDFPatternMember rightArgument)
            : base(predicate, leftArgument, rightArgument) { }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluates the SWRL built-in in the context of the given antecedent results
        /// </summary>
        internal override DataTable Evaluate(DataTable antecedentResults, OWLOntology ontology)
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