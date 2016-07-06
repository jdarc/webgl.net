#if defined(GL_ES)
varying mediump vec3 v_normal;
#else
varying vec3 v_normal;
#endif

void main()
{
    gl_FragColor = vec4(v_normal/2.0+vec3(0.5), 1);
}