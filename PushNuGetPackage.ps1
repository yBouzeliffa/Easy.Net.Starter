# Script PowerShell pour publier automatiquement le dernier package NuGet et incrémenter la version dans le fichier .csproj

# Demander la clé API à l'utilisateur
$apiKey = Read-Host -Prompt "Entrez votre API Key pour NuGet.org"

# Vérifier si une clé API a été fournie
if (-not $apiKey) {
    Write-Host "Erreur : Aucun API Key fourni. Exiting..." -ForegroundColor Red
    pause
    exit
}

# Trouver le fichier .csproj dans le répertoire courant
$csprojFile = Get-ChildItem -Path . -Filter "*.csproj" | Select-Object -First 1
if (-not $csprojFile) {
    Write-Host "Erreur : Aucun fichier .csproj trouvé dans le répertoire." -ForegroundColor Red
    pause
    exit
}

# Lire le contenu du fichier .csproj
[xml]$csprojXml = Get-Content $csprojFile.FullName

# Trouver le noeud <Version> dans le bon <PropertyGroup>
$versionNode = $csprojXml.Project.PropertyGroup.Version
if (-not $versionNode) {
    Write-Host "Erreur : Aucun noeud <Version> trouvé dans le fichier .csproj." -ForegroundColor Red
    pause
    exit
}

$currentVersion = $versionNode
Write-Host "Version actuelle : $currentVersion" -ForegroundColor Yellow

# Demander le type de mise à jour
$updateType = Read-Host "Type de mise à jour (fix, minor, major)"

# Valider le type de mise à jour
if ($updateType -notin @("fix", "minor", "major")) {
    Write-Host "Erreur : Type de mise à jour invalide. Choisissez 'fix', 'minor' ou 'major'." -ForegroundColor Red
    pause
    exit
}

# Incrémenter la version
$versionParts = $currentVersion -split '\.'
if ($versionParts.Length -ne 3) {
    Write-Host "Erreur : La version actuelle n'est pas au format 'major.minor.patch'." -ForegroundColor Red
    pause
    exit
}

switch ($updateType) {
    "fix"   { $versionParts[2] = [int]$versionParts[2] + 1 }  # Patch update
    "minor" { $versionParts[1] = [int]$versionParts[1] + 1; $versionParts[2] = 0 }  # Minor update
    "major" { $versionParts[0] = [int]$versionParts[0] + 1; $versionParts[1] = 0; $versionParts[2] = 0 }  # Major update
}

$newVersion = "$($versionParts[0]).$($versionParts[1]).$($versionParts[2])"
Write-Host "Nouvelle version : $newVersion" -ForegroundColor Green

# Mettre à jour la version dans le fichier .csproj
$csprojXml.Project.PropertyGroup.Version = $newVersion
$csprojXml.Save($csprojFile.FullName)
Write-Host "Fichier .csproj mis à jour avec la nouvelle version." -ForegroundColor Green

# Exécuter la commande dotnet pack
Write-Host "Génération du package NuGet..." -ForegroundColor Yellow
try {
    Invoke-Expression "dotnet pack -c Release"
    Write-Host "Package généré avec succès." -ForegroundColor Green
} catch {
    Write-Host "Erreur lors de la génération du package : $_" -ForegroundColor Red
    pause
    exit
}

# Se déplacer dans le répertoire de sortie
Set-Location -Path "./bin/Release/"

# Trouver le dernier fichier .nupkg (trié par ordre alphabétique inverse)
$latestPackage = Get-ChildItem -Path . -Filter "*.nupkg" | Sort-Object Name -Descending | Select-Object -First 1

if (-not $latestPackage) {
    Write-Host "Erreur : Aucun fichier .nupkg trouvé dans le répertoire." -ForegroundColor Red
    pause
    exit
}

# Afficher le fichier sélectionné
Write-Host "Fichier sélectionné : $($latestPackage.Name)" -ForegroundColor Green

# Construire et exécuter la commande dotnet nuget push
$command = "dotnet nuget push `"$($latestPackage.FullName)`" -k $apiKey -s https://api.nuget.org/v3/index.json"

Write-Host "Exécution de la commande : $command" -ForegroundColor Yellow

# Exécuter la commande
try {
    Invoke-Expression $command
    Write-Host "Le package a été publié avec succès." -ForegroundColor Green
} catch {
    Write-Host "Erreur lors de la publication du package : $_" -ForegroundColor Red
    pause
}


# Ajouter les modifications au commit
Write-Host "Ajout des modifications au commit Git..." -ForegroundColor Yellow
try {
    Invoke-Expression "git add ."
    Write-Host "Modifications ajoutées avec succès." -ForegroundColor Green
} catch {
    Write-Host "Erreur lors de l'ajout des modifications : $_" -ForegroundColor Red
    pause
    exit
}

# Committer les modifications
$commitMessage = "Mise à jour automatique de la version à $newVersion"
Write-Host "Commit des modifications avec le message : $commitMessage" -ForegroundColor Yellow
try {
    Invoke-Expression "git commit -m `"$commitMessage`""
    Write-Host "Modifications committées avec succès." -ForegroundColor Green
} catch {
    Write-Host "Erreur lors du commit : $_" -ForegroundColor Red
    pause
    exit
}

# Pousser les modifications sur la branche courante
Write-Host "Poussée des modifications sur la branche courante..." -ForegroundColor Yellow
try {
    Invoke-Expression "git push"
    Write-Host "Modifications poussées avec succès sur la branche courante." -ForegroundColor Green
    pause
} catch {
    Write-Host "Erreur lors de la poussée des modifications : $_" -ForegroundColor Red
    pause
    exit
}
