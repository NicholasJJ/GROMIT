using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Linq;

public class OpenmojiImporter : EditorWindow
{
    [System.Serializable]
    public class OpenmojiSpritemapFormat {
        [System.Serializable]
        public class EmojiFormat {
            public string emoji;
            public string hexcode;
            public string group;
            public string subgroups;
            public string annotation;
            public string tags;
            public string openmoji_tags;
            public string openmoji_author;
            public string openmoji_date;
            public string skintone;
            public string skintone_combination;
            public string skintone_base_emoji;
            public string skintone_base_hexcode;
            public float unicode;
            public int order;
        }

        [System.Serializable]
        public class EmojiDataFormat {
            public EmojiFormat emoji;
            public int top;
            public int left;
            public int index;
        }

        public string name;
        public int columns;
        public int rows;
        public int height;
        public int width;
        public int emojiSize;
        public List<EmojiDataFormat> emojis = new List<EmojiDataFormat>();
    }

    [MenuItem("Window/Openmoji/Import Openmoji JSON")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(OpenmojiImporter), false, "Openmoji Spritemap Importer");
    }

    private TextAsset textAsset;
    private Texture spriteAsset;

    // This appears to be a magic number :)
    private float yOffset = 61.0f;

    // Unity puts (0,0) at bottom left, OpenmojiSpritemap puts it at top left
    // Two coordinate changes: One at the texture sheet level, and one at the glyph level
    private Rect GetGlyphRect(OpenmojiSpritemapFormat spritemap, int index) {
        int y = spritemap.height - (index / spritemap.columns + 1) * spritemap.emojiSize;
        int x = (int)(index % spritemap.columns) * spritemap.emojiSize;
        return new Rect(x, y, spritemap.emojiSize, spritemap.emojiSize);
    }

            // static void AddDefaultMaterial(SpriteAsset spriteAsset)
            // {

            // }


    private void OnGUI()
    {
        GUILayout.Label("Importer Settings", EditorStyles.boldLabel);

        textAsset = (TextAsset)EditorGUILayout.ObjectField("JSON File", textAsset, typeof(TextAsset), false);
        spriteAsset = (Texture)EditorGUILayout.ObjectField("Spritemap", spriteAsset, typeof(Texture), false);

        if (GUILayout.Button("Process and Save"))
        {
            if (textAsset != null)
            {
                var obj = CreateInstance<TMP_SpriteAsset>();
                obj.spriteSheet = spriteAsset;
                obj.spriteInfoList = new List<TMP_Sprite>();

                // Save the new asset
                // string outputPath = "Assets/Resources/ProcessedData.asset";
                string outputPath = EditorUtility.SaveFilePanel("Save Sprite Asset File", "", textAsset.name, "asset");
                outputPath = outputPath.Substring(outputPath.IndexOf("Assets/"));
                AssetDatabase.CreateAsset(obj, outputPath);
                AssetDatabase.Refresh();

                Debug.Log("Processing complete. New asset saved at: " + outputPath);

                // Need to add the material and serialize them together (so the version updating process functions)
                Shader shader = Shader.Find("TextMeshPro/Sprite");
                Material material = new Material(shader);
                material.SetTexture(ShaderUtilities.ID_MainTex, obj.spriteSheet);
                material.name = "TextMeshPro/Sprite";
                obj.material = material;
                AssetDatabase.AddObjectToAsset(material, obj);

                EditorUtility.SetDirty(textAsset); // Mark the original text asset as dirty to save changes
                AssetDatabase.SaveAssets();

                // Convert the blank Sprite Asset to the "new version" so we can properly fill in the glyph/character tables
                obj.UpdateLookupTables();

                var spritemapData = JsonUtility.FromJson<OpenmojiSpritemapFormat>(textAsset.text);
                // Debug.Log(textAsset.text);
                // Debug.Log(spritemapData.emojis.Count);
                string filePath = AssetDatabase.GetAssetPath(spriteAsset);
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(filePath).Select(x => x as Sprite).Where(x => x != null).ToArray();
                
                uint index = 0;
                foreach (var emoji in spritemapData.emojis) {
                    var glyphRect = GetGlyphRect(spritemapData, (int)index);
                    var glyph = new TMP_SpriteGlyph
                    {
                        glyphRect = new UnityEngine.TextCore.GlyphRect(glyphRect),
                        metrics = new UnityEngine.TextCore.GlyphMetrics(spritemapData.emojiSize, spritemapData.emojiSize, 0, yOffset, spritemapData.emojiSize),
                        scale = 1.0f,
                        index = index,
                        atlasIndex = 0,
                        sprite = sprites[index]
                    };
                    obj.spriteGlyphTable.Add(glyph);

                    uint uni = 0;
                    Debug.Log(emoji.emoji.hexcode);
                    if (uint.TryParse(emoji.emoji.hexcode, System.Globalization.NumberStyles.HexNumber, null, out uint result)) {
                        uni = result;
                    }
                    var character = new TMP_SpriteCharacter
                    {
                        name = emoji.emoji.annotation,
                        glyphIndex = glyph.index,
                        scale = 1.0f,
                        unicode = uni,
                        glyph = glyph,
                    };
                    obj.spriteCharacterTable.Add(character);

                    index++;
                }
                obj.UpdateLookupTables();
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Please select a valid Text Asset.");
            }
        }

        

        if (GUILayout.Button("Create Empty Sprite Asset"))
        {
            var obj = CreateInstance<TMP_SpriteAsset>();
            // string outputPath = "Assets/Resources/ProcessedData.asset";
            string outputPath = EditorUtility.SaveFilePanel("Save Sprite Asset File", "", "openmoji_spritemaps", "asset");
            outputPath = outputPath.Substring(outputPath.IndexOf("Assets/"));
            AssetDatabase.CreateAsset(obj, outputPath);
            AssetDatabase.Refresh();
        }
    }
}

