# Digital Learning Solutions
## Installs

- [Visual Studio Professional 2019](https://visualstudio.microsoft.com/downloads/)
    - Make sure you have the [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension
- [SQL Server 2019](https://www.notion.so/SQL-Server-2019-setup-e6ee124735c445d79010afed3ace3cc0)
- [SQL Server Management Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
- [Git](https://git-scm.com/)

## Getting the code

Checkout the `digitallearningsolutions` repository from [Softwire's GitLab](https://gitlab.softwire.com/softwire/digitallearningsolutions):

```bash
git checkout git@gitlab.softwire.com:softwire/digitallearningsolutions.git
```

You should now be able to open the solution in Visual Studio 2019 by finding and double-clicking the `DigitalLearningSolutions.sln` file.

# Setting up the database

Get a database backup `.bak` file from the current system.

Restore the database from the backup:

- Open SQL Server Managerment Studio and connect to your `localhost` instance
- Right-click *Databases* → *Restore Database…*
- On the *General* tab, select *Device* under source and click the *…* button to the right
- In the backup media window, click *Add* and navigate to and select the `.bak` file
- Click *OK* on the various windows until the restore starts to run

You should now see the `mbdbx101` database in your *Databases* folder on the `localhost` server.

# Running the app

The project should now build. Confirm this via *Build* → *Build Solution* (or `CTRL+SHIFT+B`).

You can now run the app by clicking the play button (▶), which should say *IIS Express*.

This should launch the website at: [https://localhost:44363/](https://localhost:44363/)

# Troubleshooting

## Undeclared variable warning in `index.scss` (DigitalLearningSolutions.Web)

This might be a result of the css directory not being built.

It should have been automatically built by the [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension. Check if you have it installed in Visual Studio.

Otherwise, you can work around it by manually building the css by opening a console in the project directory and running `npm run dev` (for development) or `npm run build`.

## npm error when opening the project in Visual Studio
If you see an error that looks something like:
```bash
npm WARN lifecycle The node binary used for scripts is C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\NodeJs\win-x64\node.exe but npm is using C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\NodeJs\node.exe itself. Use the `--scripts-prepend-node-path` option to include the path for the node binary npm was executed with.
```
This can be fixed by making sure PATH is on the top of the 'External Web Tools' list:
1. Open the options menu in Visual Studio (F4).
2. Search for 'External Web Tools'.
3. In the list select `$(PATH)` and use the up arrow button to move it to the top of the list.
4. Restart Visual Studio or double click the build command in the Task Runner Explorer to rerun the npm build.