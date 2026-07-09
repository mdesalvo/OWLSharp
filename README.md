# OWLSharp <a href="https://www.nuget.org/packages/OWLSharp"><img src="https://img.shields.io/nuget/dt/OWLSharp?style=flat&color=9f7aea&logo=nuget&label=downloads"/></a> [![codecov](https://codecov.io/gh/mdesalvo/OWLSharp/graph/badge.svg?token=s7ifp1Uf6D)](https://codecov.io/gh/mdesalvo/OWLSharp)

OWLSharp is a .NET library built atop <a href="https://github.com/mdesalvo/RDFSharp">RDFSharp</a> with the goal of delivering **expressivity** for ontology:

<b><a href="https://github.com/mdesalvo/OWLSharp/releases/download/v5.0.0/OWLSharp.Ontology-5.0.pdf">Modeling</a></b>
<ul>
    <li>Create and manage <b>OWL2 ontologies</b> (classes, properties, individuals, expressions, axioms, rules, ...)</li>
    <li>Exchange them using standard <b>OWL2 formats</b> (OWL2/XML, OWL2/Manchester)</li>
</ul>

<b><a href="https://github.com/mdesalvo/OWLSharp/releases/download/v5.0.0/OWLSharp.Reasoner-5.0.pdf">Reasoning</a></b>
<ul>
    <li>Apply forward-chain inference to derive knowledge that is entailed but not stated (<b>26 rules</b>)</li>
    <li>Create, exchange and execute <b>SWRL rules</b> encoding domain-specific logic deductions</li>
</ul>

<b><a href="https://github.com/mdesalvo/OWLSharp/releases/download/v5.0.0/OWLSharp.Profiler-5.0.pdf">Profiling</a></b>
<ul>
    <li>Check grammar compliance against <b>OWL2 profiles</b> (EL, RL, QL)</li>
</ul>

<b><a href="https://github.com/mdesalvo/OWLSharp/releases/download/v5.0.0/OWLSharp.Validator-5.0.pdf">Validation</a></b>
<ul>
    <li>Detect pitfalls, structural inconsistencies and constraint violations (<b>29 rules</b>)</li>
</ul>
<hr />

Along with core features, it also includes a set of <a href="https://github.com/mdesalvo/OWLSharp.Extensions">extensions</a> for working with <b>LinkedData ontologies</b>!
