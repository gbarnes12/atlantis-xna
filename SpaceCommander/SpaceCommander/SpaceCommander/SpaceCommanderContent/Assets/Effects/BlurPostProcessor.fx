sampler firstSampler;
float2 targetSize;

float4 PS_BLUR(float2 texCoord: TEXCOORD0) : COLOR
{
   float2 invSize = (1.0f / (targetSize));

   float4 color = 0;
   color += tex2D(firstSampler, texCoord + float2(invSize.x, 0));
   color += tex2D(firstSampler, texCoord + float2(invSize.x * 3.0f, 0));
   color += tex2D(firstSampler, texCoord + float2(-invSize.x, 0));
   color += tex2D(firstSampler, texCoord + float2(-invSize.x * 3.0f, 0));

   color /= 4.0f;
   
   return color;
}

technique BlurShader
{
   pass pass0
   {
      PixelShader = compile ps_2_0 PS_BLUR();
   }
} 