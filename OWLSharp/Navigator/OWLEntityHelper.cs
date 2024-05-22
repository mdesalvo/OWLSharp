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
        internal static List<RDFResource> EmptyList => Enumerable.Empty<RDFResource>().ToList();

        #region Methods
        public static List<RDFResource> GetClasses(this OWLOntology ontology)
            => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLClass)
                                          .Select(dax => ((OWLClass)dax.Expression).GetIRI())
                                          .ToList() ?? EmptyList;

        public static bool CheckHasClass(this OWLOntology ontology, RDFResource classIRI)
            => ontology?.DeclarationAxioms.Any(dax => dax.Expression is OWLClass daxCls && daxCls.GetIRI().Equals(classIRI)) ?? false;

        public static List<RDFResource> GetDatatypes(this OWLOntology ontology)
           => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLDatatype)
                                         .Select(dax => ((OWLDatatype)dax.Expression).GetIRI())
                                         .ToList() ?? EmptyList;

        public static bool CheckHasDatatype(this OWLOntology ontology, RDFResource datatypeIRI)
            => ontology?.DeclarationAxioms.Any(dax => dax.Expression is OWLDatatype daxDtt && daxDtt.GetIRI().Equals(datatypeIRI)) ?? false;

        public static List<RDFResource> GetDataProperties(this OWLOntology ontology)
           => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLDataProperty)
                                         .Select(dax => ((OWLDataProperty)dax.Expression).GetIRI())
                                         .ToList() ?? EmptyList;

        public static bool CheckHasDataProperty(this OWLOntology ontology, RDFResource dataPropertyIRI)
            => ontology?.DeclarationAxioms.Any(dax => dax.Expression is OWLDataProperty daxDtp && daxDtp.GetIRI().Equals(dataPropertyIRI)) ?? false;

        public static List<RDFResource> GetObjectProperties(this OWLOntology ontology)
          => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLObjectProperty)
                                        .Select(dax => ((OWLObjectProperty)dax.Expression).GetIRI())
                                        .ToList() ?? EmptyList;

        public static bool CheckHasObjectProperty(this OWLOntology ontology, RDFResource objectPropertyIRI)
            => ontology?.DeclarationAxioms.Any(dax => dax.Expression is OWLObjectProperty daxObp && daxObp.GetIRI().Equals(objectPropertyIRI)) ?? false;

        public static List<RDFResource> GetAnnotationProperties(this OWLOntology ontology)
           => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLAnnotationProperty)
                                         .Select(dax => ((OWLAnnotationProperty)dax.Expression).GetIRI())
                                         .ToList() ?? EmptyList;

        public static bool CheckHasAnnotationProperty(this OWLOntology ontology, RDFResource annotationPropertyIRI)
            => ontology?.DeclarationAxioms.Any(dax => dax.Expression is OWLAnnotationProperty daxAnp && daxAnp.GetIRI().Equals(annotationPropertyIRI)) ?? false;

        public static List<RDFResource> GetNamedIndividuals(this OWLOntology ontology)
          => ontology?.DeclarationAxioms.Where(dax => dax.Expression is OWLNamedIndividual)
                                        .Select(dax => ((OWLNamedIndividual)dax.Expression).GetIRI())
                                        .ToList() ?? EmptyList;

        public static bool CheckHasNamedIndividual(this OWLOntology ontology, RDFResource namedIndividualIRI)
            => ontology?.DeclarationAxioms.Any(dax => dax.Expression is OWLNamedIndividual daxIdv && daxIdv.GetIRI().Equals(namedIndividualIRI)) ?? false;
        #endregion
    }
}