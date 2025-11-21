@echo off
chcp 65001 > nul
echo ========================================
echo TeamcityAPI 自包含发布脚本
echo ========================================
echo.

:: 设置输出目录
set OUTPUT_DIR=TeamcityAPI\Build\Win64
set PROJECT_PATH=TeamcityAPI\TeamcityAPI.csproj

:: 清理旧的构建文件
if exist "%OUTPUT_DIR%" (
    echo 正在清理旧的构建文件...
    rmdir /s /q "%OUTPUT_DIR%"
)

echo 正在编译项目（自包含模式）...
echo.

:: 执行自包含发布
dotnet publish "%PROJECT_PATH%" ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -o "%OUTPUT_DIR%" ^
    /p:PublishSingleFile=false ^
    /p:PublishTrimmed=false

if %errorlevel% neq 0 (
    echo.
    echo ========================================
    echo 编译失败！错误代码: %errorlevel%
    echo ========================================
    pause
    exit /b %errorlevel%
)

echo.
echo ========================================
echo 编译成功！
echo 输出目录: %OUTPUT_DIR%
echo ========================================
echo.
echo 可执行文件: %OUTPUT_DIR%\TeamcityAPI.exe
echo.

pause

