using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public enum Action
{
    Start      = 0,
    Quit       = 1,
    PlayAgain  = 2,
    MainMenu   = 3,
    Victory    = 4,
    Defeat     = 5
}
public class MenuController : MonoBehaviour
{
    private GridManager gridMgr;
    [SerializeField] private TMP_Text playerSkillText;

    [SerializeField] Canvas gameCanvas;
    [SerializeField] Canvas menuCanvas;
    [SerializeField] Canvas loseCanvas;
    [SerializeField] Canvas winCanvas;

    private static MenuController instance;
    public static MenuController Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        instance = this;
        gridMgr = FindObjectOfType<GridManager>();    
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        OnAction(3);
    }
    public void OnAction(int action)
    {
        switch ((Action)action)
        {
            case Action.Quit:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
                break;
            case Action.MainMenu:
                gameCanvas.gameObject.SetActive(false);
                menuCanvas.gameObject.SetActive(true);
                loseCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(false);
                Time.timeScale = 0f;
                break;
            case Action.Start:
                gameCanvas.gameObject.SetActive(true);
                menuCanvas.gameObject.SetActive(false);
                loseCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(false);
                Time.timeScale = 1f;

                break;
            case Action.Victory:
                gameCanvas.gameObject.SetActive(true);
                menuCanvas.gameObject.SetActive(false);
                loseCanvas.gameObject.SetActive(false);
                winCanvas.gameObject.SetActive(true);
                Time.timeScale = 0f;
                break;
            case Action.Defeat:
                gameCanvas.gameObject.SetActive(true);
                menuCanvas.gameObject.SetActive(false);
                loseCanvas.gameObject.SetActive(true);
                winCanvas.gameObject.SetActive(false);
                Time.timeScale = 0f;
                break;
            case Action.PlayAgain:
                SceneManager.LoadScene("SampleScene");

                break;
        }

    }

    //public void SetDescriptionText(string text)
    //{
    //    descriptionText.text = text;
    //}
    public void OnPlayerSkill(float value)
    {
        GameUtil.playerSkill = value;
        playerSkillText.text = "Player Skill (" + (int)value + ")";
    }

    public void OnDifficultyDropDown(int val)
    {
        GameUtil.difficulty = (Difficulty)val;
        //string desc = (Difficulty)val switch
        //{
        //    Difficulty.Begginer       => "Get 20x matches in 5 minutes!\n<size=60%>Get atleast 20 matches. Your not punished by miss clicking</size>",
        //    Difficulty.Intermediate   => "Get a 30x chain in 5 minutes!\n<size=60%>In order to get and MAINTAIN a chain, you need to get multiple matches in a row. If you make a move and theres no matches, your chain is reset</size>",   
        //    Difficulty.Advanced       => "Destroy 4 gems in one match in 5 minutes!\n<size=60%>You have to get a match of 5 of any type of gem. Keep in mind though, you have to match 4 gems in one match, not one move, cascades don't count for this challenge</size>",   
        //    Difficulty.Expert         => "Get a 3x cascade in 5 minutes!\n<size=60%>This one WILL be difficult I can assure you. Getting a cascade requires you to get MULTIPLE matches in ONE move!. For example, getting 3 matches in one move would be a x3 cascade!</size>",   
        //};
        //SetDescriptionText(desc);
    }

}
