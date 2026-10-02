using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using GuateGeeks.AnatomyLab.Core;
public static class AnatomyPreview {
 [MenuItem("GuateGeeks/Anatomy Lab/Generate Preview")]
 public static void Run(){EditorSceneManager.OpenScene("Assets/AnatomyLab/Scenes/02_AnatomyLab.unity");var lab=Object.FindAnyObjectByType<LabRuntime>();lab.gameObject.AddComponent<PreviewCapture>();EditorApplication.isPlaying=true;}
}
