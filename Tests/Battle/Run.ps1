$ErrorActionPreference = 'Stop'
$repoPath = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
Add-Type -Path @(
    (Join-Path $repoPath 'Assets/Scripts/Manager/BattleProgress.cs'),
    (Join-Path $repoPath 'Assets/Scripts/Enemy/EnemySpawner.cs'),
    (Join-Path $PSScriptRoot 'TestDoubles.cs')
)
[BattleChecks]::Run()
