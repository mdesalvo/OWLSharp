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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology.Expressions;
using RDFSharp.Model;

namespace OWLSharp.Test.Ontology.Expressions
{
    [TestClass]
    public class OWLExpressionHelperTest
    {
        #region Tests
		[TestMethod]
		public void ShouldRemoveDuplicates()
		{
			OWLDataOneOf dataOneOf = new OWLDataOneOf([new OWLLiteral(new RDFPlainLiteral("hello","en"))]);
			OWLNamedIndividual namedIdv = new OWLNamedIndividual(new RDFResource("ex:Mark"));

			List<OWLExpression>	expressions = new List<OWLExpression>()
			{
				new OWLClass(RDFVocabulary.FOAF.PERSON),
				namedIdv,
				new OWLClass(RDFVocabulary.FOAF.AGENT),
				new OWLClass(RDFVocabulary.FOAF.PERSON),
				dataOneOf,
				dataOneOf,
				namedIdv
			};

			Assert.IsTrue(OWLExpressionHelper.RemoveDuplicates(expressions).Count == 4);
			Assert.IsTrue(OWLExpressionHelper.RemoveDuplicates(new List<OWLExpression>()).Count == 0);
			Assert.IsTrue(OWLExpressionHelper.RemoveDuplicates(null as List<OWLExpression>).Count == 0);
		}
		#endregion
	}
}