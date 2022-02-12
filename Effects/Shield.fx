float2 offset;
float2 conversion;
float4 inner;
float4 border;
float time;
float sinMult;
float2 spriteRatio;
float frameAmount;

matrix transformMatrix;

sampler2D samplerTex;

texture sampleTexture;
sampler2D samplerTex2 = sampler_state
{
	texture = <sampleTexture>;
	AddressU = wrap;
	AddressV = wrap;
};
texture sampleTextur2;
sampler2D samplerTex3 = sampler_state
{
	texture = <sampleTextur2>;
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

float4 Shield(VertexShaderOutput input) : COLOR
{
	// Get our colors
	float4 color = tex2D(samplerTex, input.TexCoords);
	float4 up = tex2D(samplerTex, input.TexCoords + float2(0.0f, conversion.y));
	float4 down = tex2D(samplerTex, input.TexCoords - float2(0.0f, conversion.y));
	float4 left = tex2D(samplerTex, input.TexCoords + float2(conversion.x, 0.0f));
	float4 right = tex2D(samplerTex, input.TexCoords - float2(conversion.x, 0.0f));
	// Multiply coords by amount of frames to account for stretching UV coords over Y axis
	float preY = input.TexCoords.y * frameAmount;
	float postY = preY - floor(preY);
	float mult = sin(postY * sinMult + time) + 1.5f;

	float2 coords = float2(input.TexCoords.x, preY) * spriteRatio;
	coords += offset;
	float4 lerp1 = lerp(inner * mult, inner * mult * 2.0, mult - 0.35);
	if (color.a == 1.0)
	{
		float4 color2 = tex2D(samplerTex2, coords);
		if (all(color2 == 1.0))
		{
			return half4(color.x + lerp1.x, color.y + lerp1.y, color.z + lerp1.z, color.a);
		}
		else
		{
			float4 lerp3 = lerp(inner / 2.0, inner, mult - 0.35);
			color.xyz += (lerp3.xyz);
			return float4(color);
		}
	}
	if (up.a + down.a + left.a + right.a > 0.0 && color.a == 0.0)
		return lerp1;
	return color;
}

technique Technique1
{
	pass Shield
	{
		PixelShader = compile ps_2_0 Shield();
	}
};