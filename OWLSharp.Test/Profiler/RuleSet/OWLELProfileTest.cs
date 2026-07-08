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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Ontology;
using OWLSharp.Profiler;
using RDFSharp.Model;

namespace OWLSharp.Test.Profiler.RuleSet;

[TestClass]
public class OWLELProfileTest
{
    #region Tests

    //EL's textbook realm is biomedical terminology (SNOMED/GO-like): large TBoxes dominated by existential
    //restrictions, which is exactly what EL is designed to classify in polynomial time. This small "infectious
    //disease" TBox is the fixture the EL implementation phase (task #2) will extend with real violating
    //constructs (e.g. ObjectAllValuesFrom, ObjectUnionOf) once OWLELProfile stops being a stub.
    private static OWLOntology BuildInfectiousDiseaseOntology()
    {
        OWLClass disease = new OWLClass(new RDFResource("http://biomed.org/Disease"));
        OWLClass infectiousDisease = new OWLClass(new RDFResource("http://biomed.org/InfectiousDisease"));
        OWLClass symptom = new OWLClass(new RDFResource("http://biomed.org/Symptom"));
        OWLClass fever = new OWLClass(new RDFResource("http://biomed.org/Fever"));
        OWLObjectProperty hasSymptom = new OWLObjectProperty(new RDFResource("http://biomed.org/hasSymptom"));

        return new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(disease), new OWLDeclaration(infectiousDisease),
                new OWLDeclaration(symptom), new OWLDeclaration(fever),
                new OWLDeclaration(hasSymptom)
            ],
            ClassAxioms = [
                new OWLSubClassOf(fever, symptom),
                new OWLSubClassOf(infectiousDisease, disease),
                //EL's flagship construct: existential restriction admitted in EL's unique ClassExpression grammar
                new OWLSubClassOf(infectiousDisease, new OWLObjectSomeValuesFrom(hasSymptom, fever))
            ],
            ObjectPropertyAxioms = [
                //TransitiveObjectProperty is explicitly admitted by EL's ObjectPropertyAxiom grammar (§2.2.5)
                new OWLTransitiveObjectProperty(hasSymptom)
            ]
        };
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnRealisticOntologyAsync()
    {
        OWLOntology ontology = BuildInfectiousDiseaseOntology();

        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(violations);
        //Baseline for the scaffolding phase: OWLELProfile is still a stub (see task #2), so no grammar
        //check runs yet. This assertion is expected to be revisited once the EL implementation phase lands,
        //at which point this fixture (fully EL-compliant on purpose) must keep producing zero violations.
        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnEmptyOntologyAsync()
    {
        List<OWLProfileViolation> violations = await OWLELProfile.ExecuteRuleAsync(new OWLOntology());

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }
    #endregion
}
