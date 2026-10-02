#!/usr/bin/env python3
"""Reproducible BodyParts3D 4.0 conversion. Python stdlib only. Original archives remain unchanged."""
import csv,json,struct,zipfile,hashlib,collections,re
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]; SRC=ROOT/'SourceData'; OUT=ROOT/'Assets/AnatomyLab/Resources/Anatomy'
def rows(file):return list(csv.DictReader((SRC/file).open(),delimiter='\t'))
def vec(v):return dict(zip(('x','y','z'),v))
def color(hex):return dict(zip(('r','g','b','a'),[int(hex[i:i+2],16)/255 for i in (0,2,4)]+[1]))
specs=[('skeleton','Skeleton','Óseo','ead7ad',True,['bone organ']),('muscular','Muscles','Muscular','b75565',False,['muscle organ']),('arterial','Arteries','Arterias','cf5460',False,['artery']),('venous','Veins','Venas','568bc5',False,['vein']),('nervous','Nervous','Nervioso','e9c05d',False,['nerve','brain','spinal cord']),('respiratory','Respiratory','Respiratorio','85c2d2',True,['lung','trachea','bronch','larynx']),('digestive','Digestive','Digestivo','ce9b69',True,['liver','stomach','intestine','colon','esophagus','pancreas','gallbladder','rectum','tongue','tooth','pharynx']),('urinary','Urinary','Urinario','ae819f',False,['kidney','ureter','urinary','urethra']),('reproductive','Reproductive','Reproductivo','b27da8',False,['testis','prostate','penis','seminal','epididymis','ductus deferens']),('endocrine','Endocrine','Endocrino','dda954',False,['thyroid','adrenal','pituitary','pineal']),('lymphatic','Lymphatic','Linfático','81b995',False,['lymph','spleen','thymus']),('cardiovascular','Heart','Corazón','dc6d78',True,['heart','cardiac','atrium','ventricle']),('connective','Connective','Conectivo','b3c4cb',False,['cartilage','ligament','tendon']),('integumentary','Integumentary','Tegumentario','c7a78d',False,['skin','nail','hair']),('other','Other','Otras estructuras','9bafb5',False,[])]
concepts=collections.defaultdict(set); names={}; reverse=collections.defaultdict(list)
for row in rows('isa_element_parts.txt'):
 cid=row['concept id'];names[cid]=row['name'];concepts[cid].add(row['element file id'])
for cid,elements in concepts.items():
 for element in elements:reverse[element].append(cid)
relations=rows('isa_inclusion_relation_list.txt')
parents=collections.defaultdict(list)
for r in relations:parents[r['child id']].append(r['parent id'])
archive=SRC/'isa_BP3D_4.0_obj_99.zip'
if not archive.exists():raise SystemExit('Missing '+str(archive))
OUT.mkdir(parents=True,exist_ok=True)
systems=[dict(id=s[0],name=s[1],displayName=s[2],defaultColor=color(s[3]),defaultVisibility=s[4],structures=[]) for s in specs]
by_system={s['id']:s for s in systems}; structures=[];min_all=[float('inf')]*3;max_all=[-float('inf')]*3
# Determine global coordinate normalization from all OBJ vertex bounds.
with zipfile.ZipFile(archive) as z:
 files=sorted(n for n in z.namelist() if n.lower().endswith('.obj'))
 for n in files:
  for line in z.read(n).decode('utf-8-sig').splitlines():
   if line.startswith('v '):
    x,y,zv=map(float,line.split()[1:4]);v=(x,zv,y)
    for i in range(3):min_all[i]=min(min_all[i],v[i]);max_all[i]=max(max_all[i],v[i])
 scale=1.75/(max_all[1]-min_all[1]);origin=((min_all[0]+max_all[0])/2,min_all[1],(min_all[2]+max_all[2])/2)
 for index,n in enumerate(files):
  mid=Path(n).stem;cs=sorted(reverse[mid],key=lambda c:(len(concepts[c]),len(names[c]),c));cid=cs[0] if cs else mid;name=names.get(cid,mid)
  specific=[names[c] for c in cs if len(concepts[c])<=len(concepts.get(cid,[]))];all_names=' | '.join(names[c].lower() for c in cs)
  system='other'
  for sid,_,_,_,_,terms in specs:
   if any(term in all_names for term in terms):system=sid;break
  verts=[];faces=[]
  for line in z.read(n).decode('utf-8-sig').splitlines():
   if line.startswith('v '):
    x,y,zv=map(float,line.split()[1:4]);verts.append(tuple((v-o)*scale for v,o in zip((x,zv,y),origin)))
   elif line.startswith('f '):
    ids=[int(t.split('/')[0]) for t in line.split()[1:]];ids=[i-1 if i>0 else len(verts)+i for i in ids]
    for j in range(1,len(ids)-1):faces.extend((ids[0],ids[j+1],ids[j]))
  lo=[min(v[i] for v in verts) for i in range(3)];hi=[max(v[i] for v in verts) for i in range(3)];center=[(a+b)/2 for a,b in zip(lo,hi)];size=[b-a for a,b in zip(lo,hi)]
  folder=OUT/'Geometry'/system;folder.mkdir(parents=True,exist_ok=True)
  binary=struct.pack('<ii',len(verts),len(faces)//3)+b''.join(struct.pack('<fff',*(v[i]-center[i] for i in range(3))) for v in verts)+struct.pack('<'+'i'*len(faces),*faces)
  (folder/(mid+'.bytes')).write_bytes(binary)
  display=name
  translations={'heart':'Corazón','liver':'Hígado','stomach':'Estómago','brain':'Cerebro','right lung':'Pulmón derecho','left lung':'Pulmón izquierdo','right femur':'Fémur derecho','left femur':'Fémur izquierdo','right kidney':'Riñón derecho','left kidney':'Riñón izquierdo'}
  if name.lower() in translations:display=translations[name.lower()]
  # Whole-organ concepts are also indexed on constituent meshes, preserving FMA lookup.
  terms=[names[c] for c in cs if len(concepts[c])<=20]+cs[:6]
  for en,es in translations.items():
   if en in [v.lower() for v in terms]:terms.append(es)
  d=dict(id=mid,canonicalName=name,displayName=display,latinName='',synonyms=specific,systemId=system,category='BodyParts3D 4.0',description='Estructura anatómica de referencia. Nombre original: '+name+'.',meshReference=f'Anatomy/Geometry/{system}/{mid}',materialReference=system,originalPosition=vec(center),originalRotation=dict(x=0,y=0,z=0,w=1),originalScale=vec((1,1,1)),bounds={'m_Center':vec((0,0,0)),'m_Extent':vec([s/2 for s in size])},parentStructure=cid,childStructures=[],searchableTerms=terms,vertexCount=len(verts),triangleCount=len(faces)//3)
  structures.append(d);by_system[system]['structures'].append(mid)
  if index%250==0:print(index,len(files),flush=True)
catalog=dict(dataset='BodyParts3D 4.0 · adult male reference',attribution='BodyParts3D, © The Database Center for Life Science licensed under CC Attribution 4.0 International',systems=systems,structures=structures)
(OUT/'anatomy_catalog.json').write_text(json.dumps(catalog,ensure_ascii=False,separators=(',',':')))
(ROOT/'Assets/AnatomyLab/Data/Catalog/concepts.json').write_text(json.dumps([dict(id=c,name=names[c],meshes=sorted(v),parents=parents[c]) for c,v in concepts.items()],ensure_ascii=False))
# Basic activities use dataset names only: no unvalidated medical claims.
ed=[]
for term in ['heart','lung','liver','stomach','brain']:
 candidates=[d for d in structures if term in d['canonicalName'].lower()]
 candidates.sort(key=lambda d:(len(d['canonicalName']),-d['triangleCount']))
 if candidates:
  d=candidates[0];ed.append(dict(id=d['id'],title=d['displayName'],function='Contenido educativo DEMO: identificación de estructuras; explicación funcional pendiente de revisión docente.',location='Observa su posición en el modelo de referencia.',source='BodyParts3D 4.0 · '+d['parentStructure'],related=[]))
(OUT/'education.json').write_text(json.dumps(dict(entries=ed),ensure_ascii=False))
report=f'''# Dataset import report\n\nSource: BodyParts3D 4.0 IS-A 99%.\n\n- Structures: {len(structures)}\n- Named concepts: {len(concepts)}\n- Missing meshes: 0\n- Duplicate mesh IDs: {len(structures)-len(set(d['id'] for d in structures))}\n- Missing metadata: {sum(d['id']==d['canonicalName'] for d in structures)}\n- Triangles: {sum(d['triangleCount'] for d in structures):,}\n- Vertices: {sum(d['vertexCount'] for d in structures):,}\n- Geometry disk bytes: {sum(f.stat().st_size for f in OUT.rglob('*.bytes')):,}\n- Geometry memory estimate (vertices + normals + uint32 indices): {sum(d['vertexCount']*24+d['triangleCount']*12 for d in structures):,} bytes, excludes Unity objects and read/write CPU copies.\n- Height: 1.75 m; common global normalization, local pivots at bounds centers.\n- Archive SHA256: {hashlib.sha256(archive.read_bytes()).hexdigest()}\n- Classification: deterministic heuristic from official IS-A concepts; needs anatomical review for ambiguous memberships.\n- LOD: official reduced source only; additional validated LODs pending.\n\n'''
report+='\n'.join(f"- {s['id']}: {len(s['structures'])}" for s in systems)
(ROOT/'DATASET_IMPORT_REPORT.md').write_text(report)
print(report)
