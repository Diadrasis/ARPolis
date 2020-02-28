using UnityEngine;
using System.Collections;

public class AutoFade : MonoBehaviour {

	public CanvasGroup[] canvgroups;

	public float speedFade = 8f;

	void OnEnable () {
		StopAllCoroutines ();
		Init ();
		FadeIn ();
	}

	void FadeIn () {
		StartCoroutine (fIn ());
	}

	IEnumerator fIn(){
		yield return new WaitForSeconds (0.25f);

		if (canvgroups.Length > 0) {
			for (int i = 0; i < canvgroups.Length; i++) {
				//fade in
				while (canvgroups [i].alpha < 0.99f) {
					canvgroups [i].alpha = Mathf.Lerp (canvgroups [0].alpha, 1f, Time.deltaTime * speedFade);	
					yield return null;
				}

				//fade in full
				canvgroups [i].alpha = 1f;

				yield return null;
			}
		}


		yield break;
	}
	
	public void FadeOut () {
		StopAllCoroutines ();
		StartCoroutine (fOut ());
	}

	IEnumerator fOut(){
		if (canvgroups.Length > 0) {
			for (int i = 0; i < canvgroups.Length; i++) {
				//fade in
				while (canvgroups [i].alpha > 0.01f) {
					canvgroups [i].alpha = Mathf.Lerp (canvgroups [0].alpha, 0f, Time.deltaTime * speedFade);	
					yield return null;
				}

				//fade in full
				canvgroups [i].alpha = 0f;

				yield return null;
			}
		}

		Init ();

		gameObject.SetActive (false);

		yield break;
	}

	void Init(){
		if (canvgroups.Length > 0) {
			foreach (CanvasGroup cg in canvgroups) {
				cg.alpha = 0f;
			}
		}
	}

}
