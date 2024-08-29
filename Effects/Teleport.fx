sampler uImage0 : register(s0);
sampler noiseTexture : register(s1);

float2 uImageSize0;
float4 uSourceRect;
float2 uImageSize1;

float direction;
float progress;
float widthFactor;
float4 lineColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float opacity = saturate(direction * widthFactor * (frameY - progress));

    if (opacity > 0 && opacity < 1)
    {
        float4 noise = tex2D(noiseTexture, coords * uImageSize1);
        noise.a = noise.r;
        color *= lineColor * noise;
    }
    else
    {
        color *= opacity;
    }
    return color * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}