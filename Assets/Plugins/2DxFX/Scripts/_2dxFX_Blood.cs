﻿//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2015 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("2DxFX/Standard/Blood")]
[System.Serializable]
public class _2dxFX_Blood : _moiling_2dxFX_BaseClass
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/Blood";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

	[HideInInspector] public Texture2D __MainTex2; 
	[HideInInspector] [Range(0f, 1f)] public float TurnToLiquid = 0.052f;
	[HideInInspector] [Range(0.0f, 1f)] public float Heat =  1.0f;
	[HideInInspector] [Range(0.0f, 4f)] public float Speed = 1.0f;
	[HideInInspector] [Range(0.0f, 1f)] public float EValue = 1.0f;
	[HideInInspector] [Range(-4.0f, 4f)] public float Light = 3.0f;


	[HideInInspector] public int ShaderChange=0;
	Material tempMaterial;
	Material defaultMaterial;

	
	void Start ()
	{ 
		__MainTex2 = Resources.Load ("_2dxFX_WaterTXT") as Texture2D;
		ShaderChange = 0;
		rendererMaterial.SetTexture ("_MainTex2", __MainTex2);
	}

 	public void CallUpdate()
	{
		Update ();
	}

	void Update()
	{	
		if ((ShaderChange == 0) && (ForceMaterial != null)) 
		{
			ShaderChange=1;
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			rendererMaterial = ForceMaterial;
			ForceMaterial.hideFlags = HideFlags.None;
			ForceMaterial.shader=Shader.Find(shader);
			

		}
		if ((ForceMaterial == null) && (ShaderChange==1))
		{
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			rendererMaterial = tempMaterial;
			ShaderChange=0;
		}
		
		#if UNITY_EDITOR
		if (rendererMaterial.shader.name == "Sprites/Default")
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			rendererMaterial = ForceMaterial;
			__MainTex2 = Resources.Load ("_2dxFX_WaterTXT") as Texture2D;
			rendererMaterial.SetTexture ("_MainTex2", __MainTex2);
		}
		#endif
		if (ActiveChange)
		{
			rendererMaterial.SetFloat("_Alpha", 1-_Alpha);
			rendererMaterial.SetFloat("_Distortion", Heat);
			rendererMaterial.SetFloat("_Speed", Speed);
			rendererMaterial.SetFloat("EValue",EValue);
			rendererMaterial.SetFloat("Light",Light);
			rendererMaterial.SetFloat("TurnToLiquid",TurnToLiquid);

		}
		
	}
	
	void OnDestroy()
	{
		if ((Application.isPlaying == false) && (Application.isEditor == true)) {
			
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			
			if (gameObject.activeSelf && defaultMaterial!=null) {
				rendererMaterial = defaultMaterial;
				rendererMaterial.hideFlags = HideFlags.None;
			}
		}
	}
	void OnDisable()
	{ 
		if (gameObject.activeSelf && defaultMaterial!=null) {
			rendererMaterial = defaultMaterial;
			rendererMaterial.hideFlags = HideFlags.None;
		}		
	}
	
	void OnEnable()
	{
		
		if (defaultMaterial == null) {
			defaultMaterial = new Material(Shader.Find("Sprites/Default"));
			
			
		}
		if (ForceMaterial==null)
		{
			ActiveChange=true;
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			rendererMaterial = tempMaterial;
			__MainTex2 = Resources.Load ("_2dxFX_WaterTXT") as Texture2D;
		}
		else
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			rendererMaterial = ForceMaterial;
			__MainTex2 = Resources.Load ("_2dxFX_WaterTXT") as Texture2D;
		}
		
		if (__MainTex2)	
		{
			__MainTex2.wrapMode= TextureWrapMode.Repeat;
			rendererMaterial.SetTexture ("_MainTex2", __MainTex2);
		}
	}
	
}



#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_Blood)),CanEditMultipleObjects]
public class _2dxFX_Blood_Editor : Editor
{
	private SerializedObject m_object;
	
	public void OnEnable()
	{
		m_object = new SerializedObject(targets);
	}
	
	public override void OnInspectorGUI()
	{
		m_object.Update();
		DrawDefaultInspector();
		
		_2dxFX_Blood _2dxScript = (_2dxFX_Blood)target;
	
		Texture2D icon = Resources.Load ("2dxfxinspector-anim") as Texture2D;
		if (icon)
		{
			Rect r;
			float ih=icon.height;
			float iw=icon.width;
			float result=ih/iw;
			float w=Screen.width;
			result=result*w;
			r = GUILayoutUtility.GetRect(ih, result);
			EditorGUI.DrawTextureTransparent(r,icon);
		}

		EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));
		
		if (_2dxScript.ForceMaterial == null)
		{
			_2dxScript.ActiveChange = true;
		}
		else
		{
			if(GUILayout.Button("Remove Shared Material"))
			{
				_2dxScript.ForceMaterial= null;
				_2dxScript.ShaderChange = 1;
				_2dxScript.ActiveChange = true;
				_2dxScript.CallUpdate();
			}
		
			EditorGUILayout.PropertyField (m_object.FindProperty ("ActiveChange"), new GUIContent ("Change Material Property", "Change The Material Property"));
		}

		if (_2dxScript.ActiveChange)
		{

			EditorGUILayout.BeginVertical("Box");

			Texture2D icone = Resources.Load ("2dxfx-icon-distortion") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("TurnToLiquid"), new GUIContent("Turn To Blood", icone, "Turn to Blood = 0 = normal sprite : 1 = Blood Liquified"));
			icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Heat"), new GUIContent("Blood Distortion", icone, "Change the distortion of the blood"));

			EditorGUILayout.BeginVertical("Box");

			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
	

		}
		
		m_object.ApplyModifiedProperties();
		
	}
}
#endif