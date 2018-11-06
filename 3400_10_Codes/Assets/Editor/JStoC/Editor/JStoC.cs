using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class JStoC : EditorWindow
{

    [MenuItem("Tools/Convert selected JS file(s) to C#")]
    static void ConvertJStoC()
    {
        UnityEngine.Object[] objects = Selection.GetFiltered(typeof(MonoScript), SelectionMode.Editable);
        int converted = 0;
        foreach (UnityEngine.Object obj in objects)
        {
            if (obj.GetType().ToString() == "UnityEditor.MonoScript")
            {

                string jsFile = AssetDatabase.GetAssetPath(obj);
                int jsEnd = jsFile.LastIndexOf(".js");
                if (jsEnd <= 0)
                {
                    Debug.LogError("JsToCs error: You did not select a .js file!");
                    return;
                }
                converted++;
                string cFilename = jsFile.Substring(0, jsEnd);
                cFilename += ".cs";

                if (AssetDatabase.LoadAssetAtPath(cFilename, typeof(MonoScript)) != null)
                {
                    Debug.LogError("JsToCs error: " + cFilename + " already exists!");
                    return;
                }

                string sourceCode = obj.ToString();
                //string cCode = OnlineConvert(sourceCode, obj.name);
                string cCode = ConvertToC(sourceCode, obj.name);
                if (cCode == "")
                {
                    Debug.LogError("Failed converting " + jsFile);
                    continue;
                }
                           
                try
                {
                    TextWriter tw = new StreamWriter(cFilename);
                    tw.Write(cCode);
                    tw.Close();
                }
                catch (System.IO.IOException IOEx)
                {
                    Debug.LogError("Incorrect file permissions? "+IOEx);
                }
                //Optional: Delete old JS file
                //AssetDatabase.DeleteAsset(jsFile);

                AssetDatabase.ImportAsset(cFilename, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
               

                Debug.Log("Converted " + Selection.activeObject.name + " to " + cFilename);

            }
        }
        if (converted < 1)
        {
            Debug.LogError("You did not select a JS file to convert! " + objects.Length + " files selected.");
        }
    }

    static string ConvertToC(string output, string className)
    {
        string VAR = @"[A-Za-z0-9_\[\]\.]";
        string VAR_NONARRAY = @"[A-Za-z0-9_]";

        string[] patterns = new string[32];
        string[] replacements = new string[32];
        int patrs = 0;
        int reps = 0;

        //var AAA;
        patterns[patrs++] = @"var(\s+)(" + VAR_NONARRAY + @"+)(\s*);";
        replacements[reps++] = "FIXME_VAR_TYPE $2;";
        // var AAAA : XXX	
        patterns[patrs++] = @"var(\s+)(" + VAR_NONARRAY + @"+)(\s*):(\s*)(" + VAR + @"+)";
        replacements[reps++] = "$5 $2";
        // var AAAA =	
        patterns[patrs++] = @"var(\s+)(" + VAR_NONARRAY + @"+)(\s*)=";
        replacements[reps++] = "FIXME_VAR_TYPE $2=";

        //Try to fix the "FIXME_VAR_TYPE" for strings;
        patterns[patrs++] = @"FIXME_VAR_TYPE(\s+)(" + VAR_NONARRAY + @"+)(\s*)=(\s*)""";
        replacements[reps++] = "string $2 = \"";
        //Try to fix the "FIXME_VAR_TYPE" for floats;
        patterns[patrs++] = @"FIXME_VAR_TYPE(\s+)(" + VAR_NONARRAY + @"+)(\s*)=(\s*)(([0-9\-]*\.+[0-9]+[f]?)+)(\s*);";
        replacements[reps++] = "float $2 = $5;";
        //Try to fix the "FIXME_VAR_TYPE" for int;
        patterns[patrs++] = @"FIXME_VAR_TYPE(\s+)(" + VAR_NONARRAY + @"+)(\s*)=(\s*)([0-9\-]+)(\s*);";
        replacements[reps++] = "float $2 = $5;";
        //Try to fix the "FIXME_VAR_TYPE" for bool true/false;
        patterns[patrs++] = @"FIXME_VAR_TYPE(\s+)(" + VAR_NONARRAY + @"+)(\s*)=(\s*)true(\s*);";
        replacements[reps++] = "bool $2 = true;";
        patterns[patrs++] = @"FIXME_VAR_TYPE(\s+)(" + VAR_NONARRAY + @"+)(\s*)=(\s*)false(\s*);";
        replacements[reps++] = "bool $2 = false;";

        //0.05f
        patterns[patrs++] = @"([\s\t\r\n\,=\( ]*)(\d+)\.(\d+)([\s\t\r\,;\) ]*)";
        replacements[reps++] = "$1$2.$3f$4";

        //Style
        patterns[patrs++] = "\n\n\n";
        replacements[reps++] = "\n";

        //RPC
        patterns[patrs++] = @"([\n\s\(\)\{\}\r]+)@RPC";
        replacements[reps++] = "$1[RPC]";

        //boolean
        patterns[patrs++] = @"([\n\t\s \(\)\{\}\r:;]*)\bboolean\b([\n\t\s \,\[=\(\)\{\}\r]+)";
        replacements[reps++] = "${1}bool${2} ";
        //String
        patterns[patrs++] = @"([\n\t\s \(\)\{\}\r:;]*)\bString\b([\n\t\s \,\[=\(\)\{\}\r]+)";
        replacements[reps++] = "${1}string${2}";

        //Remove #pragma strict	
        patterns[patrs++] = "#pragma strict";
        replacements[reps++] = "";
        patterns[patrs++] = "#pragma implicit";
        replacements[reps++] = "";
        patterns[patrs++] = "#pragma downcast";
        replacements[reps++] = "";


        //parseInt parseFloat
        patterns[patrs++] = "parseInt";
        replacements[reps++] = "int.Parse";
        patterns[patrs++] = "parseFloat";
        replacements[reps++] = "float.Parse";

        // (Rect(
        patterns[patrs++] = @"([]\(=)]+)[\s]*Rect[\s]*\(";
        replacements[reps++] = "${1} new Rect(";

        //Yield
        patterns[patrs++] = @"yield(\s+)(\w+);";
        replacements[reps++] = "yield return ${2};";
        patterns[patrs++] = @"yield;";
        replacements[reps++] = "yield return 0;";
        patterns[patrs++] = @"yield(\s+)(\w+)\(";
        replacements[reps++] = "yield return new ${2}(";
        patterns[patrs++] = @"yield new";
        replacements[reps++] = "yield return new";

        //For -> foreach
        patterns[patrs++] = @"for[\s]*\(([A-Za-z0-9_ :\,\.\[\]\s\n\r\t]*) in ([A-Za-z0-9_ :\,\.\s\n\r\t]*)\)";
        replacements[reps++] = "foreach($1 in $2)";

        //function rewrite
        patterns[patrs++] = @"function(\s+)(\w+)(\s*)\(([\n\r\tA-Za-z0-9_\[\]\*\/ \.:\,]*)\)(\s*):(\s*)(" + VAR + @"+)(\s*)\{";
        replacements[reps++] = "$7 $2 ($4){";
        //function rewrite
        patterns[patrs++] = @"function(\s+)(\w+)(\s*)\(([\n\r\tA-Za-z0-9_\[\]\*\/ \.:\,]*)\)(\s*)\{";
        replacements[reps++] = @"void $2 ($4){";

        //Getcomponent
        patterns[patrs++] = @"AddComponent\(([\n\r\tA-Za-z0-9_ ""]*)\)";
        replacements[reps++] = "AddComponent<$1>()";
        patterns[patrs++] = @"GetComponent\(([\n\r\tA-Za-z0-9_ ""]*)\)";
        replacements[reps++] = "GetComponent<$1>()";
        patterns[patrs++] = @"GetComponentsInChildren\(([\n\r\tA-Za-z0-9_ ""]*)\)";
        replacements[reps++] = "GetComponentsInChildren<$1>()";
        patterns[patrs++] = @"GetComponentInChildren\(([\n\r\tA-Za-z0-9_ ''""]*)\)";
        replacements[reps++] = "GetComponentInChildren<$1>()";
        patterns[patrs++] = @"FindObjectsOfType\(([\n\r\tA-Za-z0-9_ ""]*)\)";
        replacements[reps++] = "FindObjectsOfType(typeof($1))";
        patterns[patrs++] = @"FindObjectOfType\(([\n\r\tA-Za-z0-9_ ''""]*)\)";
        replacements[reps++] = "FindObjectOfType(typeof($1))";

        output = PregReplace(output, patterns, replacements);

        string before = "";
        while (before != output)
        {
            //( XX : YY) rewrite
            before = output;
            string patt = @"\(([\t\n\rA-Za-z0-9_*\/ \.\,\]\[]*)\b(\w+)\b(\s*):(\s*)(" + VAR + @"+)([\s\,]*)([\[\]\n\r\t\sA-Za-z0-9_\*\/ :\,\.]*)\)";
            string repp = "(${1} ${5} ${2} ${6} ${7})";//'(${1} 5--${5}-- 2--${2}-- 5--${6}-- 7=--${7})'
            output = Regex.Replace(output, patt, repp);
        }


        output = "using UnityEngine;\nusing System.Collections;\n\npublic class " + className + " : MonoBehaviour {\n" + output + "\n}";
        return output;
    }

    // If the Unity store updating doesn't work well enough I might allow online covnerting
    /*static string OnlineConvert(string input, string className)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("oldJS", input);
        WWW www = new WWW("http://www.m2h.nl/files/js_to_c.php?noprint=1", wwwForm);
        while (!www.isDone) { }

        if (www.error != null)
            Debug.LogError("Error converting: " + www.error);
        else
        {
            string cCode = www.text;
            cCode = cCode.Replace("MYCLASSNAME", className);
            return cCode;
        }
        return "";
    }*/



    static string PregReplace(string input, string[] pattern, string[] replacements)
    {
        if (replacements.Length != pattern.Length)
            throw new ArgumentException("Replacement and Pattern Arrays must be balanced");

        for (var i = 0; i < pattern.Length; i++)
        {
            input = Regex.Replace(input, pattern[i], replacements[i]);
        }

        return input;
    }


}