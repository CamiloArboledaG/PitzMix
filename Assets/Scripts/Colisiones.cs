using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colisiones : MonoBehaviour
{
    public GameObject cabeza;
    public GameObject torso;
    public GameObject personaje;
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.name == cabeza.name)
        {
            Destroy(personaje);

        }
        if (other.gameObject.name == torso.name)
        {
            //this.GetComponent<Rigidbody>().AddRelativeForce(other.transform.forward * 15.0f, ForceMode.Impulse);
        }
    }
}
