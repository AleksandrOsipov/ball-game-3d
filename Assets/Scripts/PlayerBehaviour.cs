using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBehaviour : MonoBehaviour
{
    //Control and id
    public int player_id = 1;
    private string movement_axis_hor, movement_axis_vert;
    Dictionary<string, float> inputs = new Dictionary<string, float>();

    //handles
    Rigidbody2D body;
    Rigidbody2D ballBody;

    private float concentration; //Depletes while using power
    private float internalInfluence;
    public float influence;
    const float maxConcentration = 1000;
    const float minConcentration = 0;
    const float maxInfluence = 1000;
    const float minInfluence = 1;
    const float rechargeLimit = 80;//The point at which influence stops depleting
    const float rechargeStep = 1;
    const float depleteStep = 5;
    const float velocityInfluenceDampenerX = 0.4f;
    const float velocityInfluenceDampenerY = 0.1f;
    const float distMax=160;//Top of the dist influence range
    const float distMin=10;//Bottom of the dist influence range

    //Movement
    Boolean grounded, jumpReset;
    Vector2 maxVelocity = new Vector2(150, 150);
    Vector2 tempVelocity = new Vector2();
    Vector2 motionForce = new Vector2(2000, 0);
    Vector2 jumpVelocity = new Vector2(0, 300);
    const float lessGravity = 50;
    const float normalGravity = 90;

    // Use this for initialization
    void Start()
    {
        ballBody =GameObject.FindGameObjectWithTag(Constants.ball_tag).GetComponent<Rigidbody2D>();
        body = GetComponent<Rigidbody2D>();

        // The axes names are based on player number.
        movement_axis_hor = "Horizontal" + player_id;
        movement_axis_vert = "Vertical" + player_id;
        gameObject.SetActive(true);
        concentration = maxConcentration;
    }

    // Update is called once per frame
    void Update()
    {
        getInputs();
       

        if (inputs["telekinesis"] > 0)
        {
            internalInfluence += concentration;
            concentration -= 15;
        }
        else
        {
            concentration += 10;
            internalInfluence -= depleteStep;
        }
        

        if (inputs["jump"] > 0)
        {
            jump();
        }
        else
        {
            jumpReset = true;
            body.gravityScale = normalGravity;
        }
        limitPowers();
        calcConcentration();
        calcInfluence();
        body.AddForce(motionForce * inputs["horizontal"]);


        limitVelocity();
        grounded = false;
    }

    void getInputs()
    {
        inputs["horizontal"] = Input.GetAxis(movement_axis_hor);
        inputs["jump"] = Input.GetAxis(movement_axis_vert);
        //TODO change this to a better mapping for controllers
        inputs["telekinesis"] = inputs["jump"] < 0 ? 1 : 0;
    }

    void jump()
    {
        if (grounded && jumpReset) {
            body.velocity += jumpVelocity;
            jumpReset = false;
        }
        else
        {
            body.gravityScale = lessGravity;
        }
    }

    void calcConcentration()
    {
        if (concentration < rechargeLimit)
        {
            concentration += rechargeStep;
        }

    }

    void calcInfluence()
    {
        if (internalInfluence < rechargeLimit)
        {
            internalInfluence += rechargeStep;
        }
        else
        {
            internalInfluence -= depleteStep;
        }

        //The faster you're moving the less you can concentrate
        internalInfluence *= 1-Math.Abs(body.velocity.normalized.x)*velocityInfluenceDampenerX;
        internalInfluence *= 1-Math.Abs(body.velocity.normalized.y)*velocityInfluenceDampenerY;

        influence = internalInfluence;
        //The closer to the ball you are, the more you can influence it
        float dist = (body.position - ballBody.position).magnitude;
        dist = dist < distMin ? distMin:(dist>distMax?distMax:dist);
        float distInf = dist / distMax;
        //Dist is between 10 and 160, so 
        influence /= 1000+distInf;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y < body.position.y)
            {
                grounded = true;
                return;
            }
        }
        
    }
    void limitVelocity()
    {
        tempVelocity = body.velocity;
        if (Math.Abs(body.velocity.x) > maxVelocity.x)
        {
            tempVelocity.x = body.velocity.x > 0 ? maxVelocity.x : -maxVelocity.x;
        }
        if (Math.Abs(body.velocity.x) > maxVelocity.x)
        {
            tempVelocity.x = body.velocity.x > 0 ? maxVelocity.x : -maxVelocity.x;
        }
        body.velocity = tempVelocity;
    }

    void limitPowers()
    {
        concentration = concentration < minConcentration ? minConcentration : concentration;
        concentration = concentration > maxConcentration ? maxConcentration : concentration;

        internalInfluence = internalInfluence < minInfluence ? minInfluence : internalInfluence;
        internalInfluence = internalInfluence > maxInfluence ? maxInfluence : internalInfluence;
    }

}
