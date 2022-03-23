using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : NetworkBehaviour
{
    public float speed = 30f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        rb.simulated = true;

        rb.velocity = Vector2.right * speed;
    }

    private float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
    {
        return (ballPos.y - racketPos.y) / racketHeight;
    }

    [ServerCallback] //Only the server can call this method
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponent<PlayerMovement>())
        {
            float y = HitFactor(transform.position, collision.transform.position, collision.collider.bounds.size.y);

            float x = collision.relativeVelocity.x > 0 ? 1 : -1;

            Vector2 direction = new Vector2(x, y).normalized;

            rb.velocity = direction * speed;
        }
        else if (collision.transform.GetComponent<Goal>())
        {
            Goal goal = collision.transform.GetComponent<Goal>();
            GameEvents.Instance.Score(goal.Place);
        }
    }

    [ServerCallback]
    public void SetDirection(GameEvents.PlayerPlace pPlace)
    {
        rb.simulated = true;

        if(pPlace == GameEvents.PlayerPlace.Left)
            rb.velocity = Vector2.left * speed;
        else
            rb.velocity = Vector2.right * speed;
    }
}
