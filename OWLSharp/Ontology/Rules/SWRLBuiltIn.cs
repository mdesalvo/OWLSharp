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
using OWLSharp.Ontology.Rules.BuiltIns;
using OWLSharp.Reasoner;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules
{
    //Register here all derived types of SWRLBuiltIn
    [XmlInclude(typeof(SWRLFilterBuiltIn))]
    [XmlInclude(typeof(SWRLMathBuiltIn))]
    [XmlRoot("BuiltInAtom")]
    public abstract class SWRLBuiltIn : SWRLAtom
    {
        #region Ctors
        internal SWRLBuiltIn(OWLExpression predicate, RDFPatternMember leftArgument, RDFPatternMember rightArgument)
            : base(predicate, leftArgument, rightArgument) { }
        #endregion

        #region Methods
        internal abstract DataTable EvaluateOnAntecedent(DataTable antecedentResults, OWLOntology ontology);

        //Derived from SRWLAtom
        internal override DataTable EvaluateOnAntecedent(OWLOntology ontology) => null;
        internal override List<OWLInference> EvaluateOnConsequent(DataTable antecedentResults, OWLOntology ontology) => null;
        #endregion
    }
}