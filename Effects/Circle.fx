float3 uColor : register(C0);
float uOpacity : register(C1);
float uProgress : register(C2);

// Code by John Snail
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float circle = length((uv - 0.5) * uProgress) - (uProgress / 2 - 1);
	if (circle > 1)
	{
		circle = 0;
	}
	return float4(uColor.r, uColor.g, uColor.b, uOpacity) * circle;
}

technique Tech1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}