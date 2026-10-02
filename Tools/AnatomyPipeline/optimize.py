#!/usr/bin/env python3
"""Audit exact vertex welding savings without changing the source geometry."""
import json,struct
from pathlib import Path
root=Path(__file__).resolve().parents[2];catalog=json.loads((root/'Assets/AnatomyLab/Resources/Anatomy/anatomy_catalog.json').read_text());report=[]
for d in catalog['structures']:
 path=root/'Assets/AnatomyLab/Resources'/(d['meshReference']+'.bytes');b=path.read_bytes();nv,nt=struct.unpack_from('<ii',b);unique=len(set(b[8+i*12:20+i*12] for i in range(nv)))
 report.append(dict(id=d['id'],verticesBefore=nv,uniquePositions=unique,triangles=nt,potentialVertexBytesSaved=(nv-unique)*24))
out=root/'TestResults/mesh-optimization-audit.json';out.write_text(json.dumps(report,indent=2));print('Audit written:',out);print('Potential exact-position welding savings:',sum(r['potentialVertexBytesSaved'] for r in report),'bytes. No files modified. Review shading before welding.')
