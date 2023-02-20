using UnityEngine;
using SmartLocalization;
using System.Collections.Generic;

public enum Language {

    Autodetect = 0,
    English = 1,
    Chinese = 2,
    Japanese = 3,
    German = 4,
    Korean = 5,
    French = 6,
    Italian = 7,
    Spanish = 8,
    Portuguese = 9,
    Russian = 10
}

[System.Serializable]
public struct LanguageCode {

    public Language Language;
    public string Code;
}

public class LocalizationControl : MonoBehaviour {

    [SerializeField]
    private LanguageCode[] language_codes;

    private SmartCultureInfo device_culture;
    public SmartCultureInfo Devce_culture { get { return device_culture; } }

	private LanguageManager language_manager;
    public LanguageManager Language_manager { get { return language_manager; } }

	private List<string> current_language_keys;
	private List<SmartCultureInfo> available_languages;

    public string GetTextValue( string text_value ) { return language_manager.GetTextValue( text_value ); }
    public AudioClip GetAudioClip( string audio_clip_name ) { return language_manager.GetAudioClip( audio_clip_name ); }
    
    // Initialization the language system ######################################################################################################################################
    private void Awake() {

		language_manager = LanguageManager.Instance;
		
        if( language_manager.NumberOfSupportedLanguages == 0 ) {
         
            return;
        }

        if( Game.Localization == null ) Game.Localization = this;
        		
		device_culture = GetDeviceCulture( Game.Language );
		if( device_culture != null ) language_manager.ChangeLanguage( device_culture );	

        current_language_keys = language_manager.GetAllKeys();
		available_languages = language_manager.GetSupportedLanguages();

		LanguageManager.Instance.OnChangeLanguage += OnChanged;
	}

    // Returns a specified CultureInfo #########################################################################################################################################
    private SmartCultureInfo GetDeviceCulture( Language language ) {

        SmartCultureInfo culture_info = null;

        if( language == Language.Autodetect ) culture_info = language_manager.GetDeviceCultureIfSupported();

        else {

            for( int i = 0; i < language_codes.Length; i++ ) {

                if( language_codes[i].Language == language ) {

                    for( int j = 0; j < available_languages.Count; j++ ) {

                        if( string.Equals( language_codes[i].Code, available_languages[j].languageCode ) ) {

                            culture_info = available_languages[j];
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }

        return culture_info;
    }

    // Event: on destroy system ################################################################################################################################################
	void OnDestroy() {

		if( LanguageManager.HasInstance ) LanguageManager.Instance.OnChangeLanguage -= OnChanged;
	}

    // Event: on change language ###############################################################################################################################################
	void OnChanged( LanguageManager manager ) { 

		current_language_keys = manager.GetAllKeys();
	}

    // External changing the language ##########################################################################################################################################
    public void ChangeLanguage( Language language ) {

        Game.Language = language;

        language_manager.ChangeLanguage( GetDeviceCulture( language ) );
    }
}