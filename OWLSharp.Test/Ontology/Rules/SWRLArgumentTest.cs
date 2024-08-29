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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Rules;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Rules
{
    [TestClass]
    public class SWRLArgumentTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSWRLIndividualArgument()
        {
            SWRLIndividualArgument arg = new SWRLIndividualArgument(new RDFResource("ex:Mark"));

            Assert.IsNotNull(arg);
            Assert.IsTrue(string.Equals("ex:Mark", arg.IRI));
            Assert.IsTrue(string.Equals("ex:Mark", arg.ToString()));
            Assert.IsTrue(arg.GetResource().Equals(new RDFResource("ex:Mark")));
            Assert.IsTrue(string.Equals("<NamedIndividual IRI=\"ex:Mark\" />", OWLSerializer.SerializeObject(arg)));
            Assert.ThrowsException<OWLException>(() => new SWRLIndividualArgument(null));
            Assert.ThrowsException<OWLException>(() => new SWRLIndividualArgument(new RDFResource()));
            Assert.ThrowsException<OWLException>(() => new SWRLIndividualArgument(new RDFResource("bnode:jd8d7f")));
        }
        #endregion
    }
}