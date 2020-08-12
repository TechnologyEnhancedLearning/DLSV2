properties([[$class: 'GitLabConnectionProperty', gitLabConnection: 'Softwire Gitlab']])

pipeline {
    agent {
        label 'windows'
    }
    environment {
        DlsRefactor_ConnectionStrings__DefaultConnection = credentials('ci-db-connection-string')
    }
    stages {
        stage('Checkout') {
            steps {
                gitlabCommitStatus(name: 'Checkout') {
                    checkout scm
                }
            }
        }
        stage('Build') {
            steps {
                gitlabCommitStatus(name: 'Build') {
                    bat "dotnet build DigitalLearningSolutions.sln"
                }
            }
        }
        stage('TS Build') {
            steps {
                gitlabCommitStatus(name: 'Build TypeScript') {
                    dir("DigitalLearningSolutions.Web/") {
                        bat "npm ci"
                        bat "npm run build"
                    }
                }
            }
        }
        stage('Web Tests') {
            steps {
                gitlabCommitStatus(name: 'Web Tests') {
                    bat "dotnet test DigitalLearningSolutions.Web.Tests"
                }
            }
        }
        stage('Data Tests') {
            steps {
                gitlabCommitStatus(name: 'Data Tests') {
                    bat "dotnet test DigitalLearningSolutions.Data.Tests"
                }
            }
        }
        stage('TS Tests') {
            steps {
                gitlabCommitStatus(name: 'TS Tests') {
                    dir ("DigitalLearningSolutions.Web/") {
                        bat "npm test"
                    }
                }
            }
        }
        stage('TS Lint') {
            steps {
                gitlabCommitStatus(name: 'TS Lint') {
                    dir ("DigitalLearningSolutions.Web/") {
                        bat "npm run lint"
                    }
                }
            }
        }
        stage('Deploy') {
            when {
                branch 'master'
            }
            steps {
                gitlabCommitStatus(name: 'Deploy') {
                    withCredentials([string(credentialsId: 'deploy-test-password', variable: 'PASSWORD')]) {
                        bat "dotnet publish DigitalLearningSolutions.Web/DigitalLearningSolutions.Web.csproj /p:PublishProfile=DigitalLearningSolutions.Web/Properties/PublishProfiles/PublishToTest.pubxml /p:Password=$PASSWORD /p:AllowUntrustedCertificate=True"
                    }
                }
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
            channel: "#hee-dls-refactor-notifications",
            token: "$SLACKTOKEN",
            message: "*$message* - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)",
            color: color
    }
}
