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

using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLAxiom 
    {
        #region Properties
        [XmlIgnore]
        public bool IsInference { get; set; }
        [XmlIgnore]
        internal int SerializationOrder { get; set; }
        #endregion
    }

    //Derived

    public class OWLAnnotationAxiom : OWLAxiom 
    {
        internal OWLAnnotationAxiom() => SerializationOrder = 7;
    }

    public class OWLAssertionAxiom : OWLAxiom
    {
        internal OWLAssertionAxiom() => SerializationOrder = 6;
    }

    public class OWLClassAxiom : OWLAxiom
    {
        internal OWLClassAxiom() => SerializationOrder = 2;
    }

    public class OWLDataPropertyAxiom : OWLAxiom
    {
        internal OWLDataPropertyAxiom() => SerializationOrder = 4;
    }

    public class OWLHasKeyAxiom : OWLAxiom
    {
        internal OWLHasKeyAxiom() => SerializationOrder = 5;
    }

    public class OWLObjectPropertyAxiom : OWLAxiom
    {
        internal OWLObjectPropertyAxiom() => SerializationOrder = 3;
    }
}