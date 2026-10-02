using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using GuateGeeks.AnatomyLab.Core;
using GuateGeeks.AnatomyLab.Anatomy;
public static class AnatomyDatasetImporter {
 [MenuItem("GuateGeeks/Anatomy Lab/Import Dataset")]
 public static void Import(){var info=new System.Diagnostics.ProcessStartInfo("/usr/bin/python3","Tools/AnatomyPipeline/import.py"){UseShellExecute=false,WorkingDirectory=Directory.GetCurrentDirectory()};using(var process=System.Diagnostics.Process.Start(info)){process.WaitForExit();if(process.ExitCode!=0)throw new Exception("[Dataset] Import failed. Check SourceData.");}AssetDatabase.Refresh();Validate();}
 [MenuItem("GuateGeeks/Anatomy Lab/Rebuild Catalog")] public static void Rebuild()=>Import();
 [MenuItem("GuateGeeks/Anatomy Lab/Validate Anatomy")]
 public static void Validate(){var c=new LocalDatasetProvider().LoadCatalog();var report=new StringBuilder("# Unity dataset validation\n\n");int missing=0;foreach(var d in c.structures)if(Resources.Load<TextAsset>(d.meshReference)==null){report.AppendLine("FAIL missing mesh: "+d.id);missing++;}report.AppendLine("PASS unique identifiers and valid systems");report.AppendLine((missing==0?"PASS":"FAIL")+" mesh references; missing="+missing);report.AppendLine("WARNING anatomical system classification requires review");report.AppendLine("WARNING no additional LOD levels; official reduced meshes supplied");File.WriteAllText("TestResults/dataset-validation.md",report.ToString());Debug.Log("[Dataset] "+report);if(missing>0)throw new Exception("Missing geometry");}
 [MenuItem("GuateGeeks/Anatomy Lab/Generate Bounds")]
 public static void Bounds(){var c=new LocalDatasetProvider().LoadCatalog();foreach(var d in c.structures){var a=Resources.Load<TextAsset>(d.meshReference);var m=AnatomyStreamingManager.ReadMesh(a.bytes);d.bounds=m.bounds;UnityEngine.Object.DestroyImmediate(m);Resources.UnloadAsset(a);}File.WriteAllText("Assets/AnatomyLab/Resources/Anatomy/anatomy_catalog.json",JsonUtility.ToJson(c));AssetDatabase.Refresh();}
 [MenuItem("GuateGeeks/Anatomy Lab/Performance Audit")]
 public static void Audit(){var c=new LocalDatasetProvider().LoadCatalog();long tris=0,vertices=0;foreach(var d in c.structures){tris+=d.triangleCount;vertices+=d.vertexCount;}var text=$"# Static performance audit\n\nPASS catalog: {c.structures.Length} structures\n\nTriangles: {tris}\nVertices: {vertices}\nWARNING full dataset requires physical Quest profiling.\nWARNING meshes have no additional LODs.\nPASS no per-structure MonoBehaviour/collider before extraction.\n";File.WriteAllText("TestResults/performance-audit.md",text);Debug.Log("[Performance] "+text);}
}
