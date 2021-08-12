using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LetterDefinition;
using System.Linq;

public class Draw : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;
    public GameObject lengthCounterText;
    float lineLength=0;
    float minDistanse = 0.6f;
    int theStrokeDrawingID = 0;

    [SerializeField] GameObject win_finall_window; 
    [SerializeField] GameObject try_again_window;

    [SerializeField] LineRenderer aLine;

    //An array which has strokes
    Stroke[] strokesArray;
    public List<Path> pathList=new List<Path>();

    //For the smallPoints

    bool startDraw = true;
    bool strokeFinised = false;

    //For the smallPoints
    List<Vector2>[] smallpointsList;
    public int currentPointIndex;
    public int nextPointIndex;

    LineRenderer currentLineRenderer;

   //a list which has the stroke length 
    List<float> pathLength = new List<float>();


    //A vector that has the last mouse position
    Vector2 lastPos;
    private void Start()
    {
        currentPointIndex=0;
        nextPointIndex=1;

        strokesArray = new Stroke[Main.noOfStrokes.Count];
        smallpointsList = new List<Vector2>[10];

        //Create a list with the length of each stroke
        foreach(Vector4 p in Main.globalpointsList)
        {
            if (p.z != 0 && p.z!=-1)
            {
                pathLength.Add(p.z);
            }
        }
        
        //Initialize objects
        ObjCreation();
        for (int i = 0; i < Main.noOfStrokes.Count; i++)
        {
            smallpointsList[i] = strokesArray[i].GetSmallPointsList();
        }
        DrawTheLetter();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) 
            && Vector2.Distance(m_camera.ScreenToWorldPoint(Input.mousePosition),smallpointsList[theStrokeDrawingID][0])<minDistanse)
        {
            startDraw = true;
        }
        if (startDraw)
        {
        Drawing();
        }
        lengthCounterText.GetComponent<Text>().text = lineLength.ToString();
        CheckAllTheKeyPoints();       
    }

    void Drawing()
    {
        Debug.Log("IsStrokeFinished() " + IsStrokeFinished());
        //If the mouse button is pressed for the first time
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastPos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            CreateBrush();
        }

        //When the mouse Button is down
        else if (Input.GetKey(KeyCode.Mouse0) && startDraw)
        {
            PointToMousePos();
        }

        //if all the strokes were completed successfully
        if (IsStrokeFinished() )
        {
            startDraw = false;
            currentLineRenderer = null;
            Debug.Log("theStrokeDrawingID= " + theStrokeDrawingID + "Main.noOfStrokes.Count " + Main.noOfStrokes.Count);
            if ((theStrokeDrawingID+1) < Main.noOfStrokes.Count)
            {
                theStrokeDrawingID++;
                currentPointIndex = 0;
                strokeFinised = false;
            }
            else
            {
                win_finall_window.SetActive(true);
            }
            ResetKeyPoints();
            lineLength = 0;
            //Destroy(currentLineRenderer);
            currentLineRenderer = null;
            //startDraw = true;
        }

        //if the stroke wasn't completed successfully -Fail
        else if(!strokeFinised && Input.GetKeyUp(KeyCode.Mouse0))
        {
            FailDaw();
        }
    }

    IEnumerator PrintAndWait()
    {
        
        try_again_window.SetActive(true);
        yield return new WaitForSeconds(1);
        try_again_window.SetActive(false);

    }

    //Brush creation
    void CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush,transform);

        currentLineRenderer = brushInstance.GetComponent < LineRenderer >();      

        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(mousePos, smallpointsList[theStrokeDrawingID][currentPointIndex]) < 0.5){
        currentLineRenderer.SetPosition(0, smallpointsList[theStrokeDrawingID][currentPointIndex]);
        currentLineRenderer.SetPosition(1, smallpointsList[theStrokeDrawingID][currentPointIndex]);
        }

    }

    //Add a new point to the Brush -Stroke
    void AddAPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    //Calculate the distance between the old mouse position and the new one
    void PointToMousePos()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        while ( Vector2.Distance(mousePos,GetTheNextPoint())< Vector2.Distance(mousePos, smallpointsList[theStrokeDrawingID][currentPointIndex]))
        {
            //if(currentPointIndex+1)
            currentPointIndex++;
            lineLength += Vector2.Distance(mousePos, lastPos); 
            AddAPoint(smallpointsList[theStrokeDrawingID][currentPointIndex]);
            lastPos = mousePos;
        }
        if(lastPos != mousePos 
            && Vector2.Distance(mousePos, smallpointsList[theStrokeDrawingID][currentPointIndex]) > 0.6)
        {
            FailDaw();
        }
    }

    //to get the length of the drawn stroke
    public float GetDistanseMouseToKey(KeyPoint aKeyPoint)
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        return Mathf.Sqrt(Mathf.Pow(mousePos.x - aKeyPoint.xCoord, 2) +
                Mathf.Pow(mousePos.y - aKeyPoint.yCoord, 2));
    }


    //Creation of the objects
    public void ObjCreation()
    {
        //Creation of Strokes objects

        for (int i = 0; i < Main.noOfStrokes.Count; i++)
        {
            //create the strokes
            strokesArray[i] = new Stroke( pathLength[i]);

            //add the key point of the strokes
            for(int j = 0; j < Main.globalpointsList.Count; j++)
            {
                if(Main.globalpointsList[j].w== (i + 1))
                {
                    //Path creation
                    if(Main.globalpointsList.Count>(j+1) && Main.globalpointsList[j+1].z==-1)
                    {

                        //add a Curve path
                        strokesArray[i].AddAPath(new Path(new KeyPoint(Main.globalpointsList[j].x,
                        Main.globalpointsList[j].y, j), 
                        new KeyPoint(Main.globalpointsList[j+2].x,
                        Main.globalpointsList[j+2].y, j+2),
                        new Vector2(Main.globalpointsList[j+1].x, Main.globalpointsList[j+1].y)));                        

                        strokesArray[i].AddAKeyPoint(new KeyPoint(Main.globalpointsList[j].x,
                            Main.globalpointsList[j].y, j));
                        strokesArray[i].AddAKeyPoint(new KeyPoint(Main.globalpointsList[j+2].x,
                            Main.globalpointsList[j+2].y, j + 2));

                        j += 1;
                    }
                    else 
                    {
                        //add a strait path
                        if (j < Main.globalpointsList.Count - 1)
                        {
                            if (Main.globalpointsList[j + 1].w == (i + 1))
                            {
                                strokesArray[i].AddAPath(new Path(new KeyPoint(Main.globalpointsList[j].x,
                                Main.globalpointsList[j].y, j),
                                new KeyPoint((int)Main.globalpointsList[j + 1].x,
                                Main.globalpointsList[j + 1].y, j + 1)));
                            }
                        }
                        strokesArray[i].AddAKeyPoint(new KeyPoint(Main.globalpointsList[j].x,
                        Main.globalpointsList[j].y, j));
                    }
                }
            }
        }
    }

    //change the bool value of a KeyPoint
    public void SetKeyBool(KeyPoint aKeyPoint)
    {
        //Check if the previous keypoints are true
        bool temp = true;
        for(int i = 0; i < strokesArray[theStrokeDrawingID].strokeKeyPointList.FindIndex(a=> a==aKeyPoint); i++)
        {
            temp = temp && strokesArray[theStrokeDrawingID].strokeKeyPointList[i].collider;
        }

        
        for (int i=0;i<strokesArray[theStrokeDrawingID].strokeKeyPointList.Count; i++)
        {
            Debug.Log(i.ToString() + strokesArray[theStrokeDrawingID].strokeKeyPointList[i].collider.ToString()
                + strokesArray[theStrokeDrawingID].maxStrokeLength+"count "+
                strokesArray[theStrokeDrawingID].strokeKeyPointList.Count);
        }
        

        // If the previous keypoints are true then set this KeyPoint true as well
        if (GetDistanseMouseToKey(aKeyPoint)<=minDistanse && temp && Input.GetKey(KeyCode.Mouse0))
        {
            aKeyPoint.SetKeyPointCollider(true);
        }

    }

    //Check all the keypoints of the stroke
    public void CheckAllTheKeyPoints()
    {
        for(int i=0; i<strokesArray[theStrokeDrawingID].strokeKeyPointList.Count; i++)
        {
            SetKeyBool(strokesArray[theStrokeDrawingID].strokeKeyPointList[i]);
        }

    }
    //Cheks if the stroke is finished
    public bool IsStrokeFinished()
    {

        //check if I passed from all the strokes keyPoints
        
        bool temp = true;
        for(int i=0; i< strokesArray[theStrokeDrawingID].strokeKeyPointList.Count; i++)
        {
            temp = temp && strokesArray[theStrokeDrawingID].strokeKeyPointList[i].collider;
        }

        //check if The length Is ok
        bool temp2 = false;
        if(lineLength < strokesArray[theStrokeDrawingID].maxStrokeLength)
        {
            temp2 = true;
        }

        Debug.Log("temp= " + temp+" startDraw= "+startDraw);
        return (temp && temp2);
    } 

    //Make false all the KeyPoints of the Stroke (in case the user fails ))    
    public void ResetKeyPoints()
    {
        for (int i = 0; i < strokesArray[theStrokeDrawingID].strokeKeyPointList.Count; i++)
        {
            strokesArray[theStrokeDrawingID].strokeKeyPointList[i].collider=false;
        }
    }


    public void FailDaw()
    {
        ResetKeyPoints();
        lineLength = 0;
        Destroy(currentLineRenderer);
        currentLineRenderer = null;
        startDraw = false;
        currentPointIndex = 0;
        StartCoroutine(PrintAndWait());
    }

    public Vector2 GetTheNextPoint()
    {
        if (smallpointsList[theStrokeDrawingID].Count >(currentPointIndex+1))
        {
            return smallpointsList[theStrokeDrawingID][currentPointIndex+1];
        }
        else
        {
            return smallpointsList[theStrokeDrawingID][currentPointIndex];
        }

    }
    public void DrawTheLetter()
    {
        for (int i = 0; i <= strokesArray.Length-1; i++)
        {
            LineRenderer line2 = Instantiate(aLine, transform.parent) as LineRenderer;

            //----------draw the letter------

            //How many key points are with this ID
            int IDcount = 0;
            for (int j = 0; j < smallpointsList[i].Count; j++)
            {
                IDcount++;
            }

            //A temporary array which has the number of strokes
            Vector3[] tempArr = new Vector3[IDcount];

            line2.SetWidth(0.8f, 0.8f);

            //the list of vector3 with the coordinates of a stroke
            List<Vector3[]> tempList = new List<Vector3[]>();

            //set the vertexes of the line
            line2.SetVertexCount(IDcount);
            //line2.startColor=new Color(162,255,251);

            //now draw the line 
            int arrayPossition = 0;
            foreach (Vector2 stroke in smallpointsList[i])
            {
                tempArr[arrayPossition] = new Vector3(stroke.x, stroke.y, 10);
                
                arrayPossition++;

            }
            line2.SetPositions(tempArr);
        }
    }

}



