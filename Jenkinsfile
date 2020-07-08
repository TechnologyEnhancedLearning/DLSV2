pipeline {
	agent {
		label 'windows'
	}
	stages {
		stage('Checkout'){
			steps{
				checkout scm
			}
		}
		stage('Build') {
			steps {
				bat "dotnet msbuild DigitalLearningSolutions.sln"
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