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

namespace OWLSharp.Ontology.Expressions
{
	public static class OWLExpressionHelper
	{
		#region Methods
		public static List<T> RemoveDuplicates<T>(List<T> expressions) where T : OWLExpression
        {
            List<T> deduplicatedExpressions = new List<T>();
            if (expressions?.Count > 0)
            {
                HashSet<long> lookup = new HashSet<long>();
                expressions.ForEach(expression =>
                {
					long expressionID = expression.GetIRI().PatternMemberID;
                    if (!lookup.Contains(expressionID))
                    {
                        lookup.Add(expressionID);
                        deduplicatedExpressions.Add(expression);
                    }
                });
            }
            return deduplicatedExpressions;
        }
		#endregion
	}
}