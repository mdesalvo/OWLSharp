# OWLSharp <a href="https://github.com/mdesalvo/OWLSharp/releases"><img src="https://img.shields.io/nuget/v/OWLSharp?style=flat-square&color=abcdef&logo=nuget&label=version"/></a> <a href="https://www.nuget.org/packages/OWLSharp"><img src="https://img.shields.io/nuget/dt/OWLSharp?style=flat-square&color=abcdef&logo=nuget&label=downloads"/></a> <a href="https://app.codecov.io/gh/mdesalvo/OWLSharp"><img src="https://img.shields.io/codecov/c/github/mdesalvo/OWLSharp?style=flat-square&color=04aa6d&logo=codecov&label=coverage"/></a>

OWLSharp is an API built atop <a href="https://github.com/mdesalvo/RDFSharp">RDFSharp</a> with the goal of enabling semantic expressivity for:
<ul>
  <li>Modeling <b><a href="https://www.w3.org/TR/owl2-overview/">OWL2</a> ontologies</b> (with tested <a href="https://protege.stanford.edu/">Protégé</a> compatibility)</li>
  <li>Exchanging them using standard <b>OWL2 formats</b> (OWL2/XML) and also <b>RDF formats</b> (via RDFSharp)
  <li><b>Reasoning</b> on them with a set of <b>25 OWL2 inference rules</b></li>
  <li><b>Validating</b> them with a set of <b>29 OWL2 + 6 SKOS analysis rules</b></li>
</ul>
It also integrates a powerful <b><a href="https://www.w3.org/submissions/SWRL/">SWRL</a> engine</b> for modeling, exchange and execution of custom inference rules
