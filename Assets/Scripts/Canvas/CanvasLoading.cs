using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CanvasLoading : MonoBehaviour {

    [System.Serializable]
    private class LoadingPlanet {

        public LevelType level_type;
        public GameObject planet_prefab;
        public float starting_offset = 0f;
    }

    [SerializeField]
    [Tooltip( "Время имитации загрузки; по умолчанию = 3 секунды" )]
    private float loading_time = 3f;

    [SerializeField]
    [Tooltip( "Ссылка на базовый объект-контейнер, удаляющийся от камеры" )]
    private Transform running_transform;

    [SerializeField]
    [Tooltip( "Скорость удаления объекта от камеры; по умолчанию = 100" )]
    private float running_speed = 100f;

    [Space( 10 )]
    [SerializeField]
    [Tooltip( "Перечень объектов уровня: перед загрузкой определённого уровня данный объект будет удаляться от камеры, имитируя, что игрок покидает эту зону" )]
    private LoadingPlanet[] loading_planets;

    private GameObject planet;

    private Animator animator;
    private AsyncOperation async_operation_loading;

    private float animation_time = 0f;

    private bool is_mobile_platform = false;

	// Use this for initialization #############################################################################################################################################
    void Start() {

        Game.Current_level = LevelType.Level_Loading;

        animator = GetComponent<Animator>();

        // Имитируем разное время загрузки
        loading_time += Random.Range( 0f, (Game.Loading_level > LevelType.Level_Menu) ? 1f : -1f );

        // Находим соответствующий уровню префаб объекта и создаём удаляющийся объект
        for( int i = 0; i < loading_planets.Length; i++ ) {

            if( Game.Loading_level == loading_planets[i].level_type ) {

                planet = Instantiate( loading_planets[ (int) Game.Loading_level ].planet_prefab ) as GameObject;
                planet.transform.parent = running_transform;
                planet.transform.localPosition = new Vector3( 0f, 0f, loading_planets[i].starting_offset );
                break;
            }
        }

        #if UNITY_IOS
        is_mobile_platform = true;
        #elif UNITY_ANDROID
        is_mobile_platform = true;
        #elif UNITY_EDITOR
        StartCoroutine( Loading() );
        #elif UNITY_STANDALONE
        StartCoroutine( Loading() );
        #endif
    }

    // Loading a new level #####################################################################################################################################################
	IEnumerator Loading() {

        async_operation_loading = SceneManager.LoadSceneAsync( (int) Game.Loading_level );
        async_operation_loading.allowSceneActivation = false;

        yield return async_operation_loading;
    }
	
	// Update is called once per frame #########################################################################################################################################
	void Update() {

        loading_time -= Time.deltaTime;
        animation_time += Time.deltaTime;

        running_transform.Translate( 0f, 0f, running_speed * Time.deltaTime );

        // For Android, iOS patforms (don't works async loading)
        if( is_mobile_platform ) {

            if( loading_time < 0f ) animator.SetBool( "Loading_is_complete", true );
        }

        // For Windows platform (can use async loading)
        else {

            if( !async_operation_loading.isDone && (loading_time < 0f) ) animator.SetBool( "Loading_is_complete", true );
        }
	}

    // Event: loading is complete ##############################################################################################################################################
    public void EventAnimationLoadingIsComplete() {

        if( async_operation_loading != null ) {
        
            async_operation_loading.allowSceneActivation = true;
        }
    }
}
