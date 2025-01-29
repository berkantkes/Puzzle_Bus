using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendedScrollRect : ScrollRect
{
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    // ScrollRect dışında tıklamaları da algıla
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
    }
}