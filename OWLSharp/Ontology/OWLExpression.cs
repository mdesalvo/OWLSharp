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

using RDFSharp.Model;

namespace OWLSharp.Ontology
{
    public class OWLExpression 
	{
        #region Methods
        public virtual RDFResource GetIRI()
            => new RDFResource();

        internal virtual RDFGraph ToRDFGraph(RDFResource expressionIRI=null)
			=> new RDFGraph();
        #endregion
    }

    //Derived

    public class OWLAnnotationPropertyExpression : OWLExpression { }

    public class OWLClassExpression : OWLExpression { }

    public class OWLDataRangeExpression : OWLExpression { }

    public class OWLDataPropertyExpression : OWLExpression { }

    public class OWLObjectPropertyExpression : OWLExpression { }

    public class OWLIndividualExpression : OWLExpression { }

    public class OWLLiteralExpression : OWLExpression { }
}