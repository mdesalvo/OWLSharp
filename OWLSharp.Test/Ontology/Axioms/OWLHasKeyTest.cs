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
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using OWLSharp.Test;
using RDFSharp.Model;


namespace OWLSharp.Ontology.Axioms.Test
{
    [TestClass]
    public class OWLHasKeyTest
    {
        #region Tests
        [TestMethod]
		public void ShouldCreateHasKey()
		{
			OWLHasKey hasKey = new OWLHasKey(
				new OWLClass(RDFVocabulary.FOAF.AGENT),
				[new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
				[new OWLDataProperty(RDFVocabulary.FOAF.AGE)]);

			Assert.IsNotNull(hasKey);
			Assert.IsTrue(hasKey.ClassExpression is OWLClass cls 
							&& string.Equals(cls.IRI, "http://xmlns.com/foaf/0.1/Agent"));
            Assert.IsTrue(hasKey.ObjectPropertyExpressions.Single() is OWLObjectProperty objProp 
							&& string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsTrue(hasKey.DataProperties.Single() is OWLDataProperty dtProp
                            && string.Equals(dtProp.IRI, "http://xmlns.com/foaf/0.1/age"));
        }

		[TestMethod]
		public void ShouldSerializeHasKey()
		{
            OWLHasKey hasKey = new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS), new OWLObjectProperty(RDFVocabulary.FOAF.FOCUS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]);
            string serializedXML = OWLTestSerializer<OWLHasKey>.Serialize(hasKey);

            Assert.IsTrue(string.Equals(serializedXML,
@"<HasKey><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/focus"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /></HasKey>"));
		}

		[TestMethod]
		public void ShouldDeserializeHasKey()
		{
			OWLHasKey hasKey = OWLTestSerializer<OWLHasKey>.Deserialize(
@"<HasKey><Class IRI=""http://xmlns.com/foaf/0.1/Agent"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/knows"" /><ObjectProperty IRI=""http://xmlns.com/foaf/0.1/focus"" /><DataProperty IRI=""http://xmlns.com/foaf/0.1/age"" /></HasKey>");

            Assert.IsNotNull(hasKey);
            Assert.IsTrue(hasKey.ClassExpression is OWLClass cls
                            && string.Equals(cls.IRI, "http://xmlns.com/foaf/0.1/Agent"));
            Assert.IsTrue(hasKey.ObjectPropertyExpressions[0] is OWLObjectProperty objProp
                            && string.Equals(objProp.IRI, "http://xmlns.com/foaf/0.1/knows"));
            Assert.IsTrue(hasKey.ObjectPropertyExpressions[1] is OWLObjectProperty objProp1
                            && string.Equals(objProp1.IRI, "http://xmlns.com/foaf/0.1/focus"));
            Assert.IsTrue(hasKey.DataProperties.Single() is OWLDataProperty dtProp
                            && string.Equals(dtProp.IRI, "http://xmlns.com/foaf/0.1/age"));
        }

		[TestMethod]
		public void ShouldThrowExceptionOnCreatingHasKeyBecauseNullClassExpression()
			=> Assert.ThrowsException<OWLException>(() => new OWLHasKey(
                null,
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingHasKeyBecauseFoundNullObjectPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [null],
                [new OWLDataProperty(RDFVocabulary.FOAF.AGE)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingHasKeyBecauseFoundNullDataPropertyExpression()
            => Assert.ThrowsException<OWLException>(() => new OWLHasKey(
                new OWLClass(RDFVocabulary.FOAF.AGENT),
                [new OWLObjectProperty(RDFVocabulary.FOAF.KNOWS)],
                [null]));
        #endregion
    }
}