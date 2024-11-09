using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FlashCardActivateEvent : UnityEvent { };
public class FlashCardCollider : MonoBehaviour
{
    public List<Sprite> displayImages;
    [SerializeField]
    FlashCardActivateEvent activateEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FlashCardDisplay.Instance.Activate(displayImages);
            activateEvent.Invoke();
            Destroy(gameObject);
        }
    }

    public void ActivateFlashCard()
    {
        FlashCardDisplay.Instance.Activate(displayImages);
    }
}
