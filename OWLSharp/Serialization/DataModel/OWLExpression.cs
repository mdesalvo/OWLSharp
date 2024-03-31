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

namespace OWLSharp
{
    public abstract class OWLExpression { }

    //Derived

    public abstract class OWLAnnotationPropertyExpression : OWLExpression { }

    public abstract class OWLClassExpression : OWLExpression { }

    public abstract class OWLDataRangeExpression : OWLExpression { }

    public abstract class OWLDataPropertyExpression : OWLExpression { }

    public abstract class OWLObjectPropertyExpression : OWLExpression { }

    public abstract class OWLIndividualExpression : OWLExpression { }

    public abstract class OWLLiteralExpression : OWLExpression { }
}