float4x4 World;
float4x4 View;
float4x4 Projection;

texture DiffuseTexture;

struct VS_INPUT
{
	float4 Position : POSITION0;
};

struct VS_OUTPUT
{
	float4 Position : POSITION0;
};


VS_OUTPUT VertexShaderFunction(VS_INPUT input)
{
	VS_OUTPUT output;

	float4 worldPosition = mul(input.Position, World);
	float4x4 viewProjection = mul(View, Projection);

	output.Position = mul(worldPosition, viewProjection);

	return output;
};

float4 PixelShaderFunction(VS_OUTPUT input) : COLOR0
{
	return float4(.5, .5, .5, 1);
};


technique TextureMappingEffect
{
	pass Pass1
	{
		VertexShader = compile vs_1_1 VertexShaderFunction();
		PixelShader  = compile ps_2_0 PixelShaderFunction();
	}
}