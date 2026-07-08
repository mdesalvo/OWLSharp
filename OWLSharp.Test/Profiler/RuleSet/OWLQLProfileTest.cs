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
public class OWLQLProfileTest
{
    #region Tests

    //QL's textbook realm is OBDA/data integration (DL-Lite): thin TBoxes over large relational-like ABoxes,
    //answered via query rewriting. This fixture uses ObjectPropertyDomain/Range (both admitted by QL) and
    //will be extended by the QL implementation phase (task #4) with real violating constructs — most notably
    //a qualified ObjectSomeValuesFrom used in SUBclass position, which QL's grammar reserves for
    //superclass position only (see owl2_profiles_w3c_spec memory note).
    private static OWLOntology BuildEmployeeDepartmentOntology()
    {
        OWLClass employee = new OWLClass(new RDFResource("http://corp.org/Employee"));
        OWLClass manager = new OWLClass(new RDFResource("http://corp.org/Manager"));
        OWLClass department = new OWLClass(new RDFResource("http://corp.org/Department"));
        OWLObjectProperty worksFor = new OWLObjectProperty(new RDFResource("http://corp.org/worksFor"));

        return new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(employee), new OWLDeclaration(manager),
                new OWLDeclaration(department), new OWLDeclaration(worksFor)
            ],
            ClassAxioms = [
                new OWLSubClassOf(manager, employee)
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(worksFor, employee),
                new OWLObjectPropertyRange(worksFor, department)
            ]
        };
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnRealisticOntologyAsync()
    {
        OWLOntology ontology = BuildEmployeeDepartmentOntology();

        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(violations);
        //Baseline for the scaffolding phase: OWLQLProfile is still a stub (see task #4). This fixture is
        //fully QL-compliant on purpose, so it must keep producing zero violations once real rules land.
        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnEmptyOntologyAsync()
    {
        List<OWLProfileViolation> violations = await OWLQLProfile.ExecuteRuleAsync(new OWLOntology());

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }
    #endregion
}
