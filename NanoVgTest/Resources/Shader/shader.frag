#version 330 core

out vec4 FragColor; 
in vec2 TexCoords;

uniform float hue; //0.0
uniform float saturation; //30.0
uniform float brightness; //1.0
uniform float ntscFreqScale; //(1.)
uniform int impulseResponseSize; //29

uniform vec2 iResolution;
uniform sampler2D iChannel0;
uniform float iTime;

uniform bool enabled;
uniform float grainAmplitude;
uniform float maskSize;
uniform float scanlineSize;
uniform float jitterChance;
uniform float trackingLossChance;

float ntscFreqColor = (1.0 / (4.0 * ntscFreqScale));
float ntscFreqLuma = (1.0 / (8.0 * ntscFreqScale));
float ntscFreqGreyscale = (1.0 / 50.0);

float pi = atan(1.0)*4.0;
float tau = atan(1.0)*8.0;

mat3 yiq2rgb = mat3(1.000, 1.000, 1.000,
                    0.956,-0.272,-1.106,
                    0.621,-0.647, 1.703);

mat3 rgb2yiq = mat3(0.299, 0.596, 0.211,
                    0.587,-0.274,-0.523,
                    0.114,-0.322, 0.312);

vec3 kernel[3];

vec3 mod289(vec3 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec2 mod289(vec2 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec3 permute(vec3 x) {
  return mod289(((x*34.0)+1.0)*x);
}

float rand(vec2 v)
{
	v.x = mod(v.x, 1000);
	v.y = mod(v.y, 1000);

	const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
						0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
						-0.577350269189626,  // -1.0 + 2.0 * C.x
						0.024390243902439); // 1.0 / 41.0
	// First corner
	vec2 i  = floor(v + dot(v, C.yy) );
	vec2 x0 = v -   i + dot(i, C.xx);

	// Other corners
	vec2 i1;
	//i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
	//i1.y = 1.0 - i1.x;
	i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
	// x0 = x0 - 0.0 + 0.0 * C.xx ;
	// x1 = x0 - i1 + 1.0 * C.xx ;
	// x2 = x0 - 1.0 + 2.0 * C.xx ;
	vec4 x12 = x0.xyxy + C.xxzz;
	x12.xy -= i1;

	// Permutations
	i = mod289(i); // Avoid truncation effects in permutation
	vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))	+ i.x + vec3(0.0, i1.x, 1.0 ));

	vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
	m = m*m ;
	m = m*m ;

	// Gradients: 41 points uniformly over a line, mapped onto a diamond.
	// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

	vec3 x = 2.0 * fract(p * C.www) - 1.0;
	vec3 h = abs(x) - 0.5;
	vec3 ox = floor(x + 0.5);
	vec3 a0 = x - ox;

	// Normalise gradients implicitly by scaling m
	// Approximation of: m *= inversesqrt( a0*a0 + h*h );
	m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

	// Compute final noise value at P
	vec3 g;
	g.x  = a0.x  * x0.x  + h.x  * x0.y;
	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
	return ((130.0 * dot(m, g)) + 1.) / 2.;
}

vec2 ntscInterlaceJitter(vec2 randvec, vec2 uv)
{
    int uvY = int(uv.y * iResolution.y / maskSize);
    if (rand(randvec + vec2(0., uvY)) < jitterChance && mod(float(uvY), 2.) == 0.)
        uv.x += (scanlineSize / iResolution.x) * (rand(randvec + vec2(0., uvY)) * 2. - 1.);
    return uv;
}

//Angle -> 2D rotation matrix 
mat2 rotate(float a)
{
    return mat2( cos(a), sin(a),
                -sin(a), cos(a));
}

//Complex multiply
vec2 cmul(vec2 a, vec2 b)
{
   return vec2((a.x * b.x) - (a.y * b.y), (a.x * b.y) + (a.y * b.x));
}

float sinc(float x)
{
	return (x == 0.0) ? 1.0 : sin(x*pi)/(x*pi);   
}

//https://en.wikipedia.org/wiki/Window_function
float WindowBlackman(float a, int N, int i)
{
    float a0 = (1.0 - a) / 2.0;
    float a1 = 0.5;
    float a2 = a / 2.0;
    
    float wnd = a0;
    wnd -= a1 * cos(2.0 * pi * (float(i) / float(N - 1)));
    wnd += a2 * cos(4.0 * pi * (float(i) / float(N - 1)));
    
    return wnd;
}

//FIR lowpass filter 
//Fc = Cutoff freq., Fs = Sample freq., N = # of taps, i = Tap index
float Lowpass(float Fc, float Fs, int N, int i)
{    
    float wc = (Fc/Fs);
    
    float wnd = WindowBlackman(0.16, N, i);
    
    return 2.0*wc * wnd * sinc(2.0*wc * float(i - N/2));
}

//FIR bandpass filter 
//Fa/Fb = Low/High cutoff freq., Fs = Sample freq., N = # of taps, i = Tap index
float Bandpass(float Fa, float Fb, float Fs, int N, int i)
{    
    float wa = (Fa/Fs);
    float wb = (Fb/Fs);
    
    float wnd = WindowBlackman(0.16, N, i);
    
    return 2.0*(wb-wa) * wnd * (sinc(2.0*wb * float(i - N/2)) - sinc(2.0*wa * float(i - N/2)));
}

//Complex oscillator, Fo = Oscillator freq., Fs = Sample freq., n = Sample index
vec2 Oscillator(float Fo, float Fs, float n)
{
    float phase = (tau*Fo*floor(n))/Fs;
    return vec2(cos(phase),sin(phase));
}

vec4 bufferA(vec2 fragCoord)
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord/iResolution.xy;

    // Output to screen
    return texture(iChannel0, uv);
}

vec4 bufferB(vec2 fragCoord )
{
    float Fs = iResolution.x;
    float Fcol = Fs * ntscFreqColor;
    float n = floor(fragCoord.x);
    
    vec3 cRGB = bufferA(fragCoord).rgb;
    vec3 cYIQ = rgb2yiq * cRGB;
    
    vec2 cOsc = Oscillator(Fcol, Fs, n);
    
    float sig = cYIQ.x + dot(cOsc, cYIQ.yz);

    return vec4(sig,0,0,0);
}

vec4 bufferC(vec2 fragCoord)
{
    float Fs = iResolution.x;
    float Fcol = Fs * ntscFreqColor;
    float Fcolbw = Fs * ntscFreqGreyscale;
    float Flumlp = Fs * ntscFreqLuma;
    float n = floor(fragCoord.x);
    
    float y_sig = 0.0;    
    float iq_sig = 0.0;
    
    vec2 cOsc = Oscillator(Fcol, Fs, n);
	
    n += float(impulseResponseSize)/2.0;
    
    //Separate luma(Y) & chroma(IQ) signals
    for(int i = 0;i < impulseResponseSize;i++)
    {
        int tpidx = impulseResponseSize - i - 1;
        float lp = Lowpass(Flumlp, Fs, impulseResponseSize, tpidx);
        float bp = Bandpass(Fcol - Fcolbw, Fcol + Fcolbw, Fs, impulseResponseSize, tpidx);
        
        y_sig += bufferB(vec2(n - float(i), fragCoord.y)).r * lp;
        iq_sig += bufferB(vec2(n - float(i), fragCoord.y)).r * bp;
    }
    
    //Shift IQ signal down from Fcol to DC 
    vec2 iq_sig_mix = cmul(vec2(iq_sig, 0), cOsc);
    
    return vec4(y_sig, iq_sig_mix, 0);
}

vec4 ntsc(vec2 fragCoord)
{
    float Fs = iResolution.x;
    float Fcol = Fs * ntscFreqColor;
    float Flumlp = Fs * ntscFreqLuma;
    float n = floor(fragCoord.x);
    
	vec2 uv = fragCoord.xy;
    
    float luma = bufferC(uv).r;
    vec2 chroma = vec2(0);
    
    //Filtering out unwanted high freqency content from the chroma(IQ) signal.
    for(int i = 0;i < impulseResponseSize;i++)
    {
        int tpidx = impulseResponseSize - i - 1;
        float lp = Lowpass(Flumlp, Fs, impulseResponseSize, tpidx);
        chroma += bufferC(uv - vec2(i - impulseResponseSize / 2, 0)).yz * lp;
    }
    
    chroma *= rotate(tau * hue);
    
    vec3 color = yiq2rgb * vec3(brightness * luma, chroma * saturation);
    
    return vec4(color, 0);
}

void main()
{
	if (!enabled)
	{
		FragColor = texture(iChannel0, TexCoords);
		return;
	}

    // current time seed
    vec2 now = vec2(0., iTime * 10.);

    // create UV
    vec2 uv = TexCoords;

	if (rand(now) < trackingLossChance)
		uv.y += (maskSize / iResolution.x);
    
    // jitter lines
    uv = ntscInterlaceJitter(now, uv);
    
    // sample texture
    vec4 col = ntsc(uv * iResolution);

    // add grain
    col += vec4(vec3(grainAmplitude * (rand(gl_FragCoord.xy + now) * 2. - 1.)), 0.0);
    
    // output to screen
    FragColor = vec4(col.rgb, 1.0);
}