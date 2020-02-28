using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoFitToCanvas : MonoBehaviour {

	public RectTransform target, father, kanvas;
	public bool isWidthRelative, isHeightRelative;
	//the size of canvas during development
	private Vector2 initKanvasSize = new Vector2(1280f, 800f);
	bool hasLayoutElement;
	LayoutElement layOutElem;

	void OnEnable () {
		layOutElem = target.gameObject.GetComponent<LayoutElement>();
		if(layOutElem != null && !hasLayoutElement){
			hasLayoutElement=true;
		}

		Vector2 size = target.sizeDelta;
		
		if(isWidthRelative){

			float val = (size.x / initKanvasSize.x) * target.sizeDelta.x;

			if(hasLayoutElement){
				layOutElem.minWidth = val;
				layOutElem.preferredWidth = val;
			}else{
				size.x = val ;
			}
		}

		if(isHeightRelative){
			
			float val = (size.y / initKanvasSize.y) * target.sizeDelta.y;
			
			if(hasLayoutElement){
				layOutElem.minHeight = val;
				layOutElem.preferredHeight = val;
			}else{
				size.y = val ;
			}
		}

		target.sizeDelta = size;
	}
	

}
