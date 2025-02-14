/*
   Copyright 2014-2025 Marco De Salvo

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
using OWLSharp.Extensions.TIME;
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEUnitTypeRegistryTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAccessInstance()
        {
            Assert.IsNotNull(TIMEUnitTypeRegistry.Instance);
        }

        [TestMethod]
        public void ShouldOperateOnRegistry()
        {
            //Test initial configuration
            Assert.AreEqual(13, TIMEUnitTypeRegistry.UnitTypeCount);
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Millennium));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Century));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Decade));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Year));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Month));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Week));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Day));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Hour));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Minute));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.Second));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.BillionYearsAgo));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.MillionYearsAgo));
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(TIMEUnit.MarsSol));

            //Test different forms of iteration
            IEnumerator<TIMEUnit> unitTypeEnumerator = TIMEUnitTypeRegistry.UnitTypeEnumerator;
            while (unitTypeEnumerator.MoveNext())
                Assert.IsNotNull(unitTypeEnumerator.Current);
            foreach (TIMEUnit unitType in TIMEUnitTypeRegistry.Instance)
                Assert.IsNotNull(unitType);

            //Test addition of unit types
            TIMEUnit timeUnit = new TIMEUnit(new RDFResource("ex:HalfHour"), TIMEEnums.TIMEUnitType.Hour, 0.5);
            Assert.IsFalse(TIMEUnitTypeRegistry.ContainsUnitType(timeUnit));
            TIMEUnitTypeRegistry.AddUnitType(timeUnit);
            TIMEUnitTypeRegistry.AddUnitType(timeUnit); //Duplicates are avoided
            Assert.AreEqual(14, TIMEUnitTypeRegistry.UnitTypeCount);
            Assert.IsTrue(TIMEUnitTypeRegistry.ContainsUnitType(timeUnit));
        }
        #endregion
    }
}