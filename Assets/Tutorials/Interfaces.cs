
using UnityEngine;

public class Human : IAgeProvider
{
    string personalityDesc = "Cool guy";
    float height = 2f;
    int age = 20;


    public Human(string i_personality, float i_height, int i_age)
    {
        SetAge(i_age);
    }

    public int Age => age;

    public void SetAge(int i_age)
    {
        age = i_age;
    }
}

public interface IAgeProvider
{
    int Age { get; }
}

public interface ICharacterStatsProvider : IAgeProvider
{
    bool HasWings { get; }
}

public class AlienDude : ICharacterStatsProvider
{
    public bool HasWings => throw new System.NotImplementedException();

    public int Age => throw new System.NotImplementedException();
}


public class HumanCharacter : Human, ICharacterStatsProvider
{
    bool hasWings = true;

    public HumanCharacter(bool i_hasWings,string i_personality, float i_height, int i_age) : base(i_personality, i_height, i_age)
    {
        SetWings(i_hasWings);
    }

    public bool HasWings => hasWings;

    public void SetWings(bool i_hasWings)
    {
        hasWings = i_hasWings;
    }
}


public static class CharacterFactory
{
    public static ICharacterStatsProvider GetHumanCharacter(bool i_hasWings, string i_personality, float i_height, int i_age)
    {
        return new HumanCharacter(i_hasWings,  i_personality,  i_height,  i_age);
    }

    public static ICharacterStatsProvider GetAlienDude()
    {
        return new AlienDude();
    }
}


// Responsible for printing character stats in a UI
public class CharacterStatsUI : MonoBehaviour
{
    private void Start()
    {
        ICharacterStatsProvider myChar = CharacterFactory.GetAlienDude();
        showUI(myChar);

    }

    void showUI(ICharacterStatsProvider i_char)
    {
        // show UI
    }
}





