using UnityEngine;

[RequireComponent( typeof( AudioSource ) )]
public class SoundEffects : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Звук включения какого-либо объекта" )]
    private AudioClip on_clip;
    public AudioClip On_clip { get { return on_clip; } }
    public void PlayOn() { if( on_clip != null ) audio_source_on_off.PlayOneShot( on_clip, Game.Sound_volume ); }

    [SerializeField]
    [Tooltip( "Звук выключения какого-либо объекта" )]
    private AudioClip off_clip;
    public AudioClip Off_clip { get { return off_clip; } }
    public void PlayOff() { if( off_clip != null ) audio_source_on_off.PlayOneShot( off_clip, Game.Sound_volume ); }

    [SerializeField]
    [Tooltip( "Набор звуков для реакции объекта (звучат при каждом вызове на выбор из этого перечня в случайном порядке)" )]
    private AudioClip[] effect_clips;

    private AudioSource audio_source_effects;
    public AudioSource Audio_source_effects { get { return audio_source_effects; } }

    private AudioSource audio_source_on_off;
    public AudioSource Audio_source_on_off { get { return audio_source_on_off; } }

    private GameObject effects_object;

    private Transform cached_transform;
    private Transform audio_effects_transform;

    // Override start ##########################################################################################################################################################
    void Start() {

        cached_transform = transform;

        // Проигрыватель для включения/выключения эффекта
        audio_source_on_off = GetComponent<AudioSource>();

        // Далее создаём проигрыватель для включения/выключения эффектов столкновеня с объектами
        effects_object = new GameObject( gameObject.name + "_audio_effects" );
        effects_object.transform.parent = cached_transform;

        audio_source_effects = effects_object.AddComponent<AudioSource>();
        audio_source_effects.playOnAwake = false;
        audio_source_effects.loop = false;

        audio_effects_transform = audio_source_effects.GetComponent<Transform>();
        audio_effects_transform.localPosition = Vector3.zero;

        effects_object.SetActive( true );
    }

    // Private show hit effect #################################################################################################################################################
    public void PlayEffect( ref Vector3 point ) {

        if( effects_object == null ) return;

        if( effect_clips.Length > 0 ) {

            if( !effects_object.activeInHierarchy ) effects_object.SetActive( true );

            if( !audio_source_effects.isPlaying ) {

                int index = Random.Range( 0, effect_clips.Length - 1 );
                audio_effects_transform.position = point;
                audio_source_effects.PlayOneShot( effect_clips[index], Game.Sound_volume );
            }
        }
    }
}