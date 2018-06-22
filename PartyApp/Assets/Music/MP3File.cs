using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Networking
{
    [Serializable]
    public class MP3File
    {
        [NonSerialized]
        public string filePath;
        public string fileName;
        public string sTitle;
        public string sSinger;
        public string sAlbum;
        public string sYear;
        public string sComment;

        public MP3File(string path)
        {
            filePath = path;
            fileName = Path.GetFileName(path);
        }

        public void GenerateMetaData()
        {
            byte[] b = new byte[128];
            FileStream fs = new FileStream(filePath, FileMode.Open);
            fs.Seek(-128, SeekOrigin.End);
            fs.Read(b, 0, 128);
            bool isSet = false;
            String sFlag = System.Text.Encoding.Default.GetString(b, 0, 3);
            if (sFlag.CompareTo("TAG") == 0)
            {
                System.Console.WriteLine("Tag   is   setted! ");
                isSet = true;
            }
            if (isSet)
            {
                //get   title   of   song; 
                sTitle = Encoding.Default.GetString(b, 3, 30);
                //get   singer; 
                sSinger = Encoding.Default.GetString(b, 33, 30);
                //get   album; 
                sAlbum = Encoding.Default.GetString(b, 63, 30);
                //get   Year   of   publish; 
                sYear = Encoding.Default.GetString(b, 93, 4);
                //get   Comment; 
                sComment = Encoding.Default.GetString(b, 97, 30);
            }
            fs.Close();
        }
    }
}
