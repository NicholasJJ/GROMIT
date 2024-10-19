using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


[Serializable]
public class CombinerObjectData
{
    public string display;
    public string description;
    public string code;

    public override string ToString() => JsonUtility.ToJson(this);
}

public abstract class CombinerObject : MonoBehaviour
{
    public CombinerObjectData data;
    protected Transform player;
    protected GameObject particleSys;

    public void Setup(CombinerObjectData data)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        particleSys = (GameObject) Resources.Load("Particle System");
        this.data = data;
        GetComponentInChildren<TextMeshProUGUI>().text = OpenmojiSpriiteStringBuilder.inst.GetEmojiHex(data.display);
        //OpenmojiSpriiteStringBuilder.inst.getem
    }

    public void Use()
    {
        Debug.Log($"using item {data.description}");
        Action();
    }

    public abstract void Action();

}
