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
using OWLSharp.Ontology.Rules.Atoms;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules
{
    //Register here all derived types of SWRLAtom
    [XmlInclude(typeof(SWRLBuiltIn))]
    [XmlInclude(typeof(SWRLClassAtom))]
    [XmlInclude(typeof(SWRLDataPropertyAtom))]
    [XmlInclude(typeof(SWRLDataRangeAtom))]
    [XmlInclude(typeof(SWRLDifferentIndividualsAtom))]
    [XmlInclude(typeof(SWRLObjectPropertyAtom))]
    [XmlInclude(typeof(SWRLSameIndividualAtom))]
    public abstract class SWRLAtom
    {
        #region Properties
        //Register here all derived types of OWLExpression (in scope of SWRL)
        //ClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=1)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=1)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=1)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=1)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=1)]
        [XmlElement(typeof(OWLObjectSomeValuesFrom), ElementName="ObjectSomeValuesFrom", Order=1)]
        [XmlElement(typeof(OWLObjectAllValuesFrom), ElementName="ObjectAllValuesFrom", Order=1)]
        [XmlElement(typeof(OWLObjectHasValue), ElementName="ObjectHasValue", Order=1)]
        [XmlElement(typeof(OWLObjectHasSelf), ElementName="ObjectHasSelf", Order=1)]
        [XmlElement(typeof(OWLObjectMinCardinality), ElementName="ObjectMinCardinality", Order=1)]
        [XmlElement(typeof(OWLObjectMaxCardinality), ElementName="ObjectMaxCardinality", Order=1)]
        [XmlElement(typeof(OWLObjectExactCardinality), ElementName="ObjectExactCardinality", Order=1)]
        [XmlElement(typeof(OWLDataSomeValuesFrom), ElementName="DataSomeValuesFrom", Order=1)]
        [XmlElement(typeof(OWLDataAllValuesFrom), ElementName="DataAllValuesFrom", Order=1)]
        [XmlElement(typeof(OWLDataHasValue), ElementName="DataHasValue", Order=1)]
        [XmlElement(typeof(OWLDataMinCardinality), ElementName="DataMinCardinality", Order=1)]
        [XmlElement(typeof(OWLDataMaxCardinality), ElementName="DataMaxCardinality", Order=1)]
        [XmlElement(typeof(OWLDataExactCardinality), ElementName="DataExactCardinality", Order=1)]
        //DataRangeExpression
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=1)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=1)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=1)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=1)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=1)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=1)]
        //DataProperty
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        //ObjectProperty
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=1)]
        public OWLExpression Predicate { get; internal set; }

        public RDFPatternMember LeftArgument { get; internal set; }

        public RDFPatternMember RightArgument { get; internal set; }
        #endregion

        #region Ctors
        internal SWRLAtom(OWLExpression predicate, RDFPatternMember leftArgument, RDFPatternMember rightArgument)
        {
            Predicate = predicate ?? throw new OWLException("Cannot create atom because given \"predicate\" parameter is null");
            LeftArgument = leftArgument ?? throw new OWLException("Cannot create atom because given \"leftArgument\" parameter is null");
            RightArgument = rightArgument;
        }
        #endregion

		#region Interfaces
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Predicate
            sb.Append(RDFModelUtilities.GetShortUri(Predicate.GetIRI().URI));

            //Arguments
            sb.Append($"({LeftArgument}");
            if (RightArgument != null)
            {
                //When the right argument is a resource, it is printed in a SWRL-shortened form
                if (RightArgument is RDFResource rightArgumentResource)
                    sb.Append($",{RDFModelUtilities.GetShortUri(rightArgumentResource.URI)}");

                //Other cases of right argument (variable, literal) are printed in normal form
                else
                    sb.Append($",{RDFQueryPrinter.PrintPatternMember(RightArgument, RDFNamespaceRegister.Instance.Register)}");
            }
            sb.Append(")");

            return sb.ToString();
        }
        #endregion

        #region Methods
        internal abstract DataTable EvaluateOnAntecedent(OWLOntology ontology);

        internal abstract List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology);
        #endregion
    }
}