using Unity.VisualScripting;
using UnityEngine;

public class AppFlowDriver : MonoBehaviour
{
    public static AppFlowDriver I { get; private set; }

    [Header("Scene Names")]
    public string titleScene = "Title";
    public string tutorialScene = "Tutorial";
    public string campScene = "Hub_Camp";
    public string battleScene = "Battle_Map01";
    public string devScene = "DevScene"; // 开发阶段

    [Header("Player")]
    public bool keepPlayerBetweenScenes = true;
    public GameObject playerPrefab; 

    public AppFlow appFlow { get; private set; }

    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return; 
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        appFlow = new AppFlow
        {
            titleScene = titleScene,
            tutorialScene = tutorialScene,
            campScene = campScene,
            battleScene = battleScene,

            keepPlayerBetweenScenes = keepPlayerBetweenScenes,
            playerPrefab = playerPrefab
        };
    }

    void Start()
    {
        StartCoroutine(appFlow.GoToDevScene());
    }

    // 给UI/传送点调用的便携方法
    public void GoToTitle  (string spawn="Default") => StartCoroutine(appFlow.GoToTitle(spawn));
    public void GoToTutorial(string spawn="Default") => StartCoroutine(appFlow.GoToTutorial(spawn));
    public void GoToCamp    (string spawn="Default") => StartCoroutine(appFlow.GoToCamp(spawn));
    public void GoToBattle  (string spawn="Default") => StartCoroutine(appFlow.GoToBattle(spawn));

    public void GoToDevScene(string spawn="Default") => StartCoroutine(appFlow.GoToDevScene(spawn));  // 开发阶段
}
