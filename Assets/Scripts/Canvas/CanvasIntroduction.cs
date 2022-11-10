using UnityEngine;
using UnityEngine.SceneManagement;

// Начальная интродукция: можно отключать, если игра запущена не впервые
public class CanvasIntroduction : MonoBehaviour {

    [SerializeField]
    [Tooltip( "Если необходимо, чтобы при повторном запуске игры интродукция не проигрывалась, нужно установить <false>: тогда если в сохранениях есть флаг использования игры, интродукция пропускается" )]
    private bool use_introduction = true;

    private Animator animator;

	// Use this for initialization #############################################################################################################################################
	void Start() {

        animator = GetComponent<Animator>() as Animator;

        if( use_introduction ) animator.enabled = true;
        else if( PlayerPrefs.HasKey( "GRAVITY RESISTANCE" ) ) animator.enabled = false;
        else animator.enabled = true;

        if( !animator.enabled ) AnimationEventIntroductionComplete();
	}

    // Event ###################################################################################################################################################################
    public void AnimationEventIntroductionComplete() {

        if( Game.Use_hero ) Game.Loading_level = LevelType.Level_Hero;
        else Game.Loading_level = LevelType.Level_Menu;

        SceneManager.LoadScene( (int) LevelType.Level_Loading, LoadSceneMode.Single );
    }
}