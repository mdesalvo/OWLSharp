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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyLoaderTest
    {
        #region Initialize
        private OWLOntology Ontology { get; set; }
        private OWLOntology OntologyWithExtensions { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            RDFGraph graph = new RDFGraph();

            //Declarations (Ontology)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            //Declarations (ClassModel)
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
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers"), RDFVocabulary.RDF.FIRST, new RDFPlainLiteral("hello")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:enumlitMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:enumlitMembers2"), RDFVocabulary.RDF.FIRST, new RDFTypedLiteral("25", RDFModelEnums.RDFDatatypes.XSD_LONG)));
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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointclasses"), RDFVocabulary.OWL.MEMBERS, new RDFResource("bnode:alldisjointClassMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointClassMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointClassMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointClassMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:alldisjointClassMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointClassMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointClassMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:class4")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointClassMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
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
            //Declarations (PropertyModel)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop4"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DEPRECATED_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:annprop"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ANNOTATION_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.TRANSITIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INVERSE_FUNCTIONAL_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop3"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ASYMMETRIC_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop4"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.IRREFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop5"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.SYMMETRIC_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop6"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:dtprop2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.FUNCTIONAL_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:propertyChainAxiom"), RDFVocabulary.OWL.PROPERTY_CHAIN_AXIOM, new RDFResource("bnode:pcaMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop11")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:pcaMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:pcaMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointproperties"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_PROPERTIES));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldisjointproperties"), RDFVocabulary.OWL.MEMBERS, new RDFResource("bnode:alldisjointPropertyMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointPropertyMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointPropertyMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointPropertyMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:alldisjointPropertyMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointPropertyMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointPropertyMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:objprop5")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:alldisjointPropertyMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            //Declarations (Data)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv2"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv3"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv4"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class4")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NAMED_INDIVIDUAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.INDIVIDUAL));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv3"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv4"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldiff"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DIFFERENT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:alldiff"), RDFVocabulary.OWL.DISTINCT_MEMBERS, new RDFResource("bnode:distinctMembers")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv3")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers"), RDFVocabulary.RDF.REST, new RDFResource("bnode:distinctMembers2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers2"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDF.LIST));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers2"), RDFVocabulary.RDF.FIRST, new RDFResource("ex:indiv4")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:distinctMembers2"), RDFVocabulary.RDF.REST, RDFVocabulary.RDF.NIL));
            //Annotations (Ontology)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Annotations (ClassModel)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Annotations (PropertyModel)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Annotations (Data)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv2"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
            //Relations (ClassModel)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class2"), RDFVocabulary.RDFS.SUB_CLASS_OF, new RDFResource("ex:class1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class2"), RDFVocabulary.OWL.EQUIVALENT_CLASS, new RDFResource("ex:class3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:class1"), RDFVocabulary.OWL.DISJOINT_WITH, new RDFResource("ex:class5")));
            //Relations (PropertyModel)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.RDFS.SUB_PROPERTY_OF, new RDFResource("ex:objprop1")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop2"), RDFVocabulary.OWL.EQUIVALENT_PROPERTY, new RDFResource("ex:objprop3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.OWL.PROPERTY_DISJOINT_WITH, new RDFResource("ex:objprop4")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:objprop1"), RDFVocabulary.OWL.INVERSE_OF, new RDFResource("ex:objprop6")));
            //Relations (Data)
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.OWL.SAME_AS, new RDFResource("ex:indiv2")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), RDFVocabulary.OWL.DIFFERENT_FROM, new RDFResource("ex:indiv3")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indiv1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFResource("ex:objprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negobjasn"), RDFVocabulary.OWL.TARGET_INDIVIDUAL, new RDFResource("ex:indiv2")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.NEGATIVE_PROPERTY_ASSERTION));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.OWL.SOURCE_INDIVIDUAL, new RDFResource("ex:indiv1")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.OWL.ASSERTION_PROPERTY, new RDFResource("ex:dtprop")));
            graph.AddTriple(new RDFTriple(new RDFResource("bnode:negdtasn"), RDFVocabulary.OWL.TARGET_VALUE, new RDFPlainLiteral("negval")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv3")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("val")));

            //Load
            Ontology = OWLOntology.FromRDFGraph(graph);
            OntologyWithExtensions = OWLOntology.FromRDFGraph(graph, new OWLOntologyLoaderOptions() { 
                EnableGEOSupport=true, EnableTIMESupport=true });

            Assert.IsNotNull(Ontology);
            Assert.IsNotNull(Ontology.Model);
            Assert.IsNotNull(Ontology.Model.ClassModel);
            Assert.IsNotNull(Ontology.Model.PropertyModel);
            Assert.IsNotNull(Ontology.Data);
            Assert.IsFalse(Ontology.Model.ClassModel.CheckHasClass(RDFVocabulary.GEOSPARQL.SF.POINT));
            Assert.IsFalse(Ontology.Model.ClassModel.CheckHasClass(RDFVocabulary.TIME.INSTANT));

            Assert.IsNotNull(OntologyWithExtensions);
            Assert.IsNotNull(OntologyWithExtensions.Model);
            Assert.IsNotNull(OntologyWithExtensions.Model.ClassModel);
            Assert.IsNotNull(OntologyWithExtensions.Model.PropertyModel);
            Assert.IsNotNull(OntologyWithExtensions.Data);
            Assert.IsTrue(OntologyWithExtensions.Model.ClassModel.CheckHasClass(RDFVocabulary.GEOSPARQL.SF.POINT));
            Assert.IsTrue(OntologyWithExtensions.Model.ClassModel.CheckHasClass(RDFVocabulary.TIME.INSTANT));
        }
        #endregion

        #region Test (Ontology)
        [TestMethod]
        public void ShouldLoadOntologyURI()
        {
            Assert.IsTrue(Ontology.Equals(new RDFResource("ex:ont")));
        }

        [TestMethod]
        public void ShouldLoadOntologyAnnotations()
        {
            Assert.IsTrue(Ontology.OBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment"))));
            Assert.IsTrue(Ontology.OBoxGraph.ContainsTriple(new RDFTriple(new RDFResource("ex:ont"), new RDFResource("ex:annprop"), new RDFResource("ex:res"))));
        }
        #endregion

        #region Test (ClassModel)
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

        #region Test (PropertyModel)
        [TestMethod]
        public void ShouldLoadAnnotationPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAnnotationProperty(new RDFResource("ex:annprop")));
        }

        [TestMethod]
        public void ShouldLoadDatatypePropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDatatypeProperty(new RDFResource("ex:dtprop")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDatatypeProperty(new RDFResource("ex:dtprop2")));
        }

        [TestMethod]
        public void ShouldLoadObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop1")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop2")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop3")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop4")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop5")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:objprop6")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasObjectProperty(new RDFResource("ex:propertyChainAxiom")));
        }

        [TestMethod]
        public void ShouldLoadDeprecatedPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDeprecatedProperty(new RDFResource("ex:objprop4")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasDeprecatedProperty(new RDFResource("ex:dtprop")));
        }

        [TestMethod]
        public void ShouldLoadSymmetricObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasSymmetricProperty(new RDFResource("ex:objprop1")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasSymmetricProperty(new RDFResource("ex:objprop5")));
        }

        [TestMethod]
        public void ShouldLoadTransitiveObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasTransitiveProperty(new RDFResource("ex:objprop1")));
        }

        [TestMethod]
        public void ShouldLoadFunctionalPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasFunctionalProperty(new RDFResource("ex:objprop2")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasFunctionalProperty(new RDFResource("ex:dtprop2")));
        }

        [TestMethod]
        public void ShouldLoadInverseFunctionalObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasInverseFunctionalProperty(new RDFResource("ex:objprop2")));
        }

        [TestMethod]
        public void ShouldLoadAsymmetricObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAsymmetricProperty(new RDFResource("ex:objprop3")));
        }

        [TestMethod]
        public void ShouldLoadReflexiveObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasReflexiveProperty(new RDFResource("ex:objprop3")));
        }

        [TestMethod]
        public void ShouldLoadIrreflexiveObjectPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasIrreflexiveProperty(new RDFResource("ex:objprop4")));
        }

        [TestMethod]
        public void ShouldLoadAllDisjointPropertiesDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAllDisjointProperties(new RDFResource("ex:alldisjointproperties")));
        }

        [TestMethod]
        public void ShouldLoadPropertyChainAxiomDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasPropertyChainAxiom(new RDFResource("ex:propertyChainAxiom")));
        }

        [TestMethod]
        public void ShouldLoadSubPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetSubPropertiesOf(new RDFResource("ex:objprop1")).Count == 2);
        }

        [TestMethod]
        public void ShouldLoadSuperPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetSuperPropertiesOf(new RDFResource("ex:objprop2")).Count == 1);
            Assert.IsTrue(Ontology.Model.PropertyModel.GetSuperPropertiesOf(new RDFResource("ex:objprop3")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadEquivalentPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:objprop2")).Count == 1);
            Assert.IsTrue(Ontology.Model.PropertyModel.GetEquivalentPropertiesOf(new RDFResource("ex:objprop3")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadDisjointPropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetDisjointPropertiesWith(new RDFResource("ex:objprop1")).Count == 2);
        }

        [TestMethod]
        public void ShouldLoadInversePropertyDeclarations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.GetInversePropertiesOf(new RDFResource("ex:objprop1")).Count == 1);
            Assert.IsTrue(Ontology.Model.PropertyModel.GetInversePropertiesOf(new RDFResource("ex:objprop6")).Count == 1);
        }

        [TestMethod]
        public void ShouldLoadPropertyAnnotations()
        {
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:objprop1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            Assert.IsTrue(Ontology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:objprop1"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
        }
        #endregion

        #region Test (Data)
        [TestMethod]
        public void ShouldLoadIndividualDeclarations()
        {
            Assert.IsTrue(Ontology.Data.IndividualsCount == 4);
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv1")));
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv2")));
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv3")));
            Assert.IsTrue(Ontology.Data.CheckHasIndividual(new RDFResource("ex:indiv4")));
        }

        [TestMethod]
        public void ShouldLoadIndividualAnnotations()
        {
            Assert.IsTrue(Ontology.Data.CheckHasAnnotation(new RDFResource("ex:indiv1"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("comment")));
            Assert.IsTrue(Ontology.Data.CheckHasAnnotation(new RDFResource("ex:indiv2"), new RDFResource("ex:annprop"), new RDFResource("ex:res")));
        }

        [TestMethod]
        public void ShouldLoadSameAsRelations()
        {
            Assert.IsTrue(Ontology.Data.CheckIsSameIndividual(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv2")));
            Assert.IsTrue(Ontology.Data.CheckIsSameIndividual(new RDFResource("ex:indiv2"), new RDFResource("ex:indiv1")));
        }

        [TestMethod]
        public void ShouldLoadDifferentFromRelations()
        {
            Assert.IsTrue(Ontology.Data.CheckIsDifferentIndividual(new RDFResource("ex:indiv1"), new RDFResource("ex:indiv3")));
            Assert.IsTrue(Ontology.Data.CheckIsDifferentIndividual(new RDFResource("ex:indiv3"), new RDFResource("ex:indiv1")));
        }

        [TestMethod]
        public void ShouldLoadNegativeObjectAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasNegativeObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv2")));

        [TestMethod]
        public void ShouldLoadNegativeDatatypeAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasNegativeDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("negval")));

        [TestMethod]
        public void ShouldLoadObjectAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasObjectAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:objprop"), new RDFResource("ex:indiv3")));

        [TestMethod]
        public void ShouldLoadDatatypeAssertions()
            => Assert.IsTrue(Ontology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:indiv1"), new RDFResource("ex:dtprop"), new RDFPlainLiteral("val")));

        [TestMethod]
        public void ShouldLoadAllDifferentRelations()
        {
            Assert.IsTrue(Ontology.Data.AllDifferentCount == 1);
            IEnumerator<RDFResource> alldiffEnum = Ontology.Data.AllDifferentEnumerator;
            while (alldiffEnum.MoveNext())
                Assert.IsTrue(alldiffEnum.Current.Equals(new RDFResource("ex:alldiff")));
            Ontology.Data.CheckIsDifferentIndividual(new RDFResource("ex:indiv3"), new RDFResource("ex:indiv4"));
        }
        #endregion

        [TestMethod]
        public void ShouldLoadComplexOntology()
        {
            string ontString =
@"<!DOCTYPE rdf:RDF [
    <!ENTITY owl ""http://www.w3.org/2002/07/owl#"" >
    <!ENTITY xsd ""http://www.w3.org/2001/XMLSchema#"" >
    <!ENTITY rdfs ""http://www.w3.org/2000/01/rdf-schema#"" >
    <!ENTITY otherOnt ""http://example.org/otherOntologies/families/"" >
]>
 
 <rdf:RDF xml:base=""http://example.com/owl/families/""
   xmlns=""http://example.com/owl/families/""
   xmlns:otherOnt=""http://example.org/otherOntologies/families/""
   xmlns:owl=""http://www.w3.org/2002/07/owl#""
   xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
   xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
   xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
 
   <owl:Ontology rdf:about=""http://example.com/owl/families"">
     <owl:imports rdf:resource=""http://example.org/otherOntologies/families.owl"" />
   </owl:Ontology>
 
 
   <owl:ObjectProperty rdf:about=""hasWife"">
     <rdfs:subPropertyOf rdf:resource=""hasSpouse""/>
     <rdfs:domain rdf:resource=""Man""/>
     <rdfs:range rdf:resource=""Woman""/>
   </owl:ObjectProperty>
 
   <owl:ObjectProperty rdf:about=""hasParent"">
     <owl:inverseOf rdf:resource=""hasChild""/>
     <owl:propertyDisjointWith rdf:resource=""hasSpouse""/>
   </owl:ObjectProperty>
 
   <owl:ObjectProperty rdf:about=""hasSon"">
     <owl:propertyDisjointWith rdf:resource=""hasDaughter""/>
   </owl:ObjectProperty>
 
   <owl:ObjectProperty rdf:about=""hasFather"">
     <rdfs:subPropertyOf rdf:resource=""hasParent""/>
   </owl:ObjectProperty>
 
   <owl:SymmetricProperty rdf:about=""hasSpouse""/>
   <owl:AsymmetricProperty rdf:about=""hasChild""/>
   <owl:ReflexiveProperty rdf:about=""hasRelative""/>
   <owl:IrreflexiveProperty rdf:about=""parentOf""/>
   <owl:FunctionalProperty rdf:about=""hasHusband""/>
   <owl:InverseFunctionalProperty rdf:about=""hasHusband""/>
   <owl:TransitiveProperty rdf:about=""hasAncestor""/>
 
   <rdf:Description rdf:about=""hasGrandparent"">
     <owl:propertyChainAxiom rdf:parseType=""Collection"">
       <owl:ObjectProperty rdf:about=""hasParent""/>
       <owl:ObjectProperty rdf:about=""hasParent""/>
     </owl:propertyChainAxiom>
   </rdf:Description>
 
   <rdf:Description rdf:about=""hasUncle"">
     <owl:propertyChainAxiom rdf:parseType=""Collection"">
       <owl:ObjectProperty rdf:about=""hasFather""/>
       <owl:ObjectProperty rdf:about=""hasBrother""/>
     </owl:propertyChainAxiom>
   </rdf:Description>

   <owl:ObjectProperty rdf:about=""hasChild"">
     <owl:equivalentProperty rdf:resource=""&otherOnt;child""/>
   </owl:ObjectProperty>
 
   <owl:DatatypeProperty rdf:about=""hasAge"">
     <rdfs:domain rdf:resource=""Person""/>
     <rdfs:range rdf:resource=""&xsd;nonNegativeInteger""/>
     <owl:equivalentProperty rdf:resource=""&otherOnt;age""/>
   </owl:DatatypeProperty>
   <owl:FunctionalProperty rdf:about=""hasAge""/>
 
   <owl:Class rdf:about=""Woman"">
     <rdfs:subClassOf rdf:resource=""Person""/>
   </owl:Class>
 
   <owl:Class rdf:about=""Mother"">
     <rdfs:subClassOf rdf:resource=""Woman""/>
     <owl:equivalentClass>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Woman""/>
           <owl:Class rdf:about=""Parent""/>
         </owl:intersectionOf>
       </owl:Class>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""Person"">
     <rdfs:comment>Represents the set of all people.</rdfs:comment>
     <owl:equivalentClass rdf:resource=""Human""/>
     <owl:hasKey rdf:parseType=""Collection"">
       <owl:DataProperty rdf:about=""hasSSN""/>
     </owl:hasKey>
   </owl:Class>
 
   <owl:Class rdf:about=""Parent"">
     <owl:equivalentClass>
       <owl:Class>
         <owl:unionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Mother""/>
           <owl:Class rdf:about=""Father""/>
         </owl:unionOf>
       </owl:Class>
     </owl:equivalentClass>
     <owl:equivalentClass>
       <owl:Restriction>
         <owl:onProperty rdf:resource=""hasChild""/>
         <owl:someValuesFrom rdf:resource=""Person""/>
       </owl:Restriction>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""ChildlessPerson"">
     <owl:equivalentClass>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Person""/>
           <owl:Class>
             <owl:complementOf rdf:resource=""Parent""/>
           </owl:Class>
         </owl:intersectionOf>
       </owl:Class>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""Grandfather"">
     <rdfs:subClassOf>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Man""/>
           <owl:Class rdf:about=""Parent""/>
         </owl:intersectionOf>
       </owl:Class>
     </rdfs:subClassOf>
   </owl:Class>
 
   <owl:Class rdf:about=""HappyPerson"">
     <owl:equivalentClass>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Restriction>
             <owl:onProperty rdf:resource=""hasChild""/>
             <owl:allValuesFrom rdf:resource=""HappyPerson""/>
           </owl:Restriction>
           <owl:Restriction>
             <owl:onProperty rdf:resource=""hasChild""/>
             <owl:someValuesFrom rdf:resource=""HappyPerson""/>
           </owl:Restriction>
         </owl:intersectionOf>
       </owl:Class>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""JohnsChildren"">
     <owl:equivalentClass>
       <owl:Restriction>
         <owl:onProperty rdf:resource=""hasParent""/>
         <owl:hasValue rdf:resource=""John""/>
       </owl:Restriction>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""NarcisticPerson"">
     <owl:equivalentClass>
       <owl:Restriction>
         <owl:onProperty rdf:resource=""loves""/>
         <owl:hasSelf rdf:datatype=""&xsd;boolean"">
           true
         </owl:hasSelf>
       </owl:Restriction>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""MyBirthdayGuests"">
     <owl:equivalentClass>
       <owl:Class>
         <owl:oneOf rdf:parseType=""Collection"">
           <rdf:Description rdf:about=""Bill""/>
           <rdf:Description rdf:about=""John""/>
           <rdf:Description rdf:about=""Mary""/>
         </owl:oneOf>
       </owl:Class>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""Orphan"">
     <owl:equivalentClass>
       <owl:Restriction>
         <owl:onProperty>
           <owl:ObjectProperty>
             <owl:inverseOf rdf:resource=""hasChild""/>
           </owl:ObjectProperty>
         </owl:onProperty>
         <owl:allValuesFrom rdf:resource=""Dead""/>
       </owl:Restriction>
     </owl:equivalentClass>
   </owl:Class>
 
   <owl:Class rdf:about=""Man"">
     <rdfs:subClassOf rdf:resource=""Person""/>
   </owl:Class>
   <owl:Axiom>
     <owl:annotatedSource rdf:resource=""Man""/>
     <owl:annotatedProperty rdf:resource=""&rdfs;subClassOf""/>
     <owl:annotatedTarget rdf:resource=""Person""/>
     <rdfs:comment>States that every man is a person.</rdfs:comment>
   </owl:Axiom>
 
   <owl:Class rdf:about=""Adult"">
     <owl:equivalentClass rdf:resource=""&otherOnt;Grownup""/>
   </owl:Class>
 
   <owl:Class rdf:about=""Father"">
     <rdfs:subClassOf>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Man""/>
           <owl:Class rdf:about=""Parent""/>
         </owl:intersectionOf>
       </owl:Class>
     </rdfs:subClassOf>
   </owl:Class>
 
   <owl:Class rdf:about=""ChildlessPerson"">
     <rdfs:subClassOf>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Person""/>
           <owl:Class>
             <owl:complementOf>
               <owl:Restriction>
                 <owl:onProperty>
                   <owl:ObjectProperty>
                     <owl:inverseOf rdf:resource=""hasParent""/>
                   </owl:ObjectProperty>
                 </owl:onProperty>
                 <owl:someValuesFrom rdf:resource=""&owl;Thing""/>
               </owl:Restriction>
             </owl:complementOf>
           </owl:Class>
         </owl:intersectionOf>
       </owl:Class>
     </rdfs:subClassOf>
   </owl:Class>
 
   <owl:Class>
     <owl:intersectionOf rdf:parseType=""Collection"">
       <owl:Class>
         <owl:oneOf rdf:parseType=""Collection"">
           <rdf:Description rdf:about=""Mary""/>
           <rdf:Description rdf:about=""Bill""/>
           <rdf:Description rdf:about=""Meg""/>
         </owl:oneOf>
       </owl:Class>
       <owl:Class rdf:about=""Female""/>
     </owl:intersectionOf>
     <rdfs:subClassOf>
       <owl:Class>
         <owl:intersectionOf rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Parent""/>
           <owl:Restriction>
             <owl:maxCardinality rdf:datatype=""&xsd;nonNegativeInteger"">
               1
             </owl:maxCardinality>
             <owl:onProperty rdf:resource=""hasChild""/>
           </owl:Restriction>
           <owl:Restriction>
             <owl:onProperty rdf:resource=""hasChild""/>
             <owl:allValuesFrom rdf:resource=""Female""/>
           </owl:Restriction>
         </owl:intersectionOf>
       </owl:Class>
     </rdfs:subClassOf>
   </owl:Class>
 
   <owl:AllDisjointClasses>
     <owl:members rdf:parseType=""Collection"">
       <owl:Class rdf:about=""Woman""/>
       <owl:Class rdf:about=""Man""/>
     </owl:members>
   </owl:AllDisjointClasses>
 
   <owl:AllDisjointClasses>
     <owl:members rdf:parseType=""Collection"">
       <owl:Class rdf:about=""Mother""/>
       <owl:Class rdf:about=""Father""/>
       <owl:Class rdf:about=""YoungChild""/>
     </owl:members>
   </owl:AllDisjointClasses> 
 
   <Person rdf:about=""Mary"">
     <rdf:type rdf:resource=""Woman""/>
     <owl:sameAs rdf:resource=""&otherOnt;MaryBrown""/>
   </Person>
 
   <owl:NamedIndividual rdf:about=""James"">
     <owl:sameAs rdf:resource=""Jim""/>
   </owl:NamedIndividual>
 
   <rdf:Description rdf:about=""Jack"">
     <rdf:type>
       <owl:Class>
         <owl:intersectionOf  rdf:parseType=""Collection"">
           <owl:Class rdf:about=""Person""/>
           <owl:Class>
             <owl:complementOf rdf:resource=""Parent""/>
           </owl:Class>
         </owl:intersectionOf>
       </owl:Class>
     </rdf:type>
   </rdf:Description>
 
   <owl:NamedIndividual rdf:about=""John"">
     <hasWife rdf:resource=""Mary""/>
     <hasAge rdf:datatype=""&xsd;integer"">51</hasAge>
     <owl:differentFrom rdf:resource=""Bill""/>
     <owl:sameAs rdf:resource=""&otherOnt;JohnBrown""/>
     <rdf:type rdf:resource=""Father""/>
     <rdf:type>
       <owl:Restriction>
         <owl:maxQualifiedCardinality rdf:datatype=""&xsd;nonNegativeInteger"">
           4
         </owl:maxQualifiedCardinality>
         <owl:onProperty rdf:resource=""hasChild""/>
         <owl:onClass rdf:resource=""Parent""/>
       </owl:Restriction>
     </rdf:type>
     <rdf:type>
       <owl:Restriction>
         <owl:minQualifiedCardinality rdf:datatype=""&xsd;nonNegativeInteger"">
           2
         </owl:minQualifiedCardinality>
         <owl:onProperty rdf:resource=""hasChild""/>
         <owl:onClass rdf:resource=""Parent""/>
       </owl:Restriction>
     </rdf:type>
     <rdf:type>
       <owl:Restriction>
         <owl:qualifiedCardinality rdf:datatype=""&xsd;nonNegativeInteger"">
           3
         </owl:qualifiedCardinality>
         <owl:onProperty rdf:resource=""hasChild""/>
         <owl:onClass rdf:resource=""Parent""/>
       </owl:Restriction>
     </rdf:type>
     <rdf:type>
       <owl:Restriction>
         <owl:cardinality rdf:datatype=""&xsd;nonNegativeInteger"">
           5
         </owl:cardinality>
         <owl:onProperty rdf:resource=""hasChild""/>
       </owl:Restriction>
     </rdf:type>
   </owl:NamedIndividual>
 
   <SocialRole rdf:about=""Father""/>
 
   <owl:NegativePropertyAssertion>
     <owl:sourceIndividual rdf:resource=""Bill""/>
     <owl:assertionProperty rdf:resource=""hasWife""/>
     <owl:targetIndividual rdf:resource=""Mary""/>
   </owl:NegativePropertyAssertion>
 
   <owl:NegativePropertyAssertion>
     <owl:sourceIndividual rdf:resource=""Jack""/>
     <owl:assertionProperty rdf:resource=""hasAge""/>
     <owl:targetValue rdf:datatype=""&xsd;integer"">53</owl:targetValue>
   </owl:NegativePropertyAssertion>
 
   <owl:NegativePropertyAssertion>
     <owl:sourceIndividual rdf:resource=""Bill""/>
     <owl:assertionProperty rdf:resource=""hasDaughter""/>
     <owl:targetIndividual rdf:resource=""Susan""/>
   </owl:NegativePropertyAssertion>
 </rdf:RDF>
";

            using (MemoryStream stream = new MemoryStream())
            { 
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(ontString);
                    writer.Flush();
                    stream.Position = 0;

                    using (RDFGraph graph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.RdfXml, stream))
                    {
                        using (OWLOntology ontology = OWLOntology.FromRDFGraph(graph))
                        {
                            //using (MemoryStream stream2 = new MemoryStream())
                            //{
                            //ontology.ToStream(OWLEnums.OWLFormats.OwlXml, stream2);
                            //string owlxml = Encoding.UTF8.GetString(stream2.ToArray());
                            //}

                            Assert.IsNotNull(ontology);
                            Assert.IsTrue(ontology.URI.Equals(new Uri("http://example.com/owl/families")));
                            Assert.IsTrue(ontology.OBoxGraph.TriplesCount == 2);
                            Assert.IsNotNull(ontology.Model);
                            Assert.IsNotNull(ontology.Model.ClassModel);
                            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 46);
                            Assert.IsTrue(ontology.Model.ClassModel.AllDisjointClassesCount == 2);
                            Assert.IsTrue(ontology.Model.ClassModel.CompositesCount == 13);
                            Assert.IsTrue(ontology.Model.ClassModel.DeprecatedClassesCount == 0);
                            Assert.IsTrue(ontology.Model.ClassModel.EnumeratesCount == 2);
                            Assert.IsTrue(ontology.Model.ClassModel.RestrictionsCount == 13);
                            Assert.IsTrue(ontology.Model.ClassModel.SimpleClassesCount == 16);
                            Assert.IsTrue(ontology.Model.ClassModel.OBoxGraph.TriplesCount == 1);
                            Assert.IsNotNull(ontology.Model.PropertyModel);
                            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 14);
                            Assert.IsTrue(ontology.Model.PropertyModel.AllDisjointPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.AnnotationPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.AsymmetricPropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.DatatypePropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.DeprecatedPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.FunctionalPropertiesCount == 2);
                            Assert.IsTrue(ontology.Model.PropertyModel.InverseFunctionalPropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.IrreflexivePropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.ObjectPropertiesCount == 13);
                            Assert.IsTrue(ontology.Model.PropertyModel.ReflexivePropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.SymmetricPropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.TransitivePropertiesCount == 1);
                            Assert.IsTrue(ontology.Model.PropertyModel.OBoxGraph.TriplesCount == 0);
                            Assert.IsNotNull(ontology.Data);
                            Assert.IsTrue(ontology.Data.IndividualsCount == 4);
                            Assert.IsTrue(ontology.Data.OBoxGraph.TriplesCount == 0);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldLoadOntologyWithAnonymousRelation()
        {
            string ontString =
@"<!DOCTYPE rdf:RDF [
    <!ENTITY owl ""http://www.w3.org/2002/07/owl#"" >
    <!ENTITY xsd ""http://www.w3.org/2001/XMLSchema#"" >
    <!ENTITY rdfs ""http://www.w3.org/2000/01/rdf-schema#"" >
    <!ENTITY otherOnt ""http://example.org/otherOntologies/families/"" >
]>
 
 <rdf:RDF xml:base=""http://example.com/owl/families/""
          xmlns=""http://example.com/owl/families/""
          xmlns:otherOnt=""http://example.org/otherOntologies/families/""
          xmlns:owl=""http://www.w3.org/2002/07/owl#""
          xmlns:rdfs=""http://www.w3.org/2000/01/rdf-schema#""
          xmlns:rdf=""http://www.w3.org/1999/02/22-rdf-syntax-ns#""
          xmlns:xsd=""http://www.w3.org/2001/XMLSchema#"">
 
   <owl:Ontology rdf:about=""http://example.com/owl/families"">
     <owl:imports rdf:resource=""http://example.org/otherOntologies/families.owl"" />
   </owl:Ontology>

   <owl:ObjectProperty rdf:about=""hasChild"" />

   <owl:ObjectProperty rdf:about=""hasParent"">
     <owl:inverseOf rdf:resource=""hasChild""/>
   </owl:ObjectProperty>
 
   <owl:Class rdf:about=""Orphan"">
     <owl:equivalentClass>
       <owl:Restriction>
         <owl:onProperty>

           <!-- ENTIRE NODE SHOULD GIVE hasParent -->
           <owl:ObjectProperty>
             <owl:inverseOf rdf:resource=""hasChild""/>
           </owl:ObjectProperty>

         </owl:onProperty>
         <owl:allValuesFrom rdf:resource=""Dead""/>
       </owl:Restriction>
     </owl:equivalentClass>
   </owl:Class>
 </rdf:RDF>";

            using (MemoryStream stream = new MemoryStream())
            { 
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(ontString);
                    writer.Flush();
                    stream.Position = 0;

                    using (RDFGraph graph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.RdfXml, stream))
                    {
                        using (OWLOntology ontology = OWLOntology.FromRDFGraph(graph))
                        {
                            Assert.IsNotNull(ontology);
                            Assert.IsTrue(ontology.URI.Equals(new Uri("http://example.com/owl/families")));
                            Assert.IsTrue(ontology.OBoxGraph.TriplesCount == 2);
                            Assert.IsNotNull(ontology.Model);
                            Assert.IsNotNull(ontology.Model.ClassModel);
                            Assert.IsTrue(ontology.Model.ClassModel.ClassesCount == 2);
                            Assert.IsTrue(ontology.Model.ClassModel.AllDisjointClassesCount == 0);
                            Assert.IsTrue(ontology.Model.ClassModel.CompositesCount == 0);
                            Assert.IsTrue(ontology.Model.ClassModel.DeprecatedClassesCount == 0);
                            Assert.IsTrue(ontology.Model.ClassModel.EnumeratesCount == 0);
                            Assert.IsTrue(ontology.Model.ClassModel.RestrictionsCount == 1);
                            Assert.IsTrue(ontology.Model.ClassModel.SimpleClassesCount == 1);
                            Assert.IsTrue(ontology.Model.ClassModel.OBoxGraph.TriplesCount == 0);
                            Assert.IsNotNull(ontology.Model.PropertyModel);
                            Assert.IsTrue(ontology.Model.PropertyModel.PropertiesCount == 2);
                            Assert.IsTrue(ontology.Model.PropertyModel.AllDisjointPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.AnnotationPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.AsymmetricPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.DatatypePropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.DeprecatedPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.FunctionalPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.InverseFunctionalPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.IrreflexivePropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.ObjectPropertiesCount == 2);
                            Assert.IsTrue(ontology.Model.PropertyModel.ReflexivePropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.SymmetricPropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.TransitivePropertiesCount == 0);
                            Assert.IsTrue(ontology.Model.PropertyModel.OBoxGraph.TriplesCount == 0);
                            Assert.IsNotNull(ontology.Data);
                            Assert.IsTrue(ontology.Data.IndividualsCount == 0);
                            Assert.IsTrue(ontology.Data.OBoxGraph.TriplesCount == 0);

                            RDFResource equivalentToOrphan = ontology.Model.ClassModel.GetEquivalentClassesOf(new RDFResource("http://example.com/owl/families/Orphan")).Single();
                            Assert.IsNotNull(equivalentToOrphan);
                            Assert.IsTrue(ontology.Model.ClassModel.CheckHasAllValuesFromRestrictionClass(equivalentToOrphan));
                            Assert.IsTrue(ontology.Model.ClassModel.TBoxGraph[equivalentToOrphan, RDFVocabulary.OWL.ON_PROPERTY, new RDFResource("http://example.com/owl/families/hasParent"), null].TriplesCount > 0);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldLoadOntologyWithAnonymousProperty()
        {
            string ontString =
@"
@prefix : <http://example.com/owl/families/> .
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix owl: <http://www.w3.org/2002/07/owl#> .

:clsA rdf:type owl:Class .
:prpA rdf:type owl:ObjectProperty .
:prpB rdf:type owl:ObjectProperty .
:prpB owl:inverseOf :prpA .
:svfRest rdf:type owl:Restriction ;
     owl:onProperty [ owl:inverseOf  :prpB ] ; #this is an anonymous inline property expression (https://www.w3.org/2007/OWL/wiki/FullSemanticsInversePropertyExpressions)
     owl:someValuesFrom  :clsA .
:idv1 rdf:type :clsA .
:idv2 rdf:type :clsA .
:idv1 :prpA :idv2 .
:idv2 :prpB :idv1 .
";

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(ontString);
                    writer.Flush();
                    stream.Position = 0;

                    using (RDFGraph graph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, stream))
                    {
                        using (OWLOntology ontology = OWLOntology.FromRDFGraph(graph))
                        {
                            List<RDFResource> svRestMembers = ontology.Data.GetIndividualsOf(ontology.Model, new RDFResource("http://example.com/owl/families/svfRest"));

                            Assert.IsNotNull(svRestMembers);
                            Assert.IsTrue(svRestMembers.Single().Equals(new RDFResource("http://example.com/owl/families/idv1")));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldLoadOntologyWithAnonymousRestriction()
        {
            string ontString =
@"@prefix : <http://My.test/ItemBox#> .
@prefix ibx: <http://My.test/ItemBox#> .
@prefix owl: <http://www.w3.org/2002/07/owl#> .
@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
@prefix xml: <http://www.w3.org/XML/1998/namespace> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
<http://My.test/ItemBox> rdf:type owl:Ontology .

#################################################################
#    Object Properties
#################################################################

###  http://My.test/ItemBox#contains
ibx:contains rdf:type owl:ObjectProperty ;
             rdfs:domain ibx:Box ;
             rdfs:range ibx:Item .


#################################################################
#    Classes
#################################################################

###  http://My.test/ItemBox#Box
ibx:Box rdf:type owl:Class .


###  http://My.test/ItemBox#Item
ibx:Item rdf:type owl:Class .


#################################################################
#    Individuals
#################################################################

###  http://My.test/ItemBox#TheBox
ibx:TheBox rdf:type owl:NamedIndividual ,
                    ibx:Box ,
                    #Bug13 was that this rdf:type relation was not loaded into ontology
                    [ rdf:type owl:Restriction ;
                      owl:onProperty ibx:contains ;
                      owl:maxQualifiedCardinality ""1""^^xsd:nonNegativeInteger ;
                      owl:onClass ibx:Item
                    ] ;
           ibx:contains ibx:TheItem .


###  http://My.test/ItemBox#TheItem
ibx:TheItem rdf:type owl:NamedIndividual ,
                     ibx:Item .


###  Generated by the OWL API (version 4.5.9.2019-02-01T07:24:44Z) https://github.com/owlcs/owlapi
";
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(ontString);
                    writer.Flush();
                    stream.Position = 0;

                    using (RDFGraph graph = RDFGraph.FromStream(RDFModelEnums.RDFFormats.Turtle, stream))
                    {
                        using (OWLOntology ontology = OWLOntology.FromRDFGraph(graph))
                        {
                            RDFGraph graphOut = ontology.ToRDFGraph();

                            Assert.IsNotNull(graphOut);
                            Assert.IsTrue(graphOut.Any(t1 => t1.Subject.Equals(new RDFResource("http://My.test/ItemBox#TheBox"))
                                                              && t1.Predicate.Equals(RDFVocabulary.RDF.TYPE)
                                                               && t1.Object is RDFResource blankObject 
                                                                && graphOut[blankObject, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.RESTRICTION, null].Any()));
                        }
                    }
                }
            }
        }
    }
}