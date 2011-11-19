//=============================================================================
// 	[GLOBALS]
//=============================================================================

float4x4 World;
float4x4 Projection;
float4x4 View;
Texture2D DiffuseTexture;

sampler2D DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
};

//=============================================================================
//	[STRUCTS]
//=============================================================================

struct VertexPositionTexture
{
    float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
};

//=============================================================================
// 	[FUNCTIONS]
//=============================================================================

//-----------------------------------------------------------------------------
// Textured Vertex Shader
//-----------------------------------------------------------------------------

VertexPositionTexture TexturedVertexShader(VertexPositionTexture input)
{
    VertexPositionTexture output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.UV       = input.UV;

    return output;
}

float4 TexturedPixelShader(VertexPositionTexture input) : COLOR0
{
    return tex2D(DiffuseSampler, input.UV);
}

//=============================================================================
//	[TECHNIQUES]
//=============================================================================

technique DefaultEffect
{
    Pass
    {
        VertexShader = compile vs_2_0 TexturedVertexShader();
        PixelShader  = compile ps_2_0 TexturedPixelShader();
    }
}
