/*
   Copyright 2014-2026 Marco De Salvo

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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OWLSharp.Ontology
{
    /// <summary>
    /// OWLManchesterSerializer orchestrates the OWL2/Manchester rendering of an ontology:
    /// it collects the frame items contributed by each axiom (via ToManchesterFrameItem),
    /// groups them by owning entity frame and emits the resulting Manchester document.
    /// Axioms not representable in Manchester syntax (GCIs, SWRL rules, ...) are skipped with a warning.
    /// </summary>
    internal static class OWLManchesterSerializer
    {
        #region Properties
        /// <summary>
        /// The Manchester frame keywords, in the order frames are emitted into the document
        /// </summary>
        private static readonly List<(OWLManchesterFrameKind Kind, string Keyword)> FrameKeywords = new List<(OWLManchesterFrameKind, string)>
        {
            (OWLManchesterFrameKind.Datatype, "Datatype:"),
            (OWLManchesterFrameKind.AnnotationProperty, "AnnotationProperty:"),
            (OWLManchesterFrameKind.ObjectProperty, "ObjectProperty:"),
            (OWLManchesterFrameKind.DataProperty, "DataProperty:"),
            (OWLManchesterFrameKind.Class, "Class:"),
            (OWLManchesterFrameKind.Individual, "Individual:")
        };
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the given ontology to its OWL2/Manchester document representation
        /// </summary>
        internal static string SerializeOntology(OWLOntology ontology)
        {
            OWLManchesterContext ctx = new OWLManchesterContext(ontology.Prefixes);

            #region Collect frames
            //Frames are keyed by (kind, entityName) and map section keywords to their items
            Dictionary<(OWLManchesterFrameKind, string), Dictionary<string, List<string>>> frames = new Dictionary<(OWLManchesterFrameKind, string), Dictionary<string, List<string>>>();
            List<(OWLManchesterFrameKind Kind, string EntityName)> frameOrder = new List<(OWLManchesterFrameKind, string)>();
            List<(string SectionKeyword, string ItemText)> miscItems = new List<(string, string)>();
            List<OWLManchesterFrameItem> entityAnnotationItems = new List<OWLManchesterFrameItem>();

            void CollectAxiom(OWLAxiom axiom)
            {
                OWLManchesterFrameItem frameItem = axiom.ToManchesterFrameItem(ctx);
                if (frameItem == null)
                {
                    OWLEvents.RaiseWarning($"Axiom of type {axiom.GetType().Name} is not representable in OWL2/Manchester syntax and has been skipped ({axiom.GetXML()})");
                    return;
                }

                switch (frameItem.FrameKind)
                {
                    case OWLManchesterFrameKind.Misc:
                        miscItems.Add((frameItem.SectionKeyword, frameItem.ItemText));
                        break;

                    //Resolved against the collected frames after all axioms have been visited
                    case OWLManchesterFrameKind.EntityAnnotation:
                        entityAnnotationItems.Add(frameItem);
                        break;

                    default:
                        (OWLManchesterFrameKind, string) frameKey = (frameItem.FrameKind, frameItem.EntityName);
                        if (!frames.TryGetValue(frameKey, out Dictionary<string, List<string>> frameSections))
                        {
                            frameSections = new Dictionary<string, List<string>>();
                            frames.Add(frameKey, frameSections);
                            frameOrder.Add(frameKey);
                        }
                        if (frameItem.SectionKeyword != null)
                        {
                            if (!frameSections.TryGetValue(frameItem.SectionKeyword, out List<string> sectionItems))
                            {
                                sectionItems = new List<string>();
                                frameSections.Add(frameItem.SectionKeyword, sectionItems);
                            }
                            sectionItems.Add(frameItem.ItemText);
                        }
                        break;
                }
            }

            //Declarations are visited first, so that every declared entity owns a frame
            ontology.DeclarationAxioms.ForEach(CollectAxiom);
            ontology.DatatypeDefinitionAxioms.ForEach(CollectAxiom);
            ontology.ClassAxioms.ForEach(CollectAxiom);
            ontology.ObjectPropertyAxioms.ForEach(CollectAxiom);
            ontology.DataPropertyAxioms.ForEach(CollectAxiom);
            ontology.KeyAxioms.ForEach(CollectAxiom);
            ontology.AssertionAxioms.ForEach(CollectAxiom);
            ontology.AnnotationAxioms.ForEach(CollectAxiom);
            ontology.Rules.ForEach(swrlRule =>
                OWLEvents.RaiseWarning($"SWRL rule '{swrlRule}' is not representable in OWL2/Manchester syntax and has been skipped"));

            //Entity annotations are attached to the frame of their subject entity, whatever its kind
            foreach (OWLManchesterFrameItem entityAnnotationItem in entityAnnotationItems)
            {
                (OWLManchesterFrameKind Kind, string EntityName) subjectFrameKey = frameOrder.FirstOrDefault(frameKey => frameKey.EntityName == entityAnnotationItem.EntityName);
                if (subjectFrameKey.EntityName == null)
                {
                    OWLEvents.RaiseWarning($"AnnotationAssertion having subject '{entityAnnotationItem.EntityName}' is not representable in OWL2/Manchester syntax, since its subject is not a declared entity, and has been skipped");
                    continue;
                }

                Dictionary<string, List<string>> subjectFrameSections = frames[subjectFrameKey];
                if (!subjectFrameSections.TryGetValue("Annotations:", out List<string> annotationItems))
                {
                    annotationItems = new List<string>();
                    subjectFrameSections.Add("Annotations:", annotationItems);
                }
                annotationItems.Add(entityAnnotationItem.ItemText);
            }
            #endregion

            #region Emit document
            StringBuilder sb = new StringBuilder();

            //Prefixes
            foreach (OWLPrefix prefix in ontology.Prefixes)
                sb.AppendLine($"Prefix: {prefix.Name}: <{prefix.IRI}>");
            sb.AppendLine();

            //Ontology header (IRI, version, imports, annotations)
            sb.Append("Ontology:");
            if (!string.IsNullOrEmpty(ontology.IRI))
                sb.Append($" <{ontology.IRI}>");
            if (!string.IsNullOrEmpty(ontology.VersionIRI))
                sb.Append($" <{ontology.VersionIRI}>");
            sb.AppendLine();
            foreach (OWLImport import in ontology.Imports)
                sb.AppendLine($"Import: <{import.IRI}>");
            if (ontology.Annotations.Count > 0)
                sb.AppendLine($"Annotations: {string.Join(", ", ontology.Annotations.Select(ctx.RenderAnnotation))}");
            sb.AppendLine();

            //Entity frames (datatypes, then properties, then classes, then individuals)
            foreach ((OWLManchesterFrameKind frameKind, string frameKeyword) in FrameKeywords)
                foreach ((OWLManchesterFrameKind, string EntityName) frameKey in frameOrder.Where(fk => fk.Kind == frameKind))
                {
                    sb.AppendLine($"{frameKeyword} {frameKey.EntityName}");
                    foreach (KeyValuePair<string, List<string>> frameSection in frames[frameKey])
                        sb.AppendLine($"    {frameSection.Key} {string.Join(", ", frameSection.Value)}");
                    sb.AppendLine();
                }

            //Frame-less misc sections (n-ary axioms)
            foreach ((string sectionKeyword, string itemText) in miscItems)
            {
                sb.AppendLine($"{sectionKeyword} {itemText}");
                sb.AppendLine();
            }
            #endregion

            return sb.ToString();
        }
        #endregion
    }
}
