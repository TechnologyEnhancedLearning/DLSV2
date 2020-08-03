# Digital Learning Solutions
## Installs

- [Visual Studio Professional 2019](https://visualstudio.microsoft.com/downloads/)
    - Make sure you have the [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension
- SQL Server 2019
- [SQL Server Management Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
- [Git](https://git-scm.com/)
- [NPM](https://www.npmjs.com/get-npm)

## Getting the code

Checkout the `digitallearningsolutions` repository from [Softwire's GitLab](https://gitlab.softwire.com/softwire/digitallearningsolutions):

```bash
git checkout git@gitlab.softwire.com:softwire/digitallearningsolutions.git
```

You should now be able to open the solution in Visual Studio 2019 by finding and double-clicking the `DigitalLearningSolutions.sln` file.

## Setting up the database

Get a database backup `.bak` file from the current system.

### Restore the database from the backup

- Open SQL Server Management Studio and connect to your `localhost` instance
- Right-click *Databases* → *Restore Database…*
- On the *General* tab, select *Device* under source and click the *…* button to the right
- In the backup media window, click *Add* and navigate to and select the `.bak` file
- Click *OK* on the various windows until the restore starts to run

You should now see the `mbdbx101` database in your *Databases* folder on the `localhost` server.

### Add the self assessment data

We've added data for the Digital Capabilities self assessment to the database. To add this data to the restored database:
1. Open SQL Server Management Studio
2. Select File -> Open -> File -> Choose AddDigitalCapabilitiesSelfAssessment.sql from the root of this repo.
3. Press the Execute button to run the script. 

### Inspecting the database

It can be useful to have a look at what's in the database, to test out and plan SQL queries. The easiest way to do this is:

1. Open SQL Server Management Studio
2. Connect to `localhost`
3. Expand databases -> `mbdbx101` in the menu on the left.
4. Expand tables. You can now see all the tables in the database.
5. Right click a table and click "Select top 1000 rows". This should open an editor with an SQL query to get the first 1000 rows in the DB. You should also be able to see the result of running that query below. You can change this SQL query to anything you like and click the "execute" button to run it and update the results.

### Making changes to the database
If you just want to make temporary changes to the database for testing (e.g. adding in some specific data to a table to test something) then you can do that in SQL Management Studio with the SQL scripts as described in the previous section. However if you want to make a permanent change to the database, for example to add a new table, then you need to use a migration.

We're using [fluent migrator](https://fluentmigrator.github.io/articles/intro.html) for our migrations. Our migrations will live in DigitalLearningSolutions.Data.Migrations but we don't currently have any. The migrations will be applied by the RegisterMigrationRunner method in MigrationHelperMethods.cs. They should get applied when you run the app and when you run the data unit tests.

#### Add a new migration
Right click on DigitalLearningSolutions.Data.Migrations and select Add -> New item -> C# class. Name it using the convention ID_NAME.cs. Here ID should be the date and time in the format yyyyMMddHHmm for example 202007151810 for 18:10 on 15/07/2020. The NAME should be some descriptive name for what the migration does, e.g. AddCustomerTable. The fluent migrator docs have a good example of what a migration should look like: https://fluentmigrator.github.io/.

Once you've added your migration file you need to make sure it's applied when running the app and when running the data unit tests. Do this by adding the migration to the ScanIn call in RegisterMigrationRunner in MigrationHelperMethods.cs, something like:
```
.ConfigureRunner(rb => rb
  .AddSqlServer2016()
  .WithGlobalConnectionString(connectionString)
  .ScanIn(typeof(AddCustomerTable).Assembly, typeof(AddConnectionsTable).Assembly).For.Migrations()
)
```
The migration should now get applied the next time you run the app or when you run the data unit tests.

#### Reversing a migration
If the migration has already been deployed and therefore has run on any other database than your local one, then you should create a new migration to reverse the effects. However if you've just been running it locally then you can:
* Remove it from the `ScanIn` statement in Startup.cs
* In Configure in Startup.cs call migrationRunner.MigrateDown(ID) where ID is the id of the migration before the one you want to reverse. Run the app once and then remove this change.
* Delete the migration file.

## Setting up the old code

For testing the integration with the old system (for example launching a course will redirect to the old system) when running locally we assume you have the old code running at https://localhost:44367. To change this change the CurrentSystemBaseUrl setting in appsettings.Development.json.

## Running the app

The project should now build. Confirm this via *Build* → *Build Solution* (or `CTRL+SHIFT+B`).

You can now run the app by clicking the play button (▶), which should say *IIS Express*.

This should launch the website at: [https://localhost:44363/](https://localhost:44363/)

## Running the app with login from the old system

When running our app on its own we have our own dummy login. This is controlled by a feature flag in FeatureManagement.Login in appsettings.Development.json. When this is true the app will start by going to the Login controller where it will set up the claims for our test user and the redirect to the current courses.

However when our system is deployed to live it will run alongside the old system and the old system will handle login. You can test this locally by:
1. Changing the login feature flag to false
2. Running the old code and logging in
3. Running our app 

The app should now get the login from the old system. Whenever you login it sets a cookie which will persist. So whenever you change login (e.g. swapping between using our dummy login and the old system login) you need to clear your cookies.

## Running the tests
These tests will also be run by the Jenkins job whenever you push.

### Running the web tests
These tests are in the DigitalLearningSolutions.Web.Tests project. No setup is required to run them and they'll also be run by the jenkins job whenever you push. See the sections below for how to run one test, all tests in a file or all the tests in the project.

### Running the data tests
These tests are in the DigitalLearningSolutions.Data.Tests project. Some setup is required as these tests use a real db instance.

You need to copy the local db you've setup so that you can use the copy for testing, make sure you name the copy `mbdbx101_test`. You can copy the db either by restoring the backup file again but making sure you change the file names, or using the SQL server copy database wizard. See https://stackoverflow.com/questions/3829271/how-can-i-clone-an-sql-server-database-on-the-same-server-in-sql-server-2008-exp for details.

See the sections below for how to run one test, all tests in a file or all the tests in the project.

### Run one test
Open the test file, find the test you want to run, click the icon to the left of the test name.

### Run all tests in a file
Open the file and click the icon to the left of the class name.

### Run all the tests
Open the solution explorer. Right click the test project you want (DigitalLearningSolutions.Web.Tests or DigitalLearningSolutions.Data.Tests) and select "Run tests".

### Typescript tests
The typescrpt tests are run using Jasmine, and can be found in `DigitalLearningSolutions.Web/Scripts/spec`. The tests can be run using the Task Runner Explorer, or from the terminal using `npm t` inside DigitalLearningSolutions.Web.

### Typescript linting
The typescript is linted with eslint. In Visual Studio, go to `Tools>Options>Text Editor>Javascript/Typescript>Linting>General` and tick "Enable ESLint".  This should highlight any lint errors in the editor. It's not the most reliable, and if in doubt, run the lint manually.

Linting can be run with `npm lint` inside `DigitalLearningSolutions.Web`. `npm lint-fix` may autofix some errors.

## Troubleshooting

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

## Logging
We're using [serilog](https://serilog.net/), specifically [serilog for .net core](https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/). This will automatically log:
* Any ASP.NET Core logs with level warning or above
* Any requests (excluding requests to static files)

We can add any additional logs using the `.Log` method.

The log output will go to the console and to a table in the database.

To view the console in Visual Studio select View -> Output and set "Show output from:" to "DigitalLearningSolutions.Web - ASP.NET Core Web Server".

To view the logs in the database connect to the local db in SQL Server Management Studio. The table with the logs is V2LogEvents (the Logs table stores logs for the old system).

### Useful queries

#### Get all the logs for today
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE DAY([TimeStamp]) = DAY(GETDATE())
```

#### Get all the logs for a session
Look for the entry with message "starting up", they indicate the start of a session. Once you know the id for the line indicating the start of your session you can do, e.g.
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Id] > 10
```
if the id for the start of your session was 11. If there's a session after the one you want to look at you can include:
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Id] > 10 AND [Id] < 16
```
if the id for the start of the next session was 16.

#### Get the logs for all requests
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [MessageTemplate] LIKE '%RequestMethod%'
```

#### Get all exceptions
```
SELECT [Exception] FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Exception] IS NOT NULL
```
or
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Exception] IS NOT NULL
```
to get the full log. The stack trace for the exception will be logged but isn't very easy to view in the table. I'd recommend copying it and pasting it to a text editor or similar.
