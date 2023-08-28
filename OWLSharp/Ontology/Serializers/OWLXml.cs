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
using System.Globalization;
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
        internal static readonly RDFResource OWLDeprecatedAnnotation = new RDFResource("http://www.w3.org/2002/07/owl#deprecated");

        #region Methods

        #region Write
        /// <summary>
        /// Serializes the given ontology to the given filepath using XML data format.
        /// </summary>
        internal static void Serialize(OWLOntology ontology, string filepath, bool includeInferences=true)
            => Serialize(ontology, new FileStream(filepath, FileMode.Create), includeInferences);

        /// <summary>
        /// Serializes the given ontology to the given stream using XML data format.
        /// </summary>
        internal static void Serialize(OWLOntology ontology, Stream outputStream, bool includeInferences=true)
        {
            try
            {
                List<RDFNamespace> ontGraphNamespaces = GetGraphNamespaces(ontology);

                #region serialize
                using (XmlWriter owlxmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings() {
                    Encoding = RDFModelUtilities.UTF8_NoBOM, Indent = true, CloseOutput = true }))
                {
                    XmlDocument owlDoc = new XmlDocument();
                    owlDoc.AppendChild(owlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));

                    XmlNode ontNode = WriteOntologyNode(owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    //Declarations (ClassModel, PropertyModel, Data)
                    WriteDeclarations(ontNode, owlDoc, "Declaration", "Class", ontology.Model.ClassModel.SimpleClassesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "Declaration", "ObjectProperty", ontology.Model.PropertyModel.ObjectPropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "Declaration", "DataProperty", ontology.Model.PropertyModel.DatatypePropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "Declaration", "AnnotationProperty", ontology.Model.PropertyModel.AnnotationPropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "FunctionalObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.FunctionalObjectPropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "InverseFunctionalObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.InverseFunctionalPropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "SymmetricObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.SymmetricPropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "AsymmetricObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.AsymmetricPropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "TransitiveObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.TransitivePropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "ReflexiveObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.ReflexivePropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "IrreflexiveObjectProperty", "ObjectProperty", ontology.Model.PropertyModel.IrreflexivePropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "FunctionalDataProperty", "DataProperty", ontology.Model.PropertyModel.FunctionalDatatypePropertiesEnumerator, ontGraphNamespaces, includeInferences);
                    WriteDeclarations(ontNode, owlDoc, "Declaration", "NamedIndividual", ontology.Data.IndividualsEnumerator, ontGraphNamespaces, includeInferences);
                    WriteRestrictions(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteEnumerates(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteComposites(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    //ClassModel
                    WriteSubClassRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteEquivalentClassRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteDisjointClassRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteAllDisjointClassesRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteDisjointUnionClassRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteHasKeyRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteAnnotations(ontNode, owlDoc, "ClassModel", ontology, ontGraphNamespaces, includeInferences);
                    //PropertyModel
                    WriteSubPropertyRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    WriteEquivalentPropertyRelations(ontNode, owlDoc, ontology, ontGraphNamespaces, includeInferences);
                    //TODO: annotations(+owl:deprecated=true) and relations (DisjointProperties, InverseProperties, AllDisjointProperties, ObjectPropertyChain, Domain, Range)

                    //Data
                    //TODO: annotations(+owl:deprecated=true) and relations (Type, SameAs, DifferentFrom, AllDifferent, Assertion, NegativeAssertion)

                    owlDoc.AppendChild(ontNode);
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

        #region Write
        internal static XmlNode WriteOntologyNode(XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            //Ontology
            XmlNode ontologyNode = owlDoc.CreateNode(XmlNodeType.Element, "Ontology", RDFVocabulary.OWL.BASE_URI);
            ontologyNode.AppendAttribute(owlDoc, "ontologyIRI", ontology.URI.ToString());
            RDFPatternMember versionIRI = ontology.OBoxGraph[ontology, RDFVocabulary.OWL.VERSION_IRI, null, null].FirstOrDefault()?.Object;
            if (versionIRI != null)
                ontologyNode.AppendAttribute(owlDoc, "versionIRI", versionIRI.ToString());

            //Prefixes
            foreach (RDFNamespace ontologyGraphNamespace in ontGraphNamespaces)
            {
                XmlNode prefixNode = owlDoc.CreateNode(XmlNodeType.Element, "Prefix", RDFVocabulary.OWL.BASE_URI);
                prefixNode.AppendAttribute(owlDoc, "name", ontologyGraphNamespace.NamespacePrefix);
                prefixNode.AppendAttribute(owlDoc, "IRI", ontologyGraphNamespace.NamespaceUri.ToString());
                ontologyNode.AppendChild(prefixNode);
                ontologyNode.AppendAttribute(owlDoc, string.Concat("xmlns:", ontologyGraphNamespace.NamespacePrefix), ontologyGraphNamespace.ToString());
            }
            XmlNode basePrefixNode = owlDoc.CreateNode(XmlNodeType.Element, "Prefix", RDFVocabulary.OWL.BASE_URI);
            basePrefixNode.AppendAttribute(owlDoc, "name", string.Empty);
            basePrefixNode.AppendAttribute(owlDoc, "IRI", ontology.URI.ToString());
            ontologyNode.AppendChild(basePrefixNode);
            ontologyNode.AppendAttribute(owlDoc, "xml:base", ontology.URI.ToString());

            //Imports
            foreach (RDFTriple impAnn in ontology.OBoxGraph.Where(t => t.Predicate.Equals(RDFVocabulary.OWL.IMPORTS)))
            {
                XmlNode ontologyImportNode = owlDoc.CreateNode(XmlNodeType.Element, "Import", RDFVocabulary.OWL.BASE_URI);
                XmlText ontologyImportNodeAbbreviatedText = owlDoc.CreateTextNode(impAnn.Object.ToString());
                ontologyImportNode.AppendChild(ontologyImportNodeAbbreviatedText);
                ontologyNode.AppendChild(ontologyImportNode);
            }

            //Annotations
            foreach (RDFTriple ontAnn in ontology.OBoxGraph.Where(t => !t.Predicate.Equals(RDFVocabulary.RDF.TYPE) &&
                                                                        !t.Predicate.Equals(RDFVocabulary.OWL.IMPORTS) &&
                                                                         !t.Predicate.Equals(RDFVocabulary.OWL.VERSION_IRI)))
            {
                XmlNode ontologyAnnotationNode = owlDoc.CreateNode(XmlNodeType.Element, "Annotation", RDFVocabulary.OWL.BASE_URI);

                (bool, string) abbreviatedOntAnnProp = RDFQueryUtilities.AbbreviateRDFPatternMember(ontAnn.Predicate, ontGraphNamespaces);
                XmlNode ontologyAnnotationPropertyNode = owlDoc.CreateNode(XmlNodeType.Element, "AnnotationProperty", RDFVocabulary.OWL.BASE_URI);
                ontologyAnnotationPropertyNode.AppendAttribute(owlDoc, abbreviatedOntAnnProp.Item1 ? "abbreviatedIRI" : "IRI", abbreviatedOntAnnProp.Item2);
                ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyNode);

                if (ontAnn.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                {
                    (bool, string) abbreviatedOntAnnObj = RDFQueryUtilities.AbbreviateRDFPatternMember(ontAnn.Object, ontGraphNamespaces);
                    if (abbreviatedOntAnnObj.Item1)
                    {
                        XmlNode ontologyAnnotationPropertyAbbreviatedIRINode = owlDoc.CreateNode(XmlNodeType.Element, "AbbreviatedIRI", RDFVocabulary.OWL.BASE_URI);
                        XmlText ontologyAnnotationPropertyAbbreviatedIRINodeText = owlDoc.CreateTextNode(abbreviatedOntAnnObj.Item2);
                        ontologyAnnotationPropertyAbbreviatedIRINode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINodeText);
                        ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyAbbreviatedIRINode);
                    }
                    else
                    {
                        XmlNode ontologyAnnotationPropertyIRINode = owlDoc.CreateNode(XmlNodeType.Element, "IRI", RDFVocabulary.OWL.BASE_URI);
                        XmlText ontologyAnnotationPropertyIRINodeText = owlDoc.CreateTextNode(abbreviatedOntAnnObj.Item2);
                        ontologyAnnotationPropertyIRINode.AppendChild(ontologyAnnotationPropertyIRINodeText);
                        ontologyAnnotationNode.AppendChild(ontologyAnnotationPropertyIRINode);
                    }
                }
                else
                    WriteLiteralElement(ontologyAnnotationNode, owlDoc, (RDFLiteral)ontAnn.Object);

                ontologyNode.AppendChild(ontologyAnnotationNode);
            }
            return ontologyNode;
        }

        internal static void WriteDeclarations(XmlNode xmlNode, XmlDocument owlDoc, string declarationOuterType, string declarationInnerType, IEnumerator<RDFResource> entitiesEnumerator, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            while (entitiesEnumerator.MoveNext())
            {
                XmlNode declarationOuterNode = owlDoc.CreateNode(XmlNodeType.Element, declarationOuterType, RDFVocabulary.OWL.BASE_URI);
                WriteResourceElement(declarationOuterNode, owlDoc, declarationInnerType, entitiesEnumerator.Current, ontGraphNamespaces);
                xmlNode.AppendChild(declarationOuterNode);
            }
        }
        
        internal static void WriteRestrictions(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> restrictions = ontology.Model.ClassModel.RestrictionsEnumerator;
            while (restrictions.MoveNext())
            {
                #region Guards
                RDFResource onProperty = ontology.Model.ClassModel.TBoxGraph[restrictions.Current, RDFVocabulary.OWL.ON_PROPERTY, null, null]
                                           .FirstOrDefault()?.Object as RDFResource;
                bool onObjectProperty = ontology.Model.PropertyModel.CheckHasObjectProperty(onProperty);
                bool onDatatypeProperty = ontology.Model.PropertyModel.CheckHasDatatypeProperty(onProperty);
                if (!onObjectProperty && !onDatatypeProperty)
                    throw new OWLException($"PropertyModel does not contain a declaration for object or datatype property '{onProperty}'");
                #endregion

                //Restrictions are serialized as objectProperties equivalent to...themselves OWL/XML-reified:
                //this is due to OWL/XML lacking a syntax for expressing named or standalone restrictions
                XmlNode equivalentClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentClasses", RDFVocabulary.OWL.BASE_URI);
                WriteResourceElement(equivalentClassesNode, owlDoc, "Class", restrictions.Current, ontGraphNamespaces);

                #region [Object|Data][Some|All]ValuesFrom
                bool isSVFromRestriction = ontology.Model.ClassModel.CheckHasSomeValuesFromRestrictionClass(restrictions.Current);
                bool isAVFromRestriction = ontology.Model.ClassModel.CheckHasAllValuesFromRestrictionClass(restrictions.Current);
                if (isSVFromRestriction || isAVFromRestriction)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    string someOrAll = isSVFromRestriction ? "Some" : "All";
                    RDFResource valuesFromClass = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        isSVFromRestriction ? RDFVocabulary.OWL.SOME_VALUES_FROM : RDFVocabulary.OWL.ALL_VALUES_FROM, null, null].FirstOrDefault()?.Object as RDFResource;

                    XmlNode valuesFromNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}{someOrAll}ValuesFrom", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(valuesFromNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                    WriteResourceElement(valuesFromNode, owlDoc, "Class", valuesFromClass, ontGraphNamespaces);
                    equivalentClassesNode.AppendChild(valuesFromNode);
                }
                #endregion

                #region [Object|Data]HasValue
                bool isHVRestriction = ontology.Model.ClassModel.CheckHasValueRestrictionClass(restrictions.Current);
                if (isHVRestriction)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    RDFPatternMember value = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        RDFVocabulary.OWL.HAS_VALUE, null, null].FirstOrDefault()?.Object;

                    XmlNode hasValueNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}HasValue", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(hasValueNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                    if (value is RDFResource valueResource)
                        WriteResourceElement(hasValueNode, owlDoc, "NamedIndividual", valueResource, ontGraphNamespaces);
                    else if (value is RDFLiteral valueLiteral)
                        WriteLiteralElement(hasValueNode, owlDoc, valueLiteral);
                    equivalentClassesNode.AppendChild(hasValueNode);
                }
                #endregion

                #region ObjectHasSelf
                bool isSelfRestriction = ontology.Model.ClassModel.CheckHasSelfRestrictionClass(restrictions.Current);
                if (isSelfRestriction && onObjectProperty)
                {
                    RDFPatternMember hasSelf = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        RDFVocabulary.OWL.HAS_SELF, null, null].FirstOrDefault()?.Object;
                    if (hasSelf is RDFTypedLiteral hasSelfLiteral && hasSelfLiteral.Equals(RDFTypedLiteral.True))
                    {
                        XmlNode hasSelfNode = owlDoc.CreateNode(XmlNodeType.Element, "ObjectHasSelf", RDFVocabulary.OWL.BASE_URI);
                        WriteResourceElement(hasSelfNode, owlDoc, "ObjectProperty", onProperty, ontGraphNamespaces);
                        equivalentClassesNode.AppendChild(hasSelfNode);
                    }
                }
                #endregion

                #region [Object|Data]MinCardinality
                bool isMinCardinality = ontology.Model.ClassModel.CheckHasMinCardinalityRestrictionClass(restrictions.Current);
                bool isMinQCardinality = ontology.Model.ClassModel.CheckHasMinQualifiedCardinalityRestrictionClass(restrictions.Current);
                if (isMinCardinality || isMinQCardinality)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    RDFPatternMember cardinalityValue = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        isMinCardinality ? RDFVocabulary.OWL.MIN_CARDINALITY : RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object;
                    if (cardinalityValue is RDFTypedLiteral cardinalityValueLiteral 
                         && cardinalityValueLiteral.HasDecimalDatatype()
                          && uint.TryParse(cardinalityValueLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint cardinalityValueInteger))
                    {
                        XmlNode cardinalityNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}MinCardinality", RDFVocabulary.OWL.BASE_URI);
                        cardinalityNode.AppendAttribute(owlDoc, "cardinality", $"{cardinalityValueInteger}");
                        WriteResourceElement(cardinalityNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                        if (isMinQCardinality)
                        {
                            RDFResource onClass = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                                RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object as RDFResource;
                            WriteResourceElement(cardinalityNode, owlDoc, $"Class", onClass, ontGraphNamespaces);
                        }
                        equivalentClassesNode.AppendChild(cardinalityNode);
                    }
                }
                #endregion

                #region [Object|Data]MaxCardinality
                bool isMaxCardinality = ontology.Model.ClassModel.CheckHasMaxCardinalityRestrictionClass(restrictions.Current);
                bool isMaxQCardinality = ontology.Model.ClassModel.CheckHasMaxQualifiedCardinalityRestrictionClass(restrictions.Current);
                if (isMaxCardinality || isMaxQCardinality)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    RDFPatternMember cardinalityValue = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        isMaxCardinality ? RDFVocabulary.OWL.MAX_CARDINALITY : RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object;
                    if (cardinalityValue is RDFTypedLiteral cardinalityValueLiteral
                         && cardinalityValueLiteral.HasDecimalDatatype()
                          && uint.TryParse(cardinalityValueLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint cardinalityValueInteger))
                    {
                        XmlNode cardinalityNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}MaxCardinality", RDFVocabulary.OWL.BASE_URI);
                        cardinalityNode.AppendAttribute(owlDoc, "cardinality", $"{cardinalityValueInteger}");
                        WriteResourceElement(cardinalityNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                        if (isMaxQCardinality)
                        {
                            RDFResource onClass = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                                RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object as RDFResource;
                            WriteResourceElement(cardinalityNode, owlDoc, $"Class", onClass, ontGraphNamespaces);
                        }
                        equivalentClassesNode.AppendChild(cardinalityNode);
                    }
                }
                #endregion

                #region [Object|Data]MinMaxCardinality
                bool isMinMaxCardinality = ontology.Model.ClassModel.CheckHasMinMaxCardinalityRestrictionClass(restrictions.Current);
                bool isMinMaxQCardinality = ontology.Model.ClassModel.CheckHasMinMaxQualifiedCardinalityRestrictionClass(restrictions.Current);
                if (isMinMaxCardinality || isMinMaxQCardinality)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    RDFPatternMember minCardinalityValue = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        isMinMaxCardinality ? RDFVocabulary.OWL.MIN_CARDINALITY : RDFVocabulary.OWL.MIN_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object;
                    RDFPatternMember maxCardinalityValue = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        isMinMaxCardinality ? RDFVocabulary.OWL.MAX_CARDINALITY : RDFVocabulary.OWL.MAX_QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object;
                    if (minCardinalityValue is RDFTypedLiteral minCardinalityValueLiteral
                         && minCardinalityValueLiteral.HasDecimalDatatype()
                          && uint.TryParse(minCardinalityValueLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint minCardinalityValueInteger))
                    {
                        XmlNode minCardinalityNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}MinCardinality", RDFVocabulary.OWL.BASE_URI);
                        minCardinalityNode.AppendAttribute(owlDoc, "cardinality", $"{minCardinalityValueInteger}");
                        WriteResourceElement(minCardinalityNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                        if (isMinMaxQCardinality)
                        {
                            RDFResource onClass = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                                RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object as RDFResource;
                            WriteResourceElement(minCardinalityNode, owlDoc, $"Class", onClass, ontGraphNamespaces);
                        }
                        equivalentClassesNode.AppendChild(minCardinalityNode);
                    }
                    if (maxCardinalityValue is RDFTypedLiteral maxCardinalityValueLiteral
                         && maxCardinalityValueLiteral.HasDecimalDatatype()
                          && uint.TryParse(maxCardinalityValueLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint maxCardinalityValueInteger))
                    {
                        XmlNode maxCardinalityNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}MaxCardinality", RDFVocabulary.OWL.BASE_URI);
                        maxCardinalityNode.AppendAttribute(owlDoc, "cardinality", $"{maxCardinalityValueInteger}");
                        WriteResourceElement(maxCardinalityNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                        if (isMinMaxQCardinality)
                        {
                            RDFResource onClass = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                                RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object as RDFResource;
                            WriteResourceElement(maxCardinalityNode, owlDoc, $"Class", onClass, ontGraphNamespaces);
                        }
                        equivalentClassesNode.AppendChild(maxCardinalityNode);
                    }
                }
                #endregion

                #region [Object|Data]ExactCardinality
                bool isExactCardinality = ontology.Model.ClassModel.CheckHasCardinalityRestrictionClass(restrictions.Current);
                bool isExactQCardinality = ontology.Model.ClassModel.CheckHasQualifiedCardinalityRestrictionClass(restrictions.Current);
                if (isExactCardinality || isExactQCardinality)
                {
                    string objectOrData = onObjectProperty ? "Object" : "Data";
                    RDFPatternMember cardinalityValue = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                        isExactCardinality ? RDFVocabulary.OWL.CARDINALITY : RDFVocabulary.OWL.QUALIFIED_CARDINALITY, null, null].FirstOrDefault()?.Object;
                    if (cardinalityValue is RDFTypedLiteral cardinalityValueLiteral
                         && cardinalityValueLiteral.HasDecimalDatatype()
                          && uint.TryParse(cardinalityValueLiteral.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint cardinalityValueInteger))
                    {
                        XmlNode cardinalityNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}ExactCardinality", RDFVocabulary.OWL.BASE_URI);
                        cardinalityNode.AppendAttribute(owlDoc, "cardinality", $"{cardinalityValueInteger}");
                        WriteResourceElement(cardinalityNode, owlDoc, $"{objectOrData}Property", onProperty, ontGraphNamespaces);
                        if (isExactQCardinality)
                        {
                            RDFResource onClass = ontology.Model.ClassModel.TBoxGraph[restrictions.Current,
                                RDFVocabulary.OWL.ON_CLASS, null, null].FirstOrDefault()?.Object as RDFResource;
                            WriteResourceElement(cardinalityNode, owlDoc, $"Class", onClass, ontGraphNamespaces);
                        }
                        equivalentClassesNode.AppendChild(cardinalityNode);
                    }
                }
                #endregion

                xmlNode.AppendChild(equivalentClassesNode);
            }
        }

        internal static void WriteEnumerates(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> enumerates = ontology.Model.ClassModel.EnumeratesEnumerator;
            while (enumerates.MoveNext())
            {
                //Enumerates are serialized as objectProperties equivalent to...themselves OWL/XML-reified:
                //this is due to OWL/XML lacking a syntax for expressing named or standalone restrictions
                XmlNode equivalentClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentClasses", RDFVocabulary.OWL.BASE_URI);
                WriteResourceElement(equivalentClassesNode, owlDoc, "Class", enumerates.Current, ontGraphNamespaces);

                #region [Object|Data]OneOf
                RDFResource enumRepresentative = ontology.Model.ClassModel.TBoxGraph[enumerates.Current, RDFVocabulary.OWL.ONE_OF, null, null]
                                                    .FirstOrDefault()?.Object as RDFResource;           
                RDFModelEnums.RDFTripleFlavors enumFlavor = RDFModelUtilities.DetectCollectionFlavorFromGraph(ontology.Model.ClassModel.TBoxGraph, enumRepresentative);
                RDFCollection enumMembers = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Model.ClassModel.TBoxGraph, enumRepresentative, enumFlavor);
                string objectOrData = enumFlavor == RDFModelEnums.RDFTripleFlavors.SPO ? "Object" : "Data"; 

                XmlNode oneOfNode = owlDoc.CreateNode(XmlNodeType.Element, $"{objectOrData}OneOf", RDFVocabulary.OWL.BASE_URI);
                foreach (RDFPatternMember enumMember in enumMembers)
                {
                    if (enumMember is RDFResource individualMember)
                        WriteResourceElement(oneOfNode, owlDoc, "NamedIndividual", individualMember, ontGraphNamespaces);
                    else if (enumMember is RDFLiteral literalMember)
                        WriteLiteralElement(oneOfNode, owlDoc, literalMember);
                }   
                equivalentClassesNode.AppendChild(oneOfNode);
                #endregion

                xmlNode.AppendChild(equivalentClassesNode);
            }
        }

        internal static void WriteComposites(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> composites = ontology.Model.ClassModel.CompositesEnumerator;
            while (composites.MoveNext())
            {
                //Composites are serialized as objectProperties equivalent to...themselves OWL/XML-reified:
                //this is due to OWL/XML lacking a syntax for expressing named or standalone restrictions
                XmlNode equivalentClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentClasses", RDFVocabulary.OWL.BASE_URI);
                WriteResourceElement(equivalentClassesNode, owlDoc, "Class", composites.Current, ontGraphNamespaces);

                #region ObjectUnionOf
                if (ontology.Model.ClassModel.CheckHasCompositeUnionClass(composites.Current))
                {
                    RDFResource objectUnionRepresentative = ontology.Model.ClassModel.TBoxGraph[composites.Current, RDFVocabulary.OWL.UNION_OF, null, null]
                                                              .FirstOrDefault()?.Object as RDFResource;           
                    RDFCollection objectUnionMembers = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Model.ClassModel.TBoxGraph, objectUnionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                    
                    XmlNode unionOfNode = owlDoc.CreateNode(XmlNodeType.Element, "ObjectUnionOf", RDFVocabulary.OWL.BASE_URI);
                    foreach (RDFResource objectUnionMember in objectUnionMembers)
                        WriteResourceElement(unionOfNode, owlDoc, "Class", objectUnionMember, ontGraphNamespaces);
                    equivalentClassesNode.AppendChild(unionOfNode);
                }
                #endregion

                #region ObjectIntersectionOf
                if (ontology.Model.ClassModel.CheckHasCompositeIntersectionClass(composites.Current))
                {
                    RDFResource objectIntersectionRepresentative = ontology.Model.ClassModel.TBoxGraph[composites.Current, RDFVocabulary.OWL.INTERSECTION_OF, null, null]
                                                                     .FirstOrDefault()?.Object as RDFResource;           
                    RDFCollection objectIntersectionMembers = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Model.ClassModel.TBoxGraph, objectIntersectionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                    
                    XmlNode intersectionOfNode = owlDoc.CreateNode(XmlNodeType.Element, "ObjectIntersectionOf", RDFVocabulary.OWL.BASE_URI);
                    foreach (RDFResource objectIntersectionMember in objectIntersectionMembers)
                        WriteResourceElement(intersectionOfNode, owlDoc, "Class", objectIntersectionMember, ontGraphNamespaces);
                    equivalentClassesNode.AppendChild(intersectionOfNode);
                }
                #endregion

                #region ObjectComplementOf
                if (ontology.Model.ClassModel.CheckHasCompositeComplementClass(composites.Current))
                {
                    RDFResource complementClass = ontology.Model.ClassModel.TBoxGraph[composites.Current, RDFVocabulary.OWL.COMPLEMENT_OF, null, null]
                                                    .FirstOrDefault()?.Object as RDFResource;           

                    XmlNode complementOfNode = owlDoc.CreateNode(XmlNodeType.Element, "ObjectComplementOf", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(complementOfNode, owlDoc, "Class", complementClass, ontGraphNamespaces);
                    equivalentClassesNode.AppendChild(complementOfNode);
                }
                #endregion

                xmlNode.AppendChild(equivalentClassesNode);
            }
        }

        internal static void WriteSubClassRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> classes = ontology.Model.ClassModel.ClassesEnumerator;
            while (classes.MoveNext())
            {
                foreach (RDFResource superClass in ontology.Model.ClassModel.TBoxGraph[classes.Current, RDFVocabulary.RDFS.SUB_CLASS_OF, null, null]
                                                    .Select(t => t.Object)
                                                    .OfType<RDFResource>())
                {
                    XmlNode subClassOfNode = owlDoc.CreateNode(XmlNodeType.Element, "SubClassOf", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(subClassOfNode, owlDoc, "Class", classes.Current, ontGraphNamespaces);
                    WriteResourceElement(subClassOfNode, owlDoc, "Class", superClass, ontGraphNamespaces);
                    xmlNode.AppendChild(subClassOfNode);
                }
            }
        }

        internal static void WriteEquivalentClassRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> classes = ontology.Model.ClassModel.ClassesEnumerator;
            while (classes.MoveNext())
            {
                List<RDFResource> equivalentClasses = ontology.Model.ClassModel.TBoxGraph[classes.Current, RDFVocabulary.OWL.EQUIVALENT_CLASS, null, null]
                                                        .Select(t => t.Object)
                                                        .OfType<RDFResource>()
                                                        .ToList();
                if (equivalentClasses.Count > 0)
                {
                    XmlNode equivalentClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentClasses", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(equivalentClassesNode, owlDoc, "Class", classes.Current, ontGraphNamespaces);
                    foreach (RDFResource equivalentClass in equivalentClasses)
                        WriteResourceElement(equivalentClassesNode, owlDoc, "Class", equivalentClass, ontGraphNamespaces);
                    xmlNode.AppendChild(equivalentClassesNode);
                }
            }
        }

        internal static void WriteDisjointClassRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> classes = ontology.Model.ClassModel.ClassesEnumerator;
            while (classes.MoveNext())
            {
                List<RDFResource> disjointClasses = ontology.Model.ClassModel.TBoxGraph[classes.Current, RDFVocabulary.OWL.DISJOINT_WITH, null, null]
                                                       .Select(t => t.Object)
                                                       .OfType<RDFResource>()
                                                       .ToList();
                if (disjointClasses.Count > 0)
                {
                    XmlNode disjointClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "DisjointClasses", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(disjointClassesNode, owlDoc, "Class", classes.Current, ontGraphNamespaces);
                    foreach (RDFResource disjointClass in disjointClasses)
                        WriteResourceElement(disjointClassesNode, owlDoc, "Class", disjointClass, ontGraphNamespaces);
                    xmlNode.AppendChild(disjointClassesNode);
                }
            }
        }

        internal static void WriteAllDisjointClassesRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> classes = ontology.Model.ClassModel.ClassesEnumerator;
            while (classes.MoveNext())
            {
                //OWL/XML lacks syntax for owl:AllDisjointClasses, so it fallbacks to owl:disjointWith
                if (ontology.Model.ClassModel.TBoxGraph.ContainsTriple(new RDFTriple(classes.Current, RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.ALL_DISJOINT_CLASSES)))
                {
                    RDFResource disjointMembersRepresentative = ontology.Model.ClassModel.TBoxGraph[classes.Current, RDFVocabulary.OWL.MEMBERS, null, null]
                                                                  .FirstOrDefault()?.Object as RDFResource;
                    if (disjointMembersRepresentative != null)
                    {
                        RDFCollection disjointMembers = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Model.ClassModel.TBoxGraph, disjointMembersRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                        if (disjointMembers.ItemsCount > 0)
                        {
                            XmlNode disjointMembersNode = owlDoc.CreateNode(XmlNodeType.Element, "DisjointClasses", RDFVocabulary.OWL.BASE_URI);
                            foreach (RDFResource disjointMember in disjointMembers)
                                WriteResourceElement(disjointMembersNode, owlDoc, "Class", disjointMember, ontGraphNamespaces);
                            xmlNode.AppendChild(disjointMembersNode);
                        }
                    }
                }
            }
        }

        internal static void WriteDisjointUnionClassRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> classes = ontology.Model.ClassModel.ClassesEnumerator;
            while (classes.MoveNext())
            {
                RDFResource disjointUnionRepresentative = ontology.Model.ClassModel.TBoxGraph[classes.Current, RDFVocabulary.OWL.DISJOINT_UNION_OF, null, null]
                                                            .FirstOrDefault()?.Object as RDFResource;
                if (disjointUnionRepresentative != null)
                {
                    RDFCollection disjointUnionClasses = RDFModelUtilities.DeserializeCollectionFromGraph(ontology.Model.ClassModel.TBoxGraph, disjointUnionRepresentative, RDFModelEnums.RDFTripleFlavors.SPO);
                    if (disjointUnionClasses.ItemsCount > 0)
                    {
                        XmlNode disjointClassesNode = owlDoc.CreateNode(XmlNodeType.Element, "DisjointUnion", RDFVocabulary.OWL.BASE_URI);
                        WriteResourceElement(disjointClassesNode, owlDoc, "Class", classes.Current, ontGraphNamespaces);
                        foreach (RDFResource disjointUnionClass in disjointUnionClasses)
                            WriteResourceElement(disjointClassesNode, owlDoc, "Class", disjointUnionClass, ontGraphNamespaces);
                        xmlNode.AppendChild(disjointClassesNode);
                    }
                }
            }
        }

        internal static void WriteHasKeyRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            IEnumerator<RDFResource> classes = ontology.Model.ClassModel.ClassesEnumerator;
            while (classes.MoveNext())
            {
                List<RDFResource> keyProperties = ontology.Model.ClassModel.GetKeyPropertiesOf(classes.Current);
                if (keyProperties.Count > 0)
                {
                    XmlNode hasKeyNode = owlDoc.CreateNode(XmlNodeType.Element, "HasKey", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(hasKeyNode, owlDoc, "Class", classes.Current, ontGraphNamespaces);
                    foreach (RDFResource keyProperty in keyProperties)
                    {
                        string objectOrData = ontology.Model.PropertyModel.CheckHasObjectProperty(keyProperty) ? "Object" : "Data";
                        WriteResourceElement(hasKeyNode, owlDoc, $"{objectOrData}Property", keyProperty, ontGraphNamespaces);
                    }                            
                    xmlNode.AppendChild(hasKeyNode);
                }
            }
        }

        internal static void WriteSubPropertyRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences = true)
        {
            IEnumerator<RDFResource> objectProperties = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectProperties.MoveNext())
                foreach (RDFResource superProperty in ontology.Model.PropertyModel.TBoxGraph[objectProperties.Current, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null]
                                                        .Select(t => t.Object)
                                                        .OfType<RDFResource>())
                {
                    XmlNode subPropertyOfNode = owlDoc.CreateNode(XmlNodeType.Element, "SubObjectPropertyOf", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(subPropertyOfNode, owlDoc, "ObjectProperty", objectProperties.Current, ontGraphNamespaces);
                    WriteResourceElement(subPropertyOfNode, owlDoc, "ObjectProperty", superProperty, ontGraphNamespaces);
                    xmlNode.AppendChild(subPropertyOfNode);
                }

            IEnumerator<RDFResource> datatypeProperties = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypeProperties.MoveNext())
                foreach (RDFResource superProperty in ontology.Model.PropertyModel.TBoxGraph[datatypeProperties.Current, RDFVocabulary.RDFS.SUB_PROPERTY_OF, null, null]
                                                        .Select(t => t.Object)
                                                        .OfType<RDFResource>())
                {
                    XmlNode subPropertyOfNode = owlDoc.CreateNode(XmlNodeType.Element, "SubDataPropertyOf", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(subPropertyOfNode, owlDoc, "DataProperty", datatypeProperties.Current, ontGraphNamespaces);
                    WriteResourceElement(subPropertyOfNode, owlDoc, "DataProperty", superProperty, ontGraphNamespaces);
                    xmlNode.AppendChild(subPropertyOfNode);
                }
        }

        internal static void WriteEquivalentPropertyRelations(XmlNode xmlNode, XmlDocument owlDoc, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences = true)
        {
            IEnumerator<RDFResource> objectProperties = ontology.Model.PropertyModel.ObjectPropertiesEnumerator;
            while (objectProperties.MoveNext())
            {
                List<RDFResource> equivalentProperties = ontology.Model.PropertyModel.TBoxGraph[objectProperties.Current, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null]
                                                            .Select(t => t.Object)
                                                            .OfType<RDFResource>()
                                                            .ToList();
                if (equivalentProperties.Count > 0)
                {
                    XmlNode equivalentPropertiesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentObjectProperties", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(equivalentPropertiesNode, owlDoc, "ObjectProperty", objectProperties.Current, ontGraphNamespaces);
                    foreach (RDFResource equivalentProperty in equivalentProperties)
                        WriteResourceElement(equivalentPropertiesNode, owlDoc, "ObjectProperty", equivalentProperty, ontGraphNamespaces);
                    xmlNode.AppendChild(equivalentPropertiesNode);
                }
            }

            IEnumerator<RDFResource> datatypeProperties = ontology.Model.PropertyModel.DatatypePropertiesEnumerator;
            while (datatypeProperties.MoveNext())
            {
                List<RDFResource> equivalentProperties = ontology.Model.PropertyModel.TBoxGraph[datatypeProperties.Current, RDFVocabulary.OWL.EQUIVALENT_PROPERTY, null, null]
                                                            .Select(t => t.Object)
                                                            .OfType<RDFResource>()
                                                            .ToList();
                if (equivalentProperties.Count > 0)
                {
                    XmlNode equivalentPropertiesNode = owlDoc.CreateNode(XmlNodeType.Element, "EquivalentDataProperties", RDFVocabulary.OWL.BASE_URI);
                    WriteResourceElement(equivalentPropertiesNode, owlDoc, "DataProperty", datatypeProperties.Current, ontGraphNamespaces);
                    foreach (RDFResource equivalentProperty in equivalentProperties)
                        WriteResourceElement(equivalentPropertiesNode, owlDoc, "DataProperty", equivalentProperty, ontGraphNamespaces);
                    xmlNode.AppendChild(equivalentPropertiesNode);
                }
            }
        }

        internal static void WriteAnnotations(XmlNode xmlNode, XmlDocument owlDoc, string annotationType, OWLOntology ontology, List<RDFNamespace> ontGraphNamespaces, bool includeInferences=true)
        {
            #region Utility
            void WriteAnnotation(RDFTriple annotation)
            {
                XmlNode annotationAssertionNode = owlDoc.CreateNode(XmlNodeType.Element, "AnnotationAssertion", RDFVocabulary.OWL.BASE_URI);

                //The property of the annotation
                (bool, string) abbreviatedAnnotationProperty = RDFQueryUtilities.AbbreviateRDFPatternMember(annotation.Predicate, ontGraphNamespaces);
                XmlNode annotationPropertyNode = owlDoc.CreateNode(XmlNodeType.Element, "AnnotationProperty", RDFVocabulary.OWL.BASE_URI);
                annotationPropertyNode.AppendAttribute(owlDoc, abbreviatedAnnotationProperty.Item1 ? "abbreviatedIRI" : "IRI", abbreviatedAnnotationProperty.Item2);
                annotationAssertionNode.AppendChild(annotationPropertyNode);

                //The subject of the annotation
                (bool, string) abbreviatedAnnotationSubject = RDFQueryUtilities.AbbreviateRDFPatternMember(annotation.Subject, ontGraphNamespaces);
                if (abbreviatedAnnotationSubject.Item1)
                {
                    XmlNode annotationSubjectAbbreviatedIRINode = owlDoc.CreateNode(XmlNodeType.Element, "AbbreviatedIRI", RDFVocabulary.OWL.BASE_URI);
                    XmlText annotationSubjectAbbreviatedIRINodeText = owlDoc.CreateTextNode(abbreviatedAnnotationSubject.Item2);
                    annotationSubjectAbbreviatedIRINode.AppendChild(annotationSubjectAbbreviatedIRINodeText);
                    annotationAssertionNode.AppendChild(annotationSubjectAbbreviatedIRINode);
                }
                else
                {
                    XmlNode annotationSubjectIRINode = owlDoc.CreateNode(XmlNodeType.Element, "IRI", RDFVocabulary.OWL.BASE_URI);
                    XmlText annotationSubjectIRINodeText = owlDoc.CreateTextNode(abbreviatedAnnotationSubject.Item2);
                    annotationSubjectIRINode.AppendChild(annotationSubjectIRINodeText);
                    annotationAssertionNode.AppendChild(annotationSubjectIRINode);
                }

                //The object of the annotation
                if (annotation.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO)
                {
                    (bool, string) abbreviatedAnnotationValue = RDFQueryUtilities.AbbreviateRDFPatternMember(annotation.Object, ontGraphNamespaces);
                    if (abbreviatedAnnotationValue.Item1)
                    {
                        XmlNode annotationValueAbbreviatedIRINode = owlDoc.CreateNode(XmlNodeType.Element, "AbbreviatedIRI", RDFVocabulary.OWL.BASE_URI);
                        XmlText annotationValueAbbreviatedIRINodeText = owlDoc.CreateTextNode(abbreviatedAnnotationValue.Item2);
                        annotationValueAbbreviatedIRINode.AppendChild(annotationValueAbbreviatedIRINodeText);
                        annotationAssertionNode.AppendChild(annotationValueAbbreviatedIRINode);
                    }
                    else
                    {
                        XmlNode annotationValueIRINode = owlDoc.CreateNode(XmlNodeType.Element, "IRI", RDFVocabulary.OWL.BASE_URI);
                        XmlText annotationValueIRINodeText = owlDoc.CreateTextNode(abbreviatedAnnotationValue.Item2);
                        annotationValueIRINode.AppendChild(annotationValueIRINodeText);
                        annotationAssertionNode.AppendChild(annotationValueIRINode);
                    }
                }
                else
                    WriteLiteralElement(annotationAssertionNode, owlDoc, (RDFLiteral)annotation.Object);

                xmlNode.AppendChild(annotationAssertionNode);
            }
            #endregion

            switch (annotationType)
            {
                case "ClassModel":
                    foreach (RDFResource ontologyClass in ontology.Model.ClassModel)
                    {
                        RDFGraph ontologyClassAnnotations = ontology.Model.ClassModel.OBoxGraph[ontologyClass, null, null, null];
                        foreach (RDFTriple objectAnnotation in ontologyClassAnnotations.Where(ann => ann.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPO))
                            WriteAnnotation(objectAnnotation);
                        foreach (RDFTriple dataAnnotation in ontologyClassAnnotations.Where(ann => ann.TripleFlavor == RDFModelEnums.RDFTripleFlavors.SPL))
                            WriteAnnotation(dataAnnotation);
                        //OWL/XML uses "owl:deprecated" annotation instead of "rdf:type owl:DeprecatedClass"
                        if (ontology.Model.ClassModel.CheckHasDeprecatedClass(ontologyClass))
                            WriteAnnotation(new RDFTriple(ontologyClass, OWLDeprecatedAnnotation, RDFTypedLiteral.True));
                    }
                    break;
                case "PropertyModel":
                    break;
                case "Data":
                    break;
            }
        }

        internal static void AppendAttribute(this XmlNode xmlNode, XmlDocument owlDoc, string attrName, string attrValue)
        {
            XmlAttribute attr = owlDoc.CreateAttribute(attrName);
            XmlText attrText = owlDoc.CreateTextNode(attrValue);
            attr.AppendChild(attrText);
            xmlNode.Attributes.Append(attr);
        }

        internal static void WriteResourceElement(XmlNode xmlNode, XmlDocument owlDoc, string resourceNodeName, RDFResource resourceURI, List<RDFNamespace> ontGraphNamespaces)
        {
            XmlNode resourceNode = owlDoc.CreateNode(XmlNodeType.Element, resourceNodeName, RDFVocabulary.OWL.BASE_URI);
            (bool, string) abbreviatedIRI = RDFQueryUtilities.AbbreviateRDFPatternMember(resourceURI, ontGraphNamespaces);
            if (abbreviatedIRI.Item1)
                resourceNode.AppendAttribute(owlDoc, "abbreviatedIRI", abbreviatedIRI.Item2);
            else
                resourceNode.AppendAttribute(owlDoc, "IRI", abbreviatedIRI.Item2.Replace("bnode:","bnode://"));
            xmlNode.AppendChild(resourceNode);
        }

        internal static void WriteLiteralElement(XmlNode xmlNode, XmlDocument owlDoc, RDFLiteral literal)
        {
            XmlNode literalNode = owlDoc.CreateNode(XmlNodeType.Element, "Literal", RDFVocabulary.OWL.BASE_URI);
            XmlText literalNodeText = owlDoc.CreateTextNode(literal.Value);
            literalNode.AppendChild(literalNodeText);
            if (literal is RDFPlainLiteral hasValuePLit && hasValuePLit.HasLanguage())
                literalNode.AppendAttribute(owlDoc, "xml:lang", hasValuePLit.Language);
            else if (literal is RDFTypedLiteral hasValueTLit)
                literalNode.AppendAttribute(owlDoc, "datatypeIRI", RDFModelUtilities.GetDatatypeFromEnum(hasValueTLit.Datatype));
            xmlNode.AppendChild(literalNode);
        }

        internal static List<RDFNamespace> GetGraphNamespaces(OWLOntology ontology)
        {
            RDFGraph ontologyGraph = ontology.ToRDFGraph();
            List<RDFNamespace> ontologyGraphNamespaces = RDFModelUtilities.GetGraphNamespaces(ontologyGraph);
            RDFNamespace xmlNamespace = RDFNamespaceRegister.GetByPrefix(RDFVocabulary.XML.PREFIX);
            if (!ontologyGraphNamespaces.Any(ns => ns.Equals(xmlNamespace)))
                ontologyGraphNamespaces.Add(xmlNamespace);
            return ontologyGraphNamespaces;
        }
        #endregion

        #endregion
    }
}