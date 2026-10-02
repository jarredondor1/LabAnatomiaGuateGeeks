"""Optional: blender -b --python generate_lod.py -- input.obj output.obj 0.5
Experimental LOD; never replaces source. Requires anatomical visual approval.
"""
import sys
from pathlib import Path
import bpy
args=sys.argv[sys.argv.index('--')+1:]
if len(args)!=3:raise SystemExit('Expected input.obj output.obj ratio')
source,target,ratio=args;ratio=float(ratio)
if not 0.05<=ratio<=1:raise SystemExit('Ratio must be 0.05..1')
if Path(source).resolve()==Path(target).resolve():raise SystemExit('Refusing to overwrite source')
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
bpy.ops.wm.obj_import(filepath=str(Path(source).resolve()))
for obj in bpy.context.scene.objects:
 if obj.type!='MESH':continue
 before=len(obj.data.polygons);bpy.context.view_layer.objects.active=obj
 mod=obj.modifiers.new('Experimental LOD','DECIMATE');mod.ratio=ratio
 bpy.ops.object.modifier_apply(modifier=mod.name)
 print(obj.name,'faces before=',before,'after=',len(obj.data.polygons))
bpy.ops.wm.obj_export(filepath=str(Path(target).resolve()),export_selected_objects=False)
print('WARNING: generated LOD needs anatomical review; not auto-imported.')
