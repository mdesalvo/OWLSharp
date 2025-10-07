/*
   Copyright 2014-2025 Marco De Salvo

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
using System;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLExpression represents the foundational aspect for building OWL ontologies, contributing to the modeling of both domain entities
    /// (classes, individuals, datatypes, typed properties) and more expressive semantic aggregations (axioms)
    /// </summary>
    [XmlInclude(typeof(OWLAnnotationProperty))]
    [XmlInclude(typeof(OWLAnonymousIndividual))]
    [XmlInclude(typeof(OWLClass))]
    [XmlInclude(typeof(OWLDataAllValuesFrom))]
    [XmlInclude(typeof(OWLDataComplementOf))]
    [XmlInclude(typeof(OWLDataExactCardinality))]
    [XmlInclude(typeof(OWLDataHasValue))]
    [XmlInclude(typeof(OWLDataIntersectionOf))]
    [XmlInclude(typeof(OWLDataMaxCardinality))]
    [XmlInclude(typeof(OWLDataMinCardinality))]
    [XmlInclude(typeof(OWLDataOneOf))]
    [XmlInclude(typeof(OWLDataProperty))]
    [XmlInclude(typeof(OWLDataSomeValuesFrom))]
    [XmlInclude(typeof(OWLDataUnionOf))]
    [XmlInclude(typeof(OWLDatatype))]
    [XmlInclude(typeof(OWLDatatypeRestriction))]
    [XmlInclude(typeof(OWLLiteral))]
    [XmlInclude(typeof(OWLNamedIndividual))]
    [XmlInclude(typeof(OWLObjectAllValuesFrom))]
    [XmlInclude(typeof(OWLObjectComplementOf))]
    [XmlInclude(typeof(OWLObjectExactCardinality))]
    [XmlInclude(typeof(OWLObjectHasSelf))]
    [XmlInclude(typeof(OWLObjectHasValue))]
    [XmlInclude(typeof(OWLObjectIntersectionOf))]
    [XmlInclude(typeof(OWLObjectInverseOf))]
    [XmlInclude(typeof(OWLObjectMaxCardinality))]
    [XmlInclude(typeof(OWLObjectMinCardinality))]
    [XmlInclude(typeof(OWLObjectOneOf))]
    [XmlInclude(typeof(OWLObjectProperty))]
    [XmlInclude(typeof(OWLObjectSomeValuesFrom))]
    [XmlInclude(typeof(OWLObjectUnionOf))]
    public class OWLExpression
    {
        #region Properties
        /// <summary>
        /// The IRI of the expressions
        /// </summary>
        [XmlIgnore]
        internal RDFResource ExpressionIRI { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a blank expression
        /// </summary>
        internal OWLExpression()
            => ExpressionIRI = new RDFResource($"bnode:ex{Guid.NewGuid():N}");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the IRI representation of this expression
        /// </summary>
        public virtual RDFResource GetIRI()
            => ExpressionIRI;

        /// <summary>
        /// Gets the SWRL representation of this expression
        /// </summary>
        public virtual string ToSWRLString()
            => GetIRI().URI.GetShortUri();

        /// <summary>
        /// Exports this expression to an equivalent RDFGraph object
        /// </summary>
        internal virtual RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
            => new RDFGraph();
        #endregion
    }

    //Derived

    /// <summary>
    /// OWLAnnotationPropertyExpression is an expression suited for modeling annotation properties
    /// </summary>
    public class OWLAnnotationPropertyExpression : OWLExpression { }

    /// <summary>
    /// OWLClassExpression is an expression suited for modeling both domain classes and more expressive class aggregations
    /// </summary>
    public class OWLClassExpression : OWLExpression
    {
        #region Properties
        /// <summary>
        /// Indicates that this class expression is a domain entity
        /// </summary>
        [XmlIgnore]
        public bool IsClass => this is OWLClass;

        /// <summary>
        /// Indicates that this class expression is an algebraic composition of other class expressions
        /// </summary>
        [XmlIgnore]
        public bool IsComposite => this is OWLObjectUnionOf
                                    || this is OWLObjectIntersectionOf
                                    || this is OWLObjectComplementOf;

        /// <summary>
        /// Indicates that this class expression is an enumeration of individual expressions
        /// </summary>
        [XmlIgnore]
        public bool IsEnumerate => this is OWLObjectOneOf;

        /// <summary>
        /// Indicates that this class expression is a constraint applied on an object property expression
        /// </summary>
        [XmlIgnore]
        public bool IsObjectRestriction => this is OWLObjectAllValuesFrom
                                            || this is OWLObjectSomeValuesFrom
                                            || this is OWLObjectHasValue
                                            || this is OWLObjectHasSelf
                                            || this is OWLObjectExactCardinality
                                            || this is OWLObjectMinCardinality
                                            || this is OWLObjectMaxCardinality;

        /// <summary>
        /// Indicates that this class expression is a constraint applied on a data property
        /// </summary>
        [XmlIgnore]
        public bool IsDataRestriction => this is OWLDataAllValuesFrom
                                          || this is OWLDataSomeValuesFrom
                                          || this is OWLDataHasValue
                                          || this is OWLDataExactCardinality
                                          || this is OWLDataMinCardinality
                                          || this is OWLDataMaxCardinality;
        #endregion
    }

    /// <summary>
    /// OWLDataRangeExpression is an expression suited for modeling both domain datatypes and more expressive data aggregations
    /// </summary>
    public class OWLDataRangeExpression : OWLExpression
    {
        #region Properties
        /// <summary>
        /// Indicates that this datarange expression is a domain datatype
        /// </summary>
        [XmlIgnore]
        public bool IsDatatype => this is OWLDatatype;

        /// <summary>
        /// Indicates that this datarange expression is an algebraic composition of other datarange expressions
        /// </summary>
        [XmlIgnore]
        public bool IsComposite => this is OWLDataUnionOf
                                    || this is OWLDataIntersectionOf
                                    || this is OWLDataComplementOf;

        /// <summary>
        /// Indicates that this datarange expression is an enumeration of literal expressions
        /// </summary>
        [XmlIgnore]
        public bool IsEnumerate => this is OWLDataOneOf;

        /// <summary>
        /// Indicates that this datarange expression is a custom datatype definition
        /// </summary>
        [XmlIgnore]
        public bool IsDatatypeRestriction => this is OWLDatatypeRestriction;
        #endregion
    }

    /// <summary>
    /// OWLDataPropertyExpression is an expression suited for modeling data properties
    /// </summary>
    public class OWLDataPropertyExpression : OWLExpression { }

    /// <summary>
    /// OWLObjectPropertyExpression is an expression suited for modeling object properties
    /// </summary>
    public class OWLObjectPropertyExpression : OWLExpression { }

    /// <summary>
    /// OWLIndividualExpression is an expression suited for modeling individuals
    /// </summary>
    public class OWLIndividualExpression : OWLExpression { }

    /// <summary>
    /// OWLLiteralExpression is an expression suited for modeling literals
    /// </summary>
    public class OWLLiteralExpression : OWLExpression { }

    //Entity

    /// <summary>
    /// IOWLEntity represents the basic domain entities (classes, datatypes, annotation/data/object properties, individuals)
    /// </summary>
    public interface IOWLEntity { }
}