using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmissionZone : MonoBehaviour
{
    public PropSpawner propSpawner;
    public StageMachine stageMachine;

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Prop"))
        {
            if(!collision.gameObject.GetComponent<Rigidbody>().isKinematic)
            {
                stageMachine.SubmissionProduct(collision.GetComponent<Product>());
                Debug.Log(collision.GetComponent<Product>().productCode);
                Destroy(collision.gameObject);
                propSpawner.currentMaterialNum--;
            }
        }
    }
}
