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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;

namespace OWLSharp.Serialization.Test
{
    [TestClass]
    public class OWLDatatypeRestrictionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateDatatypeRestriction()
        {
            OWLDatatypeRestriction length6to10Facet = new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)]);

            Assert.IsNotNull(length6to10Facet);
            Assert.IsNotNull(length6to10Facet.Datatype);
            Assert.IsTrue(string.Equals(length6to10Facet.Datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
            Assert.IsNotNull(length6to10Facet.FacetRestrictions);
            Assert.IsTrue(length6to10Facet.FacetRestrictions.Count == 2);
            Assert.IsTrue(length6to10Facet.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, OWLFacetRestriction.MIN_LENGTH.ToString()) 
                                                                        && string.Equals(fr.Literal.Value, "6")
                                                                        && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
            Assert.IsTrue(length6to10Facet.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, OWLFacetRestriction.MAX_LENGTH.ToString()) 
                                                                        && string.Equals(fr.Literal.Value, "10")
                                                                        && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseNullDatatypeIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(
                null,
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseNullFacetRestrictions()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.STRING), null));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseEmptyFacetRestrictions()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.STRING), []));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseFoundNullFacetRestriction()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(new OWLDatatype(RDFVocabulary.XSD.STRING), [null]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseFacetRestrictionWithNullLiteral()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(null, OWLFacetRestriction.MIN_LENGTH),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseFacetRestrictionWithNullIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), null),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)]));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDatatypeRestrictionBecauseFacetRestrictionWithBlankIRI()
            => Assert.ThrowsException<OWLException>(() => new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), new RDFResource()),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)]));

        [TestMethod]
        public void ShouldSerializeDatatypeRestriction()
        {
            OWLDatatypeRestriction length6to10Facet = new OWLDatatypeRestriction(
                new OWLDatatype(RDFVocabulary.XSD.STRING),
                [new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("6", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MIN_LENGTH),
                 new OWLFacetRestriction(new OWLLiteral(new RDFTypedLiteral("10", RDFModelEnums.RDFDatatypes.XSD_INT)), OWLFacetRestriction.MAX_LENGTH)]);
            string serializedXML = OWLTestSerializer<OWLDatatypeRestriction>.Serialize(length6to10Facet);

            Assert.IsTrue(string.Equals(serializedXML,
@"<DatatypeRestriction>
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
  <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#minLength"">
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">6</Literal>
  </FacetRestriction>
  <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#maxLength"">
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">10</Literal>
  </FacetRestriction>
</DatatypeRestriction>"));
        }

        [TestMethod]
        public void ShouldDeserializeDatatypeRestriction()
        {
            OWLDatatypeRestriction length6to10Facet = OWLTestSerializer<OWLDatatypeRestriction>.Deserialize(
@"<DatatypeRestriction>
  <Datatype IRI=""http://www.w3.org/2001/XMLSchema#string"" />
  <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#minLength"">
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">6</Literal>
  </FacetRestriction>
  <FacetRestriction facet=""http://www.w3.org/2001/XMLSchema#maxLength"">
    <Literal datatypeIRI=""http://www.w3.org/2001/XMLSchema#int"">10</Literal>
  </FacetRestriction>
</DatatypeRestriction>");

            Assert.IsNotNull(length6to10Facet);
            Assert.IsNotNull(length6to10Facet.Datatype);
            Assert.IsTrue(string.Equals(length6to10Facet.Datatype.IRI, RDFVocabulary.XSD.STRING.ToString()));
            Assert.IsNotNull(length6to10Facet.FacetRestrictions);
            Assert.IsTrue(length6to10Facet.FacetRestrictions.Count == 2);
            Assert.IsTrue(length6to10Facet.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, OWLFacetRestriction.MIN_LENGTH.ToString()) 
                                                                        && string.Equals(fr.Literal.Value, "6")
                                                                        && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
            Assert.IsTrue(length6to10Facet.FacetRestrictions.Any(fr => string.Equals(fr.FacetIRI, OWLFacetRestriction.MAX_LENGTH.ToString()) 
                                                                        && string.Equals(fr.Literal.Value, "10")
                                                                        && string.Equals(fr.Literal.DatatypeIRI, RDFVocabulary.XSD.INT.ToString())));
        }
        #endregion
    }    
}