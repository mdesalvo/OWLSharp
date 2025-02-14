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

namespace OWLSharp.Test.Ontology
{
    [TestClass]
    public class OWLPrefixTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreatePrefix()
        {
            OWLPrefix prefix = new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX));

            Assert.IsNotNull(prefix);
            Assert.IsTrue(string.Equals(prefix.Name, RDFVocabulary.FOAF.PREFIX));
            Assert.IsTrue(string.Equals(prefix.IRI, RDFVocabulary.FOAF.BASE_URI));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingImportBecauseNullNamepsace()
            => Assert.ThrowsExactly<OWLException>(() => _ = new OWLPrefix(null));

        [TestMethod]
        public void ShouldSerializePrefix()
        {
            OWLPrefix prefix = new OWLPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.FOAF.PREFIX));
            string serializedXML = OWLSerializer.SerializeObject(prefix);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />"));
        }

        [TestMethod] 
        public void ShouldDeserializePrefix()
        {
            OWLPrefix prefix = OWLSerializer.DeserializeObject<OWLPrefix>(
@"<Prefix name=""foaf"" IRI=""http://xmlns.com/foaf/0.1/"" />");

            Assert.IsNotNull(prefix);
            Assert.IsTrue(string.Equals(prefix.Name, RDFVocabulary.FOAF.PREFIX));
            Assert.IsTrue(string.Equals(prefix.IRI, RDFVocabulary.FOAF.BASE_URI));
        }
        #endregion
    }
}