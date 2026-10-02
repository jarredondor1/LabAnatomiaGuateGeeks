using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using GuateGeeks.AnatomyLab.Core;
namespace GuateGeeks.AnatomyLab.Performance {
public sealed class PerformanceScenario:MonoBehaviour {
 public LabRuntime Lab;public float Duration=10;
 IEnumerator Start(){while(!Lab.Ready)yield return null;while(Lab.Streaming.Loading.Count>0)yield return null;Lab.Systems.ShowAll();while(Lab.Streaming.Loading.Count>0)yield return null;
 var report=new StringBuilder("visible,loaded,triangles,vertices,frames,seconds,fps,reservedMemoryBytes,device\n");
 foreach(int count in new[]{100,500,1000,Lab.Catalog.structures.Length}){int i=0,tris=0,verts=0;foreach(var s in Lab.Streaming.Loaded.Values){bool show=i++<count;s.Object.SetActive(show);if(show){tris+=s.Data.triangleCount;verts+=s.Data.vertexCount;}}yield return new WaitForSeconds(2);int frames=0;float start=Time.realtimeSinceStartup;while(Time.realtimeSinceStartup-start<Duration){frames++;yield return null;}float seconds=Time.realtimeSinceStartup-start;report.AppendLine($"{count},{Lab.Streaming.Loaded.Count},{tris},{verts},{frames},{seconds},{frames/seconds},{Profiler.GetTotalReservedMemoryLong()},{SystemInfo.deviceModel}");}
 string path=Path.Combine(Application.persistentDataPath,"performance.csv");File.WriteAllText(path,report.ToString());Lab.Status="Performance CSV: "+path;Debug.Log("[Performance] "+report);Lab.RefreshAll();
 }
}
}
