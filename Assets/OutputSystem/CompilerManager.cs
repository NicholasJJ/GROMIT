using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using Mono.CSharp.Linq;

public class CustomCompilerError : Exception
{
    // Constructor with a custom error message
    public CustomCompilerError(string message) : base(message)
    {
    }
}


public class CompilerManager : OutputHandler
{
    [SerializeField] GameObject defaultObject;
    private static string baseImports = "using UnityEngine;\n using static Create;\n using System.Collections.Generic;\n";

    public Type CombinerCompiler(GameObject obj, string actionCode,bool throwErrors = true)
    {
        Debug.Log($"combining for object {obj.name}, code is:\n {actionCode}");
        string behaviour = CombinerMonoBehaviour(actionCode);
        Debug.Log($"Behaviour is:\n {behaviour}");
        var assembly = Compile(behaviour,throwErrors);
        Debug.Log("No errors so far!");
        //obj.AddComponent(assembly.GetType(ParseClassName(behaviour)));
        return assembly.GetType(ParseClassName(behaviour));
    }


    public override void HandleOutput(Output output)
    {
        if (output.code == null || output.code.Equals("")) return;
        try
        {
            if (output.code.IndexOf("MonoBehaviour") != -1)
            {
                //we got a component to add to a class
                string behaviour = output.code;
                behaviour = $"{baseImports} \n {behaviour}";
                string behaviourName = ParseClassName(behaviour);
                var assembly = Compile(behaviour,true);
                Debug.Log("Behaviour= " + behaviour);
                Debug.Log("bName= " + behaviourName);
                GameObject obj = defaultObject;
                if (output.objectName != null && !output.objectName.Equals("") && TextTree.Instance.StringToGameobject(output.objectName) != null)
                {
                    obj = TextTree.Instance.StringToGameobject(output.objectName);
                }
                Debug.Log("obj= " + obj.name);
                obj.AddComponent(assembly.GetType(behaviourName));
            }
            else
            {
                // we (probably) got a function to run once
                string methodCode = output.code;
                Action del = (Action)CompileMethod<Action>(methodCode);
                del.Invoke();
            }
            Timer.Instance.DisplaySandbox();
        }
        catch (CustomCompilerError e)
        {
            Debug.Log("Compilation failed! trying again");
            Timer.Instance.retries += 1;
            string redoPrompt = $"the code you gave failed to compile due to the following error: {e} \n try again, and remember to only use components that already exist!";
            ChatGPTWrapper.CustomGPT.Instance.SendToChatGPTAsSystem(redoPrompt);
        }
        
    }

    public static Delegate CompileMethod<T>(string methodCode)
    {
        try
        {
            string methodName = ParseFunctionName(methodCode);
            (string, string) newClass = ClassWrapper(methodCode);
            Debug.Log(newClass.Item2);
            Debug.Log("methodName= " + methodName);
            var assembly = Compile(newClass.Item2,true);
            var compiled = assembly.GetType(newClass.Item1);
            var method = compiled.GetMethod(methodName);
            Debug.Log("type = " + typeof(T));
            return Delegate.CreateDelegate(typeof(T), method);
        }
        catch (Exception e)
        {
            Debug.Log("compiler method error, throwing");
            throw e;
        }
    }

    public static Assembly Compile (string source, bool throwErrors = false) {
        var options = new CompilerParameters ();
        options.GenerateExecutable = false;
        options.GenerateInMemory = true;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies ()) {
            if (!assembly.IsDynamic) {
                options.ReferencedAssemblies.Add (assembly.Location);
            }
        }

        var compiler = new CSharpCompiler.CodeCompiler ();
        var result = compiler.CompileAssemblyFromSource (options, source);

        foreach (var err in result.Errors) {
            Debug.Log (err);
            if (throwErrors)
            {
                throw new CustomCompilerError(err.ToString());
            }
        }

        return result.CompiledAssembly;
    }

    public static (string,string) ClassWrapper(string method, string returnType = "void") {
        string dummyName = RandomString(10);
        List<string> usings = new List<string>();
        Match match = Regex.Match(method,@"(?:\busing\s+)(\w+)");
        if (match.Success) {
            for (int i = 1; i < match.Groups.Count; i++) {
                Group group = match.Groups[i];
                usings.Add("using " + group.Value + ";\n");
                method.Remove(group.Index,group.Length);
            }
        }
        // string pub = (method.IndexOf("public static",0,method.IndexOf("void")) == -1) ? "public" : "";
        method = method.Substring(method.IndexOf(returnType));
        string imports = string.Join("",usings.ToArray());
        string ret = $"{baseImports} \n {imports} \n public class {dummyName} {{ public static {method} }}";
        return (dummyName, ret);
    }

    public static string ParseClassName(string input)
    {
        // Regular expression pattern to match the class name
        string pattern = @"(?:\bclass\s+)(\w+)";
        
        return ParseString(input,pattern);
    }

    public static string ParseFunctionName(string input, string returnType = "void") {
        string pattern = @"(?:\b"+returnType+@"\s+)(\w+)";
        
        return ParseString(input,pattern);
    }

    private static string ParseString(string input, string pattern) {
        // Search for the pattern in the input string
        Match match = Regex.Match(input, pattern);

        // If a match is found, return the class name
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // If no match is found, return null
        return null;
    }

    public static string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }


    private static string CombinerMonoBehaviour(string action)
    {
        return "using System.Collections;\n" +
                "using System.Collections.Generic;\n" +
                "using TMPro;\n"+
                "using UnityEngine;\n" +
                "\n" +
                $"public class {RandomString(5)} : CombinerObject\n" +
                "{\n" +
                //"    public override void Action()\n" +
                //"    {\n" +
                action +
                //"    }\n" +
                "}";
    }
}
