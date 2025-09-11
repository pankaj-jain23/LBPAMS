pipeline {
    agent any
    environment {
        IMAGE_NAME = "lbpamsprod"
        KUBE_YAML = "LBPAMS_Kubernetes.yaml"
        SERVER1 = "10.44.237.116"
        SERVER2 = "10.44.237.117"
        SSH_USER = "root"
        SSH_KEY = "~/.ssh/id_ed25519_lbpams" // Path to your Jenkins SSH private key
    }
    
    stages {
        stage('Checkout') {
            steps {
                git branch: 'uptomaster',
                    url: 'https://github.com/pankaj-jain23/LBPAMS.git',
                    credentialsId: 'ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIJsOkNK+PNfxziz/RVUye7nUxVFNkug8IkMIDRJsUZ+u serviceplus.pb@gmail.com'
            }
        }

     stage('Build Docker Image on Server1') {
    steps {
        script {
            sh """
            echo "Building Docker image on ${SERVER1}..."
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} '
                cd /var/lib/jenkins/workspace/LPAMS-API-PIPELINE
                docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} .
                docker save ${IMAGE_NAME}:${BUILD_NUMBER} -o ${IMAGE_NAME}_${BUILD_NUMBER}.tar
            '

            echo "Copying image directly to ${SERVER2}..."
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} "scp /var/lib/jenkins/workspace/LPAMS-API-PIPELINE/${IMAGE_NAME}_${BUILD_NUMBER}.tar ${SSH_USER}@${SERVER2}:/tmp/"

            echo "Loading Docker image on ${SERVER2}..."
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "docker load -i /tmp/${IMAGE_NAME}_${BUILD_NUMBER}.tar"
            """
        }
    }
}
        stage('Approval Required') {
            steps {
                script {
                    emailext(
                        subject: "Approval Needed for Build #${env.BUILD_NUMBER}",
                        body: """Hello Approver,<br><br>
                                Build #${env.BUILD_NUMBER} is waiting for your approval in Jenkins.<br>
                                Please <a href="${env.BUILD_URL}">click here</a> to approve/reject.<br><br>
                                Regards,<br>Jenkins""",
                        to: "lbpams-ap1"
                    )

                    input(
                        message: 'Build approval required by approver',
                        ok: 'Approve',
                        submitter: 'lbpams-ap1'
                    )
                }
            }
        }

        stage('Test') {
            steps {
                echo 'Running tests...'
                // Add your test commands here
            }
        }

        stage('Deploy to Kubernetes') {
            steps {
                script {
                    echo "Updating Kubernetes YAML with current build number..."
                    sh """
                    # Replace placeholder with build number
                    sed 's/{{BUILD_NUMBER}}/${BUILD_NUMBER}/g' ${KUBE_YAML} > temp.yaml

                    # Apply deployment on Server1
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} '
                        kubectl apply -f /var/lib/jenkins/workspace/LPAMS-API-PIPELINE/temp.yaml
                    '

                    # Apply deployment on Server2
                    scp -i ${SSH_KEY} temp.yaml ${SSH_USER}@${SERVER2}:/tmp/
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} '
                        kubectl apply -f /tmp/temp.yaml
                    '
                    """
                }
            }
        }
    }

    post {
        always {
            echo "Cleaning up temporary files..."
            sh """
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} 'rm -f /var/lib/jenkins/workspace/LPAMS-API-PIPELINE/temp.yaml /var/lib/jenkins/workspace/LPAMS-API-PIPELINE/${IMAGE_NAME}_${BUILD_NUMBER}.tar'
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} 'rm -f /tmp/temp.yaml /tmp/${IMAGE_NAME}_${BUILD_NUMBER}.tar'
            """
        }
    }
}
