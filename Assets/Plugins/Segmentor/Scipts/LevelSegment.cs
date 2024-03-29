﻿/*
 * Author: Alice von Spades
 * Created: 22.11.2014
 * Last Edit: 25.11.14
 * Package: Segmentor
 */
using UnityEngine;
using System.Collections;

namespace Segmentor
{
  public class LevelSegment : MonoBehaviour
  {
    public const int UPPER_LEFT = -4;
    public const int UPPER = -3;
    public const int UPPER_RIGHT = -2;
    public const int LEFT = -1;
    public const int CENTER = 0;
    public const int RIGHT = 1;
    public const int LOWER_LEFT = 2;
    public const int LOWER = 3;
    public const int LOWER_RIGHT = 4;

    public const int INNER_LOWER_RIGHT = 13;
    public const int INNER_LOWER_LEFT = 12;
    public const int INNER_UPPER_RIGHT = 11;
    public const int INNER_UPPER_LEFT = 10;

    public const int HALL_VERTICAL = -1;
    public const int HALL_HORIZONTAL = -2;

    public const int HALL_CORNER_LOWER_LEFT = -3;
    public const int HALL_CORNER_LOWER_RIGHT = -4;
    public const int HALL_CORNER_UPPER_LEFT = -5;
    public const int HALL_CORNER_UPPER_RIGHT = -6;

    public const int HALL_T_UPPER = -7;
    public const int HALL_T_LOWER = -8;
    public const int HALL_T_RIGHT = -9;
    public const int HALL_T_LEFT = -10;

    public const int HALL_END_UPPER = -11;
    public const int HALL_END_LOWER = -12;
    public const int HALL_END_RIGHT = -13;
    public const int HALL_END_LEFT = -14; 

    public const int HALL_4 = -15;
    /*============
     * Public variables
     ===========*/
    public int type = -1;
    public int segment = 0;
    public int x;
    public int y;
    public Transform mesh;

    public bool bDisabled = false;

    public Level level;
    /*============
     * End of public variables
     ===========*/

    public void ChangeType(int type)
    {
      if (!bDisabled)
        {
          
          if (!level.bHall)
            this.type = type;
          else
            this.type = -type;

          level.map [x] [y] = this.type;
          //Debug.Log(level.calculatePoint(x, y));
          if (this.type > 0)
            changeColor(new Color(1.2f - Mathf.Abs(type * 0.1f), 0.5f + Mathf.Abs(type * 0.1f), 0));
          else if (this.type < 0)
            changeColor(new Color(0.7f - Mathf.Abs(type * 0.1f), 0.2f + Mathf.Abs(type * 0.1f), 0));
          else if (this.type == 0)
            changeColor(new Color(0.2f, 0.2f, 0.2f));
        }
    }

      public void changeColor(Color c)
      {
        Material tempMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
      
        tempMaterial.color = c;
        GetComponent<Renderer>().sharedMaterial = tempMaterial;
      }
  }
}
