using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashCardCollider : MonoBehaviour
{
    public List<Sprite> displayImages;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FlashCardDisplay.Instance.Activate(displayImages);
            Destroy(gameObject);
        }
    }

    public void ActivateFlashCard()
    {
        FlashCardDisplay.Instance.Activate(displayImages);
    }
}
