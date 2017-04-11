using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    Vector3 originalVector, differenceVector, totalTranslation;
    bool isDragging = false;
    float constrictX = 8.0f, constrictPY = 4.0f, constrictNY = -2.0f;

    // Use this for initialization
    void Start()
    {
        totalTranslation = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            differenceVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (isDragging == false)
            {
                isDragging = true;
                originalVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            isDragging = false;
        }

        if (isDragging && CheckConstraints(true))
        {
            totalTranslation.x = originalVector.x - differenceVector.x;
        }
        if (isDragging && CheckConstraints(false))
        {
            totalTranslation.y = originalVector.y - differenceVector.y;
        }
        Camera.main.transform.position = totalTranslation;
    }

    bool CheckConstraints(bool checkX)
    {
        if (checkX)
        {
            if (originalVector.x - differenceVector.x > constrictX || originalVector.x - differenceVector.x < -constrictX)
            {
                return false;
            }
        }
        else
        {
            if (originalVector.y - differenceVector.y > constrictPY || originalVector.y - differenceVector.y < constrictNY)
            {
                return false;
            }
        }
        return true;        
    }
}