using UnityEngine;

public class EffectControl : MonoBehaviour {

    [System.Serializable]
    private class StartingEffect {

        public Effect effect;
        public int amount = 2;
    }

    [Tooltip( "Стартовый минимальный размер пула эффектов; по умолчанию = 100 (если размера не хватит, он увеличится автоматически)" )]
    [SerializeField]
    [Range( 100, 500 )]
    private int pool_size = 100;

    [Space( 10 )]
    [Tooltip( "Предварительно загружаемые в пул эффекты (правильно сбалансированная предварительная загрузка и порядок эффектов позволяют избежать рывков во время игры )" )]
    [SerializeField]
    StartingEffect[] starting_effects;

    [SerializeField]
    [HideInInspector]
    private int current_size = 0;

    private Effect[] effects_cache;

    private Transform cached_transform;

	// Use this for initialization #############################################################################################################################################
	void Start () {

        // Эффекты не используются на неигровых уровнях
        if( Game.Current_level <= LevelType.Level_Menu ) return;

        cached_transform = transform;

        // Создаём пул с адаптированным размером
        int min_size = 0;
        for( int i = 0; i < starting_effects.Length; i++ ) min_size += starting_effects[i].amount;
        pool_size = ((min_size * 2) > pool_size ) ? (min_size * 2) : pool_size;

        effects_cache = new Effect[ pool_size ];

        // Предварительно размещаем объекты в пуле в соответствии с указанными количествами для каждого эффекта
        for( int i = 0; i < starting_effects.Length; i++ ) {

            for( int j = 0; j < starting_effects[i].amount; j++ ) Show( starting_effects[i].effect, Vector3.zero, false, false );
        }
	}

    // Показывет эффект, если он есть в пуле, либо предварительно создаёт и показывает, если его нет (также служит для предварительного размещения эффектов в пул) #############
    public Effect Show( Effect effect, Vector3 effect_point, bool use_sound, bool activate = true ) {

        Effect cached_effect = null;

        if( effect == null ) return null;

        // Поиск эффекта в пуле (если только это не принудительное предварительное размещение в пуле)
        if( activate ) {

            for( int i = 0; i < current_size; i++ ) {

                if( effects_cache[i].Is_free && (effects_cache[i].ID == effect.ID) ) { cached_effect = effects_cache[i]; break; }
            }
        }

        // Если эффект найден в пуле, активизируем его (если это принудительное предварительное размещение, в эту ветку код не попадёт)
        if( cached_effect != null ) {

            // Модифицируем текущее значение гравитации для найденного эффекта (гравитация могла измениться с момента его последнего использования)
            if( cached_effect.Use_gravity_modifier ) cached_effect.Particle_system.gravityModifier = Mathf.Abs( UnityEngine.Physics.gravity.y * 0.1f );

            cached_effect.Cached_transform.position = effect_point;
            cached_effect.gameObject.SetActive( true );

            if( use_sound && (cached_effect.Audio_source != null) ) cached_effect.Audio_source.Play();
        }

        // Если эффекта нет в пуле, либо это принудительное предварительное размещение в пул, создаём его и помещаем в пул
        else {

            // Если эффект предварительно помещается в пул, сразу же деактивируем его после создания
            GameObject instance = Instantiate( effect.gameObject, effect_point, Quaternion.identity ) as GameObject;
            instance.transform.parent = cached_transform;
            instance.SetActive( activate );

            cached_effect = instance.GetComponent<Effect>();

            // Модифицируем текущее значение гравитации для эффекта
            if( activate && cached_effect.Use_gravity_modifier ) cached_effect.Particle_system.gravityModifier = Mathf.Abs( UnityEngine.Physics.gravity.y * 0.1f );

            // Если пула не хватило, увеличиваем его рамер (по умолчанию в два раза)
            if( current_size < effects_cache.Length ) effects_cache[ current_size++ ] = cached_effect;
            else ResizePoolAndPlaceEffect( cached_effect );

            if( activate && use_sound && (cached_effect.Audio_source != null) ) cached_effect.Audio_source.Play();
        }

        return cached_effect;
    }

    // Увеличивает размеры пула эффектов и помещает туда новый эффект ##########################################################################################################
    private void ResizePoolAndPlaceEffect( Effect effect, float pool_size_rate = 2f ) {

        if( pool_size_rate <= 1f ) pool_size_rate = 2f;

        Effect[] new_pool = new Effect[ Mathf.FloorToInt( ((float) pool_size) * pool_size_rate ) ];

        for( int i = 0; i < effects_cache.Length; i++ ) new_pool[i] = effects_cache[i];

        effects_cache = new_pool;

        effects_cache[ current_size++ ] = effect;
    }

    // Assign ID ###############################################################################################################################################################
    [ContextMenu( "Назначить новые ID всем эффектам в сцене" )]
    private void AssignEffectsID() {

        Effect[] effects = (Effect[]) GameObject.FindObjectsOfType( typeof( Effect ) );

        for( int i = 0; i < effects.Length; i++ ) effects[i].SetID( i + 1 );

        #if UNITY_EDITOR
        if( !Application.isPlaying ) {

            Debug.Log( "Предупреждение: " + effects.Length + " эффектов в сцене получили новые ID." );
            if( effects.Length > 0 ) Debug.Log( "Необходимо переписать префабы изменённых эффектов." );
        }
        #endif
    }
}
