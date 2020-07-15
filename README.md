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

- Open SQL Server Management Studio and connect to your `localhost` instance
- Right-click *Databases* → *Restore Database…*
- On the *General* tab, select *Device* under source and click the *…* button to the right
- In the backup media window, click *Add* and navigate to and select the `.bak` file
- Click *OK* on the various windows until the restore starts to run

You should now see the `mbdbx101` database in your *Databases* folder on the `localhost` server.

## Inspecting the database

It can be useful to have a look at what's in the database, to test out and plan SQL queries. The easiest way to do this is:

1. Open SQL Server Management Studio
2. Connect to `localhost`
3. Expand databases -> `mbdbx101` in the menu on the left.
4. Expand tables. You can now see all the tables in the database.
5. Right click a table and click "Select top 1000 rows". This should open an editor with an SQL query to get the first 1000 rows in the DB. You should also be able to see the result of running that query below. You can change this SQL query to anything you like and click the "execute" button to run it and update the results.

## Making changes to the database
If you just want to make temporary changes to the database for testing (e.g. adding in some specific data to a table to test something) then you can do that in SQL Management Studio with the SQL scripts as described in the previous section. However if you want to make a permanent change to the database, for example to add a new table, then you need to use a migration.

We're using [fluent migrator](https://fluentmigrator.github.io/articles/intro.html) for our migrations. The migrations we currently have are in DigitalLearningSolutions.Data.Migrations, and we apply them in Startup.cs.

### Add a new migration
Right click on DigitalLearningSolutions.Data.Migrations and select Add -> New item -> C# class. Name it using the convention ID_NAME.cs. Here ID should be the date and time in the format yyyyMMddHHmm for example 202007151810 for 18:10 on 15/07/2020. The NAME should be some descriptive name for what the migration does, e.g. AddCustomerTable. Look at the existing migrations or the fluent migrator documentation for an example of what the contents of the migration file should be.

Once you've added your migration file you need to make sure it's applied in Startup.cs. Do this by adding a new statement to the ConfigureRunner line, something like:
```
.ScanIn(typeof(AddCustomerTable).Assembly).For.Migrations()
```
there should be other migrations there that you can copy. The migration should now be applied when you next run the app.

### Reversing a migration
If the migration has already been deployed and therefore has run on any other database than your local one, then you should create a new migration to reverse the effects. However if you've just been running it locally then you can:
* Remove it from the `ScanIn` statements in Startup.cs
* In Configure in Startup.cs call migrationRunner.MigrateDown(ID) where ID is the id of the migration before the one you want to reverse. Run the app once and then remove this change.
* Delete the migration file.

# Running the app

The project should now build. Confirm this via *Build* → *Build Solution* (or `CTRL+SHIFT+B`).

You can now run the app by clicking the play button (▶), which should say *IIS Express*.

This should launch the website at: [https://localhost:44363/](https://localhost:44363/)

# Running the tests
These tests will also be run by the Jenkins job whenever you push.

## Running the web tests
These tests are in the DigitalLearningSolutions.Web.Tests project. No setup is required to run them and they'll also be run by the jenkins job whenever you push. See the sections below for how to run one test, all tests in a file or all the tests in the project.

## Running the data tests
These tests are in the DigitalLearningSolutions.Data.Tests project. Some setup is required as these tests use a real db instance.

You need to copy the local db you've setup so that you can use the copy for testing, make sure you name the copy `mbdbx101_test`. You can copy the db either by restoring the backup file again but making sure you change the file names, or using the SQL server copy database wizard. See https://stackoverflow.com/questions/3829271/how-can-i-clone-an-sql-server-database-on-the-same-server-in-sql-server-2008-exp for details.

See the sections below for how to run one test, all tests in a file or all the tests in the project.

## Run one test
Open the test file, find the test you want to run, click the icon to the left of the test name.

## Run all tests in a file
Open the file and click the icon to the left of the class name.

## Run all the tests
Open the solution explorer. Right click the test project you want (DigitalLearningSolutions.Web.Tests or DigitalLearningSolutions.Data.Tests) and select "Run tests". 

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