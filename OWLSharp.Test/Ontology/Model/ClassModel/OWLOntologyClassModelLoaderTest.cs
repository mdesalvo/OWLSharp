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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyClassModelLoaderTest
    {
        #region Initialize
        private OWLOntology Ontology { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RDFGraph graph = new RDFGraph();

            //Declarations (Model)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class4"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class5"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:avfromRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:avfromRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:avfromRestr"), RDFVocabulary.OWL.ALL_VALUES_FROM, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:svfromRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:svfromRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:svfromRestr"), RDFVocabulary.OWL.SOME_VALUES_FROM, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hvRestr"), RDFVocabulary.OWL.HAS_VALUE, new RDFResource("ex:indiv1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:selfRestrY"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:selfRestrY"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:selfRestrY"), RDFVocabulary.OWL.HAS_SELF, RDFTypedLiteral.True));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:selfRestrN"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:selfRestrN"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:selfRestrN"), RDFVocabulary.OWL.HAS_SELF, RDFTypedLiteral.False));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:cRestr"), RDFVocabulary.OWL.CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:mincRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:mincRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:mincRestr"), RDFVocabulary.OWL.MIN_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxcRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxcRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxcRestr"), RDFVocabulary.OWL.MAX_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxcRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxcRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxcRestr"), RDFVocabulary.OWL.MIN_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxcRestr"), RDFVocabulary.OWL.MAX_CARDINALITY, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:qcRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:qcRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:qcRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:qcRestr"), RDFVocabulary.OWL.QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minqcRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minqcRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minqcRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minqcRestr"), RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxqcRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxqcRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxqcRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:maxqcRestr"), RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxqcRestr"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxqcRestr"), RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxqcRestr"), RDFVocabulary.OWL.ON_CLASS, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxqcRestr"), RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, new RDFTypedLiteral("1", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:minmaxqcRestr"), RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, new RDFTypedLiteral("2", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:enumclass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:enumclass"), RDFVocabulary.OWL.ONE_OF, new RDFResource("bnode:enumMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:enumMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:enumlitclass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:enumlitclass"), RDFVocabulary.OWL.ONE_OF, new RDFResource("bnode:enumlitMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers"), RDFVocabulary.RDF.FIRST, new RDFPlainLiteral("val1","en-US")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:enumlitMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers2"), RDFVocabulary.RDF.FIRST, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:unionclass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:unionclass"), RDFVocabulary.OWL.UNION_OF, new RDFResource("bnode:unionMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:unionMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:unionMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:unionMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:unionMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:unionMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:unionMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class22")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:unionMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:intersectionclass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:intersectionclass"), RDFVocabulary.OWL.INTERSECTION_OF, new RDFResource("bnode:intersectionMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:intersectionMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:intersectionMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:intersectionMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:intersectionMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:intersectionMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:intersectionMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:intersectionMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:complementclass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:complementclass"), RDFVocabulary.OWL.COMPLEMENT_OF, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointclasses"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointclasses"), RDFVocabulary.OWL.MEMBERS, new RDFResource("bnode:alldisjointMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:alldisjointMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class4")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:disjointunionclass"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:disjointunionclass"), RDFVocabulary.OWL.DISJOINT_UNION_OF, new RDFResource("bnode:disjointunionMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:disjointunionMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:disjointunionMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class4")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:disjointunionMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:disjointunionMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:disjointunionMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:disjointunionMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class5")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:disjointunionMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.OWL.HAS_KEY, new RDFResource("bnode:keys")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:keys"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:keys"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:keys"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:annprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            //Declarations (Data)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv2"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv3"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv4"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class4")));
            //Annotations
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Relations
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class2"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class2"), RDFVocabulary.OWL.EQUIVALENT_CLASS, new RDFResource("ex:class3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.OWL.DISJOINT_WITH, new RDFResource("ex:class5")));

            //Load
            Ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(Ontology);
            Assert.IsNotNull(Ontology.Model);
            Assert.IsNotNull(Ontology.Model.ClassModel);
            Assert.IsNotNull(Ontology.Model.PropertyModel);
            Assert.IsNotNull(Ontology.Data);
        }
        #endregion

        #region Test
        [TestMethod]
        public void ShouldLoadClassDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:class1")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:class2")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:class3")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:class4")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSimpleClass(new RDFResource("ex:class5")));
        }

        [TestMethod]
        public void ShouldLoadDeprecatedClassDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasDeprecatedClass(new RDFResource("ex:class5")));
        }

        [TestMethod]
        public void ShouldLoadClassAnnotations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasAnnotation(new RDFResource("ex:class1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasAnnotation(new RDFResource("ex:class1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
        }

        [TestMethod]
        public void ShouldLoadRestrictionDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasAllValuesFromRestrictionClass(new RDFResource("ex:avfromRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSomeValuesFromRestrictionClass(new RDFResource("ex:svfromRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasValueRestrictionClass(new RDFResource("ex:hvRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSelfRestrictionClass(new RDFResource("ex:selfRestrY")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasSelfRestrictionClass(new RDFResource("ex:selfRestrN")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasCardinalityRestrictionClass(new RDFResource("ex:cRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasMinCardinalityRestrictionClass(new RDFResource("ex:mincRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasMaxCardinalityRestrictionClass(new RDFResource("ex:maxcRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasMinMaxCardinalityRestrictionClass(new RDFResource("ex:minmaxcRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasQualifiedCardinalityRestrictionClass(new RDFResource("ex:qcRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasMinQualifiedCardinalityRestrictionClass(new RDFResource("ex:minqcRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:maxqcRestr")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(new RDFResource("ex:minmaxqcRestr")));
        }

        [TestMethod]
        public void ShouldLoadEnumerateDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasEnumerateClass(new RDFResource("ex:enumclass")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasEnumerateClass(new RDFResource("ex:enumlitclass")));
        }

        [TestMethod]
        public void ShouldLoadCompositeDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasCompositeUnionClass(new RDFResource("ex:unionclass")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasCompositeIntersectionClass(new RDFResource("ex:intersectionclass")));
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasCompositeComplementClass(new RDFResource("ex:complementclass")));
        }

        [TestMethod]
        public void ShouldLoadAllDisjointDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasAllDisjointClasses(new RDFResource("ex:alldisjointclasses")));
        }

        [TestMethod]
        public void ShouldLoadDisjointUnionDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.CheckHasDisjointUnionClass(new RDFResource("ex:disjointunionclass")));
        }

        [TestMethod]
        public void ShouldLoadHasKeyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.GetKeyPropertiesOf(new RDFResource("ex:class1")).Single().Equals(new RDFResource("ex:objprop")));
        }

        [TestMethod]
        public void ShouldLoadSubClassDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.GetSubClassesOf(new RDFResource("ex:class1")).Count == 2);
        }

        [TestMethod]
        public void ShouldLoadSuperClassDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.GetSuperClassesOf(new RDFResource("ex:class2")).Count == 1);
            Assert.IsTrue(Ontology.Model.ClassModel.GetSuperClassesOf(new RDFResource("ex:class3")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadEquivalentClassDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.GetEquivalentClassesOf(new RDFResource("ex:class2")).Count == 1);
            Assert.IsTrue(Ontology.Model.ClassModel.GetEquivalentClassesOf(new RDFResource("ex:class3")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadDisjointClassDeclarations()
        {
            Assert.IsTrue(Ontology.Model.ClassModel.GetDisjointClassesWith(new RDFResource("ex:class1")).Count == 2);
        }
        #endregion
    }
}