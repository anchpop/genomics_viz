```
# Requires Poetry (for Python dev tooling)
# Poetry installation: https://python-poetry.org/docs/
# (Requires Windows and Choco, Choco installation instructions here: https://chocolatey.org/install)

# Set up anaconda (first time)
conda create -n chromosome_env python=3.9

# Install capnproto and the capnproto C# extension ()
choco install capnproto
choco install capnpc-csharp-win-x86

# Activate anaconda environment
conda activate chromosome_env

# Install python dependencies
cd Scripts/chromsdata_tools
poetry install

## Python commands

# Compile data from text format to chromsdata format
poetry run compile_text_to_binary

## Dev python commands

# compile schema into c# code and copy it into the unity dev directory
poetry run compile_schema
```
