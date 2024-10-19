using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Newtonsoft.Json;

public class DestroyBehaviour : Singleton<DestroyBehaviour>
{

    static string url = "https://api.openai.com/v1/embeddings";
    static string openaiApiKey = "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";
    Dictionary<string, List<double>> scripts_embed;
    List<double> request_embed;
    Dictionary<string, double> similarity_dict;

    IEnumerator FindScriptsEmbeddings(string script)
    {
        var uwr = new UnityWebRequest(url, "POST");
        string json = $"{{\"input\": \"{script}\", \"model\": \"text-embedding-ada-002\"}}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", $"Bearer {openaiApiKey}");
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            string output_json_text = uwr.downloadHandler.text;
            EmbeddingData embeddingData = JsonConvert.DeserializeObject<EmbeddingData>(output_json_text);
            List<double> embedding = embeddingData.Data[0].Embedding;

            Debug.Log("Received: " + output_json_text);
            if (scripts_embed == null) scripts_embed = new Dictionary<string, List<double>>();
            scripts_embed.Add(script, embedding);
            Debug.Log("script embedded");
        }
    }

    IEnumerator FindRequestEmbedding(string script)
    {
        var uwr = new UnityWebRequest(url, "POST");
        string json = $"{{\"input\": \"{script}\", \"model\": \"text-embedding-ada-002\"}}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", $"Bearer {openaiApiKey}");
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            string output_json_text = uwr.downloadHandler.text;
            EmbeddingData embeddingData = JsonConvert.DeserializeObject<EmbeddingData>(output_json_text);
            List<double> embedding = embeddingData.Data[0].Embedding;

            Debug.Log("Received: " + output_json_text);

            request_embed = embedding;
            Debug.Log("request embedded");
        }
    }

    IEnumerator FindSimilarScript(GameObject obj, string request)
    {
        var scripts = obj.GetComponents<MonoBehaviour>();
        List<string> scriptNames = new List<string>();
        Dictionary<string, MonoBehaviour> scriptDict = new Dictionary<string, MonoBehaviour>();
        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i] != null && scripts[i].GetType().Name.Length > 0)
            {
                var name = scripts[i].GetType().Name;
                scriptNames.Add(scripts[i].GetType().Name);
                scriptDict.Add(name, scripts[i]);
            }
        }
        // find embeddings of both scripts and request
        for (int i = 0; i < scriptNames.Count; i++)
        {
            yield return FindScriptsEmbeddings(scriptNames[i]);
        }
        yield return FindRequestEmbedding(request);


        // for each script embed, calculate cos similarity with the request embed.
        // Append to dictionary with script name keys, similarity values
        for (int i = 0; i < scripts_embed.Count; i++)
        {
            double similarity = Agent.Instance.GetCosineSimilarity(scripts_embed.Values.ElementAt(i), request_embed);
            if (similarity_dict == null) similarity_dict = new Dictionary<string, double>();
            similarity_dict.Add(scripts_embed.Keys.ElementAt(i), similarity);
        }

        // Sort list, choose request with highest similarity
        var top_similar_script = similarity_dict.OrderByDescending(pair => pair.Value).Take(1)
            .ToDictionary(pair => pair.Key, pair => pair.Value).Keys.First();


        Debug.Log($"To Destroy = {top_similar_script}, obj = {obj.name}");

        // Destroy that component
        Destroy(scriptDict[top_similar_script]);

        // script = dict key with the highest cos embedding
    }

    public void DestroyScriptObj(GameObject obj, string request)
    {
        StartCoroutine(FindSimilarScript(obj, request));
        Debug.Log("Destroyed");

    }


    public static void DestroyScript(GameObject obj, string request)
    {
        Instance.DestroyScriptObj(obj, request);
    }


}
