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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OWLSharp.Ontology
{
    public class OWLAxiom 
    {
        #region Properties
        [XmlIgnore]
        public bool IsInference { get; set; }

        [XmlElement("Annotation", Order=1)]
        public List<OWLAnnotation> Annotations { get; set; }
        #endregion

        #region Ctors
        internal OWLAxiom() 
            => Annotations = new List<OWLAnnotation>();
        #endregion

        #region Methods
        internal virtual RDFGraph ToRDFGraph()
            => new RDFGraph();
        #endregion
    }

    //Derived

    public class OWLClassAxiom : OWLAxiom
    {
		#region Ctors
        internal OWLClassAxiom() : base() { }
		#endregion	
	}

    public class OWLObjectPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLObjectPropertyAxiom() : base() { }
        #endregion
    }

    public class OWLDataPropertyAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLDataPropertyAxiom() : base() { }
        #endregion
    }

    public class OWLAssertionAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAssertionAxiom() : base() { }
        #endregion
    };

    public class OWLAnnotationAxiom : OWLAxiom
    {
        #region Ctors
        internal OWLAnnotationAxiom() : base() { }
        #endregion
    }
}