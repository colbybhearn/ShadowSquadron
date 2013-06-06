float4x4 xViewProjection;
struct VertexToPixel
{
    float4 Position     : POSITION;
    float4 Color        : COLOR0;
    float3 Position3D    : TEXCOORD0;
};

struct PixelToFrame
{
    float4 Color        : COLOR0;
};

VertexToPixel SimplestVertexShader( float4 inPos : POSITION, float4 inColor : COLOR0)
{
    VertexToPixel Output = (VertexToPixel)0;
    
    Output.Position =mul(inPos, xViewProjection);
    Output.Color = inColor;
	Output.Position3D = inPos;

    return Output;
}


PixelToFrame OurFirstPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;    

    Output.Color = PSIn.Color;    
	Output.Color.rgb = PSIn.Position3D.xyz;

    return Output;
}


technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SimplestVertexShader();
        PixelShader = compile ps_2_0 OurFirstPixelShader();
    }
}

