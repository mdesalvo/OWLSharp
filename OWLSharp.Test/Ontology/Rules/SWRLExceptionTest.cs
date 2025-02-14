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
using OWLSharp.Ontology;
using System;

namespace OWLSharp.Test.Ontology;

[TestClass]
public class SWRLExceptionTest
{
    #region Tests
    [TestMethod]
    public void ShouldRaiseEmptyException()
    {
        try
        {
            throw new SWRLException();
        }
        catch (SWRLException mex)
        {
            Assert.IsTrue(mex.Message.Contains("OWLSharp.Ontology.SWRLException", StringComparison.OrdinalIgnoreCase));
        }
    }

    [TestMethod]
    public void ShouldRaiseMessageException()
    {
        try
        {
            throw new SWRLException("This is an exception coming from SWRL modeling!");
        }
        catch (SWRLException mex)
        {
            Assert.IsTrue(mex.Message.Equals("This is an exception coming from SWRL modeling!", StringComparison.OrdinalIgnoreCase));
            Assert.IsNull(mex.InnerException);
        }
    }

    [TestMethod]
    public void ShouldRaiseMessageWithInnerException()
    {
        try
        {
            throw new SWRLException("This is an exception coming from SWRL modeling!", new Exception("This is the inner exception!"));
        }
        catch (SWRLException mex)
        {
            Assert.IsTrue(mex.Message.Equals("This is an exception coming from SWRL modeling!", StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(mex.InnerException);
            Assert.IsTrue(mex.InnerException.Message.Equals("This is the inner exception!"));
        }
    }
    #endregion
}