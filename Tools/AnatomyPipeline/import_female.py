#!/usr/bin/env python3
"""Importación reproducible del GLB oficial HRA femenino v1.5. Requiere numpy.
Conserva las 888 mallas, sin deformación ni mezcla con la referencia masculina.
"""
import json, struct, hashlib, re, collections
from pathlib import Path
import numpy as np
ROOT=Path(__file__).resolve().parents[2]
SOURCE=ROOT/'SourceData/3d-vh-f-united-v1.5.glb'
OUT=ROOT/'Assets/AnatomyLab/Resources/Anatomy/Female'
raw=SOURCE.read_bytes()
magic,version,length=struct.unpack_from('<III',raw)
assert magic==0x46546c67 and version==2 and length==len(raw)
json_len,json_type=struct.unpack_from('<II',raw,12)
assert json_type==0x4e4f534a
model=json.loads(raw[20:20+json_len]); bin_len,bin_type=struct.unpack_from('<II',raw,20+json_len)
assert bin_type==0x004e4942
binary=memoryview(raw)[28+json_len:28+json_len+bin_len]
assert not model.get('extensionsRequired')
# This pinned release has no node transforms; fail rather than silently misplace future data.
assert not any(any(k in n for k in ('matrix','translation','rotation','scale')) for n in model['nodes'])
def read(index):
 a=model['accessors'][index];v=model['bufferViews'][a['bufferView']]
 assert 'sparse' not in a and v.get('buffer',0)==0
 dtype=np.dtype({5121:'u1',5123:'<u2',5125:'<u4',5126:'<f4'}[a['componentType']]);width={'SCALAR':1,'VEC3':3}[a['type']]
 return np.ndarray((a['count'],width),dtype=dtype,buffer=binary,offset=v.get('byteOffset',0)+a.get('byteOffset',0),strides=(v.get('byteStride',width*dtype.itemsize),dtype.itemsize)).copy()
def vec(v):return dict(zip(('x','y','z'),map(float,v)))
spec=[('integumentary','Piel y mamas','bc8f9b',True),('reproductive','Reproductivo','df739f',True),('urinary','Urinario','ac81c1',True),('digestive','Digestivo','d29e6b',True),('skeleton','Óseo (parcial)','ead7ad',True),('cardiovascular','Corazón','dc6d78',True),('respiratory','Respiratorio','85c2d2',True),('nervous','Nervioso','e9c05d',False),('muscular','Muscular (parcial)','b75565',False),('arterial','Arterias','cf5460',False),('venous','Venas','568bc5',False),('lymphatic','Linfático','81b995',False),('pregnancy','Embarazo (referencia)','d9aa70',False)]
systems=[dict(id=i,name=n,displayName=n,defaultColor=dict(zip(('r','g','b','a'),[int(c[j:j+2],16)/255 for j in (0,2,4)]+[1])),defaultVisibility=vis,structures=[]) for i,n,c,vis in spec]
sys_map={s['id']:s for s in systems}; parents={}
for i,n in enumerate(model['nodes']):
 for child in n.get('children',[]):parents[child]=i
def ancestors(i):
 result=[]
 while i in parents:i=parents[i];result.append(model['nodes'][i]['name'])
 return result
# Names curated from source node identity, retaining original labels and ontology IDs for search.
names={
 'skin':'Piel del cuerpo','vagina':'Vagina','cervicovaginal_junction':'Unión cervicovaginal',
 'uterovesical_pouch':'Fondo de saco vesicouterino','broad_ligament':'Ligamento ancho del útero',
 'left_ovary':'Ovario izquierdo','right_ovary':'Ovario derecho','abdominal_ostium_of_uterine_tube':'Orificio abdominal de la trompa uterina',
 'body_of_uterus':'Cuerpo del útero','fundus_of_uterus':'Fondo del útero','cornua':'Cuernos uterinos',
 'lower_uterine_segment':'Segmento uterino inferior','posterior_wall_of_uterus':'Pared posterior del útero','anterior_wall_of_uterus':'Pared anterior del útero',
 'cervix':'Cuello uterino','internal_cervical_os':'Orificio cervical interno','external_cervical_os':'Orificio cervical externo',
 'basal_plate':'Placa basal de la placenta','chorionic_plate':'Placa coriónica','placenta_vessels':'Vasos de la placenta','amnion':'Amnios','umbilical_cord':'Cordón umbilical','umbilical_artery_1':'Arteria umbilical 1','umbilical_artery_2':'Arteria umbilical 2','umbilical_vein':'Vena umbilical',
 'sacrum':'Sacro','coccyx':'Cóccix','stomach':'Estómago','liver':'Hígado','spleen':'Bazo','urinary_bladder':'Vejiga urinaria'}
paired={'ampulla_of_uterine_tube':('Ampolla de la trompa uterina','f'),'isthmus_of_fallopian_tube':('Istmo de la trompa uterina','f'),'fibria_of_uterine_tube':('Fimbrias de la trompa uterina','f'),'uterine_tube_infundibulum':('Infundíbulo de la trompa uterina','f'),'suspensory_ligament_of_ovary':('Ligamento suspensorio del ovario','m'),'ovarian_ligament':('Ligamento propio del ovario','m'),'mesosalpinx':('Mesosálpinx','m'),'mesovarium':('Mesovario','m'),'fat':('Tejido adiposo de la mama','f'),'areolar_tubercles':('Tubérculos de la aréola','f'),'nipple':('Pezón','m'),'areola':('Aréola','f'),'mammary_lobes':('Lóbulos de la mama','f'),'main_lactiferous_ducts':('Conductos lactíferos de la mama','f'),'main_lactiferous_sinuses':('Senos lactíferos de la mama','f'),'suspensory_ligaments':('Ligamentos suspensorios de la mama','f'),'femur':('Fémur','m'),'kidney':('Riñón','m'),'lung':('Pulmón','m')}
for base,(es,gender) in paired.items():
 for suffix,side in [('L','izquierd'),('R','derech')]:names[base+'_'+suffix]=es+' '+side+('a' if gender=='f' else 'o')
for en,es in [('round_ligament_of_uterus','Ligamento redondo del útero'),('uterosacral_ligament','Ligamento uterosacro'),('cardinal_ligament_of_uterus','Ligamento cardinal del útero')]:
 for side,span in [('left','izquierdo'),('right','derecho')]:names[side+'_'+en]=es+' '+span
for bone,span in [('pubis','pubis'),('ilium','ilion'),('ischium','isquion')]:
 for tissue,tr in [('compact','compacto'),('spongy','esponjoso')]:
  for side,sr in [('L','izquierdo'),('R','derecho')]:names[f'{bone}_{tissue}_bone_{side}']=f'Tejido {tr} del {span} {sr}'
# Preserve source height/proportions; only move feet to the stage and convert glTF handedness.
skin=read(model['meshes'][0]['primitives'][0]['attributes']['POSITION'])
origin=(skin.min(axis=0)+skin.max(axis=0))/2;origin[1]=skin[:,1].min()
structures=[]; source_triangles=0
for i,node in enumerate(model['nodes']):
 if 'mesh' not in node:continue
 chain=' '.join(ancestors(i));node_name=node['name'];short=node_name.removeprefix('VH_F_');extra=node.get('extras',{});canonical=extra.get('label','')
 if canonical in ('','-'):canonical=short.replace('_',' ')
 if 'VH_F_placenta' in chain:sid='pregnancy'
 elif 'VH_F_circulatory_system' in chain:
  sid='venous' if any(t in (node_name+' '+chain).lower() for t in ('vein','venous')) else 'cardiovascular' if 'heart' in (node_name+' '+chain).lower() else 'arterial'
 else:
  sid=next((s for n,s in [('integumentary','integumentary'),('reproductive','reproductive'),('urinary','urinary'),('digestive','digestive'),('skeletal','skeleton'),('respiratory','respiratory'),('nervous','nervous'),('muscular','muscular'),('lymphatic','lymphatic')] if f'VH_F_{n}_system' in chain),None)
 assert sid is not None,node_name
 verts=[];indices=[];offset=0
 for primitive in model['meshes'][node['mesh']]['primitives']:
  assert primitive.get('mode',4)==4
  v=read(primitive['attributes']['POSITION']).astype(np.float64);t=read(primitive['indices']).reshape(-1,3).astype(np.int32)
  assert np.isfinite(v).all() and t.min()>=0 and t.max()<len(v)
  v-=origin;v[:,2]*=-1;t=t[:,[0,2,1]]
  verts.append(v);indices.append(t+offset);offset+=len(v)
 v=np.concatenate(verts);t=np.concatenate(indices);source_triangles+=len(t)
 # Weld only identical coordinates. No decimation or estimated anatomy.
 v,remap=np.unique(v,axis=0,return_inverse=True);t=remap[t].astype('<i4')
 lo=v.min(axis=0);hi=v.max(axis=0);center=(lo+hi)/2
 ident=f'HRAF{i:04d}';folder=OUT/'Geometry'/sid;folder.mkdir(parents=True,exist_ok=True)
 (folder/(ident+'.bytes')).write_bytes(struct.pack('<ii',len(v),len(t))+(v-center).astype('<f4').tobytes()+t.tobytes())
 display=names.get(short,canonical);localized=short in names
 terms=[node_name,canonical,extra.get('ontologyid','')]+[a.removeprefix('VH_F_').replace('_',' ') for a in ancestors(i)]
 if sid=='reproductive':
  if 'uter' in node_name or 'cervi' in node_name or 'cornua' in node_name:terms+=['útero','utero']
  if 'ovary' in node_name or 'ovari' in node_name:terms+=['ovario','ovarios']
  if 'tube' in node_name:terms+=['trompa uterina','trompas de falopio']
 if 'mammary' in chain:terms+=['mama','mamas','glándula mamaria']
 d=dict(id=ident,canonicalName=canonical,displayName=display,localized=localized,defaultTransparent=short=='skin',latinName='',systemId=sid,category='HRA femenino v1.5',description='Estructura de la referencia femenina HRA. Explora su posición y relación con las piezas cercanas.',meshReference=f'Anatomy/Female/Geometry/{sid}/{ident}',materialReference=sid,originalPosition=vec(center),originalRotation=dict(x=0,y=0,z=0,w=1),originalScale=vec((1,1,1)),bounds={'m_Center':vec((0,0,0)),'m_Extent':vec((hi-lo)/2)},parentStructure=extra.get('ontologyid','') or node_name,childStructures=[],synonyms=[],searchableTerms=terms,vertexCount=len(v),triangleCount=len(t))
 structures.append(d);sys_map[sid]['structures'].append(ident)
assert len(structures)==888 and source_triangles==5557599
attribution='Kristen Browne y Heidi Schlehlein · Human Reference Atlas / HuBMAP · referencia femenina v1.5 (2023) · CC BY 4.0 · doi:10.48539/HBM352.BTSQ.586'
catalog=dict(dataset='HRA femenino v1.5',attribution=attribution,systems=systems,structures=structures)
(OUT/'anatomy_catalog.json').write_text(json.dumps(catalog,ensure_ascii=False,separators=(',',':')))
entries=[]
for node_index in (473,469,470,479,431,435,443):
 d=next(d for d in structures if d['id']==f'HRAF{node_index:04d}')
 entries.append(dict(id=d['id'],title=d['displayName'],function='Identificación anatómica: '+d['displayName']+'. Selecciona, aísla o extrae la pieza para explorar su forma.',location='Referencia femenina HRA; conserva la posición relativa del conjunto original.',source=attribution,related=[]))
(OUT/'education.json').write_text(json.dumps(dict(entries=entries),ensure_ascii=False))
report=dict(source='https://cdn.humanatlas.io/digital-objects/ref-organ/united-female/v1.5/assets/3d-vh-f-united.glb',sha256=hashlib.sha256(raw).hexdigest(),license='https://creativecommons.org/licenses/by/4.0/',attribution=attribution,meshes=len(structures),triangles=source_triangles,vertices=sum(d['vertexCount'] for d in structures),sourceOrigin=vec(origin),heightMeters=float(skin[:,1].max()-skin[:,1].min()),adaptations=['Conversión glTF a Unity: reflexión del eje Z y orden de caras invertido','Traslación común a la base del escenario, pivotes individuales y soldadura de vértices coincidentes','Sin simplificación, deformación ni mezcla con anatomía masculina','Colores por sistema y nombres de interfaz en español; etiquetas originales conservadas para búsqueda'],coverage='Superficie corporal y órganos seleccionados. Esqueleto y musculatura parciales. Capa de embarazo independiente, oculta al inicio.',systems={s['id']:len(s['structures']) for s in systems})
(ROOT/'Tools/AnatomyPipeline/female-import-report.json').write_text(json.dumps(report,ensure_ascii=False,indent=2))
print(json.dumps(report,ensure_ascii=False,indent=2))
