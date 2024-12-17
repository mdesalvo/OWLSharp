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

namespace OWLSharp.Ontology
{
    [XmlRoot("AnonymousIndividual")]
    public class OWLAnonymousIndividual : OWLIndividualExpression
    {
        #region Properties
        [XmlAttribute("nodeID", DataType="NCName")]
        public string NodeID { get; set; }
        #endregion

        #region Ctors
        public OWLAnonymousIndividual()
            => NodeID = $"ANON{Guid.NewGuid():N}";
        public OWLAnonymousIndividual(string xsdNCName)
        {
            try
            {
                RDFTypedLiteral xsdNCNameLiteral = new RDFTypedLiteral(xsdNCName, RDFModelEnums.RDFDatatypes.XSD_NCNAME);
                NodeID = xsdNCNameLiteral.Value;
            }
            catch { throw new OWLException("Cannot create OWLAnonymousIndividual because given \"anonymousName\" parameter is null or is not a valid xsd:NCName"); }
        }
        #endregion

        #region Methods
        public override RDFResource GetIRI()
            => new RDFResource(string.Concat($"bnode:{NodeID}"));
        #endregion
    }
}