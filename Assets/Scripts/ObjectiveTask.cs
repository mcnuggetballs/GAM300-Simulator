using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveTask : MonoBehaviour
{
    [SerializeField]
    public string id;
    [SerializeField]
    public TextMeshProUGUI textUI;
    [SerializeField]
    public GameObject checkMark;
    [SerializeField]
    public GameObject strikeTop;
    [SerializeField]
    public GameObject strikeMid;
    [SerializeField]
    public GameObject strikeBot;
    public bool complete = false;

    public void CreateObjective(string id,string objText)
    {
        this.id = id;
        textUI.text = objText;
    }
    public void SetCompleteTrue()
    {
        complete = true;
        Destroy(gameObject);
    }
    public void CompleteTask()
    {
        GetComponent<Animator>().SetTrigger("Complete");
    }
}
