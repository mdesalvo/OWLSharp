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
using System;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Expressions
{
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
        internal bool IsClass => this is OWLClass;

        [XmlIgnore]
        internal bool IsComposite => this is OWLObjectUnionOf
                                      || this is OWLObjectIntersectionOf
                                      || this is OWLObjectComplementOf;

        [XmlIgnore]
        internal bool IsEnumerate => this is OWLObjectOneOf;

        [XmlIgnore]
        internal bool IsObjectRestriction => this is OWLObjectAllValuesFrom
                                              || this is OWLObjectSomeValuesFrom
                                              || this is OWLObjectHasValue
                                              || this is OWLObjectHasSelf
                                              || this is OWLObjectExactCardinality
                                              || this is OWLObjectMinCardinality
                                              || this is OWLObjectMaxCardinality;

        [XmlIgnore]
        internal bool IsDataRestriction => this is OWLDataAllValuesFrom
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
        internal bool IsDatatype => this is OWLDatatype;

        [XmlIgnore]
        internal bool IsComposite => this is OWLDataUnionOf
                                      || this is OWLDataIntersectionOf
                                      || this is OWLDataComplementOf;

        [XmlIgnore]
        internal bool IsEnumerate => this is OWLDataOneOf;

        [XmlIgnore]
        internal bool IsDatatypeRestriction => this is OWLDatatypeRestriction;
        #endregion
    }

    public class OWLDataPropertyExpression : OWLExpression { }

    public class OWLObjectPropertyExpression : OWLExpression { }

    public class OWLIndividualExpression : OWLExpression { }

    public class OWLLiteralExpression : OWLExpression { }

    //Entity

    public interface IOWLEntity { }
}