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

    private Queue<string> currSymbol = new Queue<string>();
    private int index = 0;

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

        throw new UnityException("ERROR: unexpected symbol - " + currSymbol.Peek());
    }

    void Program()
    {
        Debug.Log("->program");
        if (Expect("program"))
        {
            Block();
            Expect(".");
            Debug.Log("<-program");
        }
    }

    void Block()
    {
        Debug.Log("->block");
        if (Expect("begin"))
        {
            StmtList();
            Expect("end");
            Debug.Log("<-block");
        }
    }

    void StmtList()
    {
        Debug.Log("->stmtlist");
        Stmt();
        MoreStmts();
        Debug.Log("<-stmtlist");
    }

    void MoreStmts()
    {
        Debug.Log("->morestmts");
        if (Accept(";"))
        {
            StmtList();
            Debug.Log("<-morestmts");
        }
    }

    void Stmt()
    {
        Debug.Log("->stmt");
        if (currSymbol.Peek() == "if")
            IfStmt();
        else if (currSymbol.Peek() == "while")
            WhileStmt();
        else if (currSymbol.Peek() == "begin")
            Block();
        else if (currSymbol.Peek() == "a" || currSymbol.Peek() == "b" || currSymbol.Peek() == "c")
            Assign();
        Debug.Log("<-stmt");
    }

    void Assign()
    {
        Debug.Log("->assign");
        Variable();
        if (Expect("="))
        {
            Expr();
            assignmentStatements++;
            Debug.Log("<-assign");
        }
    }

    void IfStmt()
    {
        Debug.Log("->ifstmt");
        if (Accept("if"))
        {
            TestExpr();
            if (Expect("then"))
            {
                Stmt();
                if (Expect("else"))
                {
                    Stmt();
                    Debug.Log("<-ifstmt");
                }
            }
        }
    }

    void WhileStmt()
    {
        Debug.Log("->while");
        Expect("while");
        TestExpr();
        Expect("do");
        Stmt();
        Debug.Log("<-while");
    }

    void TestExpr()
    {
        Debug.Log("->testexpr");
        Variable();
        Expect("<=");
        Expr();
        Debug.Log("<-testexpr");
    }

    void Expr()
    {
        Debug.Log("->expr");
        if (Accept("+"))
        {
            Expr();
            Expr();
            Debug.Log("<-expr+");
        }
        else if (Accept("*"))
        {
            Expr();
            Expr();
            Debug.Log("<-expr*");
        }
        else
        {
            if (currSymbol.Peek() == "a" || currSymbol.Peek() == "b" || currSymbol.Peek() == "c")
            {
                Variable();
                Debug.Log("<-exprV");
            }
            else if (currSymbol.Peek() == "0" || currSymbol.Peek() == "1" || currSymbol.Peek() == "2")
            {
                Digit();
                Debug.Log("<-exprD");
            }
        }
    }

    void Variable()
    {
        Debug.Log("->variable");
        if (Accept("a"))
        {
            Debug.Log("<-variableA");
            variableReferences++;
        }
        else if (Accept("b"))
        {
            Debug.Log("<-variableB");
            variableReferences++;
        }
        else if (Accept("c"))
        {
            Debug.Log("<-variableC");
            variableReferences++;
        }
    }

    void Digit()
    {
        Debug.Log("->digit");
        if (Accept("0") || Accept("1") || Accept("2"))
            Debug.Log("<-digit");
    }
#endregion
}
