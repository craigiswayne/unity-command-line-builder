# Command Line Builder for Unity

| Options         | Description                                              | Default                                              | Supported Values                                                                               |
|-----------------|----------------------------------------------------------|------------------------------------------------------|------------------------------------------------------------------------------------------------|
| compressionMode |                                                          | `Disabled`                                           | `brotli` \| `gzip`                                                                             |
| buildTarget     | Platform you want to target                              | `WebGL`                                              | `WebGL` \| `StandaloneWindows64`\|`StandaloneOSX` \| `StandaloneLinux64` \| `Android` \| `iOS` |
| sceneList       | Provide the absolute path to the scene you wish to build | value in `ProjectSettings/EditorBuildSettings.asset` | comma separated values                                                                         |
| quit            |                                                          |                                                      |                                                                                                |
| batchmode       |                                                          |                                                      |                                                                                                |
| nographics      |                                                          |                                                      |                                                                                                |
| projectPath     |                                                          |                                                      |                                                                                                |
| executeMethod   |                                                          |                                                      |                                                                                                |
| outputPath      |                                                          |                                                      |                                                                                                |
| timestamps      |                                                          |                                                      |                                                                                                |
| logFile         |                                                          |                                                      |                                                                                                |
| bundleVersion   |                                                          | will use a timestamp in the format `YYYYMMddhhmm`     |                                                                                                |


## Build from CLI
*if you have your game opened in Unity, you will need to close it before running the script*

```shell
project_directory=$(pwd);
"C:\Program Files\Unity\Hub\Editor\6000.1.14f1\Editor\Unity" \
-quit \
-batchmode \
-nographics \
-projectPath "$project_directory" \
-executeMethod CommandLineBuild.Build \
-outputPath "$project_directory/build" \
-timestamps \
-logFile "$project_directory/build.log"
```

## Installing in Unity

### Method 1: From git
1. Open Package Manager
2. Click `Add package from git URL`
3. Enter `https://github.com/craigiswayne/command-line-builder.git`
4. Click `Add`

### Method 2: From disk
1. clone down project to your device
2. open package manager
3. click `Add package from disk`
4. select the `package.json` file from the directory in step 1.
5. click `Add`

### Notes
* When the build starts, the `build.log` file is created
* When the build starts, the build directory is emptied
* the default scene can be found in `ProjectSettings/EditorBuildSettings.asset`, look for the property `m_Scenes`

