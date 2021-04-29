from pathlib import Path
import subprocess
import shutil

def compile_and_copy_schema():
    basepath = Path("../../")
    filename = Path("chromsdata.capnp.cs")

    shema_path = basepath / Path("Schema") / filename
    codepath = basepath / Path("../../Scripts/Generated")  / filename

    subprocess.run(["capnp", "compile", "-ocsharp", ".\Schema\chromsdata.capnp"], cwd=basepath)
    shutil.move(shema_path, codepath)