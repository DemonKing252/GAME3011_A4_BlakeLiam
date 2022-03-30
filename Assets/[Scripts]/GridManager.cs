using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Text;

public enum Difficulty
{
    EASY,
    NORMAL,
    HARD
}

public class GridManager : MonoBehaviour
{

    [SerializeField] private Vector2Int gridDimensions;
    [SerializeField] private GridCharacter[] gridCharacterPrefabs;
    [SerializeField] private GridLayoutGroup layoutGroup;

    private GridCharacter[,] grid;

    private GridManager instance;
    public GridManager Instance { get { return instance; } }

    [SerializeField] private List<string> easyCodes;
    [SerializeField] private List<string> normalCodes;
    [SerializeField] private List<string> hardCodes;

    public List<string> selectedCodes;

    [SerializeField] private TMP_Text[] codeTexts;
    [SerializeField] private TMP_Text[] lockedStats;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Vector2Int[] wordLocations = new Vector2Int[3];

    [SerializeField] private float startingTime = 180f;
    [SerializeField] private TMP_InputField inputFieldTMP;

    public float currentTime;
    private int minutes = 0;
    private int seconds = 0;
    private int codesUnlocked = 0;
    
    [SerializeField] private int maxCodes = 3;

    void Awake()
    {
        currentTime = startingTime;
        instance = this;

        if (GameUtil.difficulty == Difficulty.EASY)
        {
            gridDimensions = new Vector2Int(6, 6);
            layoutGroup.cellSize = new Vector2(64f, 64f);
            layoutGroup.spacing = new Vector2(10f, 10f);
        }
        else if (GameUtil.difficulty == Difficulty.NORMAL)
        {
            gridDimensions = new Vector2Int(12, 12);
            layoutGroup.cellSize = new Vector2(32f, 32f);
            layoutGroup.spacing = new Vector2(5f, 5f);
        }
        else
        {
            gridDimensions = new Vector2Int(24, 24);
            layoutGroup.cellSize = new Vector2(16f, 16f);
            layoutGroup.spacing = new Vector2(2.2f, 2.2f);
        }

        grid = new GridCharacter[gridDimensions.x, gridDimensions.y];
        for (int r = 0; r < gridDimensions.y; r++)
        {
            for (int c = 0; c < gridDimensions.x; c++)
            {
                GridCharacter ch = Instantiate(gridCharacterPrefabs[(int)GameUtil.difficulty], transform.position, Quaternion.identity, transform);

                int randomASCII = Random.Range(64, 91);
                randomASCII = (randomASCII == 64 ? 95 : randomASCII);
                ch.GridChar = (char)(randomASCII);

                grid[c, r] = ch;
            }
        }
        LoadCodes();
        ChooseRandomCodes();

        try
        {
            for (int i = 0; i < codeTexts.Length; i++)
            {
                StringBuilder code = new StringBuilder(selectedCodes[i]);

                for (int j = 0; j < code.Length; j++)
                {
                    int random = Random.Range(1, 101);

                    // At player skill of 100, theres an 10% chance for each character to be censored out
                    // At player skill of 0, theres an 80% chance for each character to be censored out 
                    float skill = Mathf.Lerp(80f, 10f, GameUtil.playerSkill / 100f);
                    if (random < skill)
                    {
                        code[j] = '*';
                    }
                }

                codeTexts[i].text = code.ToString();
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("Code text count does not match selected code count [Exp Msg: " + e.Message + "]");
        }

        int randomColumn = Random.Range(0, gridDimensions.x - selectedCodes[0].Length);
        int randomRow1 = 0, randomRow2 = 1, randomRow3 = 2;
        randomRow1 = Random.Range(0, gridDimensions.y);
        randomColumn = 0;
        wordLocations[0] = new Vector2Int(randomColumn, randomRow1);

        for (int col = 0; col < selectedCodes[0].Length; col++)
        {
            grid[randomColumn + col, randomRow1].GridChar = selectedCodes[0][col];
        }

        do
        {
            randomColumn = Random.Range(0, gridDimensions.x - selectedCodes[1].Length);
            randomRow2 = Random.Range(0, gridDimensions.y);
        } while (randomRow2 == randomRow1);
        wordLocations[1] = new Vector2Int(randomColumn, randomRow2);


        for (int col = 0; col < selectedCodes[1].Length; col++)
        {
            grid[randomColumn + col, randomRow2].GridChar = selectedCodes[1][col];
        }
        
        do
        {
            randomColumn = Random.Range(0, gridDimensions.x - selectedCodes[2].Length);
            randomRow3 = Random.Range(0, gridDimensions.y);     
        } while (randomRow3 == randomRow2 || randomRow3 == randomRow1);
        wordLocations[2] = new Vector2Int(randomColumn, randomRow3);

        for (int col = 0; col < selectedCodes[2].Length; col++)
        {
            grid[randomColumn + col, randomRow3].GridChar = selectedCodes[2][col];
        }

    }
    private void LoadCodes()
    {
        string path = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Codes.txt";
        StreamReader sr = new StreamReader(path);

        string code;
        while ((code = sr.ReadLine()) != null)
        {
            string[] csv = code.Split('-');
            int difficulty = csv[0] switch
            {
                "Easy"   => 0,
                "Normal" => 1,
                "Hard"   => 2,
            };

            if (difficulty == 0)
                easyCodes.Add(csv[1]);
            else if (difficulty == 1)
                normalCodes.Add(csv[1]);
            else if (difficulty == 2)
                hardCodes.Add(csv[1]);
        }
    }
    private void ChooseRandomCodes()
    {
        if (GameUtil.difficulty == Difficulty.EASY)
        {
            int[] indexes = GetRandomIndexes(0, easyCodes.Count);

            foreach (int i in indexes)
                selectedCodes.Add(easyCodes[i]);
        }
        else if (GameUtil.difficulty == Difficulty.NORMAL)
        {
            int[] indexes = GetRandomIndexes(0, normalCodes.Count);

            foreach (int i in indexes)
                selectedCodes.Add(normalCodes[i]);
        }
        else if (GameUtil.difficulty == Difficulty.HARD)
        {
            int[] indexes = GetRandomIndexes(0, hardCodes.Count);

            foreach (int i in indexes)
                selectedCodes.Add(hardCodes[i]);
        }
    }
    private int[] GetRandomIndexes(int min, int max)
    {
        int idx1 = Random.Range(min, max);
        int idx2 = Random.Range(min, max);
        int idx3 = Random.Range(min, max);


        do
        {
            idx2 = Random.Range(min, max);
        } while (idx1 == idx2);
        do
        {
            idx3 = Random.Range(min, max);
        } while (idx3 == idx1 || idx3 == idx2);

        Debug.Log(idx1 + " " + idx2 + " " + idx3);
        return new int[] { idx1, idx2, idx3 };
    }

    public void OnCodeEntered(string value)
    {
        inputFieldTMP.text = string.Empty;
        value = value.ToUpper();
        for (int i = 0; i < selectedCodes.Count; i++)
        {
            if (selectedCodes[i] == value)
            {
                lockedStats[i].transform.parent.GetComponent<Image>().color = new Color(0f, 1f, 0f, 100f/255f);
                lockedStats[i].text = "[UNLOCKED]";

                for(int j = 0; j < selectedCodes[i].Length; j++)
                {
                    grid[wordLocations[i].x + j, wordLocations[i].y].transform.GetComponent<Image>().color = Color.green;
                }
                codesUnlocked++;
                if (codesUnlocked >= 3)
                {
                    MenuController.Instance.OnAction((int)Action.Victory);
                }

                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, Mathf.Infinity);
        if (currentTime <= 0f)
        {
            MenuController.Instance.OnAction((int)Action.Defeat);
        }

        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = (int)currentTime % 60;
        string text = minutes + ":" + seconds.ToString("00");

        if (timeText.text != text)
            timeText.text = text;
    }
}
