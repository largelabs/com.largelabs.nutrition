using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviourBase), true)]
public class MonoBehaviourBaseInspector : Editor
{
    Dictionary<string, MethodInfo> publicMethodInfos = null;
    Dictionary<string, ParameterInfo[]> publicMethodParams = null;
    Dictionary<string, object[]> publicMethodParamsObj = null;
    Dictionary<ParameterInfo, Dictionary<FieldInfo, object>> parameterFieldsObj = null;
    Dictionary<string, ExposePublicMethod> publicMethodAttributes = null;

    #region UNITY AND CORE

    private void OnEnable()
    {
        refreshPublicMethodInfo();
    }

    public override void OnInspectorGUI()
    {
        EditorStyles.miniLabel.wordWrap = true;

        base.OnInspectorGUI();

        GUILayout.Space(10);

        publicMethodsUI();

        GUILayout.Space(10);
        GUILayout.FlexibleSpace();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    #endregion

    #region PRIVATE

    void refreshPublicMethodInfo()
    {
        if (null == target) return;

        if (null == publicMethodInfos) publicMethodInfos = new Dictionary<string, MethodInfo>();
        if (null == publicMethodParams) publicMethodParams = new Dictionary<string, ParameterInfo[]>();
        if (null == publicMethodParamsObj) publicMethodParamsObj = new Dictionary<string, object[]>();
        if (null == publicMethodAttributes) publicMethodAttributes = new Dictionary<string, ExposePublicMethod>();
        if (null == parameterFieldsObj) parameterFieldsObj = new Dictionary<ParameterInfo, Dictionary<FieldInfo, object>>();

        publicMethodInfos.Clear();
        publicMethodParams.Clear();
        publicMethodParamsObj.Clear();
        parameterFieldsObj.Clear();

        System.Type exposeAttributeType = typeof(ExposePublicMethod);

        MethodInfo[] methods = target.GetType()
           .GetMethods(BindingFlags.Public | BindingFlags.Instance)
           .Where(y => y.GetCustomAttributes().OfType<ExposePublicMethod>().Any()).ToArray();

        int count = methods.Length;
        MethodInfo curr = null;
        string methodName = null;
        ParameterInfo[] currParams = null;
        for (int i = 0; i < count; i++)
        {
            curr = methods[i];
            methodName = curr.Name;
            currParams = curr.GetParameters();
            int paramsCount = currParams.Length;

            publicMethodInfos.Add(methodName, curr);
            publicMethodParams.Add(methodName, currParams);
            publicMethodParamsObj.Add(methodName, new object[paramsCount]);
            publicMethodAttributes.Add(methodName, curr.GetCustomAttribute(exposeAttributeType) as ExposePublicMethod);

            foreach (ParameterInfo param in currParams)
            {
                FieldInfo[] paramFieldInfo = param.ParameterType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                if (paramFieldInfo != null && paramFieldInfo.Length > 0)
                {
                    Dictionary<FieldInfo, object> fieldDic = new Dictionary<FieldInfo, object>();
                    parameterFieldsObj.Add(param, fieldDic);

                    foreach (FieldInfo field in paramFieldInfo)
                    {
                        fieldDic.Add(field, null);
                    }
                }
            }
        }
    }

    void publicMethodsUI()
    {
        if (null == publicMethodInfos) return;
        if (null == target) return;

        EditorGUI.indentLevel++;
        foreach (KeyValuePair<string, MethodInfo> pair in publicMethodInfos)
        {
            ExposePublicMethod attr = publicMethodAttributes[pair.Key];

            bool show = EditorApplication.isPlaying;

            if(true == show)
            {
                EditorWidgets.TitleUI(pair.Key);

                if (true == GUILayout.Button(pair.Key, GUILayout.Height(25), GUILayout.Width(200)))
                {
                    pair.Value.Invoke(target, publicMethodParamsObj[pair.Key]);
                }

                publicMethodParamsUI(publicMethodParams[pair.Key], publicMethodParamsObj[pair.Key]);

                EditorWidgets.SeparatorUI();
            }

        }

        EditorGUI.indentLevel--;

    }

    void publicMethodParamsUI(ParameterInfo[] i_parameters, object[] i_params)
    {
        if (null == i_params) return;

        int count = i_params.Length;

        for (int i = 0; i < count; i++)
        {
            i_params[i] = systemObjectFieldEditorUI(i_parameters[i], i_params[i]);
        }
    }

    object systemObjectFieldEditorUI(ParameterInfo i_parameter, object i_obj)
    {
        System.Type type = i_parameter.ParameterType;

        i_obj = activateObject(type, i_obj);

        if (null == i_obj)
        {
            EditorWidgets.InfoLabelUI("ParameterType " + type.Name + " not supported.", EditorWidgets.InfoLabelType.Invalid);
        }
        else
        {
            object result = default;
            bool success = tryGetGUIValue(type, i_parameter.Name, i_obj, false, out result);

            if (false == success)
            {
                if (type.IsPrimitive == false && parameterFieldsObj.ContainsKey(i_parameter))
                {
                    Dictionary<FieldInfo, object> fieldInfoDic = parameterFieldsObj[i_parameter];
                    Dictionary<FieldInfo, object> fieldInfoDicBuffer = null;

                    EditorWidgets.TitleUI(i_parameter.Name, true);

                    foreach (KeyValuePair<FieldInfo, object> pair in fieldInfoDic)
                    {
                        FieldInfo field = pair.Key;

                        object fieldObj = activateObject(field.FieldType, pair.Value);

                        EditorGUI.indentLevel++;
                        success = tryGetGUIValue(field.FieldType, field.Name, fieldObj, true, out result);
                        EditorGUI.indentLevel--;

                        if (success)
                        {
                            if (null == fieldInfoDicBuffer)
                                fieldInfoDicBuffer = new Dictionary<FieldInfo, object>();

                            field.SetValue(i_obj, result);
                            fieldInfoDicBuffer.Add(field, result);
                        }
                    }

                    EditorWidgets.TitleUI("", true);

                    if (null != fieldInfoDicBuffer)
                    {
                        foreach (KeyValuePair<FieldInfo, object> pair in fieldInfoDicBuffer)
                        {
                            fieldInfoDic[pair.Key] = pair.Value;
                        }
                    }

                    return i_obj;
                }
                else
                {
                    EditorWidgets.InfoLabelUI("ParameterType " + type.Name + " not supported.", EditorWidgets.InfoLabelType.Invalid);
                }
            }
            else
                return result;
        }

        return default;
    }

    object activateObject(System.Type i_type, object i_obj)
    {
        if (null == i_obj || false == i_obj.GetType().Equals(i_type))
        {
            if (true == i_type.IsValueType)
            {
                return Activator.CreateInstance(i_type);
            }
            else if (true == i_type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return null;
            }
            else
                return null;
        }
        else
            return i_obj;
    }

    bool tryGetGUIValue(System.Type i_type, string i_name, object i_obj, bool i_printErrorOnMiss, out object i_result)
    {
        i_result = default;
        bool success = true;

        if (true == i_type.IsEnum)
        {
            i_result = EditorGUILayout.EnumPopup(i_name, i_obj as System.Enum);
        }
        else if (i_type.Equals(typeof(int)))
        {
            i_result = EditorGUILayout.IntField(i_name, (int)i_obj);
        }
        else if (i_type.Equals(typeof(float)))
        {
            i_result = EditorGUILayout.FloatField(i_name, (float)i_obj);
        }
        else if (i_type.Equals(typeof(bool)))
        {
            i_result = EditorGUILayout.ToggleLeft(i_name, (bool)i_obj);
        }
        else if (i_type.Equals(typeof(Vector2Int)))
        {
            i_result = EditorGUILayout.Vector2IntField(i_name, (Vector2Int)i_obj);
        }
        else if (i_type.Equals(typeof(Vector2)))
        {
            i_result = EditorGUILayout.Vector2Field(i_name, (Vector2)i_obj);
        }
        else if (i_type.Equals(typeof(Vector3)))
        {
            i_result = EditorGUILayout.Vector3Field(i_name, (Vector3)i_obj);
        }
        else if (i_type.Equals(typeof(Vector3Int)))
        {
            i_result = EditorGUILayout.Vector3IntField(i_name, (Vector3Int)i_obj);
        }
        else
        {
            if (i_printErrorOnMiss)
                EditorWidgets.InfoLabelUI("ParameterType " + i_type.Name + " not supported.",
                                            EditorWidgets.InfoLabelType.Invalid);

            success = false;
        }

        return success;
    }

    #endregion
}
