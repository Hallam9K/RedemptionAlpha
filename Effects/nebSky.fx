sampler2D uImage0 : register(s0);
texture2D _stars;
texture2D _displace;
texture2D _mask;
texture2D _maskano;

sampler2D stars = sampler_state
{
    Texture = <_stars>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D displace = sampler_state
{
    Texture = <_displace>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D mask = sampler_state
{
    Texture = <_mask>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D maskano = sampler_state
{
    Texture = <_maskano>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
float gradientMax = 4;
float gradientMin = 0.3;
float gradientScale = 2;

float gamma = 1.5;
float starScale = 1;
float vignetteScale = 1;
float uTime;
float4 skyColor1 = float4(1, 0.15, 1.5, 1);
float4 skyColor2 = float4(1, 0.5, 2.5, 1);
float4 bgColor1 = float4(.3, 0, 1.5, 1) * 0.8;
float4 bgColor2 = float4(2, 0.15, 0.5, 1) * 0.5;
float starSpeedMult = 0.1;
float nebulaSpeedMult = 0.2;

float4 main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 centerizedUV = coords + float2(-0.5, 0);
    float2 actualUV = coords;
    

    float rot = uTime * 0.005 * nebulaSpeedMult;
    float cRot = cos(rot);
    float sRot = sin(rot);
    float2 uv = float2(
		centerizedUV.x * cRot + centerizedUV.y * -sRot,
		centerizedUV.x * sRot + centerizedUV.y * cRot
	);
	
    float4 maskSample = tex2D(mask, 1.2 * uv + uTime * 0.01);
    float4 cMask = maskSample * maskSample * maskSample * 5;
	
    float4 cStars = max(tex2D(stars, uv - float2(uTime, 0) * -0.031 * starSpeedMult)
    * tex2D(stars, starScale * 3.5 * uv + uTime * 0.01 * starSpeedMult) * 4
    , tex2D(stars, starScale * 1.7 * centerizedUV + float2(uTime * 0.1, -uTime) * 0.0236 * starSpeedMult)
    * tex2D(stars, starScale * 1.3 * uv + uTime * 0.01 * starSpeedMult) * 2);
    cStars = clamp(cStars * 5, 0, 3);
    
    float4 vignette = saturate(((1 - saturate(length(actualUV - float2(0.5, 0.5))) * vignetteScale)) * 1.5);
	
    rot = uTime * 0.02 * nebulaSpeedMult;
    cRot = cos(rot);
    sRot = sin(rot);
    uv = float2(
		centerizedUV.x * cRot + centerizedUV.y * -sRot,
		centerizedUV.x * sRot + centerizedUV.y * cRot
	);
	
    float4 d1 = tex2D(displace, 2.0 * uv);
    float4 d2 = tex2D(displace, centerizedUV);
    float4 m1 = tex2D(maskano, uv - uTime * 0.01 * nebulaSpeedMult);
    float4 cNoise = d1 * d2 * m1 * clamp(cMask + 0.5, 0, 1) * 2.5;
    cNoise *= lerp(skyColor1, skyColor2, cNoise * 0.45 + vignette * 0.3) * vignette;
    
    float4 starWeight = pow(tex2D(stars, 3 * centerizedUV + float2(uTime * starSpeedMult / 7, -uTime * 0.003))
   	+ tex2D(stars, 2 * centerizedUV + float2(uTime * starSpeedMult / 13, -uTime * 0.005)), 2)
   	* cNoise * (sin(5 * centerizedUV.x + uTime * starSpeedMult / 13) + 2);
    
    float gradient = lerp(gradientMin, gradientMax, clamp(actualUV.y * gradientScale, 0, 1));
    
    float _gamma = 1.0 / gamma;
    return gradient
    * pow(abs(lerp(bgColor1, bgColor2, cNoise) * 0.16
+ cNoise * vignette * 0.5), float4(_gamma, _gamma, _gamma, _gamma))
    + cStars * lerp(lerp(skyColor1, skyColor2, cNoise * 0.45 + vignette * 0.3), cStars, cStars) + starWeight;
}

technique Technique1
{
    pass nebSky
    {
        PixelShader = compile ps_3_0 main();
    }
}