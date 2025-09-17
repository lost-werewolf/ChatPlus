sampler uImage0 : register(s0);
float imageWidth;
float imageHeight;
float boldRadius;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 light : COLOR0) : COLOR0
{
    float paddingX = boldRadius / imageWidth;
    float paddingY = boldRadius / imageHeight;
    float aa = tex2D(uImage0, coords).a;
    float ba = tex2D(uImage0, coords + float2(0, paddingY)).a;
    float da = tex2D(uImage0, coords + float2(paddingX, 0)).a;
    float fa = tex2D(uImage0, coords + float2(0, -paddingY)).a;
    float ha = tex2D(uImage0, coords + float2(-paddingX, 0)).a;
    paddingX *= 0.7;
    paddingY *= 0.7;
    float ca = tex2D(uImage0, coords + float2(paddingX, paddingY)).a * 1.0;
    float ea = tex2D(uImage0, coords + float2(paddingX, -paddingY)).a * 1.0;
    float ga = tex2D(uImage0, coords + float2(-paddingX, -paddingY)).a * 1.0;
    float ia = tex2D(uImage0, coords + float2(-paddingX, paddingY)).a * 1.0;
    return float4(1, 1, 1, 1) * max(aa, max(ba, max(ca, max(da, max(ea, max(fa, max(ga, max(ha, ia))))))));
}

technique Technique1
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}