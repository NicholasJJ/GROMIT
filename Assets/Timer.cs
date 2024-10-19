using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class Timer : Singleton<Timer>
{

    public float generationStartTime = 0;
    public float retries = 0;
    public string lastOutput = "";
    private List<string[]> record = new List<string[]>
    {
        new string[]{"request","retries","time", "outputLen","finalOutput" }
    };
public string item1;
    public string item2;
    public int lastOutputLen = 0;
    private string id;
    public string request;

    private void Start()
    {
        id = System.DateTime.Now.ToLongTimeString();
    }

    public void Begin()
    {
        if (retries == 0)
        {
            generationStartTime = Time.time;
            retries = 0;
        }
    }


    public void Display()
    {
        Debug.Log($"TIMER: total retries: {retries}, total time: {Time.time - generationStartTime}");
        retries = 0;
    }

    public void DisplayEscape()
    {
        Debug.Log($"TIMER: total retries: {retries}, total time: {Time.time - generationStartTime}");
        record.Add(new string[] { item1, item2, retries.ToString(), (Time.time - generationStartTime).ToString(), lastOutputLen.ToString(), SanitizeForCsv(lastOutput) });
        WriteToCSV(Application.dataPath + $"/{id}_data.csv",record.ToArray());
        retries = 0;
    }

    public void DisplaySandbox()
    {
        Debug.Log($"TIMER: total retries: {retries}, total time: {Time.time - generationStartTime}");
        record.Add(new string[] { request, retries.ToString(), (Time.time - generationStartTime).ToString(), lastOutputLen.ToString(), SanitizeForCsv(lastOutput) });
        WriteToCSV(Application.dataPath + $"/{id}_data.csv", record.ToArray());
        retries = 0;
    }

    void WriteToCSV(string filePath, string[][] data)
    {
        // Create a StreamWriter to write to the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Loop through each row of data
            foreach (string[] row in data)
            {
                // Join the values in the row with commas and write to the file
                writer.WriteLine(string.Join(",", row));
            }
        }
    }

    public static string SanitizeForCsv(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Check if the string contains a comma, quote, or newline
        bool containsSpecialChar = input.Contains(",") || input.Contains("\"") || input.Contains("\n");

        if (containsSpecialChar)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");

            foreach (char c in input)
            {
                if (c == '\"')
                {
                    // Escape double quotes by doubling them
                    sb.Append("\"\"");
                }
                else
                {
                    sb.Append(c);
                }
            }

            sb.Append("\"");
            return sb.ToString();
        }

        return input;
    }
}
