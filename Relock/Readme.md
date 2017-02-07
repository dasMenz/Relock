# Relock
This is a lightwight program to enable relock of drives protected by [BitLocker](https://technet.microsoft.com/de-de/library/dd835565(v=ws.10).aspx) from a context menu item.

## Build
 1. Load the project solution into Visual Studio 
 2. Select your platform architecture for the build process (x64 or x86)
	- this is neccessary to determine the correct system path for "manage-bde.exe"
 3. Move the file to the target destination
 4. Run the program and MessageBox will appear to ask for updating the registry
 5. Accept the update if you have moved the file or started it for the first time
 6. There is a now a context menu item called "Relock this drive" on Bitlocker protected drives

If you have moved the executable, just start the file directly and update the registry with the new path.

## Useage
* just open the context menue on any unlocked drive protected by BitLocker
* start the executable with the drive you want to lock as first parameter, e.g. "relock.exe D:\"

## Additional Information
* tested on Windows 10 with Visual Studio 2015