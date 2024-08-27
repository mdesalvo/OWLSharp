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

using OWLSharp.Ontology.Rules.Atoms;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace OWLSharp.Ontology.Rules
{
    [XmlRoot("Body")]
    public class SWRLAntecedent
    {
        #region Properties
        [XmlElement(typeof(SWRLBuiltIn), ElementName="BuiltInAtom")]
        [XmlElement(typeof(SWRLClassAtom), ElementName="ClassAtom")]
        [XmlElement(typeof(SWRLDataPropertyAtom), ElementName="DataPropertyAtom")]
        [XmlElement(typeof(SWRLDataRangeAtom), ElementName="DataRangeAtom")]
        [XmlElement(typeof(SWRLDifferentIndividualsAtom), ElementName="DifferentIndividualsAtom")]
        [XmlElement(typeof(SWRLObjectPropertyAtom), ElementName="ObjectPropertyAtom")]
        [XmlElement(typeof(SWRLSameIndividualAtom), ElementName="SameIndividualAtom")]
        public List<SWRLAtom> Atoms { get; set; }
        #endregion

        #region Ctors
        public SWRLAntecedent()
            => Atoms = new List<SWRLAtom>();
        #endregion

		#region Interfaces
        public override string ToString()
            => string.Join(" ^ ", Atoms);
        #endregion

        #region Methods
        internal DataTable Evaluate(OWLOntology ontology)
        {
            //Execute the antecedent atoms
            List<DataTable> atomResults = new List<DataTable>();
            Atoms.Where(atom => !(atom is SWRLBuiltIn))
                 .ToList()
                 .ForEach(atom => atomResults.Add(atom.EvaluateOnAntecedent(ontology)));

            //Join results of antecedent atoms
            DataTable antecedentResult = RDFQueryEngine.CombineTables(atomResults, false);

            //Execute the antecedent built-ins
            Atoms.OfType<SWRLBuiltIn>()
                 .ToList()
                 .ForEach(builtin => antecedentResult = builtin.EvaluateOnAntecedent(antecedentResult, ontology));

            //Return the antecedent result
            return antecedentResult;
        }
        #endregion
    }
}