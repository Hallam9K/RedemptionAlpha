float4 border = (1.0, 1.0, 1.0, 1.0);
float4 mask;
float2 offset;
float2 spriteRatio;
float2 conversion;

sampler2D samplerTex;

texture sampleTexture;
sampler2D samplerTex1 = sampler_state
{
	texture = <sampleTexture>;
	AddressU = wrap;
	AddressV = wrap;
};
texture sampleTexture2;
sampler2D samplerTex2 = sampler_state
{
	texture = <sampleTexture2>;
	AddressU = wrap;
	AddressV = wrap;
};
texture sampleTexture3;
sampler2D samplerTex3 = sampler_state
{
	texture = <sampleTexture3>;
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


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(samplerTex, input.TexCoords);
	float4 up = tex2D(samplerTex, input.TexCoords + float2(0.0f, conversion.y));
	float4 down = tex2D(samplerTex, input.TexCoords - float2(0.0f, conversion.y));
	float4 left = tex2D(samplerTex, input.TexCoords + float2(conversion.x, 0.0f));
	float4 right = tex2D(samplerTex, input.TexCoords - float2(conversion.x, 0.0f));

	float2 coords = input.TexCoords;
	coords.x *= spriteRatio.x;
	coords.y *= spriteRatio.y;

	if (all(color == mask))
	{
		float4 color1 = tex2D(samplerTex1, coords + offset);
		float4 color2 = tex2D(samplerTex2, coords + offset * 1.1f);
		float4 color3 = tex2D(samplerTex3, coords + offset * 1.3f);
		if (!all(color3 > 0.0f))
		{
			if (!all(color2 > 0.0f))
				return color1;
			return color2;
		}
		return color3;
	}
	if ((up.a == 1.0 || down.a == 1.0 || left.a == 1.0 || right.a == 1.0) && color.a == 0.0)
		return border;
	return color;
}

technique Technique1
{
	pass Parallax
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
};