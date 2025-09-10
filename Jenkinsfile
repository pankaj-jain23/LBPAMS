pipeline {
    agent any
    
    stages {
        stage('Checkout') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/pankaj-jain23/LBPAMS.git',
                    credentialsId: 'ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIJsOkNK+PNfxziz/RVUye7nUxVFNkug8IkMIDRJsUZ+u serviceplus.pb@gmail.com'
            }
        }
        
        stage('Build') {
            steps {
                echo 'Building the project...'
                // Add build commands here (mvn, npm, dotnet etc.)
            }
        }
        
        stage('Test') {
            steps {
                echo 'Running tests...'
                // Add test commands here
            }
        }
        
        stage('Deploy') {
            steps {
                echo 'Deploying...'
                // Add deployment commands here
            }
        }
    }
}
