using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spr;

    public void SetType(int _id)
    {
        //spr.sprite = sprites[_id];

        // lidar com boxcollider, ainda n fiz
        switch (_id)
        {
            case 0:
                // mudar tamanho do boxcollider
                // mudar offset do boxcollider
                // mudar posição Y
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                break;
        }
    }
}
