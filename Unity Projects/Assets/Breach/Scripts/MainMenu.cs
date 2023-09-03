using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{

    public static MainMenu Instance;
    public TMP_Text debugText;
    public TMP_Text settingsText;
    public GameObject loadingCanvas;
    // public GameObject creditsPanel;
    // public bool creditsVisible = false;

     // canvas properties 
    private string sceneLoad = "";   
    private float loadProgress = 0f;
    private string settingsString = "(none)";
    private string settingsLoadString = "(none)";

    // players' postions for emergency scene
    public Vector3 randomPos;
    public Vector3 simplePos;
    public Vector3 frontPos;
    public Vector3 inHousePos;
    
    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // progress loading 
    int loadingDots = 0;
    bool isLoading = false;
    float totalTime = 0f;
    int totalLoadingSeconds = 0;

    // Update is called once per frame
    void Update()
    {

        if (isLoading == true)
        {
            totalTime += Time.deltaTime;
            if (totalTime > 1f)
            {
                totalTime = 0f;
                loadingDots++;
                totalLoadingSeconds++;
            }
            if (loadingDots > 3)
            {
                loadingDots = 1;
            }
            debugText.text = "Loading";
            for (int i = 0; i < loadingDots; i++)
            {
                debugText.text += ".";
            }
            debugText.text += "\n" + ((loadProgress) * 100f/0.9f).ToString("0.00") + "%";
            //debugText.text += "\n" + totalLoadingSeconds + " seconds...";
        }
        
    }
  
 

    public void LaunchPrepScene()
    {
        //debugText.text = "Loading...";
        //SceneManager.LoadScene("XR_Demo_01_20200509");
        sceneLoad = "Preparedness";
        settingsLoadString = "Preparedness Scene";
        UpdateSettingsText();
    }
    public void LaunchEmerScene()
    {
        sceneLoad = "Emergency";
        settingsLoadString = "Emergency Scene";

        UpdateSettingsText();
    }

    public void UpdateSettingsText()
    {
        settingsText.text = "" + settingsLoadString + ",\n" ;
    }

  /*  public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        creditsVisible = true;
    }
*/
    public void Launch()
    {
        debugText.text = "Processing...";
        //SceneManager.LoadScene(sceneLoad);
        isLoading = true;
        loadingCanvas.SetActive(true);
        QualitySettings.asyncUploadTimeSlice = 32;
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            loadProgress = asyncLoad.progress;
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
                yield return null;
        }
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        // TODO: replace with go-to start menu in final application
        //Application.Quit();
        SceneManager.LoadScene("MainMenu");
    }

    public void FieldVisit()
    {
        sceneLoad = "Gjerdrum_pre";
        settingsLoadString = "Field Visit Scene";
        UpdateSettingsText();
        debugText.fontSize = settingsText.fontSize;
        debugText.text = "Processing...";
        isLoading = true;
        loadingCanvas.SetActive(true);
        QualitySettings.asyncUploadTimeSlice = 32;
        StartCoroutine(LoadAsyncScene());
    }
    /*
    //random position 
    public void LoadRandom()
    {
        PlayerPositionHolder.isRandom = true;
        PlayerPositionHolder.randomPos = randomPos;
        sceneLoad = "Emergency";
    }
    
    //Simple position 
    public void LoadSimple()
    {
        PlayerPositionHolder.isSimple = true;
        PlayerPositionHolder.simplePos = simplePos;
        sceneLoad = "Emergency";
    }

    //front position 
    public void LoadFront()
    {
        PlayerPositionHolder.isFront = true;
        PlayerPositionHolder.frontPos = frontPos;
        sceneLoad = "Emergency";
    }

    public void LoadInHouse()
    {
        PlayerPositionHolder.isInHouse = true;
        PlayerPositionHolder.inHousePos = inHousePos;
        sceneLoad = "Emergency";
    }
*/

}
