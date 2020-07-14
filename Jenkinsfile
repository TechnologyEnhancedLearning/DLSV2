pipeline {
	agent {
		label 'windows'
	}
	stages {
		stage('Checkout'){
			steps {
				updateGitlabCommitStatus name: 'build', state: 'pending'
				checkout scm
			}
		}
		stage('Build') {
			steps {
				bat "dotnet build DigitalLearningSolutions.sln"
			}
		}
		stage('Test') {
			steps {
				bat "dotnet test DigitalLearningSolutions.Web.Tests"
			}
		}
	}

	post {
		failure {
			slack(":red_circle: Build failed", "danger")
			updateGitlabCommitStatus name: 'build', state: 'failure'
		}
		success {
			slack(":excellent: Build succeeded", "good")
			updateGitlabCommitStatus name: 'build', state: 'success'
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