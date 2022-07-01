using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conventions : MonoBehaviour
{
    // Any data that you want to show in the inspector is
    // private
    // [SerializeField]
    [SerializeField] int myInt = 0;


    // for unity defined functions (unity messages), keep them in a region at the beginning of your script
    #region UNITY AND CORE

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    #endregion

    // Public functions take a capital
    public void DoSomething()
    {

    }

    // We make public getters properties. Takes a capital as well
    public int MyInt => myInt;

    // For setters, we use functions
    public void SetMyInt(int i_myInt)
    {
        myInt = i_myInt;
    }

    // Function parameters have a special naming convention
    public void DoSomethingWithParam(int i_myInt)
    {
        myInt = i_myInt;
    }

    // Private and protected functions don't take a capital
    void doSomethingPrivate()
    {

    }

    protected void doSomethingProtected()
    {

    }
}
