using System;
using UnityEngine;



[Serializable]
public class Dialogue
{
    public Dialogue(int index, string person, string dialogueText)
    {
        this.index = index;
        this.nameText = person;
        this.person = Enum.Parse<Person>(person);
        this.dialogueText = dialogueText;
    }
    public enum Person
    {
        Eve,
        Cosette
    }
    public Person person;
    //public int face;
    public int index;
    public string nameText;
    public string dialogueText;
    
}
