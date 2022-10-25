using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : SingletonMono<DialogueManager>
{
    [SerializeField] private TextAsset textAsset;//所读取的对话文件
    [SerializeField] private List<Dialogue> dialogues;//对话存储列表
    [SerializeField] private GameObject dialPanel;//UI对话面板
    [SerializeField] private GameObject eveNameLabel;//eve名字标签
    [SerializeField] private GameObject CosetteNameLabel;//cosette名字标签
    [SerializeField] private Text dialText;//对话框文本
    [SerializeField] private Image eveImage;//eve图像
    [SerializeField] private Image cosetteImage;//cosette图像
    [SerializeField] private int currentLine;
    private Dialogue.Person currentPerson=Dialogue.Person.End;
    private readonly WaitForSeconds typingSpeed = new WaitForSeconds(0.1f);
    private Coroutine currentCoroutine;
    [SerializeField]private bool isTyping;
    protected override void Awake()
    {
        base.Awake();
        //todo:物品池UI池
        //InitialComponents();
        //InitialText();
        InitTextAsset(textAsset);
    }


    private void Update()
    {
        if (dialPanel.activeInHierarchy&&Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                //todo：判定预防措施
                StopCoroutine(currentCoroutine);
                dialText.text = dialogues[currentLine-1].dialogueText;
                isTyping = false;
            }
            else
            {
                ShowText();
            }
        }
    }

    private void InitTextAsset(TextAsset textFile)
    {
        dialogues.Clear();
        string[] rows=textAsset.text.Split("\r\n");
        string[] columns;
        for (int i = 0; i < rows.Length-1; i++)
        {
            columns = rows[i].Split(',');
            dialogues.Add(new Dialogue(i,columns[0],columns[1]));
        }
    }

    public DialogueManager SetLine(int lineNum)
    {
        currentLine = lineNum;
        return this;
    }


    public void ShowText()
    {
        //如果对话面板未启用则启用
        if(!dialPanel.activeInHierarchy) dialPanel.SetActive(true);
        //读到End则结束这一段
        if (dialogues[currentLine].person==Dialogue.Person.End)
        {
            ClosePanel();//关闭面板
            StartCoroutine(RoundCtrl.GetInstance().NextRoundState(null));
            return;
        }

        if (dialogues[currentLine].person!=currentPerson)
        {
            currentPerson = dialogues[currentLine].person;
            SwitchPerson(currentPerson);
        } 
        
        currentCoroutine = StartCoroutine(WordsTyping(dialogues[currentLine].dialogueText));
        currentLine++;
    }

    private IEnumerator WordsTyping(string text)
    {
        isTyping = true;
        dialText.text="";
        for (int i = 0; i < text.Length; i++)
        {
            dialText.text+=text[i];
            yield return typingSpeed;
        }

        isTyping = false;
    }

    private void ClosePanel()
    {
        dialPanel.SetActive(false);
    }

    private void SwitchPerson(Dialogue.Person person)
    {
        switch (person)
        {
            case Dialogue.Person.Eve:
                eveNameLabel.SetActive(true);
                CosetteNameLabel.SetActive(false);
                eveImage.color=Color.white;
                cosetteImage.color=Color.gray;
                break;
            case Dialogue.Person.Cosette:
                eveNameLabel.SetActive(false);
                CosetteNameLabel.SetActive(true);
                eveImage.color=Color.gray;
                cosetteImage.color=Color.white;
                break;
        }
    }

    
    
}
