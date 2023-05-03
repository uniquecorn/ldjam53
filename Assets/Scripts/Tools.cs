using UnityEditor;
using UnityEngine;

public static class Tools
{
    public static string ReadTextFile(string sFileName)
    {
        //Debug.Log("Reading " + sFileName);

        //Check to see if the filename specified exists, if not try adding '.txt', otherwise fail
        var sFileNameFound = "";
        if (System.IO.File.Exists(sFileName))
        {
            //Debug.Log("Reading '" + sFileName + "'.");
            sFileNameFound = sFileName; //file found
        }
        else if (System.IO.File.Exists(sFileName + ".txt"))
        {
            sFileNameFound = sFileName + ".txt";
        }
        else
        {
            Debug.Log("Could not find file '" + sFileName + "'.");
            return null;
        }

        System.IO.StreamReader sr;
        try
        {
            sr = new System.IO.StreamReader(sFileNameFound);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Something went wrong with read.  " + e.Message);
            return null;
        }

        var fileContents = sr.ReadToEnd();
        sr.Close();

        return fileContents;
    }
    public static void WriteTextFile(string sFilePathAndName, string sTextContents)
    {
        var sw = new System.IO.StreamWriter(sFilePathAndName);
        sw.WriteLine(sTextContents);
        sw.Flush();
        sw.Close();
    }
}