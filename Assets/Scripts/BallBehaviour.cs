using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct influencer{
        public Vector2 position;
        public float influence;
    };

public class BallBehaviour : MonoBehaviour {
    
    private influencer[] influencers = {new influencer(), new influencer() };
    public bool powered = true;
    Vector2 target;
    Rigidbody2D body;
    Vector2 integral;
    Vector2 derivative;
    Vector2 error;
    Vector2 last_error;
    GameObject[] players;
    const float integrating_dampener = 1f;
    const float general_dampener = 1f;
    const float proportional_c =12f;
    const float integrating_c = 2f;
    const float derivating_c = 1f;

    Vector2 maxVelocity = new Vector2(200,200);

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        players = GameObject.FindGameObjectsWithTag("Player");

    }
	
    void PID(influencer[] influencers)
    {
        Vector2 velocity;

        target = calculate_target(influencers);
        Debug.DrawLine(new Vector3(target.x-10,target.y-10,0), new Vector3(target.x+10, target.y+10, 0), Color.red);

        last_error = error;
        error = target - body.position;
        integral *= integrating_dampener;
        integral += last_error;
        derivative = error - last_error;

        //PID controller below
        velocity = proportional_c * error + integrating_c * integral + derivating_c * derivative;
        velocity *= general_dampener;
        if(Math.Abs(velocity.x) > maxVelocity.x)
        {
            velocity.x = velocity.x > 0 ? maxVelocity.x : -maxVelocity.x;
        }
        if (Math.Abs(velocity.y) > maxVelocity.y)
        {
            velocity.y = velocity.y > 0 ? maxVelocity.y : -maxVelocity.y;
        }
        /*
        Debug.Log(
            String.Format(
            "target: x:{0} y:{1} \n" +
            "error: x:{2} y:{3} \n" +
            "velocity: x:{4} y:{5} \n",
            target.x,target.y,
            error.x, error.y,
            velocity.x,velocity.y)
            );
            */
        body.velocity = velocity;

        
    }

    private Vector2 calculate_target(influencer[] influencers)
    {
        //This only supports two influencers for now
        //TODO: n influencer targets? i.e 3 players and up

        //linear interpolation to get a target in between the two influencers corresponding to their influence
        return           influencers[0].position 
                         + (
                                influencers[0].influence
                                    / 
                                (influencers[0].influence+ influencers[1].influence) 
                            )
                            * (influencers[1].position - influencers[0].position); 



    }

    // Update is called once per frame
    void Update () {
        
        influencers[0].position = players[0].GetComponent<Rigidbody2D>().position;
        influencers[1].position = players[1].GetComponent<Rigidbody2D>().position;
        influencers[0].influence = players[0].GetComponent<PlayerBehaviour>().influence;
        influencers[1].influence = players[1].GetComponent<PlayerBehaviour>().influence;
        if (influencers.Length > 2)
            return;//Add support for three players?
        PID(influencers);

        


	}
}
