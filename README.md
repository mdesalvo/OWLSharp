# OWLSharp <a href="https://www.nuget.org/packages/OWLSharp"><img src="https://img.shields.io/nuget/dt/OWLSharp?style=flat-square&color=abcdef&logo=nuget&label=downloads"/></a> [![codecov](https://codecov.io/gh/mdesalvo/OWLSharp/graph/badge.svg?token=s7ifp1Uf6D)](https://codecov.io/gh/mdesalvo/OWLSharp)

OWLSharp is a .NET library built atop <a href="https://github.com/mdesalvo/RDFSharp">RDFSharp</a> with the goal of enabling **semantic expressivity** for:
<ul>
  <li>Modeling <b>OWL2 ontologies</b> (classes, properties, individuals, expressions, axioms, annotations, rules, ...)</li>
  <li>Exchanging them using standard <b>OWL2 formats</b> (OWL2/XML)
  <li><b>Reasoning</b> on them with a set of <b>25 OWL2 inference rules</b></li>
  <li><b>Validating</b> them with a set of <b>29 OWL2 analysis rules</b></li>
</ul>
It also integrates a powerful <b>SWRL engine</b> for modeling, exchanging and executing custom inference rules
<hr />

Along with core ontology features, it also includes a set of extensions providing additional capabilities:
<ul>
  <li>Create and validate schemes describing, documenting and organizing vocabularies of concepts (<b>SKOS</b>)</li>
  <li>Model and analyze features having a spatio-temporal representation (<b>GeoSPARQL</b>, <b>OWL-TIME</b>)</li>
</ul>
