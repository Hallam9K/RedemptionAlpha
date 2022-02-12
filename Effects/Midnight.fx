float2 offset;
float2 conversion;
float4 innerColor;
float4 borderColor;

matrix transformMatrix;

texture sampleTexture;
sampler2D samplerTex = sampler_state
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

//VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
//{
//    VertexShaderOutput output;
    
//    output.Color = input.Color;
//    output.TexCoords = input.TexCoords;
//    output.Position = mul(input.Position, transformMatrix);

//    return output;
//}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(samplerTex, input.TexCoords);
	if (color.g != 1.0f)
		return color;
	float4 up = tex2D(samplerTex, input.TexCoords + float2(0.0f, conversion.y));
	float4 down = tex2D(samplerTex, input.TexCoords - float2(0.0f, conversion.y));
	float4 left = tex2D(samplerTex, input.TexCoords + float2(conversion.x, 0.0f));
	float4 right = tex2D(samplerTex, input.TexCoords - float2(conversion.x, 0.0f));
	if (up.g < 1.0f)
	{
		return borderColor;
	}
	else if (down.g < 1.0f)
	{
		return borderColor;
	}
	else if (left.g < 1.0f)
	{
		return borderColor;
	}
	else if (right.g < 1.0f)
	{
		return borderColor;
	}
	return innerColor;
}

technique Technique1
{
	pass Parallax
	{
        //VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
};