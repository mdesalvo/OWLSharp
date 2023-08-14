/*
   Copyright 2012-2023 Marco De Salvo

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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyDataTest
    {
        #region Test
        [TestMethod]
        public void ShouldCreateData()
        {
            OWLOntologyData data = new OWLOntologyData();

            Assert.IsNotNull(data);
            Assert.IsNotNull(data.Individuals);
            Assert.IsTrue(data.IndividualsCount == 0);
            Assert.IsTrue(data.AllDifferentCount == 0);
            Assert.IsNotNull(data.ABoxGraph);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 0);
            Assert.IsNotNull(data.OBoxGraph);
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> individualsEnumerator = data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext()) 
                i++;
            Assert.IsTrue(i == 0);

            int j = 0;
            IEnumerator<RDFResource> allDifferentEnumerator = data.AllDifferentEnumerator;
            while (allDifferentEnumerator.MoveNext())
                j++;
            Assert.IsTrue(j == 0);
        }

        [TestMethod]
        public void ShouldDisposeDataWithUsing()
        {
            OWLOntologyData data;
            using (data = new OWLOntologyData())
            {
                Assert.IsFalse(data.Disposed);
                Assert.IsNotNull(data.Individuals);
                Assert.IsNotNull(data.ABoxGraph);
                Assert.IsNotNull(data.OBoxGraph);
            };
            Assert.IsTrue(data.Disposed);
            Assert.IsNull(data.Individuals);
            Assert.IsNull(data.ABoxGraph);
            Assert.IsNull(data.OBoxGraph);
        }

        [TestMethod]
        public void ShouldDeclareIndividual()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivA")); //Will be discarded since duplicate individuals are not allowed

            Assert.IsTrue(data.IndividualsCount == 1);
            Assert.IsTrue(data.AllDifferentCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);

            int i = 0;
            IEnumerator<RDFResource> individualsEnumerator = data.IndividualsEnumerator;
            while (individualsEnumerator.MoveNext())
                i++;
            Assert.IsTrue(i == 1);

            int j = 0;
            IEnumerator<RDFResource> allDifferentEnumerator = data.AllDifferentEnumerator;
            while (allDifferentEnumerator.MoveNext())
                j++;
            Assert.IsTrue(j == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIndividualBecauseNull()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData().DeclareIndividual(null));

        [TestMethod]
        public void ShouldAnnotateResourceIndividual()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.AnnotateIndividual(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));
            data.AnnotateIndividual(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso")); //Will be discarded, since duplicate annotations are not allowed

            Assert.IsTrue(data.IndividualsCount == 1);
            Assert.IsTrue(data.AllDifferentCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.OBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"))));
        }

        [TestMethod]
        public void ShouldAnnotateLiteralIndividual()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.AnnotateIndividual(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label"));
            data.AnnotateIndividual(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label")); //Will be discarded, since duplicate annotations are not allowed

            Assert.IsTrue(data.IndividualsCount == 1);
            Assert.IsTrue(data.AllDifferentCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.OBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label"))));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceIndividualBecauseNullSubject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indiv1"))
                        .AnnotateIndividual(null, RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceIndividualBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indiv1"))
                        .AnnotateIndividual(new RDFResource("ex:indiv1"), null, new RDFResource("ex:seealso")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceIndividualBecauseBlankPredicate()
           => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                       .DeclareIndividual(new RDFResource("ex:indiv1"))
                       .AnnotateIndividual(new RDFResource("ex:indiv1"), new RDFResource(), new RDFResource("ex:seealso")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingResourceIndividualBecauseNullObject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indiv1"))
                        .AnnotateIndividual(new RDFResource("ex:indiv1"), RDFVocabulary.RDFS.SEE_ALSO, null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralIndividualBecauseNullSubject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indiv1"))
                        .AnnotateIndividual(null, RDFVocabulary.RDFS.LABEL, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralIndividualBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indiv1"))
                        .AnnotateIndividual(new RDFResource("ex:indiv1"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralIndividualBecauseBlankPredicate()
           => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                       .DeclareIndividual(new RDFResource("ex:indiv1"))
                       .AnnotateIndividual(new RDFResource("ex:indiv1"), new RDFResource(), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnAnnotatingLiteralIndividualBecauseNullObject()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indiv1"))
                        .AnnotateIndividual(new RDFResource("ex:indiv1"), RDFVocabulary.RDFS.LABEL, null as RDFLiteral));

        [TestMethod]
        public void ShouldDeclareIndividualType()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividualType(new RDFResource("ex:indivA"), new RDFResource("ex:classA"));

            Assert.IsTrue(data.ABoxGraph.TriplesCount == 2);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:classA"))));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringIndividualTypeBecauseReservedClass()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividualType(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.RESOURCE);

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("Type relation between individual 'ex:indivA' and class 'http://www.w3.org/2000/01/rdf-schema#Resource' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIndividualTypeBecauseNullIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareIndividualType(null, new RDFResource("ex:classA")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringIndividualTypeBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareIndividualType(new RDFResource("ex:indivA"), null));

        [TestMethod]
        public void ShouldDeclareSameIndividuals()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivB"));
            data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));

            Assert.IsTrue(data.ABoxGraph.TriplesCount == 4);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.OWL.SAME_AS, new RDFResource("ex:indivB"))));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivB"), RDFVocabulary.OWL.SAME_AS, new RDFResource("ex:indivA"))));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringSameIndividualsBecauseIncompatibleIndividuals()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivB"));
            data.DeclareDifferentIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));  //OWL-DL contraddiction

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("SameAs relation between individual 'ex:indivA' and individual 'ex:indivB' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:indivB"))));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSameIndividualsBecauseNullLeftIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareIndividual(new RDFResource("ex:indivB"))
                        .DeclareSameIndividuals(null, new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringSameIndividualsBecauseNullRightIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareIndividual(new RDFResource("ex:indivB"))
                        .DeclareSameIndividuals(new RDFResource("ex:indivA"), null));
						
		[TestMethod]
        public void ShouldThrowExceptionOnDeclaringSameIndividualsBecauseSelfIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivA")));

        [TestMethod]
        public void ShouldDeclareDifferentIndividuals()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivB"));
            data.DeclareDifferentIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));

            Assert.IsTrue(data.ABoxGraph.TriplesCount == 4);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:indivB"))));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivB"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:indivA"))));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringDifferentIndividualsBecauseIncompatibleIndividuals()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivB"));
            data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            data.DeclareDifferentIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));  //OWL-DL contraddiction

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("DifferentFrom relation between individual 'ex:indivA' and individual 'ex:indivB' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivB"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL)));
            Assert.IsTrue(data.ABoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:indivA"), RDFVocabulary.OWL.SAME_AS, new RDFResource("ex:indivB"))));
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDifferentIndividualsBecauseNullLeftIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareIndividual(new RDFResource("ex:indivB"))
                        .DeclareDifferentIndividuals(null, new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDifferentIndividualsBecauseNullRightIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareIndividual(new RDFResource("ex:indivB"))
                        .DeclareDifferentIndividuals(new RDFResource("ex:indivA"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDifferentIndividualsBecauseSelfIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareDifferentIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivA")));

        [TestMethod]
        public void ShouldDeclareAllDifferentIndividuals()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareAllDifferentIndividuals(new RDFResource("ex:allDiff"), new List<RDFResource>() {
                new RDFResource("ex:indivA"), new RDFResource("ex:indivB"), new RDFResource("ex:indivC") });

            Assert.IsTrue(data.IndividualsCount == 0);
            Assert.IsTrue(data.AllDifferentCount == 1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 11);
            Assert.IsTrue(data.ABoxGraph[new RDFResource("ex:allDiff"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT, null].Any());
            Assert.IsTrue(data.ABoxGraph[new RDFResource("ex:allDiff"), RDFVocabulary.OWL.DISTINCT_MEMBERS, null, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST, null].TriplesCount == 3);
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:indivA"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:indivB"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.FIRST, new RDFResource("ex:indivC"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.REST, null, null].TriplesCount == 3);
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);

            int j = 0;
            IEnumerator<RDFResource> allDifferentEnumerator = data.AllDifferentEnumerator;
            while (allDifferentEnumerator.MoveNext())
                j++;
            Assert.IsTrue(j == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllDifferentIndividualsBecauseNullClass()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareAllDifferentIndividuals(null, new List<RDFResource>() { new RDFResource("ex:indivA") }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllDifferentIndividualsBecauseNullIndividuals()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareAllDifferentIndividuals(new RDFResource("ex:diffClass"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAllDifferentIndividualsBecauseEmptyIndividuals()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareIndividual(new RDFResource("ex:indivA"))
                        .DeclareAllDifferentIndividuals(new RDFResource("ex:diffClass"), new List<RDFResource>()));

        [TestMethod]
        public void ShouldDeclareObjectAssertion()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareObjectAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:indivB"));

            Assert.IsTrue(data.IndividualsCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph[new RDFResource("ex:indivA"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:indivB"), null].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringObjectAssertionBecauseNullLeftIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareObjectAssertion(null, RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringObjectAssertionBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareObjectAssertion(new RDFResource("ex:indivB"), null, new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringObjectAssertionBecauseBlankPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareObjectAssertion(new RDFResource("ex:indivB"), new RDFResource(), new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringObjectAssertionBecauseNullRightIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareObjectAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.KNOWS, null));

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringObjectAssertionBecauseReservedProperty()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareObjectAssertion(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:indivB")); //Reserved annotation property (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("ObjectAssertion relation between individual 'ex:indivA' and individual 'ex:indivB' through property 'http://www.w3.org/2000/01/rdf-schema#seeAlso' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 0);
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringObjectAssertionBecauseIncompatibleObjectAssertion()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareNegativeObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:objProp"), new RDFResource("ex:indivB"));
            data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:objProp"), new RDFResource("ex:indivB")); //OWL-DL contraddiction (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("ObjectAssertion relation between individual 'ex:indivA' and individual 'ex:indivB' through property 'ex:objProp' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 4);
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indivA"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFResource("ex:objProp"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:indivB"), null].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeclareDatatypeAssertion()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("name"));

            Assert.IsTrue(data.IndividualsCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph[new RDFResource("ex:indivA"), RDFVocabulary.FOAF.NAME, null, new RDFPlainLiteral("name")].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDatatypeAssertionBecauseNullIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareDatatypeAssertion(null, RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("name")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDatatypeAssertionBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareDatatypeAssertion(new RDFResource("ex:indivA"), null, new RDFPlainLiteral("name")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDatatypeAssertionBecauseBlankPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource(), new RDFPlainLiteral("name")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringDatatypeAssertionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareDatatypeAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.NAME, null));

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringDatatypeAssertionBecauseReservedProperty()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFPlainLiteral("name")); //Reserved annotation property (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("DatatypeAssertion relation between individual 'ex:indivA' and value 'name' through property 'http://www.w3.org/2000/01/rdf-schema#seeAlso' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 0);
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringDatatypeAssertionBecauseIncompatibleDatatypeAssertion()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("name"));
            data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("name")); //OWL-DL contraddiction (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("DatatypeAssertion relation between individual 'ex:indivA' and value 'name' through property 'ex:dtProp' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 4);
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indivA"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFResource("ex:dtProp"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.TARGET_VALUE, null, new RDFPlainLiteral("name")].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeclareNegativeObjectAssertion()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareNegativeObjectAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:indivB"));

            Assert.IsTrue(data.IndividualsCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 4);
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indivA"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.KNOWS, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:indivB"), null].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeObjectAssertionBecauseNullLeftIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeObjectAssertion(null, RDFVocabulary.FOAF.KNOWS, new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeObjectAssertionBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeObjectAssertion(new RDFResource("ex:indivB"), null, new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeObjectAssertionBecauseBlankPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeObjectAssertion(new RDFResource("ex:indivB"), new RDFResource(), new RDFResource("ex:indivB")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeObjectAssertionBecauseNullRightIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeObjectAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.KNOWS, null));

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNegativeObjectAssertionBecauseReservedProperty()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareNegativeObjectAssertion(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:indivB")); //Reserved annotation property (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NegativeObjectAssertion relation between individual 'ex:indivA' and individual 'ex:indivB' through property 'http://www.w3.org/2000/01/rdf-schema#seeAlso' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 0);
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNegativeObjectAssertionBecauseIncompatibleNegativeObjectAssertion()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:objProp"), new RDFResource("ex:indivB"));
            data.DeclareNegativeObjectAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:objProp"), new RDFResource("ex:indivB")); //OWL-DL contraddiction (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NegativeObjectAssertion relation between individual 'ex:indivA' and individual 'ex:indivB' through property 'ex:objProp' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph[new RDFResource("ex:indivA"), new RDFResource("ex:objProp"), new RDFResource("ex:indivB"), null].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldDeclareNegativeDatatypeAssertion()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("name"));

            Assert.IsTrue(data.IndividualsCount == 0);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 4);
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indivA"), null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.ASSERTION_PROPERTY, RDFVocabulary.FOAF.NAME, null].Any());
            Assert.IsTrue(data.ABoxGraph[null, RDFVocabulary.OWL.TARGET_VALUE, null, new RDFPlainLiteral("name")].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeDatatypeAssertionBecauseNullIndividual()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeDatatypeAssertion(null, RDFVocabulary.FOAF.NAME, new RDFPlainLiteral("name")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeDatatypeAssertionBecauseNullPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), null, new RDFPlainLiteral("name")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeDatatypeAssertionBecauseBlankPredicate()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource(), new RDFPlainLiteral("name")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringNegativeDatatypeAssertionBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntologyData()
                        .DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), RDFVocabulary.FOAF.NAME, null));

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNegativeDatatypeAssertionBecauseReservedProperty()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.SEE_ALSO, new RDFPlainLiteral("name")); //Reserved annotation property (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NegativeDatatypeAssertion relation between individual 'ex:indivA' and value 'name' through property 'http://www.w3.org/2000/01/rdf-schema#seeAlso' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 0);
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringNegativeDatatypeAssertionBecauseIncompatibleNegativeDatatypeAssertion()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            OWLOntologyData data = new OWLOntologyData();
            data.DeclareDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("name"));
            data.DeclareNegativeDatatypeAssertion(new RDFResource("ex:indivA"), new RDFResource("ex:dtProp"), new RDFPlainLiteral("name")); //OWL-DL contraddiction (not allowed by policy)

            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("NegativeDatatypeAssertion relation between individual 'ex:indivA' and value 'name' through property 'ex:dtProp' cannot be declared to the data because it would violate OWL-DL integrity") > -1);
            Assert.IsTrue(data.ABoxGraph.TriplesCount == 1);
            Assert.IsTrue(data.ABoxGraph[new RDFResource("ex:indivA"), new RDFResource("ex:dtProp"), null, new RDFPlainLiteral("name")].Any());
            Assert.IsTrue(data.OBoxGraph.TriplesCount == 0);
        }

        [TestMethod]
        public void ShouldExportToGraph()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivB"));
            data.DeclareIndividualType(new RDFResource("ex:indivA"), new RDFResource("ex:classA"));
            data.DeclareIndividualType(new RDFResource("ex:indivB"), new RDFResource("ex:classB"));
            data.AnnotateIndividual(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            data.AnnotateIndividual(new RDFResource("ex:indivB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));
            data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            RDFGraph graph = data.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 8);
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsync()
        {
            OWLOntologyData data = new OWLOntologyData();
            data.DeclareIndividual(new RDFResource("ex:indivA"));
            data.DeclareIndividual(new RDFResource("ex:indivB"));
            data.DeclareIndividualType(new RDFResource("ex:indivA"), new RDFResource("ex:classA"));
            data.DeclareIndividualType(new RDFResource("ex:indivB"), new RDFResource("ex:classB"));
            data.AnnotateIndividual(new RDFResource("ex:indivA"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"));
            data.AnnotateIndividual(new RDFResource("ex:indivB"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("title"));
            data.DeclareSameIndividuals(new RDFResource("ex:indivA"), new RDFResource("ex:indivB"));
            RDFGraph graph = await data.ToRDFGraphAsync();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph.TriplesCount == 8);
        }
        #endregion
    }
}