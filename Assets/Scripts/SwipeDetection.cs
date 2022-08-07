using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    #region Events
    public delegate void Swipe(Direction direction);
    public event Swipe OnSwipe;
    #endregion

    [SerializeField]
    private float minimumDistance = .2f;
    [SerializeField, Range(0f, 1f)]
    private float directionThreshold = .9f;

    private InputManager inputManager;

    private Vector2 startPosition;

    private bool swipeInProgress = false;

    private void Awake()
    {
        inputManager = InputManager.Instance;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
    }

    private void Update()
    {
        DetectSwipe();
    }

    private void SwipeStart(Vector2 position)
    {
        startPosition = position;
        swipeInProgress = true;
    }

    private void DetectSwipe()
    {
        if (swipeInProgress)
        {
            var endPosition = inputManager.PrimaryPosition();
            if (Vector3.Distance(startPosition, endPosition) >= minimumDistance)
            {
                Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
                swipeInProgress = false;
            }
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            OnSwipe(Direction.Up);
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            OnSwipe(Direction.Right);
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            OnSwipe(Direction.Down);
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            OnSwipe(Direction.Left);
        }
    }
}
