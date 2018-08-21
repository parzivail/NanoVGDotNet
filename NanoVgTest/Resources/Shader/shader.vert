#version 330 core
layout (location = 0) in vec2 aPos;

const vec2 madd=vec2(0.5,0.5);

out vec2 TexCoords;

void main()
{
    gl_Position = vec4(aPos.xy, 0.0, 1.0); 
    TexCoords = aPos.xy*madd+madd;
} 