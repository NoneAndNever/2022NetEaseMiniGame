using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : SingletonMono<DialogueManager>
{
    [SerializeField] private TextAsset textAsset;
    [ContextMenuItem("初始化","InitialText")][ContextMenuItem("显示对话","ShowText")][SerializeField]  List<Dialogue> dialogues;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI dialText;
    [SerializeField] private Image personImage;
    [SerializeField] private int currentLine;

    protected override void Awake()
    {
        base.Awake();
        //todo:物品池UI池
        InitialComponents();
        //InitialText();
    }

    private void InitialComponents()
    {
        nameLabel = textPanel.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        dialText = textPanel.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
    }

    private void ShowText()
    {
        currentLine++;
        if (currentLine>dialogues.Count)
        {
            
        }
        nameLabel.text = dialogues[currentLine].person.ToString();
        dialText.text = dialogues[currentLine].dialogueText;
    }
    
    private void InitialText(string fileName)
    {
        Resources.Load<TextAsset>(fileName);
        dialogues.Clear();
        string[] dials=textAsset.text.Split("\r\n");
        string[] infos;
        for (int i = 0; i < dials.Length-1; i++)
        {
            infos = dials[i].Split(',');
            if (infos[0]== "End") continue;
            dialogues.Add(new Dialogue(i,infos[0],infos[1]));
        }
    }
    
    
}
