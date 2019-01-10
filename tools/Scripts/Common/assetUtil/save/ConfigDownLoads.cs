using XLua;
﻿using LitJson;
using SG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using UnityEngine;

[Hotfix]
public class ConfigDownLoads 
{
    
    virtual public void init()
    {
        load(); 
    }

    public ConfigDownLoads(string _fileName)
    {
        fileName = _fileName;
        
    }

    string fileName = @"";

    public void load()
    {
        //string tmpLog = string.Format("----config {0}  Load file :" , fileName);
        string filepath = CoreEntry.gResLoader.getPersistentDataPath() + fileName;

        if (!System.IO.File.Exists( filepath)){
            LogMgr.UnityWarning( "文件不存在"+filepath) ; 
            return; 
        }

        StreamReader file; 
        try
        {
             file = new StreamReader(filepath, false);

        }
        catch ( Exception e)
        {
            LogMgr.UnityError(e.ToString());
            return; 
        }
        //string strDecode = TeaMgr.Instance.decode(file.ReadToEnd());
        string strDecode =  file.ReadToEnd();

        //LogMgr.UnityLog(tmpLog + strDecode); 

        JsonData jd = JsonMapper.ToObject(strDecode);

        //LogMgr.UnityLog(tmpLog + "    1"); 
        JsonData jdItems = jd;

       // LogMgr.UnityLog(tmpLog + "    2"); 

        foreach (string  key in jdItems.Keys)
        {
             
        
            //LogMgr.UnityLog("key  = " + key);
            JsonData value = jdItems[key];
            if (value.IsInt)
            {
                m_config.Add((string)key, (int)value);
            }
            else if (value.IsString)
            {
                m_config.Add((string)key, (string )value);
            }
            else if (value.IsBoolean)
            {
                m_config.Add((string)key, (bool )value);
            }
            else if (value.IsDouble)
            {
                m_config.Add((string)key, (double)value);
            }
            else
            {
                LogMgr.UnityLog("未知的类型:" + value.ToString());
            }

             
        }
     
        file.Close(); 
        //VarMgr.Instance.loadFrom((string)jd["var"]);       

    }


    public void save()
    {
        string filepath = CoreEntry.gResLoader.getPersistentDataPath() + fileName;
        saveToFile(filepath);

    }




    public void saveToFile(string path)
    {
        StreamWriter m_logFile = new StreamWriter(path, false);
        //m_logFile.Write(makeJsonString());
        string strEncode  ;
        strEncode = makeJsonString();

        //if (LogMgr.isDebug)
        //{
        //    strEncode = makeJsonString();
        //}
        //else
        //{
        //    strEncode = TeaMgr.Instance.encode(makeJsonString());
        //}
        m_logFile.Write(strEncode);
        
        m_logFile.Flush();
        m_logFile.Close(); 
        
    }

    Hashtable m_config = new Hashtable();


    public T GetData<T>(string key, T defaultValue = default(T))
    {
        object obj =  GetData(  key) ;
        if (obj != null)
        {
            return (T)obj;
        }
        else
        {
            //return null; 
            return defaultValue;
        }
    }
    public object GetData(string key)
    {
        lock (m_config)
        {
            if (m_config.ContainsKey(key))
            {
                return m_config[key];
            }
            else
            {
                return null;
            }
        }
    }

    public void setData(string key, object data)
    {
        lock (m_config)
        {

            if (data == null)
            {
                LogMgr.UnityWarning("保存了空对象:" + key);
                if (m_config.ContainsKey(key))
                {
                    m_config.Remove(key); ;
                }
                return;
            }

            if (m_config.ContainsKey(key))
            {
                m_config[key] = data;
            }
            else
            {
                m_config.Add(key, data);
            }



        }

    }

    public void Flush(){
        lock (m_config)
        {
            save();
        }
    }

    public void resetGame()
    {

        m_config.Clear();

        save();
      
    }

    string makeJsonString()
    {
      
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
       
         IDictionaryEnumerator enumerator = m_config.GetEnumerator();
        while (enumerator.MoveNext())
        {
        //    System.Windows.Forms.MessageBox.Show(enumerator.Key.ToString());
        //    System.Windows.Forms.MessageBox.Show(enumerator.Value.ToString());
        //}

        //foreach (DictionaryEntry item in m_config)
        //{
            //if (item == null)
            //{
            //    continue; 
            //}
            IDictionaryEnumerator item = enumerator; 
            writer.WritePropertyName((string)item.Key );
            if (item.Value.GetType() == typeof(int))
            {
                writer.Write((int)item.Value);

            }
            else if (item.Value.GetType() == typeof(string))
            {
                writer.Write((string )item.Value);
            }
            else if (item.Value.GetType() == typeof(float))
            {
                writer.Write((float)item.Value);
            }
            else if (item.Value.GetType() == typeof(double))
            {
                writer.Write((double)item.Value);
            }
            else if (item.Value.GetType() == typeof(bool))
            {
                writer.Write((bool)item.Value);
            }
            else
            {
                writer.Write(item.Value.ToString());

                LogMgr.UnityLog("未知的类型:" + item.Value .ToString() ); 
            }
 
        }

        writer.WriteObjectEnd();

        return sb.ToString() ; 
    }






}

