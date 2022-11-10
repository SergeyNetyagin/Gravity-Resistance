using UnityEngine;
using System.Collections;

public class Brief : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Текст заголовка брифа (может просто повторять название уровня)" )]
    private EffectiveText text_title;
    public EffectiveText Text_title { get { return text_title; } }

    [SerializeField]
    [Tooltip( "Ключ для локализации заголовка данной страницы брифа (формат: Level.xx.Brief.xx.Title)" )]
    private string title_key;
    public string Title_key { get { return title_key; } }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Текст подробного описания брифа (содержит описание особенностей уровня и другие детали, полезные игроку: например, окружающие условия)" )]
    private EffectiveText text_description;
    public EffectiveText Text_description { get { return text_description; } }
    
    [SerializeField]
    [Tooltip( "Ключ для локализации описания уровня или задания для данной страницы брифа (формат: Level.xx.Brief.xx.Description)" )]
    private string description_key;
    public string Description_key { get { return description_key; } }

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Текст-подсказка <Нажмите ... для продолжения...>" )]
    private EffectiveText text_continue;
    public EffectiveText Text_continue { get { return text_continue; } }
            
    // Use this for initialization
	void Awake() {
	
        if( !Game.Level.Use_brief ) return;
	}
	
    // Use this for initialization
	void Start() {
	
        if( !Game.Level.Use_brief ) return;
    }
}
