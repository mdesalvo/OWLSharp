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
using System.Linq;
using OWLSharp.Modeler;
using OWLSharp.Modeler.Axioms;
using OWLSharp.Modeler.Expressions;
using RDFSharp.Model;
using RDFSharp.Query;

namespace OWLSharp.Navigator
{
    public static class OWLAxiomHelper
    {
        #region Methods
		public static List<RDFResource> GetSubClassesOf(this OWLOntology ontology, RDFResource classIRI)
		{
			List<OWLSubClassOf> subClassOfAxioms = ontology.ClassAxioms.OfType<OWLSubClassOf>().ToList();

			#region Utilities
			List<RDFResource> GetSubClassesOfInternal(RDFResource workingClassIRI, Dictionary<long, RDFResource> visitContext)
			{
				List<RDFResource> subClassesInternal = new List<RDFResource>();

				#region VisitContext
				if (!visitContext.ContainsKey(workingClassIRI.PatternMemberID))
					visitContext.Add(workingClassIRI.PatternMemberID, workingClassIRI);
				else
					return subClassesInternal;
				#endregion

				//Direct
				foreach (OWLSubClassOf subClassOfAxiom in subClassOfAxioms.Where(ax => ax.SuperClassExpression is OWLClass superClass && superClass.GetIRI().Equals(workingClassIRI)))
					subClassesInternal.Add(subClassOfAxiom.SubClassExpression.GetIRI());

				//Indirect
				foreach (RDFResource subClassInternal in subClassesInternal.ToList())
					subClassesInternal.AddRange(GetSubClassesOfInternal(subClassInternal, visitContext));

				return subClassesInternal;
			}
			#endregion

			List<RDFResource> subClasses = new List<RDFResource>();
			if (ontology != null && classIRI != null)
				subClasses.AddRange(GetSubClassesOfInternal(classIRI, new Dictionary<long, RDFResource>()));
			return RDFQueryUtilities.RemoveDuplicates(subClasses);
		}
        #endregion
    }
}