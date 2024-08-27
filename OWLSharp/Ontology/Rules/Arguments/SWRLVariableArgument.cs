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
using RDFSharp.Model;
using RDFSharp.Query;
using System.Xml;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules.Arguments
{
    [XmlRoot("Variable")]
    public class SWRLVariableArgument : SWRLArgument
    {
        #region Properties
        [XmlAttribute("IRI", DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal SWRLVariableArgument() { }
        public SWRLVariableArgument(RDFVariable variable)
        {
            #region Guards
            if (variable == null)
                throw new OWLException("Cannot create SWRLVariableArgument because given \"variable\" parameter is null");
            #endregion

            IRI = new RDFResource($"urn:swrl:var#{variable.VariableName.Substring(1)}").ToString();
        }
        #endregion

        #region Interfaces
        public override string ToString()
            => GetVariable().ToString();
        #endregion

        #region Methods
        public RDFVariable GetVariable()
            => new RDFVariable($"?{IRI.Replace("urn:swrl:var#", string.Empty)}");
        #endregion
    }
}