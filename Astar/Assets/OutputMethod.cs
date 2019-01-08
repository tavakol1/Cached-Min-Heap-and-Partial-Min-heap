using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEngine;

public class OutputMethod {

    private string path = "C:\\Users\\Mohsen Tavakoli\\Desktop\\Mother Of All Tests!\\";
    private string FileCreated;

    public bool CreateDirectory(string NewFolderName)
    {
        path += NewFolderName;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Console.WriteLine("suceess!");
            path += "\\";
            return true;
        }
        else
        {
            path += "\\";
            return false;
        }
    }

    public void CreateFile(string NewFileName)
    {
        var time = DateTime.Now;
        //format the time when you retrieve it, not when you store it - i.e. -
        string formattedtime = time.ToString(" - yyyy-mm-dd-hh-mm-ss");
        FileCreated = path + NewFileName + formattedtime + ".csv";
        using (StreamWriter sw = new StreamWriter(FileCreated, true))
        {
            sw.WriteLine("This is the Path for the file: {0}\n", FileCreated);
            sw.WriteLine("The date is: {0}", DateTime.Now);
        }
    }

    public void ContinueFile(string _input)
    {
        StreamWriter file = new StreamWriter(FileCreated, true);
        file.WriteLine(_input);
        file.Close();
    }
}
