$controllers = Get-ChildItem -Path "Parking-Zone\Controllers" -Filter "*.cs"

foreach ($controller in $controllers) {
    $content = Get-Content $controller.FullName -Raw
    
    # Update namespace
    $content = $content -replace "namespace ParkIRC.Controllers", "namespace Parking_Zone.Controllers"
    $content = $content -replace "namespace ParkIRC.Web.Controllers", "namespace Parking_Zone.Controllers"
    
    # Update using statements
    $content = $content -replace "using ParkIRC.Models", "using Parking_Zone.Models"
    $content = $content -replace "using ParkIRC.Web.Models", "using Parking_Zone.Models"
    $content = $content -replace "using ParkIRC.Services", "using Parking_Zone.Services"
    $content = $content -replace "using ParkIRC.Web.Services", "using Parking_Zone.Services"
    $content = $content -replace "using ParkIRC.ViewModels", "using Parking_Zone.ViewModels"
    $content = $content -replace "using ParkIRC.Web.ViewModels", "using Parking_Zone.ViewModels"
    
    # Update model references
    $content = $content -replace "UserManager<Operator>", "UserManager<AppUser>"
    $content = $content -replace "SignInManager<Operator>", "SignInManager<AppUser>"
    $content = $content -replace "RoleManager<OperatorRole>", "RoleManager<IdentityRole>"
    
    # Save changes
    $content | Set-Content $controller.FullName -NoNewline
    Write-Host "Updated $($controller.Name)"
}
