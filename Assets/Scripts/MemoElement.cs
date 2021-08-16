using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MemoElement : MonoBehaviour
{
    public TMP_Text memoNumber;
    public TMP_Text memoText;

    public void NewMemoElement(int i, string text)
    {
        memoNumber.text = i.ToString();
        memoText.text = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
