$controllersPath = "Parking-Zone/Controllers"
$files = Get-ChildItem -Path $controllersPath -Filter "*.cs" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    # Replace old namespaces with new ones
    $content = $content -replace "using ParkIRC\.Web\.Data;", "using Parking_Zone.Data;"
    $content = $content -replace "using ParkIRC\.Data\.Models;", "using Parking_Zone.Models;"
    $content = $content -replace "using ParkIRC\.Data\.Services;", "using Parking_Zone.Services;"
    $content = $content -replace "using ParkIRC\.Data\.Hub;", "using Parking_Zone.Hubs;"
    $content = $content -replace "using ParkIRC\.Web\.Hubs;", "using Parking_Zone.Hubs;"
    $content = $content -replace "using ParkIRC\.Extensions;", "using Parking_Zone.Extensions;"
    $content = $content -replace "using ParkIRC\.Models;", "using Parking_Zone.Models;"
    $content = $content -replace "using ParkIRC\.Services;", "using Parking_Zone.Services;"
    $content = $content -replace "using ParkIRC\.Data;", "using Parking_Zone.Data;"
    
    # Replace any remaining ParkIRC references
    $content = $content -replace "ParkIRC", "Parking_Zone"
    
    # Save the updated content
    $content | Set-Content $file.FullName -Force
}

Write-Host "Namespace updates completed." 