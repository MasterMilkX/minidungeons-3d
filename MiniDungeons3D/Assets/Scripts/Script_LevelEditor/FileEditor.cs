using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileEditor : MonoBehaviour {

	public void ChangeLine(string sourceAddress, string destinationAddress, int lineNumber, int charNumber, char newChar) {

		int lineToEdit = lineNumber + 1; // Warning: 1-based indexing!
		string sourceFile = sourceAddress;
		string destinationFile = destinationAddress;
		string newData = null;

		// Read the appropriate line from the file.
		string lineToWrite = null;
		using (StreamReader reader = new StreamReader(sourceFile)) {
			for (int i = 1; i <= lineToEdit; ++i)
				lineToWrite = reader.ReadLine();
		}

		//	if (lineToWrite == null)
		//	throw new InvalidDataException("Line does not exist in " + sourceFile);

		lineToWrite = ReplaceCharAt(lineToWrite, charNumber, newChar);

		// Read the old file.
		string[] lines = File.ReadAllLines(destinationFile);
		// Write the new file over the old file.
		using (StreamWriter writer = new StreamWriter(destinationFile)) {
			for (int currentLine = 1; currentLine < lines.Length; currentLine++) {
				//Debug.Log("currentLine: " + currentLine);
				if (currentLine == lineToEdit) {
					char[] array = lineToWrite.ToCharArray();
					foreach(char c in array)
					{
						//writer.Write(c);
						newData = newData + c;
					}
					if (currentLine < lines.Length - 1)
					{
						//writer.WriteLine("");
						newData = newData + "";
					}

				} else {
					char[] array = lines[currentLine - 1].ToCharArray();
					foreach (char c in array)
					{
						//writer.Write(c);
						newData = newData + c;
					}
					if (currentLine < lines.Length - 1)
					{
						//writer.WriteLine("");
						newData = newData + "";
					}
				}
				newData = newData + "\n";
			}
			//writer.Write("\nXXXXXXXXXX");
			newData = newData + "XXXXXXXXXX";
		}
		//Debug.Log (newData);
		File.WriteAllText(destinationFile, newData);
	}

	public void ChangeLine(string destinationAddress, string[][] newfile)
	{
		string newData = null;
		// Write the new file over the old file.
		using (StreamWriter writer = new StreamWriter(destinationAddress)) {
			for (int currentLine = 1; currentLine<newfile.Length; currentLine++) {
				//Debug.Log("currentLine: " + currentLine);
				string[] array = newfile[currentLine - 1];
				foreach(string c in array)
				{
					//writer.Write(c);
					newData = newData + c;
				}
				if (currentLine<newfile.Length - 1)
				{
					//writer.WriteLine("");
					newData = newData + "";
				}
				newData = newData + "\n";
			}
			//writer.Write("\nXXXXXXXXXX");
			newData = newData + "XXXXXXXXXX";
		}
		File.WriteAllText(destinationAddress, newData);
	}
	string ReplaceCharAt(string input, int index, char newChar) {
		char[] chars = input.ToCharArray();
		chars[index] = newChar;
		return new string(chars);
	}
}
