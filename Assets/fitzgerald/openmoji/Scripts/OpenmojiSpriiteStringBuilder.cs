using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenmojiSpriiteStringBuilder : MonoBehaviour
{
    private static OpenmojiSpriiteStringBuilder _inst;
    public static OpenmojiSpriiteStringBuilder inst {
        get { return _inst; }
    }

    public List<TextAsset> openmojiJSON = new List<TextAsset>();
    public Dictionary<string, string> emojiToName = new Dictionary<string, string>();
    public Dictionary<string, string> hexCodeToName = new Dictionary<string, string>();

    void Awake() {
        foreach (var jsonFile in openmojiJSON) {
            var spritemapData = JsonUtility.FromJson<OpenmojiImporter.OpenmojiSpritemapFormat>(jsonFile.text);
            foreach (var emoji in spritemapData.emojis) {
                emojiToName.Add(emoji.emoji.emoji, emoji.emoji.annotation);
                hexCodeToName.Add(emoji.emoji.hexcode, emoji.emoji.annotation);
                //Debug.Log(emoji.emoji.hexcode);
            }
        }
        _inst = this;
    }

    public string GetEmojiName(string emoji) {
        if (emojiToName.ContainsKey(emoji)) {
            return emojiToName[emoji];
        }
        return "";
    }

    public string GetEmojiSprite(string emoji) {
        return GetEmojiNameSprite(GetEmojiName(emoji));
    }

    public string GetEmojiNameSprite(string name) {
        return $"<sprite name=\"{name}\">";
    }

    public string GetEmojiHex(string hex)
    {
        name = "";
        if (hexCodeToName.ContainsKey(hex)) name = hexCodeToName[hex];
        return GetEmojiNameSprite(name);
    }
}
