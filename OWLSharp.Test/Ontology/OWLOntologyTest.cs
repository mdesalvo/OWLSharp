/*
   Copyright 2012-2024 Marco De Salvo

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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Extensions.GEO;
using OWLSharp.Extensions.SKOS;
using OWLSharp.Extensions.TIME;
using RDFSharp.Model;

namespace OWLSharp.Test
{
    [TestClass]
    public class OWLOntologyTest
    {
        #region Test
        [TestMethod]
        public void ShouldCreateOntology()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");

            Assert.IsNotNull(ontology);
            Assert.IsNotNull(ontology.Model);
            Assert.IsNotNull(ontology.Data);
            Assert.IsNotNull(ontology.OBoxGraph);
            Assert.IsTrue(ontology.OBoxGraph.Context.Equals(new Uri("ex:ont")));
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
        }

        [TestMethod]
        public void ShouldCreateOntologyFromModelAndData()
        {
            OWLOntology ontology = new OWLOntology("ex:ont", new OWLOntologyModel(), new OWLOntologyData());

            Assert.IsNotNull(ontology);
            Assert.IsNotNull(ontology.Model);
            Assert.IsNotNull(ontology.Data);
            Assert.IsNotNull(ontology.OBoxGraph);
            Assert.IsTrue(ontology.OBoxGraph.Context.Equals(new Uri("ex:ont")));
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
        }

        [TestMethod]
        public void ShouldDisposeOntologyWithUsing()
        {
            OWLOntology ontology;
            using (ontology = new OWLOntology("ex:ont"))
            {
                Assert.IsFalse(ontology.Disposed);
                Assert.IsNotNull(ontology.Model);
                Assert.IsNotNull(ontology.Data);
                Assert.IsNotNull(ontology.OBoxGraph);
            };
            Assert.IsTrue(ontology.Disposed);
            Assert.IsNull(ontology.Model);
            Assert.IsNull(ontology.Data);
            Assert.IsNull(ontology.OBoxGraph);
        }

        [TestMethod]
        public void ShouldAnnotateOntologyWithResource()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"));

            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.SEE_ALSO, new RDFResource("ex:seealso"), null].Any());
        }

        [TestMethod]
        public void ShouldAnnotateOntologyWithLiteral()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));

            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
        }

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOntologyBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").Annotate(null, new RDFResource("ex:org")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOntologyBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").Annotate(new RDFResource(), new RDFResource("ex:org")));

        [TestMethod]
        public void ShouldThrowExceptionOnResourceAnnotatingOntologyBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").Annotate(RDFVocabulary.RDFS.LABEL, null as RDFResource));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOntologyBecauseNullProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").Annotate(null, new RDFPlainLiteral("This is a test ontology")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOntologyBecauseBlankProperty()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").Annotate(new RDFResource(), new RDFPlainLiteral("This is a test ontology")));

        [TestMethod]
        public void ShouldThrowExceptionOnLiteralAnnotatingOntologyBecauseNullValue()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").Annotate(RDFVocabulary.RDFS.LABEL, null as RDFLiteral));

        [TestMethod]
        public void ShouldExportToGraph()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            RDFGraph graph = ontology.ToRDFGraph();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
        }

        [TestMethod]
        public void ShouldExportToGraphWithoutGEOInferences()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            ontology.InitializeGEO();
            RDFGraph graph = ontology.ToRDFGraph(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
            
            //Having exported as false, we dont expect any GEO knowledge than the import annotation
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.GEOSPARQL.BASE_URI), null].Any());
            Assert.IsFalse(graph[RDFVocabulary.GEOSPARQL.SF.POINT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
        }

        [TestMethod]
        public void ShouldExportToGraphWithoutTIMEInferences()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            ontology.InitializeTIME();
            RDFGraph graph = ontology.ToRDFGraph(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
            
            //Having exported as false, we dont expect any TIME knowledge than the import annotation
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.TIME.BASE_URI), null].Any());
            Assert.IsFalse(graph[RDFVocabulary.TIME.INSTANT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
        }

        [TestMethod]
        public void ShouldExportToGraphWithoutSKOSInferences()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            ontology.InitializeSKOS();
            RDFGraph graph = ontology.ToRDFGraph(false);

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
            
            //Having exported as false, we dont expect any SKOS knowledge than the import annotation
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.SKOS.BASE_URI), null].Any());
            Assert.IsFalse(graph[RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS, null].Any());
        }

        [TestMethod]
        public async Task ShouldExportToGraphAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            RDFGraph graph = await ontology.ToRDFGraphAsync();

            Assert.IsNotNull(graph);
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(graph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
        }

        [TestMethod]
        public void ShouldExportToFile()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            ontology.ToFile(OWLEnums.OWLFormats.OwlXml, Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldExportToFile.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldExportToFile.owx")));
            Assert.IsTrue(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldExportToFile.owx")).Length > 0);
        }

        [TestMethod]
        public async Task ShouldExportToFileAsync()
        {
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            await ontology.ToFileAsync(OWLEnums.OWLFormats.OwlXml, Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldExportToFileAsync.owx"));

            Assert.IsTrue(File.Exists(Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldExportToFileAsync.owx")));
            Assert.IsTrue(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "OWLOntologyTest_ShouldExportToFileAsync.owx")).Length > 0);
        }

        [TestMethod]
        public void ShouldRaiseExceptionOnExportingToNullOrEmptyFilepath()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").ToFile(OWLEnums.OWLFormats.OwlXml, null));

        [TestMethod]
        public void ShouldExportToStream()
        {
            MemoryStream stream = new MemoryStream();
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            ontology.ToStream(OWLEnums.OWLFormats.OwlXml, stream);

            Assert.IsTrue(stream.ToArray().Length > 100);
        }

        [TestMethod]
        public async Task ShouldExportToStreamAsync()
        {
            MemoryStream stream = new MemoryStream();
            OWLOntology ontology = new OWLOntology("ex:ont");
            ontology.Annotate(RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology"));
            await ontology.ToStreamAsync(OWLEnums.OWLFormats.OwlXml, stream);

            Assert.IsTrue(stream.ToArray().Length > 100);
        }

        [TestMethod]
        public void ShouldRaiseExceptionOnExportingToNullStream()
            => Assert.ThrowsException<OWLException>(() => new OWLOntology("ex:ont").ToStream(OWLEnums.OWLFormats.OwlXml, null));

        [TestMethod]
        public void ShouldCreateFromGraph()
        {
            RDFGraph graph = new RDFGraph().SetContext(new Uri("ex:ont"));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology")));
            OWLOntology ontology = OWLOntology.FromRDFGraph(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.Equals(new RDFResource("ex:ont")));
            Assert.IsNotNull(ontology.OBoxGraph);
            Assert.IsTrue(ontology.OBoxGraph.Context.Equals(new Uri("ex:ont")));
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
            Assert.IsNotNull(ontology.Model);
            Assert.IsNotNull(ontology.Data);
        }

        [TestMethod]
        public async Task ShouldCreateFromGraphAsync()
        {
            RDFGraph graph = new RDFGraph().SetContext(new Uri("ex:ont"));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("This is a test ontology")));
            OWLOntology ontology = await OWLOntology.FromRDFGraphAsync(graph);

            Assert.IsNotNull(ontology);
            Assert.IsTrue(ontology.Equals(new RDFResource("ex:ont")));
            Assert.IsNotNull(ontology.OBoxGraph);
            Assert.IsTrue(ontology.OBoxGraph.Context.Equals(new Uri("ex:ont")));
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ONTOLOGY, null].Any());
            Assert.IsTrue(ontology.OBoxGraph[new RDFResource("ex:ont"), RDFVocabulary.RDFS.COMMENT, null, new RDFPlainLiteral("This is a test ontology")].Any());
            Assert.IsNotNull(ontology.Model);
            Assert.IsNotNull(ontology.Data);
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "OWLOntologyTest_Should*"))
                File.Delete(file);
        }
        #endregion
    }
}