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

using RDFSharp.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using OWLSharp;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace RDFSharp.Test.Model
{
    [TestClass]
    public class RDFAsyncGraphTest
    {
        #region Tests
        [TestMethod]
        public void ShouldSerializeOntology()
        {
            OWLOntology ontology = new OWLOntology(new Uri("http://example.org/"), new Uri("http://example.org/v1"));
            
            string owxOntology = OWLOntologySerializer.Serialize(ontology);
            

        }
        #endregion

        public class OWLOntologySerializer
        {
            public static string Serialize(OWLOntology ontology)
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);
                xmlSerializerNamespaces.Add(RDFVocabulary.OWL.PREFIX, RDFVocabulary.OWL.BASE_URI);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(OWLOntology));
                using (UTF8StringWriter stringWriter = new UTF8StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { 
                        Encoding=Encoding.UTF8, 
                        Indent=true, 
                        NewLineHandling=NewLineHandling.None }))
                    {
                        xmlSerializer.Serialize(writer, ontology, xmlSerializerNamespaces);
                        return stringWriter.ToString();
                    }
                }
            }
        }

        public class UTF8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}