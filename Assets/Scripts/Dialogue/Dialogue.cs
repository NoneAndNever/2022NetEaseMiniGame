using System;
using UnityEngine;



[Serializable]
public class Dialogue
{
    public Dialogue(int index, string person, string dialogueText)
    {
        this.index = index;
        this.person = Enum.Parse<Person>(person);
        this.dialogueText = dialogueText;
    }

    public enum Person
    {
        Eve,
        Cosette,
        End
    }
    
    public int index;
    public Person person;
    public string dialogueText;
    
}
