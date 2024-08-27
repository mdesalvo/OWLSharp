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
using OWLSharp.Ontology.Rules.Arguments;
using OWLSharp.Ontology.Rules.Atoms;
using OWLSharp.Ontology.Rules.BuiltIns;
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
    [XmlInclude(typeof(SWRLClassAtom))]
    [XmlInclude(typeof(SWRLDataPropertyAtom))]
    [XmlInclude(typeof(SWRLDataRangeAtom))]
    [XmlInclude(typeof(SWRLDifferentIndividualsAtom))]
    [XmlInclude(typeof(SWRLObjectPropertyAtom))]
    [XmlInclude(typeof(SWRLSameIndividualAtom))]
    //Register here all derived types of SWRLBuiltIn
    [XmlInclude(typeof(SWRLAbsBuiltIn))]
    [XmlInclude(typeof(SWRLAddBuiltIn))]
    [XmlInclude(typeof(SWRLCeilingBuiltIn))]
    [XmlInclude(typeof(SWRLContainsBuiltIn))]
    [XmlInclude(typeof(SWRLContainsIgnoreCaseBuiltIn))]
    [XmlInclude(typeof(SWRLCosBuiltIn))]
    [XmlInclude(typeof(SWRLDivideBuiltIn))]
    [XmlInclude(typeof(SWRLEndsWithBuiltIn))]
    [XmlInclude(typeof(SWRLEqualBuiltIn))]
    [XmlInclude(typeof(SWRLFloorBuiltIn))]
    [XmlInclude(typeof(SWRLGreaterThanBuiltIn))]
    [XmlInclude(typeof(SWRLGreaterThanOrEqualBuiltIn))]
    [XmlInclude(typeof(SWRLLessThanBuiltIn))]
    [XmlInclude(typeof(SWRLLessThanOrEqualBuiltIn))]
    [XmlInclude(typeof(SWRLMatchesBuiltIn))]
    [XmlInclude(typeof(SWRLMultiplyBuiltIn))]
    [XmlInclude(typeof(SWRLNotEqualBuiltIn))]
    [XmlInclude(typeof(SWRLPowBuiltIn))]
    [XmlInclude(typeof(SWRLRoundBuiltIn))]
    [XmlInclude(typeof(SWRLRoundHalfToEvenBuiltIn))]
    [XmlInclude(typeof(SWRLSinBuiltIn))]
    [XmlInclude(typeof(SWRLStartsWithBuiltIn))]
    [XmlInclude(typeof(SWRLSubtractBuiltIn))]
    [XmlInclude(typeof(SWRLTanBuiltIn))]
    public abstract class SWRLAtom
    {
        #region Properties
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
        [XmlElement(typeof(OWLDatatype), ElementName="Datatype", Order=1)]
        [XmlElement(typeof(OWLDataIntersectionOf), ElementName="DataIntersectionOf", Order=1)]
        [XmlElement(typeof(OWLDataUnionOf), ElementName="DataUnionOf", Order=1)]
        [XmlElement(typeof(OWLDataComplementOf), ElementName="DataComplementOf", Order=1)]
        [XmlElement(typeof(OWLDataOneOf), ElementName="DataOneOf", Order=1)]
        [XmlElement(typeof(OWLDatatypeRestriction), ElementName="DatatypeRestriction", Order=1)]
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty", Order=1)]
        [XmlElement(typeof(OWLObjectProperty), ElementName="ObjectProperty", Order=1)]
        public OWLExpression Predicate { get; set; }

        [XmlElement(typeof(SWRLIndividualArgument), ElementName="NamedIndividual", Order=2)]
        [XmlElement(typeof(SWRLLiteralArgument), ElementName="Literal", Order=2)]
        [XmlElement(typeof(SWRLVariableArgument), ElementName="Variable", Order=2)]
        public SWRLArgument LeftArgument { get; set; }

        [XmlElement(typeof(SWRLIndividualArgument), ElementName="NamedIndividual", Order=3)]
        [XmlElement(typeof(SWRLLiteralArgument), ElementName="Literal", Order=3)]
        [XmlElement(typeof(SWRLVariableArgument), ElementName="Variable", Order=3)]
        public SWRLArgument RightArgument { get; set; }
        #endregion

        #region Ctors
        internal SWRLAtom() { }
        internal SWRLAtom(OWLExpression predicate, SWRLArgument leftArgument, SWRLArgument rightArgument)
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
            if (LeftArgument is SWRLIndividualArgument leftArgumentIndividual)
                sb.Append($"({RDFModelUtilities.GetShortUri(leftArgumentIndividual.GetResource().URI)}");
            else if (LeftArgument is SWRLLiteralArgument leftArgumentLiteral)
                sb.Append($"({RDFQueryPrinter.PrintPatternMember(leftArgumentLiteral.GetLiteral(), RDFNamespaceRegister.Instance.Register)}");
            else if (LeftArgument is SWRLVariableArgument leftArgumentVariable)
                sb.Append($"({RDFQueryPrinter.PrintPatternMember(leftArgumentVariable.GetVariable(), RDFNamespaceRegister.Instance.Register)}");
            if (RightArgument != null)
            {
                if (RightArgument is SWRLIndividualArgument rightArgumentIndividual)
                    sb.Append($",{RDFModelUtilities.GetShortUri(rightArgumentIndividual.GetResource().URI)}");
                else if (RightArgument is SWRLLiteralArgument rightArgumentLiteral)
                    sb.Append($",{RDFQueryPrinter.PrintPatternMember(rightArgumentLiteral.GetLiteral(), RDFNamespaceRegister.Instance.Register)}");
                else if (RightArgument is SWRLVariableArgument rightArgumentVariable)
                    sb.Append($",{RDFQueryPrinter.PrintPatternMember(rightArgumentVariable.GetVariable(), RDFNamespaceRegister.Instance.Register)}");
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