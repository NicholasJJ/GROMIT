using System;
using System.Collections.Generic;

namespace ChatGPTWrapper {
    // Due to OpenAI's new chat completions api, this replaces the old "Prompt" class, but the prompt class is still used for the older models.
    public class CChat
    {
        private string _initialPrompt;
        private  List<Message> _currentChat = new List<Message>();

        public CChat(string initialPrompt, string[] exampleConversation) {
            _initialPrompt = initialPrompt;
            Message systemMessage = new Message("system", initialPrompt);
            _currentChat.Add(systemMessage);
            for (int i = 0; i < exampleConversation.Length; i+=2) {
                AppendMessage(Speaker.System,exampleConversation[i]);
                AppendMessage(Speaker.ChatGPT,exampleConversation[i+1]);
            }
        }
        public List<Message> CurrentChat { get { return _currentChat; } }

        public enum Speaker {
            User,
            ChatGPT,
            System
        }

        public void AppendMessage(Speaker speaker, string text)
        {
        
            switch (speaker)
            {
                case Speaker.User:
                    Message userMessage = new Message("user", text);
                    _currentChat.Add(userMessage);
                    break;
                case Speaker.ChatGPT:
                    Message chatGPTMessage = new Message("assistant", text);
                    _currentChat.Add(chatGPTMessage);
                    break;
                case Speaker.System:
                    Message systemMessage = new Message("system", text);
                    _currentChat.Add(systemMessage);
                    break;
            }
        }
    }
}

