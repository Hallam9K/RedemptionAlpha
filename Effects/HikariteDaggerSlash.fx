texture sampleTexture;
sampler2D samplerTex = sampler_state
{
    texture = <sampleTexture>;
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

matrix transformMatrix;
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    output.Position = mul(input.Position, transformMatrix);

    return output;
}

bool horizontalFlip;
bool brightTip;
float minimumDistanceFromCenter;
float squishToEdgeFactor;
float squishPowerInverse;
float interpolantStart;
float interpolantEnd;
float intensity;
float4 tipColor;

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TexCoords;
    if (horizontalFlip)
        coords.y = 1 - coords.y;
    
    float distanceFromCenter = minimumDistanceFromCenter + squishToEdgeFactor * pow(abs(coords.x), squishPowerInverse);
    float4 texColor = tex2D(samplerTex, float2(coords.x, clamp(distanceFromCenter * coords.y - distanceFromCenter + 1, 0, 1)));
    texColor.a = texColor.r;
    
    float4 tip = tipColor * smoothstep(interpolantStart, interpolantEnd, coords.y) * intensity;
    
    float opacity = clamp(sin(coords.x * 3.1415) * 2 - 0.05, 0, 1);

    float4 color0 = input.Color * texColor * opacity;
    float4 color1 = tip * input.Color * texColor * opacity;
    
    if (brightTip)
        color1.a = 0;

    return color0 + color1;
}
technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};