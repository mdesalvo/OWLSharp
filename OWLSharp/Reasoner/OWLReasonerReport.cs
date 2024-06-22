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

using System.Collections.Generic;
using OWLSharp.Ontology;
using OWLSharp.Ontology.Axioms;

namespace OWLSharp.Reasoner
{
    public class OWLReasonerReport
    {
        #region Properties
        public List<OWLReasonerInference> Inferences { get; set; }
        #endregion

        #region Ctors
        public OWLReasonerReport()
            => Inferences = new List<OWLReasonerInference>();
        #endregion

        #region Methods
        public void JoinInferences(OWLOntology ontology)
        	=>	Inferences.ForEach(inf => 
				{
					if (inf.Content is OWLAnnotationAxiom annAxInf)
						ontology?.AnnotationAxioms.Add(annAxInf);
					else if (inf.Content is OWLAssertionAxiom asnAxInf)
						ontology?.AssertionAxioms.Add(asnAxInf);
					else if (inf.Content is OWLClassAxiom clsAxInf)
						ontology?.ClassAxioms.Add(clsAxInf);
					else if (inf.Content is OWLDataPropertyAxiom dpAxInf)
						ontology?.DataPropertyAxioms.Add(dpAxInf);
					else if (inf.Content is OWLHasKey keyAxInf)
						ontology?.KeyAxioms.Add(keyAxInf);
					else if (inf.Content is OWLObjectPropertyAxiom opAxInf)
						ontology?.ObjectPropertyAxioms.Add(opAxInf);
				});
        #endregion
    }
}