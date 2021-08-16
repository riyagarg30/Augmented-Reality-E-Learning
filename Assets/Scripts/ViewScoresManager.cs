using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewScoresManager : MonoBehaviour
{
    public static MemosAllManager instance;

    public GameObject scorePanel;
    public Transform scoreContent;



    // Start is called before the first frame update
    void Start()
    {
        int quizListCount = AuthManager.authManagerSingleton.quizListCount;
        quizListCount *= 10;
        //Debug.Log(memoListCount);
        //Debug.Log(AuthManager.authManagerSingleton.memoListCount);


        for (int i = 1; i <= quizListCount; i=i+10)
        {
            GameObject scorePanelContent = Instantiate(scorePanel, scoreContent);
            int quiz_number = (i / 10) + 1;
            scorePanelContent.GetComponent<ScoreElement>().NewScoreElement(quiz_number.ToString(),AuthManager.authManagerSingleton.quizList[i - 1], AuthManager.authManagerSingleton.quizList[i], AuthManager.authManagerSingleton.quizList[i+1], AuthManager.authManagerSingleton.quizList[i + 2], AuthManager.authManagerSingleton.quizList[i + 3], AuthManager.authManagerSingleton.quizList[i + 4], AuthManager.authManagerSingleton.quizList[i + 5], AuthManager.authManagerSingleton.quizList[i + 6], AuthManager.authManagerSingleton.quizList[i + 7], AuthManager.authManagerSingleton.quizList[i + 8]);
            Debug.Log(i);
            Debug.Log(AuthManager.authManagerSingleton.quizList[i - 1]);
        }

        //AuthManager.authManagerSingleton.clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
