using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpScore : MonoBehaviour
{
    public TextMeshProUGUI text; 
    private float speed = 1.5f;
    
    private float slowDownSpeed = 0.01f;

    private float fadeSpeed = 0.6f;
    private float fade = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeAway());
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0,Time.deltaTime*speed,0);        
    }

    void FixedUpdate()
    {
        speed -= slowDownSpeed;
        if (speed <= 0)
        {
            speed = 0;
        }
    }

    IEnumerator FadeAway()
    {
        while(fade > -0.5f)
        {
            fade -= fadeSpeed*Time.deltaTime;
            text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, fade);
            yield return 0;
        }
        Destroy(gameObject);
    }
}
