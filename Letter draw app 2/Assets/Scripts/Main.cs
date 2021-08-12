using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LetterDefinition;

public class Main : MonoBehaviour
{


    //Instatiate the x-y of each point and the max length (instead of x pixels -1 for curve) and the path i
    [SerializeField]public List<Vector4> strokesList = new List<Vector4>();
    public static List<Vector4> globalpointsList = new List<Vector4>();

    public static HashSet<int> noOfStrokes = new HashSet<int>();


    [SerializeField] LineRenderer aLine;
    [SerializeField] GameObject keyPoint;

    private void Awake()
    {
        //for use of the points on other classes
        globalpointsList = strokesList;
    }

    // ---------Draw the Strokes -----------
    void Start()
    {


        //a list of ID strokes
        for(int i=0; i < strokesList.Count; i++)
        {
            noOfStrokes.Add((int)strokesList[i].w);
        }




        //----------draw the key points-----
        int curvpoints = 0;
        for (int i = 0; i < strokesList.Count; i++)
        {
            if (strokesList[i].z != -1)
            {
                Vector3 tempPosition = strokesList[i];
                GameObject aKeyPoint = Instantiate(keyPoint, transform.parent);
                aKeyPoint.GetComponent<RectTransform>().position = tempPosition;
                aKeyPoint.GetComponentInChildren<Text>().text = (i + 1 - curvpoints).ToString();
            }
            else
            {
                curvpoints++;
            }

        }
    }
}
