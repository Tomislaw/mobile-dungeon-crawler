void GetBoltPoint_float(float2 vEndpointA, float2 vEndpointB, float lerpValue, float2 randomization, out float2 position)
{

    float2 randomizer = lerp(randomization, float2(0, 0), abs(lerpValue * 2 - 1));
    position = lerp(vEndpointA, vEndpointB, lerpValue) + randomizer;

}