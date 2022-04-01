# Unmanaged.Net
![passing](https://img.shields.io/badge/build-passing-brightgreen) 


<img align="left" width="110" height="140" src="https://i.ibb.co/cwNFYJL/icons8-cuttlefish-builds-content-managed-websites-and-mobile-apps-96.png">

## Introduction
Unmanaged.Net allows you to efficiently convert a DLL (.Net) so it can be easily injected into a process, as you would with a native DLL.

In my head the idea of injecting a DLL made in .NET (C# / VB) into a process as you would do with a Native DLL (C++) has always been around.
In fact, I made a universal loader [UdrakoLoader](https://github.com/DestroyerDarkNess/Udrakoloader), But I was not completely satisfied and that's why I created this tool.

 [![Doate Image](https://raw.githubusercontent.com/poucotm/Links/master/image/PayPal/donate-paypal.png)][PM] If you like this project, you can consider making a donation. Help us continue to improve the tool.


## What is Unmanaged.NET?

This tool is the final result of my research. Basically, I took a DLL made in .NET, exported a specific function (Equivalent to C++'s DLLMain() / entrypoint) and then I packed it with a stub made in C, finally I compiled the stub generating the new DLL that was already exported.

After these steps, you can inject your DLL with any injector. 😎

## How to use?

First we need our DLL made in .NET, the code of the DLL that we will use:

```c#
using System;
using System.Windows.Forms;

namespace TestLibrary
{
    public class Test
    {
        public static void DllMain()
        {

            MessageBox.Show("Hello World!");

        }

    }
}
```
```VB
Imports System
Imports System.Windows.Forms

Namespace TestLibrary

    Public Class Test

        Public Shared Sub DllMain()

            MessageBox.Show("Hello World!")

        End Sub

    End Class

End Namespace
```

### Once you have the DLL, follow these steps:​

- Open the tool and select the DLL (The methods will be automatically loaded)
- Select the method that will work as the EntryPoint of our DLL.
- Select the Target Architecture to compile for. Usually it is I386
- Press the "Convet To Unmanaged" Button.
- If everything finished correctly, in the same path of your DLL, there will be another DLL that will have the name of "xxxExported.dll"
- That is your final DLL, Inject it in any process!

<p align="center">
  <img src="https://i.ibb.co/Lzw3tyj/Previewdll.png" />
</p>

### Notes

- Your entrypoint method of your .NET DLL must be public and accessible to everyone.
- This tool is in its first stable version, there may be errors.

![Note1](https://i.ibb.co/8s97wgv/Sin-t-tulo.png)

## Injecting DLL (Final Test)

<p align="center">
  <img src="https://i.ibb.co/QPn8V9R/Test-Previewsa1.png" />
  <img src="https://i.ibb.co/r3npNZp/Test-Preview1.png" />
  <img src="https://i.ibb.co/brw0nCM/Test-Preview2.png" />
</p>

## Contributors
- Destroyer  : Creator      / Discord : Destroyer#8328
- BloodSharp : C/C++ Helper / Discord : BloodSharp#7180
- XPN : https://github.com/xpn 

# Download

Download Pre-Releases made for games: [UnmanagedDotNet](https://github.com/DestroyerDarkNess/Unmanaged.Net/releases/download/v1.0/Unmanaged.Net.7z)


[PM]:https://www.paypal.me/SalvadorKrilewski "PayPal"



