```
# Install capnproto and the c# extension
choco install capnproto
choco install capnpc-csharp-win-x86
pip install pycapnp

# compile schema into c# code (you then should move the scripts to the `generated` folder)
capnp compile -ocsharp .\Schema\chromsdata.capnp
```
