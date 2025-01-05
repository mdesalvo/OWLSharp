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
    //Register here all derived types of OWLExpression
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
        [XmlIgnore]
        internal RDFResource ExpressionIRI { get; set; }
        #endregion

        #region Ctors
        internal OWLExpression()
            => ExpressionIRI = new RDFResource($"bnode:ex{Guid.NewGuid():N}");
        #endregion

        #region Methods
        public virtual RDFResource GetIRI()
            => ExpressionIRI;

        public virtual string ToSWRLString() 
            => RDFModelUtilities.GetShortUri(GetIRI().URI);

        internal virtual RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
            => new RDFGraph();
        #endregion
    }

    //Derived

    public class OWLAnnotationPropertyExpression : OWLExpression { }

    public class OWLClassExpression : OWLExpression 
    {
        #region Properties
        [XmlIgnore]
        public bool IsClass => this is OWLClass;

        [XmlIgnore]
        public bool IsComposite => this is OWLObjectUnionOf
                                    || this is OWLObjectIntersectionOf
                                    || this is OWLObjectComplementOf;

        [XmlIgnore]
        public bool IsEnumerate => this is OWLObjectOneOf;

        [XmlIgnore]
        public bool IsObjectRestriction => this is OWLObjectAllValuesFrom
                                            || this is OWLObjectSomeValuesFrom
                                            || this is OWLObjectHasValue
                                            || this is OWLObjectHasSelf
                                            || this is OWLObjectExactCardinality
                                            || this is OWLObjectMinCardinality
                                            || this is OWLObjectMaxCardinality;

        [XmlIgnore]
        public bool IsDataRestriction => this is OWLDataAllValuesFrom
                                          || this is OWLDataSomeValuesFrom
                                          || this is OWLDataHasValue
                                          || this is OWLDataExactCardinality
                                          || this is OWLDataMinCardinality
                                          || this is OWLDataMaxCardinality;
        #endregion
    }

    public class OWLDataRangeExpression : OWLExpression 
    {
        #region Properties
        [XmlIgnore]
        public bool IsDatatype => this is OWLDatatype;

        [XmlIgnore]
        public bool IsComposite => this is OWLDataUnionOf
                                    || this is OWLDataIntersectionOf
                                    || this is OWLDataComplementOf;

        [XmlIgnore]
        public bool IsEnumerate => this is OWLDataOneOf;

        [XmlIgnore]
        public bool IsDatatypeRestriction => this is OWLDatatypeRestriction;
        #endregion
    }

    public class OWLDataPropertyExpression : OWLExpression { }

    public class OWLObjectPropertyExpression : OWLExpression { }

    public class OWLIndividualExpression : OWLExpression { }

    public class OWLLiteralExpression : OWLExpression { }

    //Entity

    public interface IOWLEntity { }
}