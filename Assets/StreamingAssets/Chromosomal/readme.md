```
# Install capnproto and the c# extension
choco install capnproto
choco install capnpc-csharp-win-x86
pip install pycapnp

# compile schema into c# code (you then should move the scripts to th)
capnp compile -ocsharp .\Scripts\chromosome_schema.capnp
```
