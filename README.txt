Using External Version Control Systems with Unity

Unity offers an Asset Server add-on product for easy integrated versioning of your projects. If you for some reason are not able use the Unity Asset Server, it is possible to store your project in any other version control system, such as Subversion, Perforce or Bazaar. This requires some initial manual setup of your project.

Before checking your project in, you have to tell Unity to modify the project structure slightly to make it compatible with storing assets in an external version control system. This is done by selecting Edit->Project Settings->Editor in the application menu and enabling External Version Control support by selecting Metafiles in the dropdown for Version Control. This will create a text file for every asset in the Assets directory containing the necessary bookkeeping information required by Unity. The files will have a .meta file extension with the first part being the full file name of the asset it is associated with. Moving and renaming assets within Unity should also update the relevant .meta files. However, if you move or rename assets from an external tool, make sure to syncronize the relevant .meta files as well.

When checking the project into a version control system, you should add the Assets and the ProjectSettings directories to the system. The Library directory should be completely ignored - when using external version control, it's only a local cache of imported assets.

When creating new assets, make sure both the asset itself and the associated .meta file is added to version control.
Example: Creating a new project and importing it to a Subversion repository.

IMPORTANT!!!!:::
Open the checked out project with Unity by launching it while holding down the Option or the left Alt key. Opening the project will recreate the Library directory. 