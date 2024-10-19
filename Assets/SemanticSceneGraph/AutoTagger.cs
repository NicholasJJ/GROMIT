using UnityEngine;
using System.Text.RegularExpressions;

public class AutoTagger : MonoBehaviour
{
    private int lastChildCount = -1;

    void Update()
    {
        // Check if the number of children has changed
        if (transform.childCount != lastChildCount)
        {
            lastChildCount = transform.childCount;
            UpdateChildObjects();
        }
    }

    private void UpdateChildObjects()
    {
        foreach (Transform child in transform)
        {
            TextDescription textDesc = child.gameObject.GetComponent<TextDescription>();
            if (textDesc == null)
            {
                textDesc = child.gameObject.AddComponent<TextDescription>();
                SetTextDescriptionValues(child.gameObject, textDesc);
                textDesc.movementLevel = MovementLevel.Moving;
            }
        }
    }

    private void SetTextDescriptionValues(GameObject child, TextDescription textDesc)
    {
        //// For completed vehicle pool
        //string namePattern = @"^SmallSedan(\w+)\(Clone\)(\d+)$";
        //Match match = Regex.Match(child.name, namePattern);

        //if (match.Success)
        //{
        //    string color = match.Groups[1].Value;
        //    textDesc.textName = color + "Car";
        //    textDesc.description = "A non-player small sedan in the color of " + color;
        //}
        //else
        //{
        //    textDesc.textName = child.name;
        //    textDesc.description = child.name;
        //}

        // For simplified vehicle pool
        // Get the current name of the game object
        string originalName = child.name;

        // Remove the "(Clone)" part
        string newName = originalName.Replace("(Clone)", "");

        // Remove any numbers using regular expression
        newName = Regex.Replace(newName, @"\d", "");

        // Set the cleaned name back to the game object
        child.name = newName;
        textDesc.textName = child.name;
        textDesc.description = child.name;

    }
}
