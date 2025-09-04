using UnityEngine;

public class LogicPuzzleAnswer : MonoBehaviour
{

    [SerializeField] private SOLogicPuzzleAnswers puzzleAnswer;

    public bool isDragging = false;
    private Vector3 offset;

    [SerializeField] private LayerMask socketLayer;

    public GameObject currentSocket;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = puzzleAnswer.sprite;
    }

    void Update()
    {

        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            //Debug.Log($"Touch detected at: {touch.position} - Phase: {touch.phase}");
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0; // Keep it on the same plane

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("Began");
                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        //offset = transform.position - touchPosition;
                    }
                    break;

                case TouchPhase.Moved:
                    Debug.Log("Moved");
                    if (isDragging)
                    {
                        transform.position = touchPosition;// + offset;
                    }
                    break;

                case TouchPhase.Ended:
                    Debug.Log("Ended");

                    hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        isDragging = false;

                        if (currentSocket != null)
                        {
                            gameObject.transform.position = currentSocket.transform.position;
                            currentSocket.GetComponent<LogicPuzzleSocket>().AttemptAnswer(puzzleAnswer.typeLogic, gameObject);
                        }
                    }

                    break;
            }
        }


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {


        int x = 1 << collision.gameObject.layer;

        // Trigger Clown Falling
        if (x == socketLayer.value)
        {
            currentSocket = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int x = 1 << collision.gameObject.layer;

        // Trigger Clown Falling
        if (x == socketLayer.value)
        {
            if (collision.gameObject == currentSocket)
                currentSocket = null;
        }

    }
}
