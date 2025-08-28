using UnityEngine;

public class LanguageController : MonoBehaviour
{
    [SerializeField] private LanguageType language;
    
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
}
