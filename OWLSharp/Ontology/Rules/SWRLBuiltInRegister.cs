/*
   Copyright 2014-2025 Marco De Salvo

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
    /// <summary>
    /// SWRLBuiltInRegister is a singleton in-memory container for registered SWRL builtins
    /// </summary>
    public sealed class SWRLBuiltInRegister : IEnumerable<SWRLBuiltIn>
    {
        #region Properties
        /// <summary>
        /// Singleton instance of the SWRLBuiltInRegister class
        /// </summary>
        public static SWRLBuiltInRegister Instance { get; }

        /// <summary>
        /// List of registered bult-ins
        /// </summary>
        internal List<SWRLBuiltIn> Register { get; set; }

        /// <summary>
        /// Count of the register's built-ins
        /// </summary>
        public static int BuiltInsCount
            => Instance.Register.Count;

        /// <summary>
        /// Gets the enumerator on the register's built-ins for iteration
        /// </summary>
        public static IEnumerator<SWRLBuiltIn> BuiltInsEnumerator
            => Instance.Register.GetEnumerator();
        #endregion

        #region Ctors
        /// <summary>
        /// Initializes the singleton instance of the register
        /// </summary>
        static SWRLBuiltInRegister()
            => Instance = new SWRLBuiltInRegister { Register = new List<SWRLBuiltIn>() };
        #endregion

        #region Interfaces
        /// <summary>
        /// Exposes a typed enumerator on the register's built-ins
        /// </summary>
        IEnumerator<SWRLBuiltIn> IEnumerable<SWRLBuiltIn>.GetEnumerator()
            => BuiltInsEnumerator;

        /// <summary>
        /// Exposes an untyped enumerator on the register's built-ins
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => BuiltInsEnumerator;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given built-in to the register, if it has unique IRI
        /// </summary>
        public static void AddBuiltIn(SWRLBuiltIn builtIn)
        {
            if (builtIn != null && GetBuiltIn(builtIn.IRI) == null)
            {
                //Ensure to cleanup the arguments of the given built-in before storing.
                //In fact, they will be retrieved at evaluation time by the SWRL engine.
                builtIn.Arguments = null;

                Instance.Register.Add(builtIn);
            }
        }

        /// <summary>
        /// Retrieves a built-in by seeking presence of its IRI
        /// </summary>
        public static SWRLBuiltIn GetBuiltIn(string builtinIRI)
            => Instance.Register.Find(b => string.Equals(b.IRI, builtinIRI));
        #endregion
    }
}