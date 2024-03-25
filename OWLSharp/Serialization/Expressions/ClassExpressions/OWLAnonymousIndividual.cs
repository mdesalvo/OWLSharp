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
using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLAnonymousIndividual : OWLIndividualExpression
    {
        #region Properties
        [XmlAttribute("nodeID", DataType="NCName")]
        public string NodeID { get; set; }
        #endregion

        #region Ctors
        public OWLAnonymousIndividual() { }
        public OWLAnonymousIndividual(string ncName)
        {
            try
            {
                RDFTypedLiteral xsdNCNameLiteral = new RDFTypedLiteral(ncName, RDFModelEnums.RDFDatatypes.XSD_NCNAME);
                NodeID = xsdNCNameLiteral.Value;
            }
            catch { throw new OWLException("Cannot create OWLAnonymousIndividual because given \"ncName\" parameter is not a valid xsd:NCName"); }
        }
        #endregion
    }
}