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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp
{
    /// <summary>
    /// OWLXml is responsible for managing serialization to and from XML data format.
    /// </summary>
    internal static class OWLXml
    {
        #region Methods

        #region Write
        /// <summary>
        /// Serializes the given ontology to the given filepath using XML data format.
        /// </summary>
        internal static void Serialize(OWLOntology ontology, string filepath)
            => Serialize(ontology, new FileStream(filepath, FileMode.Create));

        /// <summary>
        /// Serializes the given ontology to the given stream using XML data format.
        /// </summary>
        internal static void Serialize(OWLOntology ontology, Stream outputStream)
        {
            try
            {
                #region serialize
                using (XmlWriter owlxmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings() {
                    Encoding = RDFModelUtilities.UTF8_NoBOM, Indent = true}))
                {
                    XmlDocument owlDoc = new XmlDocument();
                    owlDoc.AppendChild(owlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
                    XmlNode ontologyNode = owlDoc.CreateNode(XmlNodeType.Element, "Ontology", RDFVocabulary.OWL.BASE_URI);

                    #region Ontology
                    XmlAttribute ontologyNodeIRIAttr = owlDoc.CreateAttribute("ontologyIRI");
                    XmlText ontologyNodeIRIAttrText = owlDoc.CreateTextNode(ontology.URI.ToString());
                    ontologyNodeIRIAttr.AppendChild(ontologyNodeIRIAttrText);
                    ontologyNode.Attributes.Append(ontologyNodeIRIAttr);
                    RDFPatternMember versionIRI = ontology.OBoxGraph[ontology, RDFVocabulary.OWL.VERSION_IRI, null, null].FirstOrDefault()?.Object;
                    if (versionIRI != null)
                    {
                        XmlAttribute ontologyNodeVIRIAttr = owlDoc.CreateAttribute("versionIRI");
                        XmlText ontologyNodeVIRIAttrText = owlDoc.CreateTextNode(versionIRI.ToString());
                        ontologyNodeVIRIAttr.AppendChild(ontologyNodeVIRIAttrText);
                        ontologyNode.Attributes.Append(ontologyNodeVIRIAttr);
                    }

                    #region Prefix
                    //Write the ontology prefixes
                    RDFGraph ontologyGraph = ontology.ToRDFGraph();
                    List<RDFNamespace> ontologyGraphNamespaces = RDFModelUtilities.GetGraphNamespaces(ontologyGraph);
                    RDFNamespace xmlNamespace = RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX);
                    if (!ontologyGraphNamespaces.Any(ns => ns.Equals(xmlNamespace)))
                        ontologyGraphNamespaces.Add(xmlNamespace);
                    ontologyGraphNamespaces.ForEach(p =>
                    {
                        if (!p.NamespacePrefix.Equals("base", StringComparison.OrdinalIgnoreCase))
                        {
                            XmlAttribute ontologyNodePrefixAttr = owlDoc.CreateAttribute(string.Concat("xmlns:", p.NamespacePrefix));
                            XmlText ontologyNodePrefixAttrText = owlDoc.CreateTextNode(p.ToString());
                            ontologyNodePrefixAttr.AppendChild(ontologyNodePrefixAttrText);
                            ontologyNode.Attributes.Append(ontologyNodePrefixAttr);
                            //Also write the corresponding element "<Prefix name='...' IRI='...'>"
                            XmlNode prefixNode = owlDoc.CreateNode(XmlNodeType.Element, "Prefix", RDFVocabulary.OWL.BASE_URI);
                            XmlAttribute prefixNodeNameAttr = owlDoc.CreateAttribute("name");
                            prefixNodeNameAttr.AppendChild(owlDoc.CreateTextNode(p.NamespacePrefix));
                            prefixNode.Attributes.Append(prefixNodeNameAttr);
                            XmlAttribute prefixNodeIRIAttr = owlDoc.CreateAttribute("IRI");
                            prefixNodeIRIAttr.AppendChild(owlDoc.CreateTextNode(p.NamespaceUri.ToString()));
                            prefixNode.Attributes.Append(prefixNodeIRIAttr);
                            ontologyNode.AppendChild(prefixNode);
                        }
                    });
                    //Write the ontology's base uri to resolve eventual relative #IDs
                    XmlAttribute ontologyNodeBaseAttr = owlDoc.CreateAttribute("xml:base");
                    XmlText ontologyNodeBaseAttrText = owlDoc.CreateTextNode(ontology.URI.ToString());
                    ontologyNodeBaseAttr.AppendChild(ontologyNodeBaseAttrText);
                    ontologyNode.Attributes.Append(ontologyNodeBaseAttr);
                    //Also write the corresponding element "<Prefix name='' IRI='...'>"
                    XmlNode basePrefixNode = owlDoc.CreateNode(XmlNodeType.Element, "Prefix", RDFVocabulary.OWL.BASE_URI);
                    XmlAttribute basePrefixNodeNameAttr = owlDoc.CreateAttribute("name");
                    basePrefixNodeNameAttr.AppendChild(owlDoc.CreateTextNode(string.Empty));
                    XmlAttribute basePrefixNodeIRIAttr = owlDoc.CreateAttribute("IRI");
                    basePrefixNodeIRIAttr.AppendChild(owlDoc.CreateTextNode(ontology.URI.ToString()));
                    basePrefixNode.Attributes.Append(basePrefixNodeIRIAttr);
                    ontologyNode.AppendChild(basePrefixNode);
                    #endregion                    

                    #region Import
                    //Write the ontology imports
                    foreach (RDFTriple impAnn in ontology.OBoxGraph.Where(t => t.Predicate.Equals(RDFVocabulary.OWL.IMPORTS)))
                    {
                        //Write the corresponding element "Import"
                        XmlNode ontologyImportNode = owlDoc.CreateNode(XmlNodeType.Element, "Import", RDFVocabulary.OWL.BASE_URI);
                        XmlText ontologyImportNodeAbbreviatedText = owlDoc.CreateTextNode(impAnn.Object.ToString());
                        ontologyImportNode.AppendChild(ontologyImportNodeAbbreviatedText);
                        ontologyNode.AppendChild(ontologyImportNode);
                    }
                    #endregion

                    #region Annotation
                    //Write the annotations (except "imports" and "versionIRI")
                    foreach (RDFTriple ontAnn in ontology.OBoxGraph.Where(t => !t.Predicate.Equals(RDFVocabulary.RDF.TYPE) && 
                                                                                !t.Predicate.Equals(RDFVocabulary.OWL.IMPORTS) &&
                                                                                 !t.Predicate.Equals(RDFVocabulary.OWL.VERSION_IRI)))
                    {
                        //Write the corresponding element "Annotation"
                        XmlNode ontologyAnnotationNode = owlDoc.CreateNode(XmlNodeType.Element, "Annotation", RDFVocabulary.OWL.BASE_URI);

                        #region AnnotationProperty
                        //Write the corresponding element "AnnotationProperty"
                        XmlNode ontologyAnnotationPropertyNode = owlDoc.CreateNode(XmlNodeType.Element, "AnnotationProperty", RDFVocabulary.OWL.BASE_URI);
                        (bool, string) abbreviatedOntAnnProp = RDFQueryUtilities.AbbreviateRDFPatternMember(ontAnn.Predicate, ontologyGraphNamespaces);
                        if (abbreviatedOntAnnProp.Item1)
                        {
                            //Write the corresponding attribute "abbreviatedIRI='...'"
                            XmlAttribute ontologyAnnotationPropertyNodeAbbreviatedIRIAttr = owlDoc.CreateAttribute("abbreviatedIRI");
                            XmlText ontologyAnnotationPropertyNodeAbbreviatedIRIAttrText = owlDoc.CreateTextNode(abbreviatedOntAnnProp.Item2);
                            ontologyAnnotationPropertyNodeAbbreviatedIRIAttr.AppendChild(ontologyAnnotationPropertyNodeAbbreviatedIRIAttrText);
                            ontologyAnnotationPropertyNode.Attributes.Append(ontologyAnnotationPropertyNodeAbbreviatedIRIAttr);
                        }
                        else
                        {
                            //Write the corresponding attribute "IRI='...'"
                            XmlAttribute ontologyAnnotationPropertyNodeIRIAttr = owlDoc.CreateAttribute("IRI");
                            XmlText ontologyAnnotationPropertyNodeIRIAttrText = owlDoc.CreateTextNode(abbreviatedOntAnnProp.Item2);
                            ontologyAnnotationPropertyNodeIRIAttr.AppendChild(ontologyAnnotationPropertyNodeIRIAttrText);
                            ontologyAnnotationPropertyNode.Attributes.Append(ontologyAnnotationPropertyNodeIRIAttr);
                        }
                        ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyNode);
                        #endregion

                        #region IRI/AbbreviatedIRI
                        if (ontAnn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                        {
                            (bool, string) abbreviatedOntAnnObj = RDFQueryUtilities.AbbreviateRDFPatternMember(ontAnn.Object, ontologyGraphNamespaces);
                            if (abbreviatedOntAnnObj.Item1)
                            {
                                //Write the corresponding element "AbbreviatedIRI"
                                XmlNode ontologyAnnotationPropertyAbbreviatedIRINode = owlDoc.CreateNode(XmlNodeType.Element, "AbbreviatedIRI", RDFVocabulary.OWL.BASE_URI);
                                XmlText ontologyAnnotationPropertyAbbreviatedIRINodeText = owlDoc.CreateTextNode(abbreviatedOntAnnObj.Item2);
                                ontologyAnnotationPropertyAbbreviatedIRINode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINodeText);
                                ontologyAnnotationPropertyNode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINode);
                                ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINode);
                            }
                            else
                            {
                                //Write the corresponding element "IRI"
                                XmlNode ontologyAnnotationPropertyIRINode = owlDoc.CreateNode(XmlNodeType.Element, "IRI", RDFVocabulary.OWL.BASE_URI);
                                XmlText ontologyAnnotationPropertyIRINodeText = owlDoc.CreateTextNode(abbreviatedOntAnnObj.Item2);
                                ontologyAnnotationPropertyIRINode.AppendChild(ontologyAnnotationPropertyIRINodeText);
                                ontologyAnnotationPropertyNode.AppendChild(ontologyAnnotationPropertyIRINode);
                                ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyIRINode);
                            }
                        }
                        #endregion

                        #region Literal
                        else
                        {
                            //Write the corresponding element "Literal"
                            XmlNode ontologyAnnotationPropertyLiteralNode = owlDoc.CreateNode(XmlNodeType.Element, "Literal", RDFVocabulary.OWL.BASE_URI);
                            XmlText ontologyAnnotationPropertyLiteralNodeText = owlDoc.CreateTextNode(((RDFLiteral)ontAnn.Object).Value.ToString());
                            ontologyAnnotationPropertyLiteralNode.AppendChild(ontologyAnnotationPropertyLiteralNodeText);
                            if (ontAnn.Object is RDFPlainLiteral ontAnnPLit && ontAnnPLit.HasLanguage())
                            {
                                //Write the corresponding attribute "xml:lang='...'"
                                XmlAttribute ontologyAnnotationPropertyLiteralLanguageAttr = owlDoc.CreateAttribute("xml:lang");
                                XmlText ontologyAnnotationPropertyLiteralLanguageAttrText = owlDoc.CreateTextNode(ontAnnPLit.Language);
                                ontologyAnnotationPropertyLiteralLanguageAttr.AppendChild(ontologyAnnotationPropertyLiteralLanguageAttrText);
                                ontologyAnnotationPropertyLiteralNode.Attributes.Append(ontologyAnnotationPropertyLiteralLanguageAttr);
                            }
                            else if (ontAnn.Object is RDFTypedLiteral ontAnnTLit)
                            {
                                //Write the corresponding attribute "datatypeIRI='...'"
                                XmlAttribute ontologyAnnotationPropertyLiteralDatatypeAttr = owlDoc.CreateAttribute("datatypeIRI");
                                XmlText ontologyAnnotationPropertyLiteralLanguageAttrText = owlDoc.CreateTextNode(RDFModelUtilities.GetDatatypeFromEnum(ontAnnTLit.Datatype));
                                ontologyAnnotationPropertyLiteralDatatypeAttr.AppendChild(ontologyAnnotationPropertyLiteralLanguageAttrText);
                                ontologyAnnotationPropertyLiteralNode.Attributes.Append(ontologyAnnotationPropertyLiteralDatatypeAttr);
                            }
                            ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyLiteralNode); 
                        }
                        #endregion

                        ontologyNode.AppendChild(ontologyAnnotationNode);
                    }
                    #endregion                   
                    #endregion

                    #region ClassModel
                    //This kind of syntax only supports simple classes into "Declaration" nodes
                    //(restrictions, composites and enumerates will be written separately in specific forms)
                    IEnumerator<RDFResource> simpleClassesEnumerator = ontology.Model.ClassModel.SimpleClassesEnumerator;
                    while (simpleClassesEnumerator.MoveNext())
                    {
                        //Write the corresponding element "Declaration"
                        XmlNode declarationNode = owlDoc.CreateNode(XmlNodeType.Element, "Declaration", RDFVocabulary.OWL.BASE_URI);
                        //Write the corresponding element "Class"
                        XmlNode classNode = owlDoc.CreateNode(XmlNodeType.Element, "Class", RDFVocabulary.OWL.BASE_URI);
                        (bool, string) abbreviatedIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(simpleClassesEnumerator.Current, ontologyGraphNamespaces);
                        if (abbreviatedIRI.Item1)
                        {
                            //Write the corresponding attribute "abbreviatedIRI='...'"
                            XmlAttribute classNodeAbbreviatedIRIAttr = owlDoc.CreateAttribute("abbreviatedIRI");
                            XmlText classNodeAbbreviatedIRIAttrText = owlDoc.CreateTextNode(abbreviatedIRI.Item2);
                            classNodeAbbreviatedIRIAttr.AppendChild(classNodeAbbreviatedIRIAttrText);
                            classNode.Attributes.Append(classNodeAbbreviatedIRIAttr);
                        }
                        else
                        {
                            //Write the corresponding attribute "IRI='...'"
                            XmlAttribute classNodeIRIAttr = owlDoc.CreateAttribute("IRI");
                            XmlText classNodeIRIAttrText = owlDoc.CreateTextNode(abbreviatedIRI.Item2);
                            classNodeIRIAttr.AppendChild(classNodeIRIAttrText);
                            classNode.Attributes.Append(classNodeIRIAttr);
                        }
                        declarationNode.AppendChild(classNode);
                        ontologyNode.AppendChild(declarationNode);
                    }
                    #endregion

                    #region PropertyModel
                    
                    #endregion

                    #region Data
                    
                    #endregion

                    owlDoc.AppendChild(ontologyNode);
                    owlDoc.Save(owlxmlWriter);
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new OWLException("Cannot serialize OWL/Xml because: " + ex.Message, ex);
            }
        }
        #endregion

        #endregion
    }
}