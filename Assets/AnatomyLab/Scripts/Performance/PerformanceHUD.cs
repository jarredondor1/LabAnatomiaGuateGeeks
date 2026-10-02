using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Unity.Profiling;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Performance {
public sealed class PerformanceHUD:MonoBehaviour {
 public LabRuntime Lab;public Text Label;float elapsed;int frames; public float FPS;
 ProfilerRecorder draws,batches,setPass;
 void OnEnable(){if(Debug.isDebugBuild){draws=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Draw Calls Count");batches=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Batches Count");setPass=ProfilerRecorder.StartNew(ProfilerCategory.Render,"SetPass Calls Count");}}
 void OnDisable(){draws.Dispose();batches.Dispose();setPass.Dispose();}
 void Update(){if(!Debug.isDebugBuild||!Lab.Ready){Label.enabled=false;return;}frames++;elapsed+=Time.unscaledDeltaTime;if(elapsed<1)return;FPS=frames/elapsed;frames=0;elapsed=0;int visible=0,triangles=0,vertices=0;foreach(var s in Lab.Streaming.Loaded.Values)if(s.Object.activeInHierarchy){visible++;triangles+=s.Data.triangleCount;vertices+=s.Data.vertexCount;}Label.text=$"DEV  {FPS:0} FPS | {visible}/{Lab.Streaming.Loaded.Count} piezas\n{triangles:N0} tris · {vertices:N0} verts · {Lab.Extracted.Count} grabs\nMemoria {Profiler.GetTotalReservedMemoryLong()/1048576} MB · Draw {draws.LastValue} / Batch {batches.LastValue} / SetPass {setPass.LastValue}";}
}
}
