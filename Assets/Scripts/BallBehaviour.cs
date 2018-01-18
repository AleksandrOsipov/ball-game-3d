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
    Vector2 velocity;
    GameObject[] players;
    const float integrating_dampener = 0.9f;
    const float general_dampener = 1f;
    const float proportional_c = 2f;
    const float integrating_c = 1f;
    const float derivating_c = 1f;
    float margin = 10;
    Vector2 offset;
    float total_influence;

    Vector2 p1diff, p2diff;

    Vector2 maxVelocity = new Vector2(300,300);

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        players = GameObject.FindGameObjectsWithTag(Constants.player_tag);

    }
	
    void PID(influencer[] influencers)
    {
        //linear interpolation to get a target in between the two influencers corresponding to their influence
        target = calculate_target(influencers);
        //but we can't let the point be AT the player, because then the ball will just keep pushing that player out
        //we add margins so that the target gets close to but not in the face of, a player
        //target = add_margins(influencers[0].position, influencers[1].position,target);

        last_error = error;
        error = target - body.position;

        integral += last_error;
        integral *= integrating_dampener;
        derivative = error - last_error;

        //PID controller below
        velocity = proportional_c * error + integrating_c * integral + derivating_c * derivative;

        if(Math.Abs(velocity.x) > maxVelocity.x)
        {
            velocity.x = velocity.x > 0 ? maxVelocity.x : -maxVelocity.x;
        }
        if (Math.Abs(velocity.y) > maxVelocity.y)
        {
            velocity.y = velocity.y > 0 ? maxVelocity.y : -maxVelocity.y;
        }
      
        body.velocity = velocity;

        
    }

    private Vector2 calculate_target(influencer[] influencers)
    {
        return lerpVec2(influencers[0].position, influencers[1].position, influencers[0].influence, influencers[1].influence);
    }

    private Vector2 lerpVec2(Vector2 pos1, Vector2 pos2,float val1,float val2)
    {
        return pos1 + (val1 / (val1 + val2)) * (pos2 - pos1);
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
