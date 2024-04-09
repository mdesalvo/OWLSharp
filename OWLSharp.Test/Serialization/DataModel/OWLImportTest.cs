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
using RDFSharp.Model;

namespace OWLSharp.Serialization.Test
{
    [TestClass]
    public class OWLImportTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateImport()
        {
            OWLImport import = new OWLImport(new RDFResource(RDFVocabulary.FOAF.BASE_URI));

            Assert.IsNotNull(import);
            Assert.IsTrue(string.Equals(import.IRI, RDFVocabulary.FOAF.BASE_URI));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingImportBecauseNullUri()
            => Assert.ThrowsException<OWLException>(() => new OWLImport(null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingImportBecauseBlankUri()
            => Assert.ThrowsException<OWLException>(() => new OWLImport(new RDFResource()));

        [TestMethod]
        public void ShouldSerializeImport()
        {
            OWLImport import = new OWLImport(new RDFResource(RDFVocabulary.FOAF.BASE_URI));
            string serializedXML = OWLTestSerializer<OWLImport>.Serialize(import);

            Assert.IsTrue(string.Equals(serializedXML,
@"<Import>http://xmlns.com/foaf/0.1/</Import>"));          
        }

        [TestMethod] 
        public void ShouldDeserializeImport()
        {
            OWLImport import = OWLTestSerializer<OWLImport>.Deserialize(
@"<Import>http://xmlns.com/foaf/0.1/</Import>");

            Assert.IsNotNull(import);
            Assert.IsTrue(string.Equals(import.IRI, RDFVocabulary.FOAF.BASE_URI));
        }
        #endregion
    }
}