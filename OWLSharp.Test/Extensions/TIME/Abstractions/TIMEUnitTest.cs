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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Test.Extensions.TIME
{
    [TestClass]
    public class TIMEUnitTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateUnit()
        {
            TIMEUnit timeUnit = new TIMEUnit(new RDFResource("ex:unit"), TIMEEnums.TIMEUnitType.Day, 0.25);

            Assert.IsNotNull(timeUnit);
            Assert.IsTrue(timeUnit.Equals(new RDFResource("ex:unit")));
            Assert.IsTrue(timeUnit.UnitType == TIMEEnums.TIMEUnitType.Day);
            Assert.IsTrue(timeUnit.ScaleFactor == 0.25);
        }
        #endregion
    }
}