﻿/*
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
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    [XmlRoot("DisjointDataProperties")]
    public class OWLDisjointDataProperties : OWLDataPropertyAxiom
    {
        #region Properties
        [XmlElement(ElementName="DataProperty", Order=2)]
        public List<OWLDataProperty> DataProperties { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointDataProperties() : base() { }
        public OWLDisjointDataProperties(List<OWLDataProperty> dataProperties) : this()
        {
            #region Guards
            if (dataProperties == null)
                throw new OWLException("Cannot create OWLDisjointDataProperties because given \"dataProperties\" parameter is null");
            if (dataProperties.Count < 2)
                throw new OWLException("Cannot create OWLDisjointDataProperties because given \"dataProperties\" parameter must contain at least 2 elements");
            if (dataProperties.Any(dp => dp == null))
                throw new OWLException("Cannot create OWLDisjointDataProperties because given \"dataProperties\" parameter contains a null element");
            #endregion

            DataProperties = dataProperties;
        }
        #endregion
    }
}