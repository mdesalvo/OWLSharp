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
public class OWLRLProfileTest
{
    #region Tests

    //RL's textbook realm is rule-engine-friendly business data (order fulfillment, e-commerce): scalable
    //reasoning over ABox-heavy data. This fixture deliberately uses FunctionalObjectProperty (admitted by RL,
    //unlike EL/QL) and a 3-way DisjointClasses, and will be extended by the RL implementation phase (task #3)
    //with real violating constructs (e.g. ReflexiveObjectProperty, which RL explicitly disallows).
    private static OWLOntology BuildOrderFulfillmentOntology()
    {
        OWLClass order = new OWLClass(new RDFResource("http://shop.org/Order"));
        OWLClass priorityOrder = new OWLClass(new RDFResource("http://shop.org/PriorityOrder"));
        OWLClass customer = new OWLClass(new RDFResource("http://shop.org/Customer"));
        OWLClass product = new OWLClass(new RDFResource("http://shop.org/Product"));
        OWLObjectProperty placedBy = new OWLObjectProperty(new RDFResource("http://shop.org/placedBy"));

        return new OWLOntology
        {
            DeclarationAxioms = [
                new OWLDeclaration(order), new OWLDeclaration(priorityOrder),
                new OWLDeclaration(customer), new OWLDeclaration(product),
                new OWLDeclaration(placedBy)
            ],
            ClassAxioms = [
                new OWLSubClassOf(priorityOrder, order),
                //RL's subClassExpression grammar admits ObjectSomeValuesFrom with a subCE filler
                new OWLSubClassOf(priorityOrder, new OWLObjectSomeValuesFrom(placedBy, customer)),
                new OWLDisjointClasses([order, customer, product])
            ],
            ObjectPropertyAxioms = [
                new OWLObjectPropertyDomain(placedBy, order),
                new OWLObjectPropertyRange(placedBy, customer),
                //FunctionalObjectProperty is explicitly admitted by RL's ObjectPropertyAxiom grammar (§4.2.5),
                //unlike EL and QL which both disallow it
                new OWLFunctionalObjectProperty(placedBy)
            ]
        };
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnRealisticOntologyAsync()
    {
        OWLOntology ontology = BuildOrderFulfillmentOntology();

        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(ontology);

        Assert.IsNotNull(violations);
        //Baseline for the scaffolding phase: OWLRLProfile is still a stub (see task #3). This fixture is
        //fully RL-compliant on purpose, so it must keep producing zero violations once real rules land.
        Assert.IsEmpty(violations);
    }

    [TestMethod]
    public async Task ShouldExecuteRuleWithoutThrowingOnEmptyOntologyAsync()
    {
        List<OWLProfileViolation> violations = await OWLRLProfile.ExecuteRuleAsync(new OWLOntology());

        Assert.IsNotNull(violations);
        Assert.IsEmpty(violations);
    }
    #endregion
}
