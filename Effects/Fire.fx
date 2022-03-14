sampler2D samplerTex;
matrix transformMatrix;
texture sampleTexture;
texture sampleTexture2;
sampler2D samplerTex2 = sampler_state
{
	texture = <sampleTexture2>;
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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(samplerTex, input.TexCoords);
	if(all(color == 0))
		return color;
	float luminosity = (color.r + color.g + color.b) / 3.0f;
	float4 value = tex2D(samplerTex2, float2(luminosity, 0.0f)) * luminosity;
	value.a = color.a;
	return value;
}

technique Technique1
{
	pass Parallax
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
};