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
using System.Collections;

namespace OWLSharp.Ontology
{
    public sealed class SWRLBuiltInRegister : IEnumerable<SWRLBuiltIn>
    {
        #region Properties
        public static SWRLBuiltInRegister Instance { get; internal set; }
        internal List<SWRLBuiltIn> Register { get; set; }

        public static int BuiltInsCount => Instance.Register.Count;

        public static IEnumerator<SWRLBuiltIn> BuiltInsEnumerator => Instance.Register.GetEnumerator();
        #endregion

        #region Ctors
        static SWRLBuiltInRegister()
            => Instance = new SWRLBuiltInRegister { Register = new List<SWRLBuiltIn>() };
        #endregion

        #region Interfaces
        IEnumerator<SWRLBuiltIn> IEnumerable<SWRLBuiltIn>.GetEnumerator() => BuiltInsEnumerator;

        IEnumerator IEnumerable.GetEnumerator() => BuiltInsEnumerator;
        #endregion

        #region Methods
        public static void AddBuiltIn(SWRLBuiltIn builtIn)
        {
            if (builtIn != null && GetBuiltIn(builtIn.IRI) == null)
                Instance.Register.Add(builtIn);
        }

        public static SWRLBuiltIn GetBuiltIn(string builtinIRI)
            => Instance.Register.Find(b => string.Equals(b.IRI, builtinIRI));
        #endregion
    }
}