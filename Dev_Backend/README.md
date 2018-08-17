# DotNetCoreWebApi

To avoid "System.IO.FileNotFoundException: Unable to find tests for Api", run "dotnet test" in test folder

export PATH=$PATH:/opt/DotNetCoreSDK


Pour avoir la couverture de code :

    Installer le plugin VSCode LCOV

    Installer coverlet :

        https://github.com/tonerdo/coverlet

        dotnet tool install --global coverlet.console
        dotnet add package coverlet.msbuild

    Lancer les tests avec la generation de couverture

        contraintes pour que coverage gutters fonctionne :
            -le fichier doit s appeler lcov.info
            -le fichier doit être dans le repertoire de la sollution


        dotnet test LaPlay.Tests/ /p:CollectCoverage=true /p:CoverletOutput='../lcov.info' /p:CoverletOutputFormat=lcov



    Afficher le rapport dans vscode

        ctrl + T
        >LCOV Menu
        >Show coverage report
