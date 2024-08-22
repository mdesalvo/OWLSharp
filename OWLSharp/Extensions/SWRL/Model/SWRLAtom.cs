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

using OWLSharp.Ontology;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Reasoner;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OWLSharp.Extensions.SWRL.Model
{
    public abstract class SWRLAtom
    {
        #region Properties
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