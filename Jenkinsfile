pipeline {
    agent any
    environment {
        IMAGE_NAME = "lbpamsprod"
        KUBE_YAML = "LBPAMS_Kubernetes.yaml"
        SERVER1 = "10.44.237.116"
        SERVER2 = "10.44.237.117"
        SSH_USER = "root"
        SSH_KEY = "/var/lib/jenkins/.ssh/id_ed25519_lbpams"
        WORKSPACE_DIR = "/var/lib/jenkins/workspace/LPAMS-API-PIPELINE"
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
                    echo "Copying workspace to Server1..."
                    scp -r -i ${SSH_KEY} ${WORKSPACE_DIR}/* ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/

                    echo "Building Docker image on ${SERVER1}..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} '
                        cd ${WORKSPACE_DIR}
                        docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} .
                        docker save ${IMAGE_NAME}:${BUILD_NUMBER} -o ${WORKSPACE_DIR}/${IMAGE_NAME}_${BUILD_NUMBER}.tar
                    '

                    echo "Copying image from Server1 to Jenkins..."
                    scp -i ${SSH_KEY} ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/${IMAGE_NAME}_${BUILD_NUMBER}.tar /tmp/

                    echo "Copying image from Jenkins to Server2..."
                    scp -i ${SSH_KEY} /tmp/${IMAGE_NAME}_${BUILD_NUMBER}.tar ${SSH_USER}@${SERVER2}:/tmp/

                    echo "Loading Docker image on Server2..."
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
                    sh """
                    echo "Updating Kubernetes YAML with current build number..."
                    sed 's/{{BUILD_NUMBER}}/${BUILD_NUMBER}/g' ${WORKSPACE_DIR}/${KUBE_YAML} > /tmp/temp.yaml

                    echo "Deploying on Server1..."
                    scp -i ${SSH_KEY} /tmp/temp.yaml ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} "kubectl apply -f ${WORKSPACE_DIR}/temp.yaml"

                    echo "Deploying on Server2..."
                    scp -i ${SSH_KEY} /tmp/temp.yaml ${SSH_USER}@${SERVER2}:/tmp/
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "kubectl apply -f /tmp/temp.yaml"
                    """
                }
            }
        }
    }

    post {
        always {
            echo "Cleaning up temporary files..."
            sh """
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} "rm -f ${WORKSPACE_DIR}/temp.yaml ${WORKSPACE_DIR}/${IMAGE_NAME}_${BUILD_NUMBER}.tar"
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "rm -f /tmp/temp.yaml /tmp/${IMAGE_NAME}_${BUILD_NUMBER}.tar"
            rm -f /tmp/${IMAGE_NAME}_${BUILD_NUMBER}.tar
            """
        }
    }
}
