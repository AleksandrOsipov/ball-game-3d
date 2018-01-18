using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {


    public int influence = 1;
    int concentration = 100; //Depletes while using power
    private string movement_axis_hor, movement_axis_vert;
    public int player_id = 1;
    float horInput;
    float vertInput;

    Rigidbody2D body;
    GameObject ball;

    // Use this for initialization
    void Start () {

        influence = 1;
        // The axes names are based on player number.
        movement_axis_hor = "Horizontal" +player_id;
        movement_axis_vert = "Vertical" + player_id;

        body = GetComponent<Rigidbody2D>();
        ball = GameObject.FindGameObjectWithTag("Ball");
    }
	
	// Update is called once per frame
	void Update () {
        horInput =Input.GetAxis(movement_axis_hor)*10;
        vertInput = Input.GetAxis(movement_axis_vert)*10;

        Debug.Log(
        String.Format(
            "playerId{3}\n"+
        "influence: {0}\n"+
        "position: x:{1},y:{2}",
           
        influence,
        body.position.x, body.position.y, player_id)
    );
        if (vertInput < 0)
        {
            influence = concentration;
            concentration -= 2;
            vertInput = 0;
        }
        else 
        {
            influence = influence / 2;
            concentration += 2;
        }
        influence = influence<1?1:influence;
        concentration = concentration < 0?0: (concentration > 100?concentration:100);
        body.velocity =  new Vector2(horInput*10,vertInput*10);
        

    }
}
