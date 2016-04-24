# What's Shenmue Translation Patch

This is the source code and assets for the What's Shenmue translation patch.

## Requirements

- Visual Studio 2015 (Maybe it works with a previous version?)
- Have 7-zip installed. Version 9.20 is the one that has been used, but it should work with a more recent version.
- Have all the external programs necessary for the patching process.
  - They need to go inside the folder *WSPatcher/WhatsShenmueTranslation/Utils*.
  - Inside that folder there's a text file that specifies the required programs.

## Instructions

To build the patcher, you need to follow these steps:

1. Generate the binary file with the patch contents. This can be done with *WSPatcher/Compress Data.bat*. 7-zip is required.
2. That will generate file called *data* with the contents found in *WSPatcher/WhatsShenmueTranslation*.
3. That file is already included inside the Visual Studio project and its *Build Action* needs to be set as *Embedded Resource*.
4. Just select *Release* configuration and build the project. And that sould do it.

For debugging purposes, you can also have the patch contents in a folder next to the patch and update that folder every time you update the patch, instead of having to recreate the binary *data* file and rebuild the patcher.

1. Set the project configuration as *Debug_ExternalPatch*.
2. Set the *Build Action* of the *data* file to *None*.
3. Build the project.
4. Now, the patcher will read the data from some folders next to the program instead of the embedded data. Just copy the contents inside of *WSPatcher/WhatsShenmueTranslation* next to the program. So you should see *WSPatcher.exe* and the folders *Utils*, *Patch-ES*, *Patch-EN*, etc. in the same folder.
