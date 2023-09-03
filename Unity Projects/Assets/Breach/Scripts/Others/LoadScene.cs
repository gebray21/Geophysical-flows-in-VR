using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadScene : MonoBehaviour
{
    // a method to lad a scene using its name as an input
    public Image progressBar;
    public FadeScreen fadeScreen;
    public float ScreenFadeTime = 0.5f;
    public bool UseSceenFader = true;
    void Start()
    {
        fadeScreen.DoFadeOut();
    }

    public void LoadNextScene(string sceneName)
    {      
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    IEnumerator LoadAsyncScene(string sceneLoad)
    {
       

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneLoad);
        asyncLoad.allowSceneActivation = false;
       

        while (!asyncLoad.isDone)
        {
           
           if(progressBar!=null) 
                progressBar.fillAmount = asyncLoad.progress*(1/0.9f);
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    IEnumerator LoadAsyncSceneFade(string sceneLoad)
    {
        fadeScreen.DoFadeOut();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneLoad);
        asyncLoad.allowSceneActivation = false;
        float timer = 0;
        
        while (timer <=fadeScreen.fadeDuration && !asyncLoad.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
            
            }
        asyncLoad.allowSceneActivation = true;
    }
    

    public void LoadSingleScene(string singleScene)
    {
        StartCoroutine(LoadSingleSceneRoutine(singleScene));
        
    }
    IEnumerator LoadSingleSceneRoutine(string singleScene)
    {
            fadeScreen.DoFadeOut();
            yield return new WaitForSeconds(fadeScreen.fadeDuration);
        
        SceneManager.LoadScene(singleScene);
    }

    public void QuitApp()
    {
        Application.Quit(); 
    }

    
}
