using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ChatGPTWrapper;

public class GM : Singleton<GM>
{
    
    // Start is called before the first frame update
    void Start()
    {
        // CreateStateCheck(manualStateDesc, success);
    }

    public void ProcessRequest(InputRequest request) {
        Agent.Instance.AgentProcessRequest(request, TextTree.Instance.ToJson());
    }

    public void ProcessRequestRelated(InputRequest request)
    {
        List<string> keywords = ExtractKeywordsFromRequest(request);
        string relatedJson = TextTree.Instance.ToJsonRelated(keywords);
        Agent.Instance.AgentProcessRequest(request, relatedJson);
    }


    public List<string> ExtractKeywordsFromRequest(InputRequest request)
    {
        List<string> keywords = new List<string>();
        string request_string = request.ToJson();
        string[] words = request_string.Split(' ');

        foreach (var word in words)
        {
            
            string normalizedWord = word.ToLower();

            if (TextTree.Instance.GetAllTextNodeNames().Contains(normalizedWord))
            {
                keywords.Add(normalizedWord);
            }
        }
        return keywords;
    }



    public void ProcessResponse(string response) {
        Debug.Log("response: \n" + response);
        // if (response.Equals("YES")) onSuccessAction();

    }

    // public void CreateStateCheck(string description, Action onSuccess) {
    //     onSuccessAction = onSuccess;
    //     stateQuestion = description;
    // }
    public void onStateMet() {

    }
}
