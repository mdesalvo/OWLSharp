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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OWLSharp.Profiler;

namespace OWLSharp.Test.Profiler;

[TestClass]
public class OWLProfileWalkerTest
{
    #region Tests
    [TestMethod]
    public void ShouldInvertSubClassPositionToSuperClass()
        => Assert.AreEqual(OWLEnums.OWLClassExpressionPosition.SuperClass, OWLProfileWalker.InvertPosition(OWLEnums.OWLClassExpressionPosition.SubClass));

    [TestMethod]
    public void ShouldInvertSuperClassPositionToSubClass()
        => Assert.AreEqual(OWLEnums.OWLClassExpressionPosition.SubClass, OWLProfileWalker.InvertPosition(OWLEnums.OWLClassExpressionPosition.SuperClass));

    //Corner case that motivated the whole InvertPosition design: RL's equivClassExpression grammar (unlike
    //subClassExpression/superClassExpression) never admits ObjectComplementOf, so it must be a fixed point
    //of the inversion, not cycle back to Sub or Super.
    [TestMethod]
    public void ShouldNotInvertEquivalentClassPosition()
        => Assert.AreEqual(OWLEnums.OWLClassExpressionPosition.EquivalentClass, OWLProfileWalker.InvertPosition(OWLEnums.OWLClassExpressionPosition.EquivalentClass));

    //Applying the inversion twice must be the identity, since Sub and Super are the two poles of a single flip:
    //this catches any future refactor that accidentally turns InvertPosition into a 3-way rotation instead of a 2-way swap.
    [TestMethod]
    public void ShouldBeInvolutoryOnSubClassPosition()
        => Assert.AreEqual(
            OWLEnums.OWLClassExpressionPosition.SubClass,
            OWLProfileWalker.InvertPosition(OWLProfileWalker.InvertPosition(OWLEnums.OWLClassExpressionPosition.SubClass)));
    #endregion
}
