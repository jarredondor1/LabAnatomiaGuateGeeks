using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Performance {
public sealed class PerformanceHUD:MonoBehaviour {
 public LabRuntime Lab;public Text Label;float elapsed;int frames; public float FPS;
 void Update(){if(!Debug.isDebugBuild||!Lab.Ready){Label.enabled=false;return;}frames++;elapsed+=Time.unscaledDeltaTime;if(elapsed<1)return;FPS=frames/elapsed;frames=0;elapsed=0;int visible=0,triangles=0,vertices=0;foreach(var s in Lab.Streaming.Loaded.Values)if(s.Object.activeInHierarchy){visible++;triangles+=s.Data.triangleCount;vertices+=s.Data.vertexCount;}Label.text=$"DEV  {FPS:0} FPS | {visible}/{Lab.Streaming.Loaded.Count} piezas\n{triangles:N0} tris · {vertices:N0} verts · {Lab.Extracted.Count} grabs\nMemoria reservada {Profiler.GetTotalReservedMemoryLong()/1048576} MB";}
}
}
