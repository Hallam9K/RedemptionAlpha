matrix transformMatrix;
texture tex0;
sampler2D samplerTex0 = sampler_state
{
    texture = <tex0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture noiseTex0;
sampler2D samplerTex1 = sampler_state
{
    texture = <noiseTex0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture noiseTex1;
sampler2D samplerTex2 = sampler_state
{
    texture = <noiseTex1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
struct VertexShaderInput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    output.Position = mul(input.Position, transformMatrix);

    return output;
}

float randomY;
float progress;
float4 uColor;
float2 noiseResolution;

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TexCoords;

    //use red value of noise texture to create random lightning nodes
    float4 noise = tex2D(samplerTex1, float2(coords.x * noiseResolution.x, randomY));
    
    float v = coords.y + (noise.r - 0.5f) * saturate(sin(coords.x * 3.1415f));
    
    float4 base = tex2D(samplerTex0, float2(coords.x, smoothstep(0.5 - noiseResolution.y, 0.5 + noiseResolution.y, v)));
    base.a = base.r;
    
    float4 core = tex2D(samplerTex0, float2(coords.x, smoothstep(0.5 - noiseResolution.y * 0.25, 0.5 + noiseResolution.y * 0.25, v)));
    core.a = core.r;

    float opacity = saturate(sin(coords.y * 3.1415f) * 5) * saturate(sin(coords.x * 3.1415f) * 10);
    
    //noisy fadeout
    float opacity2 = saturate(tex2D(samplerTex2, coords + randomY).r - 1 + progress * 2);
    
    //round opacity to create pixel -ish fade out
    return (base * 0.35f + core) * uColor * opacity * round(opacity2);
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};