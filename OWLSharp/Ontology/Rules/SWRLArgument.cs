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

using OWLSharp.Ontology.Rules.Arguments;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules
{
    //Register here all derived types of SWRLArgument
    [XmlInclude(typeof(SWRLIndividualArgument))]
    [XmlInclude(typeof(SWRLLiteralArgument))]
    [XmlInclude(typeof(SWRLVariableArgument))]    
    public abstract class SWRLArgument
    {
        #region Ctors
        internal SWRLArgument() { }
        #endregion
    }
}