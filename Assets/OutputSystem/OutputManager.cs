using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Purchasing.MiniJSON;

[Serializable]
public class Output {
    public string text;
    public string code;
    public string objectName;
}

public class OutputManager : Singleton<OutputManager>
{
    private HashSet<OutputHandler> outputHandlers = new HashSet<OutputHandler>();

    [TextArea(4,15)]
    [SerializeField] string testOutput;
    [SerializeField] KeyCode testButton;

    private void Update() {
        if (Input.GetKeyDown(testButton) && testOutput != "") {
            ProcessResponse(testOutput);
        }
    }

    public void AddHandler(OutputHandler handler) {
        outputHandlers.Add(handler);
    }

    public void ProcessResponse(string response) {
        Debug.Log("response: \n" + response);
        Output output = JsonUtility.FromJson<Output>(ReplaceCurvedQuotes(response));
        Debug.Log("parsedOutput: " + output.code);
        Debug.Log("line count: " + output.code.Count(c => c == '\n'));
        Debug.Log("handlerCount= " + outputHandlers.Count);
        foreach (OutputHandler outputHandler in outputHandlers) {
            outputHandler.HandleOutput(output);
        }
    }

    public static string ReplaceCurvedQuotes(string input)
    {
        string result = input.Replace("“", "\"").Replace("”", "\"");
        return result;
    }
}
