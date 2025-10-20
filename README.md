# OWLSharp <a href="https://www.nuget.org/packages/OWLSharp"><img src="https://img.shields.io/nuget/dt/OWLSharp?style=flat&color=abcdef&logo=nuget&label=downloads"/></a> [![codecov](https://codecov.io/gh/mdesalvo/OWLSharp/graph/badge.svg?token=s7ifp1Uf6D)](https://codecov.io/gh/mdesalvo/OWLSharp)

OWLSharp is a .NET library built atop <a href="https://github.com/mdesalvo/RDFSharp">RDFSharp</a> with the goal of enabling **semantic expressivity** for ontology:

<b>Modeling</b>
<ul>
    <li>Create and manage <b>OWL2 ontologies</b> (classes, properties, individuals, expressions, axioms, annotations, rules, ...)</li>
    <li>Exchange them using standard <b>OWL2 formats</b> (OWL2/Xml)</li>
</ul>

<b>Validation</b>
<ul>
    <li>Detect modeling pitfalls, inconsistencies and contraddictions through a set of <b>29 OWL2 analysis rules</b></li>
</ul>

<b>Reasoning</b>
<ul>
    <li>Infer new knowledge through a set of <b>24 OWL2 inference rules</b></li>
    <li>Build, exchange and execute custom <b>SWRL rules</b> targeting ontology A-BOX</li>
</ul>
<hr />

Along with core ontology features, it also includes a set of <a href="https://github.com/mdesalvo/OWLSharp.Extensions">extensions</a> for working with <b>LinkedData ontologies</b>!
