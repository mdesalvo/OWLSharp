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
using System.Collections.Generic;
using System.Linq;

namespace OWLSharp.Extensions.SKOS
{
    /// <summary>
    /// SKOSOntologyLoader is responsible for loading SKOS concept schemes from remote sources or alternative representations
    /// </summary>
    public static class SKOSOntologyLoader
    {
        #region Methods
        /// <summary>
        /// Prepares the given ontology for SKOS support, making it suitable for conceptual modeling and analysis
        /// </summary>
        public static void InitializeSKOS(this OWLOntology ontology)
        {
            #region Guards
            if (ontology == null)
                throw new OWLException("Cannot initialize SKOS ontology because given \"ontology\" parameter is null");
            #endregion

            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.SKOS.BASE_URI));
            ontology.Annotate(RDFVocabulary.OWL.IMPORTS, new RDFResource(RDFVocabulary.SKOS.SKOSXL.BASE_URI));
            BuildSKOSClassModel(ontology.Model.ClassModel);
            BuildSKOSPropertyModel(ontology.Model.PropertyModel);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Builds a reference SKOS class model
        /// </summary>
        internal static OWLOntologyClassModel BuildSKOSClassModel(OWLOntologyClassModel existingClassModel=null)
        {
            OWLOntologyClassModel skosClassModel = new OWLOntologyClassModel();

            #region SKOS T-BOX

            //SKOS
            skosClassModel.DeclareClass(RDFVocabulary.SKOS.COLLECTION);
            skosClassModel.DeclareClass(RDFVocabulary.SKOS.CONCEPT);
            skosClassModel.DeclareClass(RDFVocabulary.SKOS.CONCEPT_SCHEME);
            skosClassModel.DeclareClass(RDFVocabulary.SKOS.ORDERED_COLLECTION);
            skosClassModel.DeclareUnionClass(new RDFResource("bnode:ConceptCollection"), new List<RDFResource>() { RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.SKOS.COLLECTION });
            skosClassModel.DeclareCardinalityRestriction(new RDFResource("bnode:ExactlyOneLiteralForm"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, 1);
            skosClassModel.DeclareAllDisjointClasses(new RDFResource("bnode:AllDisjointSKOSClasses"), new List<RDFResource>() { RDFVocabulary.SKOS.COLLECTION, RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.SKOS.CONCEPT_SCHEME, RDFVocabulary.SKOS.SKOSXL.LABEL });
            skosClassModel.DeclareSubClasses(RDFVocabulary.SKOS.ORDERED_COLLECTION, RDFVocabulary.SKOS.COLLECTION);

            //SKOS-XL
            skosClassModel.DeclareClass(RDFVocabulary.SKOS.SKOSXL.LABEL);
            skosClassModel.DeclareSubClasses(RDFVocabulary.SKOS.SKOSXL.LABEL, new RDFResource("bnode:ExactlyOneLiteralForm"));

            #endregion

            #region IMPORT
            if (existingClassModel != null)
            {
                foreach (RDFTriple skosTriple in skosClassModel.TBoxGraph)
                    existingClassModel.TBoxGraph.AddTriple(skosTriple.SetImport());
                foreach (RDFResource skosClass in skosClassModel.Classes.Values)    
                {
                    if (!existingClassModel.Classes.ContainsKey(skosClass.PatternMemberID))
                        existingClassModel.Classes.Add(skosClass.PatternMemberID, skosClass);    
                }
            }
            #endregion

            return existingClassModel ?? skosClassModel;
        }

        /// <summary>
        /// Builds a reference SKOS property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildSKOSPropertyModel(OWLOntologyPropertyModel existingPropertyModel=null)
        {
            OWLOntologyPropertyModel skosPropertyModel = new OWLOntologyPropertyModel();

            #region SKOS T-BOX

            //SKOS
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.BROADER);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE, new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.CHANGE_NOTE);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.DEFINITION);
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.EDITORIAL_NOTE);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH, new OWLOntologyObjectPropertyBehavior() { Symmetric = true, Transitive = true });
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.EXAMPLE);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.CONCEPT_SCHEME, Range = RDFVocabulary.SKOS.CONCEPT });
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL);
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.HISTORY_NOTE);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.NARROWER);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.IN_SCHEME, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.CONCEPT_SCHEME });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.MAPPING_RELATION);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.MEMBER, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.COLLECTION, Range = new RDFResource("bnode:ConceptCollection") });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.MEMBER_LIST, new OWLOntologyObjectPropertyBehavior() { Functional = true, Domain = RDFVocabulary.SKOS.ORDERED_COLLECTION });
            skosPropertyModel.DeclareDatatypeProperty(RDFVocabulary.SKOS.NOTATION);
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.NOTE);
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.RELATED, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            skosPropertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.SCOPE_NOTE);
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SEMANTIC_RELATION, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.CONCEPT, Range = RDFVocabulary.SKOS.CONCEPT });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.CONCEPT, Range = RDFVocabulary.SKOS.CONCEPT_SCHEME });
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROAD_MATCH, RDFVocabulary.SKOS.BROADER);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROAD_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROADER, RDFVocabulary.SKOS.BROADER_TRANSITIVE);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROADER_TRANSITIVE, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.CLOSE_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.CLOSE_MATCH);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.MAPPING_RELATION, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROW_MATCH, RDFVocabulary.SKOS.NARROWER);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROW_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROWER, RDFVocabulary.SKOS.NARROWER_TRANSITIVE);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROWER_TRANSITIVE, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.TOP_CONCEPT_OF, RDFVocabulary.SKOS.IN_SCHEME);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.RELATED_MATCH, RDFVocabulary.SKOS.RELATED);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.RELATED_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            skosPropertyModel.DeclareSubProperties(RDFVocabulary.SKOS.RELATED, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            skosPropertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.BROAD_MATCH, RDFVocabulary.SKOS.NARROW_MATCH);
            skosPropertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.BROADER, RDFVocabulary.SKOS.NARROWER);
            skosPropertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.BROADER_TRANSITIVE, RDFVocabulary.SKOS.NARROWER_TRANSITIVE);
            skosPropertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.HAS_TOP_CONCEPT, RDFVocabulary.SKOS.TOP_CONCEPT_OF);
            skosPropertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.RELATED, RDFVocabulary.SKOS.BROADER_TRANSITIVE);
            skosPropertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.RELATED, RDFVocabulary.SKOS.NARROWER_TRANSITIVE);
            skosPropertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.BROAD_MATCH);
            skosPropertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.NARROW_MATCH);
            skosPropertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.RELATED_MATCH);
            skosPropertyModel.DeclareAllDisjointProperties(new RDFResource("bnode:AllDisjointSKOSLabelingProperties"), new List<RDFResource>() { RDFVocabulary.SKOS.PREF_LABEL, RDFVocabulary.SKOS.ALT_LABEL, RDFVocabulary.SKOS.HIDDEN_LABEL });

            //SKOS-XL
            skosPropertyModel.DeclareDatatypeProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.SKOS.SKOSXL.LABEL });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.SKOSXL.LABEL });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.SKOSXL.LABEL });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.SKOSXL.LABEL });
            skosPropertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, new OWLOntologyObjectPropertyBehavior() { Symmetric = true, Domain = RDFVocabulary.SKOS.SKOSXL.LABEL, Range = RDFVocabulary.SKOS.SKOSXL.LABEL });

            #endregion

            #region IMPORT
            if (existingPropertyModel != null)
            {
                foreach (RDFTriple skosTriple in skosPropertyModel.TBoxGraph)
                    existingPropertyModel.TBoxGraph.AddTriple(skosTriple.SetImport());
                foreach (RDFResource skosProperty in skosPropertyModel.Properties.Values)    
                {
                    if (!existingPropertyModel.Properties.ContainsKey(skosProperty.PatternMemberID))
                        existingPropertyModel.Properties.Add(skosProperty.PatternMemberID, skosProperty);    
                }
            }            
            #endregion

            return existingPropertyModel ?? skosPropertyModel;
        }
        #endregion
    }
}