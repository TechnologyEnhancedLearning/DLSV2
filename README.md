# Installs

- [Visual Studio Professional 2019](https://visualstudio.microsoft.com/downloads/)
    - Make sure you have the [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension
    - [JetBrains Rider](https://www.jetbrains.com/rider/) can work as an alternative to Visual Studio. Follow the setup steps laid out below. In addition, you'll have to run `npm run dev` manually to build the JS and SASS.
- SQL Server 2019
- [SQL Server Management Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
- [Git](https://git-scm.com/)
- [NPM](https://www.npmjs.com/get-npm)
- [SASS](http://www.sass-lang.com/install) for the command line
	- Specifically, follow the "Install Anywhere (Standalone)" guide. Simply download and extract the files somewhere, and point PATH at the dart-sass folder. This should allow you to use the "sass" command.
	- You don't want to install it via NPM, as those are JavaScript versions that perform significantly worse.
	- At time of writing (2022-03-14), we are using [version 1.49.9 of dart-sass](https://github.com/sass/dart-sass/releases/tag/1.49.9) 

# Getting the code

Checkout the `digitallearningsolutions` repository from [GitHub](https://github.com/TechnologyEnhancedLearning/DLSV2):

```bash
git checkout https://github.com/TechnologyEnhancedLearning/DLSV2.git
```

You should now be able to open the solution in Visual Studio 2019 by finding and double-clicking the `DigitalLearningSolutions.sln` file.

# Configuring text editor

To get useful git diffs and make code reviewing easier, it's helpful to have the same line separator and file encoding settings on our text editors.

Line separators should be CRLF.
File encoding should be UTF-8 with BOM.
Git auto-CRLF should be turned off - otherwise Git will convert your local CRLF line endings to LF when you push to the remote repo.

In JetBrains Rider the text editor settings can be set in `Settings > Editor > File Encodings` (UTF-8, with BOM) and `Settings > Editor > Code Style` (CRLF).

To turn off git autoCrlf, run `git config --global core.autocrlf false` in a terminal.

# Setting up the database

Get a database backup `.bak` file from the current system.

## Restore the database from the backup

- Open SQL Server Management Studio and connect to your `localhost` instance
- Right-click *Databases* → *Restore Database…*
- On the *General* tab, select *Device* under source and click the *…* button to the right
- In the backup media window, click *Add* and navigate to and select the `.bak` file
- Click *OK* on the various windows until the restore starts to run

You should now see the `mbdbx101` database in your *Databases* folder on the `localhost` server.

## Apply the migrations by running the code

We need to add the missing tables in the database, using the fluent migrator.
To do this: run the DigitalLearningSolutions.Web project.
This will throw an exception because data is missing from the table, but it applies the migrations needed first.

## Add the framework and self assessment data

We've added data for the Digital Capabilities self assessment to the database. To add this data to the restored and migrated database:
1. Open SQL Server Management Studio
2. Select File -> Open -> File -> Choose AddDigitalCapabilitiesSelfAssessment.sql from the SQLScripts folder in the root of this repo.
3. Add `USE [mbdbx101]` to the top of the script. This will ensure it runs on the mbdbx101 database
4. Press the Execute button to run the script.
5. Do the same for the EnrolUserOnSelfAssessment.sql script. This will enrol the test user on the self assessment.
6. Do the same for the [PopulateDigitalCapabilityFCandFCGs.sql](https://github.com/TechnologyEnhancedLearning/DLSV2/blob/master/SQLScripts/PopulateDigitalCapabilityFCandFCGs.sql) script. This will turn the self assessment into a framework.
7. Do the same for the PopulateTestUsers.sql script. This will create test users for the data tests.

## Fix inconsistencies with live

There are a few inconsistencies with the live database, as there were some changes made after the db backup was created. There is a script which will fix this, MakeLocalDatabaseConsistentWithLive. Run this on the mbdbx101 database in the same way as the script to add the self assessment data.

### Inspecting the database

It can be useful to have a look at what's in the database, to test out and plan SQL queries. The easiest way to do this is:

1. Open SQL Server Management Studio
2. Connect to `localhost`
3. Expand databases -> `mbdbx101` in the menu on the left.
4. Expand tables. You can now see all the tables in the database.
5. Right click a table and click "Select top 1000 rows". This should open an editor with an SQL query to get the first 1000 rows in the DB. You should also be able to see the result of running that query below. You can change this SQL query to anything you like and click the "execute" button to run it and update the results.

## Making changes to the database
If you just want to make temporary changes to the database for testing (e.g. adding in some specific data to a table to test something) then you can do that in SQL Management Studio with the SQL scripts as described in the previous section. However if you want to make a permanent change to the database, for example to add a new table, then you need to use a migration.

We're using [fluent migrator](https://fluentmigrator.github.io/articles/intro.html) for our migrations. Our migrations live in DigitalLearningSolutions.Data.Migrations. The migrations are applied by the RegisterMigrationRunner method in MigrationHelperMethods.cs. They should get applied when you run the app and when you run the data unit tests.

Fluent Migrator keeps a record of which migrations have been applied in the db, in `[dbo].[VersionInfo]`. FM will apply any unrecorded migrations even if their date is "before" some already-applied migrations ([source](https://fluentmigrator.github.io/articles/migration-example.html)).

### Add a new migration
Right click on DigitalLearningSolutions.Data.Migrations and select Add -> New item -> C# class. Name it using the convention ID_NAME.cs. Here ID should be the date and time in the format yyyyMMddHHmm for example 202007151810 for 18:10 on 15/07/2020. The NAME should be some descriptive name for what the migration does, e.g. AddCustomerTable. The fluent migrator docs have a good example of what a migration should look like: https://fluentmigrator.github.io/.

Once you've added your migration file you need to make sure it's applied when running the app and when running the data unit tests. Do this by adding the migration's assembly to the ScanIn call in RegisterMigrationRunner in MigrationHelperMethods.cs. If you created your migration in DigitalLearningSolutions.Data.Migrations, you can skip this step as the assembly DigitalLearningSolutions.Data.Migrations is already in the ScanIn procedure. This step should only be necessary if you are adding migrations outside of that project (which you shouldn't be doing). If you do, the ScanIn changes will need to look something like:
```
.ConfigureRunner(rb => rb
  .AddSqlServer2016()
  .WithGlobalConnectionString(connectionString)
  .ScanIn(typeof(AddCustomerTable).Assembly, typeof(AddConnectionsTable).Assembly).For.Migrations()
)
```
The migration should now get applied the next time you run the app or when you run the data unit tests.

### Reversing a migration
If the migration has already been deployed and therefore has run on any other database than your local one, then you should create a new migration to reverse the effects. However if you've just been running it locally then you can:
* If you added the migration's assembly to the 'ScanIn' statement in MigrationHelperMethods.cs, remove it from that `ScanIn` statement
* In Configure in Startup.cs call migrationRunner.MigrateDown(ID) where ID is the id of the migration before the one you want to reverse. Run the app once and then remove this change.
* Delete the migration file.

### Running migrations via CLI
It’s possible to run / reverse migrations using the [dotnet-fm CLI](https://fluentmigrator.github.io/articles/runners/dotnet-fm.html).

#### Installation

Run `dotnet tool install -g --ignore-failed-sources FluentMigrator.DotNet.Cli`.

(Including `--ignore-failed-sources` keeps the command from failing if it doesn’t manage to access some sources you have set up - e.g. private sources that require a login)
Commands

The docs have a good list of things that you can do, but I’ve given a few common commands here for convenience.

#### Listing migrations

`dotnet-fm list migrations -p "<path-to-repo-root>\DigitalLearningSolutions.Data.Migrations\bin\Debug\netcoreapp3.1\DigitalLearningSolutions.Data.Migrations.dll" -c "Data Source=localhost;Initial Catalog=mbdbx101;Integrated Security=True;"`

#### Migrating to a certain version

`dotnet-fm migrate -p SqlServer2016 -a "<path-to-repo-root>\DigitalLearningSolutions.Data.Migrations\bin\Debug\netcoreapp3.1\DigitalLearningSolutions.Data.Migrations.dll" -c "Data Source=localhost;Initial Catalog=mbdbx101;Integrated Security=True;" <down/up> -t <timestamp-of-target-migration>`

E.g. To migrate to `202111231427_IncreaseSelfassessmentSignOffRequestorStatementLength` from a later version, I ran:

`dotnet-fm migrate -p SqlServer2016 -a "C:\work\hee\DLSV2\DigitalLearningSolutions.Data.Migrations\bin\Debug\netcoreapp3.1\DigitalLearningSolutions.Data.Migrations.dll" -c "Data Source=localhost;Initial Catalog=mbdbx101;Integrated Security=True;" down -t 202111231708`

#### Migrating to the latest version

Just leave out the -t option from the script to migrate up to a certain version:

`dotnet-fm migrate -p SqlServer2016 -a "<path-to-repo-root>\DigitalLearningSolutions.Data.Migrations\bin\Debug\netcoreapp3.1\DigitalLearningSolutions.Data.Migrations.dll" -c "Data Source=localhost;Initial Catalog=mbdbx101;Integrated Security=True;" up`

# Setting up the old code

For testing the integration with the old system (for example logout or showing SCORM content) when running locally we assume you have the old code running at https://localhost:44367. To change this change the CurrentSystemBaseUrl setting in appsettings.Development.json.

To allow loading pages from the old code in an iframe (which is necessary for tutorials/assessments) you need to make a small tweak to the old code:
1. Open Web.config in the old code.
2. On line 150 change `<add name="X-Frame-Options" value="ALLOW-FROM https://future.nhs.uk/" />` to `<add name="X-Frame-Options" value="ALLOWALL" />`

# Running the app

The project should now build. Confirm this via *Build* → *Build Solution* (or `CTRL+SHIFT+B`).

You can now run the app by clicking the play button (▶), which should say *IIS Express*.

This should launch the website at: [https://localhost:44363/](https://localhost:44363/)

# Running the app with login from the old system

When running our app on its own we have our own dummy login. This is controlled by a feature flag in FeatureManagement.Login in appsettings.Development.json. When this is true the app will start by going to the Login controller where it will set up the claims for our test user and the redirect to the current courses.

However when our system is deployed to live it will run alongside the old system and the old system will handle login. You can test this locally by:
1. Changing the login feature flag to false
2. Running the old code and logging in
3. Running our app

The app should now get the login from the old system. Whenever you login it sets a cookie which will persist. So whenever you change login (e.g. swapping between using our dummy login and the old system login) you need to clear your cookies.

NB the redirect urls for when a user is not logged in or is not authorized (doesn't have the correct claims in the login cookie) always point to the old system; `{CurrentSystemBaseUrl}/home?action=login&app=lp`. If you get redirected to these urls but you want to use our dummy login then just go to the base url for our system (e.g. https://localhost:44363/ when running locally). This should log you in, as long as the login feature flag is set to true.

# Running the tests
These tests will also be run by the Jenkins job whenever you push.

## Running the web tests
These tests are in the DigitalLearningSolutions.Web.(...)Tests projects. No setup is required to run them and they'll also be run by the jenkins job whenever you push. See the sections below for how to run one test, all tests in a file or all the tests in the project.

## Running the data tests
These tests are in the DigitalLearningSolutions.Data.Tests project. Some setup is required as these tests use a real db instance.

You need to copy the local db you've setup so that you can use the copy for testing, make sure you name the copy `mbdbx101_test`. You can copy the db either by restoring the backup file again but making sure you change the file names, or using the SQL server copy database wizard. See https://stackoverflow.com/questions/3829271/how-can-i-clone-an-sql-server-database-on-the-same-server-in-sql-server-2008-exp for details.
Make sure you've applied the migrations, added the self assessment data to the test database as well and enrolled the test user on the self assessment, using the same process as for the main database if you build it from the same backup file.

See the sections below for how to run one test, all tests in a file or all the tests in the project.

## Run one test
Open the test file, find the test you want to run, click the icon to the left of the test name.

## Run all tests in a file
Open the file and click the icon to the left of the class name.

## Run all the tests
Open the solution explorer. Right click the test project you want (DigitalLearningSolutions.Web.Tests, DigitalLearningSolutions.Data.Tests, etc.) and select "Run tests".

## Typescript tests
The typescrpt tests are run using Jasmine, and can be found in `DigitalLearningSolutions.Web/Scripts/spec`. The tests can be run using the Task Runner Explorer, or from the terminal using `npm t` inside DigitalLearningSolutions.Web.

## Typescript linting
The typescript is linted with eslint. In Visual Studio, go to `Tools>Options>Text Editor>Javascript/Typescript>Linting>General` and tick "Enable ESLint".  This should highlight any lint errors in the editor. It's not the most reliable, and if in doubt, run the lint manually.

Linting can be run with `npm run lint` inside `DigitalLearningSolutions.Web`. `npm run lint-fix` may autofix some errors.

# Troubleshooting

## Errors thrown when running scripts on database
The migrations may not have run properly. Occasionally you need to run the project twice to get them all to complete. You should end up with around 134 db tables after running the migrations.

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
5. This might cause the build to fail, if tools such as SASS can no longer be found. If this occurs:
    1. Delete the `node_modules` folder in `DigitalLearningSolutions.Web`
    2. Run the install command in the Task Runner Explorer to reinstall the `node_modules`
    3. Run the build command, it should now work as normal

## Random data service tests failing (esp. if using Rider)

The tests may rely on new migrations which haven't been run on the test project.

Running tests from the Data.Tests project should cause any new migrations to be run on the test database,
but sometimes Rider doesn't build referenced projects when you'd expect it to,
so you may need to build Data.Migrations manually in order for new migrations to get picked up.

Build the Data.Migrations project manually and run the failing tests again - they should pass now.

# Logging
We're using [serilog](https://serilog.net/), specifically [serilog for .net core](https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/). This will automatically log:
* Any ASP.NET Core logs with level warning or above
* Any requests (excluding requests to static files)

We can add any additional logs using the `.Log` method.

The log output will go to the console and to a table in the database.

To view the console in Visual Studio select View -> Output and set "Show output from:" to "DigitalLearningSolutions.Web - ASP.NET Core Web Server".

To view the logs in the database connect to the local db in SQL Server Management Studio. The table with the logs is V2LogEvents (the Logs table stores logs for the old system).

## Useful queries

### Get all the logs for today
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE CAST([TimeStamp] as DATE) = CAST(GETDATE() as DATE)
```

### Get all the logs for a session
Look for the entry with message "starting up", they indicate the start of a session. Once you know the id for the line indicating the start of your session you can do, e.g.
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Id] > 10
```
if the id for the start of your session was 11. If there's a session after the one you want to look at you can include:
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Id] > 10 AND [Id] < 16
```
if the id for the start of the next session was 16.

### Get the logs for all requests
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [MessageTemplate] LIKE '%RequestMethod%'
```

### Get all exceptions
```
SELECT [Exception] FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Exception] IS NOT NULL
```
or
```
SELECT * FROM [mbdbx101].[dbo].[V2LogEvents] WHERE [Exception] IS NOT NULL
```
to get the full log. The stack trace for the exception will be logged but isn't very easy to view in the table. I'd recommend copying it and pasting it to a text editor or similar.

# Email
Some features, such as requesting an unlock of a course, require the sending of emails. A test email account, heedlstest@outlook.com has been set up for this purpose.
In order to set up your dev environment to send emails, make the following changes to the Config table of your local database:
- MailServer = smtp.office365.com
- MailFromAddress = heedlstest@outlook.com
- MailUsername = heedlstest@outlook.com
- MailPW = <Secret value, found in Zoho>
- MailPort = 587

The recipient addresses can be set with Centres.NotifyEmail and Candidates.EmailAddress, and a course can be locked using Progress.PLLocked

On test, the centre email is heedlstest@mailinator.com, and the user email is heedlstestuser@mailinator.com. These can be viewed by visiting [mailinator.com](https://www.mailinator.com/) and entering the email address at the top of the page.

# Environment variables
We're using environment variables to set any settings we don't want to have committed to source control (e.g. database connection string on uat as it includes a password). In addition to the default .net core environment variables we're using any variables set with the prefix `DlsRefactor{EnvironmentName}_`. The prefix will be removed when reading the variable, see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#environment-variables for details.

For example, if you want to set the db connection string on uat then set an environment variable called DlsRefactorUAT_ConnectionStrings:DefaultConnection to the connection string you want (`__` can be used instead of `:` in situations where it's not possible to use `:` such as in a jenkinsfile or on a mac). This will override the ConnectionStrings:DefaultConnection set in the appSettings file.

To set an environment variable you can either:
1. When running locally you can specify environment variables in launchSettings.json.
2. For a deployed instance you can set the environment variable in IIS manager on the server the app is deployed to. See https://stackoverflow.com/questions/31049152/publish-to-iis-setting-environment-variable for details. **NB** deploying will remove any environment variables for the site you're deploying to. Therefore to set an environment variable permanently you need to set it for the whole IIS server in IIS manager and lock it.

# GitHub Actions
We have a GitHub Actions workflow set up. See `.github/workflows/continuous-integration-workflow.yml` for the config. This will build and test the code. If it fails it will email the committer. You can also see the build results on any open pull requests or in the actions tab of the repository: https://github.com/TechnologyEnhancedLearning/DLSV2/actions.
