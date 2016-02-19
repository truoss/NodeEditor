using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System;
using UnityEngine;
using System.Collections.ObjectModel;

[Serializable]
public class NameKeyedCollection<T> : KeyedCollection<string, T> where T : IHasName
{
    protected override string GetKeyForItem(T item)
    {
        //Debug.Log("key: '" +item.Name+"'");
        return item.Name;
    }

    public void AddOrReplace(T item)
    {
        Remove(item.Name);
        Add(item);
    }
}

public interface IHasName
{
    string Name { get; }
}

[Serializable]
[XmlInclude(typeof(Property<float>))]
[XmlInclude(typeof(Property<Color>))]
[XmlInclude(typeof(Property<Vector2>))]
[XmlInclude(typeof(Property<Vector3>))]
[XmlInclude(typeof(Property<string>))]
public abstract class Property : IHasName
{
    [XmlIgnore]
    [SerializeField]
    protected string name;

    public string Name { get { return name; } set { name = value; } }

    abstract public Property CreateCopyProperty();
    abstract public void CopyPropertyValueTo(Property p);

    abstract public string ValueString { get; }

    abstract public void SetValueString(string newVal);
}



[Serializable]
public class Property<T> : Property
{
    public T Value;


    [XmlIgnore]
    override public string ValueString { get { return Value.ToString(); } }


    override public Property CreateCopyProperty()
    {
        var p = new Property<T>();
        p.Name = Name;
        p.Value = Value;
        return p;
    }

    override public void SetValueString(string newVal)
    {
        this.Value = (T)Convert.ChangeType(newVal, typeof(T));
    }


    override public void CopyPropertyValueTo(Property p)
    {
        ((Property<T>)p).Value = Value;
    }

}

[Serializable]
public class PropertiesData
{
    public NameKeyedCollection<Property> properties = new NameKeyedCollection<Property>();

    public Property Get(string name)
    {
        if (properties.Contains(name))
            return properties[name];
        else
            return null;
    }

    public Property<T> Get<T>(string name)
    {
        if (properties.Contains(name))
            return properties[name] as Property<T>;
        else
            return null;
    }


    public Property<T> ForceGet<T>(string name, T defaultValue)
    {
        var p = Get<T>(name);
        if (p == null)
        {
            p = new Property<T>();
            p.Name = name;
            p.Value = defaultValue;
            properties.Add(p);
        }

        return p;
    }
    
    // bool values
    public void ForceSetBool(string key, bool value)
    {
        string v = value ? "true" : "false";
        ForceGet<string>(key, v).Value = v;
    }

    public bool ForceGetBool(string key, bool defaultValue)
    {
        return ForceGet<string>(key, defaultValue ? "true" : "false").Value == "true" ? true : false;
    }


    private void CopyPropertyFrom(Property p)
    {
        if (!properties.Contains(p.Name))
        {
            properties.Add(p.CreateCopyProperty());
        }
        else
        {
            p.CopyPropertyValueTo(properties[p.Name]);
        }
    }


    public void CopyPropertiesDataFrom(PropertiesData ps)
    {
        foreach (var p in ps.properties)
        {
            CopyPropertyFrom(p);
        }
    }

    public void DebugPrint()
    {
        foreach (var item in properties)
        {
            Debug.LogWarning(item.ValueString);
        }
    }
}


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
}