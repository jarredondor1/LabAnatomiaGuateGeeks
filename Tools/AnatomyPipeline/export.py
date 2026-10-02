#!/usr/bin/env python3
"""Export an offline dataset bundle: python3 export.py /path/to/new-directory"""
import sys,json,shutil,struct
from pathlib import Path
root=Path(__file__).resolve().parents[2];source=root/'Assets/AnatomyLab/Resources/Anatomy'
if len(sys.argv)!=2:raise SystemExit('Provide a new output directory')
target=Path(sys.argv[1]).resolve()
if target.exists():raise SystemExit('Destination exists; choose a new directory')
c=json.loads((source/'anatomy_catalog.json').read_text())
for d in c['structures']:
 b=(source.parent/(d['meshReference']+'.bytes')).read_bytes();nv,nt=struct.unpack_from('<ii',b)
 if len(b)!=8+nv*12+nt*12:raise SystemExit('Invalid mesh '+d['id'])
shutil.copytree(source,target,ignore=shutil.ignore_patterns('*.meta'));shutil.copyfile(root/'LICENSE_ATTRIBUTION.md',target/'LICENSE_ATTRIBUTION.md');print('Exported',len(c['structures']),'meshes to',target)
