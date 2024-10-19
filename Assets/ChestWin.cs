using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestWin : MonoBehaviour
{
    public GameObject winMessage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            winMessage.SetActive(true);
        }
    }
}
