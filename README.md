# Host HTTPS on a Win10IoT device
Run a ASP.NET Core 2.0 webapplication on a Windows 10 IoT device (like the Pi)

# Goal
This repo will explain how to get an ASP.NET Core app up and running on a Windows 10 IoT device.

# Prerequisites
 - Windows IoT device ([get started](https://developer.microsoft.com/en-us/windows/iot/getstarted), I used the Pi 3)
 - Windows 10 IoT v.10.0.15063.0 (didn't test other version, might work though)
 - [dotnet 2.0](https://www.microsoft.com/net/download/core#/sdk)
 - Visual Studio 2017 ([community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15) will do fine)
   - with the ".NET Core cross-platform development" workload

# Summary
1. Create the project
2. Create a certificate
3. Use kestral
4. Publish
5. Prepare device
6. Run


# Create the project
Use visual studio to create an ASP.NET Core project, make sure you select .NET Core and ASP.NET Core 2.0 in the popup. I choose the WebAPI template, but it should work with MVC as well.

# Create the certificate
To run a SSL enabled server, we need a certificate. I suppose you don't have one laying around, so let's create one.

Open a powershell console (must be administrator) and run:

```ps1
$certData = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname localhost$pwd = ConvertTo-SecureString -String "p@ssw0rd" -Force -AsPlainTextExport-PfxCertificate -cert "cert:\localMachine\my\$($certData.Thumbprint) -FilePath c:\temp\testCertificate.pfx -Password $pwd
```

