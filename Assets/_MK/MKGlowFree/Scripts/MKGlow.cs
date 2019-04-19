//////////////////////////////////////////////////////
// MK Glow 	    	    	                        //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2019 All rights reserved.            //
//////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MK.Glow.Legacy
{
	#if UNITY_2018_3_OR_NEWER
        [ExecuteAlways]
    #else
        [ExecuteInEditMode]
    #endif
    [DisallowMultipleComponent]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
	public class MKGlow : MonoBehaviour
	{
        #if UNITY_EDITOR
        public bool showEditorMainBehavior = true;
		public bool showEditorBloomBehavior;
		public bool showEditorLensSurfaceBehavior;
		public bool showEditorLensFlareBehavior;
		public bool showEditorGlareBehavior;
        #endif

        //Main
        public DebugView debugView = MK.Glow.DebugView.None;
        public Workflow workflow = MK.Glow.Workflow.Luminance;
        public LayerMask selectiveRenderLayerMask = -1;

        //Bloom
        [MK.Glow.MinMaxRange(0, 10)]
        public MinMaxRange bloomThreshold = new MinMaxRange(1.0f, 10f);
        [Range(1f, 10f)]
		public float bloomScattering = 7f;
		public float bloomIntensity = 1f;

        private Effect _effect;

        private RenderTexture _tmpTexture;
        private RenderContext _tmpContext;
        private RenderDimension _tmpDimension = new RenderDimension();

		private Camera renderingCamera
		{
			get { return GetComponent<Camera>(); }
		}

		public void OnEnable()
		{
            _effect = new Effect();
            _tmpContext = new RenderContext();
			_effect.Enable();
		}

		public void OnDisable()
		{
			_effect.Disable();
		}

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(workflow == Workflow.Selective && PipelineProperties.xrEnabled)
            {
                Graphics.Blit(source, destination);
                return;
            }
            
            _tmpDimension.width = renderingCamera.pixelWidth;
            _tmpDimension.height = renderingCamera.pixelHeight;

            _tmpContext.UpdateRenderContext(renderingCamera, _effect.renderTextureFormat, source.depth, _tmpDimension);
            _tmpTexture = PipelineExtensions.GetTemporary(_tmpContext, _effect.renderTextureFormat);

			_effect.Build(source, _tmpTexture, this, renderingCamera);

            Graphics.Blit(_tmpTexture, destination);
            RenderTexture.ReleaseTemporary(_tmpTexture);
        }
	}
}