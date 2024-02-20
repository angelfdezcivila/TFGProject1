using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StageParametersEvents : MonoBehaviour
{
    //En esta clase se van a crear los eventos (tal y como Dani Soto)
    // Lo primero que quiero implementar es que el input text field de filas solo escriba números enteros
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Edited()
    {
        Regex rgx = new Regex("[^0-9]");

        /* replace all non-numeric characters with an empty string */

        //stringToEditH sería el string recibido del input text field
        // stringToEditH = rgx.Replace(stringToEditH, "");
    }
}
