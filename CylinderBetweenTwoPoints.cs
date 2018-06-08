/************************************************************
description
	2点指定でcylinderを作成する.
	
調査したきっかけ
	Golem app for BackStageにて、AIの作成するJoiint間のBone(長さが変わる可能性あり)を動的に生成したい

limitation
	textureは、伸び縮みしてしまう.
	今回は、身長によって決定されるjoint間距離が、動作中一定 = 動作中は伸び縮みしない
	ので、これで良しとした。
	
参考URL
	https://cubic9.com/Devel/Unity/2%C5%C0%BB%D8%C4%EA%A4%C7Cylinder%A4%F2%C0%B8%C0%AE%A4%B9%A4%EB/
	
cylinder prefabについて
	LookAtは、objectのz軸を指定方向に向けるmethod.
	ところが、cylinderは、上下方向がy軸となっている。
	
	3ds maxのように、基点を変更すれば良いのだが、Unityでは直接的にこれを変更する手段がない。
	そこで、空のObjectを作成し、このObjectをx軸周りに-90deg回転させる。
	Cylinderを、この空のObjectの子にする。
	-> この親子をPrefab化
	でOK.
	
	本scriptのcylinderPrefabには、親をset.
	
		参考URL : http://portaltan.hatenablog.com/entry/2016/04/15/134129
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderBetweenTwoPoints : MonoBehaviour {
    [SerializeField] // 設計的にはprivateにしたいが、inspectorに表示させたい.
    private Transform cylinderPrefab;

    private GameObject leftSphere;
    private GameObject rightSphere;
    private GameObject cylinder;

    private void Start () {
        leftSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftSphere.transform.position = new Vector3(-2, 0, 0);
        rightSphere.transform.position = new Vector3(2, 0, 0);

        InstantiateCylinder(cylinderPrefab, leftSphere.transform.position, rightSphere.transform.position);
    }

    private void Update () {
        leftSphere.transform.position = new Vector3(-2, -2f * Mathf.Sin(Time.time), 0);
        rightSphere.transform.position = new Vector3(2, 2f * Mathf.Sin(Time.time), 0);

        UpdateCylinderPosition(cylinder, leftSphere.transform.position, rightSphere.transform.position);
    }

    private void InstantiateCylinder(Transform cylinderPrefab, Vector3 beginPoint, Vector3 endPoint)
    {
        cylinder = (GameObject)Instantiate(cylinderPrefab.gameObject, Vector3.zero, Quaternion.identity);
        UpdateCylinderPosition(cylinder, beginPoint, endPoint);
    }

    private void UpdateCylinderPosition(GameObject cylinder, Vector3 beginPoint, Vector3 endPoint)
    {
		Vector3 offset = endPoint - beginPoint;
        Vector3 position = beginPoint + (offset / 2.0f);

        cylinder.transform.position = position;
        cylinder.transform.LookAt(beginPoint);
		
        Vector3 localScale = cylinder.transform.localScale;
		/********************
		[cylinderの長手方向について]
		scale = 1の時、プラス・マイナス方向にそれぞれ1[m].
		つまり、高さは2[m]である。
		高さを1[m]にするには、scale = 0.5である点に注意.
		********************/
        localScale.z = (endPoint - beginPoint).magnitude / 2;
        cylinder.transform.localScale = localScale;
    }
}

