using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    Vector2 screenBounds;
    Vector2 objectSize;

    Vector3 position;
    private void Start()
    {
        position = transform.position;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectSize = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x / 2, GetComponent<SpriteRenderer>().bounds.size.y / 2);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 movement = new Vector3(0, Input.GetAxis("Vertical"), 0);
        position += movement * speed * Time.deltaTime;

        position.x = Mathf.Clamp(movement.x, screenBounds.x + objectSize.x, screenBounds.x * -1 - objectSize.x);
        position.y = Mathf.Clamp(movement.y, screenBounds.y + objectSize.y, screenBounds.y * -1 - objectSize.y);

        transform.position = position;




    }
}
