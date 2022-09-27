using System;

/// <summary>
/// Add this attribute to a public method in a MonoBehaviourBase script 
/// and get a button for your method in the inspector
/// </summary>
public class ExposePublicMethod : Attribute
{
    bool playModeOnly = true;

    public ExposePublicMethod(bool i_playModeOnly = true) { playModeOnly = i_playModeOnly; }

    public bool PlayModeOnly => playModeOnly;
}