pipeline {
    agent {
        label 'windows'
    }
    environment {
        DlsRefactor_ConnectionStrings__UnitTestConnection = credentials('ci-db-connection-string')
    }
    parameters {
        booleanParam(name: 'DeployToUAT', defaultValue: false, description: 'Deploy changes to UAT after build? NB will not deploy to test if this is set')
    }
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        stage('Build') {
            steps {
                bat "dotnet build DigitalLearningSolutions.sln"
            }
        }
        stage('TS Build') {
            steps {
                dir("DigitalLearningSolutions.Web/") {
                    bat "npm ci"
                    bat "npm run build"
                }
            }
        }
        stage('Web Tests') {
            steps {
                bat "dotnet test DigitalLearningSolutions.Web.Tests"
            }
        }
        stage('Data Tests') {
            steps {
                bat "dotnet test DigitalLearningSolutions.Data.Tests"
            }
        }
        stage('TS Tests') {
            steps {
                dir ("DigitalLearningSolutions.Web/") {
                    bat "npm test"
                }
            }
        }
        stage('TS Lint') {
            steps {
                dir ("DigitalLearningSolutions.Web/") {
                    bat "npm run lint"
                }
            }
        }
        stage('Deploy to test') {
            when {
                allOf { branch 'learning-menu-master'; not { expression { params.DeployToUAT } } }
            }
            steps {
                withCredentials([string(credentialsId: 'deploy-test-password', variable: 'PASSWORD')]) {
                    bat "dotnet publish DigitalLearningSolutions.Web/DigitalLearningSolutions.Web.csproj /p:PublishProfile=DigitalLearningSolutions.Web/Properties/PublishProfiles/PublishToTest.pubxml /p:Password=$PASSWORD /p:AllowUntrustedCertificate=True"
                }
            }
        }
        stage('Deploy to UAT') {
            when {
                expression { params.DeployToUAT }
            }
            steps {
                withCredentials([string(credentialsId: 'ftp-password', variable: 'PASSWORD')]) {
                    bat "DeployToUAT.bat \"Frida.Tveit:$PASSWORD\" 40.69.40.103"
                }
                slack(":tada: Successfully deployed to UAT", "good")
            }
        }
    }

    post {
        failure {
            slack(":red_circle: Build failed", "danger")
        }
        success {
            slack(":excellent: Build succeeded", "good")
        }
    }
}

def slack(message, color = "") {
    withCredentials([string(credentialsId: 'slack-token', variable: 'SLACKTOKEN')]) {
        slackSend teamDomain: "softwire",
            channel: "#hee-notifications",
            token: "$SLACKTOKEN",
            message: "*$message* - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)",
            color: color
    }
}
