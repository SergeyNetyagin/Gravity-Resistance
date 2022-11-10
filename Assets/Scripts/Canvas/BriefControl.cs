using UnityEngine;
using System.Collections;

public class BriefControl : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Уровень, для которого предназначен данный бриф" )]
    private LevelType level_type;
    public LevelType Level_type { get { return level_type; } }

    [SerializeField]
    [Tooltip( "Ключ для локализации надписи <Нажмите> (формат: Level.xx.Brief.Next.Press)" )]
    private string press_next_key;

    [SerializeField]
    [Tooltip( "Ключ для локализации надписи <или> (формат: Level.xx.Brief.Next.Or)" )]
    private string or_next_key;

    [SerializeField]
    [Tooltip( "Ключ для локализации надписи <для продолжения...> (формат: Level.xx.Brief.Next.Continue)" )]
    private string continue_next_key;

    [SerializeField]
    [Tooltip( "Ключ для локализации подписи продолжения для геймпада (формат: Level.xx.Brief.Next.Device.Gamepad)" )]
    private string gamepad_next_key;

    [SerializeField]
    [Tooltip( "Ключ для локализации подписи продолжения для клавиатуры (формат: Level.xx.Brief.Next.Device.Keyboard)" )]
    private string keyboard_next_key;

    [SerializeField]
    [Tooltip( "Ключ для локализации подписи продолжения для мыши (формат: Level.xx.Brief.Next.Device.Mouse)" )]
    private string mouse_next_key;

    [SerializeField]
    [Tooltip( "Ключ для локализации подписи продолжения для мобильных платформ (формат: Level.xx.Brief.Next.Device.Mobile)" )]
    private string mobile_next_key;

    private Brief[] brief_pages;

    private int current_brief_page = 0;

    private Transform cached_transform;
    
    // Use this for initialization
	void Awake() {

        if( Game.Level.Use_brief && (Game.Level.Type == level_type) ) gameObject.SetActive( true );
        else gameObject.SetActive( false );

        if( !Game.Level.Use_brief ) return;

        cached_transform = transform;
	
        brief_pages = new Brief[ cached_transform.childCount ];

        for( int i = 0; i < cached_transform.childCount; i++ ) {

            brief_pages[i] = cached_transform.GetChild( i ).GetComponent<Brief>();
            brief_pages[i].gameObject.SetActive( false );
        }
    }
    
    // Use this for initialization #############################################################################################################################################
	void Start () {

        if( !Game.Level.Use_brief ) return;
        else if( (brief_pages != null) && (brief_pages.Length > 0) )  StartCoroutine( UpdateCoroutine() );
    }

    // Служит в основном для анализа натия любой клавиши #######################################################################################################################
    private IEnumerator UpdateCoroutine() {

        Game.Level.SetBriefMode( true );
        Game.Control.DisabeAudioExceptMusic();

        TranslateBriefPage( current_brief_page );
        brief_pages[ current_brief_page ].gameObject.SetActive( true );

        while( Game.Level.Is_brief_mode ) {

            if( Game.Input_control.Space_key_pressed || Game.Input_control.Mouse_button_pressed ) {

                // Deactivate current brief's page
                brief_pages[ current_brief_page++ ].gameObject.SetActive( false );

                // Actiavte a next brief's page, if it accessible
                if( current_brief_page < cached_transform.childCount ) {

                    TranslateBriefPage( current_brief_page );
                    brief_pages[ current_brief_page ].gameObject.SetActive( true );
                }

                // Else close a brief's pages and go play the game
                else {

                    cached_transform.gameObject.SetActive( false );

                    Game.Resume();
                    Game.SetState( GameState.Playing );
                    Game.Input_control.EnableControl();

                    Game.Canvas.RefreshFlightIndicators( true );
                    Game.Level.SetBriefMode( false );
                    Game.Control.EnabeAudioAll();
                }
            }

            yield return null;
        }

        yield break;
    }

    // Локализует текст на странице брифа в зависимости от платформы и устройства управления ###################################################################################
    private void TranslateBriefPage( int page ) {

        Brief brief = brief_pages[ page ];

        brief.Text_title.Rewrite( Game.Localization.GetTextValue( brief.Title_key ) );
        brief.Text_description.Rewrite( Game.Localization.GetTextValue( brief.Description_key ) );

        // Начало строки подсказки о продолжении
        brief.Text_continue.Rewrite( Game.Localization.GetTextValue( press_next_key ) ).Append( Game.Separator_space );

        // Если это мобильное приложение
        if( Application.isMobilePlatform ) {

            brief.Text_continue.Append( Game.Localization.GetTextValue( mobile_next_key ) ).Append( Game.Separator_space );
        }

        // Если это приложение на PC-платформе (может быть ввод от клавиатуры, от мыши, от геймпада или джойстика)
        else {

            // Клавиатура
            brief.Text_continue.Append( Game.Localization.GetTextValue( keyboard_next_key ) ).Append( Game.Separator_space );

            // Мышь
            if( Input.mousePresent ) brief.Text_continue.Append( Game.Localization.GetTextValue( or_next_key ) ).Append( Game.Separator_space ).
                Append( Game.Localization.GetTextValue( mouse_next_key ) ).Append( Game.Separator_space );

            // Геймпад или джойстик
            string[] joysticks = Input.GetJoystickNames();
            if( (joysticks != null) && (joysticks.Length > 0) ) brief.Text_continue.Append( Game.Localization.GetTextValue( or_next_key ) ).Append( Game.Separator_space ).
                Append( Game.Localization.GetTextValue( gamepad_next_key ) ).Append( Game.Separator_space );
        }

        // Завершение строки подсказки о продолжении
        brief.Text_continue.Append( Game.Localization.GetTextValue( continue_next_key ) );
    }
}