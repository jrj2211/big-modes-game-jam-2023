using System;
using TMPro;
using UnityEngine;

using Color = UnityEngine.Color;

class LineInfo
{
    public int index;
    public string line;
    public int offset;

    public LineInfo(string line, int index, int offset)
    {
        this.index = index;
        this.line = line;
        this.offset = offset;
    }
}

public class TypingController : MonoBehaviour
{
    public TextMeshProUGUI text;

    public string[] lines;
    public TMP_FontAsset font;

    public Color markColor;
    public Color typedColor;
    public Color untypedColor;

    private string encrpytionSymbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";


    private int cursorIndex;

    private TimerController timer;

    private bool penalized = false;

    private string combined;

    // Start is called before the first frame update
    void Start()
    {
        foreach(string line in lines)
        {
            combined += line;
        }

        timer = GetComponent<TimerController>();
        cursorIndex = 0;
        UpdateText();
    }

    private void Reset()
    {
        cursorIndex = 0;
        UpdateText();
    }

    private void Update()
    {
        if(Input.anyKeyDown && Input.inputString.Length > 0 && cursorIndex < combined.Length && timer.IsActive()) {
            ParseInput();
        }
    }

    void ParseInput()
    {
        string input = Input.inputString;
        foreach(char c in input)
        {
            if (char.IsWhiteSpace(c) || char.IsLetterOrDigit(c) || char.IsSymbol(c) || char.IsPunctuation(c)) {

                if (c == combined[cursorIndex])
                {
                    if (IsEndOfWord())
                    {
                        timer.TriggerWord();
                    }

                    cursorIndex += 1;

                    if(IsEndOfMessage())
                    {
                        timer.TriggerSuccess();
                    }
                }
                else
                {
                    if (penalized == false && timer.GetRemaining() > 10)
                    {
                        timer.TriggerError(true);

                        // Limit how many errors can penalize player over duration
                        penalized = true;
                        Invoke(nameof(ResetPenalty), 0.75f);
                    }
                    else
                    {
                        timer.TriggerError(false);
                    }
                }

                UpdateText();
            }
        }
    }

    bool IsEndOfMessage()
    {
        // Check if cursor is past the total length of the message
        return cursorIndex >= combined.Length;
    }

    bool IsEndOfWord()
    {
        // Get line that the cursor is currently in
        LineInfo li = GetCurrentLine();

        if(li.index + 1 < li.line.Length)
        {
            // Cursor is not at the end of the line so check if its on whitespace
            return char.IsWhiteSpace(li.line[li.index]);
        }
        
        // End of the line so that counts as end of a word
        return true;
    }

    LineInfo GetCurrentLine()
    {
        int previousLength = 0;

        // Loop through each line to figure out if the cursor is in it
        foreach(string line in lines) 
        {
            if(cursorIndex < previousLength + line.Length)
            {
                // Cursor is in the line
                return new LineInfo(line, cursorIndex - previousLength, previousLength);
            }

            // Not in it so add count and go to next line
            previousLength += line.Length;
        }

        // Cursor is not in any lines
        return null;
    }


    Range GetMarkRange()
    {
        if (IsEndOfMessage())
        {
            // Cursor at end of the message so we don't want to mark anything
            return new Range(int.MaxValue, int.MaxValue);
        }

        if (char.IsWhiteSpace(combined[cursorIndex]))
        {
            // Cursor is on whitespace so we just want to mark the single space
            return new Range(cursorIndex, cursorIndex);
        }

        // Find the line that cursor is in
        LineInfo li = GetCurrentLine();

        int start = li.index;
        int end = li.index;

        // Find previous space or start of string
        for (int s = li.index; s >= 0; s--)
        {
            if (char.IsWhiteSpace(li.line[s]))
            {
                break;
            }
            start = s;
        }

        // Find next space or end of string
        for (int s = li.index; s < li.line.Length; s++)
        {
            if (char.IsWhiteSpace(li.line[s]))
            {
                break;
            }
            end = s;
        }

        return new Range(start + li.offset, end + li.offset);
    }

    void UpdateText()
    {
        // Wrap the entire paragraph in a font block
        string textString = "<font=\"" + font.name + "\">";

        // Get the mark range denoting which word is currently being typed
        Range mark = GetMarkRange();

        // Running total for previous line char counts 
        int previousLineCount = 0;

        // Loop through each line to render it
        foreach (string line in lines)
        {
            // Start a no line break block to stop words from getting cut off
            textString += "<nobr>";

            // Conver the cursor 
            int lineCursorIndex = cursorIndex - previousLineCount;

            // Loop through each char in the line
            for (int i = 0; i < line.Length; i++)
            {

                // This character is the first marked character so start the block
                if (i == mark.Start.Value - previousLineCount)
                {
                    textString += "<mark=#" + ColorUtility.ToHtmlStringRGBA(markColor) + ">";
                }

                // Cache the current character so we can modify what gets printed
                string c = line[i].ToString();

                if (c.Equals(" "))
                {
                    // Replace whitespace with an additional zero width special character so the mark shows up
                    textString += "<color=#00000000> ؜</color>";
                }
                else
                {
                    // Encrypt future characters
                    if (i > lineCursorIndex + 10)
                    {
                        c = encrpytionSymbols[UnityEngine.Random.Range(0, encrpytionSymbols.Length)].ToString();
                    }

                    if (i < lineCursorIndex)
                    {
                        // Print typed colors in a color
                        textString += "<color=#" + ColorUtility.ToHtmlStringRGBA(typedColor) + ">" + c + "</color>";
                    }
                    else
                    {
                        // Print untyped colors in another color
                        textString += "<color=#" + ColorUtility.ToHtmlStringRGBA(untypedColor) + ">" + c + "</color>";
                    }
                }

                // This character is the last marked character so end the block
                if (i == mark.End.Value - previousLineCount)
                {
                    textString += "</mark>";
                }
            }

            // End no line break section
            textString += "</nobr>";

            // Add some line breaks for formatting
            textString += "<br><br>";

            // Add this lines length to the running character count
            previousLineCount += line.Length;
        }

        // End the font tag
        text.text = textString + "</font>";
    }

    void ResetPenalty()
    {
        penalized = false;
    }
}
