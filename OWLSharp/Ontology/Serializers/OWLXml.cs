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
                List<RDFNamespace> ontologyGraphNamespaces = GetGraphNamespaces(ontology);

                #region serialize
                using (XmlWriter owlxmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings() {
                    Encoding = RDFModelUtilities.UTF8_NoBOM, Indent = true}))
                {
                    XmlDocument owlDoc = new XmlDocument();
                    owlDoc.AppendChild(owlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
                    
                    #region Ontology
                    XmlNode ontologyNode = owlDoc.CreateNode(XmlNodeType.Element, "Ontology", RDFVocabulary.OWL.BASE_URI);
                    ontologyNode.AppendAttribute(owlDoc, "ontologyIRI", ontology.URI.ToString());
                    RDFPatternMember versionIRI = ontology.OBoxGraph[ontology, RDFVocabulary.OWL.VERSION_IRI, null, null].FirstOrDefault()?.Object;
                    if (versionIRI != null)
                        ontologyNode.AppendAttribute(owlDoc, "versionIRI", versionIRI.ToString());

                    #region Prefixes
                    foreach (RDFNamespace ontologyGraphNamespace in ontologyGraphNamespaces)
                    {
                        ontologyNode.AppendAttribute(owlDoc, string.Concat("xmlns:", ontologyGraphNamespace.NamespacePrefix), ontologyGraphNamespace.ToString());
                        //Also write the corresponding element "<Prefix name='...' IRI='...'>"
                        XmlNode prefixNode = owlDoc.CreateNode(XmlNodeType.Element, "Prefix", RDFVocabulary.OWL.BASE_URI);
                        prefixNode.AppendAttribute(owlDoc, "name", ontologyGraphNamespace.NamespacePrefix);
                        prefixNode.AppendAttribute(owlDoc, "IRI", ontologyGraphNamespace.NamespaceUri.ToString());
                        ontologyNode.AppendChild(prefixNode);
                    }
                    //Write the ontology's base uri to resolve eventual relative #IDs
                    ontologyNode.AppendAttribute(owlDoc, "xml:base", ontology.URI.ToString());
                    //Also write the corresponding element "<Prefix name='' IRI='...'>"
                    XmlNode basePrefixNode = owlDoc.CreateNode(XmlNodeType.Element, "Prefix", RDFVocabulary.OWL.BASE_URI);
                    basePrefixNode.AppendAttribute(owlDoc, "name", string.Empty);
                    basePrefixNode.AppendAttribute(owlDoc, "IRI", ontology.URI.ToString());
                    ontologyNode.AppendChild(basePrefixNode);
                    #endregion                    

                    #region Imports
                    foreach (RDFTriple impAnn in ontology.OBoxGraph.Where(t => t.Predicate.Equals(RDFVocabulary.OWL.IMPORTS)))
                    {
                        XmlNode ontologyImportNode = owlDoc.CreateNode(XmlNodeType.Element, "Import", RDFVocabulary.OWL.BASE_URI);
                        XmlText ontologyImportNodeAbbreviatedText = owlDoc.CreateTextNode(impAnn.Object.ToString());
                        ontologyImportNode.AppendChild(ontologyImportNodeAbbreviatedText);
                        ontologyNode.AppendChild(ontologyImportNode);
                    }
                    #endregion

                    #region Annotations
                    foreach (RDFTriple ontAnn in ontology.OBoxGraph.Where(t => !t.Predicate.Equals(RDFVocabulary.RDF.TYPE) && 
                                                                                !t.Predicate.Equals(RDFVocabulary.OWL.IMPORTS) &&
                                                                                 !t.Predicate.Equals(RDFVocabulary.OWL.VERSION_IRI)))
                    {
                        XmlNode ontologyAnnotationNode = owlDoc.CreateNode(XmlNodeType.Element, "Annotation", RDFVocabulary.OWL.BASE_URI);

                        #region AnnotationProperty
                        (bool, string) abbreviatedOntAnnProp = RDFQueryUtilities.AbbreviateRDFPatternMember(ontAnn.Predicate, ontologyGraphNamespaces);
                        XmlNode ontologyAnnotationPropertyNode = owlDoc.CreateNode(XmlNodeType.Element, "AnnotationProperty", RDFVocabulary.OWL.BASE_URI);
                        ontologyAnnotationPropertyNode.AppendAttribute(owlDoc, abbreviatedOntAnnProp.Item1 ? "abbreviatedIRI" : "IRI", abbreviatedOntAnnProp.Item2);
                        ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyNode);
                        #endregion

                        #region IRI/AbbreviatedIRI
                        if (ontAnn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                        {
                            (bool, string) abbreviatedOntAnnObj = RDFQueryUtilities.AbbreviateRDFPatternMember(ontAnn.Object, ontologyGraphNamespaces);
                            if (abbreviatedOntAnnObj.Item1)
                            {
                                XmlNode ontologyAnnotationPropertyAbbreviatedIRINode = owlDoc.CreateNode(XmlNodeType.Element, "AbbreviatedIRI", RDFVocabulary.OWL.BASE_URI);
                                XmlText ontologyAnnotationPropertyAbbreviatedIRINodeText = owlDoc.CreateTextNode(abbreviatedOntAnnObj.Item2);
                                ontologyAnnotationPropertyAbbreviatedIRINode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINodeText);
                                ontologyAnnotationPropertyNode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINode);
                                ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINode);
                            }
                            else
                            {
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
                            XmlNode ontologyAnnotationPropertyLiteralNode = owlDoc.CreateNode(XmlNodeType.Element, "Literal", RDFVocabulary.OWL.BASE_URI);
                            XmlText ontologyAnnotationPropertyLiteralNodeText = owlDoc.CreateTextNode(((RDFLiteral)ontAnn.Object).Value.ToString());
                            ontologyAnnotationPropertyLiteralNode.AppendChild(ontologyAnnotationPropertyLiteralNodeText);
                            if (ontAnn.Object is RDFPlainLiteral ontAnnPLit && ontAnnPLit.HasLanguage())
                                ontologyAnnotationPropertyLiteralNode.AppendAttribute(owlDoc, "xml:lang", ontAnnPLit.Language);
                            else if (ontAnn.Object is RDFTypedLiteral ontAnnTLit)
                                ontologyAnnotationPropertyLiteralNode.AppendAttribute(owlDoc, "datatypeIRI", RDFModelUtilities.GetDatatypeFromEnum(ontAnnTLit.Datatype));
                            ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyLiteralNode); 
                        }
                        #endregion

                        ontologyNode.AppendChild(ontologyAnnotationNode);
                    }
                    #endregion                   
                    #endregion

                    #region ClassModel
                    WriteDeclarations(ontologyNode, owlDoc, "Declaration", "Class", ontology.Model.ClassModel.SimpleClassesEnumerator, ontologyGraphNamespaces);
                    //TODO: composites, enumerates, annotations(+owl:deprecated=true) and relations
                    WriteRestrictions(ontologyNode, owlDoc, ontology, ontologyGraphNamespaces);
                    #endregion

                    #region PropertyModel
                    WriteDeclarations(ontologyNode, owlDoc, "Declaration", "ObjectProperty", ontology.Model.PropertyModel.ObjectPropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "Declaration", "DataProperty", ontology.Model.PropertyModel.DatatypePropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "Declaration", "AnnotationProperty", ontology.Model.PropertyModel.AnnotationPropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "FunctionalObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.FunctionalObjectPropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "InverseFunctionalObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.InverseFunctionalPropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "SymmetricObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.SymmetricPropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "AsymmetricObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.AsymmetricPropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "TransitiveObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.TransitivePropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "ReflexiveObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.ReflexivePropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "IrreflexiveObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.IrreflexivePropertiesEnumerator, ontologyGraphNamespaces);
                    WriteDeclarations(ontologyNode, owlDoc, "FunctionalDataProperty", "DataProperty", ontology.Model.PropertyModel.FunctionalDatatypePropertiesEnumerator, ontologyGraphNamespaces);
                    //TODO: domain, range, annotations(+owl:deprecated=true) and relations
                    
                    #endregion

                    #region Data
                    WriteDeclarations(ontologyNode, owlDoc, "Declaration", "NamedIndividual", ontology.Data.IndividualsEnumerator, ontologyGraphNamespaces);
                    //TODO: anonymous individuals, domain, range, annotations(+owl:deprecated=true) and relations
                    
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

        #region Utilities
        internal static List<RDFNamespace> GetGraphNamespaces(OWLOntology ontology)
        {
            RDFGraph ontologyGraph = ontology.ToRDFGraph();
            List<RDFNamespace> ontologyGraphNamespaces = RDFModelUtilities.GetGraphNamespaces(ontologyGraph);
            RDFNamespace xmlNamespace = RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX);
            if (!ontologyGraphNamespaces.Any(ns => ns.Equals(xmlNamespace)))
                ontologyGraphNamespaces.Add(xmlNamespace);
            return ontologyGraphNamespaces;
        }

        internal static void AppendAttribute(this XmlNode xmlNode, XmlDocument owlDoc, string attrName, string attrValue)
        {
            XmlAttribute attr = owlDoc.CreateAttribute(attrName);
            XmlText attrText = owlDoc.CreateTextNode(attrValue);
            attr.AppendChild(attrText);
            xmlNode.Attributes.Append(attr);
        }

        internal static void WriteDeclarations(XmlNode xmlNode, XmlDocument owlDoc, string declarationCategory, string declarationType, IEnumerator<RDFResource> entitiesEnumerator, List<RDFNamespace> ontologyGraphNamespaces)
        {
            while (entitiesEnumerator.MoveNext())
            {
                XmlNode declarationCategoryNode = owlDoc.CreateNode(XmlNodeType.Element, declarationCategory, RDFVocabulary.OWL.BASE_URI);
                XmlNode declarationTypeNode = owlDoc.CreateNode(XmlNodeType.Element, declarationType, RDFVocabulary.OWL.BASE_URI);
                (bool, string) abbreviatedIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(entitiesEnumerator.Current, ontologyGraphNamespaces);
                if (abbreviatedIRI.Item1)
                    declarationTypeNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedIRI.Item2);
                else
                    declarationTypeNode.AppendAttribute(owlDoc, "IRI", abbreviatedIRI.Item2);
                declarationCategoryNode.AppendChild(declarationTypeNode);
                xmlNode.AppendChild(declarationCategoryNode);
            }
        }
        
        internal static void WriteRestrictions(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontologyGraphNamespaces)
        {
            IEnumerator<RDFResource> restrictionsEnumerator = ontology.Model.ClassModel.RestrictionsEnumerator;
            while (restrictionsEnumerator.MoveNext())
            {
                RDFResource onProperty = ontology.Model.ClassModel.TBoxGraph[restrictionsEnumerator.Current, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                                           .FirstOrDefault()?.Object as RDFResource;
                bool onObjectProperty = ontology.Model.PropertyModel.CheckHasObjectProperty(onProperty);
                bool onDatatypeProperty = ontology.Model.PropertyModel.CheckHasDatatypeProperty(onProperty);
                if (!onObjectProperty && !onDatatypeProperty)
                    throw new OWLException($"cannot find a declaration for object or data property '{onProperty}'");

                XmlNode equivalentClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentClasses", RDFVocabulary.OWL.BASE_URI);
                XmlNode classNode = owlDoc.CreateNode(XmlNodeType.Element, "Class", RDFVocabulary.OWL.BASE_URI);
                (bool, string) abbreviatedIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(restrictionsEnumerator.Current, ontologyGraphNamespaces);
                if (abbreviatedIRI.Item1)
                    classNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedIRI.Item2);
                else
                    classNode.AppendAttribute(owlDoc, "IRI", abbreviatedIRI.Item2.Replace("bnode:","bnode://"));
                equivalentClassesNode.AppendChild(classNode);

                #region [Object|Data][Some|All]ValuesFrom
                bool isSomeValuesFromRestriction = ontology.Model.ClassModel.CheckHasSomeValuesFromRestrictionClass(restrictionsEnumerator.Current);
                bool isAllValuesFromRestriction = ontology.Model.ClassModel.CheckHasAllValuesFromRestrictionClass(restrictionsEnumerator.Current);
                if (isSomeValuesFromRestriction || isAllValuesFromRestriction)
                {
                    //Write the corresponding element "[Object|Data][Some|All]ValuesFrom"
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    string someOrAll = isSomeValuesFromRestriction ? "Some" : "All";
                    XmlNode valuesFromNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}{someOrAll}ValuesFrom", RDFVocabulary.OWL.BASE_URI);

                    //Write the corresponding element "[Object|Data]Property"
                    XmlNode propertyNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}Property", RDFVocabulary.OWL.BASE_URI);
                    (bool, string) abbreviatedPropertyIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(onProperty, ontologyGraphNamespaces);
                    if (abbreviatedPropertyIRI.Item1)
                    {
                        //Write the corresponding attribute "abbreviatedIRI='...'"
                        propertyNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedPropertyIRI.Item2);
                    }
                    else
                    {
                        //Write the corresponding attribute "IRI='...'"
                        propertyNode.AppendAttribute(owlDoc, "IRI", abbreviatedPropertyIRI.Item2);
                    }
                    valuesFromNode.AppendChild(propertyNode);

                    //Write the corresponding element "Class"
                    RDFResource onClass = ontology.Model.ClassModel.TBoxGraph[restrictionsEnumerator.Current, 
                        isSomeValuesFromRestriction ? RDFVocabulary.OWL.SOME_VALUES_FROM : RDFVocabulary.OWL.ALL_VALUES_FROM, null, null].FirstOrDefault()?.Object as RDFResource;
                    XmlNode onClassNode = owlDoc.CreateNode(XmlNodeType.Element, "Class", RDFVocabulary.OWL.BASE_URI);
                    (bool, string) abbreviatedOnClassIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(onClass, ontologyGraphNamespaces);
                    if (abbreviatedOnClassIRI.Item1)
                    {
                        //Write the corresponding attribute "abbreviatedIRI='...'"
                        onClassNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedOnClassIRI.Item2);
                    }
                    else
                    {
                        //Write the corresponding attribute "IRI='...'" (in case of blank name switch to "bnode://" fake protocol)
                        onClassNode.AppendAttribute(owlDoc, "IRI", abbreviatedOnClassIRI.Item2.Replace("bnode:","bnode://"));
                    }
                    valuesFromNode.AppendChild(onClassNode);

                    equivalentClassesNode.AppendChild(valuesFromNode);
                }
                #endregion

                #region [Object|Data]HasValue
                bool isHasValueRestriction = ontology.Model.ClassModel.CheckHasValueRestrictionClass(restrictionsEnumerator.Current);
                if (isHasValueRestriction)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    XmlNode hasValueNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}HasValue", RDFVocabulary.OWL.BASE_URI);
                    XmlNode propertyNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}Property", RDFVocabulary.OWL.BASE_URI);
                    (bool, string) abbreviatedPropertyIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(onProperty, ontologyGraphNamespaces);
                    if (abbreviatedPropertyIRI.Item1)
                        propertyNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedPropertyIRI.Item2);
                    else
                        propertyNode.AppendAttribute(owlDoc, "IRI", abbreviatedPropertyIRI.Item2);
                    hasValueNode.AppendChild(propertyNode);

                    RDFPatternMember hasValuePMember = ontology.Model.ClassModel.TBoxGraph[restrictionsEnumerator.Current,
                        RDFVocabulary.OWL.HAS_VALUE, null, null].FirstOrDefault()?.Object;
                    if (hasValuePMember is RDFResource hasValueResource)
                    {
                        XmlNode individualNode = owlDoc.CreateNode(XmlNodeType.Element, $"NamedIndividual", RDFVocabulary.OWL.BASE_URI);
                        (bool, string) abbreviatedIndividualIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(hasValueResource, ontologyGraphNamespaces);
                        if (abbreviatedIndividualIRI.Item1)
                            individualNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedIndividualIRI.Item2);
                        else
                            individualNode.AppendAttribute(owlDoc, "IRI", abbreviatedIndividualIRI.Item2);
                        hasValueNode.AppendChild(propertyNode);
                    }
                    else if (hasValuePMember is RDFLiteral hasValueLiteral)
                    {
                        XmlNode literalNode = owlDoc.CreateNode(XmlNodeType.Element, "Literal", RDFVocabulary.OWL.BASE_URI);
                        XmlText literalNodeText = owlDoc.CreateTextNode(hasValueLiteral.Value);
                        literalNode.AppendChild(literalNodeText);
                        if (hasValueLiteral is RDFPlainLiteral hasValuePLit && hasValuePLit.HasLanguage())
                            literalNode.AppendAttribute(owlDoc, "xml:lang", hasValuePLit.Language);
                        else if (hasValueLiteral is RDFTypedLiteral hasValueTLit)
                            literalNode.AppendAttribute(owlDoc, "datatypeIRI", RDFModelUtilities.GetDatatypeFromEnum(hasValueTLit.Datatype));
                        hasValueNode.AppendChild(literalNode);
                    }
                }
                #endregion

                xmlNode.AppendChild(equivalentClassesNode);
            }
        }
        #endregion

        #endregion
    }
}