using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class hideWhenFinished : MonoBehaviour
{
    //Hide Help Panle when the help animation is completed    
    Animator animator;

    void Start()
    {
        //Get the Animator which is attached to the Help Panel 
        animator = gameObject.GetComponent<Animator>();        
    }
    
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("completed"))
        {
            Debug.Log("The animation is completed, hide the panel");
            HidePanel();
            //gameObject.SetActive(false);
        }        
    }

    public void HidePanel()
    {
            gameObject.SetActive(false);
    }
}
