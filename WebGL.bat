"C:\Program Files\Unity\Hub\Editor\2021.1.2f1\Editor\Unity.exe" -batchmode -nographics -quit -projectPath "..\vrpartygame_Unity" -executedMethod BuildUnityPlayer.PerformBuild

set WGLdata = ".\Build\WebGL\Build\WebGL.data.unityweb
set WGLframework = .\Build\WebGL\Build\WebGL.framework.js.unityweb
set WGLwasm = .\Build\WebGL\Build\WebGL.wasm.unityweb
set WGLDest = ..\..\Web\vrpartygame\assets\new\Build\.
copy WGLdata WGLDest
copy WGLframework WGLDest
copy WGLwasm WGLDest

cd ..\vrpartygame
git add -A
git commit -m "testing WebGL changes auto"
git push

