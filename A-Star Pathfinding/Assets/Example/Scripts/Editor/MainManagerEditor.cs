using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MainManager))]
public class MainManagerEditor : Editor {

    public override void OnInspectorGUI() {

        MainManager manager = (MainManager)target;

        GUILayout.BeginHorizontal();
        GUIStyle style = new GUIStyle();
        GUI.color = new Color(.8f, 1, .8f);
        if (GUILayout.Button("Set Play Mode")) {
            manager.playerMoney = 50;
            manager.playerHp = 10;
        }
        GUI.color = new Color(1f, .8f, .8f);
        if (GUILayout.Button("Set Debug Mode")) {
            manager.playerMoney = 500;
            manager.playerHp = 100;
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();


    }


}
