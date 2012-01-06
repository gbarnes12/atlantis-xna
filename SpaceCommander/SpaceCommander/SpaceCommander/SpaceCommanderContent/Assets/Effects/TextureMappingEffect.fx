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
float  DiffuseIntensity = 0.6;

float4 SpecularColor = {1, 1, 1, 1};
bool   SpecularColorActive = true;

float3 LightDirection = {1, 1, 1};
float3 CameraPosition;

// TEXTURES
Texture2D DiffuseTexture;
Texture2D NormalMapTexture;
Texture2D GlowTexture;

//=============================================================================
//	[STRUCTS]
//=============================================================================

struct VS_INPUT
{
    float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
	float3 Normal   : NORMAL;
	float3 Tangent  : TANGENT;
};

struct VS_OUTPUT 
{
	float4 Position : POSITION0;
	float2 UV       : TEXCOORD0;
	float3 Normal   : TEXCOORD1;
	float3 View     : TEXCOORD2;
	float3 Light    : TEXCOORD3;
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

sampler NormalMapTextureSampler = sampler_state {
	texture = <NormalMapTexture>;
	MinFilter = Anisotropic; // Minification Filter
	MagFilter = Anisotropic; // Magnification Filter
	MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

//-----------------------------------------------------------------------------
// Textured Vertex Shader
//-----------------------------------------------------------------------------

VS_OUTPUT TexturedVertexShader(VS_INPUT input)
{
    VS_OUTPUT output;

    float4 worldPosition =     mul(input.Position, World);
    float4 viewPosition  =     mul(worldPosition, View);

	float3x3 worldToTangentSpace;
    worldToTangentSpace[0] = mul(input.Tangent,	World);
    worldToTangentSpace[1] = mul(cross(input.Tangent,input.Normal), World);
    worldToTangentSpace[2] = mul(input.Normal,	World);

    output.Position      =     mul(viewPosition, Projection);
	output.UV            =     input.UV;
	output.Normal        =     normalize(mul(input.Normal, World));
	output.View			 =     normalize(float4(CameraPosition,1.0) - worldPosition);
	output.Light		 =	   mul(worldToTangentSpace, LightDirection);

    return output;
}

//-----------------------------------------------------------------------------
// Textured Pixel Shader
//-----------------------------------------------------------------------------

float4 TexturedPixelShader(VS_OUTPUT input) : COLOR0
{
	float4 diffuseMap = tex2D(DiffuseTextureSampler, input.UV);
	float3 normalMap = (2 * (tex2D(NormalMapTextureSampler, input.UV))) - 1.0;

	float3 LightDir = normalize(input.Light);
	float4 diffuse = saturate(dot(-LightDir,normalMap));
	float4 reflect = normalize(2*diffuse*float4(input.Normal, 1.0)-float4(LightDir,1.0));
	float4 specular = pow(saturate(dot(reflect,input.View)),15);

	if(SpecularColorActive)
		return diffuseMap + AmbientColor * AmbientIntensity + DiffuseIntensity*DiffuseColor*diffuse + SpecularColor * specular;

    return diffuseMap + AmbientColor * AmbientIntensity + DiffuseIntensity*DiffuseColor*diffuse;
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
