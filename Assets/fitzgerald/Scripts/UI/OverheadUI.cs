using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadUI : MonoBehaviour
{
    public Canvas canvas;
    public GameObject damageTextPrefab;

    public void AddDamageText(float damage, GameObject causer)
    {
        var damageText = Instantiate(damageTextPrefab, canvas.transform.position, canvas.transform.rotation, canvas.transform);
        damageText.GetComponent<TMPro.TMP_Text>().text = damage.ToString();
    }
}
