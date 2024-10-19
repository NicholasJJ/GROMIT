using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(OutputManager))]
public class OutputTextManager : OutputHandler
{
    [SerializeField] public TextMeshProUGUI textBox;
    [SerializeField] string noText;
    [SerializeField] float readTime = 10;
    private int textTracker = 0;

    private new void Start() {
        base.Start();
        textBox.text = "";
    }
    public override void HandleOutput(Output output)
    {
        //Debug.LogWarning(output.text);
        StartCoroutine(DisplayTextForTime(output.text));
        //if (output.text == null || output.text.Equals("")) {
        //    textBox.text = noText;
        //} else {
        //    textBox.text = output.text ?? noText;
        //}
    }

    public void DisplayText(string text) => StartCoroutine(DisplayTextForTime(text));

    public string CurrentText() => textBox.text;

    private IEnumerator DisplayTextForTime(string text)
    {
        textTracker++;
        var curr = textTracker;
        if (text == null || text.Equals(""))
        {
            textBox.text = noText;
        }
        else
        {
            textBox.text = text ?? noText;
        }
        yield return new WaitForSeconds(10);
        if (curr == textTracker)
            textBox.text = noText;
    }
}
