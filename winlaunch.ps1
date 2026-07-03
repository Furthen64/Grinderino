param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$GameArgs
)

$ProjectPath = Join-Path $PSScriptRoot "Grinderino\Grinderino.csproj"

& dotnet run --project $ProjectPath -c $Configuration -- @GameArgs
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
