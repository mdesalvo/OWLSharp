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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class SWRLBuiltInRegisterTest
    {
        #region Tests
        [TestMethod]
        public void ShouldExcerciseFeaturesOfRegister()
        {
            bool Evaluator(DataRow datarow) => string.Equals(datarow["?VAR"].ToString(), "value");

            SWRLBuiltIn builtIn = new SWRLBuiltIn(
                Evaluator,
                new RDFResource("http://www.w3.org/2003/11/swrl#exampleRegistered2"),
                new SWRLVariableArgument(new RDFVariable("?VAR")),
                new SWRLIndividualArgument(new RDFResource("http://test.org/")),
                new SWRLLiteralArgument(new RDFPlainLiteral("lit")));

            SWRLBuiltInRegister.AddBuiltIn(builtIn);
            SWRLBuiltInRegister.AddBuiltIn(builtIn); //This will not be added again, since we avoid duplicates
            SWRLBuiltInRegister.AddBuiltIn(null); //This will not be added, since we avoid nulls

            Assert.IsTrue(SWRLBuiltInRegister.BuiltInsCount >= 1);
            Assert.IsNotNull(SWRLBuiltInRegister.GetBuiltIn("http://www.w3.org/2003/11/swrl#exampleRegistered2"));
            Assert.IsNull(SWRLBuiltInRegister.GetBuiltIn("http://www.w3.org/2003/11/swrl#exampleGTDFFR"));
            Assert.IsNull(SWRLBuiltInRegister.GetBuiltIn(null));

            int i=0;
            IEnumerator<SWRLBuiltIn> builtins = SWRLBuiltInRegister.BuiltInsEnumerator;
            while(builtins.MoveNext())
                i++;
            Assert.IsTrue(i >= 1);
        }
        #endregion
    }
}