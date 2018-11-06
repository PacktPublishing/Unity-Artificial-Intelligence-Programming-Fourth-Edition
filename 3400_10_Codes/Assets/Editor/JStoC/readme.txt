UnityScript/Javascript to C# converter
By Mike Hergaarden from M2H - www.M2H.nl
Version: 10 December 2010

A tool to help converting your scripts from javascript to C#.

Easily convert JavaScript/UnityScript!
This editor tool saves you hours of work by allowing one-click converting to C# scripts. In some cases minor manual changes are required, but the script does the majority of the work for you making converting projects easily 10 times faster.

Thank you for buying this package from the Unity store.
To make this script easily worth your bucks start converting some scripts, add the package to your project. It's all about the "Editor/JStoC.c" file:
1. Select one or more JS files and select the new editor window option: "Tools/Convert selected JS file(s) to C#".
2. For every of the JS files you selected a C# copy will be made. Your old JS files will not be deleted.
3. Verify the conversion.


While the converter does the majority of the work, there are usually a few errors that you'll need to fix manually:
- VERY IMPORTANT: You WILL need to manually add StartCoroutine(MyFunction) for every IENumerator function; Unity does not complain about missing this, the calls will simpyly NEVER be executed.

Unity will complain about the following errors, so these are easier to fix:
- Where JS makes IENumerator of functions automatically you'll have to do this yourself in C# (for every function that's using yield, etc.). 
- If you used a lot of "loose" var in JS (without typing/strict mode) this script will not always be able to recognize the correct variable type and you will need to fix all instances of "FIXME_VAR_TYPE". 
- JS supports Array where in C# you neeed to use ArrayList, List, Hashtable, Dictionary.
- [there are some more exceptions...]


Disclaimer: I can not be held responsible for the outcome of the conversion, do validate the output.