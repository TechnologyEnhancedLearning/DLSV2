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