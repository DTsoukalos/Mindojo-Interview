using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LetterDefinition
{
    //Object classes
    //stroke class
    public class Stroke : ScriptableObject
    {        
        public bool isDone;
        public bool isCurve;

        public List<KeyPoint> strokeKeyPointList=new List<KeyPoint>();
        public List<Vector2> smallPointsList = new List<Vector2>();
        public List<Path> pathsList = new List<Path>();
        public float maxStrokeLength;

        public Stroke( float maxLength)
        {
            //pathsList = aList;
            this.isDone = false;
            this.maxStrokeLength = maxLength;
        }

        //add a path
        public void AddAPath(Path aPath)
        {
            pathsList.Add(aPath);
        }

        //getSmallPointsCoordinates
        public List<Vector2> GetSmallPointsList()
        {
            smallPointsList = new List<Vector2>();
            for (int i = 0; i < pathsList.Count; i++)
            {
                List<Vector2> newTempList = new List<Vector2>();
                newTempList = pathsList[i].GetDividedPoints();

                for (int j = 0; j < newTempList.Count; j++)
                {
                    smallPointsList.Add(newTempList[j]);
                }
            }
            return smallPointsList;
        }

        //check the keyPoints
        public bool StrokeContition()
        {
            bool temp=true;
            for (int i=0; i < strokeKeyPointList.Count; i++)
            {
                temp =temp && strokeKeyPointList[i].GetPointCollider();
            }
            return temp;
        }

        public void AddAKeyPoint(KeyPoint aKey)
        {
            strokeKeyPointList.Add(aKey);
        }

        public KeyPoint GetKeyPoint(int index)
        {
            return strokeKeyPointList[index];
        }
    }


    //class for the connection between 2 key points 
    public class Path
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        int resolution=10;

        public Path(KeyPoint a, KeyPoint b)
        {
            this.a = new Vector2(a.xCoord,a.yCoord);
            this.b = new Vector2(b.xCoord,b.yCoord);
        }

        public Path(KeyPoint a, KeyPoint b, Vector2 c)
        {
            this.a = new Vector2(a.xCoord, a.yCoord);
            this.b = new Vector2(b.xCoord, b.yCoord);
            this.c = c;
        }

        //all the small points of the path
        public List<Vector2> GetDividedPoints()
        {

            List<Vector2> newVector=new List<Vector2>();
            float length = Vector2.Distance(a,b);
            if (this.c== new Vector2(0,0))
            {
                float t;
                for(int i = 0; i < (int)length * resolution; i++)
                {
                    t = i*1.0f / (length * resolution);
                    newVector.Add(  a + (b - a) * t);
                }

            }
            else
            {
                float newLength = length + (Vector2.Distance(a, c) + Vector2.Distance(c, b)) / 2;
                //newVector = new Vector2[(int)newLength * resolution];//a+(b-a)t
                int points = (int)newLength * resolution;
                float t = 0;
                for (int i = 0; i < points; i++) { 
                    t=i * 1.0f / points;
                    Vector2 temp1 = Vector2.Lerp(a, c, t);
                    Vector2 temp2 = Vector2.Lerp(c, b, t);
                    newVector.Add( Vector2.Lerp(temp1, temp2, t));
                }
            }
            return newVector;
        }  
    }


    // the KeyPoints
    public class KeyPoint: ScriptableObject
    {
        public float xCoord;
        public float yCoord;
        public bool collider;
        //private int id;
        public int index;


        public KeyPoint(float xCoord, float yCoord, int aIndex)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.collider = false;
            //this.id = aId;
            this.index = aIndex;
        }
        public float GetKeyPointx()
        {
            return this.xCoord;
        }

        public float GetKeyPointy()
        {
            return this.yCoord;
        }

        public void SetKeyPointCollider(bool aBool)
        {
            this.collider = aBool;
        }

        public bool GetPointCollider()
        {
            return this.collider;
        }

        public int GetIndex()
        {
            return this.index;
        }

    }

    public class Letter
    {
        List<Stroke> listOfStrokes = new List<Stroke>();

        public bool IsLetterFinished()
        {
            bool temp = listOfStrokes[0].isDone;
            if (listOfStrokes.Count >= 1)
            {
                for (int i = 1; i < listOfStrokes.Count; i++)
                {
                    temp = temp && listOfStrokes[i].isDone;
                }
            }
            return temp;
        }
    }
}
