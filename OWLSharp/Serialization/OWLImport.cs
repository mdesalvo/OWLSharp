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
using System.Xml.Serialization;

namespace OWLSharp
{
    public class OWLImport
    {
        #region Properties
        [XmlText(DataType="anyURI")]
        public string IRI { get; set; }
        #endregion

        #region Ctors
        internal OWLImport() { }
        public OWLImport(RDFResource importUri)
        {
            #region Guards
            if (importUri == null)
                throw new OWLException("Cannot create OWLImport because given \"importUri\" parameter is null");
            if (importUri.IsBlank)
                throw new OWLException("Cannot create OWLImport because given \"importUri\" parameter is a blank resource");
            #endregion

            IRI = importUri.ToString();
        }
        #endregion
    }
}