using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System;
using UnityEngine;

public static class Serialization
{
    /*
    Beispiel Data:
    [Serializable]
    public class TimelineSerializationData
    {
        public StudioSetSerializationData currentSet; //aktuelles Set
        public int currentEntryIdx = -1; //index des aktuellen TimelineEntry-Eintrags
        public List<TimelineEntrySerializationData> timelineEntryDataList = new List<TimelineEntrySerializationData>(); //liste aller Timeline-Entries
    }
    
    Beispiel aufruf:
    Save:
    Serialization.SaveSerDataToZip(stream, tlsd, filename, DEFAULT_PASSWORD, imagesToCopy, imageDirStartFolder);
    
    Load:
    tlsd = LoadFromFile(path);
    -->
    try
        {						
            //Entpacke Set-Datei in einen Stream
            Stream stream = ZipUtil.UnzipFileToStream(path, DEFAULT_PASSWORD);
            isLegacyLoad = false;

            if (stream == null)
            {
                Debug.Log("Fallback: load legacy file format...");
                //Lade setfile direkt
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                isLegacyLoad = true;
                
                if (stream == null)
                 return null;
            }

            //deserialisiere und initialisiere
            using (stream)
            {
                try
                {
                    //deserialisiere               
                    deserialized = Serialization.DeserializeFromXmlStream<TimelineSerializationData>(stream);                                       
                }
                catch (Exception e)
                {
                    Debug.LogError("Error on loading timeline: " + e);

                    NewTimeline();
                }
            }
            //wenn start dialog noch offen -> close
            //MainUi.I.startDialog.Close();           
            //Ende Verarbeite XML

            //UpdateRecentProjectPaths(path);

            return deserialized;
        }
        catch
        {            
            Debug.Log("CallBackActionLoadWorkshop() failed!");
        }
    */


    static public void SaveToUTF8XmlFile<T>(T obj, string path)
    {
        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            SerializeToUTF8XmlStream(obj, stream);
        }
    }


    static public T LoadFromXmlFile<T>(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            return DeserializeFromXmlStream<T>(stream);
        }
    }

    static public void SerializeToUTF8XmlStream<T>(T obj, Stream stream)
    {        
        var settings = new XmlWriterSettings()
        {
            Encoding = Encoding.UTF8,
            ConformanceLevel = ConformanceLevel.Document,
            Indent = true,
            IndentChars = "\t",
            NewLineHandling = NewLineHandling.None,
            NewLineChars = "\r\n",
            CloseOutput = false,
            //OmitXmlDeclaration = true
        };

        using (var xmlWriter = XmlWriter.Create(stream, settings))
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(xmlWriter, obj);
        }
    }

    //NOTE: Encoding should be processed automatically by deserializer
    static public T DeserializeFromXmlStream<T>(Stream stream)
    {
        try
        {
            using (var sr = new StreamReader(stream, Encoding.UTF8, true))
            {                
                using (var reader = XmlTextReader.Create(sr))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
        } catch (Exception e) 
        {
            Debug.LogError("DeserializeFromXmlStream(): Datei konnte nicht gelesen werden! " + e);

            return default(T);
        }
    }   



    /*
    static public void SaveSerDataToZip<T>(FileStream outputFileStream, T serialisationData, string internalFilename, string password, bool copyRessources, string ressourcesPath)
    {
        var settings = new XmlWriterSettings()
        {
            Encoding = Encoding.UTF8,
            ConformanceLevel = ConformanceLevel.Document,
            Indent = true,
            IndentChars = "\t",
            NewLineHandling = NewLineHandling.None,
            NewLineChars = "\r\n",
            CloseOutput = false,
            //OmitXmlDeclaration = true
        };


        using (MemoryStream stream = new MemoryStream())
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(T));
                s.Serialize(XmlWriter.Create(stream, settings), serialisationData);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                //Durch die "using"-Anweisung wird der content aus dem stream nach dem verlassen verworfen
                ZipUtil.CreateZIPFormMemoryStream(stream, internalFilename, outputFileStream, password, copyRessources, ressourcesPath);
            }
            catch (Exception e)
            {
                Debug.LogError(e.GetBaseException().Message);
            }
        }
    }
    
   
    static public void SaveSerDataToZip<T>(FileStream outputFileStream, T serialisationData, string filename, string password, List<string> imagesToCopy, string imageDirStartFolder)
    {
        var settings = new XmlWriterSettings()
        {
            Encoding = Encoding.UTF8,
            ConformanceLevel = ConformanceLevel.Document,
            Indent = true,
            IndentChars = "\t",
            NewLineHandling = NewLineHandling.None,
            NewLineChars = "\r\n",
            CloseOutput = false,
        };


        using (MemoryStream stream = new MemoryStream())
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(T));
                s.Serialize(XmlWriter.Create(stream, settings), serialisationData);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                //Durch die "using"-Anweisung wird der content aus dem stream nach dem verlassen verworfen
                ZipUtil.CreateZIPFormMemoryStreamWithResourceCopy(stream, filename, outputFileStream, password, imagesToCopy,imageDirStartFolder);
            }
            catch (Exception e)
            {
                Debug.LogError(e.GetBaseException().Message);
            }
        }
    }
    */
}