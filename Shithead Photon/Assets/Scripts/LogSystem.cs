using System.IO;
using System;
using UnityEngine;
using System.Text;

public class LogSystem
{
    private const string fileName = "log.txt"; //name of the file including file type
    private static string filePath = Application.dataPath; //Standard path of the file, change this if you want it stored to a specific path
    private static string fullFilePath = filePath + "/" + fileName;

    /// <summary>
    /// Logs a message to the Unity console and stores it in a .txt file
    /// </summary>
    /// <param name="message"></param>
    public static void Log(string message)
    {
        Debug.Log(message);
        logMessage(message);
    }

    private static void logMessage(string message)
    {
        try
        {
            if (!File.Exists(fullFilePath))
            {
                using (FileStream fs = File.Create(fullFilePath))
                {
                    Debug.Log("Creating new log.txt at location: " + fullFilePath);
                    byte[] title = new UTF8Encoding(true).GetBytes("Log for " + Application.productName);
                    fs.Write(title, 0, title.Length);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(fullFilePath))
                {
                    sw.Write(DateTime.Now + " " + message + "\n");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
