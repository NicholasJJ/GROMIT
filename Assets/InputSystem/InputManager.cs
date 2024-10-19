using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using Whisper;
using UnityEngine.Events;

public class InputRequest {
    private string message;
    private List<(float,string)> gestures;
    private List<(float,TextDescription)> pointingAt;
    private int channels;

    public InputRequest(int channels) {
        message = null;
        gestures =  new List<(float, string)>();
        pointingAt = new List<(float, TextDescription)>();
        this.channels = channels;
    }

    public void SetMessage(WhisperResult message) {
        this.message = message.Result;
    }

    public void SetMessage(string message)
    {
        this.message = message;
    }

    public void AppendGesture(string gesture,float t) {
        this.gestures.Add((t,gesture));
    }

    public void PointAt(TextDescription description,float t) {
        pointingAt.Add((t,description));
    }

    public void CloseChannel() {
        channels -= 1;
        if (channels == 0) {
            GM.Instance.ProcessRequest(this);
        }
    }

    public string ToJson() {
        string json = message;
        Debug.Log("pointing at length = " + pointingAt.Count);
        if (pointingAt.Count > 0) {
            List<int> indicies = new List<int>();
            foreach (string word in new string[]{"this","that","there"}) {
                indicies.AddRange(IndiciesOfWord(message,word));
            }
            if (indicies.Count == pointingAt.Count)
            {
                indicies.Sort();
                int index = 0;
                int offset = 0;
                Debug.Log("idicies= " + indicies.ToString());
                while (index < indicies.Count && index < pointingAt.Count) {
                    string statement = $" *points at {pointingAt[index].Item2.GetName()}* ";
                    Debug.Log("inserting into: " + indicies[index] + offset);
                    json = json.Insert(indicies[index] + offset,statement);
                    offset += statement.Length;
                    index++;
                }
            }
            else
            {
                string mid = string.Join(", then ",pointingAt.Select(x => x.Item2.GetName()));
                json += $" *points at {mid}*";
            }
        }
        if (gestures.Count > 0) {
            json += $" *{string.Join(", ",gestures)}*";
        }
        return json;
    }

    private List<int> IndiciesOfWord(string message, string word) {
        List<int> indicies = new List<int>();
        string lm = message.ToLower();
        string lw = word.ToLower();
        int index = 0;
        while (index != -1) {
            index = lm.IndexOf(lw,index);
            if (index != -1) {
                indicies.Add(index + word.Length);
                index++;
            }
        }
        return indicies;
    }
}

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private HashSet<InputSource> sources = new HashSet<InputSource>();
    [SerializeField] private KeyCode micButton;
    [SerializeField] private KeyCode textButton;
    [SerializeField] private bool useMic;
    private bool isRecording;
    private InputRequest currentRequest;
    public UnityEvent onRecordStart;
    public UnityEvent onRecordStop;

    private void Start() {
        isRecording = false;
    }

    public void AddSource(InputSource source) {
        sources.Add(source);
    }

    // Update is called once per frame
    void Update()
    {
        if (useMic)
        {
            if (Input.GetKeyDown(micButton) && !isRecording)
            {
                currentRequest = new InputRequest(sources.Count);
                foreach (InputSource s in sources)
                {
                    s.StartRecord(currentRequest);
                }
                onRecordStart.Invoke();
                isRecording = true;
            }
            else if (Input.GetKeyUp(micButton) && isRecording)
            {
                foreach (InputSource s in sources)
                {
                    s.EndRecord();
                }
                onRecordStop.Invoke();
                isRecording = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(textButton) && !isRecording)
            {
                currentRequest = new InputRequest(sources.Count);
                foreach (InputSource s in sources)
                {
                    s.StartRecord(currentRequest);
                }
                onRecordStart.Invoke();
                isRecording = true;
            }
            else if (Input.GetKeyDown(textButton) && isRecording)
            {
                foreach (InputSource s in sources)
                {
                    s.EndRecord();
                }
                //Timer.Instance.Begin();
                onRecordStop.Invoke();
                isRecording = false;
            }
        }
        
    }

    public bool IsRecording() => isRecording;
}