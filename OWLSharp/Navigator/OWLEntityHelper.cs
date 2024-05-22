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

using OWLSharp.Modeler;
using OWLSharp.Modeler.Expressions;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Navigator
{
    public static class OWLEntityHelper
    {
        internal static List<RDFResource> EmptyList 
            => Enumerable.Empty<RDFResource>().ToList();

        #region Methods
        public static List<RDFResource> GetDeclaredClasses(this OWLOntology ontology)
            => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLClass daxCls)
                                          .Select(dax => ((OWLClass)dax.Expression).GetIRI())
                                          .ToList() ?? EmptyList;
        public static List<RDFResource> GetDeclaredDatatypes(this OWLOntology ontology)
           => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLDatatype daxDtt)
                                          .Select(dax => ((OWLDatatype)dax.Expression).GetIRI())
                                          .ToList() ?? EmptyList;
        #endregion
    }
}