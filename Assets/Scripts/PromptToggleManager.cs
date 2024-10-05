using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptToggleManager : MonoBehaviour
{
    [SerializeField]
    GameObject promptGameObject = null;
    static PromptToggleManager _instance;
    Vector3 originalScale = Vector3.one;
    public static PromptToggleManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance= this;
    }
    public void SetPrompt(GameObject theGameObject)
    {
        promptGameObject = theGameObject;
        originalScale = promptGameObject.transform.localScale;
    }

    private void Update()
    {
        if (GameManager.Instance.GetPromptsDisabled())
        {
            if (promptGameObject != null)
            {
                if (MiniTutorial.Instance)
                {
                    MiniTutorial.Instance.ignoreEverything = true;
                }
                promptGameObject.GetComponent<Animator>().enabled = false;
                promptGameObject.transform.localScale = Vector3.zero;
            }
        }
        else
        {
            if (promptGameObject != null)
            {
                if (MiniTutorial.Instance)
                {
                    MiniTutorial.Instance.ignoreEverything = false;
                }
                promptGameObject.GetComponent<Animator>().enabled = true;
                promptGameObject.transform.localScale = originalScale;
            }
        }
    }
}
