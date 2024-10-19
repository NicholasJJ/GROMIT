using System;
using System.Collections;
using System.Collections.Generic;
using ChatGPTWrapper;
using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json;


public class EmbeddingData
{
    [JsonProperty("data")]
    public List<EmbeddingItem> Data { get; set; }
    
    [JsonProperty("model")]
    public string Model { get; set; }
    
    [JsonProperty("object")]
    public string Object { get; set; }
    
    [JsonProperty("usage")]
    public Usage Usage { get; set; }
}

public class EmbeddingItem
{
    [JsonProperty("embedding")]
    public List<double> Embedding { get; set; }
    
    [JsonProperty("index")]
    public int Index { get; set; }
    
    [JsonProperty("object")]
    public string Object { get; set; }
}

public class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }
    
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}

public class Request_Info {
    public string request;
    public List<double> embedding;

    public Request_Info(string request, List<double> embedding) {
        this.request = request;
        this.embedding = embedding;
    }
}

public class Request_Similarity {
    public string request;
    public double similarity;

    public Request_Similarity(string request, double similarity) {
        this.request = request;
        this.similarity = similarity;
    }
}
// For Descending Order
class Request_SimilarityComparer : IComparer<Request_Similarity>
{
    public int Compare(Request_Similarity x, Request_Similarity y)
    {
        // Define your custom comparison logic here
        return y.similarity.CompareTo(x.similarity);
    }
}


public class FixedLengthQueue<T>
{
    public T[] array;
    private int size;
    private int head;
    private int tail;

    public FixedLengthQueue(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than 0");

        array = new T[capacity];
        size = 0;
        head = 0;
        tail = 0;
    }

    public int Count => size;

    public void Enqueue(T item)
    {
        if (size == array.Length)
        {
            Dequeue();
        }

        array[tail] = item;
        tail = (tail + 1) % array.Length;
        size++;
    }

    public T Dequeue()
    {
        if (size == 0)
            throw new InvalidOperationException("Queue is empty");

        T item = array[head];
        head = (head + 1) % array.Length;
        size--;

        return item;
    }
}



public class Agent : Singleton<Agent>
{
    public UnityStringEvent sendToGPT;

    [SerializeField] private static string wson = "I'm going to give you a file in a new format called WSON. WSON is similar to json, but is used to describe the placement of items in a virtual world. Each node has a name and a list of child nodes. A node being the child of another node means it is “contained in” or is “part of” the parent node. For example, the WSON string “[{name : House, children : [{ name : Wall, children : []}, { name : Wall1, children : []}, { name : Wall2, children : []}, { name : Wall3, children : []}]}, {name: apple, children : []} ]”represents a house with 4 walls, and an apple outside the house. If a question can't be answered using only the information in the WSON, just try your best";
    private static string stateQuery1 = "Consider the following WSON: ";
    private static string stateQuery2 = "\n given the WSON, ";

    private static string request_history_statement = "\n Here is a list of previous requests that could be useful in helping you with your task: \n";

    // private List<string> requestList = new List<string>();

    private static int max_history = 15;

    public FixedLengthQueue<Request_Info> requestQueue = new FixedLengthQueue<Request_Info>(max_history);
    // private List<List<double>> embeddingList = new List<List<double>>();

    string embeddingUrl = "https://api.openai.com/v1/embeddings";

    string openaiApiKey = "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";

    List<double> curr_embedding = null;

    //
    public int top_k = 3;
    public IComparer<Request_Similarity> request_comparer = new Request_SimilarityComparer();

        
    // Store the question in our memory of previous questions
    public void AgentProcessRequest(InputRequest request, string input_wson) {

        StartCoroutine(Process(request, input_wson));
        
    }

    IEnumerator Process(InputRequest request, string input_wson)
    {
        string request_string = request.ToJson();
        string jsonPayload = $"{{\"input\": \"{request_string}\", \"model\": \"text-embedding-ada-002\"}}";
        yield return postRequest(embeddingUrl, jsonPayload);
        // List<double> curr_embedding = GetEmbedding(request_string);

        // Find the most similar requests in our memory

        string request_history = getSimilarRequestHistory(request_string, curr_embedding);
        //The agent is handling the memory, so reset the chat history each time
        CustomGPT.Instance.ResetChat();
        string complete_request = AskQuestion(input_wson, request_string, request_history);
        
        List<double> copy_emb = new List<double>(curr_embedding);
        Request_Info request_info = new Request_Info(request_string, copy_emb);

        requestQueue.Enqueue(request_info);
        Debug.Log("Iterations: " + requestQueue.Count);
        Debug.Log("Complete Request:" + complete_request);
    }

    private string getSimilarRequestHistory(string request_string, List<double> curr_embedding) {
        // Takes a request string and returns the top_k most similar request strings in our memory, appened to make a prompt

        List<Request_Similarity> request_similarity_list = new List<Request_Similarity>();
        // iterate over the previous requests and add to array of similary scores
        
        for (int i = 0; i < requestQueue.Count; i++) {
            Request_Info request_info = requestQueue.array[i];
            double similarity = GetCosineSimilarity(request_info.embedding, curr_embedding);
            Request_Similarity request_similarity = new Request_Similarity(request_info.request, similarity);
            request_similarity_list.Add(request_similarity);
        }
        request_similarity_list.Sort(request_comparer);

        int iterations = Math.Min(request_similarity_list.Count, top_k);
        string request_history = "";
        
        for (int i = 0; i < iterations; i++) {
                Request_Similarity item = request_similarity_list[i];
                request_history += item.request + "\n";
            // Process the first 'k' elements
        }

        return request_history_statement + request_history;

    }

    private string AskQuestion(string input_wson, string question, string request_history) {
        Debug.Log("WSON:\n" + input_wson);
        string q = stateQuery1 + input_wson+ stateQuery2 + question + request_history;
        //sendToGPT.Invoke(q);
        CustomGPT.Instance.SendToChatGPTAsSystem(q);
        return q;
    }

    //public List<double> GetEmbedding(string text) {

    //    string jsonPayload = $"{{\"input\": \"{text}\", \"model\": \"text-embedding-ada-002\"}}";
    //    StartCoroutine(postRequest(embeddingUrl, jsonPayload));
        
    //    Debug.Log("Finished Coroutine");
    //    return curr_embedding_list;

    //}

    public double GetCosineSimilarity(List<double> V1, List<double> V2)
        {
            int N = 0;
            N = ((V2.Count < V1.Count) ? V2.Count : V1.Count);
            double dot = 0.0d;
            double mag1 = 0.0d;
            double mag2 = 0.0d;
            for (int n = 0; n < N; n++)
            {
                dot += V1[n] * V2[n];
                mag1 += Math.Pow(V1[n], 2);
                mag2 += Math.Pow(V2[n], 2);
            }

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }



    IEnumerator postRequest(string url, string json)
    {
     var uwr = new UnityWebRequest(url, "POST");
     byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
     uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
     uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
     uwr.SetRequestHeader("Content-Type", "application/json");
     uwr.SetRequestHeader("Authorization", $"Bearer {openaiApiKey}");

     //Send the request then wait here until it returns
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
        
        curr_embedding = embedding;
     }
 }
}
