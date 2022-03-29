using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridCharacter : MonoBehaviour
{
    [SerializeField] private char gridChar = ' ';
    [SerializeField] private TMP_Text gridCharacterText;


    public char GridChar
    {
        get { return gridChar; }
        set { gridChar = value; gridCharacterText.text = gridChar.ToString(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
