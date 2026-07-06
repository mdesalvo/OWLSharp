/*
   Copyright 2014-2026 Marco De Salvo

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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using RDFSharp.Query;

namespace OWLSharp
{
    /// <summary>
    /// OWLShims collects lightweight, isofunctional wrapper types (OWLTable/OWLTableRow/OWLTableColumn) around
    /// RDFSharp's internal RDFTable/RDFTableRow/RDFTableColumn. Those RDFSharp types are internal and only
    /// friend-visible to the OWLSharp assembly (not to OWLSharp.Test), and are sealed so they cannot be subclassed:
    /// wrapping (composition) is the only way to give OWLSharp.Test a directly-referenceable, compile-time-safe
    /// equivalent, mirroring the same member names so production and test code read identically.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class OWLTable
    {
        #region Properties
        internal RDFTable Data { get; }

        internal OWLTableColumnCollection Columns
            => new OWLTableColumnCollection(Data.Columns);

        internal int ColumnsCount
            => Data.ColumnsCount;

        internal OWLTableRowCollection Rows
            => new OWLTableRowCollection(Data.Rows);

        internal int RowsCount
            => Data.RowsCount;
        #endregion

        #region Ctors
        internal OWLTable()
            => Data = new RDFTable();

        private OWLTable(RDFTable inner)
            => Data = inner;

        internal static OWLTable FromData(RDFTable data)
            => data == null ? null : new OWLTable(data);
        #endregion

        #region Methods
        internal OWLTableColumn AddColumn(string columnName, bool isSynthetic = false)
            => new OWLTableColumn(Data.AddColumn(columnName, isSynthetic));

        internal bool HasColumn(string columnName)
            => Data.HasColumn(columnName);

        internal int OrdinalOf(string columnName)
            => Data.OrdinalOf(columnName);

        internal void RemoveColumn(string columnName)
            => Data.RemoveColumn(columnName);

        internal void AddRow(IDictionary<string, string> bindings)
            => Data.AddRow(bindings);

        internal void AddRow(string[] cells)
            => Data.AddRow(cells);

        internal OWLTableRow NewRow()
            => new OWLTableRow(Data.NewRow());

        internal OWLTable Clone()
            => FromData(Data.Clone());

        internal DataTable ToDataTable()
            => Data.ToDataTable();

        internal static OWLTable FromDataTable(DataTable dataTable)
            => FromData(RDFTable.FromDataTable(dataTable));
        #endregion
    }

    /// <summary>
    /// OWLTableRow wraps a single RDFTableRow, mirroring its member names.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct OWLTableRow
    {
        internal RDFTableRow Data { get; }

        internal OWLTableRow(RDFTableRow data)
            => Data = data;

        internal string this[string column]
            => Data[column];

        internal string this[int ordinal]
            => Data[ordinal];

        internal string Signature
            => Data.Signature;

        internal bool HasColumn(string column)
            => Data.HasColumn(column);

        internal bool IsUnbound(string column)
            => Data.IsUnbound(column);

        internal bool IsBound(string column)
            => Data.IsBound(column);
    }

    /// <summary>
    /// OWLTableColumn wraps a single RDFTableColumn, mirroring its member names.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class OWLTableColumn
    {
        private readonly RDFTableColumn _data;

        internal OWLTableColumn(RDFTableColumn data)
            => _data = data;

        internal string Name
            => _data.Name;

        internal int Ordinal
            => _data.Ordinal;

        internal bool IsSynthetic
            => _data.IsSynthetic;

        public override string ToString()
            => Name;
    }

    /// <summary>
    /// OWLTableRowCollection wraps an RDFTableRowCollection, mirroring its member names (Count, int indexer,
    /// enumeration), so that Assert.HasCount/Assert.IsEmpty and foreach loops in tests keep working unchanged.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct OWLTableRowCollection : IEnumerable<OWLTableRow>
    {
        private readonly RDFTableRowCollection _data;

        internal OWLTableRowCollection(RDFTableRowCollection data)
            => _data = data;

        internal int Count
            => _data.Count;

        internal OWLTableRow this[int index]
            => new OWLTableRow(_data[index]);

        public IEnumerator<OWLTableRow> GetEnumerator()
        {
            foreach (RDFTableRow row in _data)
                yield return new OWLTableRow(row);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    /// <summary>
    /// OWLTableColumnCollection wraps the read-only column list of an RDFTable, mirroring Count/int-indexer/
    /// enumeration lazily (no eager materialization), consistently with OWLTableRowCollection.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal readonly struct OWLTableColumnCollection : IEnumerable<OWLTableColumn>
    {
        private readonly IReadOnlyList<RDFTableColumn> _data;

        internal OWLTableColumnCollection(IReadOnlyList<RDFTableColumn> data)
            => _data = data;

        internal int Count
            => _data.Count;

        internal OWLTableColumn this[int index]
            => new OWLTableColumn(_data[index]);

        public IEnumerator<OWLTableColumn> GetEnumerator()
        {
            foreach (RDFTableColumn column in _data)
                yield return new OWLTableColumn(column);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}