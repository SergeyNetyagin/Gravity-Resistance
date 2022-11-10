using UnityEngine;
using UnityEngine.SceneManagement;

// Организует многостраничный сюжетный рассказ
public class CanvasNovell : MonoBehaviour {

    [SerializeField]
    private bool use_introduction = false;

    [SerializeField]
    private GameObject panel_introduction;

    private Animator animator;

    private Transform[] introduction_pages;

    private int total_pages = 0;
    private int current_page = 0;

	// Use this for initialization #############################################################################################################################################
	void Start () {

        animator = GetComponent<Animator>() as Animator;
        animator.enabled = true;

        if( use_introduction ) InitializeIntroduction();
	}

    // Run the introduction's pages ############################################################################################################################################
    void RunIntroductionPages() {

        introduction_pages[ current_page ].gameObject.SetActive( true );
    }

    // Initialization of introduction ##########################################################################################################################################
    void InitializeIntroduction() {

        panel_introduction.SetActive( true );

        total_pages = panel_introduction.transform.childCount;
        introduction_pages = new Transform[ total_pages ];

        // Activate the first page of the brief
        for( int i = 0; i < total_pages; i++ ) {

            introduction_pages[i] = panel_introduction.transform.GetChild( i );
            introduction_pages[i].gameObject.SetActive( false );
        }
	}

    // Event: fade in ##########################################################################################################################################################
    public void EventAnimationFadeIn() {

        if( use_introduction ) {

            animator.SetInteger( "Introduction_stage", 1 );
            RunIntroductionPages();
        }

        else EventAnimationIntroductionComplete();
    }
    
    // Event: brief page tap the screen pressed ################################################################################################################################
    public void EventButtonShowIntroductionPressed() {

        // Deactivate current brief's page
        introduction_pages[ current_page++ ].gameObject.SetActive( false );

        // Actiavte a next brief's page, if it accessible
        if( current_page < total_pages ) introduction_pages[ current_page ].gameObject.SetActive( true );

        // Else close a brief's pages and go play game
        else animator.SetInteger( "Introduction_stage", 2 );
    }
   
    // Animation event for loading a game level ################################################################################################################################
    public void EventAnimationIntroductionComplete() {

        animator.enabled = false;

        Game.Current_level = LevelType.Level_Menu;

        SceneManager.LoadScene( (int) LevelType.Level_Loading );
    }
}