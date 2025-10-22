# OWLSharp <a href="https://www.nuget.org/packages/OWLSharp"><img src="https://img.shields.io/nuget/dt/OWLSharp?style=flat&color=abcdef&logo=nuget&label=downloads"/></a> [![codecov](https://codecov.io/gh/mdesalvo/OWLSharp/graph/badge.svg?token=s7ifp1Uf6D)](https://codecov.io/gh/mdesalvo/OWLSharp)

OWLSharp is a .NET library built atop <a href="https://github.com/mdesalvo/RDFSharp">RDFSharp</a> with the goal of delivering **semantic expressivity** for:

<b>Ontology Modeling</b>
<ul>
    <li>Create and manage <b>OWL2 ontologies</b> (classes, properties, individuals, expressions, axioms, rules, ...)</li>
    <li>Exchange them using standard <b>OWL2 formats</b> (OWL2/Xml)</li>
</ul>

<b>Ontology Reasoning</b>
<ul>
    <li>Apply logical deduction to derive knowledge that is entailed, but not stated, with <b>24 inference rules</b></li>
    <li>Create, exchange and execute <b>SWRL rules</b> encoding business logic deductions targeting ontology A-BOX</li>
</ul>

<b>Ontology Validation</b>
<ul>
    <li>Detect pitfalls, structural inconsistencies and constraint violations with <b>29 analysis rules</b></li>
</ul>
<hr />

Along with core ontology features, it also includes a set of <a href="https://github.com/mdesalvo/OWLSharp.Extensions">extensions</a> for working with <b>LinkedData ontologies</b>!
