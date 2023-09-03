using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    [SerializeField] int ScoreToAdd = 10;
    private ScoreManager scoreManager;
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
       // if (scoreManager == null)
         //   Debug.LogError("The score manager script not available");
    }

    public void CollectScore()
    {
        if (scoreManager != null)
        {
           
            scoreManager.UpdateScore(20);
            Destroy(this.gameObject);

        }
    }
  
    void Update()
    {
        
    }
}
