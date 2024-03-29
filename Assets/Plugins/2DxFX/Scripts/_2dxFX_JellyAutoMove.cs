﻿//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2015 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("2DxFX/Standard/JellyAutoMove")]
[System.Serializable]
public class _2dxFX_JellyAutoMove : _moiling_2dxFX_BaseClass
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/Jelly";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

	[HideInInspector] [Range(0.0f, 4f)] public float Heat = 1.0f;
	[HideInInspector] [Range(0.0f, 4f)] public float RandomPos = 1.0f;
	[HideInInspector] [Range(  1f, 2f)] public float Inside = 1.0f;
	[HideInInspector] [Range(  1f, 8f)] public float Stabilisation = 4.0f;
	[HideInInspector] [Range(0.0f, 4f)] public float Speed = 1.0f;

	[HideInInspector] public int ShaderChange=0;
	Material tempMaterial;
	Material defaultMaterial;

	Vector3 SaveMove1;
	Vector3 SaveMove2;
	
	void Start ()
	{ 
		ShaderChange = 0;
		Heat = 0;
	}

 	public void CallUpdate()
	{
		Update ();
	}

	void Update()
	{	
		SaveMove1 = transform.position;
		if (SaveMove1.x != SaveMove2.x) Heat += 0.5f;
		if (SaveMove1.y != SaveMove2.y) Heat += 0.5f;
		if (SaveMove1.z != SaveMove2.z) Heat += 0.5f;

		Heat -= Time.deltaTime*Stabilisation;
		if (Heat > 4) Heat = 4;
		if (Heat < 0) {	RandomPos = Random.Range (0,256); Heat = 0; }
		SaveMove2 = SaveMove1;
	
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
		}
		#endif
		if (ActiveChange)
		{
			rendererMaterial.SetFloat("_Alpha", 1-_Alpha);
			rendererMaterial.SetFloat("_Distortion", Heat);
			rendererMaterial.SetFloat("_RandomPos", RandomPos);
			rendererMaterial.SetFloat("_Inside", Inside);
			rendererMaterial.SetFloat("_Speed", Speed);

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
		}
		else
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			rendererMaterial = ForceMaterial;
		}
		
	}
}




#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_JellyAutoMove)),CanEditMultipleObjects]
public class _2dxFX_JellyAutoMove_Editor : Editor
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
		
		_2dxFX_JellyAutoMove _2dxScript = (_2dxFX_JellyAutoMove)target;
	
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
		//	EditorGUILayout.PropertyField(m_object.FindProperty("Heat"), new GUIContent("Jelly Distortion", icone, "Change the distortion of the Jelly FX"));
		//	icone = Resources.Load ("2dxfx-icon-distortion") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Inside"), new GUIContent("Jelly Inside", icone, "Change the inside distortion of the Jelly FX"));
			icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Speed"), new GUIContent("Time Speed", icone, "Change the time speed"));
			icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Stabilisation"), new GUIContent("Stabilisation Speed", icone, "Change the Stabilisation speed"));

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