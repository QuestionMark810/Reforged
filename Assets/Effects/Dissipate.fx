sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float time;
float sizeMult;

texture noiseTexture;
sampler noise
{
    Texture = (noiseTexture);
};

float4 MainPS(float2 coords : TEXCOORD0, float4 ocolor : COLOR0) : COLOR0
{
    float4 texColor = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(noise, float2((coords.x + time * 1.1) % 1, (coords.y + time * .9) % 1));
    
    float dist = (abs(coords.x - 0.5f) + abs(coords.y - 0.5f) * 2) * sizeMult;
    float strength = noiseColor.r * dist;
    
    return ocolor * texColor * strength;
}

technique BasicColorDrawing
{
    pass MainPS
    {
        PixelShader = compile ps_2_0 MainPS();
    }
};