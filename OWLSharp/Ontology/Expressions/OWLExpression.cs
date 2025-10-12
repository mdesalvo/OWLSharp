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
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLExpression represents a syntactic construct that denotes an entity, class, property, or data range within the ontology,
    /// formed by combining basic elements (like classes, properties, individuals, literals) with constructors and operators.
    /// Expressions can range from simple atomic entities (like a named class or property) to complex nested structures
    /// (like class expressions combining restrictions and Boolean operators), providing the compositional building blocks
    /// for defining ontological knowledge.
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
        /// The IRI of the expression
        /// </summary>
        [XmlIgnore]
        internal RDFResource ExpressionIRI { get; set; }
        #endregion

        #region Ctors
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
    /// OWLAnnotationPropertyExpression identifies an annotation property used to attach metadata to ontology elements.
    /// Unlike class or property expressions which can be complex and nested, annotation property expressions are always
    /// atomic references to annotation properties (like rdfs:label or rdfs:comment), as OWL2 does not support
    /// complex constructors for annotation properties.
    /// </summary>
    public class OWLAnnotationPropertyExpression : OWLExpression { }

    /// <summary>
    /// OWLClassExpression is a syntactic construct that denotes a set of individuals, either through a simple named class
    /// (identified by an IRI) or through complex descriptions built using class constructors and restrictions.
    /// Class expressions can range from atomic classes like "Person" to complex combinations using operators
    /// like ObjectIntersectionOf, ObjectSomeValuesFrom, or cardinality restrictions, allowing flexible and expressive
    /// definitions of classes based on properties, relationships, and logical combinations.
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
    /// OWLDataRangeExpression is a syntactic construct that denotes a set of literal values, either through a simple named datatype
    /// (like xsd:integer) or through complex descriptions built using datatype constructors.
    /// Data range expressions can range from atomic datatypes to complex combinations using operators like
    /// DataIntersectionOf, DataUnionOf, DataComplementOf, DataOneOf, or DatatypeRestriction, allowing flexible and expressive
    /// definitions of the allowed literal values for datatype properties.
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
    /// OWLDataPropertyExpression identifies a datatype property relating individuals to literal values.
    /// Unlike object properties which support inverse constructs, datatype property expressions are always atomic references
    /// to named datatype properties (like hasAge or hasName), as OWL2 does not provide complex constructors
    /// or anonymous inverses for datatype properties.
    /// </summary>
    public class OWLDataPropertyExpression : OWLExpression { }

    /// <summary>
    /// OWLObjectPropertyExpression is a syntactic construct that denotes a binary relation between individuals, either through
    /// a simple named object property (identified by an IRI) or through an anonymous inverse property using ObjectInverseOf.
    /// For example, both hasParent and ObjectInverseOf(hasChild) are object property expressions, allowing you to reference
    /// relationships in either direction without requiring explicit declaration of both the property and its inverse.
    /// </summary>
    public class OWLObjectPropertyExpression : OWLExpression { }

    /// <summary>
    /// OWLIndividualExpression is a syntactic construct that denotes a specific instance in the ontology, either through
    /// a named individual (identified by an IRI) or through an anonymous individual (a blank node without a global identifier).
    /// Individual expressions serve as the subjects and objects of assertions, allowing you to reference concrete entities
    /// in axioms and property assertions regardless of whether they have explicit names.
    /// </summary>
    public class OWLIndividualExpression : OWLExpression { }

    /// <summary>
    /// OWLLiteralExpression is a syntactic construct that denotes a concrete data value, consisting of a lexical form (the string representation),
    /// a datatype IRI (like xsd:string or xsd:integer), and optionally a language tag for text values.
    /// Literal expressions represent the actual data values that appear as objects of datatype property assertions,
    /// providing the terminal values in the ontology's data layer (e.g., "42"^^xsd:integer or "hello"@en).
    /// </summary>
    public class OWLLiteralExpression : OWLExpression { }

    //Entity

    /// <summary>
    /// IOWLEntity represents a named component of the ontology identified by an IRI, encompassing the fundamental building blocks:
    /// classes, object properties, datatype properties, annotation properties, named individuals, and datatypes.
    /// Entities form the vocabulary of the ontology and serve as the atomic, reusable elements that can be referenced, defined,
    /// and related through axioms, providing the stable identifiers that give structure and meaning to the knowledge base.
    /// </summary>
    public interface IOWLEntity
    {
        #region Properties
        /// <summary>
        /// The IRI of the entity
        /// </summary>
        string IRI { get; set; }

        /// <summary>
        /// The xsd:qualifiedName representation of the entity
        /// </summary>
        XmlQualifiedName AbbreviatedIRI { get; set; }
        #endregion
    }
}