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

using System;
using System.Threading.Tasks;

namespace OWLSharp
{
    /// <summary>
    /// OWLEvents represents a collector for events which might happen during ontology validation and reasoning
    /// </summary>
    public static class OWLEvents
    {
        #region OnInfo
        /// <summary>
        /// Event raised when an informational message is generated (e.g: beginning of a reasoning task)
        /// </summary>
        public static event OWLInfoEventHandler OnInfo = delegate { };
        /// <summary>
        /// Delegate for handling informational event messages
        /// </summary>
        public delegate void OWLInfoEventHandler(string eventMessage);
        /// <summary>
        /// Raises an informational event with the specified message (preceded by UTC timestamp)
        /// </summary>
        internal static void RaiseInfo(string eventMessage)
            => Parallel.Invoke(() => OnInfo($"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ};INFO;{eventMessage}"));
        #endregion

        #region OnWarning
        /// <summary>
        /// Event raised when a warning message is generated (e.g: detection of inconsistent modeling)
        /// </summary>
        public static event OWLWarningEventHandler OnWarning = delegate { };
        /// <summary>
        /// Delegate for handling warning event messages
        /// </summary>
        public delegate void OWLWarningEventHandler(string eventMessage);
        /// <summary>
        /// Raises a warning event with the specified message (preceded by UTC timestamp)
        /// </summary>
        internal static void RaiseWarning(string eventMessage)
            => Parallel.Invoke(() => OnWarning($"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ};WARNING;{eventMessage}"));
        #endregion
    }
}