param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$DotnetArgs
)

$ProjectPath = Join-Path $PSScriptRoot "Grinderino\Grinderino.csproj"

& dotnet build $ProjectPath -c $Configuration @DotnetArgs
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
