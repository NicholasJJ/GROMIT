using UnityEngine;
using System.Collections.Generic;
using Reqs;
using System;
using System.Linq;
using UnityEngine.Events;

namespace ChatGPTWrapper {

    public class CustomGPT : Singleton<CustomGPT>
    {
        private int max_tokens_overall = 4097;
        [SerializeField]
        private bool _useProxy = false;
        [SerializeField]
        private string _proxyUri = null;

        [SerializeField]
        private string _apiKey = null;

        public enum Model {
            ChatGPT,
            Davinci,
            Curie
        }

        [SerializeField] public bool gpt4;

        [SerializeField]
        public Model _model = Model.ChatGPT;
        private string _selectedModel = null;
        [SerializeField]
        private int _maxTokens = 500;
        [SerializeField]
        private float _temperature = 0.5f;
        
        private string _uri;
        private List<(string, string)> _reqHeaders;
        

        private Requests requests = new Requests();
        private Prompt _prompt;
        private CChat _chat;
        private string _lastUserMsg;
        private string _lastChatGPTMsg;

        [SerializeField]
        private string _chatbotName = "ChatGPT";

        [TextArea(4,50)]
        [SerializeField]
        private string _initialPrompt = "You are ChatGPT, a large language model trained by OpenAI.";

        [SerializeField] private List<string> _exampleConversation = new List<string>();

        public UnityStringEvent chatGPTResponse = new UnityStringEvent();
        public UnityEvent onChatGPTRequestSent;
        public UnityEvent onChatGPTResponseGet;

        private Action<string> responseManager;

        private float timer=0;

        public enum Speaker {
            User,
            ChatGPT,
            System
        }

        private void OnEnable()
        {
            
            TextAsset textAsset = Resources.Load<TextAsset>("APIKEY");
            if (textAsset != null) {
                _apiKey = textAsset.text;
            }
            
            
            
            _reqHeaders = new List<(string, string)>
            { 
                ("Authorization", $"Bearer {_apiKey}"),
                ("Content-Type", "application/json")
            };
            switch (_model) {
                case Model.ChatGPT:
                    _chat = new CChat(_initialPrompt,_exampleConversation.ToArray());
                    _uri = "https://api.openai.com/v1/chat/completions";
                    //_selectedModel = gpt4 ? "gpt-4-1106-preview" : "gpt-3.5-turbo"; //change this to gpt-4o
                    _selectedModel = gpt4 ? "gpt-4o" : "gpt-3.5-turbo";
                    break;
                case Model.Davinci:
                    _prompt = new Prompt(_chatbotName, _initialPrompt);
                    _uri = "https://api.openai.com/v1/completions";
                    _selectedModel = "text-davinci-003";
                    break;
                case Model.Curie:
                    _prompt = new Prompt(_chatbotName, _initialPrompt);
                    _uri = "https://api.openai.com/v1/completions";
                    _selectedModel = "text-curie-001";
                    break;
            }
        }

        public void ResetChat(string initialPrompt = null, List<string> priorConversation = null) {
            switch (_model) {
                case Model.ChatGPT:
                    _chat = new CChat((initialPrompt == null) ? _initialPrompt : initialPrompt,
                        (priorConversation == null) ?  _exampleConversation.ToArray() : priorConversation.ToArray());
                    break;
                default:
                    _prompt = new Prompt(_chatbotName, _initialPrompt);
                    break;
            }
        }

        public void SendToChatGPTAsSystem(string message, Action<string> responseManager = null) {
            this.responseManager = responseManager;
            SendToChatGPT(message,"system");
        }

        public void SendToChatGPT(string message, string from = "user")
        {
            _lastUserMsg = message;
            onChatGPTRequestSent.Invoke();
            if (_model == Model.ChatGPT) {
                if (_useProxy) {
                    ProxyReq proxyReq = new ProxyReq
                    {
                        max_tokens = _maxTokens,
                        temperature = _temperature,
                        messages = new List<Message>(_chat.CurrentChat),
                    };
                    proxyReq.messages.Add(new Message(from, message));

                    string proxyJson = JsonUtility.ToJson(proxyReq);
                    Debug.Log($"proxyJson= {proxyJson}");

                    StartCoroutine(requests.PostReq<ChatGPTRes>(_proxyUri, proxyJson, ResolveChatGPT, _reqHeaders));
                } else {
                    ChatGPTReq chatGPTReq = new ChatGPTReq
                    {
                        model = _selectedModel,
                        max_tokens = _maxTokens,
                        temperature = _temperature,
                        messages = _chat.CurrentChat.Take(30).ToList(),
                    };
                    chatGPTReq.messages.Add(new Message(from, message));
            
                    string chatGPTJson = JsonUtility.ToJson(chatGPTReq);
                    Debug.Log($"Old Json= {chatGPTJson}");
                    chatGPTJson = chatGPTJson.Substring(0, chatGPTJson.Length - 1) + ",\"response_format\": {\"type\": \"json_object\"}}";
                    Debug.Log($"Json= {chatGPTJson}");
                    Debug.Log("about to send message: " + message + "\n prior chat is: \n" + GetFullContext());
                    timer = Time.time;
                    Timer.Instance.Begin();
                    StartCoroutine(requests.PostReq<ChatGPTRes>(_uri, chatGPTJson, ResolveChatGPT, _reqHeaders));
                }
                
            } else {

                _prompt.AppendText(Prompt.Speaker.User, message);

                GPTReq reqObj = new GPTReq
                {
                    model = _selectedModel,
                    prompt = _prompt.CurrentPrompt,
                    max_tokens = _maxTokens,
                    temperature = _temperature
                };
                string json = JsonUtility.ToJson(reqObj);

                StartCoroutine(requests.PostReq<GPTRes>(_uri, json, ResolveGPT, _reqHeaders));
            }
        }

        private void ResolveChatGPT(ChatGPTRes res)
        {
            onChatGPTResponseGet.Invoke();
            Debug.Log($"TIMER got response in {Time.time-timer} with char length {res.choices[0].message.content.Length}: {res.choices[0].message.content}");
            Timer.Instance.lastOutput = res.choices[0].message.content;
            Timer.Instance.lastOutputLen = res.choices[0].message.content.Length;
            _lastChatGPTMsg = res.choices[0].message.content;
            //_chat.AppendMessage(CChat.Speaker.User, _lastUserMsg);
            _chat.AppendMessage(CChat.Speaker.ChatGPT, _lastChatGPTMsg);
            if (responseManager == null)
            {
                chatGPTResponse.Invoke(_lastChatGPTMsg);
            }
            else
            {
                responseManager.Invoke(_lastChatGPTMsg);
            }
        }

        private void ResolveGPT(GPTRes res)
        {
            onChatGPTResponseGet.Invoke();
            _lastChatGPTMsg = res.choices[0].text
                .TrimStart('\n')
                .Replace("<|im_end|>", "");

            _prompt.AppendText(Prompt.Speaker.Bot, _lastChatGPTMsg);
            chatGPTResponse.Invoke(_lastChatGPTMsg);
        }

        public void appendMessages(string call, string response)
        {
            _chat.AppendMessage(CChat.Speaker.User, call);
            _chat.AppendMessage(CChat.Speaker.ChatGPT, response);
        }

        public string GetFullContext()
        {
            return string.Join("\n\n", _chat.CurrentChat.Take(30).ToList().Select(m => $"{m.role}: {m.content}"));
        }

        public string GetStartingPrompt() => _chat.CurrentChat[0].content;
    }
}
