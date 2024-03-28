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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLDisjointDataPropertiesAxiom : OWLDataPropertyAxiom
    {
        #region Properties
        //Register here all derived types of OWLDataPropertyExpression
        [XmlElement(typeof(OWLDataProperty), ElementName="DataProperty")]
        public List<OWLDataPropertyExpression> DataPropertyExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointDataPropertiesAxiom() : base() { }
        public OWLDisjointDataPropertiesAxiom(List<OWLDataPropertyExpression> dataPropertyExpressions) : this()
        {
            #region Guards
            if (dataPropertyExpressions == null)
                throw new OWLException("Cannot create OWLDisjointDataPropertiesAxiom because given \"dataPropertyExpressions\" parameter is null");
            if (dataPropertyExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDisjointDataPropertiesAxiom because given \"dataPropertyExpressions\" parameter must contain at least 2 elements");
            #endregion

            DataPropertyExpressions = dataPropertyExpressions;
        }
        #endregion
    }
}