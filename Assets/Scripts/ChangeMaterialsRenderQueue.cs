using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ChangeMaterialsRenderQueue : MonoBehaviour
{
	[SerializeField] private int renderQueue = 1998;
	private MeshRenderer _renderer;

	private void Awake()
	{
		_renderer = GetComponent<MeshRenderer>();
	}

	void Start()
	{
		Fix();
	}

	public void Fix()
	{
		Material[] materials = new Material[_renderer.materials.Length];
		for (var i = 0; i < _renderer.materials.Length; i++)
		{
			var material = _renderer.materials[i];
			Material mat = new Material(material);
			mat.renderQueue = renderQueue;
			materials[i] = mat;
		}

		_renderer.materials = materials;
	}
}