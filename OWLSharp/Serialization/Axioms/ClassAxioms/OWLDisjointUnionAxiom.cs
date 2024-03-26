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
using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLDisjointUnionAxiom : OWLClassAxiom
    {
        #region Properties
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=1)]
        public OWLClass ClassIRI { get; set; }

        //Register here all derived types of OWLClassExpression
        [XmlElement(typeof(OWLClass), ElementName="Class", Order=2)]
        [XmlElement(typeof(OWLObjectIntersectionOf), ElementName="ObjectIntersectionOf", Order=2)]
        [XmlElement(typeof(OWLObjectUnionOf), ElementName="ObjectUnionOf", Order=2)]
        [XmlElement(typeof(OWLObjectComplementOf), ElementName="ObjectComplementOf", Order=2)]
        [XmlElement(typeof(OWLObjectOneOf), ElementName="ObjectOneOf", Order=2)]
        public List<OWLClassExpression> ClassExpressions { get; set; }
        #endregion

        #region Ctors
        internal OWLDisjointUnionAxiom() { }
        public OWLDisjointUnionAxiom(OWLClass classIRI, List<OWLClassExpression> classExpressions) 
        {
            #region Guards
            if (classIRI == null)
                throw new OWLException("Cannot create OWLDisjointUnionAxiom because given \"classIRI\" parameter is null");
            if (classExpressions == null)
                throw new OWLException("Cannot create OWLDisjointUnionAxiom because given \"classExpressions\" parameter is null");
            if (classExpressions.Count < 2)
                throw new OWLException("Cannot create OWLDisjointUnionAxiom because given \"classExpressions\" parameter must contain at least 2 elements");
            #endregion

            ClassIRI = classIRI;
            ClassExpressions = classExpressions;
        }
        #endregion
    }
}