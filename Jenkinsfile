pipeline {
    agent {
        label 'windows'
    }
    environment {
        DlsRefactor_ConnectionStrings__UnitTestConnection = credentials('uar-ci-db-connection-string')
        DlsRefactor_LearningHubOpenAPIKey = credentials('ci-learning-hub-open-api-key')
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
        stage('TS Lint') {
            steps {
                dir ("DigitalLearningSolutions.Web/") {
                    bat "npm run lint"
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
        stage('Integration Tests') {
            environment {
                DlsRefactor_ConnectionStrings__DefaultConnection = credentials('uar-ci-db-connection-string')
        }
            steps {
                bat "dotnet test DigitalLearningSolutions.Web.IntegrationTests"
            }
        }
        stage('Automated UI Tests') {
            environment {
                DlsRefactor_ConnectionStrings__DefaultConnection = credentials('uar-ci-db-connection-string')
            }
            steps {
                bat "dotnet test DigitalLearningSolutions.Web.AutomatedUiTests"
            }
        }
        stage('TS Tests') {
            steps {
                dir ("DigitalLearningSolutions.Web/") {
                    bat "npm test"
                }
            }
        }
        stage('Deploy to UAR test') {
            when {
                branch 'uar-test-but-I-will-add-some-guff-here-so-it-does-not-get-deployed-before-everything-is-ready'
            }
            steps {
                withCredentials([string(credentialsId: 'deploy-test-password', variable: 'PASSWORD')]) {
                    bat "dotnet publish DigitalLearningSolutions.Web/DigitalLearningSolutions.Web.csproj /p:PublishProfile=DigitalLearningSolutions.Web/Properties/PublishProfiles/PublishToTest.pubxml /p:Password=$PASSWORD /p:AllowUntrustedCertificate=True"
                }
            }
        }
    }

    post {
        failure {
            sendFailureMessages(":red_circle: Build failed", "danger")
        }
        success {
            sendSlackMessageToTeamChannel(":excellent: Build succeeded", "good")
        }
    }
}

def sendFailureMessages(message, color) {
	sendSlackMessageToTeamChannel(message, color)
	sendSlackDirectMessage(message, color)
}

def sendSlackMessageToTeamChannel(message, color) {
	sendSlackNotificationToChannel("#hee-notifications", message, color)
}

def sendSlackNotificationToChannel(channel, message, color) {
	withCredentials([string(credentialsId: 'slack-token', variable: 'SLACKTOKEN')]) {
        slackSend teamDomain: "softwire",
            channel: channel,
            token: "$SLACKTOKEN",
            message: "*$message* - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)",
            color: color
    }
}

def sendSlackDirectMessage(message, color) {
	def emailAddress = getCommitterEmailAddress()
	def slackUser = getSlackUserByEmailAddress(emailAddress)
	sendSlackNotificationToChannel(slackUser, message, color)
}

def getCommitterEmailAddress() {
	if (steps.isUnix()) {
		return steps.sh (
			script: 'git --no-pager show -s --format=\'%ae\'',
			returnStdout: true
		).trim()
	} else {
		def commitDetails = steps.bat (
			script: 'git --no-pager show --no-patch --format=%%ae',
			returnStdout: true
		)

		return extractEmailAddressFromCommitDetails(commitDetails)
	}
}

def extractEmailAddressFromCommitDetails(commitDetails) {
	def tokens = commitDetails.tokenize('\n')
	return tokens.last().trim()
}

def getSlackUserByEmailAddress(emailAddress) {
	return getSlackUsers()[emailAddress.toLowerCase()] ?: '@SteVes'
}

def getSlackUsers() {
	return [
		'stella.veski@softwire.com':'@SteVes',
		'stellaveski@gmail.com':'@SteVes',
		'alexander.jackson@dorsetsoftware.com':'@AleJac',
		'daniel.bloxham@softwire.com':'@DanBlo',
		'david.may-miller@softwire.com':'@DavMay',
		'jonathan.bloxsom@softwire.com':'@JonBlo',
		'stephen.jackson@softwire.com':'@SteJac',
		'ibrahimmunir14@gmail.com' : '@IbrMun',
		'olivia.zorn@softwire.com':'@OliZor',
		'showkath.marripadu@softwire.com':'@ShoMar',
	]
}
