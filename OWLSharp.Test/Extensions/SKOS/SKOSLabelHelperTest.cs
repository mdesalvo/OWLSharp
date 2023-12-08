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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OWLSharp.Extensions.SKOS.Test
{
    [TestClass]
    public class SKOSLabelHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCheckHasLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareLabel(new RDFResource("ex:label1"));
            conceptScheme.DeclareLabel(new RDFResource("ex:label2"));

            Assert.IsTrue(conceptScheme.CheckHasLabel(new RDFResource("ex:label1")));
            Assert.IsTrue(conceptScheme.CheckHasLabel(new RDFResource("ex:label2")));
        }

        [TestMethod]
        public void ShouldCheckHasNotLabel()
        {
            SKOSConceptScheme conceptSchemeNULL = null;
            SKOSConceptScheme conceptSchemeEMPTY = new SKOSConceptScheme("ex:conceptSchemeEmpty");
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareLabel(new RDFResource("ex:label1"));
            conceptScheme.DeclareLabel(new RDFResource("ex:label2"));

            Assert.IsFalse(conceptScheme.CheckHasLabel(new RDFResource("ex:label3")));
            Assert.IsFalse(conceptScheme.CheckHasLabel(null));
            Assert.IsFalse(conceptSchemeNULL.CheckHasLabel(new RDFResource("ex:label1")));
            Assert.IsFalse(conceptSchemeEMPTY.CheckHasLabel(new RDFResource("ex:label1")));
        }

        [TestMethod]
        public void ShouldCheckHasLabelWithLiteralForm()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareLabel(new RDFResource("ex:label1"));
            conceptScheme.DeclareLabel(new RDFResource("ex:label2"));
            conceptScheme.DeclareLiteralFormOfLabel(new RDFResource("ex:label1"), new RDFPlainLiteral("aabb"));
            conceptScheme.DeclareLiteralFormOfLabel(new RDFResource("ex:label2"), new RDFPlainLiteral("bbaa"));

            Assert.IsTrue(conceptScheme.CheckHasLabelWithLiteralForm(new RDFResource("ex:label1"), new RDFPlainLiteral("aabb")));
            Assert.IsTrue(conceptScheme.CheckHasLabelWithLiteralForm(new RDFResource("ex:label2"), new RDFPlainLiteral("bbaa")));
            Assert.IsTrue(conceptScheme.CheckHasLabelWithLiteralForm(new RDFResource("ex:label1"), null));
        }

        [TestMethod]
        public void ShouldCheckHasNotLabelWithLiteralForm()
        {
            SKOSConceptScheme conceptSchemeNULL = null;
            SKOSConceptScheme conceptSchemeEMPTY = new SKOSConceptScheme("ex:conceptSchemeEmpty");
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareLabel(new RDFResource("ex:label1"));
            conceptScheme.DeclareLabel(new RDFResource("ex:label2"));
            conceptScheme.DeclareLiteralFormOfLabel(new RDFResource("ex:label1"), new RDFPlainLiteral("aabb"));
            conceptScheme.DeclareLiteralFormOfLabel(new RDFResource("ex:label2"), new RDFPlainLiteral("bbaa"));

            Assert.IsFalse(conceptScheme.CheckHasLabelWithLiteralForm(new RDFResource("ex:label1"), new RDFPlainLiteral("sscc")));
            Assert.IsFalse(conceptScheme.CheckHasLabelWithLiteralForm(null, new RDFPlainLiteral("aabb")));
            Assert.IsFalse(conceptSchemeNULL.CheckHasLabelWithLiteralForm(new RDFResource("ex:label1"), new RDFPlainLiteral("aabb")));
            Assert.IsFalse(conceptSchemeEMPTY.CheckHasLabelWithLiteralForm(new RDFResource("ex:label1"), new RDFPlainLiteral("aabb")));
        }

        [TestMethod]
        public void ShouldDeclareLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareLabel(new RDFResource("ex:label"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 2);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasIndividual(new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckIsIndividualOf(conceptScheme.Ontology.Model, new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LABEL));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.IN_SCHEME, new RDFResource("ex:conceptScheme")));

            //Test counters and enumerators
            Assert.IsTrue(conceptScheme.LabelsCount == 1);
            int i1 = 0;
            IEnumerator<RDFResource> labelsEnumerator = conceptScheme.LabelsEnumerator;
            while (labelsEnumerator.MoveNext()) i1++;
            Assert.IsTrue(i1 == 1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").DeclareLabel(null));

        //ANNOTATIONS

        [TestMethod]
        public void ShouldLiteralAnnotateLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Label!"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Label!")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(null, RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a skos:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(new RDFResource("ex:label"), null, new RDFPlainLiteral("This is a skos:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(new RDFResource("ex:label"), new RDFResource(), new RDFPlainLiteral("This is a skos:Label!")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, null as RDFLiteral));

        [TestMethod]
        public void ShouldResourceAnnotateLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:label"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seeAlso")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(null, RDFVocabulary.RDFS.COMMENT, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(new RDFResource("ex:label"), null, new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(new RDFResource("ex:label"), new RDFResource(), new RDFResource("ex:seeAlso")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme").AnnotateLabel(new RDFResource("ex:label"), RDFVocabulary.RDFS.COMMENT, null as RDFResource));

        //RELATIONS

        [TestMethod]
        public void ShouldDeclarePreferredLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseAlreadyExistingUnlanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label2")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label2' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseAlreadyExistingLanguagedLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label2", "en-US")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label", "en-US")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label2@EN-US' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringPreferredLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("PrefLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclarePreferredLabel(null, new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclarePreferredLabel(new RDFResource("ex:concept"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPreferredLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), null));

        [TestMethod]
        public void ShouldDeclareAlternativeLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.HIDDEN_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithExtendedHiddenLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringAlternativeLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("AltLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareAlternativeLabel(null, new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareAlternativeLabel(new RDFResource("ex:concept"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringAlternativeLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), null));

        [TestMethod]
        public void ShouldDeclareHiddenLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label", "en-US"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("etichetta", "it-IT"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label", "en-US")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("etichetta", "it-IT")));
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.PREF_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithExtendedPreferredLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclarePreferredLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasAnnotation(new RDFResource("ex:concept"), RDFVocabulary.SKOS.ALT_LABEL, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldEmitWarningOnDeclaringHiddenLabelBecauseClashWithExtendedAlternativeLabel()
        {
            string warningMsg = null;
            OWLEvents.OnWarning += (string msg) => { warningMsg = msg; };

            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareAlternativeLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label"));
            conceptScheme.DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), new RDFPlainLiteral("label")); //SKOS violation

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:concept"), RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new RDFResource("ex:label")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFPlainLiteral("label")));
            Assert.IsNotNull(warningMsg);
            Assert.IsTrue(warningMsg.IndexOf("HiddenLabel relation between concept 'ex:concept' and label 'ex:label' with value 'label' cannot be declared to the concept scheme because it would violate SKOS integrity") > -1);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullConcept()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareHiddenLabel(null, new RDFResource("ex:label"), new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareHiddenLabel(new RDFResource("ex:concept"), null, new RDFPlainLiteral("label")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringHiddenLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareHiddenLabel(new RDFResource("ex:concept"), new RDFResource("ex:label"), null));

        [TestMethod]
        public void ShouldDeclareRelatedLabels()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareRelatedLabels(new RDFResource("ex:label1"), new RDFResource("ex:label2"));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:label1"), RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, new RDFResource("ex:label2")));
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:label2"), RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, new RDFResource("ex:label1")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseNullLeftLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedLabels(null, new RDFResource("ex:rightLabel")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseNullRightLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedLabels(new RDFResource("ex:leftLabel"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringRelatedLabelsBecauseSelfLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedLabels(new RDFResource("ex:leftLabel"), new RDFResource("ex:leftLabel")));

        [TestMethod]
        public void ShouldDeclareLiteralFormOfLabel()
        {
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme("ex:conceptScheme");
            conceptScheme.DeclareLiteralFormOfLabel(new RDFResource("ex:label"), new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING));

            //Test evolution of SKOS knowledge
            Assert.IsTrue(conceptScheme.Ontology.URI.Equals(conceptScheme.URI));
            Assert.IsTrue(conceptScheme.Ontology.Model.ClassModel.ClassesCount == 8);
            Assert.IsTrue(conceptScheme.Ontology.Model.PropertyModel.PropertiesCount == 33);
            Assert.IsTrue(conceptScheme.Ontology.Data.IndividualsCount == 1);
            Assert.IsTrue(conceptScheme.Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:label"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLiteralFormOfLabelBecauseNullLabel()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareLiteralFormOfLabel(null, new RDFTypedLiteral("aabbcc", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLiteralFormOfLabelBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new SKOSConceptScheme("ex:conceptScheme")
                        .DeclareRelatedLabels(new RDFResource("ex:label"), null));
        #endregion
    }
}