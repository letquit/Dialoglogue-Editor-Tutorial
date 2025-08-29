using TMPro;
using UnityEngine;

/// <summary>
/// 语言控制器类，用于管理应用程序的语言设置和字体资源
/// 该类实现单例模式，确保在整个应用程序生命周期中只有一个实例存在
/// </summary>
public class LanguageController : MonoBehaviour
{
    /// <summary>
    /// 语言字体对结构体，用于关联特定语言类型与其对应的字体资源
    /// </summary>
    [System.Serializable]
    public class LanguageFontPair
    {
        public LanguageType language;
        public TMP_FontAsset fontAsset;
    }
    
    [SerializeField] private LanguageType language;
    [SerializeField] private LanguageFontPair[] languageFonts;
    
    public LanguageType Language { get => language; set => language = value; }
    public static LanguageController Instane { get; private set; }
    
    /// <summary>
    /// Unity生命周期函数，在对象初始化时调用
    /// 负责实现单例模式，确保场景中只有一个LanguageController实例
    /// </summary>
    private void Awake()
    {
        // 检查是否已存在实例，如果不存在则设置当前实例并保持对象不被销毁
        if (Instane == null)
        {
            Instane = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果已存在实例，则销毁当前重复的对象
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 根据指定的语言类型获取对应的字体资源
    /// </summary>
    /// <param name="languageType">要获取字体资源的语言类型</param>
    /// <returns>返回与指定语言类型关联的TMP字体资源，如果未找到则返回null</returns>
    public TMP_FontAsset GetFontForLanguage(LanguageType languageType)
    {
        var pair = System.Array.Find(languageFonts, lf => lf.language == languageType);
        return pair != null ? pair.fontAsset : null;
    }
}

