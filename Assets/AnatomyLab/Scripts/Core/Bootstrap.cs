using UnityEngine;
using UnityEngine.SceneManagement;
namespace GuateGeeks.AnatomyLab.Core {
public sealed class Bootstrap:MonoBehaviour {void Start(){QualitySettings.SetQualityLevel(1,true);Application.targetFrameRate=72;new LocalDatasetProvider().LoadCatalog();SceneManager.LoadScene("02_AnatomyLab");}}
}
