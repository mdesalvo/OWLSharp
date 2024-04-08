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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLObjectOneOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateObjectOneOf()
        {
            OWLObjectOneOf ObjectOneOf = new OWLObjectOneOf([
                new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLAnonymousIndividual("AnonIdv")]);

            Assert.IsNotNull(ObjectOneOf);
            Assert.IsNotNull(ObjectOneOf.IndividualExpressions);
            Assert.IsTrue(ObjectOneOf.IndividualExpressions.Count == 2);
            Assert.IsTrue(ObjectOneOf.IndividualExpressions.Any(iex => iex is OWLNamedIndividual namedIdv
                            && string.Equals(namedIdv.IRI, "ex:Bob")));
            Assert.IsTrue(ObjectOneOf.IndividualExpressions.Any(iex => iex is OWLAnonymousIndividual anonIdv
                            && string.Equals(anonIdv.NodeID, "AnonIdv")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectOneOfBecauseNullIndividualExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectOneOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectOneOfBecauseZeroIndividualExpressions()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectOneOf([]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingObjectOneOfBecauseNullIndividualExpressionFound()
            => Assert.ThrowsException<OWLException>(() => new OWLObjectOneOf([null]));

        [TestMethod]
        public void ShouldSerializeObjectOneOf()
        {
            OWLObjectOneOf ObjectOneOf = new OWLObjectOneOf([
                new OWLNamedIndividual(new RDFResource("ex:Bob")), new OWLAnonymousIndividual("AnonIdv")]);
            string serializedXML = OWLTestSerializer<OWLObjectOneOf>.Serialize(ObjectOneOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<ObjectOneOf>
  <NamedIndividual IRI=""ex:Bob"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</ObjectOneOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeObjectOneOf()
        {
            OWLObjectOneOf ObjectOneOf = OWLTestSerializer<OWLObjectOneOf>.Deserialize(
@"<ObjectOneOf>
  <NamedIndividual IRI=""ex:Bob"" />
  <AnonymousIndividual nodeID=""AnonIdv"" />
</ObjectOneOf>");

            Assert.IsNotNull(ObjectOneOf);
            Assert.IsNotNull(ObjectOneOf.IndividualExpressions);
            Assert.IsTrue(ObjectOneOf.IndividualExpressions.Count == 2);
            Assert.IsTrue(ObjectOneOf.IndividualExpressions.Any(iex => iex is OWLNamedIndividual namedIdv
                            && string.Equals(namedIdv.IRI, "ex:Bob")));
            Assert.IsTrue(ObjectOneOf.IndividualExpressions.Any(iex => iex is OWLAnonymousIndividual anonIdv
                            && string.Equals(anonIdv.NodeID, "AnonIdv")));
        }
        #endregion
    }
}