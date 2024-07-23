Shader "FD/MouthShapeMask"
{
	Properties
	{
		[IntRange] _StencilID ("sencil ID", Range(0,255))=0
	}

	SubShader 
	{
		Tags {"RenderType"="Opaque" "Queue"="Geometry-1" "RenderPipeline"="UniversalPipeline"}

		Pass
		{
			Blend Zero One
			ZWrite Off

			stencil
			{
				Ref [_StencilID]
				Comp Always
				Pass Replace
				Fail Keep
			}
		}
	}
}