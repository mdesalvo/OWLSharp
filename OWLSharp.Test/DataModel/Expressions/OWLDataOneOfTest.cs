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
using OWLSharp.Test;
using RDFSharp.Model;

namespace OWLSharp.DataModel.Test
{
    [TestClass]
    public class OWLDataOneOfTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDataOneOf()
        {
            OWLDataOneOf dataOneOf = new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("hello","en"))]);

            Assert.IsNotNull(dataOneOf);
            Assert.IsNotNull(dataOneOf.Literals);
            Assert.IsTrue(dataOneOf.Literals.Count == 1);
            Assert.IsTrue(string.Equals(dataOneOf.Literals.Single().Value, "hello"));
            Assert.IsTrue(string.Equals(dataOneOf.Literals.Single().Language, "EN"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataOneOfBecauseNullLiterals()
            => Assert.ThrowsException<OWLException>(() => new OWLDataOneOf(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataOneOfBecauseZeroLiterals()
            => Assert.ThrowsException<OWLException>(() => new OWLDataOneOf([]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDataOneOfBecauseNullLiteralFound()
            => Assert.ThrowsException<OWLException>(() => new OWLDataOneOf([null]));

        [TestMethod]
        public void ShouldSerializeDataOneOf()
        {
            OWLDataOneOf dataOneOf = new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("hello","en"))]);
            string serializedXML = OWLTestSerializer<OWLDataOneOf>.Serialize(dataOneOf);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DataOneOf>
  <Literal xml:lang=""EN"">hello</Literal>
</DataOneOf>"));
        }

        [TestMethod]
        public void ShouldDeserializeDataOneOf()
        {
            OWLDataOneOf dataOneOf = OWLTestSerializer<OWLDataOneOf>.Deserialize(
@"<DataOneOf>
  <Literal xml:lang=""EN"">hello</Literal>
</DataOneOf>");

            Assert.IsNotNull(dataOneOf);
            Assert.IsNotNull(dataOneOf.Literals);
            Assert.IsTrue(dataOneOf.Literals.Count == 1);
            Assert.IsTrue(string.Equals(dataOneOf.Literals.Single().Value, "hello"));
            Assert.IsTrue(string.Equals(dataOneOf.Literals.Single().Language, "EN"));
        }
        #endregion
    }
}