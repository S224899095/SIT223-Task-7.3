# Declare variables
$ErrorActionPreference = "Stop"
$timeout = 120
$elapsed = 0
$interval = 5
$url = "https://localhost:7014/health"

# Ignore SSL certificate errors
add-type -TypeDefinition @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public static class SSLValidator {
    public static void OverrideValidation() {
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
    }
}
"@ -Language CSharp

[SSLValidator]::OverrideValidation()

Write-Host "Waiting for $url..."

# Iterate until it timesouts
while ($elapsed -lt $timeout) {
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "App is healthy!"
            exit 0
        }
    } catch {
        Write-Host "Still waiting... ($elapsed seconds elapsed)"
    }

    Start-Sleep -Seconds $interval
    $elapsed += $interval
}

Write-Error "Timeout! $url is still unreachable after $timeout seconds."
exit 1
