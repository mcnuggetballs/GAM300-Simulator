using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverUI : MonoBehaviour
{
    public static HoverUI _instance;

    public TextMeshProUGUI TextComponent;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowUI(string message)
    {
        gameObject.SetActive(true);
        TextComponent.text = message;
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        TextComponent.text = string.Empty;
    }
}
