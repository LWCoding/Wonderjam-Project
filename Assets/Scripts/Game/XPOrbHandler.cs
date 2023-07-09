using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPOrbHandler : MonoBehaviour
{

    public float xpValue;

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerMovementHandler.Instance.transform.position);
        transform.position = Vector3.Lerp(transform.position, PlayerMovementHandler.Instance.transform.position, 4 * Time.deltaTime);
    }

}
