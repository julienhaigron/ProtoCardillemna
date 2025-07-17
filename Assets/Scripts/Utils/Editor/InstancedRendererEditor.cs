using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;

[CustomEditor(typeof(InstancedRenderer))]
public class InstancedRendererEditor : UnityEditor.Editor
{
	private InstancedRenderer instancedRenderer;
	private Shader shader;
	private Material material;

	private GUIStyle bold;

	private void Awake ()
	{
		Undo.undoRedoPerformed += OnUndo;
	}

	void OnDestroy ()
	{
		Undo.undoRedoPerformed -= OnUndo;
	}

	private void OnUndo ()
	{
		if (instancedRenderer != null)
		{
			instancedRenderer.OnUndo();
		}
	}

	private void OnEnable ()
	{
		bold = new GUIStyle();
		bold.fontStyle = FontStyle.Bold;
		bold.alignment = TextAnchor.LowerLeft;
		bold.normal.textColor = Color.gray;
	}
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		instancedRenderer = target as InstancedRenderer;

		if (instancedRenderer.material == null)
		{
			GUIStyle style = new GUIStyle();
			style.fontStyle = FontStyle.BoldAndItalic;
			style.alignment = TextAnchor.MiddleCenter;
			style.normal.textColor = Color.red;
			EditorGUILayout.LabelField("Missing Material", style);
			return;
		}

		material = instancedRenderer.material;

		shader = material.shader;
		if (GUILayout.Button("Clean Serialized Property"))
		{
			instancedRenderer.ClearSerializedProperty();
		}

		//if(instancedRenderer.attachedVisual != null)
		//{
		//	EditorGUI.BeginChangeCheck();
		//	instancedRenderer.reference = (InstancedRenderer.PropertyBlockReference)EditorGUILayout.EnumPopup("Reference (Reset All)", instancedRenderer.reference);

		//	if (EditorGUI.EndChangeCheck())
		//		instancedRenderer.ResetAllProperty();

		//	foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Attached Renderer");

		//	if (foldout)
		//	{
		//		foreach (Renderer r in instancedRenderer.attachedRenderer)
		//		{
		//			if(r == null)
		//			{
		//				instancedRenderer.attachedVisual.InitialiseInstancedRenderer();
		//				break;
		//			}
		//			EditorGUILayout.LabelField(r.gameObject.name);
		//		}
		//	}

		//	EditorGUILayout.EndFoldoutHeaderGroup();
		//}

		GUILayout.Space(10);
		EditorGUILayout.LabelField("Material Properties", bold);

		for (int i = 0; i < shader.GetPropertyCount(); i++)
		{
			DrawMaterialProperty(i);
		}
		if (GUI.changed)
			SceneView.RepaintAll();
	}

	private void DrawMaterialProperty ( int _index )
	{
		if (string.IsNullOrEmpty(shader.GetPropertyDescription(_index)))
			return;

		if (!instancedRenderer.ShouldDrawProperty(_index, out string header))
			return;

		if (!string.IsNullOrEmpty(header))
		{
			GUIStyle bold = GUIStyle.none;
			bold.fontStyle = FontStyle.Bold;
			bold.alignment = TextAnchor.LowerLeft;
			bold.normal.textColor = Color.gray;

			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField(header, bold);
		}

		instancedRenderer.AddSerializedProperty(_index, material);

		string propertyName = shader.GetPropertyName(_index);

		switch (shader.GetPropertyType(_index))
		{
			case ShaderPropertyType.Color:
				DrawColorProperty();
				break;
			case ShaderPropertyType.Vector:
				DrawVectorProperty();
				break;
			case ShaderPropertyType.Float:
				DrawFloatProperty();
				break;
			case ShaderPropertyType.Range:
				DrawRangeProperty(HasIntRangeAttribute(_index));
				break;
			case ShaderPropertyType.Texture:
				break;
			default:
				break;
		}

		void DrawColorProperty ()
		{
			if (!instancedRenderer.serializedPropertyValues.ContainsKey(propertyName))
				return;

			bool hasChangedProperty = instancedRenderer.serializedPropertyValues[propertyName].hasChanged;

			Color reference = instancedRenderer.serializedPropertyValues[propertyName].colorValue;

			if (!hasChangedProperty)
			{
				if (reference != material.GetColor(propertyName))
				{
					instancedRenderer.SetColor(propertyName, material.GetColor(propertyName));
				}
			}

			GUIStyle style = new GUIStyle();
			style.fontStyle = hasChangedProperty ? FontStyle.Bold : FontStyle.Normal;
			style.alignment = TextAnchor.LowerLeft;
			style.normal.textColor = hasChangedProperty ? Color.blue : Color.gray;

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(shader.GetPropertyDescription(_index), style);
			Color color = EditorGUILayout.ColorField(reference);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(instancedRenderer, "Instanced Renderer Color Modif");
				instancedRenderer.SetColor(propertyName, color);
			}

			DrawResetButton(propertyName, hasChangedProperty);

			GUILayout.EndHorizontal();
		}

		void DrawVectorProperty ()
		{
			if (!instancedRenderer.serializedPropertyValues.ContainsKey(propertyName))
				return;

			bool hasChangedProperty = instancedRenderer.serializedPropertyValues[propertyName].hasChanged;

			Vector4 reference = instancedRenderer.serializedPropertyValues[propertyName].vectorValue;

			if (!hasChangedProperty)
			{
				if (reference != material.GetVector(propertyName))
				{
					instancedRenderer.SetVector(propertyName, material.GetVector(propertyName));
				}
			}

			GUIStyle style = new GUIStyle();
			style.fontStyle = hasChangedProperty ? FontStyle.Bold : FontStyle.Normal;
			style.alignment = TextAnchor.LowerLeft;
			style.normal.textColor = hasChangedProperty ? Color.blue : Color.gray;

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(shader.GetPropertyDescription(_index), style);
			Vector4 vector4 = EditorGUILayout.Vector4Field("", reference);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(instancedRenderer, "Instanced Renderer Vector Modif");
				instancedRenderer.SetVector(propertyName, vector4);
			}

			DrawResetButton(propertyName, hasChangedProperty);

			GUILayout.EndHorizontal();
		}

		void DrawFloatProperty ()
		{
			if (!instancedRenderer.serializedPropertyValues.ContainsKey(propertyName))
				return;

			bool hasChangedProperty = instancedRenderer.serializedPropertyValues[propertyName].hasChanged;

			float reference = instancedRenderer.serializedPropertyValues[propertyName].floatValue;

			if (!hasChangedProperty)
			{
				if (reference != material.GetFloat(propertyName))
				{
					instancedRenderer.SetFloat(propertyName, material.GetFloat(propertyName));
				}
			}

			GUIStyle style = new GUIStyle();
			style.fontStyle = hasChangedProperty ? FontStyle.Bold : FontStyle.Normal;
			style.alignment = TextAnchor.LowerLeft;
			style.normal.textColor = hasChangedProperty ? Color.blue : Color.gray;

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(shader.GetPropertyDescription(_index), style);

			float value = EditorGUILayout.FloatField(reference);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(instancedRenderer, "Instanced Renderer Float Modif");
				instancedRenderer.SetFloat(propertyName, value);
			}

			DrawResetButton(propertyName, hasChangedProperty);

			GUILayout.EndHorizontal();
		}

		void DrawRangeProperty ( bool intRange )
		{
			if (!instancedRenderer.serializedPropertyValues.ContainsKey(propertyName))
				return;

			bool hasChangedProperty = instancedRenderer.serializedPropertyValues[propertyName].hasChanged;

			float reference = instancedRenderer.serializedPropertyValues[propertyName].floatValue;

			if (!hasChangedProperty)
			{
				if (reference != material.GetFloat(propertyName))
				{
					instancedRenderer.SetFloat(propertyName, material.GetFloat(propertyName));
				}
			}

			GUIStyle style = new GUIStyle();
			style.fontStyle = hasChangedProperty ? FontStyle.Bold : FontStyle.Normal;
			style.alignment = TextAnchor.LowerLeft;
			style.normal.textColor = hasChangedProperty ? Color.blue : Color.gray;

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(shader.GetPropertyDescription(_index), style);

			Vector2 range = shader.GetPropertyRangeLimits(_index);
			float value;
			if (!intRange)
			{
				value = EditorGUILayout.Slider(reference, range.x, range.y);
			}
			else
			{
				value = Mathf.Round(EditorGUILayout.Slider(reference, range.x, range.y));
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(instancedRenderer, "Instanced Renderer Float Modif");
				instancedRenderer.SetFloat(propertyName, value);
			}

			DrawResetButton(propertyName, hasChangedProperty);

			GUILayout.EndHorizontal();
		}
	}

	private void DrawResetButton ( string _propertyName, bool _hasChangedProperty )
	{
		GUIStyle buttonStyle = new GUIStyle();
		if (_hasChangedProperty)
		{
			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.normal.textColor = Color.blue;
		}
		else
		{
			buttonStyle.normal.textColor = Color.gray;
			buttonStyle.alignment = TextAnchor.MiddleCenter;
		}

		buttonStyle.margin = new RectOffset(0, 0, 0, 0);
		buttonStyle.border = new RectOffset(0, 0, 0, 0);
		buttonStyle.fixedWidth = 48;

		if (GUILayout.Button("Reset", buttonStyle))
		{
			instancedRenderer.ResetProperty(_propertyName);
			EditorUtility.SetDirty(instancedRenderer);
		}
	}

	private bool HasIntRangeAttribute ( int _index )
	{
		foreach (string a in shader.GetPropertyAttributes(_index))
		{
			if (Regex.IsMatch(a, "IntRange", RegexOptions.IgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
}