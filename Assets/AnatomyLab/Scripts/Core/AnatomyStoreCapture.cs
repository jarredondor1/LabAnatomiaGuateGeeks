#if UNITY_EDITOR
using System.Collections;
using System.IO;
using GuateGeeks.AnatomyLab.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class AnatomyStoreCapture : MonoBehaviour
{
    const string DirectoryPath = "Documentation/Store";
    static AnatomyStoreCapture current;

    [MenuItem("GuateGeeks/Anatomy Lab/Capturar imágenes para Meta")]
    public static void Run()
    {
        if (EditorApplication.isPlaying) return;
        PlayerPrefs.SetString("anatomy.reference", "male");
        PlayerPrefs.Save();
        EditorSceneManager.OpenScene("Assets/AnatomyLab/Scenes/02_AnatomyLab.unity");
        new GameObject("Captura de tienda").AddComponent<AnatomyStoreCapture>();
        EditorApplication.isPlaying = true;
    }

    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        if (current != this) yield break;
        Directory.CreateDirectory(DirectoryPath);
        var lab = GetLab();
        yield return WaitForReady(lab);
        yield return Capture(lab, "01-vista-masculina.png");

        lab.SelectById("FJ2564");
        yield return WaitForSelection(lab);
        yield return Capture(lab, "02-estomago.png");
        lab.ExtractSelected();
        yield return new WaitForSeconds(.5f);
        yield return Capture(lab, "03-pieza-extraida.png");

        lab.SwitchReference("female");
        yield return new WaitUntil(() => (lab = GetLab()) != null && lab.Ready && lab.ReferenceId == "female");
        yield return WaitForReady(lab);
        yield return Capture(lab, "04-vista-femenina.png");
        lab.SelectById("HRAF0473");
        yield return WaitForSelection(lab);
        yield return Capture(lab, "05-utero.png");
        PlayerPrefs.SetString("anatomy.reference", "male");
        PlayerPrefs.Save();
        Debug.Log("[AnatomyLab] Cinco imágenes de tienda capturadas en " + DirectoryPath);
        EditorApplication.isPlaying = false;
    }

    static LabRuntime GetLab() => FindAnyObjectByType<LabRuntime>();

    static IEnumerator WaitForReady(LabRuntime lab)
    {
        yield return new WaitUntil(() => lab != null && lab.Ready && lab.Streaming != null && lab.Streaming.Loading.Count == 0);
        yield return new WaitForSeconds(1);
    }

    static IEnumerator WaitForSelection(LabRuntime lab)
    {
        yield return new WaitUntil(() => lab != null && lab.Selection.Selected != null && lab.Streaming.Loading.Count == 0);
        yield return new WaitForSeconds(.5f);
    }

    static IEnumerator Capture(LabRuntime lab, string filename)
    {
        yield return new WaitForEndOfFrame();
        var camera = lab.ViewCamera;
        var target = new RenderTexture(2560, 1440, 24, RenderTextureFormat.ARGB32);
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            var image = new Texture2D(2560, 1440, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 2560, 1440), 0, 0);
            image.Apply();
            File.WriteAllBytes(Path.Combine(DirectoryPath, filename), image.EncodeToPNG());
            Destroy(image);
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            Destroy(target);
        }
    }
}
#endif
