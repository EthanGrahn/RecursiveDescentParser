using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    [SerializeField]
    private Button parseButton;
    [SerializeField]
    private Text inputText;

    private Queue<string> currSymbol = new Queue<string>();
    private int index = 0;
    private List<string> grammarTokens = new List<string>(new string[] 
    {"program",
    "block",
    "stmtlist",
    "morestmts",
    "stmt",
    "assign",
    "ifstmt",
    "whilestmt",
    "testexpr",
    "expr",
    "variable",
    "digit"});

    private int assignmentStatements = 0;
    private int variableReferences = 0;
    
	void Start () {
        parseButton.onClick.AddListener(Parse);
	}

    private void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            FillSymbols(inputText.text);
        }
    }

    void FillSymbols(string input)
    {
        string[] tmp = Regex.Split(input, @"\s+");
        for (int i = 0; i < tmp.Length; i++)
        {
            currSymbol.Enqueue(tmp[i]);
        }
        //foreach (string s in tmp)
        //{
        //    Debug.Log(s);
        //}
    }

    void Parse()
    {
        FillSymbols(inputText.text);
        Debug.Log("first == " + currSymbol.Peek() + " || count = " + currSymbol.Count);
    }

#region Grammar Functions
    bool Accept(string s)
    {
        if (currSymbol.Peek() == s)
        {
            currSymbol.Dequeue();
            return true;
        }
        else if (grammarTokens.Contains(s))
            return true;

        return false;
    }

    bool Expect(string s)
    {
        if (Accept(s))
            return true;
        //error
        Debug.Log("error: unexpected symbol - " + s);
        return false;
    }

    void NextSymbol()
    {
        currSymbol.Dequeue();
    }

    void Program()
    {
        if (Expect("program"))
        {
            Block();
            Expect(".");
        }
    }

    void Block()
    {
        if (Expect("begin"))
        {
            StmtList();
            Expect("end");
        }
    }

    void StmtList()
    {
        Stmt();
        MoreStmts();
    }

    void MoreStmts()
    {
        if (Accept(";"))
        {
            StmtList();
        }
    }

    void Stmt()
    {
        if (Accept("assign"))
            Assign();
        else if (Accept("ifstmt"))
            IfStmt();
        else if (Accept("whilestmt"))
            WhileStmt();
        else if (Accept("block"))
            Block();
    }

    void Assign()
    {
        Variable();
        if (currSymbol.Peek() == "=")
        {
            Expr();
        }
    }

    void IfStmt()
    {
        if (Accept("if"))
        {
            TestExpr();
            if (Accept("then"))
            {
                Stmt();
                if (Accept("else"))
                    Stmt();
            }
        }
    }

    void WhileStmt()
    {
        Expect("while");
        TestExpr();
        Expect("do");
        Stmt();
    }

    void TestExpr()
    {
        if (Accept("variable"))
        {
            Expect("<=");
            Expr();
        }
    }

    void Expr()
    {
        if (Accept("+"))
        {
            Expr();
            Expr();
        }
        else if (Accept("*"))
        {
            Expr();
            Expr();
        }
        else if (Accept("variable"))
        {
            Variable();
        }
        else if (Accept("digit"))
        {
            Digit();
        }
    }

    void Variable()
    {
        if (Accept("a"))
            ;
        else if (Accept("b"))
            ;
        else if (Accept("c"))
            ;
    }

    void Digit()
    {
        if (Accept("0"))
            ;
        else if (Accept("1"))
            ;
        else if (Accept("2"))
            ;
    }
#endregion
}
