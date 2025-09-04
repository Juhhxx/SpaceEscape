using UnityEngine;

public class LogicGateSpawner : MonoBehaviour
{

    [SerializeField] private GameObject answerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        GameObject prefab = Instantiate(answerPrefab, gameObject.transform);
                        prefab.transform.position = touchPosition;
                        prefab.GetComponent<LogicPuzzleAnswer>().isDragging = true;
                    }

                    break;

                case TouchPhase.Moved:

                    break;

                case TouchPhase.Ended:

                    break;
            }
        }

    }
}
