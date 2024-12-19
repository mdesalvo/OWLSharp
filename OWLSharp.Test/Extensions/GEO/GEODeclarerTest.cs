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

using OWLSharp.Ontology;
using OWLSharp.Extensions.GEO;
using RDFSharp.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OWLSharp.Test.Extensions.GEO
{
    [TestClass]
    public class GEODeclarerTest
    {
        #region Methods
        [TestMethod]
        public void ShouldDeclarePointFeatureWithDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, 45.464664));

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(null, new RDFResource("ex:MilanGM"), (9.188540, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), null, (9.188540, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (-182.0, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (182.0, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, -92.0)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, 92.0)));
        }

        [TestMethod]
        public void ShouldDeclarePointFeatureWithNotDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, 45.464664), false);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(null, new RDFResource("ex:MilanGM"), (9.188540, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), null, (9.188540, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (-182.0, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (182.0, 45.464664)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, -92.0)));
            Assert.ThrowsException<OWLException>(() => ontology.DeclarePointFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), (9.188540, 92.0)));
        }

        [TestMethod]
        public void ShouldDeclareLineFeatureWithDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664)]);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(null, new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null, null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(-182.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(182.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, -92.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 92.464664),(9.198540, 45.474664)]));
        }

        [TestMethod]
        public void ShouldDeclareLineFeatureWithNotDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664)], false);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(null, new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null, null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(-182.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(182.188540, 45.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, -92.464664),(9.198540, 45.474664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareLineFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 92.464664),(9.198540, 45.474664)]));
        }

        [TestMethod]
        public void ShouldDeclareAreaFeatureWithDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(null, new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null, null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(-182.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(182.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, -92.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 92.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
        }

        [TestMethod]
        public void ShouldDeclareAreaFeatureWithNotDefaultGeometry()
        {
            OWLOntology ontology = new OWLOntology();
            ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664),(9.288540, 45.664664)], false);

            Assert.IsTrue(ontology.DeclarationAxioms.Count == 8);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLClassAssertion>().Count == 3);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLObjectPropertyAssertion>().Count == 1);
            Assert.IsTrue(ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>().Count == 1);

            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(null, new RDFResource("ex:MilanGM"), [(9.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null, null));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), null, [(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(-182.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(182.188540, 45.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, -92.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
            Assert.ThrowsException<OWLException>(() => ontology.DeclareAreaFeature(new RDFResource("ex:MilanFT"), new RDFResource("ex:MilanGM"), [(9.188540, 92.464664),(9.198540, 45.474664),(9.188540, 45.464664)]));
        }
        #endregion
    }
}