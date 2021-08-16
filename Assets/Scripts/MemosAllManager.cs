using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MemosAllManager : MonoBehaviour
{
    public static MemosAllManager instance;


    public GameObject memoPanel;
    public Transform memoContent;

    //public GameObject startUI;
    //public GameObject dataUI;

    void Start()
    {
        /*AuthManager.authManagerSingleton.LoadMemoData();*/
        StartProcess();

    }

    private void Awake()
    {
        /*if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }*/
    }
    public void StartProcess()
    {
        int memoListCount = AuthManager.authManagerSingleton.memoListCount;
        //Debug.Log(memoListCount);
        //Debug.Log(AuthManager.authManagerSingleton.memoListCount);


        for (int i = 1; i <= memoListCount; i++)
        {
            GameObject memoPanelContent = Instantiate(memoPanel, memoContent);
            memoPanelContent.GetComponent<MemoElement>().NewMemoElement(i, AuthManager.authManagerSingleton.memoList[i - 1]);
            Debug.Log(i);
            Debug.Log(AuthManager.authManagerSingleton.memoList[i - 1]);
        }



        //dataUI.SetActive(true);
        //startUI.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
