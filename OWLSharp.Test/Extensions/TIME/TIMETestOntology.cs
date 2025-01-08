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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Extensions.TIME;
using System;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMETestOntology
    {
        #region Properties
        internal static OWLOntology TestOntology { get; set; }
        #endregion

        #region Ctors
        [TestInitialize]
        public async Task InitializeAsync()
        {
            if (TestOntology == null)
            {
                TestOntology = new OWLOntology(new Uri("ex:ont"));
                await TestOntology.InitializeTIMEAsync(30000);

                Assert.IsTrue(OWLOntologyHelper.ImportCache.Count > 0);
            }
        }
        #endregion
    }
}