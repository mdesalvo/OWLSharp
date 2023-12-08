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

using System;
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLEvents represents a collector for events generated within the "OWLSharp" namespace
    /// </summary>
    public static class OWLEvents
    {
        #region OnInfo
        /// <summary>
        /// Event representing an information message
        /// </summary>
        public static event OWLInfoEventHandler OnInfo = delegate { };

        /// <summary>
        /// Delegate to handle information events
        /// </summary>
        public delegate void OWLInfoEventHandler(string eventMessage);

        /// <summary>
        /// Internal invoker of the subscribed information event handler
        /// </summary>
        internal static void RaiseInfo(string eventMessage)
            => Parallel.Invoke(() => OnInfo(string.Concat(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), ";INFO;", eventMessage)));
        #endregion

        #region OnWarning
        /// <summary>
        /// Event representing a warning message
        /// </summary>
        public static event OWLWarningEventHandler OnWarning = delegate { };

        /// <summary>
        /// Delegate to handle warning events
        /// </summary>
        public delegate void OWLWarningEventHandler(string eventMessage);

        /// <summary>
        /// Internal invoker of the subscribed warning event handler
        /// </summary>
        internal static void RaiseWarning(string eventMessage)
            => Parallel.Invoke(() => OnWarning(string.Concat(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), ";WARNING;", eventMessage)));
        #endregion
    }
}