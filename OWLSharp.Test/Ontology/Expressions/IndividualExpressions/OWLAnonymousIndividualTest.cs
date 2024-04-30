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
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.Ontology.Expressions.Test
{
    [TestClass]
    public class OWLAnonymousIndividualTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateAnonymousIndividual()
        {
            OWLAnonymousIndividual anonIdv = new OWLAnonymousIndividual("AnonIdv");

            Assert.IsNotNull(anonIdv);
            Assert.IsTrue(string.Equals(anonIdv.NodeID, "AnonIdv"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLAnonymousIndividual(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingAnonymousIndividualBecauseInvalidNCName()
            => Assert.ThrowsException<OWLException>(() => new OWLAnonymousIndividual("ex:org/AnonIdv"));

        [TestMethod]
        public void ShouldSerializeAnonymousIndividual()
        {
            OWLAnonymousIndividual anonIdv = new OWLAnonymousIndividual("AnonIdv");
            string serializedXML = OWLTestSerializer<OWLAnonymousIndividual>.Serialize(anonIdv);

            Assert.IsTrue(string.Equals(serializedXML,
@"<AnonymousIndividual nodeID=""AnonIdv"" />"));
        }

        [TestMethod]
        public void ShouldDeserializeAnonymousIndividual()
        {
            OWLAnonymousIndividual anonIdv = OWLTestSerializer<OWLAnonymousIndividual>.Deserialize(
@"<AnonymousIndividual nodeID=""AnonIdv"" />");

            Assert.IsNotNull(anonIdv);
            Assert.IsTrue(string.Equals(anonIdv.NodeID, "AnonIdv"));
        }

		[TestMethod]
        public void ShouldConvertAnonymousIndividualToGraph()
        {
            OWLAnonymousIndividual anonIdv = new OWLAnonymousIndividual("AnonIdv");
			RDFGraph graph = anonIdv.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 0);
        }

		[TestMethod]
        public void ShouldConvertAnonymousIndividualToResource()
        {
            OWLAnonymousIndividual anonIdv = new OWLAnonymousIndividual("AnonIdv");
			RDFResource representative = anonIdv.GetIRI();

            Assert.IsNotNull(representative);
            Assert.IsTrue(representative.Equals(new RDFResource($"bnode:{anonIdv.NodeID}")));
        }
        #endregion
    }
}