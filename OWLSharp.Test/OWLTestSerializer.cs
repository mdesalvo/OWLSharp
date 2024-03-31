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

using RDFSharp.Model;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace OWLSharp
{
    public static class OWLTestSerializer<T> where T : class
    {
        public static string Serialize(T objectToSerialize, XmlSerializerNamespaces xmlSerializerNamespaces=null) 
        {
            #region Guards
            if (objectToSerialize == null)
                throw new OWLException($"Cannot serialize {typeof(T)} because given \"objectToSerialize\" parameter is null");
            #endregion

            //Hide hard-coded .NET prefixes (e.g: xsi)
            xmlSerializerNamespaces ??= new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (UTF8StringWriter stringWriter = new UTF8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, 
                    new XmlWriterSettings()
                    {
                        Encoding = stringWriter.Encoding,
                        Indent = true,
                        NewLineHandling = NewLineHandling.None,
                        OmitXmlDeclaration = true,
                        NamespaceHandling = NamespaceHandling.OmitDuplicates
                    }))
                {
                    xmlSerializer.Serialize(writer, objectToSerialize, xmlSerializerNamespaces);
                    return stringWriter.ToString();
                }
            }
        }

        public static T Deserialize(string objectToDeserialize)
        {
            #region Guards
            if (objectToDeserialize == null)
                throw new OWLException($"Cannot deserialize {typeof(T)} because given \"objectToDeserialize\" parameter is null");
            #endregion

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(objectToDeserialize))
            {
                using (XmlReader reader = XmlReader.Create(stringReader,
                    new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Parse,
                        IgnoreComments = true,
                        IgnoreWhitespace = true,
                        IgnoreProcessingInstructions = true,
                        XmlResolver = null
                    }))
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
        }
    }
}