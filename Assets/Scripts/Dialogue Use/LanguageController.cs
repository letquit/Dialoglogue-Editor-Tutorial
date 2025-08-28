
using TMPro;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
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
    
    private void Awake()
    {
        if (Instane == null)
        {
            Instane = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public TMP_FontAsset GetFontForLanguage(LanguageType languageType)
    {
        var pair = System.Array.Find(languageFonts, lf => lf.language == languageType);
        return pair != null ? pair.fontAsset : null;
    }
}
