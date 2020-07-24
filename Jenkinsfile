properties([[$class: 'GitLabConnectionProperty', gitLabConnection: 'Softwire Gitlab']])

pipeline {
    agent {
        label 'windows'
    }
    environment {
        SqlTestCredentials = credentials('sql-test-credentials')
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
        stage('Build TypeScript') {
            steps {
                gitlabCommitStatus(name: 'Build TypeScript') {
                    dir("DigitalLearningSolutions.Web/") {
                        bat "npm install"
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
