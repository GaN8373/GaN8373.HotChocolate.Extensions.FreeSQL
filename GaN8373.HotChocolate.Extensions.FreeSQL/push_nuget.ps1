# 设置要查找 .nupkg 文件的目录
# 假设 .nupkg 文件位于当前脚本所在目录的 Release\net9.0 文件夹中
# 您可以根据实际情况修改此路径
$packagePath = Join-Path (Split-Path $MyInvocation.MyCommand.Path) "bin\Release"

# 检查包目录是否存在
if (-not (Test-Path $packagePath -PathType Container))
{
    Write-Error "错误: 未找到包目录 '$packagePath'。请检查路径或确保您的项目已成功构建。"
    exit 1
}

# 执行 dotnet pack 命令
# 确保 dotnet CLI 在您的系统 PATH 中
try
{
    # --output 参数指定了包的输出目录，这里我们让它输出到 $packageOutputBaseDir
    # 如果您的项目文件在子文件夹中，可能需要指定项目文件路径，例如: dotnet pack .\src\MyProject\MyProject.csproj --configuration Release --output $packageOutputBaseDir
    dotnet pack --configuration Release
    Write-Host "dotnet pack 命令执行成功。"
}
catch
{
    Write-Error "错误: 执行 dotnet pack 命令时发生错误: $( $_.Exception.Message )"
    Write-Error "请确保 dotnet CLI 已安装且项目可以成功打包。"
    exit 1
}

# 查找最新的 .nupkg 文件
# 我们会寻找 .nupkg 文件，并按创建时间降序排序，取最新的一个
$nupkgFile = Get-ChildItem -Path $packagePath -Filter "*.nupkg" | Sort-Object CreationTime -Descending | Select-Object -First 1

if (-not $nupkgFile)
{
    Write-Error "错误: 在目录 '$packagePath' 中未找到任何 .nupkg 文件。请确保您的项目已成功构建。"
    exit 1
}

$packageFullPath = $nupkgFile.FullName
Write-Host "检测到要推送的NuGet包: $( $packageFullPath )"

# 从环境变量中读取 NuGet API Key
# 建议将您的API密钥存储在名为 NUGET_API_KEY 的环境变量中
$apiKey = $env:NUGET_API_KEY

if ( [string]::IsNullOrWhiteSpace($apiKey))
{
    Write-Error "错误: 未找到 NuGet API Key。请确保您已设置名为 'NUGET_API_KEY' 的环境变量。"
    Write-Host "或者在系统环境变量中添加此变量。"
    exit 1
}

Write-Host "正在使用环境变量中的API密钥推送NuGet包..."

# 执行 nuget push 命令
# 您可能需要确保 nuget.exe 在您的系统 PATH 中，或者提供 nuget.exe 的完整路径
try
{
    # 默认推送到 nuget.org。如果您需要推送到其他源，请添加 -Source 参数
    dotnet nuget push $packageFullPath --api-key $apiKey --source https://api.nuget.org/v3/index.json
    Write-Host "--------------------------------------------------------"
    Write-Host "🎉 恭喜! NuGet包 $( $nupkgFile.Name ) 已成功推送到NuGet服务器。"
    Write-Host "--------------------------------------------------------"
}
catch
{
    Write-Error "推送NuGet包时发生错误: $( $_.Exception.Message )"
    exit 1
}