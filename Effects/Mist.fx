sampler uImage0 : register(s0);
sampler noiseTexture : register(s1);

float2 noiseResolution;
float2 noiseOffset;
float progress;
float fadeoutFactor = 10;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(uImage0, coords);
    float4 noise = tex2D(noiseTexture, (coords + noiseOffset) * noiseResolution);

    float2 centerCoords = coords * 2 - 1;
    float distFromCenter = dot(centerCoords, centerCoords);
    float opacityNoise = noise.r * pow(distFromCenter, 0.1);
    float opacity = saturate(((opacityNoise + distFromCenter) * -0.5 + 1 - progress) * fadeoutFactor);

    return tex * sampleColor * opacity;
}
technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}