float4 border = (1.0, 1.0, 1.0, 1.0);
float4 mask;
float2 offset;
float2 spriteRatio;
float2 conversion;

sampler2D samplerTex;

texture sampleTexture;
sampler2D samplerTex2 = sampler_state
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
		float4 color2 = tex2D(samplerTex2, coords + offset);
		return color2;
	}
	else
	{
		if (all(up == mask))
		{
			return border;
		}
		else if (all(down == mask))
		{
			return border;
		}
		else if (all(left == mask))
		{
			return border;
		}
		else if (all(right == mask))
		{
			return border;
		}
	}
	return color;
}

technique Technique1
{
	pass Parallax
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
};