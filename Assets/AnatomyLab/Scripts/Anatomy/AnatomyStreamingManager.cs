using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Anatomy {
public sealed class AnatomyStreamingManager : MonoBehaviour {
 public readonly Dictionary<string,StructureState> Loaded=new Dictionary<string,StructureState>();
 public readonly HashSet<string> Loading=new HashSet<string>();
 public Transform Body; public AnatomyCatalog Catalog; public Material Opaque,Transparent;
 public Func<string,bool> IsSystemDesired;
 public event Action<StructureState> Added; public event Action<string> Removed;
 readonly Dictionary<string,int> generations=new Dictionary<string,int>();
 readonly Dictionary<string,List<AnatomyStructure>> groups=new Dictionary<string,List<AnatomyStructure>>();
 public void Initialize(AnatomyCatalog catalog,Transform body,Material opaque,Material transparent){Catalog=catalog;Body=body;Opaque=opaque;Transparent=transparent; foreach(var d in catalog.structures){if(!groups.ContainsKey(d.systemId))groups[d.systemId]=new List<AnatomyStructure>();groups[d.systemId].Add(d);}}
 public IEnumerator LoadSystemAsync(string id) {
 if(Loading.Contains(id))yield break; Loading.Add(id); int generation=generations.ContainsKey(id)?generations[id]:0;
 if(!groups.TryGetValue(id,out var entries)){Loading.Remove(id);yield break;}
 int batch=0;
 foreach(var d in entries) {
 if(generations.TryGetValue(id,out var current)&&current!=generation)break;
 if(Loaded.ContainsKey(d.id))continue;
 var request=Resources.LoadAsync<TextAsset>(d.meshReference);yield return request;
 if(generations.TryGetValue(id,out current)&&current!=generation){if(request.asset!=null)Resources.UnloadAsset(request.asset);break;}
 var bytes=request.asset as TextAsset;
 if(bytes==null){Debug.LogError("[Dataset] Missing mesh "+d.meshReference);continue;}
 var mesh=ReadMesh(bytes.bytes); mesh.UploadMeshData(true); Resources.UnloadAsset(bytes);
 var go=new GameObject(d.id);go.transform.SetParent(Body,false);go.transform.localPosition=d.originalPosition;go.transform.localRotation=d.originalRotation;go.transform.localScale=d.originalScale;
 go.AddComponent<MeshFilter>().sharedMesh=mesh;var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=Opaque;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
 var state=new StructureState(d){Object=go,Mesh=mesh,Renderer=renderer}; Loaded.Add(d.id,state); Added?.Invoke(state);
 if(++batch%8==0)yield return null;
 } Loading.Remove(id);
 }
 public IEnumerator UnloadSystemAsync(string id) {
 generations[id]=(generations.ContainsKey(id)?generations[id]:0)+1;
 // Wait for cancelled loader before allowing another request for the same group.
 while(Loading.Contains(id))yield return null;
 if(IsSystemDesired!=null&&IsSystemDesired(id)){yield return LoadSystemAsync(id);yield break;}
 if(groups.TryGetValue(id,out var entries))foreach(var d in entries)if(Loaded.TryGetValue(d.id,out var state)){Removed?.Invoke(d.id);Destroy(state.Object);Destroy(state.Mesh);Loaded.Remove(d.id);}
 yield return Resources.UnloadUnusedAssets();
 }
 public static Mesh ReadMesh(byte[] bytes) {
 using(var r=new BinaryReader(new MemoryStream(bytes))) {
 int nv=r.ReadInt32(),nt=r.ReadInt32();if(nv<=0||nt<=0||nv>2000000||nt>10000000)throw new InvalidDataException("Invalid geometry header");
 var v=new Vector3[nv];var t=new int[nt*3];for(int i=0;i<nv;i++)v[i]=new Vector3(r.ReadSingle(),r.ReadSingle(),r.ReadSingle());for(int i=0;i<t.Length;i++)t[i]=r.ReadInt32();
 var m=new Mesh{name="BodyParts3D",indexFormat=nv>65535?IndexFormat.UInt32:IndexFormat.UInt16};m.vertices=v;m.triangles=t;m.RecalculateNormals();m.RecalculateBounds();return m;
 }
 }
 void OnDestroy(){foreach(var state in Loaded.Values)if(state.Mesh!=null)Destroy(state.Mesh);}
}
}
