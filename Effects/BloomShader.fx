sampler uImage0 : register(s0);

//Genuine params
float2 resolution;
float satLevel;
float radius;
float alphaMult;

float4 reColor;

//Calculations
float Epsilon = 1e-10;
 
float3 RGBtoHCV(in float3 RGB)
{
    // Based on work by Sam Hocevar and Emil Persson
    float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
    float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
    float C = Q.x - min(Q.w, Q.y);
    float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
    return float3(H, C, Q.x);
}

float3 RGBtoHSV(in float3 RGB)
{
    float3 HCV = RGBtoHCV(RGB);
    float S = HCV.y / (HCV.z + Epsilon);
    return float3(HCV.x, S, HCV.z);
}

float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R, G, B));
}

float3 HSVtoRGB(in float3 HSV)
{
    float3 RGB = HUEtoRGB(HSV.x);
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float4 BloomFloat(float2 coords : TEXCOORD0) : COLOR0
{
    // GAUSSIAN BLUR SETTINGS
    
    float Directions = 2.5; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 4.0; // BLUR QUALITY (Default 4.0 - More is better but slower)
    float Size = radius; // BLUR SIZE (Radius)
    
    // GAUSSIAN BLUR SETTINGS
   
    float2 Radius = Size / resolution;
    
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = coords;
    
    // Pixel colour
    float4 Color = float4(reColor.x, reColor.y, reColor.z, tex2D(uImage0, uv).a * reColor.a);
    
    // Blur calculations
    for (float d = 0.0; d < 6.28; d += 6.28 / Directions)
    {
        for (float i = 1.0 / Quality; i <= 1.0; i += 1.0 / Quality)
        {
            Color += tex2D(uImage0, uv + (float2(cos(d), sin(d)) * Radius * i));
        }
    }
    
    // Output to screen
    Color /= Quality * Directions;
    
    //return Color;
    
    float4 newColor = float4(RGBtoHSV(Color.xyz), Color.w);
    
    newColor.y = satLevel;
    
    //newColor.w = (((Color.x + Color.y + Color.z) / 3) * alphaMult);
    
    newColor.w *= alphaMult;

    
    return float4(HSVtoRGB(newColor.xyz), newColor.w);
}

technique BloomShader
{
    pass P0
    {
        PixelShader = compile ps_2_0 BloomFloat();
    }
};