// Always remove useless usings to reduce build size
using UnityEngine;

public class CodingConventions : MonoBehaviour
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

    #region PUBLIC API

    // Public functions take a capital latter
    public void DoSomething()
    {

    }

    // We write public getters as C# properties. Takes a capital latter as well
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

    #endregion

    #region PRIVATE

    // Private and protected functions don't take a capital letter
    void doSomethingPrivate()
    {

    }

    #endregion

    #region PROTECTED

    protected void doSomethingProtected()
    {

    }

    #endregion
}
