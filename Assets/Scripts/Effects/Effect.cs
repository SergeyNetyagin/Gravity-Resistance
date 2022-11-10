using UnityEngine;
using System.Collections;

[System.Serializable]
public class Effect : MonoBehaviour {

    [Tooltip( "Использовать ли гравитационный фактор (для систем частиц) во время воспроизведения эффекта" )]
    [SerializeField]
    private bool use_gravity_modifier = true;
    public bool Use_gravity_modifier { get { return use_gravity_modifier && (particle_system != null); } }

    [Tooltip( "Уникальный идентификатор эффекта, позволяющий помещать его в пул эффектов и находить его там впоследствии, чтобы не создавать заново" )]
    [SerializeField]
    private int iD = 0;
    public int ID { get {  return iD; } }
    public void SetID( int iD ) { this.iD = iD; }

    [Tooltip( "Длительность эффекта: если эффект не основан на системе частиц, используется это значение; иначе длительность эффекта равна ParticleSystem.Duration" )]
    [SerializeField]
    private float duration = 1f;

    private bool is_free = true;
    public bool Is_free { get { return is_free; } }

    private WaitForSeconds hide_time_wait_for_seconds;

    private Transform cached_transform;
    public Transform Cached_transform { get { return cached_transform; } }

    private AudioSource audio_source;
    public AudioSource Audio_source { get { return audio_source; } }

    private ParticleSystem particle_system;
    public ParticleSystem Particle_system { get { return particle_system; } }
    public bool Has_particle_system { get { return (particle_system != null); } }

    // Awake ###################################################################################################################################################################
    void Awake() {

        cached_transform = transform;

        audio_source = GetComponent<AudioSource>();
        particle_system = GetComponent<ParticleSystem>();

        if( particle_system != null ) hide_time_wait_for_seconds = new WaitForSeconds( particle_system.duration );
        else hide_time_wait_for_seconds = new WaitForSeconds( duration );
    }

    // Calculate hide time #####################################################################################################################################################
	void OnEnable() {

        is_free = false;

        if( (audio_source != null) && (Game.Sound_volume == 0f) ) audio_source.mute = true;
        else if( audio_source != null ) audio_source.mute = false;

        if( particle_system != null ) particle_system.Play( true );

        StartCoroutine( DeactivateAfterTime() );
	}

    // Deactivate effect #######################################################################################################################################################
    void OnDisable() {

        is_free = true;

        if( audio_source != null ) audio_source.Stop();
        if( particle_system != null ) particle_system.Stop( true );
    }

    // Check a hide time #######################################################################################################################################################
    IEnumerator DeactivateAfterTime() {

        yield return hide_time_wait_for_seconds;

        if( audio_source != null ) audio_source.time = 0f;

        gameObject.SetActive( false );

        yield break;
    }
}
