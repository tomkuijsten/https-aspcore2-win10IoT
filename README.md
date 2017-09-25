# Host an https webservice using ASP.NET Core 2.0 on a Win10IoT device

## Goal
This repo will explain how to get an ASP.NET Core app up and running on a Windows 10 IoT device.

## Prerequisites
 - Windows IoT device ([get started](https://developer.microsoft.com/en-us/windows/iot/getstarted), I used the Pi 3)
 - Windows 10 IoT v.10.0.15063.0 (didn't test other version, might work though)
 - [dotnet 2.0](https://www.microsoft.com/net/download/core#/sdk)
 - Visual Studio 2017 ([community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15) will do fine)
   - with the ".NET Core cross-platform development" workload

## Summary
1. Create the project
2. Create a certificate
3. Use kestral
4. Publish
5. Prepare device
6. Run


## Create the project
Use visual studio to create an ASP.NET Core project, make sure you select .NET Core and ASP.NET Core 2.0 in the popup. I choose the WebAPI template, but it should work with MVC as well.

![vs new project popup](https://github.com/tomkuijsten/https-aspcore2-win10IoT/raw/master/docs/img/vs-newAspNetProject-popup.PNG)

## Create the certificate
To run a SSL enabled server, we need a certificate. I suppose you don't have one laying around, so let's create one.

Open a powershell console (**must be administrator**) and run:

```ps1
$certData = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname localhost
$pwd = ConvertTo-SecureString -String "p@ssw0rd" -Force -AsPlainText
Export-PfxCertificate -cert "cert:\localMachine\my\$($certData.Thumbprint)" -FilePath c:\temp\testCertificate.pfx -Password $pwd
```
Copy the certificate to your project root and make sure it pops up in your visual studio. If it doesn't:
use `project->Add->Existing item`

Select the certificate in your project, set it's `Copy to Output Directory` to `Copy if newer`. This will make sure it will be copied when published.

## Use kestral
Next, we have to make sure we're using kestral as our webserver and configure it properly. Change the `Program.BuildWebHost` method untill it looks like this:

```cs
public static IWebHost BuildWebHost(string[] args)
{
    return WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseKestrel(options =>
        {
            options.Listen(
                IPAddress.Any,
                5000,
                listenOptions =>
                {
                    listenOptions.UseHttps("testCertificate.pfx", "p@ssw0rd");
                }
            );
        })
        .Build();
}
```

And yes, you should add some namespaces to make the errors go away.

**Warning:** As you can see, I'm using a very easy password and put it in plain text in the code. That's not something you should do in a real life scenario!

## Publish
Open a command prompt with dotnet in the path (you can use the `Developer Command Prompt for Visual Studio 2017` which should be available if you installed VS). Now cd into your solution folder and publish your app with `win-arm` as runtime:

```cmd
dotnet publish -r win-arm
```

## Prepare device

Find the deploy folder (my case `src\https-aspcore2-win10iot\bin\Debug\netcoreapp2.0\win-arm\publish` and copy it to your device. In case of a pi, you can use explorer to browse to `\\yourpihostnameorip\c$`. Make some directory of your liking, and copy your files.

Open a powershell console (again, as administrator) and enter a remote connection to your device. If this is the first time you try to create this remote connection, make sure you follow [this guide](https://docs.microsoft.com/en-us/windows/iot-core/connect-your-device/powershell) first. When done, create the remote connection:

```ps1
Enter-PSSession -ComputerName [yourpcnameorip] -Credential [yourpcnameorip]\Administrator
```

**Optional:** If you want to test your API from another machine in your network, you have to add a firewall exception:

```ps1
netsh advfirewall firewall add rule name="WebAPI test 5000" dir=in action=allow protocol=TCP localport=5000
```

## Run

Now, cd into the folder with you published app and run the .exe. You should see something like this:

![ps output](https://github.com/tomkuijsten/https-aspcore2-win10IoT/raw/master/docs/img/ps-start-webapi.PNG)

... and that's it, happy coding all.
