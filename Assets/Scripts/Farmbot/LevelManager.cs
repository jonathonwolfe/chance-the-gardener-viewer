using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instanace;

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _ProgressBar;
    private float target;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instanace == null)
        {
            Instanace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        _ProgressBar.value = 0;
        target = 0;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }

    private void Update()
    {
        _ProgressBar.value = Mathf.MoveTowards(_ProgressBar.value, target, 3 * Time.deltaTime);
    }
}
