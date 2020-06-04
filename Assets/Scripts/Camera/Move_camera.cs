using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_camera : MonoBehaviour
{
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var player_pos = Player.transform.position;
        transform.position = new Vector3(player_pos.x, player_pos.y, transform.position.z);
    }
}
