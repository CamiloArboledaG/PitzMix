using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aro : MonoBehaviour
{

    public static int score = 0;
    public static int score2 = 0;

    public Text txtscore;
    public Text txtscore2;

    public GameObject cancha;
    public GameObject cancha2;

    // Start is called before the first frame update
    void Start()
    {
        txtscore.text = score.ToString();
        txtscore2.text = score2.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == cancha.name)
        {

            score++;
            txtscore.text = score.ToString();
        }
        if (other.gameObject.name == cancha2.name)
        {

            score2++;
            txtscore2.text = score2.ToString();
        }
    }
}
