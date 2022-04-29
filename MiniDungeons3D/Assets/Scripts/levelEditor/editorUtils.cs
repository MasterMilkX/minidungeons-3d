using UnityEngine;
using System.Collections;
using System.IO;

public class editorUtils : MonoBehaviour
{

    levelEditorManager _levelEditorManager;

    void Start()
    {
        _levelEditorManager = GetComponent<levelEditorManager>();
    }

    public void changeLine(string sourceAddress, string destinationAddress, int lineNumber, int charNumber, char newChar)
    {

        int lineToEdit = lineNumber + 1; // Warning: 1-based indexing!
        string sourceFile = sourceAddress;
        string destinationFile = destinationAddress;

        // Read the appropriate line from the file.
        string lineToWrite = null;
        using (StreamReader reader = new StreamReader(sourceFile))
        {
            for (int i = 1; i <= lineToEdit; ++i)
                lineToWrite = reader.ReadLine();
        }

        if (lineToWrite == null)
            throw new System.Exception("Line does not exist in " + sourceFile);

        lineToWrite = ReplaceCharAt(lineToWrite, charNumber, newChar);

        // Read the old file.
        string[] lines = File.ReadAllLines(destinationFile);

        // Write the new file over the old file.
        using (StreamWriter writer = new StreamWriter(destinationFile))
        {
            for (int currentLine = 1; currentLine < lines.Length; currentLine++)
            {
                Debug.Log("currentLine: " + currentLine);
                if (currentLine == lineToEdit)
                {
                    char[] array = lineToWrite.ToCharArray();
                    foreach (char c in array)
                    {
                        writer.Write(c);
                    }
                    if (currentLine < lines.Length - 1)
                    {
                        writer.WriteLine("");
                    }
                    //writer.WriteLine(lineToWrite);
                }
                else
                {
                    char[] array = lines[currentLine - 1].ToCharArray();
                    foreach (char c in array)
                    {
                        writer.Write(c);
                    }
                    if (currentLine < lines.Length - 1)
                    {
                        writer.WriteLine("");
                    }
                    //writer.WriteLine(lines[currentLine - 1]);
                }
            }
            writer.Write("\nXXXXXXXXXX");

        }
    }

    public void ChangeFile(string sourceAddress, string destinationAddress, string[][] newMap)
    {
        string sourceFile = sourceAddress;
        string destinationFile = destinationAddress;

        using (StreamWriter writer = new StreamWriter(destinationFile))
        {
            for (int i = 0; i < newMap.Length; i++)
            {
                for (int j = 0; j < newMap[i].Length; j++)
                {
                    writer.Write(newMap[i][j]);
                }
                Debug.Log("I: " + i);
                if (i != newMap.Length - 1)
                {
                    writer.WriteLine("");
                }
            }
        }

    }
    public string ReplaceCharAt(string input, int index, char newChar)
    {
        char[] chars = input.ToCharArray();
        if (chars[index] == 'H') _levelEditorManager.activeEnteranceCount--;
        if (chars[index] == 'p') _levelEditorManager.activePortalCount--;
        if (chars[index] == 'e') _levelEditorManager.activeExitCount--;

        if (_levelEditorManager.activePortalCount == 1 && newChar == ' ') Debug.Log("Please delete the other portal for the changes to execute.");

        chars[index] = newChar;
        return new string(chars);
    }


}
