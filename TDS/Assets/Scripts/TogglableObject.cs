using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglableObject : MonoBehaviour {

    public bool toggled = true;
    public enum ObjectType { Laser, Gas};
    public ObjectType objectType;
    public Sprite offImage;

    private void Update()
    {
        switch(objectType)
        {
            case ObjectType.Laser:
                if (toggled)
                {
                    GetComponent<Animator>().enabled = true;
                    transform.gameObject.layer = LayerMask.NameToLayer("Wall");
                    
                } else
                {
                    GetComponent<Animator>().enabled = false;
                    GetComponent<SpriteRenderer>().sprite = offImage;
                    transform.gameObject.layer = LayerMask.NameToLayer("Default");
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
