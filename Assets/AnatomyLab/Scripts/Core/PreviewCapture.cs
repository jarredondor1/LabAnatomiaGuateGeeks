using System.Collections;
using System.IO;
using UnityEngine;
namespace GuateGeeks.AnatomyLab.Core {
public sealed class PreviewCapture:MonoBehaviour {
 IEnumerator Start(){var lab=GetComponent<LabRuntime>();while(!lab.Ready)yield return null;lab.Systems.ResetVisibility();while(lab.Streaming.Loading.Count>0)yield return null;yield return null;
 var cam=lab.ViewCamera;var target=new RenderTexture(1800,1200,24);cam.targetTexture=target;cam.Render();RenderTexture.active=target;var image=new Texture2D(1800,1200,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1800,1200),0,0);image.Apply();Directory.CreateDirectory("Documentation/Previews");File.WriteAllBytes("Documentation/Previews/laboratory.png",image.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;Destroy(image);Destroy(target);Debug.Log("[AnatomyLab] Preview captured");
#if UNITY_EDITOR
 if(Application.isBatchMode)UnityEditor.EditorApplication.Exit(0);
#endif
 }
}
}
