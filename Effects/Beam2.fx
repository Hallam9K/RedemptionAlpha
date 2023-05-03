sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float4 uSecondaryColor;
float progress;
float4 uColor;

matrix WorldViewProjection;
texture uTexture;
sampler textureSampler = sampler_state
{
    Texture = (uTexture);
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderInput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
};

float uFadeHeight = .95f;
float TextureMod = 1;
float lerpCap = 2;
float strengthScale = .001;
float strengthCap = 2;
float textureY = .001;

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 noise = tex2D(textureSampler, float2((input.TextureCoordinates.x / max(input.TextureCoordinates.y, textureY)) % TextureMod, input.TextureCoordinates.y + progress));
    
    float strength = pow(1 - (abs(input.TextureCoordinates.x - 0.5f) * 2 / max(1 - input.TextureCoordinates.y, strengthScale)), 2);
    strength *= pow(input.TextureCoordinates.y, 2);
    strength *= 1.5f;
    strength += strength * noise.r * input.TextureCoordinates.y; //Add in the noise
    strength = min(strength, strengthCap); //Cap the strength at a certain value
    
    float4 color;
    if (strength < 0.5f)
        color = lerp(float4(0, 0, 0, 0), uColor, strength * lerpCap);
    else
        color = lerp(uColor, uSecondaryColor, (strength - 0.5f) * lerpCap);
    
    if (input.TextureCoordinates.y > uFadeHeight)
        strength *= pow(1 - ((input.TextureCoordinates.y - uFadeHeight) / (1 - uFadeHeight)), 3);
        
    color *= strength;
    return input.Color * color;
}

technique BasicColorDrawing
{
    pass Default
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
};