# OWLSharp <a href="https://github.com/mdesalvo/OWLSharp/releases"><img src="https://img.shields.io/nuget/v/OWLSharp?style=flat-square&color=abcdef&logo=nuget&label=version"/></a> <a href="https://www.nuget.org/packages/OWLSharp"><img src="https://img.shields.io/nuget/dt/OWLSharp?style=flat-square&color=abcdef&logo=nuget&label=downloads"/></a> <a href="https://app.codecov.io/gh/mdesalvo/OWLSharp"><img src="https://img.shields.io/codecov/c/github/mdesalvo/OWLSharp?style=flat-square&color=04aa6d&logo=codecov&label=coverage"/></a>

OWLSharp is a .NET library built atop <a href="https://github.com/mdesalvo/RDFSharp">RDFSharp</a> with the goal of enabling semantic expressivity for:
<ul>
  <li>Modeling <b>OWL2 ontologies</b> (with tested Protégé compatibility)</li>
  <li>Exchanging them using standard <b>OWL2 formats</b> (OWL2/XML) and also <b>RDF formats</b> (via RDFSharp)
  <li><b>Reasoning</b> on them with a set of <b>25 OWL2 inference rules</b></li>
  <li><b>Validating</b> them with a set of <b>29 OWL2 analysis rules</b></li>
</ul>
It also integrates a powerful <b>SWRL engine</b> for modeling, exchange and execution of custom inference rules
<hr style="height:0.05em !important" />

Along with core ontology features, it also includes a set of extensions providing additional capabilities:
<ul>
  <li>Create and validate schemes describing, documenting and organizing vocabularies of concepts (<b>SKOS</b>)</li>
  <li>Model and correlate features having a spatio-temporal representation (<b>GeoSPARQL</b>, <b>OWL-TIME</b>)</li>
</ul>
