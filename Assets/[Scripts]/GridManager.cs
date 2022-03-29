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
    public Difficulty difficulty;
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
    public float currentTime;
    private int minutes = 0;
    private int seconds = 0;

    void Awake()
    {
        currentTime = startingTime;
        instance = this;

        if (difficulty == Difficulty.EASY)
        {
            gridDimensions = new Vector2Int(6, 6);
            layoutGroup.cellSize = new Vector2(64f, 64f);
            layoutGroup.spacing = new Vector2(10f, 10f);
        }
        else if (difficulty == Difficulty.NORMAL)
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
                GridCharacter ch = Instantiate(gridCharacterPrefabs[(int)difficulty], transform.position, Quaternion.identity, transform);

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

                    if (random < 40)
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

        for(int i = 0; i < selectedCodes.Count; i++)
        {
            int randomColumn = Random.Range(0, gridDimensions.x - selectedCodes[i].Length);
            int randomRow = Random.Range(0, gridDimensions.y);

            for (int col = 0; col < selectedCodes[i].Length; col++)
            {
                grid[randomColumn + col, randomRow].GridChar = selectedCodes[i][col];
                grid[randomColumn + col, randomRow].transform.GetComponent<Image>().color = Color.red;
            }

            //wordLocations[i] = new Vector2Int(randomColumn, randomRow);
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
        if (difficulty == Difficulty.EASY)
        {
            int[] indexes = GetRandomIndexes(0, easyCodes.Count);

            foreach (int i in indexes)
                selectedCodes.Add(easyCodes[i]);
        }
        else if (difficulty == Difficulty.NORMAL)
        {
            int[] indexes = GetRandomIndexes(0, normalCodes.Count);

            foreach (int i in indexes)
                selectedCodes.Add(normalCodes[i]);
        }
        else if (difficulty == Difficulty.HARD)
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

        return new int[] { idx1, idx2, idx3 };
    }

    public void OnCodeEntered(string value)
    {
        value = value.ToUpper();
        for (int i = 0; i < selectedCodes.Count; i++)
        {
            if (selectedCodes[i] == value)
            {
                lockedStats[i].transform.parent.GetComponent<Image>().color = new Color(0f, 1f, 0f, 100f/255f);
                lockedStats[i].text = "[UNLOCKED]";

                //for(int j = 0; j < selectedCodes[i].Length; j++)
                //{
                //    grid[wordLocations[i].x + j, wordLocations[i].y].transform.GetComponent<Image>().color = Color.green;
                //}
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
        minutes = Mathf.FloorToInt(currentTime / 60);
        seconds = (int)currentTime % 60;
        string text = minutes + ":" + seconds.ToString("00");

        if (timeText.text != text)
            timeText.text = text;
    }
}
