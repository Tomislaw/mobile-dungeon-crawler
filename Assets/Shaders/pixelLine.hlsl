


void IsPixelInLine_float(float2 vEndpointA, float2 vEndpointB, float fThickness, float2 vPointP, float fDottedLineLength, float fDottedLineOffset, out bool outIsPixelInLine)
{
	// Origin in screen space
	float4 projectionSpaceOrigin = mul(UNITY_MATRIX_VP, float4(0.0f, 0.0f, 0.0f, 1.0f));
    //float2 vOrigin = ComputeScreenPos(projectionSpaceOrigin, -1.0f).xy * _ScreenParams.xy;
    float vOrigin = float2(200, 200);
	// The amount of pixels the camera has moved regarding a thickness-wide block of pixels
	//vOrigin = fmod(vOrigin, float2(fThickness, fThickness));
	//vOrigin = round(vOrigin);

	// This moves the line N pixels, this is necessary due to the camera moves 1 pixel each time and the line may be wider than 1 pixel
	// so this avoids the line jumping from one block (thickness-wide) to the next, and instead its movement is smoother by moving pixel by pixel
	vPointP += float2(fThickness, fThickness) - vOrigin;

	float4 projectionSpaceEndpointA = mul(UNITY_MATRIX_VP, float4(vEndpointA.x, vEndpointA.y, 0.0f, 1.0f));
	float4 projectionSpaceEndpointB = mul(UNITY_MATRIX_VP, float4(vEndpointB.x, vEndpointB.y, 0.0f, 1.0f));
	
	// Endpoints in screen space
    //vEndpointA = ComputeScreenPos(projectionSpaceEndpointA, -1.0f).xy * _ScreenParams.xy;
    //vEndpointB = ComputeScreenPos(projectionSpaceEndpointB, -1.0f).xy * _ScreenParams.xy;

	vEndpointA = round(vEndpointA);
	vEndpointB = round(vEndpointB);

	vEndpointA += float2(fThickness, fThickness) - vOrigin;
	vEndpointB += float2(fThickness, fThickness) - vOrigin;

	vEndpointA = vEndpointA - fmod(vEndpointA, float2(fThickness, fThickness));
	vEndpointB = vEndpointB - fmod(vEndpointB, float2(fThickness, fThickness));
	vEndpointA = round(vEndpointA);
	vEndpointB = round(vEndpointB);

	vPointP = vPointP - fmod(vPointP, float2(fThickness, fThickness));
	vPointP = round(vPointP);

	int x = vEndpointA.x;
	int y = vEndpointA.y;
	int x2 = vEndpointB.x;
	int y2 = vEndpointB.y;
	int pX = vPointP.x;
	int pY = vPointP.y;
	int w = x2 - x;
	int h = y2 - y;
	int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

	if (w<0) dx1 = -fThickness ; else if (w>0) dx1 = fThickness;
	if (h<0) dy1 = -fThickness ; else if (h>0) dy1 = fThickness;
	if (w<0) dx2 = -fThickness ; else if (w>0) dx2 = fThickness;

	int longest = abs(w);
	int shortest = abs(h);

	if (longest <= shortest)
	{
		longest = abs(h);
		shortest = abs(w);

		if (h < 0)
			dy2 = -fThickness; 
		else if (h > 0)
			dy2 = fThickness;
		
		dx2 = 0;
	}

	int numerator = longest >> 1;

	outIsPixelInLine = false;

	for (int i=0; i <= longest; i+=fThickness)
	{
		if(x == pX && y == pY)
		{
			outIsPixelInLine = fmod(floor((i + fDottedLineOffset * fThickness) / fDottedLineLength / fThickness), 2.0f) == 0;
			break;
		}

		numerator += shortest;

		if (numerator >= longest)
		{
			numerator -= longest;
			x += dx1;
			y += dy1;
		}
		else
		{
			x += dx2;
			y += dy2;
		}
	}
}