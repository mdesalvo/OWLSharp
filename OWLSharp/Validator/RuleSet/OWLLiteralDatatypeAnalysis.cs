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

using OWLSharp.Ontology;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Validator
{
    /// <summary>
    /// <para>W3C OWL2 RL/RDF: dt-not-type (typed literal value-space compatibility check, scoped to A-Box DataPropertyAssertion/NegativeDataPropertyAssertion literals)</para>
    /// </summary>
    internal static class OWLLiteralDatatypeAnalysis
    {
        internal static readonly string rulename = nameof(OWLEnums.OWLValidatorRules.LiteralDatatypeAnalysis);
        internal const string rulesugg = "There should not be typed literals whose lexical form is not well-formed against the value space of their declared datatype!";

        internal static List<OWLIssue> ExecuteRule(OWLOntology ontology)
        {
            List<OWLIssue> issues = new List<OWLIssue>();

            //Local helper implementing dt-not-type: a typed literal is well-formed only if its lexical form
            //belongs to the value space of its declared datatype (e.g: "abc"^^xsd:integer is NOT well-formed).
            //NOTE: literals built via the public OWLSharp/RDFSharp API (RDFTypedLiteral ctor) are ALREADY guaranteed
            //to be well-formed, because RDFTypedLiteral validates eagerly and throws RDFModelException otherwise.
            //The real gap this rule closes is OWL2/XML deserialization: OWLLiteral.Value/DatatypeIRI are plain
            //XmlText/XmlAttribute-bound strings (see OWLLiteral.cs) that are NOT validated at parse time, so a
            //malformed literal loaded from an ontology file survives deserialization silently and would otherwise
            //only surface later as an uncaught RDFModelException the first time some unrelated code path calls
            //OWLLiteral.GetLiteral(). This rule makes that failure mode a graceful, reportable OWLIssue instead.
            bool IsWellFormed(OWLLiteral literal)
            {
                if (literal?.DatatypeIRI == null)
                    return true; //Plain literals (no datatype) have no value-space constraint: dt-not-type only concerns typed literals

                //Resolve the declared datatype from RDFSharp's datatype register (covers the standard XSD/RDF/OWL datatypes)
                RDFDatatype datatype = RDFDatatypeRegister.GetDatatype(literal.DatatypeIRI);
                if (datatype == null)
                    return true; //Unregistered/custom datatype: no known value space to validate against, so it is out of scope for dt-not-type

                //RDFDatatype.Validate is internal to RDFSharp, exposed to OWLSharp via the InternalsVisibleTo grant
                //(same mechanism already used for the OWLShims/RDFTable* SWRL machinery documented in CLAUDE.md)
                return datatype.Validate(literal.Value ?? string.Empty).Item1;
            }

            //DataPropertyAssertion(DP,I,LIT) ^ dt-not-type(LIT) -> ERROR
            foreach (OWLDataPropertyAssertion dpAsn in ontology.GetAssertionAxiomsOfType<OWLDataPropertyAssertion>())
                if (!IsWellFormed(dpAsn.Literal))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated datatype value-space compatibility within DataPropertyAssertion axiom with signature: '{dpAsn.GetXML()}'",
                        rulesugg));

            //NegativeDataPropertyAssertion(DP,I,LIT) ^ dt-not-type(LIT) -> ERROR
            foreach (OWLNegativeDataPropertyAssertion ndpAsn in ontology.GetAssertionAxiomsOfType<OWLNegativeDataPropertyAssertion>())
                if (!IsWellFormed(ndpAsn.Literal))
                    issues.Add(new OWLIssue(
                        OWLEnums.OWLIssueSeverity.Error,
                        rulename,
                        $"Violated datatype value-space compatibility within NegativeDataPropertyAssertion axiom with signature: '{ndpAsn.GetXML()}'",
                        rulesugg));

            return issues;
        }
    }
}
