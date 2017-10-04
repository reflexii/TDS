using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableObject : MonoBehaviour {

    public bool toggled = true;
    public enum ObjectType { Laser, Gas};
    public ObjectType ot;
    public Sprite offImage;

    private void Update()
    {
        switch(ot)
        {
            case ObjectType.Laser:
                if (toggled)
                {
                    GetComponent<Animator>().enabled = true;
                } else
                {
                    GetComponent<Animator>().enabled = false;
                    GetComponent<SpriteRenderer>().sprite = offImage;
                }
                break;
            case ObjectType.Gas:
                if (toggled)
                {

                } else
                {

                }
                break;
        }
    }
}
