/*
   Copyright 2012-2024 Marco De Salvo

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
using System.Collections;
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME
{
    /// <summary>
    /// TIMEReferenceSystemRegistry is a singleton in-memory container for registered TRS objects
    /// </summary>
    public class TIMEReferenceSystemRegistry : IEnumerable<TIMEReferenceSystem>
    {
        #region Properties
        /// <summary>
        /// Singleton instance of the TRS registry
        /// </summary>
        public static TIMEReferenceSystemRegistry Instance { get; internal set; }

        /// <summary>
        /// Count of the registry's entries
        /// </summary>
        public static int TRSCount 
            => Instance.TRS.Count;

        /// <summary>
        /// Gets the enumerator on the registry entries for iteration
        /// </summary>
        public static IEnumerator<TIMEReferenceSystem> TRSEnumerator
            => Instance.TRS.Values.GetEnumerator();

        /// <summary>
        /// Collection of registered TRS objects
        /// </summary>
        internal Dictionary<string,TIMEReferenceSystem> TRS { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Static-ctor to initialize the singleton instance of the registry
        /// </summary>
        static TIMEReferenceSystemRegistry()
        {
            Instance = new TIMEReferenceSystemRegistry()
            { 
                TRS = new Dictionary<string, TIMEReferenceSystem>()
                {
                    //Calendar TRS
                    { TIMECalendarReferenceSystem.Gregorian.ToString(), TIMECalendarReferenceSystem.Gregorian },
                    //Position TRS
                    { TIMEPositionReferenceSystem.UnixTRS.ToString(), TIMEPositionReferenceSystem.UnixTRS },
                    { TIMEPositionReferenceSystem.GeologicTRS.ToString(), TIMEPositionReferenceSystem.GeologicTRS },
                    { TIMEPositionReferenceSystem.GlobalPositioningSystemTRS.ToString(), TIMEPositionReferenceSystem.GlobalPositioningSystemTRS }
                }
            };
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Exposes a typed enumerator on the registry entries
        /// </summary>
        IEnumerator<TIMEReferenceSystem> IEnumerable<TIMEReferenceSystem>.GetEnumerator()
            => TRSEnumerator;

        /// <summary>
        /// Exposes an untyped enumerator on the registry entries
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => TRSEnumerator;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given TRS to the registry
        /// </summary>
        public static void AddTRS(TIMEReferenceSystem trs)
        {
            if (trs != null && !Instance.TRS.ContainsKey(trs.ToString()))
                Instance.TRS.Add(trs.ToString(), trs);
        }

        /// <summary>
        /// Checks if the registry contains the given TRS
        /// </summary>
        public static bool ContainsTRS(RDFResource trsURI)
            => trsURI != null && Instance.TRS.ContainsKey(trsURI.ToString());
        #endregion
    }
}