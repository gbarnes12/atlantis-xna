//=============================================================================
// 	[GLOBALS]
//=============================================================================
float4x4 World;
float4x4 View;
float4x4 Projection;

// Textues
texture DiffuseTexture;
bool    TextureEnabled = false;

// Lighting
float3 DiffuseColor = float3(1, 1, 1);
float3 AmbientColor = float3(.1, .1, .1);
float3 LightColor   = float3(0.9, 0.9, 0.9);
float3 LightDirection = float3(1, 1, 1);

float SpecularPower = 32;
float3 SpecularColor = float3(1, 1, 1);

float3 CameraPosition;

//=============================================================================
//	[STRUCTS]
//=============================================================================

struct VS_INPUT
{
	float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
	float3 Normal   : NORMAL0;
};

struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
	float3 Normal   : TEXCOORD1;
	float3 ViewDirection : TEXCOORD2;
};

//=============================================================================
// 	[FUNCTIONS]
//=============================================================================
sampler DiffuseTextureSampler = sampler_state {
	texture = <DiffuseTexture>;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

//-----------------------------------------------------------------------------
// Textured Vertex Shader
//-----------------------------------------------------------------------------

VS_OUTPUT VertexShaderFunction(VS_INPUT input)
{
	VS_OUTPUT output;

	float4 worldPosition = mul(input.Position, World);
	float4x4 viewProjection = mul(View, Projection);

	output.Position = mul(worldPosition, viewProjection);
	output.UV = input.UV;
	output.Normal = mul(input.Normal, World);
	output.ViewDirection = worldPosition - CameraPosition;

	return output;
};

//-----------------------------------------------------------------------------
// Textured Pixel Shader
//-----------------------------------------------------------------------------

float4 PixelShaderFunction(VS_OUTPUT input) : COLOR0
{
	float3 color = DiffuseColor;

	if(TextureEnabled)
		color *= tex2D(DiffuseTextureSampler, input.UV);

	float3 lighting = AmbientColor;

	float3 lightDir = normalize(LightDirection);
	float3 normal   = normalize(input.Normal);

	// Add the lambertian light
	lighting += saturate(dot(lightDir, normal)) * LightColor;

	float3 refl = reflect(lightDir, normal);
	float3 view = normalize(input.ViewDirection);

	// Add specular highlights
	lighting += pow(saturate(dot(refl, view)), SpecularPower) * SpecularColor;

	// Calculate final color
	float3 output = saturate(lighting) * color;

	return float4(output, 1);
};

//=============================================================================
//	[TECHNIQUES]
//=============================================================================

technique TextureMappingEffect
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader  = compile ps_2_0 PixelShaderFunction();
	}
}