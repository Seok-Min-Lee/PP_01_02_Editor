using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticValues
{
    public static int password = -1;
    public static StudioDataRaw studioDataRaw = null;
    public static int filterNo = -1;
    public static EditorDataRaw editorDataRaw = null;

    public static void Init()
    {
        password = -1;
        studioDataRaw = null;
        filterNo = -1; 
        editorDataRaw = null;
    }
}
