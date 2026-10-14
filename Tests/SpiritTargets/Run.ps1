$ErrorActionPreference = 'Stop'
$repoPath = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Add-Type -Path @(
    (Join-Path $repoPath 'Assets/Scripts/Spirits/SpiritTargetCoordinator.cs'),
    (Join-Path $PSScriptRoot 'TestDoubles.cs')
)
[SpiritTargetChecks]::Run()
