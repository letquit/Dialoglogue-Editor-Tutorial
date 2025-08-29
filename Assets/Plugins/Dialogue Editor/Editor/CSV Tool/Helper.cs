using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 辅助工具类，提供资源查找相关功能
/// </summary>
public static class Helper
{
    /// <summary>
    /// 从Resources文件夹及其子文件夹中查找所有指定类型的资源对象
    /// </summary>
    /// <typeparam name="T">要查找的资源类型</typeparam>
    /// <returns>包含所有找到的指定类型资源对象的列表</returns>
    public static List<T> FindAllObjectFromResources<T>()
    {
        List<T> tmp = new List<T>();
        string ResourcesPath = Application.dataPath + "/Resources";
        // 获取Resources文件夹下所有子目录
        string[] directories = Directory.GetDirectories(ResourcesPath, "*", SearchOption.AllDirectories);

        // 遍历所有目录，加载其中的指定类型资源
        foreach (string directory in directories)
        {
            string directoriesPath = directory.Substring(ResourcesPath.Length + 1);
            T[] result = Resources.LoadAll(directoriesPath, typeof(T)).Cast<T>().ToArray();

            // 避免重复添加相同的资源对象
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
    
    /// <summary>
    /// 查找项目中所有DialogueContainerSO类型的资源对象
    /// </summary>
    /// <returns>包含所有DialogueContainerSO资源对象的列表</returns>
    public static List<DialogueContainerSO> FindAllObjectContainerSO()
    {
        // 通过资源数据库查找所有DialogueContainerSO类型的资源GUID
        string[] guids = AssetDatabase.FindAssets("t:DialogueContainerSO");
        
        DialogueContainerSO[] tmp = new DialogueContainerSO[guids.Length];
        
        // 根据GUID加载对应的资源对象
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            tmp[i] = AssetDatabase.LoadAssetAtPath<DialogueContainerSO>(path);
        }
        
        return tmp.ToList();
    }
}

