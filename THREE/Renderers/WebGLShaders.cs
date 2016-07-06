using WebGL;

namespace THREE
{
	public class WebGLShaders
	{
		private const string NewLine = "\n";

		public static class ShaderChunk
		{
			public static readonly string fog_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_FOG",
				"uniform vec3 fogColor;",
				"#ifdef FOG_EXP2",
				"uniform float fogDensity;",
				"#else",
				"uniform float fogNear;",
				"uniform float fogFar;",
				"#endif",
				"#endif"
			});
			public static readonly string fog_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_FOG",
				"float depth = gl_FragCoord.z / gl_FragCoord.w;",
				"#ifdef FOG_EXP2",
				"const float LOG2 = 1.442695;",
				"float fogFactor = exp2( - fogDensity * fogDensity * depth * depth * LOG2 );",
				"fogFactor = 1.0 - clamp( fogFactor, 0.0, 1.0 );",
				"#else",
				"float fogFactor = smoothstep( fogNear, fogFar, depth );",
				"#endif",
				"gl_FragColor = mix( gl_FragColor, vec4( fogColor, gl_FragColor.w ), fogFactor );",
				"#endif"
			});
			public static readonly string envmap_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_ENVMAP",
				"uniform float reflectivity;",
				"uniform samplerCube envMap;",
				"uniform float flipEnvMap;",
				"uniform int combine;",
				"#if defined( USE_BUMPMAP ) || defined( USE_NORMALMAP )",
				"uniform bool useRefract;",
				"uniform float refractionRatio;",
				"#else",
				"varying vec3 vReflect;",
				"#endif",
				"#endif"
			});
			public static readonly string envmap_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_ENVMAP",
				"vec3 reflectVec;",
				"#if defined( USE_BUMPMAP ) || defined( USE_NORMALMAP )",
				"vec3 cameraToVertex = normalize( vWorldPosition - cameraPosition );",
				"if ( useRefract ) {",
				"reflectVec = refract( cameraToVertex, normal, refractionRatio );",
				"} else { ",
				"reflectVec = reflect( cameraToVertex, normal );",
				"}",
				"#else",
				"reflectVec = vReflect;",
				"#endif",
				"#ifdef float_SIDED",
				"float flipNormal = ( -1.0 + 2.0 * float( gl_FrontFacing ) );",
				"vec4 cubeColor = textureCube( envMap, flipNormal * vec3( flipEnvMap * reflectVec.x, reflectVec.yz ) );",
				"#else",
				"vec4 cubeColor = textureCube( envMap, vec3( flipEnvMap * reflectVec.x, reflectVec.yz ) );",
				"#endif",
				"#ifdef GAMMA_INPUT",
				"cubeColor.xyz *= cubeColor.xyz;",
				"#endif",
				"if ( combine == 1 ) {",
				"gl_FragColor.xyz = mix( gl_FragColor.xyz, cubeColor.xyz, specularStrength * reflectivity );",
				"} else if ( combine == 2 ) {",
				"gl_FragColor.xyz += cubeColor.xyz * specularStrength * reflectivity;",
				"} else {",
				"gl_FragColor.xyz = mix( gl_FragColor.xyz, gl_FragColor.xyz * cubeColor.xyz, specularStrength * reflectivity );",
				"}",
				"#endif"
			});
			public static readonly string envmap_pars_vertex = string.Join(NewLine, new[]
			{
				"#if defined( USE_ENVMAP ) && ! defined( USE_BUMPMAP ) && ! defined( USE_NORMALMAP )",
				"varying vec3 vReflect;",
				"uniform float refractionRatio;",
				"uniform bool useRefract;",
				"#endif"
			});
			public static readonly string worldpos_vertex = string.Join(NewLine, new[]
			{
				"#if defined( USE_ENVMAP ) || defined( PHONG ) || defined( LAMBERT ) || defined ( USE_SHADOWMAP )",
				"#ifdef USE_SKINNING",
				"vec4 worldPosition = modelMatrix * skinned;",
				"#endif",
				"#if defined( USE_MORPHTARGETS ) && ! defined( USE_SKINNING )",
				"vec4 worldPosition = modelMatrix * vec4( morphed, 1.0 );",
				"#endif",
				"#if ! defined( USE_MORPHTARGETS ) && ! defined( USE_SKINNING )",
				"vec4 worldPosition = modelMatrix * vec4( position, 1.0 );",
				"#endif",
				"#endif"
			});
			public static readonly string envmap_vertex = string.Join(NewLine, new[]
			{
				"#if defined( USE_ENVMAP ) && ! defined( USE_BUMPMAP ) && ! defined( USE_NORMALMAP )",
				"vec3 worldNormal = mat3( modelMatrix[ 0 ].xyz, modelMatrix[ 1 ].xyz, modelMatrix[ 2 ].xyz ) * objectNormal;",
				"worldNormal = normalize( worldNormal );",
				"vec3 cameraToVertex = normalize( worldPosition.xyz - cameraPosition );",
				"if ( useRefract ) {",
				"vReflect = refract( cameraToVertex, worldNormal, refractionRatio );",
				"} else {",
				"vReflect = reflect( cameraToVertex, worldNormal );",
				"}",
				"#endif"
			});
			public static readonly string map_particle_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_MAP",
				"uniform sampler2D map;",
				"#endif"
			});
			public static readonly string map_particle_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_MAP",
				"gl_FragColor = gl_FragColor * texture2D( map, vec2( gl_PointCoord.x, 1.0 - gl_PointCoord.y ) );",
				"#endif"
			});
			public static readonly string map_pars_vertex = string.Join(NewLine, new[]
			{
				"#if defined( USE_MAP ) || defined( USE_BUMPMAP ) || defined( USE_NORMALMAP ) || defined( USE_SPECULARMAP )",
				"varying vec2 vUv;",
				"uniform vec4 offsetRepeat;",
				"#endif"
			});
			public static readonly string map_pars_fragment = string.Join(NewLine, new[]
			{
				"#if defined( USE_MAP ) || defined( USE_BUMPMAP ) || defined( USE_NORMALMAP ) || defined( USE_SPECULARMAP )",
				"varying vec2 vUv;",
				"#endif",
				"#ifdef USE_MAP",
				"uniform sampler2D map;",
				"#endif"
			});
			public static readonly string map_vertex = string.Join(NewLine, new[]
			{
				"#if defined( USE_MAP ) || defined( USE_BUMPMAP ) || defined( USE_NORMALMAP ) || defined( USE_SPECULARMAP )",
				"vUv = uv * offsetRepeat.zw + offsetRepeat.xy;",
				"#endif"
			});
			public static readonly string map_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_MAP",
				"vec4 texelColor = texture2D( map, vUv );",
				"#ifdef GAMMA_INPUT",
				"texelColor.xyz *= texelColor.xyz;",
				"#endif",
				"gl_FragColor = gl_FragColor * texelColor;",
				"#endif"
			});
			public static readonly string lightmap_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_LIGHTMAP",
				"varying vec2 vUv2;",
				"uniform sampler2D lightMap;",
				"#endif"
			});
			public static readonly string lightmap_pars_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_LIGHTMAP",
				"varying vec2 vUv2;",
				"#endif"
			});
			public static readonly string lightmap_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_LIGHTMAP",
				"gl_FragColor = gl_FragColor * texture2D( lightMap, vUv2 );",
				"#endif"
			});
			public static readonly string lightmap_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_LIGHTMAP",
				"vUv2 = uv2;",
				"#endif"
			});
			public static readonly string bumpmap_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_BUMPMAP",
				"uniform sampler2D bumpMap;",
				"uniform float bumpScale;",
				"vec2 dHdxy_fwd() {",
				"vec2 dSTdx = dFdx( vUv );",
				"vec2 dSTdy = dFdy( vUv );",
				"float Hll = bumpScale * texture2D( bumpMap, vUv ).x;",
				"float dBx = bumpScale * texture2D( bumpMap, vUv + dSTdx ).x - Hll;",
				"float dBy = bumpScale * texture2D( bumpMap, vUv + dSTdy ).x - Hll;",
				"return vec2( dBx, dBy );",
				"}",
				"vec3 perturbNormalArb( vec3 surf_pos, vec3 surf_norm, vec2 dHdxy ) {",
				"vec3 vSigmaX = dFdx( surf_pos );",
				"vec3 vSigmaY = dFdy( surf_pos );",
				"vec3 vN = surf_norm;",
				"vec3 R1 = cross( vSigmaY, vN );",
				"vec3 R2 = cross( vN, vSigmaX );",
				"float fDet = dot( vSigmaX, R1 );",
				"vec3 vGrad = sign( fDet ) * ( dHdxy.x * R1 + dHdxy.y * R2 );",
				"return normalize( abs( fDet ) * surf_norm - vGrad );",
				"}",
				"#endif"
			});
			public static readonly string normalmap_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_NORMALMAP",
				"uniform sampler2D normalMap;",
				"uniform vec2 normalScale;",
				"vec3 perturbNormal2Arb( vec3 eye_pos, vec3 surf_norm ) {",
				"vec3 q0 = dFdx( eye_pos.xyz );",
				"vec3 q1 = dFdy( eye_pos.xyz );",
				"vec2 st0 = dFdx( vUv.st );",
				"vec2 st1 = dFdy( vUv.st );",
				"vec3 S = normalize(  q0 * st1.t - q1 * st0.t );",
				"vec3 T = normalize( -q0 * st1.s + q1 * st0.s );",
				"vec3 N = normalize( surf_norm );",
				"vec3 mapN = texture2D( normalMap, vUv ).xyz * 2.0 - 1.0;",
				"mapN.xy = normalScale * mapN.xy;",
				"mat3 tsn = mat3( S, T, N );",
				"return normalize( tsn * mapN );",
				"}",
				"#endif"
			});
			public static readonly string specularmap_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_SPECULARMAP",
				"uniform sampler2D specularMap;",
				"#endif"
			});
			public static readonly string specularmap_fragment = string.Join(NewLine, new[]
			{
				"float specularStrength;",
				"#ifdef USE_SPECULARMAP",
				"vec4 texelSpecular = texture2D( specularMap, vUv );",
				"specularStrength = texelSpecular.r;",
				"#else",
				"specularStrength = 1.0;",
				"#endif"
			});
			public static readonly string lights_lambert_pars_vertex = string.Join(NewLine, new[]
			{
				"uniform vec3 ambient;",
				"uniform vec3 diffuse;",
				"uniform vec3 emissive;",
				"uniform vec3 ambientLightColor;",
				"#if MAX_DIR_LIGHTS > 0",
				"uniform vec3 directionalLightColor[ MAX_DIR_LIGHTS ];",
				"uniform vec3 directionalLightDirection[ MAX_DIR_LIGHTS ];",
				"#endif",
				"#if MAX_HEMI_LIGHTS > 0",
				"uniform vec3 hemisphereLightSkyColor[ MAX_HEMI_LIGHTS ];",
				"uniform vec3 hemisphereLightGroundColor[ MAX_HEMI_LIGHTS ];",
				"uniform vec3 hemisphereLightDirection[ MAX_HEMI_LIGHTS ];",
				"#endif",
				"#if MAX_POINT_LIGHTS > 0",
				"uniform vec3 pointLightColor[ MAX_POINT_LIGHTS ];",
				"uniform vec3 pointLightPosition[ MAX_POINT_LIGHTS ];",
				"uniform float pointLightDistance[ MAX_POINT_LIGHTS ];",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"uniform vec3 spotLightColor[ MAX_SPOT_LIGHTS ];",
				"uniform vec3 spotLightPosition[ MAX_SPOT_LIGHTS ];",
				"uniform vec3 spotLightDirection[ MAX_SPOT_LIGHTS ];",
				"uniform float spotLightDistance[ MAX_SPOT_LIGHTS ];",
				"uniform float spotLightAngleCos[ MAX_SPOT_LIGHTS ];",
				"uniform float spotLightExponent[ MAX_SPOT_LIGHTS ];",
				"#endif",
				"#ifdef WRAP_AROUND",
				"uniform vec3 wrapRGB;",
				"#endif"
			});
			public static readonly string lights_lambert_vertex = string.Join(NewLine, new[]
			{
				"vLightFront = vec3( 0.0 );",
				"#ifdef float_SIDED",
				"vLightBack = vec3( 0.0 );",
				"#endif",
				"transformedNormal = normalize( transformedNormal );",
				"#if MAX_DIR_LIGHTS > 0",
				"for( int i = 0; i < MAX_DIR_LIGHTS; i++ ) {",
				"vec4 lDirection = viewMatrix * vec4( directionalLightDirection[ i ], 0.0 );",
				"vec3 dirVector = normalize( lDirection.xyz );",
				"float dotProduct = dot( transformedNormal, dirVector );",
				"vec3 directionalLightWeighting = vec3( max( dotProduct, 0.0 ) );",
				"#ifdef float_SIDED",
				"vec3 directionalLightWeightingBack = vec3( max( -dotProduct, 0.0 ) );",
				"#ifdef WRAP_AROUND",
				"vec3 directionalLightWeightingHalfBack = vec3( max( -0.5 * dotProduct + 0.5, 0.0 ) );",
				"#endif",
				"#endif",
				"#ifdef WRAP_AROUND",
				"vec3 directionalLightWeightingHalf = vec3( max( 0.5 * dotProduct + 0.5, 0.0 ) );",
				"directionalLightWeighting = mix( directionalLightWeighting, directionalLightWeightingHalf, wrapRGB );",
				"#ifdef float_SIDED",
				"directionalLightWeightingBack = mix( directionalLightWeightingBack, directionalLightWeightingHalfBack, wrapRGB );",
				"#endif",
				"#endif",
				"vLightFront += directionalLightColor[ i ] * directionalLightWeighting;",
				"#ifdef float_SIDED",
				"vLightBack += directionalLightColor[ i ] * directionalLightWeightingBack;",
				"#endif",
				"}",
				"#endif",
				"#if MAX_POINT_LIGHTS > 0",
				"for( int i = 0; i < MAX_POINT_LIGHTS; i++ ) {",
				"vec4 lPosition = viewMatrix * vec4( pointLightPosition[ i ], 1.0 );",
				"vec3 lVector = lPosition.xyz - mvPosition.xyz;",
				"float lDistance = 1.0;",
				"if ( pointLightDistance[ i ] > 0.0 )",
				"lDistance = 1.0 - min( ( length( lVector ) / pointLightDistance[ i ] ), 1.0 );",
				"lVector = normalize( lVector );",
				"float dotProduct = dot( transformedNormal, lVector );",
				"vec3 pointLightWeighting = vec3( max( dotProduct, 0.0 ) );",
				"#ifdef float_SIDED",
				"vec3 pointLightWeightingBack = vec3( max( -dotProduct, 0.0 ) );",
				"#ifdef WRAP_AROUND",
				"vec3 pointLightWeightingHalfBack = vec3( max( -0.5 * dotProduct + 0.5, 0.0 ) );",
				"#endif",
				"#endif",
				"#ifdef WRAP_AROUND",
				"vec3 pointLightWeightingHalf = vec3( max( 0.5 * dotProduct + 0.5, 0.0 ) );",
				"pointLightWeighting = mix( pointLightWeighting, pointLightWeightingHalf, wrapRGB );",
				"#ifdef float_SIDED",
				"pointLightWeightingBack = mix( pointLightWeightingBack, pointLightWeightingHalfBack, wrapRGB );",
				"#endif",
				"#endif",
				"vLightFront += pointLightColor[ i ] * pointLightWeighting * lDistance;",
				"#ifdef float_SIDED",
				"vLightBack += pointLightColor[ i ] * pointLightWeightingBack * lDistance;",
				"#endif",
				"}",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"for( int i = 0; i < MAX_SPOT_LIGHTS; i++ ) {",
				"vec4 lPosition = viewMatrix * vec4( spotLightPosition[ i ], 1.0 );",
				"vec3 lVector = lPosition.xyz - mvPosition.xyz;",
				"float spotEffect = dot( spotLightDirection[ i ], normalize( spotLightPosition[ i ] - worldPosition.xyz ) );",
				"if ( spotEffect > spotLightAngleCos[ i ] ) {",
				"spotEffect = max( pow( spotEffect, spotLightExponent[ i ] ), 0.0 );",
				"float lDistance = 1.0;",
				"if ( spotLightDistance[ i ] > 0.0 )",
				"lDistance = 1.0 - min( ( length( lVector ) / spotLightDistance[ i ] ), 1.0 );",
				"lVector = normalize( lVector );",
				"float dotProduct = dot( transformedNormal, lVector );",
				"vec3 spotLightWeighting = vec3( max( dotProduct, 0.0 ) );",
				"#ifdef float_SIDED",
				"vec3 spotLightWeightingBack = vec3( max( -dotProduct, 0.0 ) );",
				"#ifdef WRAP_AROUND",
				"vec3 spotLightWeightingHalfBack = vec3( max( -0.5 * dotProduct + 0.5, 0.0 ) );",
				"#endif",
				"#endif",
				"#ifdef WRAP_AROUND",
				"vec3 spotLightWeightingHalf = vec3( max( 0.5 * dotProduct + 0.5, 0.0 ) );",
				"spotLightWeighting = mix( spotLightWeighting, spotLightWeightingHalf, wrapRGB );",
				"#ifdef float_SIDED",
				"spotLightWeightingBack = mix( spotLightWeightingBack, spotLightWeightingHalfBack, wrapRGB );",
				"#endif",
				"#endif",
				"vLightFront += spotLightColor[ i ] * spotLightWeighting * lDistance * spotEffect;",
				"#ifdef float_SIDED",
				"vLightBack += spotLightColor[ i ] * spotLightWeightingBack * lDistance * spotEffect;",
				"#endif",
				"}",
				"}",
				"#endif",
				"#if MAX_HEMI_LIGHTS > 0",
				"for( int i = 0; i < MAX_HEMI_LIGHTS; i++ ) {",
				"vec4 lDirection = viewMatrix * vec4( hemisphereLightDirection[ i ], 0.0 );",
				"vec3 lVector = normalize( lDirection.xyz );",
				"float dotProduct = dot( transformedNormal, lVector );",
				"float hemiDiffuseWeight = 0.5 * dotProduct + 0.5;",
				"float hemiDiffuseWeightBack = -0.5 * dotProduct + 0.5;",
				"vLightFront += mix( hemisphereLightGroundColor[ i ], hemisphereLightSkyColor[ i ], hemiDiffuseWeight );",
				"#ifdef float_SIDED",
				"vLightBack += mix( hemisphereLightGroundColor[ i ], hemisphereLightSkyColor[ i ], hemiDiffuseWeightBack );",
				"#endif",
				"}",
				"#endif",
				"vLightFront = vLightFront * diffuse + ambient * ambientLightColor + emissive;",
				"#ifdef float_SIDED",
				"vLightBack = vLightBack * diffuse + ambient * ambientLightColor + emissive;",
				"#endif"
			});
			public static readonly string lights_phong_pars_vertex = string.Join(NewLine, new[]
			{
				"#ifndef PHONG_PER_PIXEL",
				"#if MAX_POINT_LIGHTS > 0",
				"uniform vec3 pointLightPosition[ MAX_POINT_LIGHTS ];",
				"uniform float pointLightDistance[ MAX_POINT_LIGHTS ];",
				"varying vec4 vPointLight[ MAX_POINT_LIGHTS ];",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"uniform vec3 spotLightPosition[ MAX_SPOT_LIGHTS ];",
				"uniform float spotLightDistance[ MAX_SPOT_LIGHTS ];",
				"varying vec4 vSpotLight[ MAX_SPOT_LIGHTS ];",
				"#endif",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0 || defined( USE_BUMPMAP )",
				"varying vec3 vWorldPosition;",
				"#endif"
			});
			public static readonly string lights_phong_vertex = string.Join(NewLine, new[]
			{
				"#ifndef PHONG_PER_PIXEL",
				"#if MAX_POINT_LIGHTS > 0",
				"for( int i = 0; i < MAX_POINT_LIGHTS; i++ ) {",
				"vec4 lPosition = viewMatrix * vec4( pointLightPosition[ i ], 1.0 );",
				"vec3 lVector = lPosition.xyz - mvPosition.xyz;",
				"float lDistance = 1.0;",
				"if ( pointLightDistance[ i ] > 0.0 )",
				"lDistance = 1.0 - min( ( length( lVector ) / pointLightDistance[ i ] ), 1.0 );",
				"vPointLight[ i ] = vec4( lVector, lDistance );",
				"}",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"for( int i = 0; i < MAX_SPOT_LIGHTS; i++ ) {",
				"vec4 lPosition = viewMatrix * vec4( spotLightPosition[ i ], 1.0 );",
				"vec3 lVector = lPosition.xyz - mvPosition.xyz;",
				"float lDistance = 1.0;",
				"if ( spotLightDistance[ i ] > 0.0 )",
				"lDistance = 1.0 - min( ( length( lVector ) / spotLightDistance[ i ] ), 1.0 );",
				"vSpotLight[ i ] = vec4( lVector, lDistance );",
				"}",
				"#endif",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0 || defined( USE_BUMPMAP )",
				"vWorldPosition = worldPosition.xyz;",
				"#endif"
			});
			public static readonly string lights_phong_pars_fragment = string.Join(NewLine, new[]
			{
				"uniform vec3 ambientLightColor;",
				"#if MAX_DIR_LIGHTS > 0",
				"uniform vec3 directionalLightColor[ MAX_DIR_LIGHTS ];",
				"uniform vec3 directionalLightDirection[ MAX_DIR_LIGHTS ];",
				"#endif",
				"#if MAX_HEMI_LIGHTS > 0",
				"uniform vec3 hemisphereLightSkyColor[ MAX_HEMI_LIGHTS ];",
				"uniform vec3 hemisphereLightGroundColor[ MAX_HEMI_LIGHTS ];",
				"uniform vec3 hemisphereLightDirection[ MAX_HEMI_LIGHTS ];",
				"#endif",
				"#if MAX_POINT_LIGHTS > 0",
				"uniform vec3 pointLightColor[ MAX_POINT_LIGHTS ];",
				"#ifdef PHONG_PER_PIXEL",
				"uniform vec3 pointLightPosition[ MAX_POINT_LIGHTS ];",
				"uniform float pointLightDistance[ MAX_POINT_LIGHTS ];",
				"#else",
				"varying vec4 vPointLight[ MAX_POINT_LIGHTS ];",
				"#endif",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"uniform vec3 spotLightColor[ MAX_SPOT_LIGHTS ];",
				"uniform vec3 spotLightPosition[ MAX_SPOT_LIGHTS ];",
				"uniform vec3 spotLightDirection[ MAX_SPOT_LIGHTS ];",
				"uniform float spotLightAngleCos[ MAX_SPOT_LIGHTS ];",
				"uniform float spotLightExponent[ MAX_SPOT_LIGHTS ];",
				"#ifdef PHONG_PER_PIXEL",
				"uniform float spotLightDistance[ MAX_SPOT_LIGHTS ];",
				"#else",
				"varying vec4 vSpotLight[ MAX_SPOT_LIGHTS ];",
				"#endif",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0 || defined( USE_BUMPMAP )",
				"varying vec3 vWorldPosition;",
				"#endif",
				"#ifdef WRAP_AROUND",
				"uniform vec3 wrapRGB;",
				"#endif",
				"varying vec3 vViewPosition;",
				"varying vec3 vNormal;"
			});
			public static readonly string lights_phong_fragment = string.Join(NewLine, new[]
			{
				"vec3 normal = normalize( vNormal );",
				"vec3 viewPosition = normalize( vViewPosition );",
				"#ifdef float_SIDED",
				"normal = normal * ( -1.0 + 2.0 * float( gl_FrontFacing ) );",
				"#endif",
				"#ifdef USE_NORMALMAP",
				"normal = perturbNormal2Arb( -viewPosition, normal );",
				"#elif defined( USE_BUMPMAP )",
				"normal = perturbNormalArb( -vViewPosition, normal, dHdxy_fwd() );",
				"#endif",
				"#if MAX_POINT_LIGHTS > 0",
				"vec3 pointDiffuse  = vec3( 0.0 );",
				"vec3 pointSpecular = vec3( 0.0 );",
				"for ( int i = 0; i < MAX_POINT_LIGHTS; i++ ) {",
				"#ifdef PHONG_PER_PIXEL",
				"vec4 lPosition = viewMatrix * vec4( pointLightPosition[ i ], 1.0 );",
				"vec3 lVector = lPosition.xyz + vViewPosition.xyz;",
				"float lDistance = 1.0;",
				"if ( pointLightDistance[ i ] > 0.0 )",
				"lDistance = 1.0 - min( ( length( lVector ) / pointLightDistance[ i ] ), 1.0 );",
				"lVector = normalize( lVector );",
				"#else",
				"vec3 lVector = normalize( vPointLight[ i ].xyz );",
				"float lDistance = vPointLight[ i ].w;",
				"#endif",
				"float dotProduct = dot( normal, lVector );",
				"#ifdef WRAP_AROUND",
				"float pointDiffuseWeightFull = max( dotProduct, 0.0 );",
				"float pointDiffuseWeightHalf = max( 0.5 * dotProduct + 0.5, 0.0 );",
				"vec3 pointDiffuseWeight = mix( vec3 ( pointDiffuseWeightFull ), vec3( pointDiffuseWeightHalf ), wrapRGB );",
				"#else",
				"float pointDiffuseWeight = max( dotProduct, 0.0 );",
				"#endif",
				"pointDiffuse  += diffuse * pointLightColor[ i ] * pointDiffuseWeight * lDistance;",
				"vec3 pointHalfVector = normalize( lVector + viewPosition );",
				"float pointDotNormalHalf = max( dot( normal, pointHalfVector ), 0.0 );",
				"float pointSpecularWeight = specularStrength * max( pow( pointDotNormalHalf, shininess ), 0.0 );",
				"#ifdef PHYSICALLY_BASED_SHADING",
				"float specularNormalization = ( shininess + 2.0001 ) / 8.0;",
				"vec3 schlick = specular + vec3( 1.0 - specular ) * pow( 1.0 - dot( lVector, pointHalfVector ), 5.0 );",
				"pointSpecular += schlick * pointLightColor[ i ] * pointSpecularWeight * pointDiffuseWeight * lDistance * specularNormalization;",
				"#else",
				"pointSpecular += specular * pointLightColor[ i ] * pointSpecularWeight * pointDiffuseWeight * lDistance;",
				"#endif",
				"}",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"vec3 spotDiffuse  = vec3( 0.0 );",
				"vec3 spotSpecular = vec3( 0.0 );",
				"for ( int i = 0; i < MAX_SPOT_LIGHTS; i++ ) {",
				"#ifdef PHONG_PER_PIXEL",
				"vec4 lPosition = viewMatrix * vec4( spotLightPosition[ i ], 1.0 );",
				"vec3 lVector = lPosition.xyz + vViewPosition.xyz;",
				"float lDistance = 1.0;",
				"if ( spotLightDistance[ i ] > 0.0 )",
				"lDistance = 1.0 - min( ( length( lVector ) / spotLightDistance[ i ] ), 1.0 );",
				"lVector = normalize( lVector );",
				"#else",
				"vec3 lVector = normalize( vSpotLight[ i ].xyz );",
				"float lDistance = vSpotLight[ i ].w;",
				"#endif",
				"float spotEffect = dot( spotLightDirection[ i ], normalize( spotLightPosition[ i ] - vWorldPosition ) );",
				"if ( spotEffect > spotLightAngleCos[ i ] ) {",
				"spotEffect = max( pow( spotEffect, spotLightExponent[ i ] ), 0.0 );",
				"float dotProduct = dot( normal, lVector );",
				"#ifdef WRAP_AROUND",
				"float spotDiffuseWeightFull = max( dotProduct, 0.0 );",
				"float spotDiffuseWeightHalf = max( 0.5 * dotProduct + 0.5, 0.0 );",
				"vec3 spotDiffuseWeight = mix( vec3 ( spotDiffuseWeightFull ), vec3( spotDiffuseWeightHalf ), wrapRGB );",
				"#else",
				"float spotDiffuseWeight = max( dotProduct, 0.0 );",
				"#endif",
				"spotDiffuse += diffuse * spotLightColor[ i ] * spotDiffuseWeight * lDistance * spotEffect;",
				"vec3 spotHalfVector = normalize( lVector + viewPosition );",
				"float spotDotNormalHalf = max( dot( normal, spotHalfVector ), 0.0 );",
				"float spotSpecularWeight = specularStrength * max( pow( spotDotNormalHalf, shininess ), 0.0 );",
				"#ifdef PHYSICALLY_BASED_SHADING",
				"float specularNormalization = ( shininess + 2.0001 ) / 8.0;",
				"vec3 schlick = specular + vec3( 1.0 - specular ) * pow( 1.0 - dot( lVector, spotHalfVector ), 5.0 );",
				"spotSpecular += schlick * spotLightColor[ i ] * spotSpecularWeight * spotDiffuseWeight * lDistance * specularNormalization * spotEffect;",
				"#else",
				"spotSpecular += specular * spotLightColor[ i ] * spotSpecularWeight * spotDiffuseWeight * lDistance * spotEffect;",
				"#endif",
				"}",
				"}",
				"#endif",
				"#if MAX_DIR_LIGHTS > 0",
				"vec3 dirDiffuse  = vec3( 0.0 );",
				"vec3 dirSpecular = vec3( 0.0 );",
				"for( int i = 0; i < MAX_DIR_LIGHTS; i++ ) {",
				"vec4 lDirection = viewMatrix * vec4( directionalLightDirection[ i ], 0.0 );",
				"vec3 dirVector = normalize( lDirection.xyz );",
				"float dotProduct = dot( normal, dirVector );",
				"#ifdef WRAP_AROUND",
				"float dirDiffuseWeightFull = max( dotProduct, 0.0 );",
				"float dirDiffuseWeightHalf = max( 0.5 * dotProduct + 0.5, 0.0 );",
				"vec3 dirDiffuseWeight = mix( vec3( dirDiffuseWeightFull ), vec3( dirDiffuseWeightHalf ), wrapRGB );",
				"#else",
				"float dirDiffuseWeight = max( dotProduct, 0.0 );",
				"#endif",
				"dirDiffuse  += diffuse * directionalLightColor[ i ] * dirDiffuseWeight;",
				"vec3 dirHalfVector = normalize( dirVector + viewPosition );",
				"float dirDotNormalHalf = max( dot( normal, dirHalfVector ), 0.0 );",
				"float dirSpecularWeight = specularStrength * max( pow( dirDotNormalHalf, shininess ), 0.0 );",
				"#ifdef PHYSICALLY_BASED_SHADING",
				"float specularNormalization = ( shininess + 2.0001 ) / 8.0;",
				"vec3 schlick = specular + vec3( 1.0 - specular ) * pow( 1.0 - dot( dirVector, dirHalfVector ), 5.0 );",
				"dirSpecular += schlick * directionalLightColor[ i ] * dirSpecularWeight * dirDiffuseWeight * specularNormalization;",
				"#else",
				"dirSpecular += specular * directionalLightColor[ i ] * dirSpecularWeight * dirDiffuseWeight;",
				"#endif",
				"}",
				"#endif",
				"#if MAX_HEMI_LIGHTS > 0",
				"vec3 hemiDiffuse  = vec3( 0.0 );",
				"vec3 hemiSpecular = vec3( 0.0 );",
				"for( int i = 0; i < MAX_HEMI_LIGHTS; i++ ) {",
				"vec4 lDirection = viewMatrix * vec4( hemisphereLightDirection[ i ], 0.0 );",
				"vec3 lVector = normalize( lDirection.xyz );",
				"float dotProduct = dot( normal, lVector );",
				"float hemiDiffuseWeight = 0.5 * dotProduct + 0.5;",
				"vec3 hemiColor = mix( hemisphereLightGroundColor[ i ], hemisphereLightSkyColor[ i ], hemiDiffuseWeight );",
				"hemiDiffuse += diffuse * hemiColor;",
				"vec3 hemiHalfVectorSky = normalize( lVector + viewPosition );",
				"float hemiDotNormalHalfSky = 0.5 * dot( normal, hemiHalfVectorSky ) + 0.5;",
				"float hemiSpecularWeightSky = specularStrength * max( pow( hemiDotNormalHalfSky, shininess ), 0.0 );",
				"vec3 lVectorGround = -lVector;",
				"vec3 hemiHalfVectorGround = normalize( lVectorGround + viewPosition );",
				"float hemiDotNormalHalfGround = 0.5 * dot( normal, hemiHalfVectorGround ) + 0.5;",
				"float hemiSpecularWeightGround = specularStrength * max( pow( hemiDotNormalHalfGround, shininess ), 0.0 );",
				"#ifdef PHYSICALLY_BASED_SHADING",
				"float dotProductGround = dot( normal, lVectorGround );",
				"float specularNormalization = ( shininess + 2.0001 ) / 8.0;",
				"vec3 schlickSky = specular + vec3( 1.0 - specular ) * pow( 1.0 - dot( lVector, hemiHalfVectorSky ), 5.0 );",
				"vec3 schlickGround = specular + vec3( 1.0 - specular ) * pow( 1.0 - dot( lVectorGround, hemiHalfVectorGround ), 5.0 );",
				"hemiSpecular += hemiColor * specularNormalization * ( schlickSky * hemiSpecularWeightSky * max( dotProduct, 0.0 ) + schlickGround * hemiSpecularWeightGround * max( dotProductGround, 0.0 ) );",
				"#else",
				"hemiSpecular += specular * hemiColor * ( hemiSpecularWeightSky + hemiSpecularWeightGround ) * hemiDiffuseWeight;",
				"#endif",
				"}",
				"#endif",
				"vec3 totalDiffuse = vec3( 0.0 );",
				"vec3 totalSpecular = vec3( 0.0 );",
				"#if MAX_DIR_LIGHTS > 0",
				"totalDiffuse += dirDiffuse;",
				"totalSpecular += dirSpecular;",
				"#endif",
				"#if MAX_HEMI_LIGHTS > 0",
				"totalDiffuse += hemiDiffuse;",
				"totalSpecular += hemiSpecular;",
				"#endif",
				"#if MAX_POINT_LIGHTS > 0",
				"totalDiffuse += pointDiffuse;",
				"totalSpecular += pointSpecular;",
				"#endif",
				"#if MAX_SPOT_LIGHTS > 0",
				"totalDiffuse += spotDiffuse;",
				"totalSpecular += spotSpecular;",
				"#endif",
				"#ifdef METAL",
				"gl_FragColor.xyz = gl_FragColor.xyz * ( emissive + totalDiffuse + ambientLightColor * ambient + totalSpecular );",
				"#else",
				"gl_FragColor.xyz = gl_FragColor.xyz * ( emissive + totalDiffuse + ambientLightColor * ambient ) + totalSpecular;",
				"#endif"
			});
			public static readonly string color_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_COLOR",
				"varying vec3 vColor;",
				"#endif"
			});
			public static readonly string color_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_COLOR",
				"gl_FragColor = gl_FragColor * vec4( vColor, opacity );",
				"#endif"
			});
			public static readonly string color_pars_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_COLOR",
				"varying vec3 vColor;",
				"#endif"
			});
			public static readonly string color_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_COLOR",
				"#ifdef GAMMA_INPUT",
				"vColor = color * color;",
				"#else",
				"vColor = color;",
				"#endif",
				"#endif"
			});
			public static readonly string skinning_pars_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_SKINNING",
				"#ifdef BONE_TEXTURE",
				"uniform sampler2D boneTexture;",
				"mat4 getBoneMatrix( const in float i ) {",
				"float j = i * 4.0;",
				"float x = mod( j, N_BONE_PIXEL_X );",
				"float y = floor( j / N_BONE_PIXEL_X );",
				"const float dx = 1.0 / N_BONE_PIXEL_X;",
				"const float dy = 1.0 / N_BONE_PIXEL_Y;",
				"y = dy * ( y + 0.5 );",
				"vec4 v1 = texture2D( boneTexture, vec2( dx * ( x + 0.5 ), y ) );",
				"vec4 v2 = texture2D( boneTexture, vec2( dx * ( x + 1.5 ), y ) );",
				"vec4 v3 = texture2D( boneTexture, vec2( dx * ( x + 2.5 ), y ) );",
				"vec4 v4 = texture2D( boneTexture, vec2( dx * ( x + 3.5 ), y ) );",
				"mat4 bone = mat4( v1, v2, v3, v4 );",
				"return bone;",
				"}",
				"#else",
				"uniform mat4 boneGlobalMatrices[ MAX_BONES ];",
				"mat4 getBoneMatrix( const in float i ) {",
				"mat4 bone = boneGlobalMatrices[ int(i) ];",
				"return bone;",
				"}",
				"#endif",
				"#endif"
			});
			public static readonly string skinbase_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_SKINNING",
				"mat4 boneMatX = getBoneMatrix( skinIndex.x );",
				"mat4 boneMatY = getBoneMatrix( skinIndex.y );",
				"#endif"
			});
			public static readonly string skinning_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_SKINNING",
				"#ifdef USE_MORPHTARGETS",
				"vec4 skinVertex = vec4( morphed, 1.0 );",
				"#else",
				"vec4 skinVertex = vec4( position, 1.0 );",
				"#endif",
				"vec4 skinned  = boneMatX * skinVertex * skinWeight.x;",
				"skinned 	  += boneMatY * skinVertex * skinWeight.y;",
				"#endif"
			});
			public static readonly string morphtarget_pars_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_MORPHTARGETS",
				"#ifndef USE_MORPHNORMALS",
				"uniform float morphTargetInfluences[ 8 ];",
				"#else",
				"uniform float morphTargetInfluences[ 4 ];",
				"#endif",
				"#endif"
			});
			public static readonly string morphtarget_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_MORPHTARGETS",
				"vec3 morphed = vec3( 0.0 );",
				"morphed += ( morphTarget0 - position ) * morphTargetInfluences[ 0 ];",
				"morphed += ( morphTarget1 - position ) * morphTargetInfluences[ 1 ];",
				"morphed += ( morphTarget2 - position ) * morphTargetInfluences[ 2 ];",
				"morphed += ( morphTarget3 - position ) * morphTargetInfluences[ 3 ];",
				"#ifndef USE_MORPHNORMALS",
				"morphed += ( morphTarget4 - position ) * morphTargetInfluences[ 4 ];",
				"morphed += ( morphTarget5 - position ) * morphTargetInfluences[ 5 ];",
				"morphed += ( morphTarget6 - position ) * morphTargetInfluences[ 6 ];",
				"morphed += ( morphTarget7 - position ) * morphTargetInfluences[ 7 ];",
				"#endif",
				"morphed += position;",
				"#endif"
			});
			public static readonly string default_vertex = string.Join(NewLine, new[]
			{
				"vec4 mvPosition;",
				"#ifdef USE_SKINNING",
				"mvPosition = modelViewMatrix * skinned;",
				"#endif",
				"#if !defined( USE_SKINNING ) && defined( USE_MORPHTARGETS )",
				"mvPosition = modelViewMatrix * vec4( morphed, 1.0 );",
				"#endif",
				"#if !defined( USE_SKINNING ) && ! defined( USE_MORPHTARGETS )",
				"mvPosition = modelViewMatrix * vec4( position, 1.0 );",
				"#endif",
				"gl_Position = projectionMatrix * mvPosition;"
			});
			public static readonly string morphnormal_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_MORPHNORMALS",
				"vec3 morphedNormal = vec3( 0.0 );",
				"morphedNormal +=  ( morphNormal0 - normal ) * morphTargetInfluences[ 0 ];",
				"morphedNormal +=  ( morphNormal1 - normal ) * morphTargetInfluences[ 1 ];",
				"morphedNormal +=  ( morphNormal2 - normal ) * morphTargetInfluences[ 2 ];",
				"morphedNormal +=  ( morphNormal3 - normal ) * morphTargetInfluences[ 3 ];",
				"morphedNormal += normal;",
				"#endif"
			});
			public static readonly string skinnormal_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_SKINNING",
				"mat4 skinMatrix = skinWeight.x * boneMatX;",
				"skinMatrix 	+= skinWeight.y * boneMatY;",
				"#ifdef USE_MORPHNORMALS",
				"vec4 skinnedNormal = skinMatrix * vec4( morphedNormal, 0.0 );",
				"#else",
				"vec4 skinnedNormal = skinMatrix * vec4( normal, 0.0 );",
				"#endif",
				"#endif"
			});
			public static readonly string defaultnormal_vertex = string.Join(NewLine, new[]
			{
				"vec3 objectNormal;",
				"#ifdef USE_SKINNING",
				"objectNormal = skinnedNormal.xyz;",
				"#endif",
				"#if !defined( USE_SKINNING ) && defined( USE_MORPHNORMALS )",
				"objectNormal = morphedNormal;",
				"#endif",
				"#if !defined( USE_SKINNING ) && ! defined( USE_MORPHNORMALS )",
				"objectNormal = normal;",
				"#endif",
				"#ifdef FLIP_SIDED",
				"objectNormal = -objectNormal;",
				"#endif",
				"vec3 transformedNormal = normalMatrix * objectNormal;"
			});
			public static readonly string shadowmap_pars_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_SHADOWMAP",
				"uniform sampler2D shadowMap[ MAX_SHADOWS ];",
				"uniform vec2 shadowMapSize[ MAX_SHADOWS ];",
				"uniform float shadowDarkness[ MAX_SHADOWS ];",
				"uniform float shadowBias[ MAX_SHADOWS ];",
				"varying vec4 vShadowCoord[ MAX_SHADOWS ];",
				"float unpackDepth( const in vec4 rgba_depth ) {",
				"const vec4 bit_shift = vec4( 1.0 / ( 256.0 * 256.0 * 256.0 ), 1.0 / ( 256.0 * 256.0 ), 1.0 / 256.0, 1.0 );",
				"float depth = dot( rgba_depth, bit_shift );",
				"return depth;",
				"}",
				"#endif"
			});
			public static readonly string shadowmap_fragment = string.Join(NewLine, new[]
			{
				"#ifdef USE_SHADOWMAP",
				"#ifdef SHADOWMAP_DEBUG",
				"vec3 frustumColors[3];",
				"frustumColors[0] = vec3( 1.0, 0.5, 0.0 );",
				"frustumColors[1] = vec3( 0.0, 1.0, 0.8 );",
				"frustumColors[2] = vec3( 0.0, 0.5, 1.0 );",
				"#endif",
				"#ifdef SHADOWMAP_CASCADE",
				"int inFrustumCount = 0;",
				"#endif",
				"float fDepth;",
				"vec3 shadowColor = vec3( 1.0 );",
				"for( int i = 0; i < MAX_SHADOWS; i++ ) {",
				"vec3 shadowCoord = vShadowCoord[ i ].xyz / vShadowCoord[ i ].w;",
				"bvec4 inFrustumVec = bvec4 ( shadowCoord.x >= 0.0, shadowCoord.x <= 1.0, shadowCoord.y >= 0.0, shadowCoord.y <= 1.0 );",
				"bool inFrustum = all( inFrustumVec );",
				"#ifdef SHADOWMAP_CASCADE",
				"inFrustumCount += int( inFrustum );",
				"bvec3 frustumTestVec = bvec3( inFrustum, inFrustumCount == 1, shadowCoord.z <= 1.0 );",
				"#else",
				"bvec2 frustumTestVec = bvec2( inFrustum, shadowCoord.z <= 1.0 );",
				"#endif",
				"bool frustumTest = all( frustumTestVec );",
				"if ( frustumTest ) {",
				"shadowCoord.z += shadowBias[ i ];",
				"#if defined( SHADOWMAP_TYPE_PCF )",
				"float shadow = 0.0;",
				"const float shadowDelta = 1.0 / 9.0;",
				"float xPixelOffset = 1.0 / shadowMapSize[ i ].x;",
				"float yPixelOffset = 1.0 / shadowMapSize[ i ].y;",
				"float dx0 = -1.25 * xPixelOffset;",
				"float dy0 = -1.25 * yPixelOffset;",
				"float dx1 = 1.25 * xPixelOffset;",
				"float dy1 = 1.25 * yPixelOffset;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx0, dy0 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( 0.0, dy0 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx1, dy0 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx0, 0.0 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx1, 0.0 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx0, dy1 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( 0.0, dy1 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"fDepth = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx1, dy1 ) ) );",
				"if ( fDepth < shadowCoord.z ) shadow += shadowDelta;",
				"shadowColor = shadowColor * vec3( ( 1.0 - shadowDarkness[ i ] * shadow ) );",
				"#elif defined( SHADOWMAP_TYPE_PCF_SOFT )",
				"float shadow = 0.0;",
				"float xPixelOffset = 1.0 / shadowMapSize[ i ].x;",
				"float yPixelOffset = 1.0 / shadowMapSize[ i ].y;",
				"float dx0 = -1.0 * xPixelOffset;",
				"float dy0 = -1.0 * yPixelOffset;",
				"float dx1 = 1.0 * xPixelOffset;",
				"float dy1 = 1.0 * yPixelOffset;",
				"mat3 shadowKernel;",
				"mat3 depthKernel;",
				"depthKernel[0][0] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx0, dy0 ) ) );",
				"if ( depthKernel[0][0] < shadowCoord.z ) shadowKernel[0][0] = 0.25;",
				"else shadowKernel[0][0] = 0.0;",
				"depthKernel[0][1] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx0, 0.0 ) ) );",
				"if ( depthKernel[0][1] < shadowCoord.z ) shadowKernel[0][1] = 0.25;",
				"else shadowKernel[0][1] = 0.0;",
				"depthKernel[0][2] = unpackDepth( texture2D( shadowMap[ i], shadowCoord.xy + vec2( dx0, dy1 ) ) );",
				"if ( depthKernel[0][2] < shadowCoord.z ) shadowKernel[0][2] = 0.25;",
				"else shadowKernel[0][2] = 0.0;",
				"depthKernel[1][0] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( 0.0, dy0 ) ) );",
				"if ( depthKernel[1][0] < shadowCoord.z ) shadowKernel[1][0] = 0.25;",
				"else shadowKernel[1][0] = 0.0;",
				"depthKernel[1][1] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy ) );",
				"if ( depthKernel[1][1] < shadowCoord.z ) shadowKernel[1][1] = 0.25;",
				"else shadowKernel[1][1] = 0.0;",
				"depthKernel[1][2] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( 0.0, dy1 ) ) );",
				"if ( depthKernel[1][2] < shadowCoord.z ) shadowKernel[1][2] = 0.25;",
				"else shadowKernel[1][2] = 0.0;",
				"depthKernel[2][0] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx1, dy0 ) ) );",
				"if ( depthKernel[2][0] < shadowCoord.z ) shadowKernel[2][0] = 0.25;",
				"else shadowKernel[2][0] = 0.0;",
				"depthKernel[2][1] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx1, 0.0 ) ) );",
				"if ( depthKernel[2][1] < shadowCoord.z ) shadowKernel[2][1] = 0.25;",
				"else shadowKernel[2][1] = 0.0;",
				"depthKernel[2][2] = unpackDepth( texture2D( shadowMap[ i ], shadowCoord.xy + vec2( dx1, dy1 ) ) );",
				"if ( depthKernel[2][2] < shadowCoord.z ) shadowKernel[2][2] = 0.25;",
				"else shadowKernel[2][2] = 0.0;",
				"vec2 fractionalCoord = 1.0 - fract( shadowCoord.xy * shadowMapSize[i].xy );",
				"shadowKernel[0] = mix( shadowKernel[1], shadowKernel[0], fractionalCoord.x );",
				"shadowKernel[1] = mix( shadowKernel[2], shadowKernel[1], fractionalCoord.x );",
				"vec4 shadowValues;",
				"shadowValues.x = mix( shadowKernel[0][1], shadowKernel[0][0], fractionalCoord.y );",
				"shadowValues.y = mix( shadowKernel[0][2], shadowKernel[0][1], fractionalCoord.y );",
				"shadowValues.z = mix( shadowKernel[1][1], shadowKernel[1][0], fractionalCoord.y );",
				"shadowValues.w = mix( shadowKernel[1][2], shadowKernel[1][1], fractionalCoord.y );",
				"shadow = dot( shadowValues, vec4( 1.0 ) );",
				"shadowColor = shadowColor * vec3( ( 1.0 - shadowDarkness[ i ] * shadow ) );",
				"#else",
				"vec4 rgbaDepth = texture2D( shadowMap[ i ], shadowCoord.xy );",
				"float fDepth = unpackDepth( rgbaDepth );",
				"if ( fDepth < shadowCoord.z )",
				"shadowColor = shadowColor * vec3( 1.0 - shadowDarkness[ i ] );",
				"#endif",
				"}",
				"#ifdef SHADOWMAP_DEBUG",
				"#ifdef SHADOWMAP_CASCADE",
				"if ( inFrustum && inFrustumCount == 1 ) gl_FragColor.xyz *= frustumColors[ i ];",
				"#else",
				"if ( inFrustum ) gl_FragColor.xyz *= frustumColors[ i ];",
				"#endif",
				"#endif",
				"}",
				"#ifdef GAMMA_OUTPUT",
				"shadowColor *= shadowColor;",
				"#endif",
				"gl_FragColor.xyz = gl_FragColor.xyz * shadowColor;",
				"#endif"
			});
			public static readonly string shadowmap_pars_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_SHADOWMAP",
				"varying vec4 vShadowCoord[ MAX_SHADOWS ];",
				"uniform mat4 shadowMatrix[ MAX_SHADOWS ];",
				"#endif"
			});
			public static readonly string shadowmap_vertex = string.Join(NewLine, new[]
			{
				"#ifdef USE_SHADOWMAP",
				"for( int i = 0; i < MAX_SHADOWS; i++ ) {",
				"vShadowCoord[ i ] = shadowMatrix[ i ] * worldPosition;",
				"}",
				"#endif"
			});
			public static readonly string alphatest_fragment = string.Join(NewLine, new[]
			{
				"#ifdef ALPHATEST",
				"if ( gl_FragColor.a < ALPHATEST ) discard;",
				"#endif"
			});
			public static readonly string linear_to_gamma_fragment = string.Join(NewLine, new[]
			{
				"#ifdef GAMMA_OUTPUT",
				"gl_FragColor.xyz = sqrt( gl_FragColor.xyz );",
				"#endif"
			});
		}

		public static readonly dynamic UniformsLib = JSObject.create(new
		{
			common = new
			{
				diffuse = new {type = "c", value = new Color(0xeeeeee)},
				opacity = new {type = "f", value = 1.0},
				map = new { type = "t", value = (dynamic)null },
				offsetRepeat = new {type = "v4", value = new Vector4(0, 0, 1, 1)},
				lightMap = new {type = "t", value = (dynamic)null},
				specularMap = new { type = "t", value = (dynamic)null },
				envMap = new { type = "t", value = (dynamic)null },
				flipEnvMap = new {type = "f", value = -1},
				useRefract = new {type = "i", value = 0},
				reflectivity = new {type = "f", value = 1.0},
				refractionRatio = new {type = "f", value = 0.98},
				combine = new {type = "i", value = 0},
				morphTargetInfluences = new {type = "f", value = 0}
			},
			bump = new
			{
				bumpMap = new { type = "t", value = (dynamic)null },
				bumpScale = new {type = "f", value = 1.0}
			},
			normalmap = new
			{
				normalMap = new { type = "t", value = (dynamic)null },
				normalScale = new {type = "v2", value = new Vector2(1, 1)}
			},
			fog = new
			{
				fogDensity = new {type = "f", value = 0.00025},
				fogNear = new {type = "f", value = 1},
				fogFar = new {type = "f", value = 2000},
				fogColor = new {type = "c", value = new Color(0xffffff)}
			},
			lights = new
			{
				ambientLightColor = new {type = "fv", value = new JSArray()},
				directionalLightDirection = new {type = "fv", value = new JSArray()},
				directionalLightColor = new {type = "fv", value = new JSArray()},
				hemisphereLightDirection = new {type = "fv", value = new JSArray()},
				hemisphereLightSkyColor = new {type = "fv", value = new JSArray()},
				hemisphereLightGroundColor = new {type = "fv", value = new JSArray()},
				pointLightColor = new {type = "fv", value = new JSArray()},
				pointLightPosition = new {type = "fv", value = new JSArray()},
				pointLightDistance = new {type = "fv1", value = new JSArray()},
				spotLightColor = new {type = "fv", value = new JSArray()},
				spotLightPosition = new {type = "fv", value = new JSArray()},
				spotLightDirection = new {type = "fv", value = new JSArray()},
				spotLightDistance = new {type = "fv1", value = new JSArray()},
				spotLightAngleCos = new {type = "fv1", value = new JSArray()},
				spotLightExponent = new {type = "fv1", value = new JSArray()}
			},
			particle = new
			{
				psColor = new {type = "c", value = new Color(0xeeeeee)},
				opacity = new {type = "f", value = 1.0},
				size = new {type = "f", value = 1.0},
				scale = new {type = "f", value = 1.0},
				map = new {type = "t", value = (object)null},
				fogDensity = new {type = "f", value = 0.00025},
				fogNear = new {type = "f", value = 1},
				fogFar = new {type = "f", value = 2000},
				fogColor = new {type = "c", value = new Color(0xffffff)}
			},
			shadowmap = new
			{
				shadowMap = new {type = "tv", value = new JSArray()},
				shadowMapSize = new {type = "v2v", value = new JSArray()},
				shadowBias = new {type = "fv1", value = new JSArray()},
				shadowDarkness = new {type = "fv1", value = new JSArray()},
				shadowMatrix = new {type = "m4v", value = new JSArray()}
			}
		});

		public static class UniformsUtils
		{
			public static dynamic merge(params dynamic[] uniforms)
			{
				int u;
				var merged = new JSObject();

				for (u = 0; u < uniforms.Length; u++)
				{
					var tmp = UniformsUtils.clone(uniforms[u]);

					foreach (var p in tmp)
					{
						merged[p] = tmp[p];
					}
				}

				return merged;
			}

			public static dynamic clone(dynamic uniformsSrc)
			{
				var uniformsDst = new JSObject();

				foreach (var u in uniformsSrc)
				{
					uniformsDst[u] = new JSObject();

					foreach (var p in uniformsSrc[u])
					{
						var parameterSrc = uniformsSrc[u][p];

						if (parameterSrc is Color ||
						    parameterSrc is Vector2 ||
						    parameterSrc is Vector3 ||
						    parameterSrc is Vector4 ||
						    parameterSrc is Matrix4 ||
						    parameterSrc is Texture)
						{
							uniformsDst[u][p] = parameterSrc.clone();
						}
						else if (parameterSrc is JSArray)
						{
							uniformsDst[u][p] = parameterSrc.slice();
						}
						else
						{
							uniformsDst[u][p] = parameterSrc;
						}
					}
				}

				return uniformsDst;
			}
		}

		public static readonly dynamic ShaderLib = JSObject.create(new
		{
			depthRGBA = new
			{
				uniforms = new JSObject(),
				vertexShader = string.Join(NewLine, new[]
				{
					ShaderChunk.morphtarget_pars_vertex,
					ShaderChunk.skinning_pars_vertex,
					"void main() {",
					ShaderChunk.skinbase_vertex,
					ShaderChunk.morphtarget_vertex,
					ShaderChunk.skinning_vertex,
					ShaderChunk.default_vertex,
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"vec4 pack_depth( const in float depth ) {",
					"const vec4 bit_shift = vec4( 256.0 * 256.0 * 256.0, 256.0 * 256.0, 256.0, 1.0 );",
					"const vec4 bit_mask  = vec4( 0.0, 1.0 / 256.0, 1.0 / 256.0, 1.0 / 256.0 );",
					"vec4 res = fract( depth * bit_shift );",
					"res -= res.xxyz * bit_mask;",
					"return res;",
					"}",
					"void main() {",
					"gl_FragData[ 0 ] = pack_depth( gl_FragCoord.z );",
					"}"
				})
			},
			cube = new
			{
				uniforms = new
				{
					tCube = new {type = "t", value = (object)null},
					tFlip = new {type = "f", value = -1}
				},
				vertexShader = string.Join(NewLine, new[]
				{
					"varying vec3 vWorldPosition;",
					"void main() {",
					"vec4 worldPosition = modelMatrix * vec4( position, 1.0 );",
					"vWorldPosition = worldPosition.xyz;",
					"gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform samplerCube tCube;",
					"uniform float tFlip;",
					"varying vec3 vWorldPosition;",
					"void main() {",
					"gl_FragColor = textureCube( tCube, vec3( tFlip * vWorldPosition.x, vWorldPosition.yz ) );",
					"}"
				})
			},
			normalmap = new
			{
				uniforms = UniformsUtils.merge(UniformsLib["fog"], UniformsLib["lights"], UniformsLib["shadowmap"],
				                               JSObject.create(new
				                               {
				                               	enableAO = new {type = "i", value = 0},
				                               	enableDiffuse = new {type = "i", value = 0},
				                               	enableSpecular = new {type = "i", value = 0},
				                               	enableReflection = new {type = "i", value = 0},
				                               	enableDisplacement = new {type = "i", value = 0},
				                               	tDisplacement = new {type = "t", value = (object)null},
				                               	tDiffuse = new {type = "t", value = (object)null},
				                               	tCube = new {type = "t", value = (object)null},
				                               	tNormal = new {type = "t", value = (object)null},
				                               	tSpecular = new {type = "t", value = (object)null},
				                               	tAO = new {type = "t", value = (object)null},
				                               	uNormalScale = new {type = "v2", value = new Vector2(1, 1)},
				                               	uDisplacementBias = new {type = "f", value = 0.0},
				                               	uDisplacementScale = new {type = "f", value = 1.0},
				                               	uDiffuseColor = new {type = "c", value = new Color(0xffffff)},
				                               	uSpecularColor = new {type = "c", value = new Color(0x111111)},
				                               	uAmbientColor = new {type = "c", value = new Color(0xffffff)},
				                               	uShininess = new {type = "f", value = 30},
				                               	uOpacity = new {type = "f", value = 1},
				                               	useRefract = new {type = "i", value = 0},
				                               	uRefractionRatio = new {type = "f", value = 0.98},
				                               	uReflectivity = new {type = "f", value = 0.5},
				                               	uOffset = new {type = "v2", value = new Vector2()},
				                               	uRepeat = new {type = "v2", value = new Vector2(1, 1)},
				                               	wrapRGB = new {type = "v3", value = new Vector3(1, 1, 1)}
				                               })),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform vec3 uAmbientColor;",
					"uniform vec3 uDiffuseColor;",
					"uniform vec3 uSpecularColor;",
					"uniform float uShininess;",
					"uniform float uOpacity;",
					"uniform bool enableDiffuse;",
					"uniform bool enableSpecular;",
					"uniform bool enableAO;",
					"uniform bool enableReflection;",
					"uniform sampler2D tDiffuse;",
					"uniform sampler2D tNormal;",
					"uniform sampler2D tSpecular;",
					"uniform sampler2D tAO;",
					"uniform samplerCube tCube;",
					"uniform vec2 uNormalScale;",
					"uniform bool useRefract;",
					"uniform float uRefractionRatio;",
					"uniform float uReflectivity;",
					"varying vec3 vTangent;",
					"varying vec3 vBinormal;",
					"varying vec3 vNormal;",
					"varying vec2 vUv;",
					"uniform vec3 ambientLightColor;",
					"#if MAX_DIR_LIGHTS > 0",
					"uniform vec3 directionalLightColor[ MAX_DIR_LIGHTS ];",
					"uniform vec3 directionalLightDirection[ MAX_DIR_LIGHTS ];",
					"#endif",
					"#if MAX_HEMI_LIGHTS > 0",
					"uniform vec3 hemisphereLightSkyColor[ MAX_HEMI_LIGHTS ];",
					"uniform vec3 hemisphereLightGroundColor[ MAX_HEMI_LIGHTS ];",
					"uniform vec3 hemisphereLightDirection[ MAX_HEMI_LIGHTS ];",
					"#endif",
					"#if MAX_POINT_LIGHTS > 0",
					"uniform vec3 pointLightColor[ MAX_POINT_LIGHTS ];",
					"uniform vec3 pointLightPosition[ MAX_POINT_LIGHTS ];",
					"uniform float pointLightDistance[ MAX_POINT_LIGHTS ];",
					"#endif",
					"#if MAX_SPOT_LIGHTS > 0",
					"uniform vec3 spotLightColor[ MAX_SPOT_LIGHTS ];",
					"uniform vec3 spotLightPosition[ MAX_SPOT_LIGHTS ];",
					"uniform vec3 spotLightDirection[ MAX_SPOT_LIGHTS ];",
					"uniform float spotLightAngleCos[ MAX_SPOT_LIGHTS ];",
					"uniform float spotLightExponent[ MAX_SPOT_LIGHTS ];",
					"uniform float spotLightDistance[ MAX_SPOT_LIGHTS ];",
					"#endif",
					"#ifdef WRAP_AROUND",
					"uniform vec3 wrapRGB;",
					"#endif",
					"varying vec3 vWorldPosition;",
					"varying vec3 vViewPosition;",
					ShaderChunk.shadowmap_pars_fragment,
					ShaderChunk.fog_pars_fragment,
					"void main() {",
					"gl_FragColor = vec4( vec3( 1.0 ), uOpacity );",
					"vec3 specularTex = vec3( 1.0 );",
					"vec3 normalTex = texture2D( tNormal, vUv ).xyz * 2.0 - 1.0;",
					"normalTex.xy *= uNormalScale;",
					"normalTex = normalize( normalTex );",
					"if( enableDiffuse ) {",
					"#ifdef GAMMA_INPUT",
					"vec4 texelColor = texture2D( tDiffuse, vUv );",
					"texelColor.xyz *= texelColor.xyz;",
					"gl_FragColor = gl_FragColor * texelColor;",
					"#else",
					"gl_FragColor = gl_FragColor * texture2D( tDiffuse, vUv );",
					"#endif",
					"}",
					"if( enableAO ) {",
					"#ifdef GAMMA_INPUT",
					"vec4 aoColor = texture2D( tAO, vUv );",
					"aoColor.xyz *= aoColor.xyz;",
					"gl_FragColor.xyz = gl_FragColor.xyz * aoColor.xyz;",
					"#else",
					"gl_FragColor.xyz = gl_FragColor.xyz * texture2D( tAO, vUv ).xyz;",
					"#endif",
					"}",
					"if( enableSpecular )",
					"specularTex = texture2D( tSpecular, vUv ).xyz;",
					"mat3 tsb = mat3( normalize( vTangent ), normalize( vBinormal ), normalize( vNormal ) );",
					"vec3 finalNormal = tsb * normalTex;",
					"#ifdef FLIP_SIDED",
					"finalNormal = -finalNormal;",
					"#endif",
					"vec3 normal = normalize( finalNormal );",
					"vec3 viewPosition = normalize( vViewPosition );",
					"#if MAX_POINT_LIGHTS > 0",
					"vec3 pointDiffuse = vec3( 0.0 );",
					"vec3 pointSpecular = vec3( 0.0 );",
					"for ( int i = 0; i < MAX_POINT_LIGHTS; i++ ) {",
					"vec4 lPosition = viewMatrix * vec4( pointLightPosition[ i ], 1.0 );",
					"vec3 pointVector = lPosition.xyz + vViewPosition.xyz;",
					"float pointDistance = 1.0;",
					"if ( pointLightDistance[ i ] > 0.0 )",
					"pointDistance = 1.0 - min( ( length( pointVector ) / pointLightDistance[ i ] ), 1.0 );",
					"pointVector = normalize( pointVector );",
					"#ifdef WRAP_AROUND",
					"float pointDiffuseWeightFull = max( dot( normal, pointVector ), 0.0 );",
					"float pointDiffuseWeightHalf = max( 0.5 * dot( normal, pointVector ) + 0.5, 0.0 );",
					"vec3 pointDiffuseWeight = mix( vec3 ( pointDiffuseWeightFull ), vec3( pointDiffuseWeightHalf ), wrapRGB );",
					"#else",
					"float pointDiffuseWeight = max( dot( normal, pointVector ), 0.0 );",
					"#endif",
					"pointDiffuse += pointDistance * pointLightColor[ i ] * uDiffuseColor * pointDiffuseWeight;",
					"vec3 pointHalfVector = normalize( pointVector + viewPosition );",
					"float pointDotNormalHalf = max( dot( normal, pointHalfVector ), 0.0 );",
					"float pointSpecularWeight = specularTex.r * max( pow( pointDotNormalHalf, uShininess ), 0.0 );",
					"#ifdef PHYSICALLY_BASED_SHADING",
					"float specularNormalization = ( uShininess + 2.0001 ) / 8.0;",
					"vec3 schlick = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( pointVector, pointHalfVector ), 5.0 );",
					"pointSpecular += schlick * pointLightColor[ i ] * pointSpecularWeight * pointDiffuseWeight * pointDistance * specularNormalization;",
					"#else",
					"pointSpecular += pointDistance * pointLightColor[ i ] * uSpecularColor * pointSpecularWeight * pointDiffuseWeight;",
					"#endif",
					"}",
					"#endif",
					"#if MAX_SPOT_LIGHTS > 0",
					"vec3 spotDiffuse = vec3( 0.0 );",
					"vec3 spotSpecular = vec3( 0.0 );",
					"for ( int i = 0; i < MAX_SPOT_LIGHTS; i++ ) {",
					"vec4 lPosition = viewMatrix * vec4( spotLightPosition[ i ], 1.0 );",
					"vec3 spotVector = lPosition.xyz + vViewPosition.xyz;",
					"float spotDistance = 1.0;",
					"if ( spotLightDistance[ i ] > 0.0 )",
					"spotDistance = 1.0 - min( ( length( spotVector ) / spotLightDistance[ i ] ), 1.0 );",
					"spotVector = normalize( spotVector );",
					"float spotEffect = dot( spotLightDirection[ i ], normalize( spotLightPosition[ i ] - vWorldPosition ) );",
					"if ( spotEffect > spotLightAngleCos[ i ] ) {",
					"spotEffect = max( pow( spotEffect, spotLightExponent[ i ] ), 0.0 );",
					"#ifdef WRAP_AROUND",
					"float spotDiffuseWeightFull = max( dot( normal, spotVector ), 0.0 );",
					"float spotDiffuseWeightHalf = max( 0.5 * dot( normal, spotVector ) + 0.5, 0.0 );",
					"vec3 spotDiffuseWeight = mix( vec3 ( spotDiffuseWeightFull ), vec3( spotDiffuseWeightHalf ), wrapRGB );",
					"#else",
					"float spotDiffuseWeight = max( dot( normal, spotVector ), 0.0 );",
					"#endif",
					"spotDiffuse += spotDistance * spotLightColor[ i ] * uDiffuseColor * spotDiffuseWeight * spotEffect;",
					"vec3 spotHalfVector = normalize( spotVector + viewPosition );",
					"float spotDotNormalHalf = max( dot( normal, spotHalfVector ), 0.0 );",
					"float spotSpecularWeight = specularTex.r * max( pow( spotDotNormalHalf, uShininess ), 0.0 );",
					"#ifdef PHYSICALLY_BASED_SHADING",
					"float specularNormalization = ( uShininess + 2.0001 ) / 8.0;",
					"vec3 schlick = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( spotVector, spotHalfVector ), 5.0 );",
					"spotSpecular += schlick * spotLightColor[ i ] * spotSpecularWeight * spotDiffuseWeight * spotDistance * specularNormalization * spotEffect;",
					"#else",
					"spotSpecular += spotDistance * spotLightColor[ i ] * uSpecularColor * spotSpecularWeight * spotDiffuseWeight * spotEffect;",
					"#endif",
					"}",
					"}",
					"#endif",
					"#if MAX_DIR_LIGHTS > 0",
					"vec3 dirDiffuse = vec3( 0.0 );",
					"vec3 dirSpecular = vec3( 0.0 );",
					"for( int i = 0; i < MAX_DIR_LIGHTS; i++ ) {",
					"vec4 lDirection = viewMatrix * vec4( directionalLightDirection[ i ], 0.0 );",
					"vec3 dirVector = normalize( lDirection.xyz );",
					"#ifdef WRAP_AROUND",
					"float directionalLightWeightingFull = max( dot( normal, dirVector ), 0.0 );",
					"float directionalLightWeightingHalf = max( 0.5 * dot( normal, dirVector ) + 0.5, 0.0 );",
					"vec3 dirDiffuseWeight = mix( vec3( directionalLightWeightingFull ), vec3( directionalLightWeightingHalf ), wrapRGB );",
					"#else",
					"float dirDiffuseWeight = max( dot( normal, dirVector ), 0.0 );",
					"#endif",
					"dirDiffuse += directionalLightColor[ i ] * uDiffuseColor * dirDiffuseWeight;",
					"vec3 dirHalfVector = normalize( dirVector + viewPosition );",
					"float dirDotNormalHalf = max( dot( normal, dirHalfVector ), 0.0 );",
					"float dirSpecularWeight = specularTex.r * max( pow( dirDotNormalHalf, uShininess ), 0.0 );",
					"#ifdef PHYSICALLY_BASED_SHADING",
					"float specularNormalization = ( uShininess + 2.0001 ) / 8.0;",
					"vec3 schlick = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( dirVector, dirHalfVector ), 5.0 );",
					"dirSpecular += schlick * directionalLightColor[ i ] * dirSpecularWeight * dirDiffuseWeight * specularNormalization;",
					"#else",
					"dirSpecular += directionalLightColor[ i ] * uSpecularColor * dirSpecularWeight * dirDiffuseWeight;",
					"#endif",
					"}",
					"#endif",
					"#if MAX_HEMI_LIGHTS > 0",
					"vec3 hemiDiffuse  = vec3( 0.0 );",
					"vec3 hemiSpecular = vec3( 0.0 );",
					"for( int i = 0; i < MAX_HEMI_LIGHTS; i++ ) {",
					"vec4 lDirection = viewMatrix * vec4( hemisphereLightDirection[ i ], 0.0 );",
					"vec3 lVector = normalize( lDirection.xyz );",
					"float dotProduct = dot( normal, lVector );",
					"float hemiDiffuseWeight = 0.5 * dotProduct + 0.5;",
					"vec3 hemiColor = mix( hemisphereLightGroundColor[ i ], hemisphereLightSkyColor[ i ], hemiDiffuseWeight );",
					"hemiDiffuse += uDiffuseColor * hemiColor;",
					"vec3 hemiHalfVectorSky = normalize( lVector + viewPosition );",
					"float hemiDotNormalHalfSky = 0.5 * dot( normal, hemiHalfVectorSky ) + 0.5;",
					"float hemiSpecularWeightSky = specularTex.r * max( pow( hemiDotNormalHalfSky, uShininess ), 0.0 );",
					"vec3 lVectorGround = -lVector;",
					"vec3 hemiHalfVectorGround = normalize( lVectorGround + viewPosition );",
					"float hemiDotNormalHalfGround = 0.5 * dot( normal, hemiHalfVectorGround ) + 0.5;",
					"float hemiSpecularWeightGround = specularTex.r * max( pow( hemiDotNormalHalfGround, uShininess ), 0.0 );",
					"#ifdef PHYSICALLY_BASED_SHADING",
					"float dotProductGround = dot( normal, lVectorGround );",
					"float specularNormalization = ( uShininess + 2.0001 ) / 8.0;",
					"vec3 schlickSky = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( lVector, hemiHalfVectorSky ), 5.0 );",
					"vec3 schlickGround = uSpecularColor + vec3( 1.0 - uSpecularColor ) * pow( 1.0 - dot( lVectorGround, hemiHalfVectorGround ), 5.0 );",
					"hemiSpecular += hemiColor * specularNormalization * ( schlickSky * hemiSpecularWeightSky * max( dotProduct, 0.0 ) + schlickGround * hemiSpecularWeightGround * max( dotProductGround, 0.0 ) );",
					"#else",
					"hemiSpecular += uSpecularColor * hemiColor * ( hemiSpecularWeightSky + hemiSpecularWeightGround ) * hemiDiffuseWeight;",
					"#endif",
					"}",
					"#endif",
					"vec3 totalDiffuse = vec3( 0.0 );",
					"vec3 totalSpecular = vec3( 0.0 );",
					"#if MAX_DIR_LIGHTS > 0",
					"totalDiffuse += dirDiffuse;",
					"totalSpecular += dirSpecular;",
					"#endif",
					"#if MAX_HEMI_LIGHTS > 0",
					"totalDiffuse += hemiDiffuse;",
					"totalSpecular += hemiSpecular;",
					"#endif",
					"#if MAX_POINT_LIGHTS > 0",
					"totalDiffuse += pointDiffuse;",
					"totalSpecular += pointSpecular;",
					"#endif",
					"#if MAX_SPOT_LIGHTS > 0",
					"totalDiffuse += spotDiffuse;",
					"totalSpecular += spotSpecular;",
					"#endif",
					"#ifdef METAL",
					"gl_FragColor.xyz = gl_FragColor.xyz * ( totalDiffuse + ambientLightColor * uAmbientColor + totalSpecular );",
					"#else",
					"gl_FragColor.xyz = gl_FragColor.xyz * ( totalDiffuse + ambientLightColor * uAmbientColor ) + totalSpecular;",
					"#endif",
					"if ( enableReflection ) {",
					"vec3 vReflect;",
					"vec3 cameraToVertex = normalize( vWorldPosition - cameraPosition );",
					"if ( useRefract ) {",
					"vReflect = refract( cameraToVertex, normal, uRefractionRatio );",
					"} else {",
					"vReflect = reflect( cameraToVertex, normal );",
					"}",
					"vec4 cubeColor = textureCube( tCube, vec3( -vReflect.x, vReflect.yz ) );",
					"#ifdef GAMMA_INPUT",
					"cubeColor.xyz *= cubeColor.xyz;",
					"#endif",
					"gl_FragColor.xyz = mix( gl_FragColor.xyz, cubeColor.xyz, specularTex.r * uReflectivity );",
					"}",
					ShaderChunk.shadowmap_fragment,
					ShaderChunk.linear_to_gamma_fragment,
					ShaderChunk.fog_fragment,
					"}"
				}),
				vertexShader = string.Join(NewLine, new[]
				{
					"attribute vec4 tangent;",
					"uniform vec2 uOffset;",
					"uniform vec2 uRepeat;",
					"uniform bool enableDisplacement;",
					"#ifdef VERTEX_TEXTURES",
					"uniform sampler2D tDisplacement;",
					"uniform float uDisplacementScale;",
					"uniform float uDisplacementBias;",
					"#endif",
					"varying vec3 vTangent;",
					"varying vec3 vBinormal;",
					"varying vec3 vNormal;",
					"varying vec2 vUv;",
					"varying vec3 vWorldPosition;",
					"varying vec3 vViewPosition;",
					ShaderChunk.skinning_pars_vertex,
					ShaderChunk.shadowmap_pars_vertex,
					"void main() {",
					ShaderChunk.skinbase_vertex,
					ShaderChunk.skinnormal_vertex,
					"#ifdef USE_SKINNING",
					"vNormal = normalize( normalMatrix * skinnedNormal.xyz );",
					"vec4 skinnedTangent = skinMatrix * vec4( tangent.xyz, 0.0 );",
					"vTangent = normalize( normalMatrix * skinnedTangent.xyz );",
					"#else",
					"vNormal = normalize( normalMatrix * normal );",
					"vTangent = normalize( normalMatrix * tangent.xyz );",
					"#endif",
					"vBinormal = normalize( cross( vNormal, vTangent ) * tangent.w );",
					"vUv = uv * uRepeat + uOffset;",
					"vec3 displacedPosition;",
					"#ifdef VERTEX_TEXTURES",
					"if ( enableDisplacement ) {",
					"vec3 dv = texture2D( tDisplacement, uv ).xyz;",
					"float df = uDisplacementScale * dv.x + uDisplacementBias;",
					"displacedPosition = position + normalize( normal ) * df;",
					"} else {",
					"#ifdef USE_SKINNING",
					"vec4 skinVertex = vec4( position, 1.0 );",
					"vec4 skinned  = boneMatX * skinVertex * skinWeight.x;",
					"skinned 	  += boneMatY * skinVertex * skinWeight.y;",
					"displacedPosition  = skinned.xyz;",
					"#else",
					"displacedPosition = position;",
					"#endif",
					"}",
					"#else",
					"#ifdef USE_SKINNING",
					"vec4 skinVertex = vec4( position, 1.0 );",
					"vec4 skinned  = boneMatX * skinVertex * skinWeight.x;",
					"skinned 	  += boneMatY * skinVertex * skinWeight.y;",
					"displacedPosition  = skinned.xyz;",
					"#else",
					"displacedPosition = position;",
					"#endif",
					"#endif",
					"vec4 mvPosition = modelViewMatrix * vec4( displacedPosition, 1.0 );",
					"vec4 worldPosition = modelMatrix * vec4( displacedPosition, 1.0 );",
					"gl_Position = projectionMatrix * mvPosition;",
					"vWorldPosition = worldPosition.xyz;",
					"vViewPosition = -mvPosition.xyz;",
					"#ifdef USE_SHADOWMAP",
					"for( int i = 0; i < MAX_SHADOWS; i++ ) {",
					"vShadowCoord[ i ] = shadowMatrix[ i ] * worldPosition;",
					"}",
					"#endif",
					"}"
				})
			},
			normal = new
			{
				uniforms = new {opacity = new {type = "f", value = 1.0}},
				vertexShader = string.Join(NewLine, new[]
				{
					"varying vec3 vNormal;",
					"void main() {",
					"vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );",
					"vNormal = normalize( normalMatrix * normal );",
					"gl_Position = projectionMatrix * mvPosition;",
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform float opacity;",
					"varying vec3 vNormal;",
					"void main() {",
					"gl_FragColor = vec4( 0.5 * normalize( vNormal ) + 0.5, opacity );",
					"}"
				})
			},
			depth = new
			{
				uniforms = new
				{
					mNear = new {type = "f", value = 1.0},
					mFar = new {type = "f", value = 2000.0},
					opacity = new {type = "f", value = 1.0}
				},
				vertexShader = string.Join(NewLine, new[]
				{
					"void main() {",
					"gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );",
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform float mNear;",
					"uniform float mFar;",
					"uniform float opacity;",
					"void main() {",
					"float depth = gl_FragCoord.z / gl_FragCoord.w;",
					"float color = 1.0 - smoothstep( mNear, mFar, depth );",
					"gl_FragColor = vec4( vec3( color ), opacity );",
					"}"
				})
			},
			dashed = new
			{
				uniforms = UniformsUtils.merge(UniformsLib["common"],
				                               UniformsLib["fog"],
				                               JSObject.create(new
				                               {
				                               	scale = new {type = "f", value = 1},
				                               	dashSize = new {type = "f", value = 1},
				                               	totalSize = new {type = "f", value = 2}
				                               })),
				vertexShader = string.Join(NewLine, new[]
				{
					"uniform float scale;",
					"attribute float lineDistance;",
					"varying float vLineDistance;",
					ShaderChunk.color_pars_vertex,
					"void main() {",
					ShaderChunk.color_vertex,
					"vLineDistance = scale * lineDistance;",
					"vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );",
					"gl_Position = projectionMatrix * mvPosition;",
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform vec3 diffuse;",
					"uniform float opacity;",
					"uniform float dashSize;",
					"uniform float totalSize;",
					"varying float vLineDistance;",
					ShaderChunk.color_pars_fragment,
					ShaderChunk.fog_pars_fragment,
					"void main() {",
					"if ( mod( vLineDistance, totalSize ) > dashSize ) {",
					"discard;",
					"}",
					"gl_FragColor = vec4( diffuse, opacity );",
					ShaderChunk.color_fragment,
					ShaderChunk.fog_fragment,
					"}"
				})
			},
			particle_basic = new
			{
				uniforms = UniformsUtils.merge(UniformsLib["particle"], UniformsLib["shadowmap"]),
				vertexShader = string.Join(NewLine, new[]
				{
					"uniform float size;",
					"uniform float scale;",
					ShaderChunk.color_pars_vertex,
					ShaderChunk.shadowmap_pars_vertex,
					"void main() {",
					ShaderChunk.color_vertex,
					"vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );",
					"#ifdef USE_SIZEATTENUATION",
					"gl_PointSize = size * ( scale / length( mvPosition.xyz ) );",
					"#else",
					"gl_PointSize = size;",
					"#endif",
					"gl_Position = projectionMatrix * mvPosition;",
					ShaderChunk.worldpos_vertex,
					ShaderChunk.shadowmap_vertex,
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform vec3 psColor;",
					"uniform float opacity;",
					ShaderChunk.color_pars_fragment,
					ShaderChunk.map_particle_pars_fragment,
					ShaderChunk.fog_pars_fragment,
					ShaderChunk.shadowmap_pars_fragment,
					"void main() {",
					"gl_FragColor = vec4( psColor, opacity );",
					ShaderChunk.map_particle_fragment,
					ShaderChunk.alphatest_fragment,
					ShaderChunk.color_fragment,
					ShaderChunk.shadowmap_fragment,
					ShaderChunk.fog_fragment,
					"}"
				})
			},
			phong = new
			{
				uniforms = UniformsUtils.merge(UniformsLib["common"],
				                               UniformsLib["bump"],
				                               UniformsLib["normalmap"],
				                               UniformsLib["fog"],
				                               UniformsLib["lights"],
				                               UniformsLib["shadowmap"],
				                               JSObject.create(new
				                               {
				                               	ambient = new {type = "c", value = new Color(0xffffff)},
				                               	emissive = new {type = "c", value = new Color(0x000000)},
				                               	specular = new {type = "c", value = new Color(0x111111)},
				                               	shininess = new {type = "f", value = 30},
				                               	wrapRGB = new {type = "v3", value = new Vector3(1, 1, 1)}
				                               })),
				vertexShader = string.Join(NewLine, new[]
				{
					"#define PHONG",
					"varying vec3 vViewPosition;",
					"varying vec3 vNormal;",
					ShaderChunk.map_pars_vertex,
					ShaderChunk.lightmap_pars_vertex,
					ShaderChunk.envmap_pars_vertex,
					ShaderChunk.lights_phong_pars_vertex,
					ShaderChunk.color_pars_vertex,
					ShaderChunk.morphtarget_pars_vertex,
					ShaderChunk.skinning_pars_vertex,
					ShaderChunk.shadowmap_pars_vertex,
					"void main() {",
					ShaderChunk.map_vertex,
					ShaderChunk.lightmap_vertex,
					ShaderChunk.color_vertex,
					ShaderChunk.morphnormal_vertex,
					ShaderChunk.skinbase_vertex,
					ShaderChunk.skinnormal_vertex,
					ShaderChunk.defaultnormal_vertex,
					"vNormal = normalize( transformedNormal );",
					ShaderChunk.morphtarget_vertex,
					ShaderChunk.skinning_vertex,
					ShaderChunk.default_vertex,
					"vViewPosition = -mvPosition.xyz;",
					ShaderChunk.worldpos_vertex,
					ShaderChunk.envmap_vertex,
					ShaderChunk.lights_phong_vertex,
					ShaderChunk.shadowmap_vertex,
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform vec3 diffuse;",
					"uniform float opacity;",
					"uniform vec3 ambient;",
					"uniform vec3 emissive;",
					"uniform vec3 specular;",
					"uniform float shininess;",
					ShaderChunk.color_pars_fragment,
					ShaderChunk.map_pars_fragment,
					ShaderChunk.lightmap_pars_fragment,
					ShaderChunk.envmap_pars_fragment,
					ShaderChunk.fog_pars_fragment,
					ShaderChunk.lights_phong_pars_fragment,
					ShaderChunk.shadowmap_pars_fragment,
					ShaderChunk.bumpmap_pars_fragment,
					ShaderChunk.normalmap_pars_fragment,
					ShaderChunk.specularmap_pars_fragment,
					"void main() {",
					"gl_FragColor = vec4( vec3 ( 1.0 ), opacity );",
					ShaderChunk.map_fragment,
					ShaderChunk.alphatest_fragment,
					ShaderChunk.specularmap_fragment,
					ShaderChunk.lights_phong_fragment,
					ShaderChunk.lightmap_fragment,
					ShaderChunk.color_fragment,
					ShaderChunk.envmap_fragment,
					ShaderChunk.shadowmap_fragment,
					ShaderChunk.linear_to_gamma_fragment,
					ShaderChunk.fog_fragment,
					"}"
				})
			},
			lambert = new
			{
				uniforms = UniformsUtils.merge(UniformsLib["common"],
				                               UniformsLib["fog"],
				                               UniformsLib["lights"],
				                               UniformsLib["shadowmap"],
				                               JSObject.create(new
				                               {
				                               	ambient = new {type = "c", value = new Color(0xffffff)},
				                               	emissive = new {type = "c", value = new Color(0x000000)},
				                               	wrapRGB = new {type = "v3", value = new Vector3(1, 1, 1)}
				                               })),
				vertexShader = string.Join(NewLine, new[]
				{
					"#define LAMBERT",
					"varying vec3 vLightFront;",
					"#ifdef float_SIDED",
					"varying vec3 vLightBack;",
					"#endif",
					ShaderChunk.map_pars_vertex,
					ShaderChunk.lightmap_pars_vertex,
					ShaderChunk.envmap_pars_vertex,
					ShaderChunk.lights_lambert_pars_vertex,
					ShaderChunk.color_pars_vertex,
					ShaderChunk.morphtarget_pars_vertex,
					ShaderChunk.skinning_pars_vertex,
					ShaderChunk.shadowmap_pars_vertex,
					"void main() {",
					ShaderChunk.map_vertex,
					ShaderChunk.lightmap_vertex,
					ShaderChunk.color_vertex,
					ShaderChunk.morphnormal_vertex,
					ShaderChunk.skinbase_vertex,
					ShaderChunk.skinnormal_vertex,
					ShaderChunk.defaultnormal_vertex,
					ShaderChunk.morphtarget_vertex,
					ShaderChunk.skinning_vertex,
					ShaderChunk.default_vertex,
					ShaderChunk.worldpos_vertex,
					ShaderChunk.envmap_vertex,
					ShaderChunk.lights_lambert_vertex,
					ShaderChunk.shadowmap_vertex,
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform float opacity;",
					"varying vec3 vLightFront;",
					"#ifdef float_SIDED",
					"varying vec3 vLightBack;",
					"#endif",
					ShaderChunk.color_pars_fragment,
					ShaderChunk.map_pars_fragment,
					ShaderChunk.lightmap_pars_fragment,
					ShaderChunk.envmap_pars_fragment,
					ShaderChunk.fog_pars_fragment,
					ShaderChunk.shadowmap_pars_fragment,
					ShaderChunk.specularmap_pars_fragment,
					"void main() {",
					"gl_FragColor = vec4( vec3 ( 1.0 ), opacity );",
					ShaderChunk.map_fragment,
					ShaderChunk.alphatest_fragment,
					ShaderChunk.specularmap_fragment,
					"#ifdef float_SIDED",
					"if ( gl_FrontFacing )",
					"gl_FragColor.xyz *= vLightFront;",
					"else",
					"gl_FragColor.xyz *= vLightBack;",
					"#else",
					"gl_FragColor.xyz *= vLightFront;",
					"#endif",
					ShaderChunk.lightmap_fragment,
					ShaderChunk.color_fragment,
					ShaderChunk.envmap_fragment,
					ShaderChunk.shadowmap_fragment,
					ShaderChunk.linear_to_gamma_fragment,
					ShaderChunk.fog_fragment,
					"}"
				})
			},
			basic = new
			{
				uniforms = UniformsUtils.merge(UniformsLib["common"], UniformsLib["fog"], UniformsLib["shadowmap"]),
				vertexShader = string.Join(NewLine, new[]
				{
					ShaderChunk.map_pars_vertex,
					ShaderChunk.lightmap_pars_vertex,
					ShaderChunk.envmap_pars_vertex,
					ShaderChunk.color_pars_vertex,
					ShaderChunk.morphtarget_pars_vertex,
					ShaderChunk.skinning_pars_vertex,
					ShaderChunk.shadowmap_pars_vertex,
					"void main() {",
					ShaderChunk.map_vertex,
					ShaderChunk.lightmap_vertex,
					ShaderChunk.color_vertex,
					ShaderChunk.skinbase_vertex,
					"#ifdef USE_ENVMAP",
					ShaderChunk.morphnormal_vertex,
					ShaderChunk.skinnormal_vertex,
					ShaderChunk.defaultnormal_vertex,
					"#endif",
					ShaderChunk.morphtarget_vertex,
					ShaderChunk.skinning_vertex,
					ShaderChunk.default_vertex,
					ShaderChunk.worldpos_vertex,
					ShaderChunk.envmap_vertex,
					ShaderChunk.shadowmap_vertex,
					"}"
				}),
				fragmentShader = string.Join(NewLine, new[]
				{
					"uniform vec3 diffuse;",
					"uniform float opacity;",
					ShaderChunk.color_pars_fragment,
					ShaderChunk.map_pars_fragment,
					ShaderChunk.lightmap_pars_fragment,
					ShaderChunk.envmap_pars_fragment,
					ShaderChunk.fog_pars_fragment,
					ShaderChunk.shadowmap_pars_fragment,
					ShaderChunk.specularmap_pars_fragment,
					"void main() {",
					"gl_FragColor = vec4( diffuse, opacity );",
					ShaderChunk.map_fragment,
					ShaderChunk.alphatest_fragment,
					ShaderChunk.specularmap_fragment,
					ShaderChunk.lightmap_fragment,
					ShaderChunk.color_fragment,
					ShaderChunk.envmap_fragment,
					ShaderChunk.shadowmap_fragment,
					ShaderChunk.linear_to_gamma_fragment,
					ShaderChunk.fog_fragment,
					"}"
				})
			},
		});
	}
}
