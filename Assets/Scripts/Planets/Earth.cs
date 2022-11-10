using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {

    [System.Serializable]
    private class EarthSettings {

        [Range( 0.0f, 3.0f )]
        public float detail_intensity = 0.0f;

        [Range( 0.0f, 8.0f )]
        public float specular_power = 3.0f;

        [Range( 0.0f, 1.0f )]
        public float night_detail_intensity = 1.0f;

        [Range( 1.0f, 64.0f )]
        public float night_transition_variable = 1.0f;

        [Range( 0.0f, 1.0f )]
        public float smoothness = 0.25f;

        [Range( 0.5f, 64.0f )]
        public float rim_power;

        [Range( 0.1f, 64.0f )]
        public float atmosphere_falloff = 2.5f;

        public Color rim_color;

        public Color atmosphere_near_color;

        public Color atmosphere_far_color;

        public Color clouds_color;
    }

    [SerializeField]
    private Transform sun_orbit;

    [SerializeField]
    [Tooltip( "Основной слой облаков: меняет цвет в зависимости от времени суток" )]
    private MeshRenderer sky;

    [SerializeField]
    [Tooltip( "Дополнительный слой облаков: меняет цвет в зависимости от времени суток" )]
    private MeshRenderer clouds;

    [Range( 0.1f, 10.0f )]
    public float check_time = 5f;

    [Header( "SETTINGS AT THE ANGLE OF RAYS OF THE SUN OF 0 DEGREES" )]
    [SerializeField]
    private EarthSettings day;

    [Header( "SETTINGS AT THE ANGLE OF RAYS OF THE SUN OF 180 DEGREES" )]
    [SerializeField]
    private EarthSettings night;

    private Material earth_material;
    private Material clouds_material;
    private Material sky_material;

    private int
        ID_detail_intensity,
        ID_specular_power,
        ID_night_detail_intensity,
        ID_night_transition_variable,
        ID_smoothness,
        ID_rim_power,
        ID_atmosphere_falloff,
        ID_rim_color,
        ID_atmosphere_near_color,
        ID_atmosphere_far_color,
        ID_clouds_emission_color;

    private WaitForSeconds earth_wait_for_seconds;

    private const float angle_180_inversed = 1f / 180f;
    private const float angle_360_inversed = 1f / 360f;

    private float sun_angle = 0f;
    private float sun_rotation_rate = 0f;

    // Use this for initialization #############################################################################################################################################
	void Start () {

        // Если планета используется не в самой игре, а только для иллюстрации во врем загрузки
        if( Game.Current_level == LevelType.Level_Loading ) return;

        earth_material = GetComponent<MeshRenderer>().material;
        clouds_material = clouds.GetComponent<MeshRenderer>().material;
        if( sky != null ) sky_material = sky.GetComponent<MeshRenderer>().material;

        ID_detail_intensity = Shader.PropertyToID( "_DetailIntensity" );
        ID_specular_power = Shader.PropertyToID( "_SpecularPower" );
        ID_night_detail_intensity = Shader.PropertyToID( "_NightIntensity" );
        ID_night_transition_variable = Shader.PropertyToID( "_NightTransitionVariable" );
        ID_smoothness = Shader.PropertyToID( "_Smoothness" );
        ID_rim_power = Shader.PropertyToID( "_RimPower" );
        ID_atmosphere_falloff = Shader.PropertyToID( "_AtmosFalloff" );
        ID_rim_color = Shader.PropertyToID( "_RimColor" );
        ID_atmosphere_near_color = Shader.PropertyToID( "_AtmosNear" );
        ID_atmosphere_far_color = Shader.PropertyToID( "_AtmosFar" );
        ID_clouds_emission_color = Shader.PropertyToID( "_EmissionColor" );

        if( sky != null ) sky.gameObject.transform.Rotate( Random.Range( 0f, 360f ), Random.Range( 0f, 360f ), Random.Range( 0f, 360f ) );
        if( clouds != null ) clouds.gameObject.transform.Rotate( Random.Range( 0f, 360f ), Random.Range( 0f, 360f ), Random.Range( 0f, 360f ) );

        SetPlanetIllumination();

        earth_wait_for_seconds = new WaitForSeconds( check_time );
        StartCoroutine( UpdateEarthSettings() );
	}
	
	// Update of the Earth settings ############################################################################################################################################
	IEnumerator UpdateEarthSettings() {

        while( !Game.Is( GameState.Complete ) ) {

            SetPlanetIllumination();

            yield return earth_wait_for_seconds;
        }
	
        yield break;
	}

    // Регулирует настройки света и атмосферы у планеты ########################################################################################################################
    private void SetPlanetIllumination() {

        if( sun_orbit.localEulerAngles.y == 180f ) sun_rotation_rate = 1f;
        else sun_rotation_rate = Mathf.Abs( (sun_orbit.localEulerAngles.y % 180f) * angle_180_inversed );
        if( (sun_orbit.localEulerAngles.y % 360f) > 180f ) sun_rotation_rate = 1f - sun_rotation_rate;

        earth_material.SetFloat( ID_detail_intensity, Mathf.Lerp( night.detail_intensity, day.detail_intensity, sun_rotation_rate ) );
        earth_material.SetFloat( ID_specular_power, Mathf.Lerp( night.specular_power, day.specular_power, sun_rotation_rate ) );
        earth_material.SetFloat( ID_night_detail_intensity, Mathf.Lerp( night.night_detail_intensity, day.night_detail_intensity, sun_rotation_rate ) );
        earth_material.SetFloat( ID_night_transition_variable, Mathf.Lerp( night.night_transition_variable, day.night_transition_variable, sun_rotation_rate ) );
        earth_material.SetFloat( ID_smoothness, Mathf.Lerp( night.smoothness, day.smoothness, sun_rotation_rate ) );
        earth_material.SetFloat( ID_rim_power, Mathf.Lerp( night.rim_power, day.rim_power, sun_rotation_rate ) );
        earth_material.SetFloat( ID_atmosphere_falloff, Mathf.Lerp( night.atmosphere_falloff, day.atmosphere_falloff, sun_rotation_rate ) );
        earth_material.SetColor( ID_rim_color, Color.Lerp( night.rim_color, day.rim_color, sun_rotation_rate ) );
        earth_material.SetColor( ID_atmosphere_near_color, Color.Lerp( night.atmosphere_near_color, day.atmosphere_near_color, sun_rotation_rate ) );
        earth_material.SetColor( ID_atmosphere_far_color, Color.Lerp( night.atmosphere_far_color, day.atmosphere_far_color, sun_rotation_rate ) );

        clouds_material.SetColor( ID_clouds_emission_color, Color.Lerp( night.clouds_color, day.clouds_color, sun_rotation_rate ) );
        if( sky != null ) sky_material.SetColor( ID_clouds_emission_color, Color.Lerp( night.clouds_color, day.clouds_color, sun_rotation_rate ) );
    }
}
