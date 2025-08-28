using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Helper
{
    public static List<T> FindAllObjectFromResources<T>()
    {
        List<T> tmp = new List<T>();
        string ResourcesPath = Application.dataPath + "/Resources";
        string[] directories = Directory.GetDirectories(ResourcesPath, "*", SearchOption.AllDirectories);

        foreach (string directory in directories)
        {
            string directoriesPath = directory.Substring(ResourcesPath.Length + 1);
            T[] result = Resources.LoadAll(directoriesPath, typeof(T)).Cast<T>().ToArray();

            foreach (T item in result)
            {
                if (!tmp.Contains(item))
                {
                    tmp.Add(item);
                }
            }
        }
        
        return tmp;
    }
}
