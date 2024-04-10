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

namespace OWLSharp.Ontology
{
    public class OWLAxiom 
    {
        #region Properties
        [XmlIgnore]
        public bool IsInference { get; set; }

        [XmlIgnore]
        public int SerializationPriority { get; set; }

        [XmlElement("Annotation", Order=1)]
        public List<OWLAnnotation> AxiomAnnotations { get; set; }
        #endregion

        #region Ctors
        internal OWLAxiom() 
            => AxiomAnnotations = new List<OWLAnnotation>();
        #endregion
    }

    //Derived

    public class OWLClassAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLClassAxiom() : base() 
            => SerializationPriority=2;
        #endregion
    }

    public class OWLObjectPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLObjectPropertyAxiom() : base() 
            => SerializationPriority=3;
        #endregion
    }

    public class OWLDataPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLDataPropertyAxiom() : base() 
            => SerializationPriority=4;
        #endregion
    }

    public class OWLAssertionAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAssertionAxiom() : base() 
            => SerializationPriority=7;
        #endregion
    }

    public class OWLAnnotationAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAnnotationAxiom() : base() 
            => SerializationPriority=8;
        #endregion
    }
}