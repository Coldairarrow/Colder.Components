::定义版本
set Version=1.0.2

::删除所有bin与obj下的文件
@echo off
set nowpath=%cd%
cd \
cd %nowpath%
::delete specify file(*.pdb,*.vshost.*)
for /r %nowpath% %%i in (*.pdb,*.vshost.*) do (del %%i && echo delete %%i)
 
::delete specify folder(obj,bin)
for /r %nowpath% %%i in (obj,bin) do (IF EXIST %%i (RD /s /q %%i && echo delete %%i))

echo 清理完成

::构建
dotnet build -c Release
::推送
for /r %nowpath% %%i in (*.nupkg) do (dotnet nuget push %%i --api-key  --source https://api.nuget.org/v3/index.json)

echo 完成
pause