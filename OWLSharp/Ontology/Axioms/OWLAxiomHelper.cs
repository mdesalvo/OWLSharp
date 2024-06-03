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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Ontology.Axioms
{
    public static class OWLAxiomHelper 
    {
        #region Methods

        //Declarations
        public static List<OWLDeclaration> GetDeclarationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLExpression, IOWLEntity
            => ontology?.DeclarationAxioms.Where(ax => ax.Expression is T).ToList() ?? new List<OWLDeclaration>();

        //ClassAxioms
        public static List<T> GetClassAxiomsOfType<T>(this OWLOntology ontology) where T : OWLClassAxiom
            => ontology?.ClassAxioms.OfType<T>().ToList() ?? new List<T>();

        //DataPropertyAxioms
        public static List<T> GetDataPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLDataPropertyAxiom
            => ontology?.DataPropertyAxioms.OfType<T>().ToList() ?? new List<T>();

        //ObjectPropertyAxioms
        public static List<T> GetObjectPropertyAxiomsOfType<T>(this OWLOntology ontology) where T : OWLObjectPropertyAxiom
            => ontology?.ObjectPropertyAxioms.OfType<T>().ToList() ?? new List<T>();

        //AnnotationAxioms
        public static List<T> GetAnnotationAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAnnotationAxiom
            => ontology?.AnnotationAxioms.OfType<T>().ToList() ?? new List<T>();

        //AssertionAxioms
        public static List<T> GetAssertionAxiomsOfType<T>(this OWLOntology ontology) where T : OWLAssertionAxiom
            => ontology?.AssertionAxioms.OfType<T>().ToList() ?? new List<T>();

        #endregion
    }
}