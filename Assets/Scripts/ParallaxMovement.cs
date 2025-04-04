using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    [SerializeField] Transform cam;                 // reference to camera
    [SerializeField] float matchCamXPercent = 1f;   // range [0..1] (1 matches camera position)
    [SerializeField] float matchCamYPercent = 1f;

    Vector2 camStartPos;

    private void Start()
    {
        camStartPos = cam.position; // record starting cam position
    }

    void LateUpdate()   // LateUpdate() will ensure all Update()s have happened already
    {
        // calculate offset of cam position at start to cam position now.
        Vector2 offset = (Vector2)cam.position - camStartPos;

        offset.x *= matchCamXPercent;   // calculate % of offset to use
        offset.y *= matchCamYPercent;

        transform.position = offset;    // position the sprite
    }
}
