using System;
using System.Collections;
using UnityEngine;

namespace Tools
{
    public class Singleton<T> where T : class, new()
    {
        protected static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new T();
                }
                return m_Instance;
            }
        }

        public static T GetInstance()
        {
            return Instance;
        }
    }

    public class UnitySingleton<T> : MonoBehaviour where T : Component, new()
    {
        public static string parentName = "GameGlobal";

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject parent = GameObject.Find(parentName);
                    if (parent == null)
                    {
                        parent = new GameObject();
                        parent.name = parentName;
                        GameObject.DontDestroyOnLoad(parent);
                    }

                    GameObject go = new GameObject();
                    go.name = typeof(T).ToString();
                    m_Instance = go.AddComponent<T>();
                    go.transform.SetParent(parent.transform);
                }
                return m_Instance;
            }
        }
        protected static T m_Instance;
    }
}