﻿//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2015 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("2DxFX/Standard/SkyCloud")]
[System.Serializable]
public class _2dxFX_SkyCloud : _moiling_2dxFX_BaseClass
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/SkyCloud";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

	[HideInInspector] public Texture2D __MainTex2; 
	[HideInInspector] public float _OffsetX;
	[HideInInspector] public float _OffsetY;
	[HideInInspector] [Range(0.1f, 2f)] public float _Zoom=0.2f;
	[HideInInspector] [Range(-1, 1)] public float _Intensity=0.3f;

	[HideInInspector] public bool _AutoScrollX;
	[HideInInspector] [Range(-2,2)]public float _AutoScrollSpeedX=0.08f;
	[HideInInspector] public bool _AutoScrollY;
	[HideInInspector] [Range(-2,2)] public float _AutoScrollSpeedY=0.02f;
	[HideInInspector] private float _AutoScrollCountX;
	[HideInInspector] private float _AutoScrollCountY;

	[HideInInspector] public int ShaderChange=0;
	Material tempMaterial;

		Material defaultMaterial;

	void Start ()
	{ 
		__MainTex2 = Resources.Load ("_2dxFX_ShadowTXT") as Texture2D;
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
			__MainTex2 = Resources.Load ("_2dxFX_ShadowTXT") as Texture2D;
			rendererMaterial.SetTexture ("_MainTex2", __MainTex2);
	
		}
		#endif
		if (ActiveChange)
		{
			rendererMaterial.SetFloat("_Alpha", 1-_Alpha);
			rendererMaterial.SetFloat("_Zoom",_Zoom);
			rendererMaterial.SetFloat("_Intensity",_Intensity);

			if ((_AutoScrollX == false) && (_AutoScrollY == false))
			{
				rendererMaterial.SetFloat("_OffsetX",_OffsetX);
				rendererMaterial.SetFloat("_OffsetY",_OffsetY);
			}
			
			if ((_AutoScrollX == true) && (_AutoScrollY == false))
			{
				_AutoScrollCountX+=_AutoScrollSpeedX*Time.deltaTime;
				rendererMaterial.SetFloat("_OffsetX",_AutoScrollCountX);
				rendererMaterial.SetFloat("_OffsetY",_OffsetY);
			}
			if ((_AutoScrollX == false) && (_AutoScrollY == true))
			{
				_AutoScrollCountY+=_AutoScrollSpeedY*Time.deltaTime;
				rendererMaterial.SetFloat("_OffsetX",_OffsetX);
				rendererMaterial.SetFloat("_OffsetY",_AutoScrollCountY);
			}
			if ((_AutoScrollX == true) && (_AutoScrollY == true))
			{
				_AutoScrollCountX+=_AutoScrollSpeedX*Time.deltaTime;
				rendererMaterial.SetFloat("_OffsetX",_AutoScrollCountX);
				_AutoScrollCountY+=_AutoScrollSpeedY*Time.deltaTime;
				rendererMaterial.SetFloat("_OffsetY",_AutoScrollCountY);
			}
			if (_AutoScrollCountX > 1) _AutoScrollCountX = 0;
			if (_AutoScrollCountX < -1) _AutoScrollCountX = 0;
			if (_AutoScrollCountY > 1) _AutoScrollCountY = 0;
			if (_AutoScrollCountY < -1) _AutoScrollCountY = 0;
		}

		
	}
	
	void OnDestroy()
	{
		if ((Application.isPlaying == false) && (Application.isEditor == true)) {

			if (ForceMaterial != null && tempMaterial!=null)
			{
				DestroyImmediate(tempMaterial);
			}

			if (gameObject.activeSelf) {
				rendererMaterial = defaultMaterial;
				rendererMaterial.hideFlags = HideFlags.None;
			}
		}
	}
	void OnDisable()
	{ 
		if (ForceMaterial!=null && tempMaterial!=null)
		{ 
			DestroyImmediate(tempMaterial);
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
			__MainTex2 = Resources.Load ("_2dxFX_ShadowTXT") as Texture2D;
		}
		else
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			rendererMaterial = ForceMaterial;
			__MainTex2 = Resources.Load ("_2dxFX_ShadowTXT") as Texture2D;
		}
		
		if (__MainTex2)	
		{
			__MainTex2.wrapMode= TextureWrapMode.Repeat;
			rendererMaterial.SetTexture ("_MainTex2", __MainTex2);
		}
	}
		
	
	
}




#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_SkyCloud)),CanEditMultipleObjects]
public class _2dxFX_SkyCloud_Editor : Editor
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
		
		_2dxFX_SkyCloud _2dxScript = (_2dxFX_SkyCloud)target;
	
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

			Texture2D icone = Resources.Load ("2dxfx-icon-size_x") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_OffsetX"), new GUIContent("Offset X", icone, "Change the start offset of the shadow effect"));

			icone = Resources.Load ("2dxfx-icon-size_y") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_OffsetY"), new GUIContent("Offset Y", icone, "Change the start offset of the shadow effect"));

			icone = Resources.Load ("2dxfx-icon-size") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Zoom"), new GUIContent("Zoom", icone, "Zoom the shadow effect"));
			
			icone = Resources.Load ("2dxfx-icon-value") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Intensity"), new GUIContent("Intensity", icone, "Active the X mouvement of the cloud"));

			icone = Resources.Load ("2dxfx-icon-value") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollX"), new GUIContent("Auto Scroll X", icone, "Change the value of the posterize effect"));

			if (_2dxScript._AutoScrollX) 
			{
				icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
				EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollSpeedX"), new GUIContent("Auto Scroll Speed X", icone, "Change the speed of the shadow"));
			}

			icone = Resources.Load ("2dxfx-icon-value") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollY"), new GUIContent("Auto Scroll Y", icone, "Active the Y mouvement of the cloud"));

			if (_2dxScript._AutoScrollY)
			{
				icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
				EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollSpeedY"), new GUIContent("Auto Scroll Speed Y", icone, "Change the speed of the shadow"));
			}

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