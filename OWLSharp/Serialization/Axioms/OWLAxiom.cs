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
        internal OWLAxiom() => AxiomAnnotations = new List<OWLAnnotation>();
        #endregion
    }

    //Derived

    public partial class OWLDeclarationAxiom : OWLAxiom
    {
        internal OWLDeclarationAxiom() => SerializationPriority=1;
    }

    public class OWLClassAxiom : OWLAxiom
    {
        internal OWLClassAxiom() => SerializationPriority=2;
    }

    public class OWLObjectPropertyAxiom : OWLAxiom
    {
        internal OWLObjectPropertyAxiom() => SerializationPriority=3;
    }

    public class OWLDataPropertyAxiom : OWLAxiom
    {
        internal OWLDataPropertyAxiom() => SerializationPriority=4;
    }

    public partial class OWLDatatypeDefinitionAxiom : OWLAxiom
    {
        internal OWLDatatypeDefinitionAxiom() => SerializationPriority=5;
    }

    public partial class OWLHasKeyAxiom : OWLAxiom
    {
        internal OWLHasKeyAxiom() => SerializationPriority=6;
    }

    public class OWLAssertionAxiom : OWLAxiom
    {
        internal OWLAssertionAxiom() => SerializationPriority=7;
    }

    public class OWLAnnotationAxiom : OWLAxiom
    {
        internal OWLAnnotationAxiom() => SerializationPriority=8;
    }
}