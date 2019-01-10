using XLua;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(Decal))]
[Hotfix]
public class DecalEditor : Editor {	

	private Matrix4x4 oldMatrix;
	//private Vector3 oldScale;
	private GameObject[] affectedObjects;

	public override void OnInspectorGUI() {
		Decal decal = (Decal)target;

		base.OnInspectorGUI ();

		if(GUI.changed) {
			BuildDecal( decal );
		}
	}

	private static Material DrawMaterialList(Material material, List<Material> list) {
		string[] names = new string[list.Count];
		for(int i=0; i<list.Count; i++) {
			names[i] = list[i].name;
		}

		int selected = list.IndexOf( material );
		selected = EditorGUILayout.Popup("Material", selected, names);
		if(selected != -1) return list[selected];
		return null;
	}

	private static Sprite DrawSpriteList(Sprite sprite, Texture texture) {
		string path = AssetDatabase.GetAssetPath(texture);
		Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
		List<Sprite> sprites = new List<Sprite>();
		foreach( Object o in objs ) {
			if(o is Sprite) sprites.Add( (Sprite)o );
		}

		return DrawSpriteList( sprite, sprites.ToArray(), texture );
	}

	private static Sprite DrawSpriteList(Sprite sprite, Sprite[] list, Texture texture) {
		GUILayout.BeginVertical(GUI.skin.box, GUILayout.MinHeight(50));
		for(int i=0, y=0; i<list.Length; y++) {
			GUILayout.BeginHorizontal();
			for(int x=0; x<5; x++, i++) {
				Rect rect = GUILayoutUtility.GetAspectRect(1);
				if(i < list.Length) {
					Sprite spr = list[i];
					bool selected = DrawItem(rect, spr, sprite == spr);
					if(selected) sprite = spr;
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		return sprite;
	}

	private static bool DrawItem(Rect rect, Sprite sprite, bool selected) {
		if(selected) {
			GUI.color = Color.blue;
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
		}

		Texture texture = sprite.texture;
		Rect texRect = sprite.rect;
		texRect.x /= texture.width;
		texRect.y /= texture.height;
		texRect.width /= texture.width;
		texRect.height /= texture.height;
		GUI.DrawTextureWithTexCoords(rect, texture, texRect);

		selected = Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition );
		if(selected) {
			GUI.changed = true;
			Event.current.Use();
			return true;
		}
		return false;
	}


	void OnSceneGUI() {
		Decal decal = (Decal)target;

		bool hasChanged = oldMatrix != decal.transform.localToWorldMatrix;
		oldMatrix = decal.transform.localToWorldMatrix;
		//oldScale = decal.transform.localScale;
		
		if(hasChanged) {
			BuildDecal( decal );
		}
	}

	private static LayerMask LayerMaskField(string label, LayerMask mask) {
		List<string> layers = new List<string>();
		for(int i=0; i<32; i++) {
			string name = LayerMask.LayerToName(i);
            //LogMgr.UnityLog("Layer name is " + name);
			if(name != "") layers.Add( name );
		}

        //for(int i=0; i<32; i++) {
        //LogMgr.UnityLog("Selected masks are " + LayerMask.LayerToName(mask.value));
        //}

		return EditorGUILayout.MaskField( label, mask, layers.ToArray() );
	}

	private static bool IsLayerContains(LayerMask mask, int layer) {
        //LogMgr.UnityLog("Mask value is " + mask.value);
        //LogMgr.UnityLog("Layer value is " + (layer >> 2));
        if (mask.value >= 0)
            return ((mask.value >> 2) & layer) != 0;
        else
            return true;
	}
	
	
	private void BuildDecal(Decal decal) {
		MeshFilter filter = decal.GetComponent<MeshFilter>();
		if(filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
		if(decal.GetComponent<Renderer>() == null) decal.gameObject.AddComponent<MeshRenderer>();
		decal.GetComponent<Renderer>().material = decal.material;

		if(decal.material == null) {
			filter.mesh = null;
			return;
		}

		affectedObjects = GetAffectedObjects(decal.GetBounds(), decal.affectedLayers);
		foreach(GameObject go in affectedObjects) {
			DecalBuilder.BuildDecalForObject( decal, decal.shapeType, decal.sectorShapeAngle, go );
		}
		DecalBuilder.Push( decal.pushDistance );

		Mesh mesh = DecalBuilder.CreateMesh();
		if(mesh != null) {
			mesh.name = "DecalMesh";
			filter.mesh = mesh;
		}
	}

	private static GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers) {
		MeshRenderer[] renderers = (MeshRenderer[]) GameObject.FindObjectsOfType<MeshRenderer>();
		List<GameObject> objects = new List<GameObject>();
		foreach(Renderer r in renderers) {
			if( !r.enabled ) continue;
            /*
            if (r.gameObject.name == "bonnet") {
                LogMgr.UnityLog("bonnet layer is " + r.gameObject.layer);
                //int test = (affectedLayers.value >> 2);
                LogMgr.UnityLog("affected layer is " + (affectedLayers.value >> 2));

                LogMgr.UnityLog("Mask test: " + (affectedLayers.value & r.gameObject.layer >> 2));
                //LogMgr.UnityLog("Mask test: " + (r.gameObject.layer & (affectedLayers.value >> 2)));
            }
            */
			if( !IsLayerContains(affectedLayers, r.gameObject.layer) ) continue;
			if( r.GetComponent<Decal>() != null ) continue;
			
			if( bounds.Intersects(r.bounds) ) {
				objects.Add(r.gameObject);
			}
		}
		return objects.ToArray();
	}


	
}

