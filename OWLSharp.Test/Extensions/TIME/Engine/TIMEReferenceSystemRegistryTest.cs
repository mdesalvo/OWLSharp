/*
   Copyright 2012-2024 Marco De Salvo

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
using RDFSharp.Model;
using System.Collections.Generic;

namespace OWLSharp.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEReferenceSystemRegistryTest
    {
        #region Tests
        [TestMethod]
        public void ShouldAccessInstance()
        {
            Assert.IsNotNull(TIMEReferenceSystemRegistry.Instance);
            Assert.IsTrue(TIMEReferenceSystemRegistry.Instance is IEnumerable<TIMEReferenceSystem>);
        }

        [TestMethod]
        public void ShouldOperateOnRegistry()
        {
            //Test initial configuration (built-in TRS)
            Assert.IsTrue(TIMEReferenceSystemRegistry.TRSCount >= 3);
            Assert.IsTrue(TIMEReferenceSystemRegistry.ContainsTRS(TIMECalendarReferenceSystem.Gregorian));
            Assert.IsTrue(TIMEReferenceSystemRegistry.ContainsTRS(TIMEPositionReferenceSystem.UnixTRS));
            Assert.IsTrue(TIMEReferenceSystemRegistry.ContainsTRS(TIMEPositionReferenceSystem.GeologicTRS));

            //Test different forms of iteration
            IEnumerator<TIMEReferenceSystem> trsEnumerator = TIMEReferenceSystemRegistry.TRSEnumerator;
            while (trsEnumerator.MoveNext())
                Assert.IsNotNull(trsEnumerator.Current);
            foreach (TIMEReferenceSystem trs in TIMEReferenceSystemRegistry.Instance)
                Assert.IsNotNull(trs);

            //Test addition of TRS
            TIMEPositionReferenceSystem millenniumTRS = new TIMEPositionReferenceSystem(
                new RDFResource("ex:MillenniumTRS"), new TIMECoordinate(2000, 1, 1, 0, 0, 0), TIMEUnit.Day, false);
            Assert.IsFalse(TIMEReferenceSystemRegistry.ContainsTRS(millenniumTRS));            
            TIMEReferenceSystemRegistry.AddTRS(millenniumTRS);
            TIMEReferenceSystemRegistry.AddTRS(millenniumTRS); //Duplicates are avoided
            Assert.IsTrue(TIMEReferenceSystemRegistry.TRSCount >= 4);
            Assert.IsTrue(TIMEReferenceSystemRegistry.ContainsTRS(millenniumTRS));
        }
        #endregion
    }
}