//=============================================================================
// 	[GLOBALS]
//=============================================================================

float4x4 World;
float4x4 Projection;
float4x4 View;


// LIGHTS
float4 AmbientColor = {0, 0, 0, 1};
float  AmbientIntensity = 0;

float4 DiffuseColor = {1, 1, 1, 1};
float  DiffuseIntensity = 0.2;

float4 SpecularColor = {1, 1, 1, 0.4};
bool   SpecularColorActive = true;

float3 LightDirection = {1, 1, 1};
float3 CameraPosition;

// TEXTURES
Texture2D DiffuseTexture;
Texture2D NormalTexture;
Texture2D GlowTexture;

//=============================================================================
//	[STRUCTS]
//=============================================================================

struct VS_INPUT
{
    float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
};

struct VS_OUTPUT 
{
	float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
	float3 Normal   : TEXCOORD1;
	float3 View     : TEXCOORD2;
};

//=============================================================================
// 	[FUNCTIONS]
//=============================================================================
sampler DiffuseTextureSampler = sampler_state {
	texture = <DiffuseTexture>;
	MinFilter = Anisotropic; // Minification Filter
	MagFilter = Anisotropic; // Magnification Filter
	MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

//-----------------------------------------------------------------------------
// Textured Vertex Shader
//-----------------------------------------------------------------------------

VS_OUTPUT TexturedVertexShader(VS_INPUT input, float3 Normal : NORMAL)
{
    VS_OUTPUT output;

    float4 worldPosition =     mul(input.Position, World);
    float4 viewPosition  =     mul(worldPosition, View);
    output.Position      =     mul(viewPosition, Projection);
	output.UV            =     input.UV;
	output.Normal        =     normalize(mul(Normal, World));
	output.View			 =     normalize(float4(CameraPosition,1.0) - worldPosition);

    return output;
}

//-----------------------------------------------------------------------------
// Textured Pixel Shader
//-----------------------------------------------------------------------------

float4 TexturedPixelShader(VS_OUTPUT input) : COLOR0
{
	float4 tex = tex2D(DiffuseTextureSampler, input.UV);
	float4 norm = float4(input.Normal, 1.0);
	float4 diffuse = saturate(dot(-LightDirection,norm));
	float4 reflect = normalize(2*diffuse*norm-float4(LightDirection,1.0));
	float4 specular = pow(saturate(dot(reflect,input.View)),15);

	if(SpecularColorActive)
		return tex + AmbientColor * AmbientIntensity + DiffuseIntensity*DiffuseColor*diffuse + SpecularColor * specular;

    return tex + AmbientColor * AmbientIntensity + DiffuseIntensity*DiffuseColor*diffuse;
}

//=============================================================================
//	[TECHNIQUES]
//=============================================================================

technique TextureMappingEffect
{
    Pass
    {
        VertexShader = compile vs_2_0 TexturedVertexShader();
        PixelShader  = compile ps_2_0 TexturedPixelShader();
    }
}
