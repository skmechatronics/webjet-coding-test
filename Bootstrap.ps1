# Bootstrap.ps1
function Prompt-WithDefault($promptText, $default) {
    $input = Read-Host "$promptText (default: $default)"
    if ([string]::IsNullOrWhiteSpace($input)) {
        return $default
    }
    return $input
}

function Test-PortInUse {
    param([int]$port)

    $listeners = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue | Where-Object { $_.LocalPort -eq $port }
    return $listeners.Count -gt 0
}

Write-Host "Starting frontend and backend in parallel..."

# Prompt backend environment variables
$defaultCinemaworldApiBaseUrl = "https://webjetapitest.azurewebsites.net/api/cinemaworld/"
$defaultFilmworldApiBaseUrl = "https://webjetapitest.azurewebsites.net/api/filmworld/"

$cinemaworldApiBaseUrl = Prompt-WithDefault 'Enter WebJetConfiguration__CinemaworldApiBaseUrl' $defaultCinemaworldApiBaseUrl
$filmworldApiBaseUrl = Prompt-WithDefault 'Enter WebJetConfiguration__FilmworldApiBaseUrl' $defaultFilmworldApiBaseUrl

Write-Host "Enter WebJetConfiguration__ApiKey (input hidden):"
$apiKey = Read-Host -AsSecureString
$apiKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($apiKey)
)

# Check ports
$portFrontend = 3000
$portBackendHttps = 7090

$portFrontendInUse = Test-PortInUse -port $portFrontend
$portBackendInUse = Test-PortInUse -port $portBackendHttps

# Trust dev certs (run silently)
Write-Host "Ensuring .NET dev HTTPS certificate is trusted..."
dotnet dev-certs https --trust | Out-Null

# Start backend if port free
if ($portBackendInUse) {
    Write-Warning "Port $portBackendHttps (backend HTTPS) is already in use. Skipping backend startup."
    $backendProcess = $null
} else {
    Write-Host "Starting backend (.NET API) with HTTPS on ports 6090 and 7090..."
    $backendProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", `
        "cd WebJet.Entertainment.Api\WebJet.Entertainment.Api; `
        `$Env:WebJetConfiguration__CinemaworldApiBaseUrl='$cinemaworldApiBaseUrl'; `
        `$Env:WebJetConfiguration__FilmworldApiBaseUrl='$filmworldApiBaseUrl'; `
        `$Env:WebJetConfiguration__ApiKey='$apiKeyPlain'; `
        dotnet run --urls 'https://localhost:7090;http://localhost:6090'" -PassThru
}

# Start frontend if port free
if ($portFrontendInUse) {
    Write-Warning "Port $portFrontend (frontend) is already in use. Skipping React app startup."
    $frontendProcess = $null
} else {
    Write-Host "Starting React frontend (npm run dev)..."
    $frontendProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd webjet-entertainment-ui; npm run dev" -PassThru

    # Open browser
    Start-Process "http://localhost:$portFrontend"
}
