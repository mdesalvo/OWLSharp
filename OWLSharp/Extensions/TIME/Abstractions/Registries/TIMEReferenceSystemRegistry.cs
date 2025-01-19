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

using RDFSharp.Model;
using System.Collections;
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME
{
    public class TIMEReferenceSystemRegistry : IEnumerable<TIMEReferenceSystem>
    {
        #region Properties
        public static TIMEReferenceSystemRegistry Instance { get; internal set; }

        public static int TRSCount 
            => Instance.TRS.Count;

        public static IEnumerator<TIMEReferenceSystem> TRSEnumerator
            => Instance.TRS.Values.GetEnumerator();

        internal Dictionary<string,TIMEReferenceSystem> TRS { get; set; }
        #endregion

        #region Ctors
        static TIMEReferenceSystemRegistry()
        {
            Instance = new TIMEReferenceSystemRegistry()
            { 
                TRS = new Dictionary<string, TIMEReferenceSystem>()
                {
                    //Calendar TRS
                    { TIMECalendarReferenceSystem.Gregorian.ToString(), TIMECalendarReferenceSystem.Gregorian },
                    //Position TRS
                    { TIMEPositionReferenceSystem.UnixTime.ToString(), TIMEPositionReferenceSystem.UnixTime },
                    { TIMEPositionReferenceSystem.GeologicTime.ToString(), TIMEPositionReferenceSystem.GeologicTime }
                }
            };
        }
        #endregion

        #region Interfaces
        IEnumerator<TIMEReferenceSystem> IEnumerable<TIMEReferenceSystem>.GetEnumerator()
            => TRSEnumerator;

        IEnumerator IEnumerable.GetEnumerator()
            => TRSEnumerator;
        #endregion

        #region Methods
        public static void AddTRS(TIMEReferenceSystem trs)
        {
            if (trs != null && !Instance.TRS.ContainsKey(trs.ToString()))
                Instance.TRS.Add(trs.ToString(), trs);
        }

        public static bool ContainsTRS(RDFResource trsURI)
            => trsURI != null && Instance.TRS.ContainsKey(trsURI.ToString());
        #endregion
    }
}