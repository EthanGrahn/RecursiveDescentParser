using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    [SerializeField]
    private Button parseButton;
    [SerializeField]
    private InputField inputText;
    [SerializeField]
    private Text variableText;
    [SerializeField]
    private Text assignmentText;
    [SerializeField]
    private ParseLog parseLog;

    private Queue<string> currSymbol = new Queue<string>();
    private HashSet<string> nonSpaceSymbols = new HashSet<string>
    {
        "<=", "+", "-", ";", ".", "*"
    };

    private int assignmentStatements = 0;
    private int variableReferences = 0;
    
	void Start () {
        parseButton.onClick.AddListener(Parse);
	}

    public void Clear(bool beforeParse)
    {
        currSymbol = new Queue<string>();
        parseLog.Clear();
        assignmentStatements = 0;
        variableReferences = 0;
        variableText.text = "";
        assignmentText.text = "";
        if (!beforeParse)
            inputText.text = "";
    }

    void FillSymbols(string input)
    {
        foreach (string s in nonSpaceSymbols)
        {
            input = input.Replace(s, " " + s + " ");
        }

        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == '=' && input[i - 1] != '<')
            {
                input = input.Insert(i - 1, " ");
                i++;
                input = input.Insert(i, " ");
                i++;
            }
        }

        string[] tmp = Regex.Split(input, @"\s+");
        for (int i = 0; i < tmp.Length; i++)
        {
                currSymbol.Enqueue(tmp[i]);
        }
        parseLog.AddItem("Loaded " + currSymbol.Count.ToString() + " symbols", Color.white);
        //foreach (string s in tmp)
        //{
        //    Debug.Log(s);
        //}
    }

    void Parse()
    {
        Clear(true);
        FillSymbols(inputText.text);
        Program();
        variableText.text = variableReferences.ToString();
        assignmentText.text = assignmentStatements.ToString();
    }

#region Grammar Functions
    bool Accept(string s)
    {
        if (currSymbol.Peek() == s)
        {
            currSymbol.Dequeue();
            return true;
        }

        return false;
    }

    bool Expect(string s)
    {
        if (Accept(s))
            return true;

        parseLog.AddItem("Unexpected Symbol: " + currSymbol.Peek(), Color.red);
        throw new UnityException("ERROR: unexpected symbol - " + currSymbol.Peek());
    }

    void Program()
    {
        parseLog.AddItem("Checking for <program>", Color.white);
        if (Expect("program"))
        {
            Block();
            Expect(".");
            parseLog.AddItem("Accepted <program>", Color.green);
        }
    }

    void Block()
    {
        parseLog.AddItem("Checking for <block>", Color.white);

        if (Expect("begin"))
        {
            StmtList();
            Expect("end");
            parseLog.AddItem("Accepted <block>", Color.green);
        }
    }

    void StmtList()
    {
        parseLog.AddItem("Checking for <stmtlist>", Color.white);
        Stmt();
        MoreStmts();
        parseLog.AddItem("Accepted <stmtlist>", Color.green);
    }

    void MoreStmts()
    {
        parseLog.AddItem("Checking for <morestmts>", Color.white);

        if (Accept(";"))
        {
            int start = currSymbol.Count;
            StmtList();
            if (start > currSymbol.Count) // the difference in queue size indicates it wasn't empty after the ;
                parseLog.AddItem("Accepted <morestmts>", Color.green);
            else
            {
                parseLog.AddItem("Error: ';' without <stmtlist>", Color.red);
                throw new UnityException("Error: ';' without <stmtlist>");
            }
        }
        else
            parseLog.AddItem("Accepted <morestmts> as empty", Color.green);
    }

    void Stmt()
    {
        parseLog.AddItem("Checking for <stmt>", Color.white);

        if (currSymbol.Peek() == "if")
        {
            IfStmt();
            parseLog.AddItem("Accepted <stmt>", Color.green);
        }
        else if (currSymbol.Peek() == "while")
        {
            WhileStmt();
            parseLog.AddItem("Accepted <stmt>", Color.green);
        }
        else if (currSymbol.Peek() == "begin")
        {
            Block();
            parseLog.AddItem("Accepted <stmt>", Color.green);
        }
        else if (currSymbol.Peek() == "a" || currSymbol.Peek() == "b" || currSymbol.Peek() == "c")
        {
            Assign();
            parseLog.AddItem("Accepted <stmt>", Color.green);
        }
        else
            parseLog.AddItem("<stmt> not found", Color.yellow);
    }

    void Assign()
    {
        parseLog.AddItem("Checking for <assign>", Color.white);
        Variable();
        if (Expect("="))
        {
            Expr();
            assignmentStatements++;
            parseLog.AddItem("Accepted <assign>", Color.green);
        }
    }

    void IfStmt()
    {
        parseLog.AddItem("Checking for <ifstmt>", Color.white);
        if (Accept("if"))
        {
            TestExpr();
            if (Expect("then"))
            {
                Stmt();
                if (Expect("else"))
                {
                    Stmt();
                    parseLog.AddItem("Accepted <ifstmt>", Color.green);
                }
            }
        }
        else
            parseLog.AddItem("<ifstmt> not found", Color.yellow);
    }

    void WhileStmt()
    {
        parseLog.AddItem("Checking for <while>", Color.white);
        Expect("while");
        TestExpr();
        Expect("do");
        Stmt();
        parseLog.AddItem("Accepted <while>", Color.green);
    }

    void TestExpr()
    {
        parseLog.AddItem("Checking for <testexpr>", Color.white);
        Variable();
        Expect("<=");
        Expr();
        parseLog.AddItem("Accepted <testexpr>", Color.green);
    }

    void Expr()
    {
        parseLog.AddItem("Checking for <expr>", Color.white);
        if (Accept("+"))
        {
            Expr();
            Expr();
            parseLog.AddItem("Accepted <expr> as addition", Color.green);
        }
        else if (Accept("*"))
        {
            Expr();
            Expr();
            parseLog.AddItem("Accepted <expr> as multiplication", Color.green);
        }
        else
        {
            if (currSymbol.Peek() == "a" || currSymbol.Peek() == "b" || currSymbol.Peek() == "c")
            {
                Variable();
                parseLog.AddItem("Accepted <expr> as a variable", Color.green);
            }
            else if (currSymbol.Peek() == "0" || currSymbol.Peek() == "1" || currSymbol.Peek() == "2")
            {
                Digit();
                parseLog.AddItem("Accepted <expr> as a digit", Color.green);
            }
            else
                parseLog.AddItem("<expr> not found", Color.yellow);
        }
    }

    void Variable()
    {
        parseLog.AddItem("Checking for <variable>", Color.white);
        if (Accept("a"))
        {
            parseLog.AddItem("Accepted <variable> as 'a'", Color.green);
            variableReferences++;
        }
        else if (Accept("b"))
        {
            parseLog.AddItem("Accepted <variable> as 'b'", Color.green);
            variableReferences++;
        }
        else if (Accept("c"))
        {
            parseLog.AddItem("Accepted <variable> as 'c'", Color.green);
            variableReferences++;
        }
        else
            parseLog.AddItem("<variable> not found", Color.yellow);
    }

    void Digit()
    {
        parseLog.AddItem("Checking for <digit>", Color.white);
        if (Accept("0"))
            parseLog.AddItem("Accepted <digit> as '0'", Color.green);
        else if (Accept("1"))
            parseLog.AddItem("Accepted <digit> as '1'", Color.green);
        else if (Accept("2"))
            parseLog.AddItem("Accepted <digit> as '2'", Color.green);
        else
            parseLog.AddItem("<digit> not found", Color.yellow);
    }
#endregion
}
