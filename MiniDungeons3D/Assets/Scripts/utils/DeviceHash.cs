// Taken from Christogger Holmgard

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DeviceHash {
    private BinaryFormatter binaryFormatter = new BinaryFormatter();
    private FileStream fileStream;
    private string filePathAndName;
    //	private string secretLittleWord = "secretLittleWord";

    private bool debug = false;

    public DeviceHash(string filePath) // filePath => Application.persistentDataPath
    {
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        filePathAndName = filePath + "/d.dat";

        if (!File.Exists(filePathAndName)) {
            if (debug) Debug.Log("DeviceHash doesn't exist! Generating new one... ");
            Generate();
        }
    }

    public string Get() {
        try {
            fileStream = File.Open(filePathAndName, FileMode.Open);
            string returnString = (string)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            if (debug) Debug.Log("LOADED DeviceHash: " + returnString);
            return returnString;
        } catch {
            fileStream.Close();

            if (debug) Debug.Log("ERROR LOADING DeviceHash! Generating new one... ");
            Generate();
            return Get();
        }
    }

    public void Reset() {
        Generate();
    }

    private void Generate() {
        //string newHash = (System.DateTime.Now.Ticks.ToString () + secretLittleWord + Random.value.ToString ()).GetHashCode ().ToString ();
        //string newHash = (System.DateTime.Now.Ticks.ToString () + secretLittleWord + Random.value.ToString ())

        string newHash = System.Guid.NewGuid().ToString("N");
        fileStream = File.Create(filePathAndName);
        binaryFormatter.Serialize(fileStream, newHash);
        fileStream.Close();

        if (debug) Debug.Log("NEW DeviceHash GENERATED: " + newHash);
    }
}