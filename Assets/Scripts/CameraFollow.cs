using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private float yVelocity = 0f;

    private void LateUpdate()
    {
        FollowPlayerY(); 
    }

    private void FollowPlayerY()
    {
        //float smoothSpeed = 5f;
        float smoothTime = 0.35f;
        float yOffset = 1f;

        if (player.position.y > transform.position.y)
        {
            //Vector2 currentPos = transform.position;
            //Vector2 targetPos = new Vector2(currentPos.x, player.position.y - yOffset);

            //whenver doing lerp alwasy multiply by Time.Delta cuz should be consistent across varying hardware blah blah
            //Vector2 newPos = Vector2.Lerp(currentPos, targetPos, smoothSpeed * Time.deltaTime);

            //transform.position = new Vector3(newPos.x, newPos.y, transform.position.z); //cant avoid use of Vector3 here cuz its a camera its needs a z axis

            float targetY = player.position.y + yOffset;
            float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref yVelocity, smoothTime);//Makes the jump feel more springy ease out ease in

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        }
    }
}
