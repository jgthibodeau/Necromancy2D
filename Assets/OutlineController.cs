using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public enum HIGHLIGHT_TYPE { RESSURECTED, RESURRECTABLE, EXPLODABLE }

    public Color ressurectedColor, resurrectableColor, explodableColor;
    public List<GameObject> resurrectedObjects, resurrectableObjects, explodableObjects;

    public HighlightsFX.SortingType depthType;

    //    [System.Serializable]
    //    public class OutlineData
    //    {
    //        public Color color = Color.white;
    //        public HighlightsFX.SortingType depthType;
    //        public Renderer renderer;
    //    }

    public HighlightsFX outlinePostEffect;
//    public OutlineData[] outliners;

//    private void Start()
//    {
//        foreach (var obj in outliners)
//        {
//            outlinePostEffect.AddRenderers(
//                new List<Renderer>() { obj.renderer }, 
//                obj.color, 
//                obj.depthType);
//        }
//    }

    void Update()
    {
        if (CleanList(resurrectedObjects) || CleanList(resurrectableObjects) || CleanList(explodableObjects))
        {
            UpdateHighlight();
        }
    }

    bool CleanList(List<GameObject> list)
    {
        int count = list.Count;
        list.RemoveAll(item => item == null);

        return count != list.Count;
    }

    //	public void SetRenderers(GameObject go) {
    //		if (go == null) {
    //			Debug.Log ("removing highlight");
    //			outlinePostEffect.ClearOutlineData ();
    //		}
    //		else/* if (highlightedObject != go)*/ {
    //			Debug.Log ("highlighting " + go);
    //			outlinePostEffect.ClearOutlineData ();
    //			if (go != null) {
    //				List<Renderer> renderers = new List<Renderer> (go.GetComponentsInChildren<Renderer> ());
    //				for (int i = renderers.Count - 1; i >= 0; i--) {
    //					if (renderers [i] is ParticleSystemRenderer) {
    //						renderers.RemoveAt (i);
    //					}
    ////					Debug.Log(renderers[i]);
    //				}
    //				outlinePostEffect.AddRenderers (renderers, color, depthType);
    //			}
    //		}
    //	}

    public void SetObjects(HIGHLIGHT_TYPE type, List<GameObject> list)
    {
        switch (type)
        {
            case HIGHLIGHT_TYPE.RESSURECTED:
                resurrectedObjects = list;
                break;
            case HIGHLIGHT_TYPE.RESURRECTABLE:
                resurrectableObjects = list;
                break;
            case HIGHLIGHT_TYPE.EXPLODABLE:
                explodableObjects = list;
                break;
        }
        UpdateHighlight();
    }

    public void AddObject(HIGHLIGHT_TYPE type, GameObject go)
    {
        switch (type)
        {
            case HIGHLIGHT_TYPE.RESSURECTED:
                AddObject(resurrectedObjects, go);
                break;
            case HIGHLIGHT_TYPE.RESURRECTABLE:
                AddObject(resurrectableObjects, go);
                break;
            case HIGHLIGHT_TYPE.EXPLODABLE:
                AddObject(explodableObjects, go);
                break;
        }
    }

    public void RemoveObject(HIGHLIGHT_TYPE type, GameObject go)
    {
        switch (type)
        {
            case HIGHLIGHT_TYPE.RESSURECTED:
                RemoveObject(resurrectedObjects, go);
                break;
            case HIGHLIGHT_TYPE.RESURRECTABLE:
                RemoveObject(resurrectableObjects, go);
                break;
            case HIGHLIGHT_TYPE.EXPLODABLE:
                RemoveObject(explodableObjects, go);
                break;
        }
    }

    public void AddObject(List<GameObject> objectList, GameObject go)
    {
        if (!objectList.Contains(go))
        {
            objectList.Add(go);
            UpdateHighlight();
        }
    }

    public void RemoveObject(List<GameObject> objectList, GameObject go)
    {
        if (objectList.Contains(go))
        {
            objectList.Remove(go);
            UpdateHighlight();
        }
    }

    public void UpdateHighlight() {
		outlinePostEffect.ClearOutlineData ();
        UpdateHighlight(resurrectedObjects, ressurectedColor);
        UpdateHighlight(resurrectableObjects, resurrectableColor);
        UpdateHighlight(explodableObjects, explodableColor);
    }

    public void UpdateHighlight(List<GameObject> list, Color color)
    {
        foreach (GameObject go in list)
        {
            List<Renderer> renderers = new List<Renderer>(go.GetComponentsInChildren<Renderer>());
            renderers.RemoveAll(renderer => renderer.tag != "Highlightable");
            HighlightRenderers(renderers, color);
        }
    }

    public void HighlightRenderers(List<Renderer> renderers, Color color){
		for (int i = renderers.Count - 1; i >= 0; i--) {
			if (renderers [i] is ParticleSystemRenderer/* || !renderers[i].enabled*/) {
				renderers.RemoveAt (i);
			}
		}
		outlinePostEffect.AddRenderers (renderers, color, depthType);
	}
}
