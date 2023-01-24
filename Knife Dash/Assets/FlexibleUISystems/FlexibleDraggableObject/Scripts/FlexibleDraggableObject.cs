using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class FlexibleDraggableObject : MonoBehaviour
{
    public GameObject Target;
    private EventTrigger _eventTrigger;
    private Camera mainCam;
    private RectTransform rect;
    bool canScale;
    void Start ()
    {
        _eventTrigger = GetComponent<EventTrigger>();
        _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        _eventTrigger.AddEventTrigger(OnBeginDrag, EventTriggerType.BeginDrag);
        _eventTrigger.AddEventTrigger(OnEndDrag, EventTriggerType.EndDrag);
        mainCam = Camera.main;
        rect = Target.GetComponent<RectTransform>();

    }

    private void OnEndDrag(BaseEventData arg0)
    {
        canScale = false;
    }

    private void OnBeginDrag(BaseEventData data)
    {
        canScale = true;
    }

    void OnDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData) data;
        Target.transform.position += new Vector3(ped.delta.x,ped.delta.y);

        ClampPos();
    }

    private void Update()
    {
        if(Input.touchCount == 2 && canScale)
        {
            Touch touchzero = Input.GetTouch(0);
            Touch touchone = Input.GetTouch(1);

            Vector2 touchzeroPrevPos = touchzero.position - touchzero.deltaPosition;
            Vector2 touchonePrevPos = touchone.position - touchone.deltaPosition;

            Debug.Log("zero prev is"+touchzeroPrevPos);
            Debug.Log("one prev is" + touchonePrevPos);
            float prevMagn = (touchzeroPrevPos - touchonePrevPos).magnitude;
            float currentMagn = (touchzero.position - touchone.position).magnitude;

            float difference = prevMagn - currentMagn;

            Scale(difference * 0.01f);
        }
        //Scale(Input.GetAxis("Mouse ScrollWheel"));
    }
    private void Scale(float scale)
    {
        Debug.Log("scaling" + scale);
        var NewScale = rect.localScale.x;
        NewScale = Mathf.Clamp(NewScale - scale, 0.75f, 2);

        rect.localScale = Vector3.one * NewScale;
        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scale, 3, 9);
    }
    void ClampPos()
    {
        Vector2 max = mainCam.ViewportToScreenPoint(Vector2.one);
        Vector2 min = mainCam.ViewportToScreenPoint(Vector2.zero);
       
        Vector2 pos = Target.transform.position;

        Debug.Log(max);
        
        pos.x = Mathf.Clamp(pos.x, min.x + rect.sizeDelta.x * 2 * rect.localScale.x, max.x - rect.sizeDelta.x * 2 * rect.localScale.x);
        pos.y = Mathf.Clamp(pos.y, min.y + rect.sizeDelta.y * 2 * rect.localScale.y, max.y - rect.sizeDelta.y * 2 * rect.localScale.y);
        Target.transform.position = pos;
    }
}