using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Ebook : MonoBehaviour
{
    [SerializeField] private WebViewObject webViewObject;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private string htmlText;
    private int fontSize = 50;
    private string title;
    [SerializeField] private List<string> pages = new List<string>();
    private int indexOfPage = 0;

    public void PageForward()
    {
        indexOfPage++;
        text.text = pages[indexOfPage];
    }

    public void PageBackward()
    {
        indexOfPage--;
        text.text = pages[indexOfPage];
    }

    public static bool IsDigitsOnly(string str, string operators) //Den här kollar så att det bara finns nummer eller mellanslag i passwordet. Om inte för denna så skulle spelet krascha om du skrev en bokstav.
    {
        bool containsNumber = false;
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
            {
                if (c != ' ')
                {
                    bool found = false;
                    foreach (char character in operators)
                    {
                        if (c == character)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
            }
            else if (!containsNumber && c >= '0' && c <= '9')
            {
                containsNumber = true;
            }
        }
        if (!containsNumber)
        {
            return false;
        }
        return true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //webViewObject.LoadURL("https://juulgustav.github.io/");
        int index = htmlText.IndexOf("<title>") + 7;
        title = htmlText.Substring(index, htmlText.IndexOf("</title>") - index);
        htmlText = htmlText.Substring(htmlText.IndexOf("</head>") + 7);

        string[] parts = Regex.Split(htmlText, @"(?<=[<>])");

        List<string> list = new List<string>();
        list.Add(string.Empty);
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();

            if (parts[i].Length > 0 && parts[i][0] == '<' && list[^1] != string.Empty)
            {
                list.Add(parts[i]);
            }
            else
            {
                list[^1] += parts[i];
                if (parts[i].Length > 1 && parts[i][^1] == '<')
                {
                    list[^1] = list[^1].Remove(list[^1].Length - 1);
                    list.Add("<");
                }
                if (parts[i].Length > 0 && parts[i][^1] == '>')
                {
                    list.Add(string.Empty);
                }
            }
        }

        string output = string.Empty;
        string addNextRound = string.Empty;

        List<string> newList = new List<string>();

        for (int i = 0; i < list.Count; i++)
        {
            string tempOutput = string.Empty;
            if (list[i].Length > 0)
            {
                if (list[i][0] == '<')
                {
                    if (list[i][1] == '/')
                    {
                        if (addNextRound.Length > 0)
                        {
                            output += addNextRound;
                            newList.Add(addNextRound);
                            addNextRound = string.Empty;
                        }
                        if (list[i][2] == 'h' && IsDigitsOnly(list[i][3].ToString(), string.Empty))
                        {
                            output += "\n";
                            newList.Add("\n");
                        }
                        else if (list[i][2] == 'p')
                        {
                            output += "\n";
                            newList.Add("\n");
                        }
                    }
                    else if (list[i][1] == 'h' && IsDigitsOnly(list[i][2].ToString(), string.Empty))
                    {
                        if (list.Count > 1)
                        {
                            output += "\n";
                            newList.Add("\n");
                        }
                        int temp = Convert.ToInt32(list[i][2].ToString());
                        int size = fontSize;
                        if (temp == 1)
                        {
                            size = fontSize * 2;
                        }
                        else if (temp == 3)
                        {
                            size = fontSize * 2;
                        }
                        else if (temp == 4)
                        {
                            size = (int)(fontSize * 1.5f);
                        }
                        else if (temp == 5)
                        {
                            size = fontSize * 2;
                        }
                        else if (temp == 6)
                        {
                            size = fontSize * 2;
                        }
                        output += "<size=" + (size) + ">";
                        newList.Add("<size=" + (size) + ">");
                        addNextRound += "</size>";
                        if (list[i].Contains("center"))
                        {
                            output += "<align=\"center\">";
                            addNextRound += "</align>";
                            newList.Add("<align=\"center\">");
                        }
                        else if (list[i].Contains("left"))
                        {
                            output += "<align=\"left\">";
                            addNextRound += "</align>";
                            newList.Add("<align=\"left\">");
                        }
                        else if (list[i].Contains("right"))
                        {
                            output += "<align=\"right\">";
                            addNextRound += "</align>";
                            newList.Add("<align=\"right\">");
                        }
                    }
                    else if (list[i][1] == 'p')
                    {
                        if (list.Count > 1)
                        {
                            output += "\n";
                            newList.Add("\n");
                        }
                        if (list[i].Contains("center"))
                        {
                            output += "<align=\"center\">";
                            addNextRound += "</align>";
                            newList.Add("<align=\"center\">");
                        }
                        else if (list[i].Contains("left"))
                        {
                            output += "<align=\"left\">";
                            addNextRound += "</align>";
                            newList.Add("<align=\"left\">");
                        }
                        else if (list[i].Contains("right"))
                        {
                            output += "<align=\"right\">";
                            addNextRound += "</align>";
                            newList.Add("<align=\"right\">");
                        }

                        if (list[i].Contains("bold"))
                        {
                            output += "<b>";
                            addNextRound += "</b>";
                            newList.Add("<b>");
                        }
                    }
                }
                else
                {
                    output += list[i];
                    newList.Add(list[i]);
                }
            }
        }

        htmlText = htmlText.Replace("<title>", "<size=" + (fontSize * 2) + ">");
        htmlText = htmlText.Replace("</p>", "\n\n");

        text.text = output;
        text.fontSize = fontSize;

        float textHeight = 0;
        string currentText = string.Empty;
        string oldText = string.Empty;
        index = 0;
        string[] strings = newList.ToArray();
        while (index < newList.Count)
        {
            text.text = string.Empty;
            textHeight = 0;
            currentText = string.Empty;
            oldText = string.Empty;
            if (index + 1 >= newList.Count && newList[index] == "\n")
            {
                break;
            }
            //MaxLengthOfString(ref index, strings);
            for (int i = index; i < newList.Count; i++)
            {
                oldText = currentText;
                if (!(currentText == string.Empty && (newList[i] == string.Empty || newList[i] == "\n")))
                {
                    currentText += newList[i];
                    text.text = currentText;
                }
                if (text.preferredHeight > text.rectTransform.rect.height || i + 1 >= newList.Count)
                {
                    bool moreTextOnPage = false;
                    index = i + 1 >= newList.Count ? newList.Count : index;
                    if (newList[i].Length > 0 && newList[i][0] != '<')
                    {
                        string[] words = newList[i].Split(' ');
                        if (words.Length > 1)
                        {
                            currentText = oldText;
                            //oldText = string.Empty;
                            for (int a = 0; a < words.Length; a++)
                            {
                                oldText = currentText;
                                currentText += words[a] + " ";
                                text.text = currentText;
                                if (text.preferredHeight > text.rectTransform.rect.height || a + 1 >= words.Length)
                                {
                                    string temp = string.Empty;
                                    for (int b = a; b < words.Length; b++)
                                    {
                                        temp += words[b] + " ";
                                    }
                                    newList.Insert(i + 1, temp);
                                    pages.Add(oldText);
                                    moreTextOnPage = true;
                                    index = i + 1;
                                    break;
                                }
                            }
                            if (moreTextOnPage)
                            {
                                break;
                            }
                        }
                    }
                    if (moreTextOnPage)
                    {
                        break;
                    }
                    pages.Add(oldText);
                    break;
                }
                index = i;
                //textHeight = text.preferredHeight;
            }
        }
        text.text = pages[0];
        bool abcd = true;
        //int a = abcd ? 1 : 0;
    }

    private string MaxLengthOfString(ref int index, string[] strings)
    {
        float textHeight = 0;
        string currentText = string.Empty;
        string oldText = string.Empty;
        for (int i = index; i < strings.Length; i++)
        {
            oldText = currentText;
            currentText += strings[i];
            text.text = currentText;
            if (text.preferredHeight > text.rectTransform.rect.height || i + 1 >= strings.Length)
            {
                index = i + 1 >= strings.Length ? strings.Length : index;
                if (strings[i][0] != '<')
                {
                    string[] words = Regex.Split(strings[i], @"(?<=[ ])");
                    return MaxLengthOfString(ref index, words);
                }
                return (oldText);
                break;
            }
            index = i;
            //textHeight = text.preferredHeight;
        }
        return (currentText);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}