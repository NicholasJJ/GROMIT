using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInput : InputSource
{
    [SerializeField] private TMP_InputField inputField;
    public override void EndRecord()
    {
        Debug.Log("end Text entry!");
        Timer.Instance.request = inputField.text;
        requestObject.SetMessage(inputField.text);
        inputField.DeactivateInputField();
        inputField.gameObject.SetActive(false);
        requestObject.CloseChannel();
    }

    protected override void SetupRecord()
    {
        Debug.Log("text entry!");
        inputField.gameObject.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
    }
}
