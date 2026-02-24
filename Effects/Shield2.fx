sampler2D samplerTex0; //mask
texture sampleTexture; //hexagon
sampler2D samplerTex1 = sampler_state
{
	texture = <sampleTexture>;
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

float2 offset;
float2 conversion;
float4 inner;
float4 border;
float time;
float sinMult;
float2 spriteRatio;
float frameAmount;
float2 direction;
float visibleRatio;

float Pi = 3.141592;
float WrapAngle(float angle)
{
    if ((angle > -Pi) && (angle <= Pi))
    {
        return angle;
    }
    angle %= (Pi * 2);
    if (angle <= -Pi)
    {
        return angle + (Pi * 2);
    }
    if (angle > Pi)
    {
        return angle - (Pi * 2);
    }
    return angle;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	// Get our colors
    float4 color = tex2D(samplerTex0, input.TexCoords);
    float4 up = tex2D(samplerTex0, input.TexCoords + float2(0.0f, conversion.y));
    float4 down = tex2D(samplerTex0, input.TexCoords - float2(0.0f, conversion.y));
    float4 left = tex2D(samplerTex0, input.TexCoords + float2(conversion.x, 0.0f));
    float4 right = tex2D(samplerTex0, input.TexCoords - float2(conversion.x, 0.0f));

	// Multiply coords by amount of frames to account for stretching UV coords over Y axis
    float preY = input.TexCoords.y * frameAmount;
    float postY = preY - floor(preY);
    float mult = sin(postY * sinMult + time) + 1.5f;

    float2 coords = float2(input.TexCoords.x, preY);
    float2 centerCoords = coords * 2 - 1;
    float angle = acos(dot(direction, centerCoords) / (length(direction) * length(centerCoords)));
    float diff = angle / (Pi * 2);
    float distSQ = dot(centerCoords, centerCoords);
    float opacity = distSQ * smoothstep(0.5 * (1 - visibleRatio), 0.5 * (1 + visibleRatio), diff);
	
    float4 lerp1 = lerp(inner * mult, inner * mult * 2.0, mult - 0.35);
    float4 color1 = tex2D(samplerTex1, (coords + offset) * spriteRatio);
    if (color.a == 1)
    {
        if (all(color1 == 1.0))
        {
            color.xyz += (lerp1.xyz * 2);
            color.a *= opacity;
            return float4(color);
        }
        else
        {
            float4 lerp2 = lerp(inner / 2.0, inner, mult - 0.35);
            color.xyz += (lerp2.xyz * 1);
            color.a *= opacity;
            return float4(color);
        }
    }
    if (up.a + down.a + left.a + right.a > 0.0 && color.a == 0.0)
    {
        lerp1.a *= opacity;
        return lerp1;
    }
    return color;
}
technique Technique1
{
	pass ShieldPass
	{
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};