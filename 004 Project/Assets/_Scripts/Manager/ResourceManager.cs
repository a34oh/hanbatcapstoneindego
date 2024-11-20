using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

        }
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public GameObject Instantiate(string path, Vector3 position, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        GameObject go = Object.Instantiate(original, position, Quaternion.identity, parent);
        go.name = original.name;
        return go;
    }
    public GameObject Instantiate(GameObject prefab, Vector3 position,Quaternion rotation, Transform parent = null)
    {
        GameObject original = prefab;
        if (original == null)
        {
            Debug.Log($"Failed to load prefab");
            return null;
        }
        GameObject go = Object.Instantiate(original, position, rotation, parent);
        go.name = original.name;
        return go;
    }
    public AudioClip LoadSound(string soundName)
    {
        string path = $"Sounds/{soundName}";
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogWarning($"Failed to load sound: {path}");
        }
        return clip;
    }
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}