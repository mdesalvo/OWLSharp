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
using RDFSharp.Query;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLUtilitiesTest
    {
        #region Tests
        [TestMethod]
        public void ShouldApplyQueryToOntologyWithReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:ClsA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:ClsB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:ClsB"),new RDFResource("ex:ClsA"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:Idv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:Idv1"), new RDFResource("ex:ClsA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:Idv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:Idv2"), new RDFResource("ex:ClsB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:Idv3"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:Idv2"), new RDFResource("ex:Idv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:Idv1"), new RDFResource("ex:objProp"), new RDFResource("ex:Idv2"));

            OWLReasoner reasoner = new OWLReasoner()
                .AddRule(OWLEnums.OWLReasonerRules.IndividualTypeEntailment)
                .AddRule(OWLEnums.OWLReasonerRules.SameAsEntailment);

            RDFSelectQuery query = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:ClsA")))
                    .AddPattern(new RDFPattern(new RDFVariable("?IDV"), new RDFResource("ex:objProp"), new RDFVariable("?IDV2"))));

            RDFSelectQueryResult result = query.ApplyToOntology(ontology, reasoner);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.SelectResultsCount == 2);
            Assert.IsTrue(string.Equals(result.SelectResults.Rows[0]["?IDV"], "ex:Idv1"));
            Assert.IsTrue(string.Equals(result.SelectResults.Rows[0]["?IDV2"], "ex:Idv2"));
            Assert.IsTrue(string.Equals(result.SelectResults.Rows[1]["?IDV"], "ex:Idv1"));
            Assert.IsTrue(string.Equals(result.SelectResults.Rows[1]["?IDV2"], "ex:Idv3"));
        }

        [TestMethod]
        public void ShouldApplyQueryToOntologyWithoutReasoner()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:ClsA"));
            ontology.Model.ClassModel.DeclareClass(new RDFResource("ex:ClsB"));
            ontology.Model.ClassModel.DeclareSubClasses(new RDFResource("ex:ClsB"), new RDFResource("ex:ClsA"));
            ontology.Model.PropertyModel.DeclareObjectProperty(new RDFResource("ex:objProp"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:Idv1"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:Idv1"), new RDFResource("ex:ClsA"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:Idv2"));
            ontology.Data.DeclareIndividualType(new RDFResource("ex:Idv2"), new RDFResource("ex:ClsB"));
            ontology.Data.DeclareIndividual(new RDFResource("ex:Idv3"));
            ontology.Data.DeclareSameIndividuals(new RDFResource("ex:Idv2"), new RDFResource("ex:Idv3"));
            ontology.Data.DeclareObjectAssertion(new RDFResource("ex:Idv1"), new RDFResource("ex:objProp"), new RDFResource("ex:Idv2"));

            RDFSelectQuery query = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?IDV"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:ClsA")))
                    .AddPattern(new RDFPattern(new RDFVariable("?IDV"), new RDFResource("ex:objProp"), new RDFVariable("?IDV2"))));

            RDFSelectQueryResult result = query.ApplyToOntology(ontology);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.SelectResultsCount == 1);
            Assert.IsTrue(string.Equals(result.SelectResults.Rows[0]["?IDV"], "ex:Idv1"));
            Assert.IsTrue(string.Equals(result.SelectResults.Rows[0]["?IDV2"], "ex:Idv2"));
        }
        #endregion
    }
}