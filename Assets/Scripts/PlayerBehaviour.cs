using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{


    public int influence = 1;
    int concentration = 1000; //Depletes while using power
    int maxConcentration = 1000;
    int minConcentration = 0;
    private string movement_axis_hor, movement_axis_vert;
    public int player_id = 1;
    float horInput;
    float vertInput;
    Dictionary<string, float> inputs = new Dictionary<string, float>();
    ContactPoint2D[] contacts = { };
    Vector2 jumpForce = new Vector2(0, 100);
    Vector2 motionForce = new Vector2(1000, 0);
    Vector2 maxVelocity = new Vector2(150, 150);
    Vector2 tempVelocity = new Vector2();
    Boolean canJump = false;

    Rigidbody2D body;
    CapsuleCollider2D capCollider2D;

    // Use this for initialization
    void Start()
    {

        influence = 1;
        // The axes names are based on player number.
        movement_axis_hor = "Horizontal" + player_id;
        movement_axis_vert = "Vertical" + player_id;
        gameObject.SetActive(true);
        body = GetComponent<Rigidbody2D>();
        capCollider2D = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        getInputs();

        if (inputs["telekinesis"] == 1)
        {
            influence = concentration;
            concentration -= 15;
        }
        else
        {
            influence = 1;
            concentration += 10;
        }

        if (inputs["jump"] == 1)
        {
            jump();
        }

        body.AddForce(motionForce * inputs["horizontal"]);


        limitVelocity();
        limitPowers();

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

        capCollider2D.GetContacts(contacts);
        
        Debug.Log(String.Format(
                "playerId{0}\n" +
                "collider is active?: {1}\n" +
                "contacts:{2}\n\n",
                player_id,
                capCollider2D.isActiveAndEnabled,
                contacts.Length
                ));
        foreach (var contact in contacts)
        {
            Debug.Log(
                String.Format(
                "playerId{0}\n" +
                "touching something\n\n", player_id
                ));
            if (contact.point.y < body.position.y)
            {
                Debug.Log(
                String.Format(
                "playerId{0}\n" +
                "touching something below\n\n", player_id
                ));
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

        influence = influence < minConcentration ? minConcentration : influence;
        influence = influence > maxConcentration ? maxConcentration : influence;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y < body.position.y)
            {
                canJump = true;
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
